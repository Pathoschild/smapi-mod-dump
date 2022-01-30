**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/StardewMods**

----

# Adventure Quests

Quest type: `PurrplingCat.QuestEssentials/Adventure`
Adventure quest fields:
| Field | Description |
| ----- | ----------- |
| Tasks | Array of quest tasks aka dynamic objectives (see bellow) |

Also Adventure quest type quest includes all standard quest fields from Quest Framework's quest.

**Example**

```js
{
  "Name": "TestStoryQuest",
  "Type": "PurrplingCat.QuestEssentials/Adventure",
  "Title": "Testing story quest",
  "Description": "This is an testing adventure quest",
  "Tasks": [
    {
      "Name": "TestTask3",
      "Type": "Collect",
      "Description": "Gather bug meat",
      "Data": {
        "AcceptedContextTags": "item_bug_meat" // same format as for Special Orders
      },
      //"RequiredTasks": [ "TestTask1", "TestTask2" ],
      "Goal": 10
    },
    {
      "Name": "TestTask4",
      "Type": "Craft",
      "Description": "Bake a cake",
      "Data": {
        // Cook any cake
        "AcceptedContextTags": "food_sweet, food_bakery"
      },
      "RequiredTasks": [
        "TestTask3"
      ],
      "Goal": 7
    },
    {
      "Name": "TestTask5",
      "Type": "Deliver",
      "Description": "Bring baked cake to Abigail",
      "Data": {
        // Deliver any cake to Abigail
        "NpcName": "Abigail",
        "AcceptedContextTags": "food_sweet, food_bakery",
        "Message": "Thank you for this delicious cake you made for me!$l",
        "NotEnoughMessage": "Thank you! I still need {0} more of these, though!$h"
      },
      "RequiredTasks": [
        "TestTask3"
      ],
      "Goal": 7
    },
    {
      "Name": "TestTask6",
      "Type": "Talk",
      "Description": "Talk with Abigail",
      "Data": {
        // Talk to abby and then see an event
        "NpcName": "Abigail",
        "DialogueText": "I am thinking about go to an adventure in mines. Can I go with you if you go to the mines some day?#$b#Take an ancient fruit from me",
        "StartEvent": "44017601 Data\\Events\\FarmHouse:3917601/f Emily 3500/O Emily/n emilyFiber/A emilyFiber/t 2000 2400/p Emily",
        "ReceiveItems": "object_ancient_fruit" // And get ancient fruit from Abby when talk
      },
      "RequiredTasks": [
        "TestTask5",
        "TestTask4"
      ]
    },
    {
      "Name": "TestTask7",
      "Type": "Slay",
      "Description": "Kill some slimes or bats",
      "Data": {
        "TargetName": "Slime, Bat, Jelly, Sludge"
      },
      "Goal": 20,
      "RequiredTasks": [
        "TestTask6"
      ]
    },
    {
      "Name": "TestTask8",
      "Type": "Fish",
      "Description": "Catch any ocean summer fish",
      "Data": {
        "AcceptedContextTags": "fish_ocean, season_summer"
      },
      "RequiredTasks": [
        "TestTask7"
      ],
      "Goal": 5
    },
    {
      "Name": "TestTask9",
      "Type": "Gift",
      "Description": "Give a someone gift",
      "Data": {
        "MinimumLikeLevel": "Liked"
      },
      "RequiredTasks": [
        "TestTask8"
      ],
      "Goal": 5
    }
  ],
  "AddMailOnComplete": "happy_abby noletter",
  "FriendshipGain": {
    "Abigail": 320
  },
  "Reward": 6000,
  "Cancelable": true
}
```

## Tasks

Common task fields

| Field | Type | Description |
| ----- | ---- | ----------- |
| Name  | string | (required) Unique task name (in the quest scope)
| Type  | string | (required) Task type (see bellow)
| Description | string | (required) Task description. Visible in quest objective
| When | dictionary | Pre-conditions must be matched for successful completion of this task
| RequiredTasks | string[] | Other tasks must be completed to activate this task. If some tasks defined here are not completed, this task is not shown in the quest objective list and can't be completed.
| Count | int | How much pieces of subject of this task (by type) must be reached to complete this task. (default = 1)
| Data | object | (depends on task type) An additional data for a task. See task types docs bellow.

### Basic

| Field | Type | Description |
| ----- | ---- | ----------- |
| Trigger | string | Trigger string to complete this task

This task type doesn't specify `Data` field. The `Trigger` field is on the task directly (not wrapped by data field).

### Collect

Collect specified items.

These fields for this task type are under the `Data` field:

| Field | Type | Description |
| ----- | ---- | ----------- |
| AcceptedContextTags | string | The context tags for item to collect. The format is the same as for Special Orders in vanilla SDV

```js
{
  "Name": "TestTask3",
  "Type": "Collect",
  "Description": "Gather bug meat",
  "Data": {
    "AcceptedContextTags": "item_bug_meat" // same format as for Special Orders
  },
  "Count": 10
}
```

### Craft

Craft specified items.

These fields for this task type are under the `Data` field:

| Field | Type | Description |
| ----- | ---- | ----------- |
| AcceptedContextTags | string | The context tags for item to craft. The format is the same as for Special Orders in vanilla SDV

```js
{
  "Name": "TestTask4",
  "Type": "Craft",
  "Description": "Bake a cake",
  "Data": {
    // Cook any cake
    "AcceptedContextTags": "food_sweet, food_bakery"
  },
  "Count": 7
}
```

### Deliver

Deliver an item to an NPC.

These fields for this task type are under the `Data` field:

| Field | Type | Description |
| ----- | ---- | ----------- |
| NpcName | string | A name of an NPC accepts your item delivery
| AcceptedContextTags | string | The context tags for item to delivery. The format is the same as for Special Orders in vanilla SDV
| NotEnoughMessage | string | A dialogue to be said by NPC when you deliver a part of requested count of requested item. Placeholder `{0}` in dialogue string represent how many remains to deliver to complete this task. Placeholder is optional.
| Message | string | A dialogue to be said by NPC when you deliver all requested count of item and the task is completed.

```js
{
  "Name": "TestTask5",
  "Type": "Deliver",
  "Description": "Bring baked cake to Abigail",
  "Data": {
    // Deliver any cake to Abigail
    "NpcName": "Abigail",
    "AcceptedContextTags": "food_sweet, food_bakery",
    "Message": "Thank you for this delicious cake you made for me!$l",
    "NotEnoughMessage": "Thank you! I still need {0} more of these, though!$h"
  },
  "RequiredTasks": [ "TestTask3" ],
  "Count": 7
}
```

### Enter Spot

Stand on a tile or in specific area in a location.

These fields for this task type are under the `Data` field:

| Field | Type | Description |
| ----- | ---- | ----------- |
| Tile  | `<x>,<y>` | A point of tile on the map to stand on.
| Area  | `<x>,<y>,<width>,<height>` | An area rectangle of pixels on the map to stand inside.
| EventOnComplete | `<int:EventId> <string:pathToScript>` where path to script must be `<string:GameContentFile>:<string:Key>` | An [event](#events) to be started when this task is completed

**Example**

```js
{
  "Name": "EnterSpotTask1",
  "Type": "EnterSpot",
  "Data": {
    "Tile": "14,15",
    "Location": "FarmHouse",
    "EventOnComplete": "4725065 Strings\\Locations:IslandSecret_Event_BirdieIntro"
  },
  "Description": "%i18n:story_quest.EnterSpotTask1.description"
},
```
```js
{
  "Name": "EnterSpotTask2",
  "Type": "EnterSpot",
  "Description": "Enter your kitchen",
  "Data": {
    "Area": "64,768,576,704",
    "Location": "FarmHouse"
    }
}
```

### Fish

Catch a fish or another fishable item in the water.

| Field | Type | Description |
| ----- | ---- | ----------- |
| AcceptedContextTags | string | The context tags for fish to catch. The format is the same as for Special Orders in vanilla SDV

**Example**

```js
{
  "Name": "FishTask8",
  "Type": "Fish",
  "Description": "Catch any ocean summer fish",
  "Data": {
    "AcceptedContextTags": "fish_ocean, season_summer"
  },
  "Count": 5
}
```

### Gift 

Give a present to an NPC villager in Stardew Valley.

| Field | Type | Description |
| ----- | ---- | ----------- |
| NpcName | string | A target villager NPC name receives your gift
| MinimumLikeLevel | `None`, `Hated`, `Disliked`, `Neutral`, `Liked`, `Loved` | A minimum like level required to complete this task
| AcceptedContextTags | string | The context tags for fish to catch. The format is the same as for Special Orders in vanilla SDV

**Example**

```js
{
  "Name": "GiftTask9",
  "Type": "Gift",
  "Description": "Give a someone gift",
  "Data": {
    "MinimumLikeLevel": "Liked"
  },
  "Count": 5
}
```

### Slay

Defeat a monster in caves or another place contains monsters except the farm.

| Field | Type | Description |
| ----- | ---- | ----------- |
| TargetName | string | A name of a target monster to kill. You can declare more than one monster name separated with `,`

**Example**
```js
{
  "Name": "SlayTask7",
  "Type": "Slay",
  "Description": "Kill some slimes or bats",
  "Data": {
    "TargetName": "Slime, Bat, Jelly, Sludge"
  },
  "Count": 20,
}
```

### Talk

Talk with a villager. This task can be more complex by define a required items present in player's inventory to complete this task by talk with a villager. Also you can define which items you will receive after talk. This is usefull for item exchange with villagers in your quest lines.

| Field | Type | Description |
| ----- | ---- | ----------- |
| NpcName | string | A name of a NPC villager to talk with.
| DialogueText | string | A text of dialogue.
| StartEvent | `<int:EventId> <string:pathToScript>` where path to script must be `<string:GameContentFile>:<string:Key>` | An [event](#events) to be started when you talk with an NPC.
| ReceiveItems | string | An [item description](#quest-items) receive after this task is completed. You can define more items separated with `,`.
| RequiredItems | string | An [item description](#quest-items) of item which must be present in player's inventory to complete this task.
| KeepRequiredItems | boolean | true = Required items needed to complete this task stay in your inventory. Otherwise the required items present in your inventory will be removed while completing this task.

**Example**

```js
{
  "Name": "TestTask6",
  "Type": "Talk",
  "Description": "Talk with Abigail",
  "Data": {
    // Talk to abby and then see an event
    "NpcName": "Abigail",
    "DialogueText": "I am thinking about go to an adventure in mines. Can I go with you if you go to the mines some day?#$b#Take an ancient fruit from me",
    "StartEvent": "44017601 Data\\Events\\FarmHouse:3917601/f Emily 3500/O Emily/n emilyFiber/A emilyFiber/t 2000 2400/p Emily",
    "ReceiveItems": "object_ancient_fruit" // And get ancient fruit from Abby when talk
  }
}
```

### Tile Action

Interact with a tile on the map to complete quest task. Optionaly can requires an item present in player's hands (Player must hold this item)

| Field | Type | Description |
| ----- | ---- | ----------- |
| Location | string | A location name where the tile to interact is placed.
| Tile | `<int:x>,<int:y>` | A position on map of the tile to interact.
| ItemAcceptedContextTags | string | (optional) The context tags of required item in player's hands to complete this task. Player must hold this item to complete. The format is the same as for Special Orders in vanilla SDV.
| ItemRequiredMessage | string | The message to show when item is required.
| Message | string | The message to show when this task is completed.
| StartEvent | `<int:EventId> <string:pathToScript>` where path to script must be `<string:GameContentFile>:<string:Key>` | An [event](#events) to be started when you interacted with specified tile (and you hold the required item if defined). No message will be displayed if you defined it in field `Message`.
| ConsumeItem | boolean | true = The required item will be removed from player's inventory when this task is completed. Otherwise keep it in player's hands.

**Example**

```js
{
  "Name": "TileActionTest",
  "Type": "TileAction",
  "Data": {
    "Tile": "14,15",
    "Location": "FarmHouse",
    "Message": "You did it! Winner!"
  },
  "Description": "%i18n:story_quest.TestTask1.description"
}
```


## Events

Some quest tasks allows you to define events which will be triggered when the task completion started. The event description string has a format `<int:EventId> <string:pathToScript>` where path to script must be `<string:GameContentFile>:<string:Key>`.

| Argument | Type | Description
| -------- | ---- | -----------
| EventId | int | ID of your event which will be contained in player's event seen list when this event was played. It's recomended define your event id in format `MMMMNNNN` where M represents one digit of your mod id on Nexusmods and the `N` represents any number defined by you. Example: 45820001 is a event id #1 for mod NPC Adventures used by quest task.
| pathToScript | string | Fullpath of game asset dictionary which contains the event script. This contains the path to the asset file and the key in dictionary contained in that file. The filename and dictionary id is separated by `:`, like `Strings\\Locations:IslandSecret_Event_BirdieIntro` where *Strings\\Locations:IslandSecret_Event_BirdieIntro* is the game asset file path and `IslandSecret_Event_BirdieIntro` is the key in dictionary inside the asset file

**Tip for event script path:** In linux and 64-bit SDV build you can use the `/` instead of `\\` as path separator in asset filename.

## Quest Items

Some quest tags has a field to define concrete item instead of item context tags. This item is described by the item description which describes the item name (or id) and some parameters for the item. Item description is used in fields `RequiredItems` and `ReceiveItems` in task type `Talk`.

The format of item description: `<type>_<name> [specialAttr]`

| Attribute | Type | Description
| --------- | ---- | -----------
| type | BigCraftable, Boots, Clothing, Flooring, Furniture, Hat, Object, Ring, Tool, Wallpaper, Weapon | Item type name. This attribute is case insensitive.
| name | string | Item name. If the name contains spaces, replace them with underscore `_`
| specialAttr | `special`, `quest`, `lost` | (optional) An special attribute which will be assigned to a target item. This is usefull for recieve item by quest task.

### Item special attributes

- `special`sets item flag `specialItem` to the item. This item can't be trashed.
- `quest` sets item flag `questItem` to the item. Some items used in quests has set this flag.
- `lost` sets item flag `isLostItem`. Items used and spawned by Lost Item quest type has set this flag.
