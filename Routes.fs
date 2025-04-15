module susak.Routes

open System
open susak.Model
open susak.Timetables

let mariborSusakRoute =
    { Legs =
        [ { From = Maribor
            To = Lucko
            Type = Car { Duration = TimeSpan(1, 25, 0); DurationRiskFactor = 0.25 } }
          { From = Lucko
            To = Valbiska
            Type = Car { Duration = TimeSpan(2, 10, 0); DurationRiskFactor = 0.25 } }
          { From = Valbiska
            To = Merag
            Type = Ship valbiskaMeragTimetableWinter2024 }
          { From = Merag
            To = MaliLosinj
            Type = Car { Duration = TimeSpan(1, 5, 0); DurationRiskFactor = 0.10 } }
          // { From = MaliLosinj
          //   To = MaliLosinj
          //   Type = Pause { Duration = TimeSpan(1, 0, 0) } }
          { From = MaliLosinj
            To = Susak
            Type = Ship maliLosinjToSusakTimetableSpring2025 } ] }


let mariborMaliLošinjRoute =
    { Legs =
        [ { From = Maribor
            To = Lucko
            Type = Car { Duration = TimeSpan(1, 25, 0); DurationRiskFactor = 0.25 } }
          { From = Lucko
            To = Valbiska
            Type = Car { Duration = TimeSpan(2, 10, 0); DurationRiskFactor = 0.25 } }
          { From = Valbiska
            To = Merag
            Type = Ship valbiskaMeragTimetableWinter2024 }
          { From = Merag
            To = MaliLosinj
            Type = Car { Duration = TimeSpan(1, 5, 0); DurationRiskFactor = 0.10 } }
          { From = MaliLosinj
            To = MaliLosinj
            Type = Ship maliLosinjDestinationFakeShipTimetableWinter2024 } ] }

let partinjeMaliLošinjRoute =
    { Legs =
        [ { From = Partinje
            To = Lucko
            Type = Car { Duration = TimeSpan(1, 30, 0); DurationRiskFactor = 0.25 } }
          { From = Lucko
            To = Valbiska
            Type = Car { Duration = TimeSpan(2, 10, 0); DurationRiskFactor = 0.25 } }
          { From = Valbiska
            To = Merag
            Type = Ship valbiskaMeragTimetableWinter2024 }
          { From = Merag
            To = MaliLosinj
            Type = Car { Duration = TimeSpan(1, 5, 0); DurationRiskFactor = 0.10 } }
          { From = MaliLosinj
            To = MaliLosinj
            Type = Ship maliLosinjDestinationFakeShipTimetableWinter2024 } ] }


let partinjeSusakRoute =
    { Legs =
        [ { From = Partinje
            To = Lucko
            Type = Car { Duration = TimeSpan(1, 30, 0); DurationRiskFactor = 0.25 } }
          { From = Lucko
            To = Valbiska
            Type = Car { Duration = TimeSpan(2, 10, 0); DurationRiskFactor = 0.25 } }
          { From = Valbiska
            To = Merag
            Type = Ship valbiskaMeragTimetableWinter2024 }
          { From = Merag
            To = MaliLosinj
            Type = Car { Duration = TimeSpan(1, 5, 0); DurationRiskFactor = 0.10 } }
          { From = MaliLosinj
            To = Susak
            Type = Ship maliLosinjToSusakTimetableSpring2025 } ] }

let mariborSusakRouteViaLjubljana =
    { Legs =
        [ { From = Maribor
            To = Podutik
            Type = Car { Duration = TimeSpan(1, 25, 0); DurationRiskFactor = 0.35 } }
          { From = Podutik
            To = Podutik
            Type = Pause { Duration = TimeSpan(0, 30, 0) } }
          { From = Podutik
            To = Postojna
            Type = Car { Duration = TimeSpan(0, 35, 0); DurationRiskFactor = 0.25 } }
          { From = Postojna
            To = Jelšane
            Type = Car { Duration = TimeSpan(0, 45, 0); DurationRiskFactor = 0.25 } }
          { From = Jelšane
            To = Valbiska
            Type = Car { Duration = TimeSpan(1, 00, 0); DurationRiskFactor = 0.25 } }
          { From = Valbiska
            To = Merag
            Type = Ship valbiskaMeragTimetableSpring2025 }
          { From = Merag
            To = MaliLosinj
            Type = Car { Duration = TimeSpan(1, 5, 0); DurationRiskFactor = 0.10 } }
          { From = MaliLosinj
            To = Susak
            Type = Ship maliLosinjToSusakTimetableSpring2025 } ] }
