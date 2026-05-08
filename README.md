## Fast Boarding

Fast Boarding is a transit mod focused on helping
public transport board faster without replacing the game's whole transport systems.

### Uses two lighter-touch ideas

#### Boarding speed sliders

- Make normal boarding/loading faster through vanilla transit stop data.
- On Sliders: `1x` is vanilla.

#### Skip Late Passengers toggle

After the vanilla departure frame:

- if a solo cim is still not *Ready*,
- that cim can miss this vehicle,
- the cim is removed from the vehicle passenger buffer,
- vanilla continues from there.

The cim is not deleted. Game can just naturally reassign them.

### Current Features

- Separate boarding-speed sliders for:
  - Bus
  - Rail: train, tram, subway
  - Ship + ferry
  - Airplane
- `Skip Late Passengers` toggle
- Compact Options UI status rows for current waits, worst stops, and skipped cims
- `Stats to Log` report with more details for testers
- Optional verbose logging for diagnostic testing (do not use for normal gameplay)

### How It Works

The sliders change the base game's `Game.Prefabs.TransportStopData` on passenger transport stop prefabs. Fast Boarding only changes:

- `m_LoadingFactor`
- `m_BoardingTime`

Fast Boarding does not change `PublicTransport.m_DepartureFrame`, line schedules, vehicle counts, route counts, or the vanilla transport AI systems.

At `1x`, untouched stops are left alone. At `2x-5x`, the mod reduces boarding/loading time so normal queues clear faster through vanilla systems.

The optional `Skip Late Passengers` toggle watches vehicles that are already past their vanilla departure frame. If a solo cim assigned to that vehicle is still not ready, the mod detaches that cim from the current vehicle and removes the missed vehicle leg from their current path.

Groups/families travelling together are not skipped yet. They can still delay a vehicle like vanilla, because group leader/member behavior needs more research before changing it safely.

### Compared With All Aboard

All Aboard solved the waiting problem inside the vehicle AI itself by disabling/replacing vanilla transport AI systems with patched copies.

Fast Boarding takes a smaller no-Harmony route:

- tunes the stop data vanilla already reads.
- optionally lets late solo cims miss a vehicle after vanilla departure time.
- works with the vanilla transport AI instead of replacing it.

### Notes

- Avoid Harmony and wholesale game AI system replacement
- Stay save-game safe and safe to remove
- Verbose logging is for testers only and should stay OFF during normal gameplay.


### Credits

- River-Mochi: mod author
- Inspiration and thanks to bcallender's All Aboard mod
- yenyang: testing and code feedback
- Neco1996: testing
