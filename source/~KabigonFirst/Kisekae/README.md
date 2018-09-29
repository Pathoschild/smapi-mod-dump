﻿**Get Dressed** is a [Stardew Valley](http://stardewvalley.net/) mod which lets you customise your
character's appearance in-game, including 109 new accessories (mostly lipsticks and
blushes). You can open the customisation menu by clicking a new dresser in the farmhouse, or press
`C` on your keyboard (configurable).

Compatible with Stardew Valley 1.1+ on Linux, Mac, and Windows.

## Contents
* [Installation](#installation)
* [Usage](#usage)
* [Configuration](#configuration)
* [Versions](#versions)
* [Compiling the mod](#compiling-the-mod)
* [See also](#see-also)

## Installation
1. [Install the latest version of SMAPI](http://canimod.com/guides/using-mods#installing-smapi).
3. Install [this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/331).
4. Run the game using SMAPI.

## Usage
The mod adds a new dresser in the corner of your farmhouse. The dresser is up against the wall, so
it doesn't take up any room.  
![](screenshots/dresser.png)

Right-click the dresser (or press `C`) to open the customisation menu, where you can customise your
character. You can change your clothing, hair, eyes, face, and skin:  
![](screenshots/customisation-menu.png)

You can save and load up to 36 appearances through the favorites submenu:  
![](screenshots/favorites-menu.png)  
![](screenshots/favorites-menu-2.png)

Finally, you can edit some settings and see information about the mod on the about submenu:  
![](screenshots/about-menu.png)

## Configuration
The mod will work fine out of the box, but you can tweak its settings by editing the `config.json`
file if you want. These are the main settings:

| setting           | what it affects
| ----------------- | -------------------
`MenuAccessKey`   | The keyboard button which opens the customisation menu (see [valid keys](https://msdn.microsoft.com/en-us/library/microsoft.xna.framework.input.keys.aspx)). Can also be changed through the in-game menu. Default `C`.
`ShowDresser`     | Whether to add a dresser to your farmhouse. Default `true`.
`StoveInCorner`  | Whether to move the dresser out of the corner, in case you're using another mod that adds a stove there. Default `false`.
`HideMaleSkirts` | Whether to hide skirt options for male characters. Can also be changed through the in-game menu. Default `false`.
`MenuZoomOut`    | Whether to zoom out when the menu is open for optimum viewing. (The original zoom will be restored when the menu closes.) Default `true`.
`FemaleFaceTypes`<br />`FemaleNoseTypes`<br />`FemaleBottomsTypes`<br />`FemaleShoeTypes`<br />`MaleFaceTypes`<br />`MaleNoseTypes`<br />`MaleBottomsTypes`<br />`MaleShoeTypes` | Change the number of available customisation options if you've added more sprites to the `overrides` spritesheets. Only change this if you know what you're doing.

## Versions
### 3.4
* Updated for Stardew Valley 1.2.
* Added support for 'exit to title' feature in Stardew Valley 1.2.

### 3.3
* Updated for SMAPI 1.9.
* Segregated override textures (additional appearance options are now easily implemented).
* Internal refactoring.

### 3.2
* Updated for SMAPI 1.4.
* Added support for Mac/Linux.
* Added support for new house upgrade in Stardew Valley 1.11.
* Bug fixes.

### 3.1
* Updated for Stardew Valley 1.1.
* Added a new menu with saveable favorites.

### 2.1
* Added new Haley and Penny-style shoes for female characters.

### 2.0
* Added support for changing base options (face type, nose type, pants type, etc.) among other features.

### 1.3
* Fix rare slow load issue and upgrade day issue.

### 1.2
* Added a config option to move dresser if the corner is occupied.

### 1.1
* Fixed a dresser placement issue for first house upgrade when married.

### 1.0
* Initial version.

## Compiling the mod
Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

This mods use the [crossplatform build config](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
so it can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
for troubleshooting.

### Compile for testing
To compile a mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compile for release
To package a mod for release:

1. Delete the mod's directory in `Mods`.  
   <small>(This ensures the package is clean and has default configuration.)</small>
2. Recompile the mod per the previous section.
3. Create a zip file of the mod's folder in the `Mods` folder. The zip name should include the
   mod name and version (like `GetDressed-3.3.zip`).


## See also
* [Nexus mod](http://www.nexusmods.com/stardewvalley/mods/331)
* [Discussion thread](http://community.playstarbound.com/threads/smapi-get-dressed-updated-for-1-1.113731)
