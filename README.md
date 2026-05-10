## Fast Boarding

Fast Boarding is a transit mod focused on helping
public transport board faster without replacing the game's whole transport systems.

It uses a lighter-touch approach: tune the transit stops vanilla already uses,
then optionally help with late passengers near departure time.

### Current Features

- Separate boarding-speed sliders for:
  - Bus
  - Rail: train, tram, subway
  - Ship + ferry
  - Airplane
- `Skip Late Passengers` toggle
- `Run Sooner: Bus + Tram` toggle
- Compact Options UI status rows for current waits, worst stops, and skipped cims
- `Stats to Log` report with more details for testers
- Optional verbose logging for diagnostic testing

### Boarding Speed Sliders

The sliders make normal boarding/loading faster through vanilla transit stop data.

- `1x` is vanilla.
- `3x` is the recommended default.
- `5x` is the maximum.

This helps normal queues clear faster, but late passengers can still delay departure
because of vanilla behavior.

### Skip Late Passengers

After the vanilla departure time:

- if a solo cim is still not `Ready`,
- that cim can miss this vehicle,
- the cim is removed from the vehicle passenger buffer,
- the game can naturally reassign them.

The cim is not deleted.

Groups/families travelling together are intentionally left to vanilla so they are not split up.
They can still delay a vehicle like vanilla.

### Run Sooner: Bus + Tram

Bus and tram passengers assigned to a vehicle can start running sooner before departure time.

This is meant to help cims reach buses and trams before they become late enough to miss the vehicle.
It does not force boarding, teleport citizens, or affect other transit types.

### How It Works

Fast Boarding changes the base game's `Game.Prefabs.TransportStopData` on passenger transport stop prefabs.

It only changes:

- `m_LoadingFactor`
- `m_BoardingTime`

Fast Boarding does not change `PublicTransport.m_DepartureFrame`, line schedules,
vehicle counts, route counts, or the vanilla transport AI systems.

At `1x`, untouched stops are left alone. At `2x-5x`, the mod reduces boarding/loading
time so normal queues clear faster through vanilla systems.

### Compared With All Aboard

All Aboard solved the waiting problem inside the vehicle AI itself by disabling/replacing
vanilla transport AI systems with patched copies.

Fast Boarding takes a smaller no-Harmony route:

- tunes the stop data vanilla already reads,
- optionally lets late solo cims miss a vehicle after vanilla departure time,
- optionally makes bus/tram passengers run sooner before departure,
- works with the vanilla transport AI instead of replacing it.

### Notes

- Avoid Harmony and wholesale game AI system replacement.
- Stay save-game safe and safe to remove.
- Verbose logging is for testers and should stay OFF during normal gameplay
  to avoid large log files and performance cost.

### Credits

- River-Mochi: mod author
- Inspiration and thanks to bcallender's All Aboard mod
- yenyang: testing and code feedback
- Neco1996: testing
- MayorCheeks: testing
- gagaxm: testing
- Empiiey: testing
