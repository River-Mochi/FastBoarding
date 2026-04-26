## Fast Boarding

Fast Boarding is a transit mod focused on helping public transport board faster without replacing the game's whole transport AI systems.

### Uses two lighter-touch ideas:

#### Sliders
  - make normal boarding/loading faster through vanilla transit stop data (safe prefab change)

#### Skip Late Passengers [x] toggle
  - after vanilla departure frame,
  - if a solo cim is still not *Ready*,
  - let that cim miss this vehicle,
  - remove them from the vehicle passenger buffer,
  - let vanilla continue from there


### Current Features

- Separate boarding-speed sliders for:
  - Bus
  - Rail: train, tram, subway
  - Ship + ferry
  - Airplane
- `Skip Late Passengers` toggle
- Status report for current waits, worst stops, and skipped cims (in Options menu to not affect city performance).
- `Stats to Log` button with even more details for the curious

### How It Works

The sliders change the base game's `Game.Prefabs.TransportStopData` on passenger transport stop prefabs. Fast Boarding only changes:

- `m_LoadingFactor`
- `m_BoardingTime`

Fast Boarding does not change `PublicTransport.m_DepartureFrame`, line schedules, vehicle counts, route counts, or the vanilla transport AI systems.

At `1x`, untouched stops are left alone. At `2x-10x`, the mod reduces boarding/loading time so normal queues clear faster through vanilla systems.

The optional `Skip Late Passengers` toggle watches vehicles that are already past their vanilla departure frame. If a solo cim assigned to that vehicle is still not ready, the mod detaches that cim from the current vehicle and removes the missed vehicle leg from their current path. The cim is not deleted; they simply miss that vehicle and vanilla systems continue from there.

Groups/families travelling together are not skipped yet. They can still delay a vehicle like vanilla, because group leader/member behavior needs more research before changing it safely.

### Compared With All Aboard

All Aboard solved the waiting problem inside the vehicle AI itself by disabling/replacing the vanilla road and train transport AI systems with patched copies.

Fast Boarding takes a smaller no-Harmony route:

- It tunes the stop data vanilla already reads.
- It optionally lets late solo cims miss a vehicle after vanilla departure time.
- It works with the vanilla transport AI instead of replacing it.

### Notes

- The mod is designed to be save-game safe and safe to remove.
- `Skip Late Passengers` is still experimental, so test on a copy of a save first until there is more live testing.
- Game errors and warnings still appear in the game's normal logs, but mod-specific logging goes to `FastBoarding.log`.
- Verbose logging is for testers only and should stay OFF during normal gameplay.

### Credits
- River-Mochi: mod author
- Inspiration and thanks to bcallender's All Aboard mod.
