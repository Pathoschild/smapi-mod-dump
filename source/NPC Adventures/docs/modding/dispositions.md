**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/PurrplingMod**

----

# Companion dispisitions

When we want to add new NPC as a companion, we must define it in dispositions file. **It's very important to define our NPC when we wants to be supported as companion in mod**

```yaml
# content reference token (use it in content pack key or for loading in code with ContentLoader)
Data/CompanionDispositions

# Target file in mod assets folder (refered by token)
assets/Data/CompanionDispositions.json 
# Refered localized file
assets/Data/CompanionDispositions.<lang_code>.json
```

## Structure&Format

Definition structure is a dictionary of NPC, where key is NPC name and value represents companion definitions with slash splited sections.

```json
{
  "<NPC_name>": "<recruitable>/<skills>/<availability>/<minimum_hearts>/<price>/<sword>"
}
```

**Example:**

```json
{
  "Abigail": "recruitable/warrior//5/0/Abby's Planchette",
  "Maru": "recruitable/doctor fighter//5/0/Maru's Wrench",
  "Shane": "recruitable/fighter//5/0/-1",
  "Leah": "recruitable/forager fighter//5/0/Leah's Whittler",
  "Haley": "recruitable/fighter scared//5/0/Haley's Iron",
  "Emily": "recruitable/fighter//5/0/Rusty Sword",
  "Penny": "recruitable/forager fighter//5/0/Penny's Fryer",
  "Alex": "recruitable/warrior//5/0/Alex's Bat",
  "Sam": "recruitable/fighter//5/0/Sam's Old Guitar",
  "Sebastian": "recruitable/warrior//5/0/Seb's Lost Mace",
  "Elliott": "recruitable/fighter//5/0/Elliott's Pencil",
  "Harvey": "recruitable/doctor fighter//5/0/Harvey's Mallet"
}
```

## Companion definition

| Section | Allowed values | Description |
| ------- | -------------- | ----------- |
| recruitable | `recruitable` | Is this NPC recruitable, rentable or unavailable for asks and suggestions, but available in quests only? Only recruitable value is implemented in mod. |
| skills | `warrior, fighter, doctor, forager, scared` | One or more presonal skills. Defines which persoinal skills companion has. |
| availability | NOT USED! | |
| minimum_hearts | NOT USED! | |
| price | NOT USED! |
| sword | Sword name (string) or ID (int)| Name or id of default sword. Every companion can to have own unique default sword for level 0. See game file **Data/weapons** for more details. `-1` means no sword. Custom swords supported |

## Edit dispositions in content pack

File `assets/data/myOwnCompanionDispositions.json`:

```js
{
  "Biglydz": "recruitable/warrior//5/0/Master sword" // This companion uses custom sword from another mod named 'Master sword'
  "Hastings": "recruitable/warrior//5/0/40" // This companion uses vanilla sword with ID 40 (Abby's Planchette)
  "Pam": "recruitable/warrior//5/0/Cutlass" // This companion vanilla sword named 'Cutlass'
}
```

File `content.json`:

```json
{
  "Format": "1.3",
  "Changes": [
    {
      "Target": "Data/CompanionDispositions",
      "FromFile": "assets/data/myOwnCompanionDispositions.json"
    },
  ]
}
```

This defines new companion in mod. Referenced NPC must be exists in gam. If it's a Custom NPC, this NPC mod must be put in mod folder and loaded in game.

## See also

- [Dialogues](dialogues.md)
