**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/modular-overhaul**

----

<div align="center">

# MARGO :: Combat (CMBT)

</div>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
<summary>Table of Contents</summary>
<ol>
	<li><a href="#overview">Overview</a></li>
	<li>
		<a href="#rebalanced-stats">Rebalanced Stats</a>
		<ol>
			<li><a href="#defense">Defense</a></li>
			<li><a href="#knockback">Knockback</a></li>
			<li><a href="#critical-strikes">Critical Strikes</a></li>
			<li><a href="#attack-speed">Attack Speed</a></li>
		</ol>
	</li>
	<li>
		<a href="#melee-weapon-changes">Melee Weapon Changes</a>
		<ol>
			<li><a href="#combo-framework">Combo Framework</a></li>
			<li><a href="#offensive--defensive-swords">Offensive & Defensive Swords</a></li>
			<li><a href="#base-stats--tooltips">Base Stats & Tooltips</a></li>
			<li><a href="#weapon-tiers">Weapon Tiers</a></li>
			<li><a href="#the-mines--weapon-acquisition">The Mines & Weapon Acquisition</a></li>
			<li><a href="#woody-replaces-rusty">Woody Replaces Rusty</a></li>
			<li><a href="#melee-enchantments">Melee Enchantments</a></li>
		</ol>
	</li>
	<li>
		<a href="#ranged-weapon-changes">Ranged Weapon Changes</a>
		<ol>
			<li><a href="#rebalanced-modifiers--ammo">Rebalanced Modifers & Ammo</a></li>
			<li><a href="#special-move">Special Move</a></li>
			<li><a href="#gemstone-forges">Gemstone Forges</a></li>
			<li><a href="#ranged-enchantments">Ranged Enchantments</a></li>
			<li><a href="#other-features">Other Features</a></li>
		</ol>
	</li>
	<li><a href="#ring-changes">Ring Changes</a>
		<ol>
			<li><a href="#rebalanced-combat-rings">Rebalanced Combat Rings</a></li>
			<li><a href="#craftable-gemstone-rings">Craftable Gemstone Rings</a></li>
			<li><a href="#infinity-band">Infinity Band</a></li>
		</ol>
	</li>
	<li><a href="#garnet--gemstone-resonance-theory">Garnet & Gemstone Resonance Theory</a></li>
	<li><a href="#infinity-1">Infinity +1</a></li>
	<li><a href="#status-effects">Status Effects</a></li>
	<li><a href="#enemies">Enemies</a></li>
	<li><a href="#controls--quality-of-life">Controls & Quality of Life</a>
		<ol>
			<li><a href="#face-mouse-cursor">Face Mouse Cursor</a></li>
			<li><a href="#slic-moves">Slick Moves</a></li>
			<li><a href="#auto-selection">Auto-Selection</a></li>
		</ol>
	</li>
	<li><a href="#compatibility">Compatibility</a></li>
	<li><a href="#faq">F.A.Q.</a></li>
</ol>
</details>

## Overview

This humongous module consolidates rebalances to melee weapons, ranged weapons and combat rings, together with entirely new mechanics which will overall make combat significantly more strategic and challenging.

## Rebalanced Stats

Most stats besides pure damage are not worth investing in vanilla. The following changes are designed to make all stats viable and worthy of investment through rings and weapon forges.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Defense

Defense is rather pointless in vanilla, where each point simply mitigates a single unit of damage. Consequently, small defense bonuses are completely useless late-game. This mod implements a new, simple damage mitigation formula, which allows defense to scale into late-game without becoming overpowered.

	Old formula:
	```math
	damage = Min(rawDamage - defense, 1)
	```

	New formula:
	```math
	resistance = 10 / (10 + defense)
	```
	```math
	damage = rawDamage * resistance
	```

One defense point now reduces damage by 10% regardless of incoming damage. Subsequent points have diminishing returns, such that 100% damage negation is not possible to achieve. **This also applies to monsters!**

The **Topaz Ring** which was uselss in vanilla, has been changed to increased defense by 1 point, like its corresponding weapon forge.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Knockback

Knockback in vanilla is too high by default; hitting enemies with any weapon sends them flying for several tiles. This makes weapon-spamming far more effective than any defense, and any further investment in knockback is useless.

To fix this, we begin by lowering the default knockback for all weapons down to a more reasonable level. Knockback will no longer throw enemies far enough away without enemy investment.

To compensate, knockback can now also be used offensive; enemies will now suffer damage proportional to their momentum when thrown against a wall or object. This means that cornering enemies is an extremely strong strategy, and makes heavy weapons and knockback investment an attractive deal.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Critical Strikes

I love the idea of crit. builds. But it's simply not viable in vaniilla without the Iridium Needle. Base crit. chance is too low to ever be significant, and base crit. power is way too high.

To fix this we adjust the base crit. stats of all weapons. Heavier weapons are harder to crit. with, but crit. harder when they do, while lighter weapons can crit. more reliably but require some crit. power investment.

In addition, crit. strikes will ignore enemy defense. Crit. chance is also doubled when hitting enemies from behind, but this can only be achieved by applying certain [status effects](#status-effects).

The effects of **Jade** crit. power bonuses from rings and weapon forges have also been buffed significantly:

<details>
<summary><b>Jade:</b></summary>

**+10% -> +50% crit. power.**

<font size="2">A 10% boost to crit. power is a 10% damage boost that *only* applies to crits. To put that in perspective, only when the player has 100% crit. chance then they will receive an overall 10% boost to damage. It should be clear that this is complete garbage next to a Ruby Ring, which straight up grants a 10% boost to damage, *all the time*. At 50% crit. power, the Jade Ring becomes a better choice than the Ruby Ring if the player has at least 20% crit. chance, which should be attainable by any weapon type given an appropriate build. Above that threshold, Jade Rings become even stronger.</font>
</details>

Lastly, slingshots gain the ability to critically strike. Think of them as headshots.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Attack Speed

*See [Combo Framework](#combo-framework) below.*

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Melee Weapon Changes

Vanilla weapons suffer from two major issues:
	1. Overabundance. Weapons quickly become inventory clutter. The game is also quick to gift its most powerful weapons, trivializing any weapons obtained as drops.
	2. Same-y-ness. Apart from their special moves, none of the weapon types feel particularly unique; club and dagger are simply "worse swords".

In addition to the stat changes mentioned above, we introduce several new mechanics to make overall make combat significantly more strategic and challenging.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Combo Framework

As mentioned [above](#knockback), weapon spamming is a real problem due to the way it negates all other forms of defense, and summarizes vanilla combat to a boring click-fest.

This is solved by implementating a **combo framework** for melee weapons. A combo is a short burst of continuous swings, followed by a short, forced cooldown. Each weapon type has a configurable combo limit:
	
- **Swords:** up to 4 horizontal swipes, by default.
- **Clubs:** up to 2 hits, being one horizontal swipe and one vertical swipe, by default.
- **Daggers:** unchanged, effectively up to infinite hits.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/combo_sword_small.gif" alt="combo_sword.gif" width="33%">
<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/combo_club_small.gif" alt="combo_club.gif" width="33%">
</div>

To reduce the cooldown between combos you may consider investing in **Emerald** rings and weapon forges. They will also increase the speed of every single attack frame (instead of only the last one as in vanilla). This fixes the attack speed stat and makes it a worthy investment.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Offensive & Defensive Swords

In the vanilla game code one can find traces of an abandoned fourth weapon type: **Stabbing Swords**. This module re-implements stabbing swords into the game, splittng all vanilla swords between **offensive** and **defensive** archetypes.

Defensive swords retain the vanilla parry special move. Parry damage is changed to increase by 10% for every defense point, giving defense bonuses some extra offensive value. Note that this defensive move will be paramount to survive in between your combos!

Offensive swords, meanwhile, gain a bran new stabbing thrust move. This move allows quick repositioning while granting invincibility frames. If used while hovering over an enemy, the farmer will attempt to home in on the enemy, turning if necessary. You can always manually turn mid-dash by inputting a perpendicular directional command.

By default, the Galaxy Sword and Infinity Blade are both of the Defensive type, but you can change them into Offensive swords and back at any time, once a day, by taking them to the Sand Pillars.

For any other sword (included modded), the type can be changed by adding or removing it from the stabbing swords list in the config json (not available in GMCM).

<div align="center">

<img src="https://github.com/daleao/modular-overhaul/blob/main/resources/screenshots/stabbing_special.gif?raw=true" alt="stabbing_special.gif" width="67%">
</div>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Base Stats & Tooltips

Having discussed all of the stat changes, as well as the new weapon type, we can now describe the new base stats for each weapon type, designed to more clearly distinguish a playstyle for type:

- **Clubs** are heavy-hitting but unreliable. They are imprecise and sluggish, but offer the highest knockback and damage-potential, and have the widest hitbox. Their difficulty to maneuver makes them rarely hit critical strikes, but cause significant damage when they do.
- **Daggers** are quick, nimble and precise. Their hitbox is short and narrow, but can be spammed with no downtime. They are by far the easiest weapon with which to hit critical strikes, but too small to cause significant damage. With the addition of [Status Effects](#status-effects), all daggers have a chance to cause **Bleeding**.
 - **Offensive Swords** are balanced weapons. They swing faster than clubs, and in a wider area than daggers. They are otherwise unremarkable, offerring average damage, knockback, crit. chance and power.
 - **Defensive Swords** are big and heavy, which makes them suitable for personal defense, but also makes them slower. They are somewhere between a club and an offensive sword, with a slightly wider hitbox than the latter.

Along with these stat changes, weapon tooltips have also been significantly improved, offering much better clarity. Instead of a cryptic "+1" to Speed or Weight, you will now see "+10% Attack Speed" or "+10% Knockback". You may also configure the tooltips to show **absolute** or **relative** stats; the former displays straight-forward raw stats, while the latter displays percentages **based on the weapon type's base stats**. The new tooltips also display weapon forges as gemstone sockets instead of the generic vanilla "Forged x/3" text.

<div align="center">

⚠ *Note that, while this mod does not touch non vanilla weapons, they will still be affected by the newer tooltips. If you play with mods like Ridgeside Village which add new weapons, you  will notice that they have absurdly high stats. This is not a bug. Those weapons are broken by design, and I just pulled the curtains. You're welcome.* ⚠
</div>

### Weapon Tiers

Now that the weapon types have been addressed, we need to rebalance the weapons themselves. However, due to the sheer amount of weapons in vanilla, manually adjusting the damage of each one is simply not feasible. We find a better solution, inspired by MMO's and looter RPGs, by assigning all weapons to a [color-coded tier][tropes:color-coded].

<div align="center">

<font color="red"><b>❗ The following changes apply only to vanilla weapons. ❗</b></font>

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/Modules/Combat/resources/readme/tiers.png" alt="tiers">
</div>

<br>

All weapons up to and including the Epic tier have randomized damage, determined the moment they are dropped, and scaled to your Mine progression. This way, players can always continue to loot stronger weapons; all weapons can be useful, and no specific weapon is ever trivialized. Higher-tier weapons will tend to be stronger than lower-tiered ones, but that may not always be the case.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/color-coded.gif" alt="color-coded.gif" width="67%">
</div>


**Mythic** weapons are unique or extremely rare. They are usually quest rewards or very rare monster drops, and tend to carry intrinsic perks in addition to higher-than-average stats. In exchange, they cannot receive Prismatic Shard enchantments at the forge.

The known Mythic weapons are:

<div align="center">

| Weapon | Type | Effects |
| ------ | ---- | ------- |
| ![](./resources/readme/yetitooth.png) Yeti Tooth | Defense Sword | Can cause Chilled status.* |
| ![](./resources/readme/neptuneglaive.png) Neptune Glaive | Defense Sword | Like a crashing wave, deals heavy knockback. |
| ![](./resources/readme/obsidianedge.png) Obsidian Edge | Stabbing Sword | Ignores enemy resistance. Can cause Bleeding.* |
| ![](./resources/readme/lavakatana.png) Lava Katana | Stabbing Sword | Can cause Burning.* |
| ![](./resources/readme/insecthead.png) Insect Head | Dagger | Damage depends on the number of slain bugs. |
| ![](./resources/readme/iridiumneedle.png) Iridium Needle | Dagger | Always critically strikes. |
| ![](./resources/readme/swordfish.png) Sword Fish** | Stabbing Sword | Damage depends on the number of caught fish species. |
</div>

<font size="1">

\* *Status effects are explained [further below](#status-effects).*

\** *Requires [More New Fish](https://www.nexusmods.com/stardewvalley/mods/3578).*
</font>

**Masterwork** weapons are relics of the Dwarven race, crafted from long-lost materials. To obtain them you will need to uncover the **Dwarvish Blueprints**, and kindly ask [a skilled blacksmith](https://stardewvalleywiki.com/Clint) to make sense of them. Having done so, the only thing left will be hunting down the materials:

| Weapons | Description | Material | Source |
| ------- | ----------- | -------- | ------ |
| <div align="center"> ![](./resources/readme/elven_set.png) <br> Elven </div> | Quick, nimble weapons enchanted with forest magic which grants them high reach and knockback. | <div align="center"> ![](./resources/readme/elderwood.png) <br> Elderwood </div> | [Scavenger Hunts](../Professions) |
| <div align="center"> ![](./resources/readme/dwarven_set.png) <br> Dwarven </div> | Large, bulky weapons. They grant high defense and knockback, but reduce speed. | <div align="center"> ![](./resources/readme/dwarvenscrap.png) <br> Dwarven Scrap Metal </div> | Volcano Chests |
| <div align="center"> ![](./resources/readme/dragontooth_set.png) <br> </div> Dragontooth | Light and sharp, granting the highest raw damage out of any weapon in the game. | <div align="center"> ![](./resources/readme/dragontooth.png) <br> Dragon Tooth </div> | Volcano Chests, Dragon Skeletons |

Masterwork weapons can only be obtained if the Dwarven Legacy setting is enabled.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/dwarvish_crafting.gif" alt="dwarvish_crafting.gif" width="67%">
</div>

Finally, the **Legendary** tier will be explained further down in [its own section](#infinity-1).

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### The Mines & Weapon Acquisition

Having rebalanced the weapons themselves, we need to do something about how they are obtained. As we mentioned previously, the vanilla game makes it easy to stockpile fodder weapons, to the point they become nothing more than inventory clutter and a nuisance.

A new weapon should be an exciting prospect. And to acheive that, this module removes all weapons from Mine chests, instead replacing them with valuable consumables and other valuables. In order to obtain new weapons, players will have to fight for monster drops, or get lucky with breakable containers. Monster-dropped weapons are rare, but are often much stronger.

Every few Mine levels, a new tier of weapons will also become available for sale at the Adventurer's Guild. These for-sale weapons, however, have fixed and significantly weaker stats.

### Woody Replaces Rusty

So far we've solved a lot of vanilla problems. But now we reach the most egregious: the very awkward "upgrade" from the starting Rusty Sword to the Wooden Blade. Why would Marlon be mocking about with a rusty weapon anyway?

ConcernedApe originally intended for the Rusty Sword to be upgraded to the Dark Sword, then Holy Blade, and finally Galaxy Sword. In our discussion of the [legendary weapons](#infinity-1) further below we will see how this has been reimplemented a little differently. In our case, we will not need the Rusty Sword, and so it has been removed entirely, and replaced by a Wooden Blade instead.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Melee Enchantments

Fresh new weapons require fresh new enchantments! Vanilla enchantments have been completely replaced, hopefully providing more interesting gameplay options.

| Name      | Effect |
| --------- | -------|
| Haymaker | *Unchanged from vanilla.* |
| Blasting | Accumulates and stores half of the damage from enemy hits (before mitigation). If enough damage is accumulated, the next special move releases that damage as an explosion. |
| Bloodthirsty | Enemy takedowns recover some health proportional to the enemy's max health. Excess healing is converted into a shield for up to 20% of the player's max health, which slowly decays after not dealing or taking damage for 25s. |
| Carving | Attacks on-hit reduce enemy defense by 1 (continuing below zero). Removes the armor from Armored Bugs and de-shells Rock Crabs. |
| Cleaving | Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies around the target. |
| Energized | Moving and attacking generates energy. When fully-energized, the next attack causes an electric discharge, dealing heavy damage in a large area. |
| Mammonite | Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold. This threshold increases by 1% with each consecutive takedown, resetting when you take damage.* |
| Steadfast | Can no longer critically strike, but multiplies base damage by a factor of crit. power. |
| Wabbajack | Causes unpredictable effects.** |

<font size="1">

\* *Hard caps at 1000 HP. To prevent cheesing boss monsters from expansion mods, this is implemented as a percentage chance per hit, with the chance being near-zero close to the 1000 HP hard cap and near 100% for regular monsters.*

\** *Examples: damage or heal the enemy; decrease or increase the enemie's stats; transfigure into a different enemy, creature or any random item (including illegal items).*
</font>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Ranged Weapon Changes

Ranged weapons are actually quite strong in vanilla, mainly because ammo's deal insane damage. They are also clunky and boring, however, since ranged combat is very unidimensional.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Rebalanced Modifiers & Ammo

Because slingshots [can now critically strike](#critical-strikes), and considering also the addition of [two new slingshot tiers](#infinity-1), we need to tone down their base modifiers to compensate:

- The **Master Slingshot** now deals 50% more damage than the basic slingshot (instead of double). It also increases base knockback by 10%.
- The **Galaxy Slingshot** deals 100% more damage than the basic slingshot (instead of quadruple). It also increases base knockback by 20%.
- The **Infinity Slingshot** caps at 150% extra damage, and a knockback bonus of 25%.

The following ammos have also been tweaked for a bit more immersion and balance:
<details>
<summary>Coal: 15 damage >> 2 damage</summary>

<font size="2">Have you ever held a piece of coal? That stuff is brittle, and weaker than raw wood, so the damage has been reduced accordingly. Not that anybody uses this as ammo anyway.</font>
</details>

<details>

<summary>Explosive Ammo: 20 damage >> 2 damage</summary>

<font size="2">Explosive ammo is meant to be used as a mining utility. There's no reason it should also replace your regular ammo. The explosion damage has not been changed. *Combine it with the [Rascal](../Professions) to efficiently switch between different ammo.*</font>
</details>

The following new ammos have also been added:
- Radioactive Ore: 80 damage
- Gemstones (Ruby, Emerald, etc.): 40 damage
- Prismatic Shard: 60 damage
- Diamond: 100 damage

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Special Move

Pressing the action button will engage gatling mode, enabling auto-fire for up to 3 seconds as long as you keep holding the action key.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Gemstone Forges

Gemstone forges cannot directly be applied to slingshots. But, as seen above, gemstones can be equipped as ammo, and will function as a forge while equipped, applying all corresponding effects. With the [Rascal](../Professions) profession, you may slot up to two gemstones at a time to possibly achieve a level-2 forge.

### Ranged Enchantments

It is significantly harder to create interesting ranged effects compared to melee. Still, the following new enchantments will hopefully be as attractive as the melee ones:

| Name       | Effect |
| ---------- | -------|
| Energized | Moving and shooting generates energy. When fully-energized, the next projectile carries an electric charge, which discharges dealing heavy damage when it hits an enemy. |
| Freljord | Progressively chill enemies on hit for 2 seconds, freezing after stacking 3 times. |
| Quincy | Attacks fire an energy projectile if no ammo is equipped. The projectile is stronger at lower health. Only works when enemies are nearby.* |
| Reverberant | Summons two "echoes" of the fired projectile, that auto-aim at the nearest enemy after a short delay. Only works when enemies are nearby.** |

<font size="1">

\* *Quincy projectile cannot crit nor knock back enemies, but is affected by damage modifiers. Below 2/3 max health, the projectile gains a 50% damage boost, increasing to 100% when below 1/3 (the projectile will change color to reflect these improvements). If [PRFS](../Professions) is enabled and the player has the Rascal profession, Quincy projectiles can be fired even if a different ammo is equipped in the second ammo slot. If the second ammo is a Ruby gemstone, the 10% damage boost will be applied as normal. If the player also has the Desperado profession, the Quincy projectile's size will be increased proportionally by overcharge **instead of** its velocity.*

\** *Additional projectiles inherit 40% of the main projectile's damage, but 100% of its crit. chance, crit. power, knockback and overcharge.*
</font>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Other Features

#### Removed Grace Period

Vanilla slingshots are unable to hit enemies in close-range of the player; these shots will fly straight through them. In order to make slingshots less clunky and significantly more reliable to use, this mod optionally removes this limitation.

#### Snowballs

Standing in a snowy tile with an empty slingshot will allow the player to fire a snowball. The snowball projectile deals no significant damage; this is meant as a fun little flavor feature.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Ring Changes

Only a fool would *not* use a vanilla Iridium Band on every ring slot; it's a free 3-in-1 ring that can also be combined with a fourth. This essentially locks players into pure damage builds, leaving all remaining combat rings unused. We will address these issues by rebalancing some rings and completely overhauling the Iridium Band.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Rebalanced Combat Rings

This following rings have been changed:
- **Warrior Ring:** ~~Chance of giving the Warrior Energy~~ (attack +10 for 5s) ~~buff after slaying a monster.~~ -> Gain a progressively higher attack bonus as you slay enemies (every 3 enemies increases attack by 1), which falls off gradually after some time out of combat.
- **Ring of Yoba:** ~~Chance of giving the Yoba's Blessing~~ (invincible for 5s) ~~buff after taking damage.~~ -> Taking damage that would leave you below 30% health instead grants a shield that absorbs up to 50% of your maximum health for 30s. Cannot be used again until health is fully recovered.
- **Savage Ring:** ~~+2 Speed for 3s after slaying a monster.**~~ -> Gain a rapidly decaying Speed buff after slaying a monster.
- **Ring of Thorns:** Can cause Bleeding* (in addition to reflected damage).

<font size="1">\* *Status effects are explained [further below](#status-effects).*</font>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Craftable Gemstone Rings

All gemstone rings are now craftable at various levels of the Combat skill, requiring the corresponding gemstone and a type of metal bar:

<div algin="center">

| Ring       | Ingredient | Combat Level |
| ---------- | ---------- | ------------ |
| Amethyst   | Copper Bar | 2 |
| Topaz      | Copper Bar | 2 |
| Aquamarine | Iron Bar   | 4 |
| Jade       | Iron Bar   | 4 |
| Ruby       | Gold Bar   | 6 |
| Emerald    | Gold Bar   | 6 |
| Garnet     | Gold Bar   | 7 |
</div>

This addition accompanies some visual changes to each ring to match the color of the required metal bar.

### Infinity Band

Initially, a newly crafted Iridium Band will grant no effects at all; It's merely an ordinary band made of iridium. Only with access to the Forge will you be able to awaken its true form by infusing it with a Galaxy Soul, transforming it into an **Infinity Band**.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/infinity_band.gif" alt="infinity_band.gif" width="67%">
</div>

The Infinity Band likewise does nothing on its own, but it serves as a vessel for up to 4 gemstones of your choice. To add a gemstone to the Infinity Band, you must fuse it with a corresponding gemstone ring at the Forge. The same type of gemstone can be added more than once, compounding the effect. Alternatively, combining different gemstones may lead to powerful [resonances](#garnet--gemstone-resonance-theory).

The Infinity Band cannot be combined with any non-gemstone ring. In most cases, this means that players will now be forced to choose between power and utility, and to strategically carry different types of rings for different situations.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Garnet & Gemstone Resonance Theory

To compensate for the [removal of vanilla Acrobat profession](../Professions), this mod introduces a seventh gemstone, the **Garnet**, which can be mined upwards of Mine level 80. Socketed to a ring or a weapon, it will grant 10% cooldown reduction to special moves. [As shown above](#craftable-gemstone-rings), the Garnet Ring is craftable at Combat level 7.

With the addition of Garnet, the seven gemstones together form a [Diatonic Scale](https://en.wikipedia.org/wiki/Diatonic_scale):

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/Modules/Combat/resources/readme/diatonic.png" alt="diatonic_gemstone_scale.png" width="45%">
</div>

<br>

<div align="center">
<font size="1"><i>The Diatonic Gemstone Scale. The dashed lines show examples of Tertian Tetrad chords rooted in Ruby (red), Aquamarine (blue) and Emerald (green).</i></font>
</div>

<br>

Beginning at the top, the scale progresses clockwise and is cyclic; i.e., after **Rb** comes **Aq**, **Am**, and so on until **Tp**, before again repeating **Rb**.

### Intervals

Like strings in a guitar, each gemstone has a characteristic vibration. When two gemstones are placed side-by-side, these vibrations overlap, causing [interference](https://en.wikipedia.org/wiki/Wave_interference) patterns that can be constructive or destructive. In other words, certain gemstone pairs may amplify each other, while others may instead dampen each other.

A pair of gemstones forms an [Interval](https://en.wikipedia.org/wiki/Interval_(music)). As the name implies, this is simply the distance between the two gemstones in the Diatonic Scale. A distance of 1 is known as a **Second** interval (e.g., from **Rb** to **Aq**), a distance of 2 is known as a **Third** interval (e.g., from **Aq** to **Ga**), and so on. One full rotation of the circle is called an [Octave](https://en.wikipedia.org/wiki/Octave), or [Unison](https://en.wikipedia.org/wiki/Unison) (an interval of zero), denoting the interval between a gemstone and itself.

Notice that, because the scale is cyclic, certain intervals are complementary. For instance, a **Sixth** (e.g., from **Rb** to **Jd**) is a just a **Third** counted backwards (from **Jd** to **Rb**). Likewise for **Second** and **Seventh**. These intervals are essentially equivalent, as shown by their resonances in the table below. The **Fourth** and **Fifth**, while also a complementary pair, are an exception to this rule, and result in different resonances (this is due to some over-simplifications from real life Music Theory).

As a rule of thumb, stones that are positioned farthest from each other in the scale will resonate more strongly, while those positioned adjacent to each other will dissonate. Gemstones do not resonate with themselves.

<div align="center">

| Interval | Resonance | Examples |
| -------- | --------- | -------- |
| Second   | -12.5%    | `Rb - Aq`, `Am - Ga`, `Ga - Em` |
| Third    | 16.6%     | `Rb - Am`, `Am - Em`, `Ga - Jd` |
| Fourth   | 33.3%     | `Rb - Ga`, `Am - Jd`, `Ga - Tp` |
| Fifth    | 50%       | `Rb - Em`, `Am - Tp`, `Ga - Rb` |
| Sixth    | 16.6%     | `Rb - Jd`, `Am - Rb`, `Ga - Aq` |
| Seventh  | -12.5%*   | `Rb - Tp`, `Am - Aq`, `Ga - Am` |
| Octave   | 0         | `Rb - Rb`, `Am - Am`, `Ga - Ga` |
</div>

Clearly, the **Fifth** is the strongest-resonating interval, for which reason it is also known as the **Dominant** interval. The **Fourth**, its complement, is also known as the **Sub-dominant**.

### Chords

When multiple gemstones are placed together, the complex superposition of resonances that results from all possible interval permutations is called a [Chord](https://en.wikipedia.org/wiki/Chord_(music)). Gemstones can only interact in very close proximity, wich means that chords may only be formed by up to 4 gemstones placed together in the same Infinity Band; the chords from different Infinity Bands do not interact.

The gemstone with the highest amplitude in a chord becomes the **Tonic**, or **Root**. All chords with a prominant Root will emit light of a corresponding color and amplitude.

Chords have also an associated **Richness**, which measures how "interesting" it is. A higher richness is achieved by more complex chords (i.e., avoiding repeated gemstones). Some sufficiently rich chords can also exibit **magnetism**.

In case it wasn't clear, **the order in which gemstones are placed in the ring does not matter.**

#### Monad Chords

A 1-note chord is called a **Monad**. A Monad is the simplest possible chord; it is made by simply repeating the same gemstone up to 4 times. Because it only contains Unisons, this chord offers no resonances and zero richness. As a result, it does not emit light, but achieves the highest single-stat total of any chord. The Ruby Monad is shown below:

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/monad.png" alt="monad.png" width="33%">
</div>

#### Dyad Chords

A 2-note chord is called a **Dyad**. A Dyad always contains 2 complementary intervals. Given the table above, it should be clear that the best possible Dyad is the one made from the **Dominant** interval; i.e., a `I - V` configuration, such as `Rb - Em`. This chord contains the intervals Fifth and Fourth (from the inverse, `Em - Rb`), resulting in a +50% resonance for Rb and +33.3% for Em. A double `I - I - V - V` chord is called a [Power Chord](https://en.wikipedia.org/wiki/Power_chord); the simplest resonating chord (and a staple of rock music). The Ruby Power Chord is shown below:

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/dyad.png" alt="dyad.png" width="33%">
</div>

On the other hand, a `I - II` configuration Dyad, like `Aq - Am`, would contain the intervals Second and Seventh (from the inverse, `Am - Aq`), resulting in a strong dissonance, and a dampening of both gemstones.

#### Triad Chords

A 3-note chord is called a **Triad**. A Triad always contains 6 intervals. There are many possible Triad combinations, but only one that avoids dissonances: the [Tertian](https://en.wikipedia.org/wiki/Tertian). A Tertian chord is formed by stacking sequential Third intervals. Notice that the Third of a Third is simply a Fifth (look at the wheel above to convince yourself of this). This means that a Tertian Triad is actually the configration `I - III - V`. 
The Ruby Tertian Triad is shown below:

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/triad.png" alt="triad.png" width="33%">
</div>

Notice also that, due to the cyclic nature of the scale, the `I - III - V` configuration is equivalent to a "shifted" `I - IV - VI`. Take for instance the example of `Em - Rb - Am`, which is a `I - IV - VI` configuration; if we shift all notes one position to the left, then the chord becomes `Rb - Am - Em`, which is a `I - III - V` configuration. The shifting around of notes is known as [Transposition](https://en.wikipedia.org/wiki/Transposition_(music)). This does not change the chord, but allows us to see it from a different perspective.

#### Tetrad Chords

Finally, a 4-note chord is called a **Tetrad**. A Tetrad always contains 12 intervals in total, which makes it impossible find a configuration that avoids any dissonances. But this is okay; if we extend the Tertian Triad by adding another Third interval at the end, we achieve a **Tertian Tetrad**, or `I - III - V - VII` (the `VII` is the Third of the `V`). In this special case, the dissonant Seventh interval becomes resonant, adding +12.5% resonance instead of subtracting it. The Tertian Tetrad achieves the highest possible total resonance, though it forces the distribution of these bonuses among 4 different stats. For the same reason described previously, the configuration `I - II - IV - VI` is equivalent to a transposed Tertian Tetrad. The Ruby Tertian Tetrad is shown below:

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/tetrad.png" alt="tetrad.png" width="33%">
</div>

#### Conclusion

There is no one "optimal" gemstone combination. Simpler chords can be used to optimize for a specific stat, while more complex chords optimize total resonance. As you increase the chord's complexity and richness, you essentially trade higher specific stats, for higher overall distributed stats. This system is intended to encourage experimentation and variety. It is up to each player to choose what best fits their desired build.

### Weapon Forges

If the player's currently held weapon contains forged gemstones, all resonant chords from equipped Infinity Bands will also amplify those gemstone forges which match the chord's root note. Note that forged gemstones do not form chords themselves nor share any of the same resonance and dissonance mechanics from Infinity Bands described above.

<br>

<div align="center">

*It is my hope that this mechanic will encourage experimentation, and also teach some basic Music Theory.*
</div>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Infinity +1

Finally we arrive at the discussion of Legendary weapons, and the most interesting feature of this module.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/Modules/Combat/resources/readme/infinity.png" alt="cursed_event.png">
</div>

According to [TV Tropes Wiki][tropes:infinity+1], an Infinity +1 sword is "not only the most powerful of its kind [...] , but its power is matched by how hard it is to acquire". The vanilla Infinity weapons do not quite fit that definition. Let's fix that, shall we?

To obtain your first Galaxy weapon, as in vanilla you must first unlock the desert, acquire a Prismatic Shard and offer it to the Sand Pillars. Unlike vanilla, however, the weapon will not materialize out of thin air, but will be shaped out of a configurable amount of Iridium Bars (10 by default), which must be in your inventory. This will prevent a lucky Prismatic Shard drop from the Mines or a Fishing Chest from instantly rewarding one of the strongest weapons in the game before the player has even set foot in the Skull Caverns. Now, some venturing into the Skull Caverns is required.

Subsequent Galaxy weapons will no longer be available for purchase at the Adventurer's Guild; one full set, including the slingshot, can now be acquired at the desert, but each weapon will require a larger stack of Prismatic Shards. The order in which the weapons are obtained can be influenced by placing the desired weapon type at the top of the top of the backpack.

Upgrading to Infinity is now a much more involved task, requiring the player to prove they have a virtuous and heroic soul. Doing so will require completion of a new questline revolving around the cursed sword, the Blade of Ruin.

In the interest of avoiding spoilers, the details of the quest can be found hidden in the [FAQ](#faq).

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/cursed_event.png" alt="cursed_event.png" width="67%">
</div>

In return for all that extra work, the Infinity weapons have extra perks:    
1. +1 gemstone slot (4 total). *Keeping in mind that each gemstone has the potential to [resonate](#garnet--gemstone-resonance-theory).*
2. Small boost to the weapon's special move:
	* **Stabing Sword:** Increased dash distance.
	* **Defense Sword:** Parried enemies are dazed for 1 second.
	* **Dagger:** Quick-stab deals one additional hit.
	* **Club:** Smash AoE is 25% larger.
	* **Slingshot**: Auto-fire mode lasts for 1 additional second.
3. **Melee only:** while at full health, every swing fires a mid-range energy beam.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/infinity_beam.gif" alt="infinity_beam.gif" width="40%">
</div>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Status Effects

Taking inspiration from classic RPG and strategy games, this module adds a framework for causing various status conditions to enemies, described below:

| Status | Effect | Sources |
| ------ | ------ | ------- |
| Bleeding | Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (i.e., Dwarven Sentry). | Daggers, Obsidian Edge, Ring of Thorns |
| Burning | Causes damage equal to 1/16th of max health every 3 seconds, and reduces attack by half. Also causes enemies to move about more randomly. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magma Sparkers). Insects burn 4x as quickly. Does not affect Magma Sprites or Lava Lurks | Lava Katana |
| Chilled | Reduces movement speed for the duration. If Chilled is inflicted again during this time, then applies Freeze for 5x the duration. Does not affect Ghosts or Skeleton Mage. | Yeti Tooth, Freljord Enchantment |
| Frozen | Cannot move or attack. The next hit during the duration deals double damage and ends the effect. | Chill x2 |
| Poisoned | Causes damage equal to 1/16 of max health every 3s, stacking up to 3×. Does not affect Ghosts.| *Unused* |
| Slowed | Reduces movement speed for the duration. | Slime Ammo |
| Dazed | Cannot move or attack for the duration. | Enhanced Parry |

Durations depend on the source. These status conditions are exclusively applied to monsters, with two exceptions; a few player-applied status conditions are also tweaked to be more interesting and/or more consistent:

<div align="center">

| Status | Effects | Sources | Duration |
| ------ | ------- | ------- | -------- |
| Burnt | *Same as above.* | Magma Sparker | 15s |
| Frozen | *Same as above.* | Skeleton Mage | 5s |
| Jinxed | Defense -5. Prevents the use of special moves. | Shadow Shaman | 8s |
| ~~Weakness~~ Confusion | Causes unpredictable movement. | Blue Squid | 3s |
</div>

Most status conditions accompany neat new visual and/or sound effects.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/stun_animation.gif" alt="stun_animation.gif" width="67%">
</div>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Enemies

This mod can optionally randomize enemy stats to provide more dynamic encounters. Randomized stats are biased to the player's daily luck, introducing yet another layer to that mechanic. Visiting the Mines on unlucky days will now provide a truly brutal experience.

This mod also provides three sliders to tailor general combat difficulty. These sliders allow scaling monster health, attack and defense.

Finally, certain enemy hitboxes are also improved, and others have received small visual tweaks.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Controls & Quality of Life

This mod includes the following popular control-related features, often featured in other mods.

### Face Mouse Cursor
	
When playing with mouse and keyboard the farmer will always swing their weapon in the direction of the mouse cursor.

### Slick Moves

Swinging a weapon while running will preserve the player's momentum, causing them to drift in the direction of movement. This increases the player's mobility and makes combat feel more fast-paced. 

### Auto-Selection

If enemies are nearby, players can optionally choose a weapon, melee or ranged, to be equipped automatically.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Compatibility

<details>
<summary> 🟩 <b><font color="green">The following mods are compatible:</font></b> 🟩 </summary>

- Compatible with [Advanced Melee Framework][mod:amf] and related content packs, but I do not recommend using any with this module due to inconsistent or unbalanced special moves.
- Compatible with [Stardew Valley Expanded][mod:sve]﻿﻿ and will overwrite the changes to weapons stats from that mod, and also prevent Tempered Galaxy Weapons from appearing in shops. An optional FTM file is available to overwrite SVE's weapon spawns and prevent them from breaking this module's intended balance.
- Compatible with [Better Rings][mod:better-rings], and will use compatible textures if that mod is installed. Credits to [compare123](https://www.nexusmods.com/stardewvalley/users/13917800) for Better Rings-compatible textures.
- Compatible with [Simple Weapons][mod:simple-weapons], and will use compatible textures if that mod is installed.
- Compatible with [Vanilla Tweaks][mod:vanilla-tweaks], and will use compatible weapon textures if that mod is installed.
- Compatible with [Archery][mod:archery] and the accompanying [Starter Pack][mod:archery-starter-pack]. Install the misc. Archery Rebalance file for the complete experience.
- Compatible with [Better Crafting](https://www.nexusmods.com/stardewvalley/mods/11115).
- Compatible with [Wear More Rings](https://www.nexusmods.com/stardewvalley/mods/3214).
</details>

<details>
<summary> 🟥 <b><font color="red">The following mods are NOT compatible:</font></b> 🟥 </summary>

- While the Infinity Slingshot will appear in [CJB Item Spawner][mod:cjb-spawner], it will be incorrectly classified as a Melee Weapon and will be unusable if spawned in this way. This is due to CJB not recognizing non-vanilla slingshots. This likely will be fixed in game version 1.6.
- **Not** compatible with the likes of [Combat Controls][mod:combat-controls] or [Combat Controls Redux][mod:combat-controls-redux], as those features are already included in this and other modules.
- **Not** compatible with other mods that overhaul slingshots, such as [Better Slingshots][mod:better-slingshots] or [Enhanced Slingshots][mod:enhanced-slingshots].
- Weapon rebalance features are **not** compatible with other mods that introduce new weapon types or rebalance weapon stats, such as [Angel's Weapon Rebalance][mod:angels-rebalance].
- New enchantments are **not** compatible with other mods that introduce new enchantments, such as [Enhanced Slingshots][mod:enhanced-slingshots].
- Ring features are **not** compatible with other mods with similar scope, including [Combine Many Rings][mod:combine-many-rings], [Balanced Combine Many Rings][mod:balanced-many-rings] and, to an extent, [Ring Overhaul][mod:ring-overhaul]
- Other ring retextures will be lightly incompatible with the new Infinity Band, meaning there may be some visual glitches but otherwise no real issues.
</details>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## F.A.Q.

<details>
<summary><b>How do I unlock the Dwarven relic weapons?</b></summary>

Have the Dwarvish Translation Guide and at least 6 hearts with Clint, then enter Clint's shop once you have found the first Dwarvish Blueprint. A short cutscene should play, and you will have to wait a random number of days (the higher your friendship points, the shorter the wait). Afterwards, speak to Clint again to unlock the **Forge** option.
</details>

<details>
<summary><b>Where can I find the Dwarvish Blueprints?﻿</b></summary>

They can be found in the exactly the same place as their corresponding weapons would be found in Vanilla (i.e., Volcano chests). The only exception are the Elven blueprints, which can only be obtained from Scavenger Hunts (requires the [Professions](../Professions) module). The corresponding crafting materials are also obtained in the exact same way, as described above in the Weapons section.
</details>

<details>
<summary><b>Why is my Primatic Shard not turning into a Galaxy Sword?</b></summary>

You have the Hero Quest option enabled, and you forgot to bring Iridium Bars.
See section [Infinity +1](#infinity-1).
</details>

<details>
<summary><b>Where can I find the Blade of Ruin?</b></summary>

At the end of the single-floor [Quarry Mine](https://stardewvalleywiki.com/Quarry_Mine) from the statue of the Grim Reaper.
</details>

<details>
<summary><b>What is the Blade of Ruin's curse?</b></summary>

The Blade of Ruin will grow progressively stronger by cosuming enemies; every 5 enemies slain increases its attack power by 1 point. As it grows stronger, however, it will also begin to consume your own energy, dealing damage-over-time while held. At the same time, the Blade has a nasty habit of auto-equipping itself; the stronger the Blade, the more damage you will suffer, and the more often it will auto-equip itself.

Should you choose to ignore these side-effects and continue to strengthen the Blade, you will eventually become unable to use other weapons, and be forced to engage combat with 1 HP.

But fret not, for once a day you may pray to the Altar of Yoba to reduce the intensity of the curse.

</details>

<details>
<summary><b>How do I lift the Ruined Blade's curse?</b></summary>

To begin the quest, you must slay at least 50 enemies with the Blade equipped, prompting the Wizard to invite you over for a chat. To complete this initial quest, simply interact with the Yoba altar and exhaust all possible dialogue choices.
You will then be asked to prove your virtues:
- Prove your Honor, Compassion and Wisdom by selecting certain responses during character heart events.
	- Alternatively, prove your Honor by respectfully returning the Mayor's shorts.
- Prove your Valor by completing monster eradication goals or persevering through long digs in the Mines.
	- **You must speak with Gil to complete an eradication goal.** *This is vanilla guys. I don't know why everybody seems to forget this.*
	- Alternatively, prove your Valor by reaching SVE's [Treasure Cave](https://stardew-valley-expanded.fandom.com/wiki/Treasure_Cave).
- Prove your Generosity by gifting NPCs a certain amount of gold in gifts, or by purchasing Community Upgrades from Robin.
 
Exact completion criteria will depend on your difficulty settings (you can see them in-game in the your quest journal). When you are ready, approach Yoba's altar in Pierre's house with the Blade in hand.
</details>

<details>
<summary><b>What are the IDs of heart events related the Blade of Ruin?</b></summary>

The following events provide chances to demonstrate your virtues. You can use these IDs in conjunction with the `debug ebi <id>` command to replay these events, provided that the Event Repeater mod is installed.

| ID      | Description           | Virtue |
| ------- | --------------------- | ------ |
| 7       | Maru 4 hearts         | Honor  |
| 16      | Pierre 6 hearts       | Honor  |
| 36      | Penny 6 hearts        | Honor  |
| 46      | Sam 4 hearts          | Honor  |
| 58      | Harvey 6 hearts       | Honor  |
| 100     | Kent 3 hearts         | Honor  |
| 288847  | Alex 8 hearts         | Honor  |
| 2481135 | Alex 4 hearts         | Honor  |
| 733330  | Sam 3 hearts          | Honor  |
| 8185291 | Sophia 2 hearts (SVE) | Honor  |
| 13      | Haley 6 hearts               | Compassion |
| 51      | Leah 4 hearts                | Compassion |
| 100     | Kent 3 hearts                | Compassion |
| 288847  | Alex 8 hearts                | Compassion |
| 502969  | Linus 0 hearts               | Compassion |
| 503180  | Pam 9 hearts                 | Compassion |
| 733330  | Sam 3 hearts                 | Compassion |
| 3910975 | Shane 6 hearts               | Compassion |
| 1000005 | Sebastian Mature Event (SVE) | Compassion |
| 1000013 | Caroline Mature Event (SVE)  | Compassion |
| 11      | Haley 2 hearts          | Wisdom |
| 21      | Alex 5 hearts           | Wisdom |
| 25      | Demetrius 3 hearts      | Wisdom |
| 34      | Penny 2 hearts          | Wisdom |
| 50      | Leah 2 hearts           | Wisdom |
| 56      | Harvey 2 hearts         | Wisdom |
| 97      | Clint 3 hearts          | Wisdom |
| 1000018 | Jodi Mature Event (SVE) | Wisdom |
| 1000021 | Jas Mature Event (SVE)  | Wisdom |
| 27      | Sebastian 6 hearts | Honor, Compassion or Wisdom  |
| 3219871 | Claire 2 hearts (SVE)   | Generosity |
</details>

<details>
<summary><b>How do I obtain Infinity weapons?</b></summary>

If the Hero Quest option enabled, Infinity weapons require also require forging a Hero Soul in adddition to the 3 Galaxy Souls.

<details>
<summary><i>To obtain a Hero Soul... (spoiler)</i></summary>

...unforge the Blade of Dawn. Or alternatively, if using the Deep Woods mod, you can also unforge the Excalibur.
</details>
</details>

<details>
<summary><b>How do I obtain other mythic weapons?</b></summary>

- **Neptune's Glaive:** Fishing Chests, same as Vanilla.
- **Yeti Tooth:** Dropped by enemies or crates in the icy section of the Mines.
- **Obsidian Edge:** Dropped from Shadow people in the dangerous Mines.
- **Lava Katana:** Dropped from certain enemies in Volcano Dungeon. Alternatively, from the Treasure Cave in Crimson Badlands.
</details>

<!-- MARKDOWN LINKS & IMAGES -->
[shield:common]: <https://img.shields.io/badge/Common-white?style=flat>
[shield:uncommon]: <https://img.shields.io/badge/Uncommon-green?style=flat>
[shield:rare]: <https://img.shields.io/badge/Rare-blue?style=flat>
[shield:epic]: <https://img.shields.io/badge/Epic-purple?style=flat>
[shield:mythic]: <https://img.shields.io/badge/Mythic-red?style=flat>
[shield:masterwork]: <https://img.shields.io/badge/Masterwork-orange?style=flat>
[shield:legendary]: <https://img.shields.io/badge/Legendary-gold?style=flat>
[shield:rb]: https://img.shields.io/badge/Ruby%20(Rb)-e13939?style=flat
[shield:aq]: https://img.shields.io/badge/Aquamarine%20(Aq)-2390aa?style=flat
[shield:am]: https://img.shields.io/badge/Amethyst%20(Am)-6f3cc4?style=flat
[shield:ga]: https://img.shields.io/badge/Garnet%20(Ga)-981d2d?style=flat
[shield:em]: https://img.shields.io/badge/Emerald%20(Em)-048036?style=flat
[shield:jd]: https://img.shields.io/badge/Jade%20(Jd)-759663?style=flat
[shield:tp]: https://img.shields.io/badge/Topaz%20(Tp)-dc8f08?style=flat

[mod:cjb-spawner]: <https://www.nexusmods.com/stardewvalley/mods/93> "CJB Item Spawner"
[mod:sve]: <https://www.nexusmods.com/stardewvalley/mods/3753> "Stardew Valley Expanded"
[mod:angels-rebalance]: <https://www.nexusmods.com/stardewvalley/mods/6894> "Angel's Weapon Rebalance"
[mod:combat-controls]: <https://www.nexusmods.com/stardewvalley/mods/2590> "Combat Controls - Fixed Mouse Click"
[mod:combat-controls-redux]: <https://www.nexusmods.com/stardewvalley/mods/10496> "Combat Controls Redux"
[mod:amf]: <https://www.nexusmods.com/stardewvalley/mods/7886> "Advanced Melee Framework"
[mod:vanilla-tweaks]: <https://www.nexusmods.com/stardewvalley/mods/10852> "Vanilla Tweaks"
[mod:simple-tweaks]: <https://www.nexusmods.com/stardewvalley/mods/16491> "Simple Weapons"
[mod:better-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/2067> "Better Slingshots"
[mod:enhanced-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/12763> "Enhanced Slingshots"
[mod:combine-many-rings]: <https://www.nexusmods.com/stardewvalley/mods/8801> "Combine Many Rings"
[mod:balanced-many-rings]: <https://www.nexusmods.com/stardewvalley/mods/8981> "Balanced Combine Many Rings"
[mod:ring-overhaul]: <https://www.nexusmods.com/stardewvalley/mods/10669> "Ring Overhaul"
[mod:better-rings]: <https://www.nexusmods.com/stardewvalley/mods/8642> "Better Rings"
[mod:combat-controls]: <https://www.nexusmods.com/stardewvalley/mods/2590> "Combat Controls - Fixed Mouse Click"
[mod:combat-controls-redux]: <https://www.nexusmods.com/stardewvalley/mods/10496> "Combat Controls Redux"
[mod:amf]: <https://www.nexusmods.com/stardewvalley/mods/7886> "Advanced Melee Framework"
[mod:enhanced-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/12763> "Enhanced Slingshots"
[mod:archery]: <https://www.nexusmods.com/stardewvalley/mods/16767> "Archery"
[mod:archery-starter-pack]: <https://www.nexusmods.com/stardewvalley/mods/16768> "Archery Starter Pack"

[tropes:color-coded]: <https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience> "Color-Coded for Your Convenience"
[tropes:infinity+1]: <https://tvtropes.org/pmwiki/pmwiki.php/Main/InfinityPlusOneSword> "Infinity +1"

[🔼 Back to top](#margo--combat-cmbt)