**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/sdv**

----

<div align="center">

# Springmyst

</div>

## What this is

This mod replaces vanilla weapon enchantments with an entirely new and significantly more interesting collection. It also adds a few slingshot-exclusive enchantments.

In addition, this mod optionally replaces the generic vanilla "Forged" text in weapon tooltips with actual configurable gemstone sockets.


## Melee Weapon Enchantments

| Name      | Effect |
| --------- | -------|
| Haymaker | *Unchanged from vanilla.* |
| Blasting | Absorbs and stores the damage from enemy hits (before mitigation). The next special move releases twice the accumulated damage as an explosion. |
| Carving | Attacks on-hit reduce enemy defense by 1 point (continuing below zero). Armored enemies (i.e., Armored Bugs and shelled Rock Crabs) lose their armor when their defense is reduced to zero. |
| Cleaving | Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies around the target. |
| Mammonite | Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold. Each consecutive takedown increases this threshold by 1%, resetting when you take damage.* |
| Shocking | Moving and attacking generates energy, up to 100 stacks. At maximum stacks, the next attack causes a powerful electric discharge which deals heavy damage in a large area. |
| Stabbing | Replaces the defensive parry special move with a stabbing lunge attack. **Sword only**.
| Sunburst | Deal 50% more damage to shadow and undead monsters. Weapon swings create a mid-range sunlight projectile which blinds enemies upon collision. Prevents Mummies from resurrecting. |
| Vampiric | Enemy takedowns recover some health proportional to the enemy's max health. Excess healing is converted into a shield for up to 20% of the player's max health, which slowly decays after not dealing or taking damage for 15s. |
| Wabbajack | Causes unpredictable effects.** |

<font size="1">

\* *Hard caps at 1000 HP. To prevent cheesing boss monsters from expansion mods, this is implemented as a percentage chance per hit, with the chance being near-zero close to the 1000 HP hard cap and near 100% for regular monsters.*

\** *Example effects: execute or fully-heal the enemy; apply a random [debuff](../Core); grow or shrink the enemy; split the enemy in two; transform the enemy into a random animal; transform the enemy into a random amount of cheese.*
</font>


## Slingshot Enchantments

| Name       | Effect |
| ---------- | -------|
| Shocking | Moving and shooting generates energy, up to 100 stacks. At maximum stacks, the next shot carries an electric charge, which discharges dealing heavy area damage when it hits an enemy. |
| Freljord | Progressively chills enemies on hit for 2 seconds, freezing after stacking 3 times. |
| Quincy | Attacks fire an energy projectile if no ammo is equipped. The projectile is stronger at lower health. Only works when enemies are nearby.* |
| Reverberant | Summons two "echoes" of the fired projectile, that auto-aim at the nearest enemy after a short delay. Only works when enemies are nearby.** |

<font size="1">

\* *Quincy projectiles have no knockback. Damage increase by 50% below 2/3 max health, and again to by 100% when below 1/3 (the projectile will change color to reflect this). If [Professions](../Professions) mod is installed and the player has the Rascal profession, Quincy projectiles can be fired even if a different ammo is equipped in the second ammo slot. If the player also has the Desperado profession, the Quincy projectile's size will be increased proportionally by overcharge **instead  of** its velocity and knockback.*

\** *Echo projectiles inherit 60% of the main projectile's damage.*
</font>


## Compatibility

N/A.


## Credits & Special Thanks

Credits to the following translators:
- ![](https://i.imgur.com/ezVo9Fb.png) [CaranudLapin](https://github.com/CaranudLapin) for French.

Credits and special thanks to the following for inspiration and assets:
- [Bethesda Game Studios](https://www.bethesdagamestudios.com/)﻿ for [Skyrim](https://elderscrolls.bethesda.net/en)
- [Gravity](https://ro.gnjoy.com/index.asp)﻿ for **Ragnarok Online**
- [Riot Games](https://www.riotgames.com/en)﻿ for [League Of Legends](https://www.leagueoflegends.com/en-us/)﻿
- [Tite Kubo](https://en.wikipedia.org/wiki/Tite_Kubo) for [Bleach](https://www.crunchyroll.com/series/G63VGG2NY/bleach)﻿
