module susak.Timetables

open System
open susak.Model

let maliLosinjToSusakTimetableSummer2023 =
    { Timetable =
        [
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(5, 30, 0)
            ArrivesOn = TimeSpan(7, 55, 0) }
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(6, 0, 0)
            ArrivesOn = TimeSpan(6, 35, 0) }
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(13, 30, 0)
            ArrivesOn = TimeSpan(14, 25, 0) }
          { Date = DateTime(2023, 07, 26)
            DepartsOn = TimeSpan(18, 25, 0)
            ArrivesOn = TimeSpan(18, 55, 0) }
          ]
      MinTimeBeforeDeparture = TimeSpan(1, 0, 0) }

let valbiskaMeragTimetableSummer2023 =
    { Timetable =
        [
          { Date = DateTime(2023, 07, 25)
            DepartsOn = TimeSpan(23, 59, 0)
            ArrivesOn = TimeSpan(00, 24, 0) }
          { Date = DateTime(2023, 07, 25)
            DepartsOn = TimeSpan(22, 30, 0)
            ArrivesOn = TimeSpan(22, 55, 0) }
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

let valbiskaMeragTimetableWinter2024 =
    { Timetable =
        [
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(10, 45, 00)
            ArrivesOn = TimeSpan(11, 10, 00) }
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(12, 15, 00)
            ArrivesOn = TimeSpan(12, 40, 00) }
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(13, 45, 00)
            ArrivesOn = TimeSpan(14, 10, 00) }
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(15, 15, 00)
            ArrivesOn = TimeSpan(15, 40, 00) }
          ]
      MinTimeBeforeDeparture = TimeSpan(0, 30, 0) }

// This is used for Mali Lošinj as a final destination since currently there
// has to be a ship leg at the and of the route. 
let maliLosinjDestinationFakeShipTimetableWinter2024 =
    { Timetable =
        [
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(13, 0, 0)
            ArrivesOn = TimeSpan(13, 0, 0) }
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(13, 30, 0)
            ArrivesOn = TimeSpan(13, 30, 0) }
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(14, 0, 0)
            ArrivesOn = TimeSpan(14, 0, 0) }
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(14, 30, 0)
            ArrivesOn = TimeSpan(14, 30, 0) }
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(15, 0, 0)
            ArrivesOn = TimeSpan(15, 0, 0) }
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(15, 30, 0)
            ArrivesOn = TimeSpan(15, 30, 0) }
          { Date = DateTime(2024, 02, 04)
            DepartsOn = TimeSpan(16, 0, 0)
            ArrivesOn = TimeSpan(16, 0, 0) }
          ]
      MinTimeBeforeDeparture = TimeSpan(0, 0, 0) }
