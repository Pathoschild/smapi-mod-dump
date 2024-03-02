# Full changelog

## Upcoming version

- Exposed fields `KeepFriendship`, `FriendshipPointsLossOnAttack` and `FriendshipPointsLossOnKill` in spiritual skill for content pack creators
- Spouse buff text is more gender neutral
- Added Kent as freelance companion (fighter and shooter)
- Added shooter skill
- Haley got assigned a protector skill
- Added protector skill
- Tweaked fight parameters for Abigail, Alex, Maru, Harvey, Haley, Elliott, Shane, Sebastian and Sam
- Added more companion dialogues
- More immersive adventure acceptance rules for all companions
- Farmer no longer can ask for an adventure when NPC is already sleeping
- Added scared animation for Haley
- Companions having scared skill now can show scared animation frame (optional)
- Added configurable JSON options for scared skill
- Improved requests with shift button config option (Disabled, Enabled or Required)
- Fixed companion hint cursor and their transparency
- Doctor skill can show remaining medkits on their skill icon
- Companion HUD was moved to left side of screen and slighly redesigned
- Redone skill icons
- Added command `npcadventures.inspect <companion>` to inspect companion disposition data
- Added command `npcadventures.companions` to list available companions
- Added command `npcadventures.dumpbag` to immediatelly dump companion bag to farmhouse (for debug)
- Refactored dumping companion bags (hopefuly this fixes spawn bag packade to inacessible place).
  0.x dumped bags are no longer revived, but available as standard chest to avoid item loss.
- Debug commands now uses prefix `npcadventures.` (like `npcadventures.recruit Abigail`)
- Refractored location based dialogues
- Maru now has only two medkits (Harvey still has 3 medkits)
- Added configurable JSON options for doctor skill
- Warrior skill was merged under fighter skill (enable warrior by setting fighter skill option `IsWarrior` to true)
- Added configurable JSON options for fighter skill
- Added freelancers
- Refactored recruitment dialogue
- Added support for individual bed-time for each companion
- Added support for custom adventure accept/reject rules for each companion individually
- Added Charmer skill (desired for RSV)
- Lore content was separated into own Content Patcher content pack
- Added Content Patcher support
- Changed distribution files structure
- Dropped old Content Pack loader (pre-1.0 content packs are no longer loaded)
- Refactored CSM internals
- New companion disposition and content pack format

## 0.17.5 "Blackhole"

- Force sword for level 10 when combat level is out of range (higher than 10)

## 0.17.4 "Blackhole"

- Removed useless harmony patch checker

## 0.17.3 "Blackhole"

- Friendly and safe error when something failed while applying patch by a content pack
- Fixed item loss while dumping companion packages at morning

## 0.17.2 "Blackhole"

- Fixed CurrentController reference (avoid red errors spam when AI init failed - content packs)
- Fixed warp to default NPC location when no schedule after dismiss
- Fixed parsing of arrival schedule time
- Fixed null reference pointer to location in HintDriver
- English corrections in Emily companion event (SandyHouse)

## 0.17.1 "Blackhole"

- Fixed master schedule parse for no schedule

## 0.17.0 "Blackhole"

- Fixed missing strings for spiritual companion class
- Added config option `EnableSuggestions` to enable or disable adventure suggestions by NPCs
- Removed old way to check events
- Fixed horse riding problem when you have a companion

## 0.16.5 "Revival"

- Fixed talk blocking problem with quest

## 0.16.4 "Revival"

- Fixed compatibility for SDV 1.5.6
- Changed license and marked mod as closed-source

## 0.16.3 "Revival"

- Fixed Quest Framework version dependency to 1.4 in manifest.json

## 0.16.2 "Revival"

- Fixes rare error in GetCharacterFromName() postfix (thx shekurika)
- Changed Harmony references to Harmony2.0 (thx shekurika)

## 0.16.1 "Revival"

- Fixed issue with following through cooled lava in volcano
- Added some extra warnings when playing in multiplayer (which is not supported)

## 0.16.0 "Christmas Miracle"

- Added companion event for Emily
- Added companion event for Haley
- Added swimsuits (uses vanilla SDV beach sprites)
- Updated to working with SDV 1.5 (SMAPI 3.8)
- Added spouse recruitment dialogue for most companions
- Added `fisherman` skill for Elliott
- Added support for availability (via EPU condition in companion dispositions file)
- Added translation registry. Translations must have their own `manifest.json` file
- Added support for optional assets in ContentLoader
- Added support for EPU conditions in companion event cutscenes (EPU is required)

## 0.15.0 "Tasty Kimchi"

Released 26th November 2020

- Fixed log name for `lockedDoorWarp` error case
- Added mod-accessible api NpcAdventureModApi (thx aedenthorn aka iLoveLucy)
- Loading of legacy content packs (format 1.2 and older) is now DISABLED by default. (User can enable support for legacy packs in config file)
- Added support for companion events (for future NA cutscenes and for use in content packs)
- Added support for custom weapons for all combat levels (supports JSON assets weapons)
- Redefined swords for (almost) each companion
- Tuned fighting behavior
- Added new companion skill: spiritual - sends love&peace to monsters and they don't attack on farmer
- Reassigned companion skill: Emily is now spiritual and do not fight with monsters
- Added compatibility with PacifistValley mod (needs install compatibility content pack too)
- Added priority field for patches in content packs (format 1.4)

## 0.14.1 "Major Minority"

Released 27th July 2020

- Try to fix cursor fog in mines

## 0.14.0 "Major Minority"

Released 26th July 2020

- Added new hint cursor (on NPC mouse hover) for ask to follow dialog
- Fixed possible exception in Abigail's reaction on interaction with her wardrobe in bathroom.
- Default swords for companions in CompanionDispositions are file are now defined by their name (This adds support for custom swords)
- Player can entry companion's house any time when this companion is recruited.
- Prepare for upcoming features: Mod now is based on Quest Framework (Quest Framework mod is required)
- Movement control was exposed in PurrplingCore for other SMAPI modders without requiring NA (for SMAPI mods devs)
- Internal changes: Changed project structure, move some common parts to PurrplingCore shared VS project (for NA devs only)

## 0.13.0 beta "Useless mod"

Released 21st June 2020

- Removed lovely glowing when farmer recruit married spouse.
- Added random chance for quality forages
- Fixed iridium quality index (#150)
- Hardcoded forage items moved to content file
- Added debug flags for better debugging and testing while development
- Added special farm forages which foragers can find on the Farm (based on farm type).
- Fixed companion bags on Android (#116)
- Fixed unexpected move of recruited spouse in FarmHouse.
- Player can gain extra friendship points every whole hour while adventuring with companions (#109)
- Fixed walk-in-square schedule conflict with companion following.
- Added swimsuit sprites for Abigail, Alex, Emily, Haley, Sam and Sebastian (experimental feature, must be enabled in `config.json`).
- Fixed the "Jesus" bug in the SpA bathroom. Companions can swim (#142)
- Mod's translation was moved to the separated folder `/locale/<localeCode>` and not requires the suffix ~~`.<localeCode>.json`~~
- Introduced new simplified content pack definition format version *1.3*
- Redone content and content pack loader

## 0.12.0 beta "Go out"

Released **22nd May 2020**

- Fighters don't fight with bones of mummy when mummy was "defeated"
- Recruited companion run to farmer when they are too far.
- Optimized companion dialogue fetching and avoid annoing lags while gameplay with recruited companion.
- Redone events checking (added option to enable check NA's events by patched `GameLocation.checkForEvents()` method - EXPERIMENTAL, to enable switch on `Experimental.UseCheckForEventsPatch` in `config.json`) - this fixes re-enter location for trig NA event if more events in stack to play in location.
- Different (customized) bag delivery letters for each companion NPC
- Option for aks for recruit/show companion dialog with hold shift key (optional, disabled by default. To enable switch on `RequestsWithShift`, `RequestsShiftButton` to change requests shift key binding in `config.json`)
- Finally fixed annoying dialogue shown while fighting with companion (disabled by default. To enable switch on option `Experimental.FightThruCompanion` in `config.json`)
- Added critical fist cooldown
- Improved healing by doctors (added medkit power, increased healing cooldown, progressive healing only if healing countdown is not under 10% of their progress)
- Improved fighting (better check leader radius, improved damage monster, less speechbubble spam while fighting)
- Removed outdated and unmaintained localizations: French and Chinese (Chinese is still available as standalone package)
- Fighter and Warrior companion not fight with monsters which are too near farmer
- Wife/Husband companion auto-dismisses at midnight (others still at 10pm)
- Refactored `DialogueHelper` to `DialogueProvider`, added dialogues reload retry (I hope this fixes dialogue loss incident, bug #125)
- Fighter and Warrior checks only valid monsters around to switch a fight mode
- Added debug command `npcadventure_recruit` for instant recruit a companion (only for singleplayer or server and for DEBUG PURPOSES ONLY!)
- Refactored cooldown managing
- Added `scared` skill (this companion occasionally screeches when they see a monster and jump away)
- Leah got `forager` skill
- Added `forager` skill (Companions can forage)
- Refactored and enhanced follow movement system
- Mod attempts to detect potential conflicting patches and inform player in the log (in debug mode as warning, otherwise as info level log)
- Mod uses internal harmony lib in SMAPI (remove 0harmony.dll from the mod folder when upgrading)
- Harmony patching is now more safer (Hope this can help to solve problems with harmony patches on Linux/Mac)
- Changed license

## 0.11.2 beta

Released *17th April 2020*

- Fixed crash on Linux/Mac when mod's quest was opened. (thx @kdau)

## 0.11.1 beta

Released *8th April 2020*

- Fixed nullpointer in HintDriver (#122)
- Fixed Alex's spouse dialogue line
- Fixed problem with loading asset Data/AnimationDescription on linux and mac

## 0.11.0 beta "It's finally here, uf"

Released *22th February 2020*

- Android support (experimanal) (#103 #106) Thx @MaxPresi
- Added special errors and warning log messages for known incompatabilities (#101)
- Changed Marlon's invitaion letter conditions (#95)
- Fixed invalid cast for quests and events assets (#89)
- Added support for localizations in content packs (#83 #89)
- Separated follow movement system (#102)
- *INTERNAL* Added tool for localization coverage reporting

## 0.10.2 beta

Released *2nd February 2020*

- Fixed text deffects for Maru and Marlon's letter (#90)
- Fixed typos and grammar in english texts (#77) Thx to @corrinr
- Marlon's invitation letter will be delivered immediatelly when your day started, if you met all Marlon's conditions and your letter has no been yet. (#92)

## 0.10.1 beta

Released *18th January 2020*

- Fixed angry kiss rejection covered ask2follow dialog. Now dialog shows immediately when player can't kiss.
- Better failsafe when animation behavior got broken animation description (no game crash)

## 0.10.0 beta "New adventure begins"

Released *12th January 2020*

- New documentation (PR #75)
- Changed ContentPack format to version `1.1`
- Update chinese localization
- Better ask to follow compatibility (PR #73)
- Fixed bug with MAIL master schedule parse
- Fixed crash when you try to ask with unmet villager
- Fixed broken vanilla `checkAction` code
- Fixed vanilla functionality of `Game1.getCharacterFromName()`
- Added suggestion dialogues for Abigail, Alex, Haley, Sebastion, Sam, Emily, Elliott, Leah, Harvey and Maru
- NPC can suggest an adventure (requires 7 hearts and more. Can be changed in config) (PR #54)
- Added option (in config file) to disable adventure mode and switch to classic mode (gameplay like in alpha versions)
- You can recruit companion after you seen Marlon's introduction event (for adventure mode)
- Added Marlon's invitation letter
- Added introduction event with Marlon
- Added adventure mode (PR #68)
- Added new dialogue lines for Maru, Shane, Leah, Haley, Emily, Sebastian, Sam, Abigail, Alex, Elliott and Harvey
- Added prosthetic (changeable) buffs for Maru. Hold `G` to change. (PR #72)
- New dialogue engine (PR #70)

## 0.9.0 alpha

Released *31th December 2019*

- Added better compatibility with Custom Kissing Mod by Digus (requires Custom Kissing Mod 1.2.0 or newer) (PR #71)
- Updated Chinese translation

## 0.8.0 alpha

Released *29th December 2019*

- Added Buffs for companions (PR #67)
- Added Maru, Emily and Shane location dialogue lines (PR #69)
- Added support for only once companion dialogues per game (PR #69)
- Added ~ for mark randomized dialogues ($ still works, but deprecated) (PR #69)
- Moved HUD down a little bit
- Shane is no longer warrior, Sebastian got it!
- French and Portuguese translations for Maru

## 0.7.0 alpha

Released *22th December 2019*

- Added chinese translation (by [wu574932346](https://www.nexusmods.com/users/67777356))
- Fixed empty skill in HUD for Leah and Haley

## 0.6.0 alpha

Released *20th December 2019*

- Added new companion location dialogue lines for Abigail, Alex, Haley, Sam and Sebastian (thx Cora Shirou)
- Added French localization (thx [Reikounet](https://www.nexusmods.com/users/70092158))
- Added look around behavior into idle mode (PR #64)
- Enhanced animations in idle mode (PR #64)
- Remaked HUD: Moved to right side on screen (PR #63)
- Remaked HUD: Added companion NPC avatar to HUD (PR #63)
- Remaked HUD: Added status indicator icon (indicates follow, idle and fight mode)
- Companions no longer say hi to monsters when fighting with them (PR #62)
- Don't render HUD when event is up

## 0.5.1 alpha

Released *4th December 2019*

- Fixed null pointer crash in fight with Mummy (#57)

## 0.5.0 alpha

Released *3rd December 2019*

- Added support for content packs and localizations (#50)
- Added user configuration (via `config.json`, see [SMAPI docs](https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started#Configure_mods)) (#56)
- Added portugal translation (by [andril11](https://www.nexusmods.com/users/68848663), thank you!) (#52)
- Use fist once per swing
- Can use fist only on begin of swing
- Avoid damage spam
- Enhance warrior's skills with: 3% added min damage, 2-5% added knock back, 1% added effective area, added crit chance depends on positive daily luck
- Fixed vanilla game nullpointer when try to fight while warping
- Fixed sword swing animation to right layer (#55)
- Fixed bug with Elliott's iddle behavior crash
- Added Harmony patcher (for fix bug #55)

## 0.4.0 alpha

Released *26th November 2019*

- Makes NPC Adventures compatible with SDV 1.4 and SMAPI 3.0

**From this version mod is compatible only with SDV 1.4 and SMAPI 3.0, not with older!**

## 0.3.0 alpha

Released *20th November 2019*

- Different personal skills: warrior, fighter and doctor (next comming soon)
- Doctor can heal you if your health is under 30% and can try to save your life before death
- Warrior can use critical defense fists
- Fighter can level up (syncing level with player) and can upgrade swords
- Display personal skills in HUD (with mouseover tooltip)
- Better critical defense fist fight animation and sound
- Now Shane is warrior!

## 0.2.0 alpha

Released *12th November 2019*

- Idle mode (idle animations) (PR #36)
- Fight speech bubbles (PR #29)
- Refactored reflection (little bit)

## 0.1.0 alpha

Released *31th October 2019*

- Fixed NPC's end of route animation (PR #25)
- Fix English a bit (PR #11)
- **Fighting with swords (PR #17, issue #18)**
- Sword swing animation
- Damage system
- Dynamic weapon cooldown based on random, player's skill and weapon speed
- Better idle transition
- Enhanced fist fight
- Critical fist defense (if a monster is in companion's personal zone)
- Show emote ! when critical fist defense activated
- Better companion dispositions (issue #14, merged in PR #17)
- Removed impossible Abby's buffs (PR #28)
- Changed versioning system (See [wiki Versioning plan](https://github.com/purrplingcat/PurrplingMod/wiki/Versioning-plan))

## 1.0.0-prealpha

Released *22th October 2019*

Only for demo and preview purposes.

- Ask NPC to a follow farmer (5 hearts and more required)
- Recruited NPC can fight with monsters (with fists!)
- Various dialogues for different locations (incomplete yet)
- Can save items from our repository to a npc's backpack
- If you want to break adventure, then you can release a companion
- Next morning you can find a package with your items you saved in companion's backpack
