**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/QuestFramework**

----

# Changelog

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
