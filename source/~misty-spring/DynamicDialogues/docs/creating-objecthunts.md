**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Object hunts
Lets you create object hunts, akin to egg hunt and haley's event.


## Contents

* [Format](#format)
  * [AfterSequenceBehavior](#aftersequencebehavior)
  * [Adding data](#adding-data)
  * [Template](#template)
  * [Using the command](#using-the-command)



----------


## Format

Object hunts use a custom model:

name |type|Required| description
-----|---|---------|--------- 
Timer | `int` | false | A time limit for the hunt.
OnFailure | `AfterSequenceBehavior` | false | Behavior if player fails (requires `Timer`).
OnSuccess | `AfterSequenceBehavior` | false| Behavior when player completes the hunt.
CanPlayerRun | `bool` | false | Whether the player can run. Default true
Objects | `List<ObjectData>` | true | The list of objects to use in the hunt.

### AfterSequenceBehavior

The parameters for OnFailure/OnSuccess are the same:

name |type|Required| description
-----|---|---------|--------- 
Mail | `string` | false | Mail to send.
ImmediateMail | `bool` | false | If the mail should be sent today.
Flag | `string` | false| Flag to set.
Energy | `int` | false | Stamina to add/reduce.
Health | `int` | false | Health to add/reduce.


### Adding data
To add your own object hunt, patch `mistyspring.dynamicdialogues/Commands/objectHunt`. This is an example:

```json
{
         "Action": "EditData",
         "Target": "mistyspring.dynamicdialogues/Commands/objectHunt",
         "Entries": {
            "simpletest": {
               "CanPlayerRun": true,
               "OnSuccess": {
                  "Energy": "-50"
               },
               "Objects": [
                  {
                     "ItemId": "(O)541",
                     "X": 20,
                     "Y": 6
                  },
                  {
                     "ItemId": "(O)543",
                     "X": 21,
                     "Y": 6
                  }
               ]
            }
         }
```

### Template

```jsonc
"simpletest": {
    "Timer": 0,
    "CanPlayerRun": true,
    "OnSuccess": {
        "Mail":"",
        "ImmediateMail":"",
        "Flag":"",
        "Energy": "",
        "Health":""
    },
    "OnFailure": {
        "Mail":"",
        "ImmediateMail":"",
        "Flag":"",
        "Energy": "",
        "Health":""
    },
    "Objects": [
        {
            "ItemId": "(O)",
            "X": "",
            "Y": ""
        },
        {
            "ItemId": "(O)",
            "X": "",
            "Y": ""
            }
        ]
    }
```

### Using the command

In your event, just add `objectHunt <your addition's key>` and it'll begin.
(For our example, we'd use `objectHunt simpletest`)
