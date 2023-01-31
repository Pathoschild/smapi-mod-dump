**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ribeena/StardewValleyMods**

----

← [author guide](../author-guide.md)

## Contents
* [Working with other mods](#working-with-other-mods)
* [JSONAssets](#jasonassets)
  * [Limitations](#ja-limitations)
  * [Shirt Overlays](#ja-shirt-overlays)
* [Dynamic Game Assets](#dga)
  * [Limitations](#dga-limitations)
  * [Additions](#additions)
  * [Shirt Overlays](#dga-shirt-overlays)

## Working with other mods
Dynamic Bodies works with [JSONAssets](https://www.nexusmods.com/stardewvalley/mods/1720) and [Dynamic Game Assets (DGA)](https://www.nexusmods.com/stardewvalley/mods/9365)
which both allow you to add more clothing options. 

There are some considerations when using these mods however, listed below.

## JSONAssets

### JA Limitations
Clothing added by JSON Assets will probably be ignored if a bodystyle adds a custom clothing image for
shirts or pants.

### JA Shirt Overlays
When adding more shirts using JSONAssets you need to provide it a name, that name can be used to add
a new overlay. Create your black and white image and under the `[DB] YourModName\Shirts` folder create
a `shirts.json` file which may look like;

```
{
	"overlays": {
		"Hanging Overalls": ["hangingoveralls_male.png"],
		"High Waisted Belt": ["highbelt_male.png"],
	}
}
```
On the left `Hanging Overalls` is the name of the shirt for JSONAssets, and on the right is the 8x32 pixel image
to overlay onto it.
You need to add `"Metadata": "DB.PantsOverlay"` in your JSONAssets file to flag the added
shirt as a Dynamic Bodies shirt with an overlay from the pant color, for example
```
{
    "Name": "Hanging Overalls",
    "Description": "After some scissor action, these overalls provide fashion, not protection.",
    "HasFemaleVariant": false,
    "Price": 1,
    "Dyeable": false,
    "Metadata": "Sleeveless,DB.PantsOverlay"
}
```

## DGA
Due to the way Dynamic Game Assets modifies the source and index of images in the vanilla Stardew Valley
rendering system, new items need to be added a slightly different way to provide asset access
to Dyanmic Bodies.

To do this you can convert your [DGA content pack](https://github.com/spacechase0/StardewValleyMods/blob/develop/DynamicGameAssets/docs/author-guide.md) to a DB one by creating a new `manifest.json` for DB
and then a folder inside called "DGA" with the pack contents.

### DGA Limitations
As there is double handling, the DB-DGA content packs do not fully support all the features of DGA.
* All shirts, pants, hats must be specified in the `content.json` file directly. Other definitions can be 
  added using "ContentIndex"
* Animation is not supported

### Additions
The sub-content pack format for the DB-DGA includes additional options you can ass to the json;
* `TextureMaleOverlay`/`TextureFemaleOverlay` - to specify an overall image
* `Metadata` - to add additional information like [sleeve length](arms.md#how-sleeves-work)

### DGA Shirt Overlays
When adding a new shirt, specify the simple specify the textures for the overlay similar
to how you would for color, eg;
```
...
  {
    "$ItemType": "Shirt",
    "ID": "SpookyShirt",
    "TextureMale": "assets/spooky_shirt.png",
    _"TextureMaleOverlay": "assets/spooky_shirt_overlay.png",_
    "Metadata": "DB.Long, Set.Spooky"
  },
...
```