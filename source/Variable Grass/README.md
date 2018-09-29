This is a collection of Stardew Valley mods by dantheman999. Currently there's only one mod available.

**Variable Grass** adds a randomised chance for more or less plant growth per day.

Compatible with Stardew Valley 1.11+ on Linux, Mac, and Windows.

## Contents
* [Installation](#installation)
* [Configuration](#configuration)
* [Versions](#versions)
* [Compiling the mod](#compiling-the-mod)

## Installation
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
2. Download the source code.
3. Build the project in Visual Studio or MonoDevelop to automatically copy the files into your mod directory.
4. Run the game using SMAPI.

## Configuration
The mod will work fine out of the box, but you can tweak its settings by editing the `config.json`
file if you want. These are the available settings:

| setting           | what it affects
| ----------------- | -------------------
| `MinIterations`   | The minimum iterations of grass growing function to perform.
| `MaxIterations`   | The maximum iterations of grass growing function to perform.

## Versions
1.0:
* Initial version.

1.1:
* Fixed not growing at correct times.
* Slightly improved.

1.2:
* Added ini file.
* Internal cleanup.

1.3:
* Updated to SMAPI 1.3+ and Stardew Valley 1.1+.
* Standardised config file.

## Compiling the mod
This mod uses the [crossplatform build config](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
so it can be built on Linux, Mac, and Windows without changes. See [its documentation](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
for troubleshooting.

### Compiling the mod for testing
To compile the mod and add it to the mods directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compiling the mod for release
To package the mod for release:

1. Delete the game's `Mods/VariableGrass` directory.  
   <small>(This ensures the package is clean and has default configuration.)</small>
2. Recompile the mod per the previous section.
3. Launch the game through SMAPI to generate the default `config.json`.
2. Create a zip file of the game's `Mods/VariableGrass` folder. The zip name should include the
   mod name and version. For example:

   ```
   VariableGrass-1.3.zip
      VariableGrass/
         VariableGrass.dll
         VariableGrass.pdb
         config.json
         manifest.json
   ```