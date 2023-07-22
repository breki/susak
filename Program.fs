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
        [
          { Date = DateTime(2023, 07, 25)
            DepartsOn = TimeSpan(23, 59, 0)
            ArrivesOn = TimeSpan(00, 24, 0) }
          { Date = DateTime(2023, 07, 26)
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
          ]
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

type Trip with

    member this.TripDuration() =
        let firstPoint = this.Points.[0]
        let lastPoint = this.Points.[this.Points.Length - 1]
        lastPoint.Time - firstPoint.Time

let rec continueTrip
    (routeLegsReversed: TripLeg list)
    legIndex
    trip
    currentTime
    : Trip option seq =
    if legIndex >= routeLegsReversed.Length then
        Some trip |> Seq.singleton
    else
        let currentLeg = routeLegsReversed[legIndex]

        match currentLeg.Type with
        | Ship shipTravel ->
            coverShipTravel
                routeLegsReversed
                legIndex
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

and coverShipVoyage
    (routeLegsReversed: TripLeg list)
    legIndex
    shipTravel
    voyage
    trip
    =
    let currentLeg = routeLegsReversed.[legIndex]

    let currentTime = voyage.Date.Date + voyage.ArrivesOn

    let trip =
        { Points =
            { Point = currentLeg.To
              Description = "ship arrival"
              Time = currentTime }
            :: trip.Points }

    let currentTime = voyage.Date.Date + voyage.DepartsOn

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


and coverShipTravel
    routeLegsReversed
    legIndex
    shipTravel
    (trip: Trip)
    currentTime
    : Trip option seq =
    let viableVoyages =
        shipTravel.Timetable
        |> List.filter (fun voyage ->
            let voyageArrivalTime = voyage.Date.Date + voyage.ArrivesOn
            voyageArrivalTime <= currentTime)

    match viableVoyages with
    | [] -> None |> Seq.singleton
    | viableVoyages ->
        viableVoyages
        |> Seq.map (fun voyage ->
            coverShipVoyage routeLegsReversed legIndex shipTravel voyage trip)
        |> Seq.concat


let findTrips (route: TripRoute) desiredArrivalTime : Trip option seq =
    let routeLegsReversed = route.Legs |> List.rev
    let currentLeg = routeLegsReversed.[0]

    match currentLeg.Type with
    | Ship shipTravel ->
        coverShipTravel
            routeLegsReversed
            0
            shipTravel
            { Points = [] }
            desiredArrivalTime
    | _ -> raise (InvalidOperationException("Only ship legs are allowed here"))

[<EntryPoint>]
let main _ =
    let desiredArrivalTime = DateTime(2023, 07, 26, 18, 0, 0)

    let trips =
        findTrips mariborSusakRoute desiredArrivalTime
        |> Seq.choose id
        |> Seq.toList

    printfn $"Found %d{trips.Length} trips"

    trips
    |> List.iteri (fun index trip ->
        printfn ""
        let durationStr = trip.TripDuration().ToString("hh\\:mm")
        printfn $"Trip %d{index + 1} (duration %s{durationStr}):"

        trip.Points
        |> List.iter (fun point ->
            let time = point.Time.ToString("dd HH:mm")
            printfn $"%s{time}: %A{point.Point} (%s{point.Description})"))

    0 // return an integer exit code

// todo 5: add duration risk factor for car travel