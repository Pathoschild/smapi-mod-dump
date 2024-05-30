**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# How to define Custom Layers?

![Layers](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/images/layers.png)

The Layers field of a Furniture is a (directional) list of layer objects.  
See the Example Pack for examples.

A layer object has 3 fields:

## Source Rect

The part of the source image this layer should draw on the screen, it's a [Rectangle](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Rectangle.md).  
This field can be directional if the Layers list is not already directional.

## Draw Pos

This is the position, in pixels, relative to the top left of the base sprite (for the current rotation), where the layer should be drawn. It is an **integer** [Vector](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Vector.md).

Be carefull, the Draw Pos is relative to the sprite for this rotation, not to the whole spritesheet.  
In the `living_room` Furniture of the Example Pack for example, the Layer "Back of Couch facing Up" for the Left Rotation has (0, 64) as Draw Pos, but since the Source Rect for the Left Rotation starts at (80, 128) the layer mentionned would actually be drawn at (80, 192) on the spritesheet.

Defaults to `{"X": 0, "Y": 0}`

## Depth

This is the [depth](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Depth.md) at which the layer should be drawn. See the [Example](#example) to have examples of layers with depth.

Defaults to `{"Tile": 0, "Sub": 1000}` (so the bottom of the top-most tile of the Furniture's bounding box).

## Example

Here's an example on how to use layers with the down-facing `living_room` Furniture of the Example Pack. This is basically as complicated as it gets, I'd recommend checking some other Furniture from the Example Pack or the [Templates](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Templates.md) for simpler layers fitting your needs.

In this example, we will go through the layers from back to front. In this case, the lowest depth is the base sprite (at `"Depth": {"Tile": 0, "Sub": 0}`).

This gif shows how the layers are drawn from back to front, with a Farmer to show how it would be drawn in-between the layers:

![layers example gif](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/images/layers_example.gif)

Here's the list of Layers for this Furniture facing down:
```json
[
	{
		// Lower Arm of Couch facing Left
		"Source Rect": {"X": 192, "Y": 128, "Width": 32, "Height": 48},
		"Draw Pos": {"X": 96, "Y": 16},
		"Depth": {"Tile": 2, "Sub": 1000}
	},
	{
		// Lower Arm of Armchair facing Right
		"Source Rect": {"X": 192, "Y": 0, "Width": 32, "Height": 32},
		"Draw Pos": {"X": 0, "Y": 16},
		"Depth": {"Tile": 1, "Sub": 1000}
	},
	{
		// Table
		"Source Rect": {"X": 144, "Y": 224, "Width": 32, "Height": 32},
		"Draw Pos": {"X": 48, "Y": 32},
		"Depth": 2
	},
	{
		// Upper Arm of Armchair facing Right
		"Source Rect": {"X": 192, "Y": 32, "Width": 32, "Height": 32},
		"Draw Pos": {"X": 0, "Y": 16},
		"Depth": 1
	},
	{
		// Upper Arm of Couch facing Left
		"Source Rect": {"X": 192, "Y": 176, "Width": 32, "Height": 48},
		"Draw Pos": {"X": 96, "Y": 16},
		"Depth": 1
	}
]
```

Keep in mind that the Source Rect is defined from [this spritesheet](https://github.com/Leroymilo/FurnitureFramework/tree/main/%5BFF%5D%20Example%20Pack/assets/living_room.png).