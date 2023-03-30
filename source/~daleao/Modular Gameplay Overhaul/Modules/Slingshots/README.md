**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/sdv-mods**

----


<div align="center">

# Modular Overhaul :: Slingshots

</div>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#overview">Overview</a></li>
    <li><a href="#compatibility">Compatibility</a></li>
  </ol>
</details>

## Overview

This module is a complete overhaul of Slingshot gameplay and mechanics with the objective of bringing them up to par with Melee Weapons, by granting to Slingshots all of the same perks afforded to Melee Weapons in vanilla, including:

1. **Critical Hits:** Think of them as headshots. This allows slingshots to benefit from crit. chance and crit. power bonuses.
2. **Special Move:** Pressing the action button will trigger a melee smack attack that stuns enemies for 2 seconds. This allows the player to react to enemies in close range without switching weapons, and quickly reposition to continue firing from safety.
3. **Enchantments:** All gemstone enchantments can be applied to Slingshots, including:
    - Emerald, which grants firing speed (idem for Emerald Rings) and also affects Overcharge if the [Professions](../Professions) module's Desperado profession is used;
    - Jade, which is also affected by the [Enchantments](../Enchantments) module's `RebalancedForges` option.
    - Topaz, which grants defense if the [Enchantments](../Enchantments) module's `RebalancedForges` option is enabled.
    - Garnet, hich grants special move cooldown reduction, provided that the [Enchantments](../Enchantments) module is enabled and the Garnet stone is installed via the optional file;
    - Ruby, Amethyst and Aquamarine, each granting their regular effects.
    
    **Prismatic Shard** enchantments can also be applied, but the enchantment pool will be void unless the [Enchantments](../Enchantments#ranged-enchantments) module's `RangedEnchantments` are enabled.
4. **Infinity Slingshot:** Can be created from the Galaxy Slingshot after receiving enough Galaxy Souls. If the module's [Infinity+1](../Weapons#infinity-plus-one) setting is enabled, it will equally affect this slingshot.
5. **Disabled Grace Period:** Allows slingshot-users to hit enemies in very close range by removing the game's 100-ms "grace period", making them much more reliable weapons.
5. 5. **Slick Moves:** Slingshot-users can retain their running momentum while firing in order to improve mobilty and kiting-ability.
6. **Automatic Selection:** If enemies are nearby, players can optionally choose a slingshot to be equipped automatically.
7. **Snowballs:** Just for fun, snowballs can be thrown freely as long the player is standing on snowy ground and holding an empty Slingshot.

All features are optional and can be toggled individually.

### Rebalance

In order to balance out Slingshot damage against the above changes, an optional rebalance option exists which will reduce the base damage and knockback modifiers of each slingshot to more reasonable values:
- Master Slingshot: Ammo damage x2 >> x1.5
- Galaxy Slingshot: Ammo damage x4 >> x2
- (NEW) Infinity Slingshot: x2.5

If the rebalance is disabled, but the Infinity Slingshot is enabled, then the Galaxy Slingshot's damage modifier will still be slightly reduced to x3, so that the Infinity Slingshot can take the x4.

## Compatibility

- **Not** compatible with other mods that overhaul slingshots, such as [Better Slingshots][mod:better-slingshots] or [Enhanced Slingshots][mod:enhanced-slingshots].
- **Not** compatible with the likes of [Combat Controls][mod:combat-controls] or [Combat Controls Redux][mod:combat-controls-redux], as those features are already included in this and other modules.
- Compatible with [Advanced Melee Framework][mod:amf] and related content packs, but I do not recommend using both together.
- Compatible with [Stardew Valley Expanded][mod:sve]﻿﻿ and will overwrite the changes to weapons stats from that mod, and prevent Tempered Galaxy Weapons from appearing in shops.
- While the Infinity Slingshot will appear in [CJB Item Spawner][mod:cjb-spawner], it will be incorrectly classified as a Melee Weapon and will be unusable if spawned in this way. This is due to CJB not recognizing non-vanilla slingshots. This likely will be fixed in game version 1.6.

<!-- MARKDOWN LINKS & IMAGES -->

[mod:cjb-spawner]: <https://www.nexusmods.com/stardewvalley/mods/93> "CJB Item Spawner"
[mod:better-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/2067> "Better Slingshots"
[mod:enhanced-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/12763> "Enhanced Slingshots"
[mod:combat-controls]: <https://www.nexusmods.com/stardewvalley/mods/2590> "Combat Controls - Fixed Mouse Click"
[mod:combat-controls-redux]: <https://www.nexusmods.com/stardewvalley/mods/10496> "Combat Controls Redux"
[mod:amf]: <https://www.nexusmods.com/stardewvalley/mods/7886> "Advanced Melee Framework"
[mod:sve]: <https://www.nexusmods.com/stardewvalley/mods/3753> "Stardew Valley Expanded"
