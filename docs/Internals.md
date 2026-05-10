# Fast Boarding Internals

Purpose: quick memory map for future maintenance. Keep brief, source-code oriented.

Core vanilla problem:
- `m_DepartureFrame` is a planned departure gate, not a hard "leave now" cutoff.
- `StopBoarding` can still return `false` when a passenger has `CurrentVehicle` but lacks `CreatureVehicleFlags.Ready`.
- Result: late solo cims can hold the vehicle after departure time.

FB approach:
- No Harmony.
- No replacement transport AI systems.
- Tune safe vanilla stop data.
- Small ECS assist for late boarding edge cases.

## Quick Guide

- Sliders change `Game.Prefabs.TransportStopData`.
- Only tune `m_LoadingFactor` and `m_BoardingTime`.
- `1x` = vanilla.
- `3x` = default.
- `5x` = max.
- No edits to schedules, line data, vehicle counts, routes, or departure frames.
- `Skip Late Passengers`: after departure, solo not-ready cim can miss that vehicle.
- `Cims Run Sooner`: bus/tram passengers assigned to a boarding vehicle start `HumanFlags.Run` before departure.
- Skipped cims are not deleted; vanilla can continue/repath.
- Groups/families left to vanilla.
- Status UI is a snapshot. `Late today` = solo late skips this in-game day.
- `Stats to Log` = bigger troubleshooting snapshot in `FastBoarding.log`.

---

## Design Goals

- Save-safe runtime ECS tuning.
- Conservative mutation.
- Player-visible options, compact status, detailed log on demand.
- Beta/risky behavior limited to solo passengers.
- Use command-buffer playback for gameplay edits.

## Main Systems

| File | Role | Runs during city simulation? |
| --- | --- | --- |
| `System/TransportStopTuningSystem.cs` | One-shot retune of passenger `TransportStopData` prefab components after speed sliders change. At `1x`, untouched prefabs are skipped; marked prefabs are restored. | Only after load or slider changes. |
| `System/LateBoarderCancelSystem.cs` | Boarding assist pass: late-solo skip after departure, and bus/tram run-sooner before departure. | Yes, only while a behavior toggle is enabled. |
| `System/Status/TransitWaitStatus.cs` | UI-facing status text, today counters, and detailed log report formatting. | No per-frame sim work; called by Options/status buttons. |
| `System/Status/TransitWaitStatusSystem.cs` | On-demand ECS snapshot builder for wait stats, line hints, late groups, and follow-up snapshots. | Only when status/report/verbose follow-up needs data. |
| `Settings/BoardingRuntimeSettings.cs` | Static runtime snapshot of settings plus revision counters. | Shared by Options UI and systems. |

## Decompiled Code Reference Table

| Decompiled file | Important members | Why this mod cares |
| --- | --- | --- |
| `Game.Prefabs/TransportStopData.cs` | `m_LoadingFactor`, `m_BoardingTime`, `m_AccessDistance`, `m_TransportType`, `m_PassengerTransport` | Main no-Harmony tuning target. Faster boarding changes loading and boarding-time values on passenger stop prefabs. |
| `Game.Prefabs/TransportStop.cs` | Authoring values copied into `TransportStopData` during prefab init. | Recompute from authoring values so slider changes do not double-scale. |
| `Game.Simulation/TransportStopSystem.cs` | Reads `TransportStopData.m_LoadingFactor` and station factors into runtime stop values. | Confirms prefab stop data is a safe input knob for boarding speed. |
| `Game.Simulation/TransportCarAISystem.cs` | `StopBoarding`, `PublicTransport.m_DepartureFrame`, `m_MaxBoardingDistance`, `m_MinWaitingDistance`, `CreatureVehicleFlags.Ready` | Road transit hold logic. Main comparison point for bus/tram boarding behavior. |
| `Game.Simulation/ResidentAISystem.cs` | `HumanFlags.Run`, `CurrentVehicle`, `CancelEnterVehicle` | Vanilla starts running at departure time; FB starts bus/tram running earlier. Also confirms detach/path-trim patterns. |
| `Game.Vehicles/PublicTransport.cs` | `m_State`, `m_DepartureFrame`, `m_MaxBoardingDistance`, `m_MinWaitingDistance` | Boarding assist only acts while `Boarding` is set and uses the vanilla departure frame. |
| `Game.Vehicles/PublicTransportFlags.cs` | `Boarding`, `Evacuating`, `PrisonerTransport`, `Refueling` | Skip special gameplay states. |
| `Game.Vehicles/Passenger.cs` | `Passenger.m_Passenger` vehicle buffer element | Vehicle passenger buffer must stay consistent with `CurrentVehicle`. |
| `Game.Vehicles/LayoutElement.cs` | `m_Vehicle` child vehicle/car | Needed for multi-car transit such as trams. |
| `Game.Creatures/CurrentVehicle.cs` | `m_Vehicle`, `m_Flags` | Not-ready passenger attachment source. Absence of component means no current vehicle in vanilla. |
| `Game.Creatures/CreatureVehicleFlags.cs` | `Ready`, `Leader`, `Driver`, `Entering`, `Exiting` | Skip only passengers whose `Ready` flag is not set. |
| `Game.Creatures/ResidentFlags.cs` | `InVehicle`, `WaitingTransport` | Clear `InVehicle` when detaching a missed passenger. |
| `Game.Creatures/HumanFlags.cs` | `Run`, `Emergency` | Run-sooner sets `Run`; skip-late clears run/emergency from the missed attempt. |
| `Game.Pathfind/PathOwner.cs` | `m_ElementIndex`, `m_State` | After path trim, reset `m_ElementIndex` to continue from new first leg. |
| `Game.Pathfind/PathElement.cs` | `m_Target` | Locate current vehicle in remaining path; keep only later legs. |
| `Game.Routes/WaitingPassengers.cs` | `m_Count`, `m_AverageWaitingTime` | Status rows and reports use vanilla waiting counters. |
| `Game.Routes/Connected.cs` | `m_Connected` | Route waypoint back to served stop entity. |
| `Game.Common/Owner.cs` | `m_Owner` | Route waypoint owner points to transit line entity. |
| `Game.Prefabs/TransportLineData.cs` | `m_TransportType`, `m_PassengerTransport`, `m_DefaultUnbunchingFactor`, `m_StopDuration` | Used for status grouping. Unbunching intentionally not changed. |
| `Game.Prefabs/PublicTransportVehicleData.cs` | `m_TransportType`, `m_PassengerCapacity`, `m_MaintenanceRange` | Classify vehicles for counters. Capacity intentionally left to other mods. |
| `Game.Creatures/GroupMember.cs` | `m_Leader` | Group members are reported but not canceled. |
| `Game.Creatures/GroupCreature.cs` | `m_Creature` buffer on group leader | Group leaders are avoided. |
| `Game.Creatures/ReferencesSystem.cs` | Adds/removes vehicle `Passenger` buffer entries based on `CurrentVehicle` | Safety rule: `Passenger` buffers and `CurrentVehicle` must stay consistent. |

## Boarding Speed Formula

Player-facing slider:

```text
1x = vanilla
2x = ~1/2 planned dwell
3x = default (~1/3 planned dwell)
5x = max (~1/5 planned dwell)
```

Implementation:

```text
baseEffectiveLoading = max(0, 1 + authoringStop.m_LoadingFactor)
tunedStop.m_LoadingFactor = baseEffectiveLoading * speedMultiplier - 1
tunedStop.m_BoardingTime = authoringStop.m_BoardingTime / speedMultiplier
```

Important:
- Always start from authoring `TransportStop` prefab values.
- Avoid cumulative scaling when sliders move repeatedly.
- At `1x`, no-op for untouched prefabs; restore and unmark previously tuned prefabs.

## Cims Run Sooner: Bus + Tram

Goal:
- reduce late bus/tram misses before skip-late has to intervene.

Rules:
- Only bus/tram.
- Only assigned passenger already in vehicle passenger buffer.
- Only while target vehicle is `Boarding`.
- Only before latest departure frame.
- Window: `RunSoonerLeadFrames = 512`.
- Skip evacuation, prisoner transport, refueling, loading resources.
- Requires `CurrentVehicle.m_Vehicle == vehicle`.
- Requires not `Ready`.
- Requires `Human`, not deleted/destroyed/temp/overridden.
- If already running, do nothing.

Mutation:
- Set `HumanFlags.Run`.
- No teleport.
- No force boarding.
- No path edit.
- No departure-frame edit.

Proof:
- Status row counts bus/tram run-sooner assists today.
- Verbose sampled follow-up logs: `made same vehicle`, `different vehicle`, `has path`, `no path yet`.

## Skip Late Passengers

Conservative solo-only pass.

Requirements:
- Vehicle has `PublicTransportFlags.Boarding`.
- Current frame is past latest departure frame.
- Passenger has `CurrentVehicle` pointing at that vehicle.
- Passenger is not `Ready`.
- Passenger is resident human with `PathOwner` and `PathElement` buffer.
- Passenger is not `GroupMember`.
- Passenger does not own `GroupCreature` buffer.
- Remaining path contains the exact vehicle entity.

Mutation through ECB:
- Remove `CurrentVehicle`.
- Clear `ResidentFlags.InVehicle`.
- Clear `HumanFlags.Run | HumanFlags.Emergency`.
- Replace passenger path with legs after missed vehicle.
- Reset `PathOwner.m_ElementIndex = 0`.
- Replace vehicle `Passenger` buffer without canceled passengers.

Index safety:
- Search starts at `max(0, pathOwner.m_ElementIndex)`.
- `vehiclePathIndex = -1` sentinel only.
- Abort if vehicle not found.
- Copy starts at `vehiclePathIndex + 1`.
- No `Clear()` after `SetBuffer`; ECB `SetBuffer` replaces existing buffer at playback.
- No blind `PathFlags.Obsolete`; vanilla trim path flow does not set it when vehicle leg is found.

## Status Report Notes

- Options status is cached per UI frame and throttled.
- `stop entity` = actual stop/terminal after grouping waypoints through `Connected`.
- `waypoint entity` = route waypoint with `WaitingPassengers`.
- `line entity` = `Owner.m_Owner`.
- Generic debug line names hidden when not useful.
- `Late today` = current in-game day counter.
- `Cims run earlier` = current in-game day counter.

## Intentional Non-Features

- No Harmony transit system replacements.
- No unbunching changes; line data can be save-persistent and would need restore tooling.
- No group/family cancellation yet.
- No direct edits to `m_AccessDistance`.
- No forced boarding.
- No run-sooner for train/subway/ship/ferry/airplane yet.
