**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Custom visits

This mod lets you make custom visits. This is done by editing its Schedules file.

## Accepted values

name |type|Required| description
-----|---|---------|--------- 
From | `int` | true | The hour the NPC arrives at.
To | `int` | false | The hour the NPC leaves at. If empty, will use default time.
MustBeExact | `bool` | false| If true, the visit time must be exact. E.g: If From is 900, the visit will only trigger if you're inside the Farm at 9:00, and 9:10 (etc) won't work.
EntryBubble | `string` | false | Text to show over their head when entering.
EntryQuestion | `string` | false | If `Confirmation` is enabled, they'll use this to ask for permission.
EntryDialogue | `string` | false | Dialogue used when entering.
ExitDialogue | `string` | false | Dialogue used when leaving.
Dialogues | `List<string>` | false| Dialogues to use during the visit.
Extras | `ExtraBehavior` | false | Extra behavior for the NPC.

### Extrabehavior
Extra behavior is purely optional, and has the following parameters:
name |type| description
-----|---|--------- 
Force|bool| Visit will happen even if you're not in the farmhouse.
Mail|string|Mail ID to send after visit.
GameStateQuery|string|Extra conditions for the visit to happen. See [GSQ in the wiki](https://stardewvalleywiki.com/Modding:Game_state_queries).


## Example:

```
{
  "LogName": "Add George visit",
  "Action":"EditData",
  "Target":"mistyspring.farmhousevisits/Schedules",
  "Entries": {
    "George": 
    {
      "From": 700,
      "To": 1000,
      "EntryDialogue": "Hi, @. What are you young kids getting up to?#$b#I came to visit.",
      "ExitDialogue": "It's getting cold...#$b# I have to go. Goodbye, @.",
      "Dialogues":
      [
        "What's that over there?#$b#A picture you drew?", 
        "This house isn't so bad...", 
        "%George is looking at you as you work.",
      ]
    },
  "When":{
    "Hearts:George": 5
    }
  }
```

Empty template:
```
{

  "LogName": "Add visit",
  "Action":"EditData",
  "Target":"mistyspring.farmhousevisits/Schedules",
  "Entries": {
    "name_of_character": {
      "From": ,
      "To": ,
      "EntryBubble": "",
      "EntryQuestion: "",
      "EntryDialogue": "",
      "ExitDialogue": "",
      "Dialogues":
      [
        "", 
        "", 
        "",
      ],
      "Extras":{
	"Force": false,
	"Mail": null,
	"GSQ": null
      }
    }
  }
```
