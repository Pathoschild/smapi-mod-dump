====================================
Stardew Druid
====================================

40+ quests, 3 unique NPCs, 25+ abilities, 2000+ lines of dialogue and lore, Stardew Druid is a unique kind of expansion created to satisfy a desire for adventure.

This mod provides an alternative to early tool-based gameplay in the form of five schools of magic, which incorporate motifs and themes inspired by the Druids of antiquity!

The expansive questline starts with a custom NPC, the Effigy, located in the Farmcave, who will initially offer a new lesson or quest each day. The mod includes over ten epic boss battles to test your skills, with a range of configuration options to suit your playstyle. There are 21 lessons in total 

Your journey is recorded in the Stardew Druid journal, a custom journal menu that provides all the instruction and lore you could ask for. 

The mod works primarily though the use of the 'rite button', which is by default configured to keyboard.V, mouse.X4, mouse.X5 and controller.LeftShoulder.

For any mod support, discussion or feedback we recommend you join us on discord as opposed to posting on Nexus. Depending on timezones you might be able to get a response in under a minute!

https://discord.gg/XK29PHPqSg

-----------------------------------------
1. Rite of the Weald
-----------------------------------------
Weald is an old fashioned name for a woodland area, where ancient druids would perform ceremonies honouring the kings of Oak and Holly. This rite is about rewilding the empty spaces of the Valley and drawing on the life essence of the Earth.

Draw out the hidden bounty of the valley, add trees and grass to barren spaces, enhance the growth rate of crops, explode weeds, twigs, ore veins and trigger rock falls in the mines.

-----------------------------------------
2. Rite of the Mists
-----------------------------------------
The isle of mists is an important place in medieval sagas, and the residence of the Lady Beyond the Shore. This rite is about being a commanding presence with awesome displays of power and lots of lightning bolts.

Extract power from totem shrines, target scarecrows, rods and campfires for special functions, create high-yield fishing spots, and smite any obstacle in your way. 
Create monster rifts from ordinary candles for a bit of extra challenge and combat experience. 

-----------------------------------------
3. Rite of the Stars 
-----------------------------------------
Celestial bodies and formations were an important device of storytelling and calendar traditions. That's all very wonderful and this rite is about calling down little stars as fireballs.

Call a meteor shower and rain down fire from the sky with reckless abandon. Call an even bigger comet down when used in conjunction with abilities learned later.

-----------------------------------------
4. Rite of the Fates
-----------------------------------------
The fates are mentioned in many traditions of cultures that were adjacent to or inspired by the Fates of Greek mythology, and are the etymological root for faeries, fae and the like. This rite is about chance and destiny intertwined to produce random effects.

Meddle with the powers of the fates and to warp travel, dazzle enemies and villagers, power machines with essence and create anomalies in reality itself. 

-----------------------------------------
5. Rite of the Ether
----------------------------------------
The fifth element, ether, which inspires a lot of alchemical traditions, is considered to incorporate both the spiritual/divine and the material. The traditional progenitors and agents of this element were... Dragons.

Transform into a Dragon!

-----------------------------------------
+ Additional Features
-----------------------------------------
Gently caress your farm animals and neighbours with magic for soft friendship points each day.
Prove yourself in new, unique challenges that test your ability with each rite.
Encounter customised creatures with enhanced stats, cosmetics and behaviour.
Boost your output with the specialised autoconsume feature, unique to Stardew Druid!
Gain a dungeon delving companion, a treasure hunting friend and a gardener for your farm.

==========================================
SETUP
==========================================

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
Note: Enabling "Cast Anywhere" in configuration will enable rite casting in any location. Have tested, and can confirm, will spawn trees on the beach.

-----------------------------------------
TOOl - RITE Behaviour
-----------------------------------------
Any of these four tools (Pickaxe, Axe, Hoe, WateringCan) or any Melee Weapon (Including Scythe) must be selected in toolbar to perform a rite (cast)
Some of the main questlines offer melee weapons that are each aligned with a druidic theme
Forest Sword will always activate Rite of the Earth regardless of what blessing has been chosen with the Effigy. This will be true for other weapons reference or gained in quests for new rites.

-----------------------------------------
CONFIG FILE
-----------------------------------------
GMCM has been integrated

Rite Buttons "riteButtons" is a list of keybinds set to MouseX1, MouseX2, Keyboard.V and LeftShoulder
- Holding the specified control will increase the range of the mechanic, up to ~7.5 tiles radius.
- Keybinds can be changed in mod configuration after installing

-----------------------------------------
Action Buttons "actionButtons" is a list of keybinds set to MouseLeft, ControllerX and Keyboard.C
This OPTIONAL configuration does not override or edit in-game keybinds for the action controls, instead it just enables the mod to detect when the action/use tool button has been pressed. This is useful in the case that the action button is mapped to a non-conventional input, such as a special button on non-standard controllers or gaming mice.

-----------------------------------------
Special Buttons "specialButtons" is a list of keybinds set to MouseRight, ControllerY and Keyboard.X
This works exactly as above but for the special function. 

-----------------------------------------
Journal Button is a list of keybinds set to K
This OPTIONAL configuration will open the Stardew Druid journal from the game world.

-----------------------------------------
Disable Cast Hands
Stardew Druid employs a 'pause and play single frame animation' for the farmer sprite when the rite button is used to trigger Druid events. This can be disabled to improve compatibility with mods that manage or modify the farmer sprite.

-----------------------------------------
Slot Attunement "slotAttune"
Attunes blessings to slots 1,2,3,4 in the inventory, overrides patron selection and weapon attunement.

-----------------------------------------
Auto Progress "autoProgress"
The mod will load the next lesson/quest at the start of each in game day, without the need to approach any of the quest NPCs.

-----------------------------------------
Colour Preference "colourPreference"
Determines the colour of the sprites used to animate transformations

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
Cast Anywhere "castAnywhere"
Cast any rite effect on any map. May have unintended consequences! Proceed with caution.

----------------------------------------- 
Set Progress "setProgress"
Can be used to override the mod state on game load. -1 is the default for no effective change.


Weald:
0. Discover new NPC "The Effigy"
1. Quest to retrieve a weapon attuned for Rite of the Weald
2. Effect: Explode weeds and twigs. Greet Villagers, Pets and Animals remotely, once a day
3. Effect: Extract foragables from large bushes, wood from trees, fibre and seeds from grass and small fish from water. Might spawn monsters.
4. Effect: Sprout trees, grass, seasonal forage and flowers in empty spaces.
5. Effect: Increase the growth rate and quality of growing crops. Convert planted wild seeds into random cultivations.
6. Effect: Shake loose rocks free from the ceilings of mine shafts. Explode gem ores.
7. Challenge quest for RotWeald

Mists:
8. Quest to gain weapon attuned to Rite of the Mists
9. Effect: Strike warp shrines once a day to extract totems, artifact spots to dig up items, and boulders and stumps to extract resources.
10. Effect: Strike scarecrows, campfires and lightning rods to activate special functions. Villager firepits will work too..
11. Effect: Strike deep water to produce a fishing-spot that yields rare species of fish, strike lava to create walkable terrain.
12. Effect: Strike enemies for massive damage. Triggers a mist zone that buffs defense and provides regeneration.
13. Effect: Strike candle torches to create monster portals. Only works in remote outdoor locations.
14. Challenge Quest for RotMists

Stars:
15. Quest to gain weapon attuned to Rite of the Stars
16. Effect: Summon meteors (This is the only ability for this rite)
17. Challenge Quest for RotStars
18. Challenge Quests for Hidden threats to the valley

Fates: (Requires quarry bridge repaired)
19. Discover new NPC "The Jester of Fate"
20. Quest to gain weapon attuned to Rite of the Fates
21. Effect: Warp move ability. Hold the rite button to teleport to map exits/entrances based on the direction you are facing.
22. Effect: Perform Magic tricks for villagers
23. Effect: Use Solar and Void essence to power farm machines
24. Effect: Create gravity wells to harvest crops and stun enemies
25. Effect: Perform warp strikes on dazed enemies
26. Challenge Quest for RotFates

Ether:
27. Quest to gain weapon attuned to Rite of the Ether
28. Effect: Transform into a Dragon for one minute.
29. Effect: In dragon form, Leftclick/UseTool/Action to fly.
30. Effect: In dragon form, Rightclick/Check/Special to breathe fire.
31. Effect: In dragon form, fly over water to land on the surface, Rightclick to dive for treasure
32. Effect: In dragon form, search large maps for treasure indicators, Rightclick to collect. Provides a specialised journal update.
33. Challenge Quest for RotEther
34. Discover new NPC "Shadowtin Bear"

Companions:
19. When the hidden threats are defeated, Effigy may be recruited on the farm as a gardener
27. When the RotFates challenge is completed, Jester and/or Effigy may join you on your adventures, and each has two powerful attacks to assist you.
27. Companions will offer a rotating list of quests at secondary difficulties. 
35. Shadowtin Bear is available for recruitment. Shadowtin will pick up forageables and dig up artifact spots during idle moments when on adventures.
35. Companion quest "At the Beach" that focusses on Effigy
35. RotWeald Effect: Wisps - unlockable by triggering all the dialogue prompts during Effigy's companion quest.


36. Everything unlocked. 

**COMING SOON** Companion quest "Town Cat, Country Cat" that focusses on Jester

-----------------------------------------
Monster Difficulty "monsterDifficulty"
Various modes to make mod-spawned monsters harder or easier to handle

-----------------------------------------
Maximum Damage "maximumDamage"
Some spell effects have damage modifiers that consider player combat level, highest upgrade on Pickaxe, Axe, and applied enchantments.
Enable to apply max damage everytime

-----------------------------------------
Ostentatious Hats "partyHats"
Enable to put cute hats on some mod-spawned monsters. Cosmetic effect only.

-----------------------------------------
Disable/Enable effects
1. Disable seed harvest from grass, disable Effigy crop seeding
2. Disable fish harvest from nearby water
3. Disable tree spawning and growth upticks
4. Disable grass spawning

-----------------------------------------
MULTIPLAYER
-----------------------------------------
Mod is stable for multiplayer for the entirety of the content up to the conclusion of Rite of the Ether.

-----------------------------------------
COMPATIBILITY AND SAVE DATA
-----------------------------------------
Custom data model stored in save files
- only affects the mod, so won't impact any save files after uninstalling/installing
Enable and Disable the mod with confidence on any of your save files
Stardew Druid is stable with most popular mods, including
- Expansions like SVE, ES and RSV (for now it is not optimised for the custom content)
- DeepWoods
- NPC Adventures
- Immersive Farm
- Farmcave mods
- That modular gameplay alternative mod
- All the major frameworks including SpaceCore, Content Patcher, etc

If there is a mod compatibility issue, come onto discord and log a ticket!

-----------------------------------------
OPENSOURCE
-----------------------------------------
Opensource repository
https://github.com/Neosinf/StardewDruid
GNU GPL 3 License
Please note there is an exception to the license that covers all original artwork (pixel art) that is packaged with the mod.