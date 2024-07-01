**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/sdv**

----

<div align="center">

# DaLionheart

</div>

## What this is

This is the core mod which provides shared functionality required by other DaLion mods.

By itself, also adds the following non-controversial features which should've been part of the vanilla game all along:
- **Improved Hoppers:** Adds the ability for Hoppers to pull items back out from machines, allowing them to fully automate a single machine at a time and transforming them from completely useless into a more balanced version of Automate﻿.
- **Colored Slime Balls:** Causes Slime Balls to take on the color of the Slimes which produced them, and adds regular color-based Slime drops﻿ to Slime Ball loot tables.

These features are required for the functionality of certain [Professions](../Professions) and therefore are not-configurable and non-negotiable.

This mod also adds one "controversial" but entirely optional and disabled by default feature:
- **Witherable Crops:** Crops may wither if left un-watered.

If the above concerns you, really ask yourself how often do you ever forget to water crops? If the answer is not "never" then you deserve the consequences.


## Status Effects

Taking inspiration from classic game tropes, this mod adds a framework for causing various status conditions to enemies. These effects will be used by the various DaLion mods, and can also be used by any C# mod which consumed the provided [API](/ICoreApi).

| Status | Effect |
| ------ | ------ |
| Bleeding | Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (i.e., Dwarven Sentry). |
| Blinded | Causes enemies to lose track of their target and miss attacks. Does not affect Bats or Duggys. |
| Burning | Causes damage equal to 1/16th of max health every 3 seconds, and reduces attack by half. Also causes enemies to move about more randomly. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magma Sparkers). Insects burn 4x as quickly. |
| Chilled | Reduces speed for the duration. If Chilled is inflicted again during this time, the target may be Frozen for three times the duration. Does not affect Ghosts or Skeleton Mage. |
| Frozen | Cannot move or attack. The next hit during the duration deals double damage and ends the effect. |
| Poisoned | Causes damage equal to 1/16 of max health every 3s, stacking up to 3×. If enough stacks are applied the target may suffer instant death. Does not affect Ghosts or Skeletons.|
| Slowed | Reduces speed for the duration. |
| Stunned | Cannot move or attack for the duration. |

Durations depend on the source. These status conditions are exclusively applied to monsters.


## Compatibility

N/A.


## Credits & Special Thanks

Credits to [Roscid](https://next.nexusmods.com/profile/Roscid/about-me?gameId=1303)﻿ for [Slime Produce﻿](https://www.nexusmods.com/stardewvalley/mods/7634).
