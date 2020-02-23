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
  "Abigail": "recruitable/warrior//5/0/40",
  "Maru": "recruitable/doctor fighter//5/0/36",
  "Shane": "recruitable/warrior//5/0/-1",
  "Leah": "recruitable/forager fighter//5/0/39",
  "Haley": "recruitable/anxious fighter//5/0/42",
  "Emily": "recruitable/fighter//5/0/0",
  "Penny": "recruitable/fighter//5/0/38",
  "Alex": "recruitable/warrior//5/0/25",
  "Sam": "recruitable/fighter//5/0/30",
  "Sebastian": "recruitable/fighter//5/0/41",
  "Elliott": "recruitable/fighter//5/0/35",
  "Harvey": "recruitable/doctor fighter//5/0/37"
}
```

## Companion definition

| Section | Allowed values | Description |
| ------- | -------------- | ----------- |
| recruitable | `recruitable` | Is this NPC recruitable, rentable or unavailable for asks and suggestions, but available in quests only? Only recruitable value is implemented in mod. |
| skills | `warrior, fighter` | One or more presonal skills. Defines which persoinal skills companion has. |
| availability | NOT USED! | |
| minimum_hearts | NOT USED! | |
| price | NOT USED! |
| sword | integer | Index of default sword. Every companion can to have own unique default sword for level 0. See game file **Data/weapons** for more details. `-1` means no sword |

## Edit dispositions in content pack

File `assets/data/myOwnCompanionDispositions.json`:

```json
{
  "Biglydz": "recruitable/warrior//5/0/40"
}
```

File `content.json`:

```json
{
  "Format": "1.0",
  "Changes": [
    {
      "Action": "Edit",
      "Target": "Data/CompanionDispositions",
      "FromFile": "assets/data/myOwnCompanionDispositions.json"
    },
  ]
}
```

This defines new companion in mod. Referenced NPC must be exists in gam. If it's a Custom NPC, this NPC mod must be put in mod folder and loaded in game.

## See also

- [Dialogues](dialogues.md)
