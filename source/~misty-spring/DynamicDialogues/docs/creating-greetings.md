**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

## Adding greetings

"Greetings" refers to the text NPCs will use when passing by another NPC. 
This can occur, for example, when two NPCs walk by each other on the way to their schedules.

Greetings use a file called `mistyspring.dynamicdialogues/Greetings`.

This doesn't use any specific format- it's just the name of the NPC, and entries with the second NPC + their dialogue.

Template:
```
      "nameOfNpc": {
          "NpcA": "",
          "NpcB": "",
          "NpcC": ""
          //...etc. you can add for any and all NPCs
        }    
```

**Example:**
This will edit Alex's greetings towards Evelyn and George. If he meets them on the way to his schedule, he will say "Hello"(if he meets Evelyn), or "Good morning"(if he encounters George instead).

```
{
      "Action": "EditData",
      "Target": "mistyspring.dynamicdialogues/Greetings",
      "Entries": {
        "Alex": {
          "Evelyn": "Hello",
          "George": "Good morning"
        }
      }
    }
```
