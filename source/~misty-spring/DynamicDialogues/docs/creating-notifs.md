**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

## Adding notifications

Notifications are loaded from `mistyspring.dynamicdialogues/Notifs`.

| name     | required | description                                                                                                                              |
|----------|----------|------------------------------------------------------------------------------------------------------------------------------------------|
| Time     | (*)      | Time to add a notification at.                                                                                                           |
| Location | (*)      | Name of map to display the notif in.                                                                                                     |
| Message  | yes      | Message to display.                                                                                                                      |
| IsBox    | no       | If `true`, will make notification a box.                                                                                                 |
| Sound    | no       | Sound the notification will make. ([see sound IDs](https://docs.google.com/spreadsheets/d/18AtLClQPuC96rJOC-A4Kb1ZkuqtTmCRFAKn9JJiFiYE)) |
| Conditions | no        | [Conditions](https://github.com/misty-spring/DynamicDialogues/blob/main/docs/player-conditions.md) to use.                               |

`(*)` = like with dialogues,you need either a time or location (or both) for the dialogue to load.

**Template**:

```
        "example_asBox": {
          "Time": "",
          "Location": "",
          "Message": "",
          "IsBox": false,
          "Sound": "",
          }


```
**Note:** If you don't want the notif to appear every day, use CP's "When" field 
(e.g only send when it rains, when you've got x hearts with a NPC, etc. All conditions are compatible).

Example:

```
{
      "Action": "EditData",
      "Target": "mistyspring.dynamicdialogues/Notifs",
      "Entries": {
        "example_asBox": {
          "Location": "Farm",
          "Message": "The weather seems gloomy today...",
          "IsBox": true
        }
      },
      "When":{
        "Weather":"Rain, Storm"
      }
    },
```
