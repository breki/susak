# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

F# (.NET 7) console application that plans a multi-leg trip (car + ferry) from a starting point in Slovenia to the island of Susak in Croatia. It searches ferry timetables to enumerate feasible trips that arrive by a given `desiredArrivalTime` and ranks them by total travel duration.

## Commands

- Build: `dotnet build susak.sln`
- Run: `dotnet run --project susak.fsproj`
- Restore: `dotnet restore`

There are no tests in this repo.

## Architecture

F# compile order matters and is declared in `susak.fsproj`:

1. `Model.fs` — domain types: `TripNode` (locations), `TripLegType` (`Car` / `Ship` / `Pause`), `ShipVoyage`, `ShipTravel` (timetable + `MinTimeBeforeDeparture` buffer), `TripRoute` (list of `TripLeg`s), `Trip` (list of `TripPoint`s) with `TripDuration` / `Score` members.
2. `Timetables.fs` — hand-maintained ferry timetables (e.g. `valbiskaMeragTimetableSpring2025`, `maliLosinjToSusakTimetableSpring2025`). Updated per trip/season.
3. `Routes.fs` — concrete `TripRoute` values composing car legs, ferry legs, and optional pauses (e.g. `mariborSusakRoute`, `mariborSusakRouteViaLjubljana`).
4. `Program.fs` — the search. `findTrips` walks the legs **in reverse** from the desired arrival time: for each ship leg it branches over every voyage that arrives in time (`coverShipTravel` → `coverShipVoyage`), and for car/pause legs it subtracts the leg duration. Car durations are inflated by `DurationRiskFactor` via `CarTravel.DurationWithRiskFactor`. The result is a `seq<Trip option>`; `None` means no viable voyage at some step. `main` filters to `Some`, sorts by `Score` (total minutes), and prints each trip's timestamped points.

When adding a new trip:
- Add new timetable values to `Timetables.fs`.
- Add a new route in `Routes.fs`, referencing those timetables.
- Point `main` in `Program.fs` at the new route and update `desiredArrivalTime`.
- A route's final leg must be a `Ship` — `findTrips` throws otherwise.

New `TripNode` cases go in `Model.fs`.
