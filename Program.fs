open System

type TripNode =
    | Maribor
    | Lucko
    | Valbiska
    | Merag
    | MaliLosinj
    | Susak

type CarTravel = { Duration: TimeSpan }

type ShipVoyage =
    { Date: DateTime
      DepartsOn: TimeSpan
      ArrivesOn: TimeSpan }

    member this.Duration = this.ArrivesOn - this.DepartsOn

type ShipTravel =
    { Timetable: ShipVoyage list
      MinTimeBeforeDeparture: TimeSpan }

type Pause = { Duration: TimeSpan }

type TripLegType =
    | Car of CarTravel
    | Ship of ShipTravel
    | Pause of Pause


type TripLeg =
    { From: TripNode
      To: TripNode
      Type: TripLegType }

type TripRoute = { Legs: TripLeg list }


let maliLosinjToSusakTimetable =
    { Timetable =
        [ { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(5, 30, 0)
            ArrivesOn = TimeSpan(7, 55, 0) }
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(13, 30, 0)
            ArrivesOn = TimeSpan(14, 25, 0) } ]
      MinTimeBeforeDeparture = TimeSpan(1, 0, 0) }

let valbiskaMeragTimetable =
    { Timetable =
        [ { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(5, 45, 0)
            ArrivesOn = TimeSpan(6, 10, 0) }
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(7, 15, 0)
            ArrivesOn = TimeSpan(7, 40, 0) }
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(9, 15, 0)
            ArrivesOn = TimeSpan(9, 40, 0) }
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(10, 45, 0)
            ArrivesOn = TimeSpan(11, 10, 0) }
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(12, 15, 0)
            ArrivesOn = TimeSpan(12, 40, 0) }
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(23, 59, 0)
            ArrivesOn = TimeSpan(00, 24, 0) } ]
      MinTimeBeforeDeparture = TimeSpan(1, 0, 0) }

let mariborSusakRoute =
    { Legs =
        [ { From = Maribor
            To = Lucko
            Type = Car { Duration = TimeSpan(1, 25, 0) } }
          { From = Lucko
            To = Valbiska
            Type = Car { Duration = TimeSpan(2, 10, 0) } }
          { From = Valbiska
            To = Merag
            Type = Ship valbiskaMeragTimetable }
          { From = Merag
            To = MaliLosinj
            Type = Car { Duration = TimeSpan(1, 5, 0) } }
          // { From = MaliLosinj
          //   To = MaliLosinj
          //   Type = Pause { Duration = TimeSpan(1, 0, 0) } }
          { From = MaliLosinj
            To = Susak
            Type = Ship maliLosinjToSusakTimetable } ] }

type TripPoint =
    { Point: TripNode
      Description: string
      Time: DateTime }

type Trip = { Points: TripPoint list }

let rec continueTrip
    (routeLegsReversed: TripLeg list)
    legIndex
    trip
    currentTime
    =
    if legIndex >= routeLegsReversed.Length then
        trip
    else
        let currentLeg = routeLegsReversed[legIndex]

        match currentLeg.Type with
        | Ship shipTravel ->
            coverShipTravel
                routeLegsReversed
                legIndex
                currentLeg
                shipTravel
                trip
                currentTime

        | Car { Duration = duration } ->
            // todo 10: don't include car arrival if there's already an identical point in the trip
            let tripPoint =
                { Point = currentLeg.To
                  Description = "car arrival"
                  Time = currentTime }

            let trip = { Points = tripPoint :: trip.Points }

            let currentTime = currentTime - duration

            let tripPoint =
                { Point = currentLeg.From
                  Description = "car departure"
                  Time = currentTime }

            let trip = { Points = tripPoint :: trip.Points }

            continueTrip routeLegsReversed (legIndex + 1) trip currentTime
        | Pause { Duration = duration } ->
            let currentTime = currentTime - duration
            continueTrip routeLegsReversed (legIndex + 1) trip currentTime


and coverShipTravel
    routeLegsReversed
    legIndex
    currentLeg
    shipTravel
    trip
    currentTime
    =
    let viableVoyages =
        shipTravel.Timetable
        |> List.filter (fun voyage ->
            let voyageArrivalTime = voyage.Date.Date + voyage.ArrivesOn
            voyageArrivalTime <= currentTime)

    match viableVoyages with
    | [] -> trip
    | viableVoyages ->
        let firstViableVoyage = viableVoyages |> List.head

        let currentTime =
            firstViableVoyage.Date.Date + firstViableVoyage.ArrivesOn

        let trip =
            { Points =
                { Point = currentLeg.To
                  Description = "ship arrival"
                  Time = currentTime }
                :: trip.Points }

        let currentTime =
            firstViableVoyage.Date.Date + firstViableVoyage.DepartsOn

        let trip =
            { Points =
                { Point = currentLeg.From
                  Description = "ship departure"
                  Time = currentTime }
                :: trip.Points }

        let currentTime = currentTime - shipTravel.MinTimeBeforeDeparture

        let tripPoint =
            { Point = currentLeg.From
              Description = "car arrival at the port"
              Time = currentTime }

        let trip = { Points = tripPoint :: trip.Points }

        continueTrip routeLegsReversed (legIndex + 1) trip currentTime


let findFirstFeasibleTrip (route: TripRoute) desiredArrivalTime =
    let routeLegsReversed = route.Legs |> List.rev
    let currentLeg = routeLegsReversed.[0]

    match currentLeg.Type with
    | Ship shipTravel ->
        coverShipTravel
            routeLegsReversed
            0
            currentLeg
            shipTravel
            { Points = [] }
            desiredArrivalTime
    | _ -> raise (InvalidOperationException("Only ship legs are allowed here"))


let findTrips (route: TripRoute) : Trip list =
    let legs = mariborSusakRoute.Legs |> List.rev

    let firstLeg = legs |> List.head

    match firstLeg.Type with
    | Ship { Timetable = timetable } ->
        timetable
        |> List.mapi (fun voyageIndex voyage ->
            findFirstFeasibleTrip route (DateTime(2023, 07, 26, 17, 0, 0)))
    | _ -> raise (NotImplementedException())

[<EntryPoint>]
let main argv =
    let trips = findTrips mariborSusakRoute
    printfn $"Found %d{trips.Length} trips"

    trips
    |> List.iteri (fun index trip ->
        printfn ""
        printfn $"Trip %d{index + 1}:"

        trip.Points
        |> List.iter (fun point ->
            let time = point.Time.ToString("HH:mm")
            printfn $"%s{time}: %A{point.Point} (%s{point.Description})"))

    0 // return an integer exit code



// todo 7: transform the search function(s) to return a sequence of trips