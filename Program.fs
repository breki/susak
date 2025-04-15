open System
open susak.Model
open susak.Routes


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

        | Car carTravel ->
            // todo 10: don't include car arrival if there's already an identical point in the trip
            let tripPoint =
                { Point = currentLeg.To
                  Description = "car arrival"
                  Time = currentTime }

            let trip = { Points = tripPoint :: trip.Points }

            let currentTime = currentTime - carTravel.DurationWithRiskFactor

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
    let desiredArrivalTime = DateTime(2025, 04, 18, 16, 0, 0)

    let trips =
        findTrips partinjeSusakRoute desiredArrivalTime
        |> Seq.choose id
        |> Seq.sortBy (fun trip -> trip.Score)
        |> Seq.toList

    printfn $"Found %d{trips.Length} trips"

    trips
    |> List.iteri (fun index trip ->
        printfn ""
        let durationStr = trip.TripDuration.ToString("hh\\:mm")
        printfn $"Trip %d{index + 1} (duration %s{durationStr}):"

        trip.Points
        |> List.iter (fun point ->
            let time = point.Time.ToString("HH:mm")
            printfn $"%s{time}: %A{point.Point} (%s{point.Description})"))

    0 // return an integer exit code
