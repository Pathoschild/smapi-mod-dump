**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/su226/StardewValleyMods**

----

# Content Patcher HD

This mod allows you to create HD content packs with [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915).

## Download

Sorry, currently you can only download source code and compile by yourself.

## Usage

Just add `ScaleRequests` to content pack's `content.json`.

⚠️ Currently it only supports `EditImage`, not `Load`. 

```jsonc
{
  "ScaleRequests": [
    "LooseSprites/Cursors", // Textures here will be scaled to 4x.
  ],
  "Changes": [
    {
      "Action": "EditImage",
      // Edit textures ends with ".4x" will edit the scaled version.
      // So you can add HD texture with this method.
      "Target": "LooseSprites/Cursors.4x",
      // You need to multiply ToArea's X, Y, Width and Height by 4.
      "ToArea": { "X": 0, "Y": 0, "Width": 0, "Height": 0 }
    },
    {
      "Action": "EditImage",
      // Edit textures not ends with ".4x" will edit the original version.
      // Source image will be automatically scaled to 4x for compatibility.
      // Which means you CANNOT add HD texture with this method.
      "Target": "LooseSprites/Cursors",
      // You DON'T need multiply ToArea.
      "ToArea": { "X": 0, "Y": 0, "Width": 0, "Height": 0 }
    },
  }
}
```

## Others

This mod has no config file and no console command, but Content Patcher's `patch reload` is supported.
