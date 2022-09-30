**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ribeena/StardewValleyMods**

----

← [author guide](../author-guide.md)

## Contents
* [Introduction](#introduction)
* [Arms](#arms)
* [How sleeves work](#how-sleeves-work)

## Introduction
After creating your [content JSON file](../author-guide.md#body-parts), you'll need to add
entries for each of the options.

Sometimes looking at the [examples](https://www.nexusmods.com/stardewvalley/mods/12893?tab=files#file-container-optional-files) make it easier!

## Arms
The main arms files only includes the right two thirds which are the arms of the character.
There are 4 images to allow for each to use the [color palette](color-palette.md) properly when showing and different
states of sleeves. This image is drawn over the [bodies](bodies.md) image, and is partially
changes by the [eyes](face-and-parts.md#eyes) options. This image is specific to
a gender height as the arms tend to site a pixel lower on females/

When making you picture it's good to start off by copying the
default ones from this mod. The image is
192x672, and you will need a [Sleeveless](../../asset/Character/arm_Sleeveless.png),
[Short](../../asset/Character/arm_Short.png), [Normal](../../asset/Character/arm_Normal.png) and
[Long](../../asset/Character/arm_Long.png) variation using the shirt colors. Refer to
the [color palette](color-palette.md) for what colors to use for the shirt vs skin.

Once a file is made, add an entry to the JSON file;

```
...
  "female": {
    ...
    "arms": {
      "Toned Arms":"toned"
    },
...
```
Above in the `female` section, a 'toned_... .png' files has been added. The folder will look like;
```
📁 Mods/
   📁 [DB] YourModName/
      🗎 content.json
      🗎 manifest.json
      📁 assets/
         📁 arms/
            🗎 toned_Sleeveless.png
            🗎 toned_Short.png
            🗎 toned_Normal.png
            🗎 toned_Long.png
            ...
         ...
```
## How sleeves work
When a player equips a shirt Dyanmic Bodies will check to see if it already has metadata
attached to it on what it is, for example the vanilla tank top is
`"1129": "Tank Top/Tank Top/A sleeveless shirt./129/130/50/255 235 203/true/Shirt/Sleeveless",`
The metadata is the last information after '/', in this case Dynamic Bodies will use
the sleeveless arms. For shirts without this metadata there are some
arbitrary decision on sleeve lengths - the player can edit this as needed in-game. When
a player changes the sleeves that saves only on that item, all future shirts
have the default sleeve length.

If you are adding clothing using JSONAssets, then you can also
add metadata to make it clear what sleeves it is when they first get the top
by using the metadata field;
```
{
    "Name": "Princess Shirt",
    "Description": "A cute shirt with short sleeves.",
    "HasFemaleVariant": false,
    "Price": 1,
    "Dyeable": false,
    "Metadata": "DB.Short"
}
```
You have the options of vanilla's `Sleeveless`, `DB.Short` and `DB.Long`.