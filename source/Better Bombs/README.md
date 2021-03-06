**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/PhillZitt/BetterBombs**

----

**BetterBombs** is a [Stardew Valley](https://stardewvalley.net/) mod which increases
the utility of bombs by offering several configurable options to change bomb behavior.

## Contents
* [Install](#install)
* [Configure](#configure)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/7787).
3. Run the game using SMAPI.

## Configure
### config.json
The mod creates a `config.json` file in its mod folder the first time you run it. You can open that
file in a text editor to configure the mod.

These are the available settings:

<table>
  <tr>
    <th>Option</th>
    <th>Effect</th>
    <th>Default</th>
    <th>Notes</th>
  </tr>
  <tr>
    <td>
      <code>
        BreakClumps
      </code>
    </td>
    <td>
      Bombs will break resource clumps - Boulders, Stumps, Logs, etc.
    </td>
    <td>
      <code>
        True
      </code>
    </td>
    <td>
    </td>
  </tr>
  <tr>
    <td>
      <code>
        CollectMinerals
      </code>
    </td>
    <td>
      Bombs will collect grabbable objects on the ground instead of destroying them.
    </td>
    <td>
      <code>
        True
      </code>
    </td>
    <td>
    </td>
  </tr>
  <tr>
    <td>
      <code>
        DamageFarmers
      </code>
    </td>
    <td>
      Bombs will deal damage to players.
    </td>
    <td>
      <code>
        False
      </code>
    </td>
    <td>
    </td>
  </tr>
  <tr>
    <td>
      <code>
        Radius
      </code>
    </td>
    <td>
      A multiplier to the default radius of bombs.
    </td>
    <td>
      <code>
        1.0f
      </code>
    </td>
    <td>
      [Be careful of values above 3](https://github.com/PhillZitt/BetterBombs/issues/6).
    </td>
  </tr>
  <tr>
    <td>
      <code>
        Damage
      </code>
    </td>
    <td>
      A multiplier to the default damage of bombs.
    </td>
    <td>
      <code>
        1.0f
      </code>
    </td>
    <td>
    </td>
  </tr>
</table>
