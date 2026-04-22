## Fast Boarding

Fast Boarding is a transit mod focused on boarding tweaks.

Instead of replacing the vanilla transport AI systems, this version uses:

- Prefab stop tuning to shorten dwell time
- Optional late-boarder cancellation so tardy passengers can miss a vehicle instead of holding it forever

### Current Features

- Separate boarding-speed sliders for:
  - Bus
  - Rail (train, tram, subway)
  - Water (ship, ferry)
  - Air
- `Cancel Late Boarders` toggle

### How It Works

The boarding speed sliders increase each stop family's effective loading speed and also adjust the related boarding-time estimate used by the game.

The optional late-boarder system watches public transport vehicles that are already past their departure frame.
If a passenger assigned to that vehicle still is not ready, the mod can cancel that boarding attempt so the vehicle is free to leave and the passenger can try again later.

### Design Goals

- Avoid Harmony where possible
- Avoid wholesale AI system replacement
- Keep the mod compatible with the standard CS2 toolchain and Options UI patterns
- Give players broad tuning ranges for testing what feels best

### Notes

- Group boarding is handled conservatively to avoid splitting groups across platform and vehicle.
- The mod is designed to be save-game safe and safe to remove.
- The late-boarder behavior is still experimental, so it is smart to test it on a copy of a save first until we have a little live testing behind it.
- Game errors and warnings still appear in the game's normal logs, but mod-specific logging goes to `FastBoarding.log`.

### Credits
- River-Mochi mod author
- Inspiration and thanks to bcallender's All Aboard mod.


All Aboard solved the problem inside the vehicle AI itself. It changed the exact “should this bus/train keep waiting?” logic,
so it had to replace the transport AI systems that own that decision.

Fast Boarding takes a lighter route. It changes the inputs around boarding instead of replacing the AI:

- it shortens dwell time by tuning stop prefab data
- it can cancel overdue passengers from that vehicle’s boarding list after departure time
- Once those late passengers are no longer counted as pending for that vehicle, vanilla AI can leave on its own.
- So this mod works with the game’s transport AI instead of overriding it.


This mod does not replace the vanilla TransportAIsystems. It speeds up stop boarding data and, when enabled,
removes overdue passengers from a vehicle’s pending boarding list so vanilla departure logic can continue normally.”
