**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# Custom Furniture definition

To make Furniture highly customizable, the definition of a Furniture has a lot of properties. But don't worry, everything is explained here, and there's a good chance you won't need to use everything.

Once again, this documentation uses the [Example Pack](https://github.com/Leroymilo/FurnitureFramework/tree/main/%5BFF%5D%20Example%20Pack) as an example, it is strongly recommeded to go back and forth between the explanation here and the examples to identify what is being explained.

## Contents

* [Required Fields](#required-fields)
	* [Display Name](#display-name)
	* [Rotations](#rotations)
	* [Source Image](#source-image)
	* [Source Rect](#source-rect)
	* [Collisions](#collisions)
* [Optional Fields](#optional-fields)
	* [Vanilla Fields](#vanilla-fields)
		* [Force Type](#force-type)
		* [Price](#price)
		* [Indoor & Outdoor](#indoor--outdoor)
		* [Context Tags](#exclude-from-random-sales)
		* [Exclude from Random Sales](#exclude-from-random-sales)
	* [Custom Catalogue Shop](#custom-catalogue-shop)
		* [Shows in Shops](#shows-in-shops)
		* [Shop Id](#shop-id)
	* [Variants](#variants)
	* [Description](#description)
	* [Animation](#animation)
	* [Special Type](#special-type)
	* [Placement Type](#placement-type)
	* [Icon Rect](#icon-rect)
	* [Seasonal](#seasonal)
	* [Layers](#layers)
	* [Seats](#seats)
	* [Slots](#slots)
	* [Toggle](#toggle)
	* [Sounds](#sounds)

## Required Fields

### Display Name

This is the name of the Furniture as it will be displayed in game, it has basically no restriction, except that it must be a string (text between quotation marks `"`).  
This field is not actually required, but it doesn't really make sense to not have it: it will default to "No Name".

If using [Furniture Variants](#variants), you can use 2 tokens to use the variants keys in the Furniture Display Name:
- `{{ImageVariant}}`
- `{{RectVariant}}`

For example, the `"{{ImageVariant}} Armchair Test"` Furniture from the Example Pack will create 3 Furniture:
- Brown Armchair Test
- Yellow Armchair Test
- Blue Armchair Test

And the `"{{RectVariant}} Cat Statue"` Furniture will create 2 Furniture:
- White Cat Statue
- Black Cat Statue

### Rotations

This field allow you to define rotations. It can be of 2 types: an integer (a whole number), or a list of rotation names.  
If it is set to a list of rotation names, the number of names given corresponds to the number of rotations. The names given will be used as "rotation keys" in [Directional Fields](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Directional%20Fields.md) (you'll see about this later).  
If it is set to an integer, it simply gives the number of rotations of the Furniture. For example, a table has 2 rotations (horizontal and vertical), and a chair has 4 rotations (up, right, down and left).  
When using a number of rotations instead of a list of names, the resulting "rotation keys" are hardcoded:
- `"Rotations": 1` -> no need for rotations keys
- `"Rotations": 2` -> `"Rotations": ["Horizontal", "Vertical"]`
- `"Rotations": 4` -> `"Rotations": ["Down", "Right", "Up", "Left"]`

Any other number will result in an error because rotation keys are essential to parse Directional Fields. If you don't need to use Directional Fields, then the Rotations should be set to 1 (the Furniture cannot be rotated).

Note: the order of the rotation names will only define in which order they cycle when using right-click when placing a Furniture in game.

Note 2: you can set as many rotations as you want! Just make sure that they have distinct names. Here's an example with 6 rotations: 
```json
"Rotations": ["r1", "r2", "r3", "r4", "r5", "r6"]
```
You'll just have to remember to use these names as keys when defining directional fields.

### Source Image

This is the path, **relative to your mod's directory**, to the sprite-sheet to use for this Furniture. All sprites used in drawing your Furniture in the game (all rotations and layers) have to be in the same sprite-sheet. It is possible to use the same sprite-sheet for multiple Furniture.  
<span style="color:red">It is **strongly** recommended to align all sprites on a 16x16 pixel grid</span>, because every game tile is 16x16 pixels large, not doing so will cause a lot of issues down the line.

### Source Rect

This defines what part of the provided image will be used as your Furniture sprite.

This field is a [Rectangle](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Rectangle.md), it is important to understand how it works because it will be used later in other fields.

This field is also the first [Directional Field](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Directional%20Fields.md) you'll encounter, so make sure to check how they work.

### Collisions

This field defines the collisions of your Furniture, it's what defines what part of the Furniture the player will not be able to walk through and place other Furniture on. Since they are quite complicated, they have their own [Collisions documentation](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Complex%20Fields/Collisions.md).

## Optional Fields

### Vanilla Fields

These fields are basically what you'll find in a Furniture defined in [`Data/Furniture`](https://stardewvalleywiki.com/Modding:Items#Furniture).

#### Force Type

In this field, you can force the vanilla type of the Furniture (as a string). If you don't know how it works, don't set it, most types have not been tested and are replaced with other fields in the Furniture Framework. Please report it if you find Furniture types that completely break the mod so that I can list them here.

Do not use these types:
- chair
- bench
- couch
- armchair
- long table
- table
- dresser
- rug
- painting

#### Price

This is the default price of the Furniture, it will be used if it is added to a shop's item list without specifying a price. It defaults to 0g.

#### Placement Restriction

This field is a number that defines if the Furniture can be placed indoor and/or outdoor. Here are the possible values:
- `0`: indoors only
- `1`: outdoors only
- `2`: indoors and outdoors

#### Context Tags

This is an array (a list) of context tags you want to add to your Furniture, it defaults to an empty list. If you want to learn more about context tags, check [the wiki](https://stardewvalleywiki.com/Modding:Items#Context_tags).

#### Exclude from Random Sales

This defines wether or not this Furniture will show-up in random sales in the vanilla Furniture Catalogue and other Furniture shops. It's a boolean value (true or false), defaulting to true.

### Custom Catalogue Shop

Those fields are related to Shops and Catalogues.

#### Shows in Shops

This is an array (a list) of string Shop IDs where you want your Furniture to show-up, it defaults to an empty list.  For example, having:
```json
"Shows in Shops": ["Carpenter"]
```
will add your Furniture to Robin's Shop. Here's the list of [vanilla Shop IDs](https://stardewvalleywiki.com/Modding:Shops#Vanilla_shop_IDs) on the wiki.

When used in combination to the "Shop Id" field, you can create a custom Catalogue for your custom Furniture.

The token `{{ModID}}` can be used in this field.

#### Shop Id

The Shop ID of the Shop the game should open when right-clicking on the Furniture, it's a string that defaults to `null` (no Shop attached).  
You can attach one of the [vanilla Shops](https://stardewvalleywiki.com/Modding:Shops#Vanilla_shop_IDs), or your own Shop. Be carefull, some shops have some weird quirks when their owner is not around.  
By default, if the Shop ID given doesn't match any existing shop, a default shop based on the vanilla Furniture Catalogue (no owner) will be created.  
You can then use the same Shop ID in the "Shows in Shops" field of other Furniture you created to add them to this new Catalogue. If you want to add more rules to your custom Catalogue (multipliers, owners, ...), you'll need to define it in another Pack using Content Patcher, see how to make a [Mixed Content Pack](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Author.md#mixed-content-pack).

An example of this is in the [Example Pack](https://github.com/Leroymilo/FurnitureFramework/blob/main/%5BFF%5D%20Example%20Pack/content.json).

The token `{{ModID}}` can be used in this field.

Note: the Shop ID is raw, your mod's UniqueID will not be prepended to it, so make sure it's unique (you can manually add your mod's ID to it for example).

### Variants

This is kind of a replacement for a compatibility with Alternative Textures because making this mod truly compatible with Alternative Textures might not be possible.
You might want to create a batch of Furniture with the same properties but different sprites, there are 2 options for this:

#### Image Variants

Image Variants will allow you to have each variant based on a different Source Image.

To define them, instead of giving a single path in the [Source Image](#source-image), you can give a dictionary of paths:
```json
"Source Image": {
	"Brown": "assets/armchair.png",
	"Yellow": "assets/armchair_yellow.png",
	"Blue": "assets/armchair_blue.png"
},
```
This example is taken from the `armchair_test` Furniture of the Example Pack.

Note: this will create as many separate Furniture as source images are given, but all their properties (aside from Source Image) will be identical, including their Display Name. However, you can use the `{{ImageVariant}}` token in the Display Name field so that it will be replaced with the variant key (see the `armchair_test` Furniture in the Example Pack).  

Note 2: it is possible to use both Source Images Variants and [Seasonal](#seasonal) sprites, but all of the variants path given must have seasonal suffixes.

Note 3: you can also use a list of Source Image Variants instead of a dictionary but the `{{ImageVariant}}` token will be empty.

#### Rect Variants

Rect Variants will allow you to have each variant based on a different part of the Source Image.

To define them, you have to give `"Source Rect Offsets"`:
```json
"Source Rect Offsets": {
	"White": {"X": 0, "Y": 0},
	"Black": {"X": 0, "Y": 32}
},
```
This example is taken from the `cat_statue` Furniture of the Example Pack.

These Offsets are **integer** [Vectors](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Vector.md) that defines by how many pixels the original source rect (or source rects if directional) should be offset to find the sprites for the given Variant.

Note: this will create as many separate Furniture as offsets are given, but all their properties will be identical, including their Display Name. However, you can use the `{{RectVariant}}` token in the Display Name field so that it will be replaced with the variant key (see the `cat_statue` Furniture in the Example Pack).  

Note 2: you can also use a list of Source Rect Offsets instead of a dictionary but the `{{RectVariant}}` token will be empty.

### Description

The description of the Furniture that will be displayed in the game. If this is not set, the game will use one of the default descriptions that depends on the [placement restriction](#placement-restriction) of the Furniture (like vanilla Furniture have).

### Animation

You can define animations for your Furniture, but you'll need to fill a few fields for it to work:
- `Frame Count` the number of animation frames
- `Frame Duration` the length of every frame in milliseconds
- `Animation Offset` the position of each frame relative to the preceding frame.

The `Animation Offset` is an **Integer** [Vector](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Vector.md) field. At every new animation frame, the top left of every Source Rect (base sprite and layers) will be moved by this offset, so you can have your animations aligned however you want in your sprite-sheet.

Note: if any of these field is zero ((0, 0) for the Offset), the animation will be disabled.  

Here's an example of the fields for a working animation taken from the `cat_statue` in the Example Pack:
```json
"Frame Count": 7,
"Frame Duration": 500,
"Animation Offset": {"X": 16, "Y": 0}
```

Note 2: If using both [Source Rect Offsets](#rect-variants) and Animation, the offsets will be added together.

### Special Type

This kind of replace the "Type" field in the vanilla Furniture data. It's a string that can take one if these values:
- None (no special type)
- Dresser
- [TV](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Special%20Types/TV.md)
- [Bed](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Special%20Types/Bed.md)
- [FishTank](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Special%20Types/FishTank.md)

Some Special Types have their own documentation linked in this list for extra info.

### Placement Type

This will change the way you place the Furniture, it's only used for Rugs and Wall mounted Furniture. The possible values are:
- Normal
- Rug
- Mural

Please note that Rug Furniture can't have right-click interactions. However, Mural Furniture are compatible with all other features.

### Icon Rect

This field is another Rectangle, like a [Source Rect](#source-rect). This rectangle will tell the game which part of the texture to use to display the Furniture in the menu. It is affected by [Variants](#variants).

### Seasonal

This field will allow you to create Furniture with different Sprites depending on the season, it's a boolean (true or false).  
If false, the [Source Image](#source-image) will be read as is and the mod will try to read the image from this path.   If true, the mod will try to read an image for each season, based on the given Source Image path. For example, if Source Image is `assets/bush.png`, the mod will try to read these 4 images:
- `assets/bush_spring.png`
- `assets/bush_summer.png`
- `assets/bush_fall.png`
- `assets/bush_winter.png`

:warning: <span style="color:red">If any of these images is missing, the Furniture won't be created!</span>

The `seasonal_bush_test` in the Example Pack uses this feature.

### Layers

Layers are an important tool for making custom Furniture, they are necessary to properly display your Furniture when other objects are passing through it (the player, or other Furniture). Since they are quite complicated, they have their own [Layers documentation](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Complex%20Fields/Layers.md).

### Seats

Seats are what allow the Farmer to sit on your Furniture (duh), since they are quite complicated, they have their own [Seats documentation](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Complex%20Fields/Seats.md).

### Slots

Slots are where you can place items or other Furniture on a table-like Furniture. Since they are quite complicated, they have their own [Slots documentation](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Complex%20Fields/Slots.md).

### Toggle

This field is boolean (true or false) and will make a Furniture toggleable. "Toggleable" means that it can be turned on and off with right-click.  
When a Furniture can be toggled, every sprite in its sprite-sheet needs to be duplicated: for every "Source Rect" you defined (for the [base sprite](#source-rect) or for [Layers](#layers)), the origin of the Width of the Rectangle will be added to its horizontal position when the Furniture is turned on. This way, your Furniture can change how it looks when it's toggled.

A good example of this is the `Custom Cauldron` Furniture in the Example Pack: you can see in its sprite-sheet that it has its base sprite in the top left corner, and a Layer in the bottom left corner, while the "On" variants of these sprites are on the right of the sprite-sheet.  
![Custom Cauldron sprite-sheet](https://raw.githubusercontent.com/Leroymilo/FurnitureFramework/main/%5BFF%5D%20Example%20Pack/assets/cauldron.png)

### Sounds

With sounds, you can make your Furniture play custom sound effects when you click on it! Since they are quite complicated, they have their own [Sounds documentation](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Complex%20Fields/Sounds.md).

### Particles

Particles have so many settings, you have to read the [Custom Particles Documentation](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Complex%20Fields/Particles.md).
