# Fast Boarding (FB) Internals

Problem: `m_DepartureFrame` is used as a gate,
but `StopBoarding` still returns `false` if any passenger in the vehicle buffer has `CurrentVehicle` without the `Ready` flag.
Returning false means “do not stop boarding yet, someone is still on the way.”
So vanilla has a planned departure frame, but it is not a hard “leave now even if someone is late” cutoff which is where this mod helps.

Game Design hole: in transportAI code, the departure frame itself does not forcibly clear that late passenger or make the vehicle leave.
That is the design hole Fast Boarding is working around.

This mod map records the important assumptions from decompiled game code so I don't have to reread them later and tracks the design goals.
- also for anyone who wants to understand the mod's inner workings.

## Quick Guide

- FB sliders change passenger public transport stop prefab data, not whole transport AI systems.
- FB changes the game's `Game.Prefabs.TransportStopData`.
- FB only tunes `m_LoadingFactor` and `m_BoardingTime`.
- FB does not change `PublicTransport.m_DepartureFrame`, line schedules, vehicle AI systems, vehicle counts, or route counts.
- At `1x` with `Let vehicles leave without late cims` OFF, behavior should be vanilla.
- At `2x-10x`, FB reduces boarding/loading time by tuning stop data that vanilla already reads.
- If `Let vehicles leave without late cims` is ON, FB also checks vehicles already past `PublicTransport.m_DepartureFrame`.
- If a solo cim is still not ready, FB detaches that cim from `CurrentVehicle` so the vehicle can leave.
- The cim is not deleted. They miss the vehicle and vanilla can continue/repath them.
- Groups/families are not skipped yet; a late group can still hold up a vehicle like vanilla.

The Status section in Options UI is a snapshot. `Skipped` is the number of solo late passengers skipped on the current in-game day. `Stats to Log` writes a larger troubleshooting snapshot to `FastBoarding.log`.

---


## Design Goals

- Avoid Harmony and avoid replacing vanilla transport AI systems.
- Keep normal behavior save-safe: prefab tuning is recomputed from authoring values and stored only as runtime ECS component changes.
- Keep the risky behavior opt-in: `Let vehicles leave without late cims` is beta and only acts on solo passengers. Groups with a leader are avoided for now because of unknowns.
- Use ECS queries and command-buffer playback patterns where we mutate gameplay state.

## Main Systems

| File | Role | Runs during city simulation? |
| --- | --- | --- |
| `System/TransportStopTuningSystem.cs` | One-shot retune of passenger `TransportStopData` prefab components after speed sliders change. At `1x`, untouched prefabs are skipped; only previously marked FB prefabs are restored. | Only briefly after load or slider changes. |
| `System/LateBoarderCancelSystem.cs` | Optional beta pass that lets solo not-ready passengers miss a vehicle after vanilla departure time. | Yes, only while toggle is enabled. |
| `System/Status/TransitWaitStatus.cs` | Cached UI-facing text and detailed log report formatting. | No per-frame simulation work; called by Options UI/status buttons. |
| `System/Status/TransitWaitStatusSystem.cs` | On-demand ECS snapshot builder for wait stats and late group diagnostics. | Only when Options UI requests status/report. |
| `Settings/BoardingRuntimeSettings.cs` | Static runtime snapshot of settings plus revision counters. | Shared by Options UI and systems. |

## Decompiled Code Reference Table

| Decompiled file | Important members | Why this mod cares |
| --- | --- | --- |
| `Game.Prefabs/TransportStopData.cs` | `m_LoadingFactor`, `m_BoardingTime`, `m_AccessDistance`, `m_TransportType`, `m_PassengerTransport` | Main no-Harmony tuning target. Faster boarding changes `m_LoadingFactor` and `m_BoardingTime` on stop prefabs. |
| `Game.Prefabs/TransportStop.cs` | Authoring values copied into `TransportStopData` during prefab init. | Recompute from authoring values so slider changes do not double-scale. |
| `Game.Simulation/TransportStopSystem.cs` | Reads `TransportStopData.m_LoadingFactor` and station factors into runtime stop values. | Confirms prefab stop data is a safe input knob for boarding speed. |
| `Game.Vehicles/PublicTransport.cs` | `m_State`, `m_DepartureFrame`, `m_MaxBoardingDistance`, `m_MinWaitingDistance` | Late-boarder pass only acts while `Boarding` is set and current frame is past `m_DepartureFrame`. |
| `Game.Vehicles/PublicTransportFlags.cs` | `Boarding`, `Evacuating`, `PrisonerTransport` | Skip evacuation/prisoner transport to avoid special-case gameplay. |
| `Game.Vehicles/Passenger.cs` | `Passenger.m_Passenger` buffer element on vehicles. | Vehicle passenger buffer must be trimmed when a cim misses the vehicle. |
| `Game.Creatures/CurrentVehicle.cs` | `m_Vehicle`, `m_Flags` | A not-ready passenger is attached to the vehicle through this component. |
| `Game.Creatures/CreatureVehicleFlags.cs` | `Ready`, `Leader`, `Driver`, `Entering`, `Exiting` | Only cancel passengers whose `Ready` flag is not set. |
| `Game.Creatures/ResidentFlags.cs` | `InVehicle`, `WaitingTransport` | Clear `InVehicle` when detaching a missed passenger. |
| `Game.Creatures/HumanFlags.cs` | `Run`, `Emergency` | Clear urgent movement flags left over from the missed boarding attempt. |
| `Game.Pathfind/PathOwner.cs` | `m_ElementIndex`, `m_State` | After removing the missed vehicle path leg, then reset `m_ElementIndex` to continue from the new first leg. |
| `Game.Pathfind/PathElement.cs` | `m_Target` | Locate the current vehicle in the path and keep only later path legs. |
| `Game.Routes/WaitingPassengers.cs` | `m_Count`, `m_AverageWaitingTime` | Status rows and reports are built from vanilla waiting-passenger counters. |
| `Game.Routes/Connected.cs` | `m_Connected` | Waiting data is often on route waypoints; this links the waypoint back to the served stop entity. |
| `Game.Common/Owner.cs` | `m_Owner` | Route waypoint owner points to the transit line entity. |
| `Game.Prefabs/TransportLineData.cs` | `m_TransportType`, `m_PassengerTransport`, `m_DefaultUnbunchingFactor`, `m_StopDuration` | Used for status grouping. Unbunching is intentionally not changed because line changes are save data. |
| `Game.Prefabs/PublicTransportVehicleData.cs` | `m_TransportType`, `m_PassengerCapacity`, `m_MaintenanceRange` | Used to classify vehicles for late-boarder counters. Capacity is intentionally left to Public Works Plus. |
| `Game.Creatures/GroupMember.cs` | `m_Leader` | Group passengers are reported but not canceled in the beta pass. |
| `Game.Creatures/GroupCreature.cs` | `m_Creature` buffer on group leader | Presence marks a group leader; avoid canceling group leaders/members. |
| `Game.Creatures/ReferencesSystem.cs` | Adds/removes `Passenger` buffer entries based on `CurrentVehicle`. | Supports our safety rule: vehicle passenger buffers and `CurrentVehicle` must stay consistent. |

## Boarding Speed Formula

FB uses a simple player-facing sliders:

```text
1x = vanilla
2x = roughly twice as fast
10x = aggressive test value
```

Implementation:

```text
baseEffectiveLoading = max(0, 1 + authoringStop.m_LoadingFactor)
tunedStop.m_LoadingFactor = baseEffectiveLoading * speedMultiplier - 1
tunedStop.m_BoardingTime = authoringStop.m_BoardingTime / speedMultiplier
```

The important detail is that the calculation always starts from the authoring `TransportStop` prefab values, not from the last tuned `TransportStopData`.
- avoids cumulative scaling when players move sliders repeatedly.

At `1x`, FB uses a strict no-op rule: if a stop prefab has no `TransportStopTuningMarker`, the system does not write `TransportStopData`. If the marker exists, FB restores vanilla authoring values and removes the marker.

## Let Vehicles Leave Without Late Cims

The beta pass is deliberately conservative.

It only considers a passenger if:

- Vehicle has `PublicTransportFlags.Boarding`.
- Current simulation frame is at or past `PublicTransport.m_DepartureFrame`.
- Passenger has `CurrentVehicle` pointing at that vehicle.
- Passenger does not have `CreatureVehicleFlags.Ready`.
- Passenger is a resident human with a path buffer.
- Passenger is not a `GroupMember` and does not own a `GroupCreature` buffer.
- Passenger path contains the current vehicle as a future/current `PathElement.m_Target`.

When canceling, it queues ECS changes in an `EntityCommandBuffer`:

- Remove `CurrentVehicle` from the passenger.
- Clear `ResidentFlags.InVehicle`.
- Clear `HumanFlags.Run` and `HumanFlags.Emergency`.
- Replace the path buffer with only the legs after the missed vehicle.
- Reset `PathOwner.m_ElementIndex` to `0`.
- Replace the vehicle `Passenger` buffer without the canceled passenger.

The pass first scans and collects candidates, then plays back mutations through ECB. This avoids editing buffers while enumerating them.

## Status Report Notes

Status is a snapshot, not a live background system. The Options UI calls status properties separately, so `TransitWaitStatus.RefreshIfNeeded()` caches one snapshot per UI frame and then throttles refreshes.

`stop entity` in the log is the actual stop/terminal entity after grouping route waypoints through `Connected.m_Connected`.

`waypoint entity` is the route waypoint entity that owns the `WaitingPassengers` component.

`line entity` is the owner line entity from `Owner.m_Owner`. If the name resolves to a generic debug name such as `Bus Line Tool`, it is hidden as a line hint because it is not useful to players.

## Intentional Non-Features

- No Harmony transpiler or AI system replacement.
- No unbunching changes for now because line data can be save-persistent and would need reset button for true restore.
- No group/family cancellation yet. Groups can still hold a vehicle because vanilla leader/member behavior needs more research before mutation is safe.
- No direct edits to `m_AccessDistance` yet. That may affect path/access behavior rather than just boarding speed.
