====================================
Stardew Druid
====================================

This mod provides an alternative to early tool-based gameplay in the form of four branches of Druidry.
The questline to unlock these branches is provided by a custom NPC, the Effigy, located in the Farmcave, who will offer a new lesson or quest each day. Later in the game a new NPC will appear when the mountain bridge is repaired to provide further challenges.

-----------------------------------------
1. Rite of the Earth
-----------------------------------------
Draw out the hidden bounty of the valley, add trees and grass to barren spaces, enhance the growth rate of crops, explode weeds, twigs, ore veins and trigger rock falls in the mines.
Takes 5 in game days to learn completely.

-----------------------------------------
2. Rite of the Water
-----------------------------------------
Extract power from totem shrines, target scarecrows, rods and campfires for special functions, create high-yield fishing spots, and smite any obstacle in your way. 
Create monster rifts from ordinary candles for a bit of extra challenge and combat experience. 
Takes 5 in game days to learn completely.

-----------------------------------------
3. Rite of the Stars 
-----------------------------------------
Only one effect - call a meteor shower and rain down fire from the sky with reckless abandon.

-----------------------------------------
4. Rite of the Fates
-----------------------------------------
Meddle with the powers of the fates and to warp travel, dazzle enemies and villagers, power machines with essence and create anomalies in reality itself. 
Takes 5 in game days to learn completely.

-----------------------------------------
5. Additional Features
-----------------------------------------
Gently caress your farm animals and neighbours with magic for soft friendship points each day.
Prove yourself in new, unique challenges that test your ability with each rite.
Encounter customised creatures with enhanced stats, cosmetics and behaviour.
Boost your output with the specialised autoconsume feature, unique to Stardew Druid!
Gain a dungeon delving companion and a gardener for your farm.

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

Any of these four tools (Pickaxe, Axe, Hoe, WateringCan) or any Melee Weapon (Including Scythe) must be selected in toolbar to perform a rite (cast)
Some of the main questlines offer melee weapons that are each aligned with a druidic theme
Forest Sword will always activate Rite of the Earth regardless of what blessing has been chosen with the Effigy
Neptune Glaive and Lava Katana behave in similar fashion for Rite of the Water and Rite of The Stars, respectively
Tool upgrades provide extra utility for various rites:
Upgraded Pick, Upgraded Axe:
Higher damage for each upgrade level, greater damage radius of some spells from Gold onwards
Upgraded Hoe:
Ability to spawn artifact spots once Steel Hoe obtained, with increasing chance for each upgrade level
Higher effect radius for Rite of the Earth
Upgraded Watering Can:
Increased radius of scarecrow and lava water effect from each upgrade level after Steel Can
Higher effect radius for Rite of the Earth
Fishing Rods:
Using Bait and Bobbers will increase the proc speed of the Rite of the Water fishspot effect

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
Set Progress "setProgress"
Can be used to override the mod state on game load. -1 is the default for no effective change.
Earth:
0. Quest to gain Rite of the Earth
1. Explode weeds and twigs. Greet Villagers, Pets and Animals remotely, once a day
2. Extract foragables from large bushes, wood from trees, fibre and seeds from grass and small fish from water. Might spawn monsters.
3. Sprout trees, grass, seasonal forage and flowers in empty spaces.
4. Increase the growth rate and quality of growing crops. Convert planted wild seeds into random cultivations.
5. Shake loose rocks free from the ceilings of mine shafts. Explode gem ores.
5. Earth Challenge

Water
6. Quest to gain Rite of the Water
7. Strike warp shrines once a day to extract totems, artifact spots to dig up items, and boulders and stumps to extract resources.
8. Strike scarecrows, campfires and lightning rods to activate special functions. Villager firepits will work too..
9. Strike deep water to produce a fishing-spot that yields rare species of fish, strike lava to create walkable terrain.
10. Expend high amounts of stamina to instantly destroy enemies.
11. Strike candle torches to create monster portals. Only works in remote outdoor locations.
11. Water Challenge

Stars:
12. Quest to gain Rite of the Stars
13. Summon meteors (This is the only ability for this rite)
13. Stars Challenge
14. Four Hidden Challenges!
15. Invite the Effigy to be a passive gardener on your farm

Fates: (Requires quarry bridge repaired)
15. Quest to gain Rite of the Fates and a secret companion
15. Warp move ability. Hold the rite button to teleport to map exits/entrances based on the direction you are facing.
16. Perform Magic tricks for villagers
17. Use Solar and Void essence to power farm machines
18. Create gravity wells to harvest crops and stun enemies
19. Perform warp strikes on dazed enemies
20. Fates Challenge
21. Everything unlocked. Rotating list of quests at secondary difficulties. Secret companion can be invited to join you on adventures!

-----------------------------------------
Monster Difficulty "monsterDifficulty"
Various modes to make mod-spawned monsters harder or easier to handle

-----------------------------------------
Maximum Damage "maximumDamage"
Some spell effects have damage modifiers that consider player combat level, highest upgrade on Pickaxe, Axe, and applied enchantments.
Enable to cast at max damage and effect everytime, as was the case in versions up to 1.1.3

-----------------------------------------
Ostentatious Hats "partyHats"
Enable to put cute hats on some mod-spawned monsters. Cosmetic effect only.

-----------------------------------------
Disable/Enable effects
1. Disable seed harvest from grass
2. Disable fish harvest from nearby water
3. Disable wild monster spawns
4. Disable tree/grass spawning

-----------------------------------------
COMPATIBILITY AND SAVE DATA
-----------------------------------------
Custom data model stored in save files
- only affects the mod, so won't impact any save files after uninstalling/installing
Enable and Disable the mod with confidence on any of your save files
Stardew Druid is stable with most popular mods, including
- SVE
- DeepWoods

-----------------------------------------
OPENSOURCE
-----------------------------------------
Opensource repository
https://github.com/Neosinf/StardewDruid
GNU GPL 3 License