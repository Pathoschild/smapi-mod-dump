**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/PurrplingMod**

----

# Swimsuits

**NOTE:** This feature is experimental. Can be changed or removed in future versions.

Swimsuit sprites are defined in special data JSON file which is a map to which PNG resource file is used as swimsuit sprites for which companion NPC in mod. You can also define swimsuit sprites for your custom companion NPCs by add them to the swimsuit sprite map (swimsuit definition file).

## Swimsuit definition file

You can define sprite sources in `Data/Swimsuits`

```js
{
  "<CompanionName>": "<PngResourceFilePath>"
}
```

**IMPORTANT:** PNG resource files can't be patched by `Edit` patch in mod's content packs and can't be localized directly by NPC Adventures! If you want to patch sprite files, load sprites from the game folder and use **Content Patcher**

### Sprite reource files

Every sprite which is defined in the sprite definition file must have specified a path to the sprite file. This path can target to:

- Mod's assets folder (use path prefix `~/`). Suffix must be specified (like `.png`)
- Mod's content pack folder (use path prefix `@<contentpackUID>/`). Suffix must be specified (like `.png`)
- Game folder (use relative path without any prefix (like `Characters/Abigail_Swimsuit`)). Without file suffix.

### Use swimsuit sprites in content packs

If you want define custom sprites, you must register them in `Data/Sprites`. As a file path you can specify from which main folder your sprite will be loaded. You can specify:

- `@yourcp.uid/path/to/sprite.png` for lookup sprite in your content pack folder.
- `Path/In/Game/Folder/Sprite` for lookup sprite in the game folder.

If you want to lookup your sprite in game folder, you must to load your file in your custom way. NPC Adventures can't place your sprite to game folder. Use **Content Patcher** to do it.

#### Example of usage

```js
// Data/Sprites
{
  "Abigail": "~/Sprites/Abigail_swimsuit.png", // Lookup `Sprites/Abigail_swimsuit.png` in the mod folder
  "Maru": "Characters/Maru_swimsuit", // Lookup `Characters/Maru_swimsuit.xnb` in the game folder. Must be added by another mod, like Content Patcher
  "Shane": "@purrplingcat.customshanecompanion/assets/Shane_swimsuit.png" // Lookup `assets/Shane_swimsuit.png` in the 'purrplingcat.customshanecompanion' content pack folder
}
```

If you define you custom content pack for NPC Adventures and you want to add your custom swimsuits for your custom NPC as companion, add to `Changes` section in `content.json` inside your content pack folder.

```js
{
  "Target": "Data/Swimsuits",
  "FromFile": "assets/mySwimsuits.json"
}
```
