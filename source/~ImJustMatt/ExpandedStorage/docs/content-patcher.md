**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ImJustMatt/StardewMods**

----

## Content Patcher Support

### SpriteSheets

Target Path:

`Mods/furyx639.ExpandedStorage/SpriteSheets/{StorageName}`

Example:

```json
{
  "Format": "1.20.0",
  "Changes": [
    {
      "Action": "EditImage",
      "Target": "Mods/furyx639.ExpandedStorage/SpriteSheets/Large Chest",
      "FromFile": "assets/large-chest.png"
    }
  ]
}
```


### Tab Images

Target Path:

`Mods/furyx639.ExpandedStorage/Tabs/{Mod.UniqueID}/{TabName}`

Use `furyx639.ExpandedStorage` as the Mod.UniqueID to patch the default tab
images available to all content packs

Example:

```json
{
  "Format": "1.20.0",
  "Changes": [
    {
      "Action": "EditImage",
      "Target": "Mods/furyx639.ExpandedStorage/Tabs/furyx639.ExpandedStorage/Crops",
      "FromFile": "assets/crops-tab.png"
    }
  ]
}
```