

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
    "MinimumVersion": "1.0.0-alpha" // optional
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
Name              | required  | (string) Name (UID) of your quest (this never shows to player)
Type              | required  | (string) Vanilla SDV quest type (see quest type)
CustomTypeId      |           | (int) Custom Quest type id (see custom quest types in advanced API). Can be handled by any other mod.
Title             | required  | (string) Quest title in the quest log
Description       |           | (string) Quest description
Objective         | required  | (string) Quest objective
NextQuests        |           | (string[]) One of more next quests which will be added to player's quest log when this quest is completed.
DaysLeft          |           | (int) If this field is filled or is greater than 0, this quest is marked as daily and limited for specified number of days here.
Reward            |           | (int) Reward in SDV currency "golds". If this field is not defined or has value `0`, then player will receive no money reward.
RewardDescription |           | (string) Reward description
Cancelable        |           | (boolean) Can player cancel this quest?
ReactionText      |           | (string) NPC's reaction text when you complete this quest (only for quests which interacts with NPCs)
Trigger           |           | (string) Completion trigger (see quest types for more info) Supports [JSON Assets](#json-assets-support)
Hooks             |           | (Hook) Quest hooks (see hooks for more info)

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
      "ReactionText": "Oh, it's looks delicious. I am really hungry."
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

*Trigger*: `<string:NPC_name> <int:object_id>` like `Abigail 66` for bring Amethyst for Abigail or `Willy {{ja:Fish Oil}}` if you want to use JsonAssets item (JsonAssets required for use JA token)
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

*Trigger*: Custom defined. In JSON api use hooks instead for handle your pure JSON custom quest. If you target a custom quest type defined by any other mod in your JSON content pack, follow instructions of the source mod of this quest type.

## Hooks

You can add some "magic" to your custom quests with hooks. Hooks do specified action with your quests when something was triggered in the game.

Hooks has conditions for check if the action of hook will be triggered or not. There are two types of conditions: local and environment. You don't must specify all available conditions. You can specify conditions which you need.

Field  | Description
------ | -----------
When   | When this hook will be trigered (see hook types)
Action | Which action will be executed (see actions)
Has    | Conditions for execute the hook (see conditions)

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
Remove          | Remove the quest from player's questlog (without completion, just remove)
CheckIfComplete | Handles completion checker and complete the quest if inner quest conditions are met.
Accept          | Accept this quest and add it to player's quest log as new.

### Hook types

#### Location

This hook is triggered when player entered or leaved specific location and all other conditions are met.

Local condition | Description
--------------- | -----------
Enter           | Location name of entered location by player
Leave           | Location name of leaved location

If you specify both, then the hook action will be triggered when player leaved specified location and entered to another specified location. For example:

Enter condition is `Town` and leave condition is `BusStop`. This hook trigs their action when player entered Town from the Bus Stop.

#### Tile

This hook is triggered when player stands on specific tile position or in specific area of specific location.

Local condition | Description
--------------- | -----------
Location        | Game location
Position        | Standing tile position (X, Y). Example `13 10`
Area            | Specified standing area (X, Y, Width, Height) Example: `2038 5573 326 245`
TouchAction     | Touch action property value must be this defined value for trig hook's action.

### Common environment conditions

Condition name  | Example value              | Description
--------------- | -------------------------- | -----------
Weather         | `sunny`                    | Current weather. Allowed values: `rainy`, `snowy`, `stormy`, `cloudy`, `sunny`
Date            | `17 spring`, `5 summer Y2` | Game date in format `<day> <season>` or `<day> <season> Y<year>`
Days            | `2 6 12`                   | Trig action only when today is one of these days. You can specify any count of days.
Seasons         | `summer fall`              | Trig action only when current season is on of these seasons. You can specify any count of seasons.
DaysOfWeek      | `monday wednesday`         | Trig action when today's weekday is one of these weekdays. You can specify any count of weekdays
Friendship      | `Abigail 8`, `Maru 5 Shane 4` | Trig action when friendship heart level is the same as specified value for specified NPC. You can specify more than one friendship conditions.
MailReceived    | `CF_Fish`                  | Trig action when this mail was received by farmer.
EventSeen       | `3910674`                  | Trig action when event with this event id seen by player.
MailNotReceived | `JunimoKart`               | Same as `MailReceived`, but trigs action when specified mail **was not received** by farmer.
EventNotSeen    | `3910674`                  | Same as `EventSeen` but trigs action when event was **not seen yet** by player.
MinDaysPlayed   | `34`                       | Minimum played days from start of new game (from 1 spring year 1)
MaxDaysPlayed   | `51`                       | Maximum played days from start of new game (from 1 spring year 1)
DaysPlayed      | `19`                       | Total played days from start of new game (from 1 spring year 1)
IsPlayerMarried | `yes` or `no`              | Is player married?

## Offers

You can define offers. Offers are descriptions when and by which source your quest will be offered to player for accepting (add to quest log). You can define one ore more offers (for different or for the same quest).

Field          | Description
-------------- | -----------
QuestName      | (string) Quest for schedule to offer (available for add to quest log).
OfferedBy      | (string) Which source offers this quest to accept. (See quest sources bellow)
OfferDetails   | (custom) Details for quest offer. This field is customized. (Optional. See quest sources bellow)
When           | (dictionary<string, string>) When this quest will be offered (see environment conditions).
OnlyMainPlayer | (boolean) Set to true if you want to offer this quest only for main player (server).

### Quests sources

There are available few quests sources provided by native Quest Framework. Some sources must have defined offer details. For some quest sources are different type of offer details.

#### Bulletinboard

Offers quest on bulletinboard on the seeds shop house.

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

NPC can offer your quest via dialogue (speek with them and get a quest)

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

By place `![<questname>@<modUID>]` to the dialogue player get a quest when speak with NPC and this dialogue line is shown.

#### Example

```js
// Some dialogue definition file
{
    "yourDialogueKey": "Can you bring me an Amethyst? ![bringAmethyst@purrplingcat.myquestmod]"
    "anotherDIalogue": "What's up? $h#$b#Are you interested to small fighting adventure?#$b#Bless your sword! ![slayMonsters@purrplingcat.myquestmod]"
}
```
