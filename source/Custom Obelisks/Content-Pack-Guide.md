**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/XxHarvzBackxX/Custom-Obelisks**

----

← [README](README.md)
This document helps mod authors create a content pack for Custom Obelisks.

**See the [main README](README.md) for other info**.

# Content Packs

## How-to
In your `manifest.json` file, specify that this is a content pack for CO by placing CO's unique ID inside the `ContentPackFor` field (`harvz.CustomObelisks`).
Next, create a `content.json` file. It should look similar to this:
```js
{
  "Obelisks": [
    {
      "Name": "...",
      "InternalName": "...",
      "WhereToWarp": "...",
      "ToX": 0,
      "ToY": 0
    }
  ],
  "Totems": [
    {
      "Name": "...",
      "WhereToWarp": ".",
      "ToX": 0,
      "ToY": 0,
      "PersistItem": false
    }
  ]
}
```

# Obelisks

### Example Entry
```js
{
  "Obelisks": [
    {
      "Name": "Secret Woods Obelisk",
      "InternalName": "Secret Woods Obelisk",
      "WhereToWarp": "Woods",
      "ToX": 0,
      "ToY": 0
    },
  ]
}
```

## Content Patcher Element
This framework works in harmony with Content Patcher; you'll need to load your assets through a \[CP] pack. Example:
```js
{
  "Format": "1.23.0",
  "Changes": [
    { // load texture
      "LogName": "Obelisk Texture Load",
      "Action": "Load",
      "Target": "Buildings/NAME",
      "FromFile": "assets/NAME.png"
    },
    { // load blueprint
      "LogName": "Patch Obelisk Blueprint",
      "Action": "EditData",
      "Target": "Data/Blueprints",
      "Entries": {
        "NAME": "MATERIALS/3/2/-1/-1/-2/-1/null/NAME/DESCRIPTION/Buildings/none/48/128/-1/null/Farm/7500/true"
      }
    }
  ]
}
```
For more info on Content Patcher, see [the documentation](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md) for it.
For info on the data entries in `Data/Blueprints`, see [the page on it in the Stardew Wiki](https://stardewvalleywiki.com/Modding:Blueprint_data).
### Assets
The obelisk image file must be 48x128 pixels. If the image resolution does not match this (without loading it specially through external tools like [PyTK](https://gist.github.com/hatrat/6339a7975ae5d13802488d88a8b10a64#scaleup)), it could cause a crash.


## Fields

Field                | Valid Entries                        
-------------------- | ------------------------------- 
Name                 | `(string)` The display name (translations, input your localised name).
InternalName         | `(string)` The internal name (translations, use whatever the internal name is).
WhereToWarp          | `(string)` The internal name of the location to warp to.
ToX                  | `(int)` The X coordinate where to warp the player.
ToY                  | `(int)` The Y coordinate where to warp the player.

An example content pack can be found on the [Nexus page under files tab](https://www.nexusmods.com/stardewvalley/mods/10202?tab=files).

### Multiple Obelisks
To add multiple obelisks, simply add multiple 'Obelisks' entries. Copy and paste the first one, adding a comma to the end, and fill in the appropriate info. Example:
```js
{
  "Obelisks": [
    {
      "Name": "Secret Woods Obelisk",
      "InternalName": "Secret Woods Obelisk",
      "WhereToWarp": "Woods",
      "ToX": 0,
      "ToY": 0
    },
    {
      "Name": "Town Obelisk",
      "InternalName": "Town Obelisk",
      "WhereToWarp": "Town",
      "ToX": 0,
      "ToY": 0
    }
  ]
}
```
# Totems

### Example Entry
```js
{
  "Totems": [
    {
      "Name": "Secret Woods Totem",
      "WhereToWarp": "Woods",
      "ToX": 9,
      "ToY": 10,
      "PersistItem": false
    }
  ]
}
```

## Json Assets Element
This framework works in harmony with Json Assets; you'll need to load your totems through a \[JA] pack.
For more info on Json Assets, see [the documentation](https://github.com/spacechase0/JsonAssets#objects) for it.

## Fields

Field                | Valid Entries                        
-------------------- | ------------------------------- 
Name                 | `(string)` The display name used in the JA pack.
WhereToWarp          | `(string)` The internal name of the location to warp to.
ToX                  | `(int)` The X coordinate where to warp the player.
ToY                  | `(int)` The Y coordinate where to warp the player.
PersistItem          | `(bool)` True if the item should be removed when used. False if not (for custom sceptres).

An example content pack can be found on the [Nexus page under files tab](https://www.nexusmods.com/stardewvalley/mods/10202?tab=files).

### Multiple Totems
To add multiple totems, simply add multiple 'Totems' entries. Copy and paste the first one, adding a comma to the end, and fill in the appropriate info. Example:
```js
{
  "Totems": [
    {
      "Name": "Secret Woods Totem",
      "WhereToWarp": "Woods",
      "ToX": 9,
      "ToY": 10,
      "PersistItem": false
    },
    {
      "Name": "Witch's Sceptre",
      "WhereToWarp": "WitchHut",
      "ToX": 7,
      "ToY": 8,
      "PersistItem": true
    }
  ]
}
```

# Omission

## Class Omission
To omit Obelisks or Totems, simply do this:
```js
{
  "Obelisks": null,
  "Totems": null
}
```
For whatever part you wish to omit.
## Field Omission
As of current, no fields can be omitted. Fill all of the fields for now.




# Questions
If you have any questions, feel free to open an issue on the Github, leave a comment on Nexus, or DM me on Discord at `XxHarvzBackxX#3665`!

<em>-Harvz</em>
