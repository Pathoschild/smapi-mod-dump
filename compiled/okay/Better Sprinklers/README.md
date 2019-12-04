**Better Sprinklers** by Maurício Gomes (Speeder) is a [Stardew Valley](http://stardewvalley.net/)
mod which lets you edit sprinkler coverage (while adjusting their crafting cost), and lets you view
a sprinkler's or scarecrow's coverage by pointing at it in `F3` mode.

## Contents
* [Install](#install)
* [Use](#use)
* [Compatibility](#compatibility)
* [Versions](#versions)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
3. Unzip [the mod files](http://www.nexusmods.com/stardewvalley/mods/41) into your `Mods` folder.
4. Run the game using SMAPI.

## Use
### Editing sprinkler coverage
* Press `K` (editable in `config.json`) to show a sprinkler coverage editor and click the squares
  to set the coverage. You can use any shape you want, though you can't remove the default squares.
  Press "OK" when you're done, and the mod will automatically adjust the crafting and purchase cost
  for balance.

  > ![](docs/screenshot.png)
  > ![](docs/circle.png)
  > ![](docs/butterfly.png)

* When placing a sprinkler or scarecrow, its coverage will be highlighted automatically. You can also
  press `F3` and point at a sprinkler or scarecrow to highlight its coverage.

  > ![](docs/scarecrowarea.png)

* The sprinklers activate in the morning when the day starts.

## Compatibility
Better Sprinklers is compatible with Stardew Valley 1.3+ on Linux/Mac/Windows, both single-player and multiplayer.

If two players have the mod installed in multiplayer, both ranges will apply.

## Versions
### 2.4
- Updated for Stardew Valley 1.3 (including multiplayer support) and SMAPI 3.0.
- Added support for controller bindings.
- Added mod-provided API to let other mods access the custom sprinkler coverage.
- Added support for sprinklers inside custom buildings.
- Improved compatibility with other mods that change object/recipe data.

### 2.3
- Corrected forum thread link, and default config minor error.

### 2.2
- Updated for Stardew Valley 1.1 and SMAPI 0.40.0 1.1.

### 2.1
- Added highlighting to the area of sprinklers and scarecrows.
- Added grid rendering.
- Added html readme.
- Fixed config bug (it was always `K` even if you edited the config.json).

### 2.0.1
- Fixed compatibility with SMAPI 0.39.2.

### 2.0
- Updated to SMAPI 0.39.2.
- Added a GUI to configure the sprinklers.
- Sprinklers now work on all farmable areas, including greenhouses and anything added by mods.

## See also
* [![Patreon](docs/ipatreon.png)](https://patreon.com/user?u=3066937)
* [Nexus page](http://www.nexusmods.com/stardewvalley/mods/41)
* [Discussion thread](http://community.playstarbound.com/threads/configurable-improved-sprinklers-scarecrow-and-sprinklers-area-highlights.112443/)
