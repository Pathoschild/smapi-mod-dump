**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/nikperic/energytime**

----

## EnergyTime

[NexusMods Page](https://www.nexusmods.com/stardewvalley/mods/6488)

### Overview and Goal

This mod lets time pass proportionally to your energy usage. You can also fast forward by (default) pressing and holding the 'U' key.

With the default configuration, using all of your stamina will cause your day to reach roughly 6:30pm / 18:30.

The inspiration for this mod comes from the relaxed feeling when there is no time constraint in games like Littlewood. I always found myself modifying the default time passage, often freezing it as a result. In the end, I prefer the approach in games like Littlewood and wanted something similar for Stardew hence this mod.

### In-Game Controls

Default controls below:

- `U`: hold key down fast forward time
- `N`: pause or unpause time
- `B`: reload the config settings from file
- `.`: increase the energy requirement multiplier by 0.1. Combine with Shift to increase by 1.0, or Alt to increase by 0.01
- `,`: decrease the energy requirement multiplier by 0.1. Combine with Shift to decrease by 1.0, or Alt to decrease by 0.01
- ';': toggle between Vanilla Time and Energy Time

While in Vanilla Time, time will pass by normally, unless you are holding U to fast forward, or have paused time with N (default keybindings, may differ given your configuration).

Shortcuts will only work when there are no active menus.

### Multiplayer

In multiplater, all connected players stamina is pooled together for calculations. Whenever a player joins or leaves the game, the base factor for time passing is calculated and reset.

In theory, this could allow for an infinite day exploit if a player keeps leaving and rejoining since it also resets the stamina used value, but this seems like a lot of effort for an exploit.

Only the host can pass time via the pass time shortcut during a multiplayer game.

### Configuration Options

Configuration options are listed below, with their default values.

The default key bindings are meant to match the TimeSpeed (https://www.nexusmods.com/stardewvalley/mods/169) mod configuration options

```
  "EnergyRequirementMultiplier": 2.0,
  "PassTimeKey": "U",
  "PauseTimeKey": "N",
  "ReloadConfigKey": "B",
  "IncreaseMultiplierKey": ".",
  "DecreaseMultiplierKey": ",",
  "TimeModeToggleKey": ";"
```

To modify how quickly your energy depletion makes time pass, tweak the `EnergyRequirementMultiplier` configuration value. The higher the value, the more energy must be expended for a 10 minute tick to pass.

The mod expects that at 1.0, there will be 150 ticks that you want passed.

With the default configuration, using all of your stamina will cause your day to reach roughly 6:30pm / 18:30.

Presently, the exceptions include using stamina above the amount required for a tick (e.g. typically happens with charged tool usage, fishing rod usage at low max stamina). I'm not yet sure if this will be tweaked as the goal is to create a relaxed environment but I'm also not 100% satisfied with the balance there.

### Compatibility

This mod should not impact most other mods, however there is a high likelihood of conflicts with other mods that manipulate time mechanics in the game.

### Feedback

I'm open to suggestions for improvements and tweaks, but have a full time job and other responsibilities. This is very much just something I did for fun for myself that I wanted to share so please bear with me if I take some time to respond to improvement suggestions.

That said, I shall do my best to address any critical bugs in a timely fashion.

### License

Copyright Nik Peric

Distributed under the MIT License