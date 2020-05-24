# Full changelog

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
