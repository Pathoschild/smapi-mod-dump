/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

==========================================
STARDEW DRUID
==========================================

A druidry themed mod that adds magic mechanics to the core game experience

Requires SMAPI version 3.18.5 +

Optimised for:
- PC
- Single Player
- English

Expected to work without issue on expansion mods and multiplayer

-----------------------------------------
CASTING LOCATIONS
-----------------------------------------

Stardew Druid attempts, when possible, to detect appropriate maps for casting, namely outdoor locations and mineshafts.

No interior locations, except the Greenhouse in limited capacity, enable the player to reach the otherworld to cast rites.

Stardew Druid is not optimised for SDV 1.5 content at this time (It doesn't do much on Ginger Island, nor does it engage with any Island themed content)

Some expansion mods may have exterior maps with names and properties that Stardew Druid is unable to detect, such as "Expansion_cliffside_resort".

SVE and DeepWoods are the only Map expansion mods that Stardew Druid has optimised for at this stage.

As expected Winter is a sad time for Rite of the Earth due to Tree, Crop, Grass and other behaviour.

-----------------------------------------
SWORD - RITE BEHAVIOIUR
-----------------------------------------

Some of the main questlines offer melee weapons that are each aligned with a druidic theme

Forest Sword will always activate Rite of the Earth regardless of what blessing has been chosen with the Effigy

Neptune Glaive and Lava Katana behave in similar fashion for Rite of the Water and Rite of The Stars, respectively

-----------------------------------------
CONFIG FILE
-----------------------------------------

GMCM has been integrated

Rite Buttons "riteButtons" is a list of keybinds set to MouseX1, MouseX2 and LeftShoulder

- Holding the specified control will increase the range of the mechanic, up to ~7.5 tiles radius.

- Keybinds can be changed in mod configuration after installing

-----------------------------------------
Cast Buffs "castBuffs"

The castbuffs ease cast-running, when you cast continuously while running through the map, with three specialised effects
- Enables magnetic buff of radius +3 tiles for 6 seconds during an cast to ease pick up of debris at the outer range of the rite
- Enables speed buff of +2 for 6 seconds during cast while the farmer sprite is on a Grass tile - move through grass with ease!

-----------------------------------------
Consume Roughage, Consume Lunch

When enabled allows automatic consumption of listed items from the inventory when attempting to cast Rites with critically low stamina

Roughage: "consumeRoughage"
Autoconsume usually inedible but often inventory-crowding items: Sap, all TreeSeeds, Slime, Batwings, Red Mushrooms.

Lunch: "consumeQuickSnack"
Autoconsume common sustenance items: Joja Cola, SpringOnion, Snackbar, Mushrooms, Algae, Seaweed, CaveCarrots, Sashimi, Salmonberry, Cheese.

-----------------------------------------
Master Start "masterStart"

Unlocks all levels of all Rites at start of playthrough, and sets the default rite to Earth.
The default is false as we recommend you play Stardew Druid on a new save at Day 1, Spring, Year 1, with a new ability unlocked at the Effigy each day via lessons and quests

-----------------------------------------
Maximum Damage "maximumDamage"

Some spell effects have damage modifiers that consider player combat level, highest upgrade on Pickaxe, Axe, and applied enchantments.
Enable to cast at max damage and effect everytime, as was the case in versions up to 1.1.3

-----------------------------------------
Unrestricted Stars "unrestrictedStars"

Enable to disable the cast buffer on Rite of Stars, so that it casts every button press instead of with reasonable delay.
This was an unintended behaviour of the rite in versions up to 1.1.3

-----------------------------------------

Effigy Location X, etc

Various options to move, activate and hide the Effigy in the farmcave

- Pressing the Rite Button key anywhere in the cave will activate dialogue with the Effigy

-----------------------------------------
(The following config parameters can only be changed in the file)

BlessingList "blessingList"

Determines how many abilities of each rite are unlocked

Earth:

1. Explode weeds and twigs. Greet Villagers, Pets and Animals remotely, once a day
2. Extract foragables from large bushes, wood from trees, fibre and seeds from grass and small fish from water. Might spawn monsters.
3. Sprout trees, grass, seasonal forage and flowers in empty spaces.
4. Increase the growth rate and quality of growing crops. Convert planted wild seeds into random cultivations.
5. Shake loose rocks free from the ceilings of mine shafts. Explode gem ores.

Water:

1. Strike warp shrines once a day to extract totems, and boulders and stumps to extract resources.
2. Strike scarecrows, campfires and lightning rods to activate special functions. Villager firepits will work too..
3. Strike deep water to produce a fishing-spot that yields rare species of fish.
4. Expend high amounts of stamina to smite your enemies.
5. Strike candle torches to create monster portals. Only works in remote outdoor locations.

Stars:

1. Summon meteors (This is the only ability for this rite)

blessingList also contains some other level related values, which include
"levelPickaxe" - the highest upgrade level obtained for a Pickaxe tool, from 0 (Initial) to 5 (Iridium+)
"levelAxe" - as above for Axe tools

-----------------------------------------

QuestList "questList"
Determines what quests are available to complete

False. Quest is not considered complete, and the next available quest in the list will provided by the Effigy when approached
True. Considered complete for purposes of the game

-----------------------------------------
COMPATIBILITY AND SAVE DATA
-----------------------------------------

Custom data model stored in save files
- only affects the mod, so won't impact any save files after uninstalling/installing

Enable and Disable the mod with confidence on any of your save files

Stardew Druid is stable with most popular mods, including
- SVE
- DeepWoods
- Content Patcher
- SpaceCore
- JSON Assets

-----------------------------------------
OPENSOURCE
-----------------------------------------

Opensource repository
https://github.com/Neosinf/StardewDruid
GNU GPL 3 License