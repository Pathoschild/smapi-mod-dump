**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/BananaFruit1492/BulkStaircases**

----

**BulkStaircases** is a [Stardew Valley](https://stardewvalley.net/) mod which lets you use multiple [staircases](https://stardewvalleywiki.com/Staircase) at once.

## Contents
* [Install](#install)
* [Use](#use)
* [Configure](#configure)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
3. Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/10622).
4. Run the game using SMAPI.

## Use
Hold a stack of staircases while being in the mine or the skull cavern and press [Shift] + [C] (configurable) to descend the number of levels equal to the number of staircases you are holding.
Special levels (e.g. treasure rooms, infested levels,... ) can be configured to not be skipped. By default, all <b>special levels will not be skipped</b>.

## Configure
### config.json
The mod creates a `config.json` file in its mod folder the first time you run it. You can open that
file in a text editor to configure the mod.

These are the available settings:

<table>
<tr>
  <th>setting</th>
  <th>effect</th>
</tr>
<tr>
  <td><code>NumberOfStaircasesToLeaveInStack</code></td>
  <td>

Default `0`. Number of staircases that will not be used in the held stack of staircases. In case you may want to use those manually later.

  </td>
</tr>
<tr>
  <td><code>SkipLevel100SkullCavern</code></td>
  <td>

Default `false`. Whether to skip level 100 in skull cavern.

  </td>
</tr>
<tr>
  <td><code>SkipDinosaurLevels</code></td>
  <td>

Default `false`. Whether to skip prehistoric floors.

  </td>
</tr>
<tr>
  <td><code>SkipTreasureLevels</code></td>
  <td>

Default `false`. Whether to skip levels with a treasure.

  </td>
</tr>
<tr>
  <td><code>SkipQuarryDungeonLevels</code></td>
  <td>

Default `false`. Whether to skip quarry dungeon levels that may appear after having been to the quarry mine.

  </td>
</tr>
<tr>
  <td><code>SkipSlimeLevels</code></td>
  <td>

Default `false`. Whether to skip slime infested levels.

  </td>
</tr>
<tr>
  <td><code>SkipMonsterLevels</code></td>
  <td>

Default `false`. Whether to skip monster infested levels.

  </td>
</tr>
<tr>
  <td><code>SkipMushroomLevels</code></td>
  <td>

Default `false`. Whether to skip mushroom levels.

  </td>
</tr>
<tr>
  <td><code>ToggleKey</code></td>
  <td>

The configured controller, keyboard, and mouse buttons (see [key bindings](https://stardewvalleywiki.com/Modding:Key_bindings)).
You can separate multiple buttons with commas. The default value is `LeftShift + C` to descend the levels.

  </td>
</tr>
</table>
