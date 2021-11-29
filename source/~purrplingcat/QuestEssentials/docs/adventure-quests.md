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

### Fish

### Gift

### Slay

### Talk

### Tile Action
