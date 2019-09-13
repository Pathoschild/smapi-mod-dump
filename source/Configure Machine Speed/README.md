# Stardew Valley Mod: Configure Machine Speed
Allows you to change how long it takes for various machines (e.g. kegs, oil maker) to finish.

## Installation:
1. Install the latest version of SMAPI
2. Unzip `ConfigureMachineSpeed.zip` into Stardew Valley/Mods.
3. Run the game using SMAPI.


## Compatibility:
- Works with Stardew Valley 1.3 on Linux/Mac/Windows.
- Compatible with SMAPI 2.11.2+ (untested on earlier versions, but it might still work).
- This mod does NOT work for Farmhands in multiplayer (though it should work for hosts).
- Untested on custom machines (let me know what happens if you try it).
- No known mod conflicts (let me know if you find one).
- Should be compatible with SMAPI 3.0.0 once it is released.


## How To Use:
1. Run the game using SMAPI at least once to generate a `config.json` file inside `Stardew Valley/Mods/ConfigureMachineSpeed`.
2. Modify `config.json` with whatever settings you choose and save the file.
3. Press the `ReloadConfigKey` key (by default this is `L`) while playing the game to reload your modified config file.
4. Enjoy! 

(Note that if some machines are currently processing items, they will need to finish their current task before the new settings take effect)

## The Config File:
1. In the `config.json` file you will see two keys: `UpdateInterval`, `Machines` and `ReloadConfigKey`.
2. `ReloadConfigKey` specifies which key you can press to re-load the settings found in `config.json` while playing the game (useful if you want to change the speed of machines without exiting to desktop).
3. `UpdateInterval` sets how often the mod will check the machine's remaining time and alter it if necessary. It's probably fine as is, but feel free to change it.
4. Each machine in `Machines` has three keys: `Name`, `Time`, and `UsePercent`. 
    * If `UsePercent` is `true`, then `Time` specifies how much time each machine will take as a **percentage** (e.g., setting this to `100` will change nothing, setting it to `50` will make the machines run 2x as fast).
    * If `UsePercent` is `false`, then `Time` specifies how much time each machine will take in **minutes** (e.g., setting this to `100` will make the machine take 100 in-game minutes to finish processing).

## Supported Machines:
- Bee House
- Casks (note: you still need to sleep at least once to make the wine/cheese mature)
- Charcoal Kiln
- Cheese Press
- Crystalarium
- Furnace
- Incubator (note: you still need to sleep to make the egg hatch after the timer runs out)
- Keg
- Lightning Rod
- Loom
- Mayonnaise Machine
- Oil Maker
- Preserves Jar
- Recycling Machine
- Seed Maker
- Slime Egg-Press
- Slime Incubator (note: you still need to sleep to make the egg hatch after the timer runs out)
- Tapper
- Worm Bin

## Known Bugs/Unsupported Machines:
- If you use a mod like UI Info Suite to see how much longer the machines have left to process, it will look like the machines are two minutes too fast. This is just cosmetic and has to do with how the speed change is done. The game only measures increments of 10 minutes anyways so it should not affect your gameplay. (The mod is implemented this way on purpose, since the alternative would be less efficient.)

## Source Code:
This mod is open source under an MIT License. Feel free to check out it's [GitHub repository](https://github.com/BayesianBandit/ConfigureMachineSpeed). I use the issue tracker there if you would prefer to open an issue there (leaving a post on Nexus Mods is fine too though).
