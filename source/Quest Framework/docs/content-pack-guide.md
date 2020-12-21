**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/QuestFramework**

----



← [README](../README.md)

# Content pack guide

aka how to use JSON API for creating content packs with your custom quests.

## Introduction

### What is quest framework?

Quest framework is a "core" mod which provides a tools for work with quests in StardewValley. In the JSON api you can do these things:

- Create custom quests
- Handle quests with predefined hooks

## Define quest

### Prepare content pack

For more info about SMAPI content packs see [How to create content pack](https://stardewvalleywiki.com/Modding:Content_packs)

Create your `manifest.json` file which must contains these minimum contents:

```js
{
  "Name": "Your Project Name",
  "Author": "your name",
  "Version": "1.0.0",
  "Description": "One or two sentences about the mod.",
  "UniqueID": "YourName.YourProjectName",
  "MinimumApiVersion": "3.6.0",
  "UpdateKeys": [],
  "ContentPackFor": {
    "UniqueID": "PurrplingCat.QuestFramework", // Quest Framework unique id must be here
    "MinimumVersion": "1.2.0" // optional
  }
}
```

Then create file `quests.json` and follow next instructions.

### The content pack format

Your `quests.json` has this format:

```js
{
  "Format": "1.0", // this is required
  "Quests": [
    // quests are defined here
  ],
  "Offers": [
    // quest offers are defined here (optional)
  ]
}
```

### Quest basics

In the `Quests` section you can define one ore more custom quests. Quest definition has these fields:

Field             | Required? | Description 
----------------- | --------- | -----------
Name              | required  | `string` Name (UID) of your quest (this never shows to player)
[Type](#quest-types) | required  | `string` Vanilla SDV quest type (see quest type)
CustomTypeId      |           | `int` Custom Quest type id (see custom quest types in advanced API). Can be handled by any other mod.
Title             | required  | `string` Quest title in the quest log
Description       |           | `string` Quest description
Objective         | required  | `string` Quest objective
NextQuests        |           | `string[]` One of more next quests which will be added to player's quest log when this quest is completed.
DaysLeft          |           | `int` If this field is filled or is greater than 0, this quest is marked as daily and limited for specified number of days here.
[Reward](#rewards) || `int|string` Reward in SDV currency "golds". If this field is not defined or has value `0`, then player will receive no money reward.
RewardType        |           | `RewardType` Type of [reward for player](#Rewards). (default: *Money*)
RewardAmount      |           | `int` Amount of reward items. Only for `Object` reward type.
RewardDescription |           | `string` Reward description
Cancelable        |           | `boolean` Can player cancel this quest?
ReactionText      |           | `string` NPC's reaction text when you complete this quest (only for quests which interacts with NPCs)
Trigger           |           | `string` Completion trigger (see quest types for more info) Supports [JSON Assets](#json-assets-support)
[Texture](#colors--texture) || `string` Path to PNG file with custom background texture for this quest in quest log menu. Path is relative to your content pack root directory.
[Colors](#colors--texture)  || `QuestLogColors` Settings of font colors for this quest in the quest log menu.
FriendshipGain    |           | `{[string]: int}` Additional friendship points for enumarated NPCs which player gains after this quest was completed.
[Hooks](#hooks) || `Hook[]` Quest hooks (see hooks for more info)
[ConversationTopic](#conversation-topic) || `ConversationTopic` Add or remove conversation topic (see conversation topic for more info)

#### Example

```js
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "abigail_amethyst", // No id needed, will be automatically generated
      "Type": "ItemDelivery", // Vanilla quest type
      "Title": "The purple lunch",
      "Description": "Abigail are very hungry. She wants to eat something special from mines.",
      "Objective": "Bring amethyst to Abigail",
      "DaysLeft": 5, // If player don't complete this quest until 5 days, this quest will be removed from quest log automatically without completion
      "Reward": 300, // 300g
      "RewardType": "Money",
      "Cancelable": true, // This quest can be cancelled by player
      "Trigger": "Abigail 66", // Bring amethyst to Abby
      "ReactionText": "Oh, it's looks delicious. I am really hungry.",
      "FriendshipGain": {
        "Abigail": 100 // Gain 100 additional friendship points when this quest was completed
      }
    }
  ]
}
```

### Quest types

This part describes only vanilla StardewValley quest types existing in the game. If you want to define custom quests, see the [Advanced API guide](advanced-api-guide.md) or Hooks bellow. For more information about SDV quests see the [Quests wiki](https://stardewvalleywiki.com/Quests) and [Modding quest data](https://stardewvalleywiki.com/Modding:Quest_data)

#### Basic

The basic SDV quest which is handled properly by the game (this is hardcoded in vanilla game).

*Trigger:* No trigger for this quest type

#### Building

Build a building quest. Hardcoded in SDV, but you can define which building type must player build for complete this quest (in the `Trigger` property)

*Trigger*: Building type name (like `Coop`). For available building types see `Data/Blueprints` SDV content resource.

#### Crafting

Craft a specified item.

*Trigger*: `<int:item_id> <boolean:is_bigcraftable>` (like `13 true` for craft furance to complete this quest). 
See the `Data/ObjectInformation` game resource for available items.

#### ItemDelivery

Deliver specified item to specified NPC.

*Trigger*: `<string:NPC_name> <int:object_id> [<int:count>]` like `Abigail 66` for bring Amethyst to Abigail, `Abigail 66 5` for bring 5 pieces of Amethyst to Abigail; or `Willy {{ja:Fish Oil}}` if you want to use JsonAssets item (JsonAssets required for use JA token)
See the `Data/ObjectInformation` game resource for available items.

This quest type accepts `ReactionText`.

#### ItemHarvest

Harvest or pick specific count of specific items.

*Trigger*: `<int:item_id> <int:count>`  like `334 3` for harvest 3 pieces of copper bar to complete this quest.
See the `Data/ObjectInformation` game resource for available items.

#### Location

Go to specified location.

*Trigger*: Location name like `Town` for complete this quest when player enters Town.

#### LostItem

Find a lost item and deliver it to an NPC.

*Trigger*: `<string:NPC_name> <int:item_id> <string:Location_name> <int:TileX> <int:TileY>` like `Robin 788 Forest 110 81` bring Robin Axe to robin which are spawned in Forest on tile location `X=110; Y=81`.
See the `Data/ObjectInformation` game resource for available items.

This quest type accepts `ReactionText`.

#### Monster

Slay a specified count of specified monster.

*Trigger*: `<string:Monster_name> <int:count> [<string:talkWithNpc>]` (monster name can't contain spaces. Replace spaces with `_` in monster name) like `Green_Slime 10` slay 10 Green slimes to complete this quest. `Bat 5 Lewis` means slay five bats and then talk with *Lewis* to complete this quest.

#### SecretLostItem

Same usage as *LostItem*

#### Custom

Custom quest type. You can specify field `CustomTypeId` for more explicit which custom quest type. In JSON api you can use hooks to create custom quest handling.

You can define custom quest via setup value `Custom` into field `Type` or in format `<modUID>/<QuestTypeName>` for custom quests defined by other mods which exposes their quest types for content packs.

*Trigger*: Custom defined. In JSON api use hooks instead for handle your pure JSON custom quest. If you target a custom quest type defined by any other mod in your JSON content pack, follow instructions of the source mod of this quest type.

### Rewards

There are supported some reward types for quests. You can specify reward type in field `RewardType` in your quest definition. Reward is paid to player after quest is completed by clicking the reward in questlog menu (in quest details for completed quest)

Reward type | Description
----------- | -----------
Money       | Amount of money player earn by complete this quest
Object      | Which item player gets by complete this quest. You can specify your item by name (JSON assets items supported) or via their id (integer). Also you can specify amount (stack) of this item in field `RewardAmount`
Weapon      | Which weapon player gets by complete this quest. You can specify your weapon by name (JSON assets items supported) or via their id (integer)

#### Examples

```js
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "abigail_amethyst1",
      "Type": "ItemDelivery",
      "Title": "The purple lunch",
      "Description": "Abigail are very hungry. She wants to eat something special from mines.",
      "Objective": "Bring amethyst to Abigail",
      "DaysLeft": 5,
      "Reward": "Chocolate Cake",
      "RewardType": "Object"
      "RewardAmount": 2, // Player gets two chocolate cakes after complete this quest
      "Cancelable": true, // This quest can be cancelled by player
      "Trigger": "Abigail 66", // Bring amethyst to Abby
      "ReactionText": "Oh, it's looks delicious. I am really hungry."
    },
    {
      "Name": "abigail_amethyst2",
      "Type": "ItemDelivery",
      "Title": "The purple lunch again",
      "Description": "Abigail are very hungry. She wants to eat something special from mines.",
      "Objective": "Bring amethyst to Abigail",
      "DaysLeft": 3,
      "Reward": "Emerald", // Reward is 1x emerald for this quest
      "RewardType": "Object"
      "Cancelable": true, // This quest can be cancelled by player
      "Trigger": "Abigail 66", // Bring amethyst to Abby
      "ReactionText": "Oh, it's looks delicious. I am really hungry."
    },
    {
      "Name": "clint_copperbar",
      "Type": "ItemDelivery",
      "Title": "Clint needs a copper bar",
      "Description": "Clint asked you if you can bring him a copper bar.",
      "Objective": "Bring a copperbar to Clint",
      "Reward": 320, // Reward is 320g
      "RewardType": "Money"
      "Cancelable": true,
      "Trigger": "Clint 334", // Bring copper bar to Clint
      "ReactionText": "Thanks."
    },
    {
      "Name": "marlon_slay_bats",
      "Type": "Monster",
      "Title": "Dangerous mine bats",
      "Description": "Marlon wants help with slay dangerous bats which threaten villagers in the mountains every night",
      "Objective": "Slay 10 bats",
      "DaysLeft": 2,
      "Reward": "Pirate's Sword", // Pirate's Sword is an reward for this quest
      "RewardType": "Weapon"
      "Cancelable": true,
      "Trigger": "Bat 10", // Kill 10 bats
    }
  ]
}
```

### Colors & Texture

Be different! You can specify background texture of quest details window in quest log menu. Just specify an relative path to your content pack root in field `Texture` for your quest. Also you can customize text colors for this quest with field `Colors`.

Color property | Type   | Desription
-------------- | ------ | ----------
TitleColor     | `int`  | Color of quest title.
TextColor      | `int`  | Standard quest color (for decription)
ObjectiveColor | `int`  | Color of quest objective

#### Available colors

Color id | Description
-------- | -----------
0        | Black
1        | Sky Blue
2        | Red
3        | Purple
4        | White
5        | Orange Red
6        | Lime Green
7        | Cyan
8        | Grey

#### Example

```js
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "abigail_amethyst", // No id needed, will be automatically generated
      "Type": "ItemDelivery", // Vanilla quest type
      "Title": "%i18n:abigail_amethyst.title",
      "Description": "Abigail is very hungry. She wants to eat something special from mines.",
      "Objective": "Bring amethyst to Abigail",
      "DaysLeft": 10,
      "Reward": 1000,
      "RewardType": "Money",
      "Cancelable": true,
      "Trigger": "Abigail 66", // Bring amethyst to Abby
      "ReactionText": "Oh, this looks so delicious. I am really hungry, thank you, @!$h",
      "CustomField": "%i18n:custom.field",
      "Texture": "assets/wizardQuest.png",
      "Colors": {
        "TitleColor": 4,
        "TextColor": 4,
        "ObjectiveColor": 1
      }
    }
  ]
}
```

## Hooks

You can add some "magic" to your custom quests with hooks. Hooks do specified action with your quests when something was triggered in the game.

Hooks has conditions for check if the action of hook will be triggered or not. There are two types of conditions: local and environment. You don't must specify all available conditions. You can specify conditions which you need.

Field  | Description
------ | -----------
When   | When this hook will be trigered (see hook types)
Action | Which action will be executed (see actions)
Has    | Conditions for execute the hook (see conditions and hook types).

#### Example

```js
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "my_custom_quest",
      "Type": "ItemDelivery", // Vanilla quest type
      "Title": "The purple lunch",
      "Description": "Abigail are very hungry. She wants to eat something special from mines.",
      "Objective": "Bring amethyst to Abigail",
      "Reward": 300, // 300g
      "Cancelable": true,
      "Trigger": "Abigail 66", // Bring amethyst to Abby
      "ReactionText": "Oh, it's looks delicious. I am really hungry.",
      "Hooks": [
         // Quest will be accepted (added to quest log) 
         // when farmer go near the tree on the west side of graveyard in Town
         // and when is sunny weather today
        {
          "When": "Tile",
          "Has": {
            "Location": "Town",
            "Area": "2038 5573 326 245",
            "Weather": "sunny"
          },
          "Action": "Accept"
        }
      ]
    }
  ]
}
```

### Actions

There are these types of actions:

Action name     | Description
--------------- | -----------
Complete        | Complete the quest
Remove          | Remove the quest from player's questlog (quest is not considered complete, just removed)
CheckIfComplete | Handles completion checker and complete the quest if inner quest conditions are met.
Accept          | Accept this quest and add it to player's quest log as new quest (has the sign new in quest log).

### Hook types

Some hook types has own specific conditions. See them bellow.
**NOTE:** For specialized hook type's conditions you cant't use the negation `not:` prefix. This prefix is only for [common conditions](common-environment-conditions) and for global custom conditions.

#### Location

This hook is triggered when player entered or leaved specific location and all other conditions are met.

Local condition | Description
--------------- | -----------
Enter           | Name of the location the player enter
Leave           | Name of the location the player left

If you specify both, then the hook action will be triggered when player left the Leave location and entered the Enter location. For example:

Enter condition is `Town` and leave condition is `BusStop`. This hook trigs their action when player entered Town from the Bus Stop.

#### Tile

This hook is triggered when player stands on specific tile position or in specific area of specific location.

Local condition | Description
--------------- | -----------
Location        | Game location
Position        | Standing tile position (X, Y). Example `13 10`
Area            | Specified standing area (X, Y, Width, Height) Example: `2038 5573 326 245`
TouchAction     | Touch action property value must be this defined value for trig hook's action.

## Conversation topic

[Conversation topic](https://stardewvalleywiki.com/Modding:Dialogue#Conversation_topics) can make the character speak of certain dialogue when the specific conversation topic is active. Please refer to the conversation topic explanation on the wiki before using this feature. With this field you can make NPCs give dialogue when a quest is accepted, cancelled, and or completed. The respective dialogue itself must be added using [Content patcher](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md#editdata)

### Options 

Valid field              | Example value              | Description
------------------------ | -------------------------- | -----------
AddWhenQuestAccepted     | `AbigailLookingForward 2 CarolineConcerned 5`  | The keys of the conversation topic to be added and its duration after a quest is accepted. In the example after the quest is accepted it will activate two conversation topics: AbigailLookingForward for 2 days and CarolineConcerned for 5 days.
AddWhenQuestRemoved      | `AbigailDisappointed 3`    | The keys of the conversation topic to be added and its duration after a quest is removed. In the example after the quest is removed it will activate a conversation topic: AbigailDisappointed for 3 days.
AddWhenQuestCompleted    | `AbigailSeenEatingRock 10 AbigailHappier 2` | The keys of the conversation topic to be added and its duration after a quest is completed. In the example after the quest is completed it will activate two conversation topics: AbigailDisappointed for 3 days and AbigailHappier 2.
RemoveWhenQuestAccepted | `AbigailLookingForward `    | The key of the conversation topic to be removed. The conversation topic will end before the set when the quest is accepted. In the example after the quest is acappted it will remove the conversation topics: AbigailLookingForward.
RemoveWhenQuestRemoved   | `AbigailLookingForward`    | The key of the conversation topic to be removed. The conversation topic will end before the set when the quest is removed.
RemoveWhenQuestCompleted | `AbigailLookingForward`    | The key of the conversation topic to be removed. The conversation topic will end before the set when the quest is completed.

#### Example

```js
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "abigail_amethyst", // No id needed, will be automatically generated
      "Type": "ItemDelivery", // Vanilla quest type
      "Title": "The purple lunch",
      "Description": "Abigail are very hungry. She wants to eat something special from mines.",
      "Objective": "Bring amethyst to Abigail",
      "DaysLeft": 5, // If player don't complete this quest until 5 days, this quest will be removed from quest log automatically without completion
      "Reward": 300, // 300g
      "Cancelable": true, // This quest can be cancelled by player
      "Trigger": "Abigail 66", // Bring amethyst to Abby
      "ReactionText": "Oh, it's looks delicious. I am really hungry.",
      "ConversationTopic": {
        "AddWhenQuestAccepted": "add_abigail_amethyst_accepted 7", 
		"AddWhenQuestRemoved": "add_abigail_amethyst_removed 7",
        "AddWhenQuestCompleted": "add_abigail_amethyst_completed 7",
        "RemoveWhenQuestAccepted":"remove_abigail_amethyst_accepted", 
		"RemoveWhenQuestRemoved": "remove_abigail_amethyst_removed",
        "RemoveWhenQuestCompleted": "remove_abigail_amethyst_completed",
        // Use only what is needed. The field explain itself.
        // Any string is fine as long as it is unique. You'll need to add the same key to respective character dialogue using content patcher.
	  }
    }
  ]
}
```

## Common global conditions

These global conditions can be used in hooks or in the quest offers.

Condition name           | Example value              | Description
------------------------ | -------------------------- | -----------
Weather                  | `sunny`                    | Current weather. Allowed values: `rainy`, `snowy`, `stormy`, `cloudy`, `sunny`
Date                     | `17 spring`, `5 summer Y2` | Game date in format `<day> <season>` or `<day> <season> Y<year>`
Days                     | `2 6 12`                   | Trig action only when today is one of these days. You can specify any count of days.
Seasons                  | `summer fall`              | Trig action only when current season is on of these seasons. You can specify any count of seasons.
DaysOfWeek               | `monday wednesday`         | Trig action when today's weekday is one of these weekdays. You can specify any count of weekdays.
FriendshipLevel          | `Abigail 8`, `Maru 5 Shane 4` | Trig action when friendship heart level is the same as specified value for specified NPC. You can specify more than one friendship heart level conditions. Replacement for the previous Friendship.
FriendshipStatus         | `Abigail Dating`, `Maru Divorced Shane Married` | Trig action when friendship status is the same as specified value for specified NPC. You can specify more than one friendship status conditions. Allowed status values: `Friendly`, `Engaged`, `Married`, `Divorce`
MailReceived             | `CF_Fish`                  | Trig action when mail with the specified id was received by farmer.
EventSeen                | `3910674`                  | Trig action when event with this event id seen by player.
MinDaysPlayed            | `34`                       | Minimum played days from start of new game (from 1 spring year 1)
MaxDaysPlayed            | `51`                       | Maximum played days from start of new game (from 1 spring year 1)
DaysPlayed               | `19`                       | Total played days from start of new game (from 1 spring year 1)
IsPlayerMarried          | `yes` or `no`              | Is player married?
QuestAcceptedInPeriod    | `season` or `season year` or `today` or `season year weekday` | Checks if this quest was accepted in current specified time period. The input could be combinations of: `day`, `weekday`, `season`, `year`, `date`. Value `season year` means quest was accepted in this year in current season and in any day; `season` means quest was accepted in current season in any year and any day; `today` means quest was accepted just today.
QuestAcceptedDate        | `17 spring`, `5 summer Y2` | Checks if this quest was **accepted** in specified date. Game date in format `<day> <season>` or `<day> <season> Y<year>`
QuestCompletedDate       | `17 spring`, `5 summer Y2` | Check if this quest was **completed** in specified date. Game date in format `<day> <season>` or `<day>`
QuestAcceptedToday       | `yes` or `no`              | Check if this quest was (or wasn't) **accepted today**
QuestCompletedToday      | `yes` or `no`              | Check if this quest was (or wasn't) **completed today**
QuestNeverAccepted       | `yes` or `no`              | Check if this quest was (or wasn't) **never accepted yet**
QuestNeverCompleted      | `yes` or `no`              | Check if this quest was (or wasn't) **never completed yet**
SkillLevel               | `Farming 1`, `Foraging 2 Fishing 3 Mining 2`   | Check if player skill level equal or higher than what is defined. Allowed skill values: `Farming`, `Fishing`, `Foraging`, `Mining`, `Combat`, `Luck`
IsCommunityCenterCompleted | `yes` or `no`              | Check if community center is already completed.
BuildingConstructed      | `Coop` or `Deluxe_Coop Well Coop` | Check if specified building is currently present in farm.
KnownCraftingRecipe      | `Furnace`                  | Player knows specified crafting recipe.
KnownCookingRecipe       | `Fried Egg`                | Player knows specified cooking recipe.
HasMod                   | `PurrplingCat.NpcAdventure` | Checks if mod with specified mod UID(s) are loaded in SMAPI. You can put here one or more mod UIDs.
Random                   | `52` or `22.272`           | A random chance in % (0 - 100). Number `52` means 52% of chance, number `22.272` means 22.272% of chance.
EPU                      | EPU string like `!z spring/t 600 1000` | Condition processed by [Expanded Preconditions Utility](https://www.nexusmods.com/stardewvalley/mods/6529). For use this condition, EPU must be installed in SDV mods folder. See [EPU docs](https://github.com/ChroniclerCherry/stardew-valley-mods/blob/master/ExpandedPreconditionsUtility/README.md) for more information.

Every condition name enlisted in this common conditions list you can prefix with `not:` for negate condition result.
For example: `not:EventSeen` means event with specified id was not seen by player;

Also you can chain condition values with `OR` logic function with character separator `|`:

```js
{
  "Weather": "rainy | snowy", // Means current weather is 'rainy' OR 'snowy'
  "Date": "6 summer Y1 | 19 fall Y1 | 24 spring Y3", // Means current date is '6th summer year 1' OR '19th fall year 1' OR '24th spring year 3'
  "EventSeen": "335478 | 44125", // Means player seen event ID 335478 OR event ID 44125
  "not:MailReceived": "ccComplete | jojaMember" // Player NOT received mail 'ccComplete' OR 'jojaMember'
}
```

## Offers

You can define offers. Offers are descriptions of when and by which source your quest will be delivered to player to accept (add to quest log). You can define one or more offers (for different or for the same quest).

Field          | Description
-------------- | -----------
QuestName      | (string) Quest for schedule to offer (available for add to quest log).
OfferedBy      | (string) Which source offers this quest to accept. (See quest sources bellow)
OfferDetails   | (custom) Details for quest offer. This field is customized. (Optional. See quest sources bellow)
When           | (dictionary<string, string>) When this quest will be offered (see environment conditions).
OnlyMainPlayer | (boolean) Set to true if you want to offer this quest only for main player (server).

### Quests sources

These are the available quests sources provided by native Quest Framework. Some sources require defined offer details. There are different offer details based on the source.

#### Bulletinboard

Offers quest on bulletinboard located in front of the seeds shop (Pierre).

*This source NOT requires or accepts any offer details*

**Example**
```js
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "abigail_amethyst", // No id needed, will be automatically generated
      "Type": "ItemDelivery", // Vanilla quest type
      "Title": "The purple lunch",
      "Description": "Abigail are very hungry. She wants to eat something special from mines.",
      "Objective": "Bring amethyst to Abigail",
      "Reward": 300, // 300g
      "Cancelable": true,
      "Trigger": "Abigail 66", // Bring amethyst to Abby
      "ReactionText": "Oh, it's looks delicious. I am really hungry.",
    }
  ],
  "Offers": [
    {
      "QuestName": "abigail_amethyst",
      "OfferedBy": "Bulletinboard",
      "When": {
        "DaysOfWeek": "Monday Thursday Friday",
        "Seasons": "spring fall",
      }
    }
  ]
}
```

#### NPC

NPC can offer your quest via dialogue (speak with them and get a quest)

**Requires these offer details**
Field        | Description 
------------ | -----------
NpcName      | (string) Which NPC offers this quest.
DialogueText | (string) What to NPC say to offer this quest.

**Example**
```js
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "abigail_amethyst", // No id needed, will be automatically generated
      "Type": "ItemDelivery", // Vanilla quest type
      "Title": "The purple lunch",
      "Description": "Abigail is very hungry. She wants to eat something special from mines.",
      "Objective": "Bring amethyst to Abigail",
      "Reward": 300, // 300g
      "Cancelable": true,
      "Trigger": "Abigail 66", // Bring amethyst to Abby
      "ReactionText": "Oh, this looks so delicious. I am really hungry, thank you, @!$h"
    }
  ],
  "Offers": [
    {
      "QuestName": "abigail_amethyst",
      "OfferedBy": "NPC",
      "OfferDetails": {
        "NpcName": "Abigail", // Speak with Abigail to get this quest
        "DialogueText": "I have a craving for something special.#$b#@, can you bring me amethyst?"
      },
      "When": {
        "DaysOfWeek": "Monday Thursday Friday",
        "Seasons": "spring fall"
      }
    }
  ]
}
```

#### Mail

Farmer can get a quest via received letter in their mailbox on the farm.

**Requires these offer details**
Field        | Description 
------------ | -----------
Topic        | (string) Title or topic of the letter (optional)
Text         | (string) Text of the quest source letter (required)

**Example**
```js
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "bat_problem",
      "Type": "Monster",
      "Title": "The bat problem",
      "Description": "Bats are attacking the town every night. Slay some bats and make town more safe.",
      "Objective": "Slay 10 bats",
      "Reward": 500, // 500g
      "Cancelable": false,
      "Trigger": "Bat 10 Lewis", // Slay 10 bats and then talk with Lewis
      "ReactionText": "Good job, @. Our town is safe for this time.#$b#Bats are big problem after mines in mountains are open again. We must be carefull."
    }
  ],
  "Offers": [
    {
      "QuestName": "bat_problem",
      "OfferedBy": "Mail",
      "OfferDetails": {
        "Topic": "The bat problem",
        "Text": "Hi, @,^Our town is not safe during nights, because agressive kind of bats attacking villagers and they are scared. Reduce the bat population and make this town safe again. Thanks ^   -Mayor Lewis"
      },
      "When": {
        "Weather": "cloudy",
        "Seasons": "spring summer",
        "Days": "4 11 24"
      }
    }
  ]
}
```

## Compatibility with other mods

### Json Assets support

Quest Framework is compatible with JsonAssets mod. The framework supports JsonAssets objects for the `Trigger` quest field via token named `ja` and supports two item types: `object` and `bigcraftable`.

Token looks like: `{{ja: <itemName> [|bigcraftable]}}`

If you add suffix `|bigcraftable` after item name, then Quest Framework looks in the BigCraftable sheet. Otherwise looks in Object sheet.

#### Example

```js
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "willy_fish_oil", // No id needed, will be automatically generated
      "Type": "ItemDelivery", // Vanilla quest type
      "Title": "Fish Oil needed",
      "Description": "Hi. I need a fish oil. ASAP please.\n                                - Willy",
      "Objective": "Bring Fish Oil to Willy",
      "Reward": 220, // 220g
      "Cancelable": true,
      "Trigger": "Willy {{ja:Fish Oil}}", // Bring Fish Oil to Willy (this item is from JA items)
      "ReactionText": "This one smells very intensive! Thank you so much, @!$h"
    }
  ]
}

```

## Outbound

### Add quest via NPC dialogue (external)

You can add known quest in Quest Framework via NPC dialogues. You can define it with content patcher (Patch `Dialogue/<NPC_name>`) and add your dialogue which adds a quest to the player's questlog or edit existing dialogue line.

By place `[quest:<questname> <modUID>]` to the dialogue player get a quest when speak with NPC and this dialogue line is shown.

#### Example

```js
// Some dialogue definition file
{
    "yourDialogueKey": "Can you bring me an Amethyst? [´quest:bringAmethyst purrplingcat.myquestmod]"
    "anotherDIalogue": "What's up? $h#$b#Are you interested to small fighting adventure?#$b#Bless your sword! [quest:slayMonsters purrplingcat.myquestmod]"
}
```
