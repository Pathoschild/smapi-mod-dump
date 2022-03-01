**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/QuestFramework**

----

# Changelog

## 1.4.1

- Recompiled native for SDV 1.5.6

## 1.4.0

This version reworked some C# Quest apis (used by new [Quest Essentials mod](https://www.nexusmods.com/stardewvalley/mods/8827), the QF extension), fixes lot of problems with QF quests, adds fields for mail letters after quest completion and new some new conditions for interact between inventory and quests and current location. Also this upgrades to new Harmony 2.1 and SMAPI 3.12, adds Hungarian and Spanish translations.

- Added Hungarian translation by martin66789 and spanish translation by Carlos
- Updated Harmony references to work with Harmony2.0 (thx shekurika)
- Enhanced logging for loading content packs and exposing quest types
- Fixed minor issue when resizing while CustomBoard is open
- Fixed controller input problem on CustomBoards
- Added better log for used action tiles for custom dropboxes
- Fixed slay monster quest reward problem
- Added new common global conditions: `HasItemInInventory`, `HasActiveQuest` and `CurrentLocation`
- Quest offer manager now internally checks if offering quest is already in log (and offers it ONLY if not)
- Added offering quest by pick up an item
- Reworked NPC quest offer system (optimized)
- Added fields `AddMailOnComplete` and `RemoveMailOnComplete` for content pack quests
- Show talk bubble mouse cursor when quest should be offered on NPC talk
- Added adjust quest trigger when farmer try talk with NPC
- Added `ITalkMessage` interface
- Quests with `BaseType` value `Custom` always returns false for Quest.checkIfComplete and your custom quest is not vanilla base type or doesn't implements `IQuestObserver` interface (for checking completion for custom quests override OnCompletionCheck method on your custom quest type class)
- Update title, objective and description methods are splited from `IQuestObserver` to `IQuestInfoUpdater` (IQuestObserver extends IQuestInfoUpdater)
- `CustomQuest<TState>.Sync` now marks quest for sync at end of update sync instead of sync immediatelly (for immediatelly sync use new API method `CustomQuest<TState>.PerformSync`)
- CustomQuest class now implements IDisposable interface
- Added new extensions: `CheckGlobalConditions`, `CheckGlobalCondition`
- Added new APIs for `CustomQuest<TState>` class: `NeedsSync`, `PerformSync`; and protected virtual methods `OnStateReset`, `OnStateRestored`
- Added new APIs for `CustomQuest` class: `onCompletionCheck`, `OnRegister`, `OnInitialize`, `ForceUpdate`, `OnUpdate`, `OnAdjust`
- Added new managed API: `CheckForQuestComplete`, `AdjustQuest` and `GetAllManagedQuests`
- Added support for offer special order via NPC talk

## 1.3.4

- Fixed problem with quest type id reintegration
- Refactored quest reintegration and better log messages

## 1.3.3

- Optimized NPC offer indicator performance
- Better error message when invalid client connected in multiplayer
- Fixed exception when connected client running without SMAPI or without QF

## 1.3.2

- Fixed problem with special order board in town (hopefully)

## 1.3.1

- Fixed problems with reintegrate quests

## 1.3.0

### Common changes

- Quest bulletin board in town now chooses random quest when is offered more quests of the day in the same day
- Added custom drop box support for special orders
- Added texture loader for content packs (allows load texture added by Content Patcher)
- When NPC offers you a quest via dialogue, an indicator is shown above their head. (You can disable it in config.json)
- Added support for custom quest and special order boards (in any location what you want)
- Managed quests are scrollable in questlog if content needs scroll (like special orders)
- Added support for multiple objectives (for future features)
- Fixed some multiplayer problems

### For Content Pack modders

- Added extra optional field `Secret` for `OfferDetails` of `NPC` quest offer.
- Added field `CustomDropBoxes` for define custom drop boxes for special orders (see docs)
- Added field `CustomBoards` for define custom quest or special order boards (see docs)

### For SMAPI modders

- New managed API method `RegisterCustomBoard`
- New CustomQuest apis: `GetCurrentObjectives`, `UpdateUpdateCurrentObjectives` methods for define multiple objectives for a quest. (You must handle objectives manually in your quest code)

## 1.2.1

### Common changes

- Update to work with Stardew Valley 1.5 (SMAPI 3.8)

## 1.2.0

### Common changes

- Internal hook API is deprecated
- Added support for gain additional friendship by complete a quest
- Added support for custom background texture and font colors for quest details window in quest log menu
- Added support for new types of quest reward: `Object` and `Weapon`
- Fixed some issues with multiplayer
- Write quest type classes in console for `quests_list`, `quests_log` and `quests_customtypes`
- Added new command `quests_customtypes`

### For Content Pack modders

- Added field `FriendshipGain` for define how much friendship points for which NPCs player gains after quest completion
- Added new fields for quest: `RewardType`, `RewardAmount`, `Texture` and `Colors`
- Added support for custom quest types in content packs (via `<modUid>/<questTypeName>` in `Type` field)
- Better error messages

### For SMAPI modders

- Added simple API for mods which wants work with QF and do basic stuff (like resolve quest id from name; check if quest is managed; ...) without referencing dll.
- Added property `ConditionFunc` on `QuestOffer` class for support additional programatic condition
- Added new API on `CustomQuest` class for define how much friendship points for which NPCs player gains after quest completion
- Added new APIs for define quest reward type, custom bg texture nad font colors
- Deprecated `IStateful.ResetState()` and `IManagedQuestApi.GetById` now raise a compilation error when it is used in code
- Added active state
- Added new state interfaces: `IPersistentState` and `IReactiveState`
- Added support for disposable quest state (by implementing `IDisposable` interface)
- Added new APIs for expose custom quest types (class)
- Exposed `ITriggerLoader` interface

## 1.1.1

### Common changes

- Fixed vanilla SDV quest type id resolving (this fixes problem with item harvest quest completion)
- Fixed item harvest quest current objective
- Added command for list raw content from `Data\Quests` (command `quests_list raw`)
- Added korean translation (thx KAYA)

## 1.1.0

### Common changes

- Fixed red alert caused by wrong quest dialogue pattern (Thx @Jonquora for help, issue #16)
- Improved check condition logging.
- Fixed `BuildingConstructed` condition
- Avoid renaming already named custom quests
- Added `HasMod` condition for check if mod with specified mod UID is loaded in SMAPI
- Added comptibility with Expanded Preconditions Utility (`EPU` was added to common conditions)
- Added state verifier (disabled by default)
- Added configuration model for config.json

## For Content Pack modders

- Added support for custom properties for `CustomQuest` from content packs
- Added option of ConversationTopic to add or remove conversation topic when quest accepted, removed, or completed. (thx itsbenter)
- Added support for i18n translations (provided by SMAPI)

## For SMAPI modders

- Added new APIs for `CustomQuest` and statefull `CustomQuest<TState>`
- Added `GetQuestByName` to Managed mod API.
- Added `GetQuestById` as replacement for `GetbyId` (old API method is deprecated and will be removed in one of next versions)

## 1.0.0

### Common changes

- Added new debug commands: `quests_accept`, `quests_complete` and `quests_remove`.
- Fixed wrong quest type binding in asset editor for `Data/Quests`
- Fixed removing quests marked as destroy from questlog
- Added new conditions: `QuestCompletedDate`, `QuestAcceptedToday`, `QuestCompletedToday`, `QuestNeverAccepted`, `QuestNeverCompleted`, 
- Added new condition `SkillLevel` to check if the skill level met the minimum requirement 
- Added new condition `BuildingConstructed` to check if specified building is present on farm 
- Added new condition `IsCommunityCenterCompleted` to check whether community center completed or not
- Added new condition `FriendshipStatus` to check friendship status of specified character (Married, Engaged, Dating, Friendly)
- Rename conditions: `Friendship` to `FriendshipLevel` to distinguish from `FriendshipStatus`

### For Content Pack modders

- Added support for `OR` logical function between values in conditions (with `|` separator)

### For Content Pack modders

- Added support for `OR` logical function between values in conditions (with `|` separator)

### For SMAPI modders

- Added new extensions for class `CustomQuest`: `IsNeverAccepted` and `IsNeverCompleted`

## 1.0.0-beta.3

### Common changes

- Fixed problematic issues with Android
- Added Portuguese translation (thx Randy)

## 1.0.0-beta.2

### Common changes

- Improved Bulletinboard quest offer
- Added `not:` prefix for negate result of checked condition (can be used in hooks and offers and in both programatic and content pack usage)
- Added new conditions for check known recipes: `KnownCraftingRecipe` and `KnownCookingRecipe`
- Added new condition `Random` with random chance in % to offer or execute hook
- Added new condition `QuestAcceptedDate` for check if quest was accepted in specified date
- Added new condition `QuestAcceptedInPeriod` for check if quest was accepted in current season, day, year and other time period(s).
- Added stats support
- Improved some minor refactors

### For SMAPI modders

- Added API method `ForceRefresh()`
- Improved accept quest API which allows add quest quietly (without popup alert message and without "new" flag in questlog)
- Added new global quest events: `QuestCompleted`, `QuestAccepted`, `QuestRemoved`, `QuestLogMenuOpen`, `QuestLogMenuClosed` and `Refreshed`.
- Added new managed API: `ExposeGlobalCondition` for expose global condition(s) for using in quest offers or hooks.
- Added extensions for `CustomQuest` class (use namespace `QuestFramework.Extensions` for apply)

## 1.0.0-beta.1

### Common changes

- Fixed saving game
- Added new global conditions: *MinDaysPlayed*, *MaxDaysPlayed*, *DaysPlayed* and *IsFarmerMarried*
- Added support for daily quests (day limited quests)

### For Content Pack modders

- You can define days left for quests (daily quests)
- Added token for use objects from JsonAssets in quest triggers. 
(example for ItemDelivery quest type: `Willy {{ja:Fish Oil}}`)

### For SMAPI modders

- You can define days left for quests (daily quests) via SMAPI mod-provided API
- Added trigger parser-loader in CustomQuest class

## 1.0.0-beta

- First release
