**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/FarmhouseVisits**

----

# FarmhouseVisits
A mod that allows villagers to visit you.

## Features
- NPC blacklist. Read every time you load a save (and every time your farmer goes to sleep).
- Villagers will bring a gift (optional).
- In-laws will ask about your spouse and children (if any).
- The visitor will walk around and comment on your furniture.
- Once the visit ends, the character will tell you they're going away (and return to whichever action they were doing pre-visit).
- Customizable start and end hours, along with chance for every 10 minutes and max visits per day.
- Scheduled visits (via CP patching of the schedule file).
- Visits can walk around your farm.

## Scheduled visits
You can make characters visit you at any time you want- just edit `mistyspring.farmhousevisits/Schedules` via ContentPatcher. Because you're editing via CP, you can use conditions just fine.

Example:

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
    "": 
    {
      "From": ,
      "To": ,
      "EntryDialogue": "",
      "ExitDialogue": "",
      "Dialogues":
      [
        "", 
        "", 
        "",
      ]
    }
  }
```
## Contributing
If you'd like to translate this mod, you can either: 1. Post the translation in nexus OR 2. Send the i18n json to me so i integrate it into the mod.
Currently, there's translations for: 
- Spanish, 
- English (both in original mod), 
- Russian (by crybaby00000), 
- Turkish (by BURAKMESE),
- Portuguese (by BrasileiroTop).

## For more info
You can add as many dialogues as you want, but they'll be only updated every 10 minutes.
If you don't set a finish to the visit ("To"), they'll go away when the regular visit time ends.


[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/G2G7CXX9P)
