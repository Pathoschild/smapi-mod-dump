**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/theLion/smapi-mods**

----

<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->
![](https://stardewcommunitywiki.com/mediawiki/images/8/82/Farming_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/2/2f/Mining_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/f/f1/Foraging_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/e/e7/Fishing_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/c/cf/Combat_Skill_Icon.png)

# Walk Of Life

**A Professions Overhaul**

<br/>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#features">Features</a></li>
    <li>
      <a href="#professions">Professions</a>
      <ul>
        <li><a href="#farming">Farming</a></li>
        <li><a href="#foraging">Foraging</a></li>
        <li><a href="#mining">Mining</a></li>
        <li><a href="#fishing">Fishing</a></li>
        <li><a href="#combat">Combat</a></li>
      </ul>
    </li>
    <li><a href="#compatibility">Compatbility</a></li>
    <li><a href="#recommended-mods">Recommended Mods</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#special-thanks">Special Thanks</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

</td></tr></table>


Ever wondered why there aren't any profession overhaul mods on the Nexus? Me too.


## Features

- Rebalanced and reworked [almost] every profession to be an equally unique and attractive choice.
- Each profession targets a specific style of gameplay, some which were not viable in vanilla (i.e. ranching).
- No more ~~boring~~ uninspiring flat value bonuses.
- Bomberman mining; Assassin, Sniper and Slime-army combat.
- Scaling end-game objectives.
- Level 5 professions provide simple early-game buffs that benefit most styles of general gameplay.
- Level 10 professions are more specialized and engaging, providing two bonuses which change the way you play.
- Professions are more consistent across the board, with several analogous perks and synergies.
- Lightweight. This mod is built with a dynamic event handling system to avoid overhead.
- New icons for most professions, courtesy of [IllogicalMoodSwing](https://forums.nexusmods.com/index.php?/user/38784845-illogicalmoodswing/) (please make sure to [endorse their original](https://www.nexusmods.com/stardewvalley/mods/4163) mod).


## Professions

### ![](https://i.imgur.com/p9QdB6L.png) Farming

- **Lv5 - Harvester** - 10% chance for extra yield from harvested crops.
- **Lv10 - Agriculturist** - Crops grow 10% faster. Grow best-quality crops organically without fertilizer.
    - Chance of iridium quality is unchanged from vanilla and is equal to half the chance of gold. Fertilizers will still massively increase that chance.
- **Lv10 - Artisan** - All artisan machines work 10% faster. Ship quality artisan goods to increase brand value over time.
    - Every artisan good shipped is worth a number of points based on its value and quality. Accumulating enough points will have your goods be recognized in the Stardew Artisan Fair's Seasonal Awards. Their stamp on your products can be worth a premium of up to 40% extra value. You will receive a congratulatory mail from the Stardew Artisan's Society upon winning a new award.
- **Lv5 - Rancher** - Befriend animals quicker.
    - Double mood and friendship from petting. Newborn animals are born with some starting friendship.
- **Lv10 - Breeder** - Animals incubate faster and breed more frequently. Increase value of animals at high friendship.
    - Mammals are three times as likely to give birth and oviparous (egg-laying) animals incubate twice as fast. At max friendship animals are worth 2.5x their base price, instead of vanilla 1.3x.
- **Lv10 - Producer** - Happy animals produce twice as frequently. Produce worth 5% more for every full barn or coop.
    - Note that happiness (mood) is not the same as friendship, and that animals cannot produce more than once per day. Bonus sell value also applies to artisan goods derived from animal produce. There is no limit to the scaling.

### ![](https://i.imgur.com/jf88nPt.png) Foraging

- **Lv5 - Forager** - 20% chance for double yield of foraged items.
- **Lv10 - Ecologist** - Wild berries restore 50% more health and energy. Progressively identify forage of higher quality.
    - All foraged items will have the same quality for immediate inventory convenience. Quality starts off at silver and increases when enough items have been foraged.
- **Lv10 - Scavenger** - Location of forageable items revealed. Occasionally track buried treasure.
    - On switching maps while outside you will occasionally detect hidden buried treasure. Find it and dig it up (with a hoe) within the time limit to obtain your reward. The larger your win streak the better your odds of obtaining rare items. Bonus: hold LeftShift (or LeftShoulder on gamepad) to reveal all forageables on-screen.
- **Lv5 - Lumberjack** - Felled trees yield 25% more wood.
- **Lv10 - Arborist** - All trees grow faster and can drop hardwood.
    - Bonus tree growth works as a global buff (applies to all farmhands) and stacks in multiplayer with multiple Arborists.
- **Lv10 - Tapper** - Tappers are cheaper to craft. Tapped trees give syrup 25% faster.

### ![](https://i.imgur.com/TidtIw0.png) Mining 

- **Lv5 - Miner** - +1 ore per ore vein.
- **Lv10 - Spelunker** - Chance to find ladder and shafts increases as you progress down the mines. +1 speed while mining.
    - Bonus is 10% after reaching the bottom of the Mines, and continues to scale indefinitely.
- **Lv10 - Prospector** - Location of ladders and mining nodes revealed. Occasionally track special rocks.
    - Analogous to Scavenger. Will track any mining nodes or mineral forages off-screen with a yellow pointer, and any ladders or shafts with a green pointer. On entering a new mine floor you will occasionally detect stones with prospective treasure. Find the stone and break it within the time limit to obtain your reward. The larger your win streak the better your odds of obtaining rare items. Bonus: hold LeftShift to highlight nodes and ladders on-screen.
- **Lv5 - Blaster** - Bombs are cheaper to craft. Exploding rocks yields twice as much coal.
- **Lv10 - Demolitionist** - Bomb radius +1. Exploding rocks yields 20% more resources.
    - Bonus: get excited when hit by explosions.
- **Lv10 - Gemologist** - Progressively identify gems and minerals of higher quality. Crystalariums work 25% faster.
    - Analogous to Ecologist. All gems and minerals mined from nodes have fixed quality, starting at silver and increasing once enough minerals have been collected. Applies to Crystalariums and Geode Crushers, but not to geodes broken at Clint's.

### ![](https://i.imgur.com/XvdVsAn.png) Fishing

- **Lv5 - Fisher** - Live baits reduce the chance to fish junk items.
- **Lv10 - Angler** - Fish are worth 1% more for every unique max-sized fish caught and 5% more for every legendary fish caught.
- **Lv10 - Aquarist** - Fish pond max capacity +2. Fishing bar height increases for every fish pond at max capacity.
    - Gain 6 pixels per Fish Pond. Every four ponds equal a permanent cork bobber.
- **Lv5 - Trapper** - Crab pots are cheaper to craft. Can trap higher-quality haul.
    - All trapped fish can have quality up to gold. Chance depends on your fishing level.
- **Lv10 - Luremaster** - Crab pots no longer produce junk items. Use different baits to attract different catch.
    - Each type bait will attract different catch:
        - **Regular bait:** 10% chance to catch fish up to level 70. Trappable fish are subject to the same location and season limitations as fishing.
        - **Wild bait:** 10% chance to catch fish up to level 90 (i.e. anything but legendary fish and octopus). 50% chance to double your catch.
        - **Magnet:** repel all fish, but attract metal items dragged by the currents. Traps resources, artifacts, treasure chests, rings and even weapons.
        - **Magic bait:** 25% chance to catch fish above level 70, excluding legendary fish. Removes all location and seasonal restrictions for catching fish. Makes all catch iridium-quality.
- **Lv10 - Conservationist** - Crab pots without bait will trap junk. Remove junk from bodies of water to gain village favor and tax deductions.
    - Every 10 junk items collected from Crab Pots increases friendship with all villagers by 1. Every 100 junk items collected will earn you a 1% tax deduction the following season (max 37%), increasing the value of all shipped goods. You will receive a formal mail from the Ferngill Revenue Service every season informing your tax deduction.

### ![](https://i.imgur.com/fUnZSTj.png) Combat

Level 10 combat professions each introduce a unique stackable attribute that doubles as a resource; performing a special move while holding the Mod Key will trigger an exceed state that rapidly consumes available stacks. Maximum number of stacks is 50.

- **Lv5 - Fighter** - Deal 10% more damage. +15 HP.
- **Lv10 - Brute** - Deal 15% more damage. +25 HP. Hammer kills build Rage, further increasing damage by 0.5% per stack. **Exceed:** Hammer special move cooldown reduced to zero.
- **Lv10 - Gambit** - Higher Crit. Power at high HP, higher Crit. Chance at low HP. Crit. kills build Finesse, granting 2% chance for extra enemy drops per stack. **Exceed:** Dagger special move cooldown reduced to zero.
- **Lv 5 - Rascal** - Slingshots deal up to 50% more damage from afar. 60% chance to recover spent ammo.
- **Lv10 - Desperado** - Shots break into shrapnel, dealing area damage. Precisely-timed shots build Overdrive, increasing fire rate by 2% per stack. **Exceed:** Auto fire a barrage of spread shots.
    - Shots must be fired as soon as they are charged to 
- **Lv10 - Slimed Piper** - Attract more Slimes in dungeons. Slimes damage other monsters. Build Eubstance on contact with Slimes, increasing resistance by 2% per stack. **Exceed:** Rapidly recover health and energy over time.
    - For each Slime in a hutch or outside on your farm, the game will try to spawn an additional Slime whenever a new dungeon floor is being prepared. Slimes will damage any grounded enemies that intersect their hitbox (i.e. bats and serpents are immune). Eubstance stacks are gained whenever the Slimed debuff would be applied. The player will be immune to this debuff.

## Compatbility

- Compatible with [Multi Yield Crops](https://www.nexusmods.com/stardewvalley/mods/6069).
- Compatible with [Capstone Professions](https://www.nexusmods.com/stardewvalley/mods/7636).
- Limited compatibility with [All Professions](https://www.nexusmods.com/stardewvalley/mods/174). (Profession perks will not be applied immediately, but the following morning).
- Limited compatibility with [Skill Prestige](https://www.nexusmods.com/stardewvalley/mods/569#). (Same as above. The Prestige menu also won't reflect modded profession names or descriptions).
- Compatible with any mod that adds SpaceCore custom skills (i.e. [Love Of Cooking](https://www.nexusmods.com/stardewvalley/mods/6830)).
        
- Not compatible with mods that change vanilla skills.
- Not compatible with [Crab Pot Loot Has Quality And Bait Effects](https://www.nexusmods.com/stardewvalley/mods/7767). (Makes Trapper and Luremaster professions redundant).
- Not compatible with [Forage Fantasy](https://www.nexusmods.com/stardewvalley/mods/7554). (May cause bad interactions with Foraging professions).

## Recommended Mods

- Recommended use with [Advanced Casks](https://www.nexusmods.com/stardewvalley/mods/8413). (If you miss Eonologist profession perk).
- Recommended use with [Artisan Valley](https://www.nexusmods.com/stardewvalley/mods/1926).
- Recommended use with [Quality Artisan Products](https://www.moddrop.com/stardew-valley/mods/707502-quality-artisan-products) and [Quality Artisan Products for Artisan Valley](https://www.moddrop.com/stardew-valley/mods/726947-quality-artisan-products-for-artisan-valley).
- Recommended use with [Slime Produce](https://www.nexusmods.com/stardewvalley/mods/7634).

## Installation

- You can install this mod on an existing save; all perks will be applied on loading a saved game.
- To install simply drop the extracted folder onto your mods folder.
- To update make sure to delete the old version first and sthen install the new version.
- There are no dependencies outside of SMAPI.

## Special Thanks

- [Bpendragon](https://www.nexusmods.com/stardewvalley/users/20668164) for [Forage Pointers](https://www.nexusmods.com/stardewvalley/mods/7781).
- [IllogicalMoodSwing](https://forums.nexusmods.com/index.php?/user/38784845-illogicalmoodswing/) for [Profession Icons Redone](https://www.nexusmods.com/stardewvalley/mods/4163).
- [Pathoschild](https://www.nexusmods.com/stardewvalley/users/1552317) for SMAPI support.
- **ConcernedApe** for Stardew Valley.

## License

Distributed under the MIT License. See [LICENSE](../LICENSE) for more information.
