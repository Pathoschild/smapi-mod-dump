**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/daleao/sdv-mods**

----

<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->

# Fellowship - Immersive Rings

<br/>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#features">Features</a></li>
    <li><a href="#compatibility">Compatbility</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#special-thanks">Special Thanks</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

</td></tr></table>

## Features

This mod brings the following immersive changes to ring effects and recipes:

- Rebalances the Jade and Topaz rings.
    - **Jade:** *+10% -> +30 crit. power.* Vanilla 10% crit. power grants a 10% damage bonus that only applies to crits. This makes it significantly worse than the Ruby ring, which grants the same 10% bonus, but all the time. At 30% crit. power, the Jade ring becomes a better choice than the Ruby ring when crit. chance is at least 33%.
    - **Topaz:** *literally nothing -> +3 defense.* This makes it consistent with the Topaz enchantment, which also grants defense.
    - **Crabshell:** *+5 defense -> +10 defense.* Because the Topaz rings now also grants defense, the Crabshell ring is buffed so that it remains relevant, but not as strong as a full-Topaz Iridium Band.
- Adds progressive crafting recipes for gemstone rings, with corresponding visuals.
    - **Amethyst and Topaz:** *combat level 2, uses copper bars.*
    - **Aquamarine and Jade:** *combat level 4, uses iron bars.*
    - **Emerald and Ruby:** *combat level 6, uses gold bars.*
- Adds recipes for the Glow and Magnet rings, which consume their smaller counterparts.
- Replaces the recipe for the Glowstone ring, to actually consume one Glow and one Magnet rings.
- Overhauls the Iridium Band.
    - By default the Iridium Band grants no effects.
    - Instead the Iridium Band functions as a vessel for up to **four** gemstone rings. Naturally, this process requires access to the Forge.
    - The same type of gemstone ring can be fed to one Iridium Band more than once, compounding the effect.
    - In result, the Iridium Band provides a balanced way to wear up to eight rings at once by forcing the player to give up utility for combat advantage.

All features can toggled on or off.
If you are a fan of planning character builds and making tough meaningful choices, this mod is for you. If you don't care about balance and just want to stack all possible effects and chill, then [Ring Overhaul](https://www.nexusmods.com/stardewvalley/mods/10669) will be more your cup of tea.

## Compatibility

This mod makes use of Harmony to patch vanilla crafting behavior. As such there may be unexpected behavior with mods that change the crafting menu.
NEW: [Better Crafting](https://www.nexusmods.com/stardewvalley/mods/11115) is supported since v1.0.3 of this mod.

This mod is my own take on a "balanced combine many rings". Obviously it is not compatible with other mods with similar scope, including [Combine Many Rings](https://www.nexusmods.com/stardewvalley/mods/8801), [Balanced Combine Many Rings](https://www.nexusmods.com/stardewvalley/mods/8981) and, to an extent, [Ring Overhaul](https://www.nexusmods.com/stardewvalley/mods/10669); because of it's highly modular nature, Ring Overhaul in particular can still be used with this mod, provided you know how to customize the configs to cherry-pick non-conflicting features.

NEW: [Better Rings](https://www.nexusmods.com/stardewvalley/mods/8642) is now supported! I'm not aware of any other ring retextures but they will not be compatible.

Should be compatible with Wear More Rings, although I haven't tested it.

This is a companion mod for [Walk Of Life](https://www.nexusmods.com/stardewvalley/mods/8111), but can be used independently.

Should be fully compatible with multiplayer. Not compatible with Android.

## Installation

Install like any other mod, by extracting the content of the downloaded zip file to your mods folder and starting the game via SMAPI.

To update, first delete the old version and then install the new one. You can optionally keep your configs.json in case you have personalized settings.

To uninstall simply delete the mod from your mods folder. This mod is safe to uninstall at any point.

## Special Thanks

- [Goldenrevolver](https://www.nexusmods.com/stardewvalley/users/5347339) for the idea of progressive gemstone rings.
- [compare123](https://www.nexusmods.com/stardewvalley/users/13917800) for Better Rings-compatible textures.
- **ConcernedApe** for StardewValley.
- [JetBrains](https://jb.gg/OpenSource) for providing a free license to their tools.

<table>
  <tr>
    <td><img width="64" src="https://smapi.io/Content/images/pufferchick.png" alt="Pufferchick"></td>
    <td><img width="80" src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" alt="JetBrains logo."></td>
  </tr>
</table>

## License

See [LICENSE](../../LICENSE) for more information.
