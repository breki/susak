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

type ShipTravel = { Timetable: ShipVoyage list }

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

// type TimeOnly = {
//     SecondsOfDay: int
// }
//     with
//     member this.ToString =
//         // format to hours and minutes
//         let hours = this.SecondsOfDay / 3600
//         let minutes = (this.SecondsOfDay - (hours * 3600)) / 60
//         $"%02d{hours}:%02d{minutes}"
//
//     static member FromHoursMinutes (hours: int) (minutes: int) =
//         { SecondsOfDay = (hours * 3600) + (minutes * 60) }




let maliLosinjToSusakTimetable =
    { Timetable =
        [ { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(5, 30, 0)
            ArrivesOn = TimeSpan(7, 55, 0) }
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(13, 30, 0)
            ArrivesOn = TimeSpan(14, 25, 0) } ] }

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
            ArrivesOn = TimeSpan(00, 24, 0) } ] }

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
          { From = MaliLosinj
            To = MaliLosinj
            Type = Pause { Duration = TimeSpan(1, 0, 0) } }
          { From = MaliLosinj
            To = Susak
            Type = Ship maliLosinjToSusakTimetable } ] }

type TripPoint = { Point: TripNode; Description: string; Time: DateTime }

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
        | Ship { Timetable = timetable } ->
            let viableVoyages =
                timetable
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

                continueTrip routeLegsReversed (legIndex + 1) trip currentTime

        | Car { Duration = duration } ->
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

            let tripPoint =
                { Point = currentLeg.To
                  Description = "after pause" 
                  Time = currentTime }

            let trip = { Points = tripPoint :: trip.Points }

            continueTrip routeLegsReversed (legIndex + 1) trip currentTime

let findFirstFeasibleTrip (route: TripRoute) voyageIndex =
    let routeLegsReversed = route.Legs |> List.rev
    let currentLeg = routeLegsReversed.[0]

    match currentLeg.Type with
    | Ship { Timetable = timetable } ->
        let voyage = timetable.[voyageIndex]
        let currentTime = voyage.Date.Date + voyage.ArrivesOn

        let tripPoint2 =
            { Point = currentLeg.To
              Description = "ship arrival"          
              Time = currentTime }

        let currentTime = currentTime - voyage.Duration

        let tripPoint1 =
            { Point = currentLeg.From
              Description = "ship departure"          
              Time = currentTime }

        let trip = { Points = [ tripPoint1; tripPoint2 ] }


        let trip = continueTrip routeLegsReversed 1 trip currentTime

        trip
    | _ -> raise (InvalidOperationException("Only ship legs are allowed here"))


let findTrips (route: TripRoute) : Trip list =
    let legs = mariborSusakRoute.Legs |> List.rev

    let firstLeg = legs |> List.head

    match firstLeg.Type with
    | Ship { Timetable = timetable } ->
        timetable
        |> List.mapi (fun voyageIndex voyage ->
            findFirstFeasibleTrip route voyageIndex)
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
            printfn $"%A{point.Point} (%s{point.Description}) at %A{point.Time}"))

    0 // return an integer exit code
