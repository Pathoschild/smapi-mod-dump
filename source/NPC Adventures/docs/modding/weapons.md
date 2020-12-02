**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/PurrplingMod**

----

# Weapons

There are two weapon definition content files:

- For concrete NPC (`Data/Weapons/<NPC_name>`)
- Default weapons (`Data/Weapons`)

These definition content files are merged. NPC Adventures loads first `Data/Weapons` and then try load `Data/Weapons/<NPC_name>`. If the second file exists, merge it with `Data\Weapons` and values with the same key are covered with the value from content file for concrete NPC.

## Weapon definition

In `Data/Weapons` or in `Data/Weapons/<NPC_name>` are defined the key-value table of which weapons companion uses for which combat level. Companion combat level is the same as farmer's. The key is number of combat level, the value is weapon name (string) in `Data\weapons` SDV content file or their id (number).

### Example

```js
// Default weapons set (Data/Weapons)
{
  "0": "Rusty Sword",
  "1": "Steel Smallsword",
  "2": "Silver Saber",
  "3": "Forest Sword",
  "4": "Iron Edge",
  "5": "Rapier",
  "6": "Claymore",
  "7": "Neptune's Glaive",
  "8": "Steel Falchion",
  "9": "Obsidian Edge",
  "10": "Galaxy Sword"
}
```

```js
// Concrete weapon set for Abigail (Data/Weapons/Abigail)
// Defines only different weapon for combat level 0. 
// For other levels uses weapons defined in `Data/Weapons` asset
{
  "0": "Abby's Planchette"
}
```

### Use fist only as weapon

If you want your companion uses only fists for specific combat level, set as value `-1`. In NPC adventures Shane uses fists for combat level 0, there is an example:

```js
// Concrete weapon set for Abigail (Data/Weapons/Shane)
// Shane uses fists for combat level 0
{
  "0": "-1",
}
```

## Define custom weapons for custom companions

There is an example of define custom weapons for custom companion:

```js
// Content pack definition file
{
  "Format": "1.3",
  "Changes": [
    {
      "Target": "Data/Weapons/Ashley", // Defines custom weapons for Ashley companion
      "FromFile": "assets/data/ashley-weapons.json",
      "LogName": "Custom weapons for Ashley" // Optional. Can be used for edit action too
    }
  ]
}
```

``` js
// Content pack asset file: assets/data/ashley-weapons.json
// Ashley uses custom weapons for combat level 0 and 2. 
// For other levels are used weapons defined by `Data/Weapons` in NA content folder
{
  "0": "Nimbus 2000" // Custom weapon (added by JSON assets or by another mod)
  "2": "Templar's Blade", // Vanilla Stardew weapon
}
```
