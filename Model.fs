namespace susak.Model

open System

type TripNode =
    | Maribor
    | Partinje
    | Podutik
    | Postojna
    | Jelšane
    | Lucko
    | Valbiska
    | Merag
    | MaliLosinj
    | Susak

type CarTravel = { Duration: TimeSpan; DurationRiskFactor: float }

type CarTravel with
    member this.DurationWithRiskFactor =
        this.Duration * (1.0 + this.DurationRiskFactor)

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


type TripPoint =
    { Point: TripNode
      Description: string
      Time: DateTime }

type Trip = { Points: TripPoint list }

type Trip with
    member this.TripDuration =
        let firstPoint = this.Points.[0]
        let lastPoint = this.Points.[this.Points.Length - 1]
        lastPoint.Time - firstPoint.Time
        
    // score function calculated by the total travel minutes
    member this.Score =
        let duration = this.TripDuration
        duration.TotalMinutes
