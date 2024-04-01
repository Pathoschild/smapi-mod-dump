**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/LeFauxMatt/StardewMods**

----

## Tools

* [FishingRod](#fishingrod)
* [MeleeWeapon](#meleeweapon)
* [Slingshot](#slingshot)
* [Tool](#tool)
* [WateringCan](#wateringcan)
* [Weapons](#weapons)

### FishingRod

#### Common Attributes

| field         | type                            | description              |
|---------------|---------------------------------|--------------------------|
| `GetBait()`   | [Object](./PatchItems#object)[] | Get the equipped bait.   |
| `GetTackle()` | [Object](./PatchItems#object)[] | Get the equipped tackle. |

### MeleeWeapon

Refer to `Data/Weapons.json` in
the [unpacked Content folder](https://stardewvalleywiki.com/Modding:Editing_XNB_files#Unpack_game_files)
for a complete list of the weapon targets/areas.

### Slingshot

#### Common Attributes

| field                     | type                            | description     |
|---------------------------|---------------------------------|-----------------|
| `attachments[0].getOne()` | [Object](./PatchItems#object)[] | Get ammunition. |

### Tool

Refer to `Data/Tools.json` in
the [unpacked Content folder](https://stardewvalleywiki.com/Modding:Editing_XNB_files#Unpack_game_files)
for a complete list of the tool targets/areas.

### WateringCan

#### Common Attributes

| field       | type | description                   |
|-------------|------|-------------------------------|
| `WaterLeft` | int  | Get the amount of water left. |

### Weapons

#### Common Attributes