====================================
Stardew Druid Lite
====================================

This mod provides an alternative to early tool-based gameplay in the form of four branches of Druidry.

-----------------------------------------
1. Rite of the Earth
-----------------------------------------

Explode weeds and twigs. Greet Villagers, Pets and Animals remotely, once a day
Extract foragables from large bushes, wood from trees, fibre and seeds from grass and small fish from water.
Sprout trees, grass, seasonal forage and flowers in empty spaces.
Increase the growth rate and quality of growing crops. 
Convert planted wild seeds into random cultivations.
Shake loose rocks free from the ceilings of mine shafts. Explode gem ores.

-----------------------------------------
2. Rite of the Water
-----------------------------------------

Strike warp shrines once a day to extract totems, artifact spots to dig up items, and boulders and stumps to extract resources.
Strike scarecrows, campfires and lightning rods to activate special functions.
Strike deep water to produce a fishing-spot that yields rare species of fish, strike lava to create walkable terrain.
Expend high amounts of stamina to instantly destroy enemies.

-----------------------------------------
3. Rite of the Stars 
-----------------------------------------

Call a meteor shower and rain down fire from the sky with reckless abandon.

-----------------------------------------
4. Rite of the Fates
-----------------------------------------

Warp move ability that can be activated by the 'action' or 'use tool' button after casting
Hold the rite button to teleport to map exits/entrances based on the direction you are facing.
Perform Magic tricks for villagers
Use Solar and Void essence to power farm machines
Create gravity wells to harvest crops and stun enemies
Perform warp strikes on dazed enemies

-----------------------------------------
5. Additional Features
-----------------------------------------
Gently caress your farm animals and neighbours with magic for soft friendship points each day.
Boost your output with the specialised autoconsume feature, unique to Stardew Druid!

==========================================
SETUP
==========================================

Join us on discord
https://discord.gg/XK29PHPqSg

Optimised for:  PC - Single Player - English
SMAPI required
Generic Mod Configuration Menu recommended
Expected to work without issue on multiplayer and with popular content mods

-----------------------------------------
CASTING LOCATIONS
-----------------------------------------

Stardew Druid attempts, when possible, to detect appropriate maps for casting, namely outdoor locations and mineshafts.
No interior locations, except the Greenhouse in limited capacity, enable the player to reach the otherworld to cast rites.
Some expansion mods may have exterior maps with names and properties that Stardew Druid is unable to detect, such as "Expansion_cliffside_resort".
SVE and DeepWoods are the only Map expansion mods that Stardew Druid has optimised for at this stage.
As expected Winter is a sad time for Rite of the Earth due to seasonal changes to Tree, Crop and Grass behaviour.

-----------------------------------------
TOOl - RITE Behaviour
-----------------------------------------

A tool, or any Melee Weapon (Including Scythe) must be selected in toolbar to perform a rite (cast)

-----------------------------------------
CONFIG FILE
-----------------------------------------

GMCM has been integrated
Rite Buttons "riteButtons" is a list of keybinds set to MouseX1, MouseX2, Keyboard.V and LeftShoulder
- Holding the specified control will increase the range of the mechanic, up to ~7.5 tiles radius.
- Keybinds can be changed in mod configuration after installing

-----------------------------------------
Cast Buffs "castBuffs"
The castbuffs ease cast-running, when you cast continuously while running through the map, with three specialised effects
- Enables automatic consumption of various items from inventory when casting with critically low stamina. The items can be of any
  quality but must be in the first/top/upper section of your inventory toolbar - cast without stopping!
- Enables magnetic buff of radius +3 tiles for 6 seconds during an cast to ease pick up of debris at the outer range of the rite
- Enables speed buff of +2 for 6 seconds during cast while the farmer sprite is on a Grass tile - move through grass with ease!

-----------------------------------------
Consume Roughage, Consume Lunch
When enabled allows automatic consumption of listed items from the inventory when attempting to cast Rites with critically low stamina
Roughage: "consumeRoughage"
Autoconsume usually inedible but often inventory-crowding items: Sap, all TreeSeeds, Slime, Batwings, Red Mushroom, Taro Tuber.
Lunch: "consumeQuickSnack"
Autoconsume common sustenance items: SpringOnion, Snackbar, Mushrooms, Algae, Seaweed, CaveCarrot, Sashimi, Salmonberry, Cheese.
Caffeine: "consumeCaffeine"
Autoconsume common speed items: Joja Cola, Coffee items, Tea items, Ginger

-----------------------------------------
Disable/Enable effects
1. Disable seed harvest from grass
2. Disable fish harvest from nearby water
3. Disable tree/grass spawning
