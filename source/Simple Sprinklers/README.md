**SimpleSprinkler** is a [Stardew Valley](http://stardewvalley.net/) mod which lets you customise
the sprinkler coverage radius by editing the `config.json`.

Compatible with Stardew Valley 1.2+ on Linux, Mac, and Windows. Requires SMAPI 2.0+.

## Install
1. [Install the latest version of SMAPI](https://smapi.io).
2. [Download the mod](https://www.nexusmods.com/stardewvalley/mods/76).
3. Unzip it into your game's `Mods` folder.
4. Run the game using SMAPI.

## Configure
The mod will work fine out of the box, but you can tweak its settings by editing the `config.json`
file if you want. (The file might only appear after you launch the game with the mod installed.)
These are the available settings:

setting                | what it affects
---------------------- | -------------------
`CalculationMethod` | How the radius should be calculated. The possible values are `vanilla` (game default), `box` (square area), `circle` (a circular area centered on the sprinkler), or `horizontal` / `vertical` (a line centered on the sprinkler). Default `circle`.
`Locations`          | Where sprinklers should be customised. The default value is fine unless you have custom locations. Default `Farm` and `Greenhouse`.
`Radius`             | The sprinkler radius for each type. The available types are `599` (basic sprinker), `621` (quality sprinkler), and `645` (iridium sprinkler). Default values 2, 3, and 5 respectively.

## Compiling the mod
Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

This mod uses the [crossplatform build config](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
so it can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
for troubleshooting.

### Compiling for testing
To compile the mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compiling for release
To package the mod for release:

1. Delete the mod's directory in `Mods`.  
   <small>(This ensures the package is clean and has default configuration.)</small>
2. Recompile the mod per the previous section.
3. Launch the game through SMAPI to generate the default `config.json`.
4. Create a zip file of the mod's folder in the `Mods` folder. The zip name should include the
   mod name and version (like `SimpleSprinklers 1.3.zip`).

## Support
* [Forum](http://community.playstarbound.com/threads/smapi-simple-sprinkler.109782/)
* If you like: [patreon](https://www.patreon.com/TZed?ty=h)
