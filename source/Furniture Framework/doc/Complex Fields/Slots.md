**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# How to define Custom Slots?

The Slots field of a Furniture is a (directional) list of slots objects.  
See the Example Pack for examples.

A slot object has multiple fields:

## Area

This is the [Rectangle](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Rectangle.md) where the slot is located on the sprite for this rotation. This area correspond to where you should click on the Furniture to place or remove something from the slot.  
Items placed in this slot will be horizontally centered in this area, and they will be aligned to the bottom of this area (the lowest pixel of the item sprite will be on the same line as the lowest pixel of the area). This can be changed with the Offset field.  

Be carefull, the Area of a Slot is relative to the sprite for this rotation, not to the whole spritesheet. For example, in the Vertical Slot of the `table_test` Furniture of the Example Pack, since the Vertical Source Rect starts at (32, 0), the Vertical Area starting at (0, 8) is actually starting at (32, 8) on the spritesheet.

Note: It is not recommended to define overlapping areas, but if you do they will be prioritized in the order they were defined.

## Offset

An offset, in pixels to change the default position of the item placed in this slot (it usually depends on the Area), it's a **decimal** [Vector](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Vector.md).  
It can be omitted, its default value being (0, 0).

## Draw Shadow

A boolean (true or false) that tells the game wether or not to draw the shadow of the placed item. Furnitures placed on Furniture have no added shadow.  
If the shadow is enabled, items will be drawn 1 (one) pixel higher.

## Shadow Offset

An offset, in pixels to change the default position of the shadow of the item placed in this slot (it usually depends on the Area)it's a **decimal** [Vector](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Vector.md).  
It can be omitted, its default value being (0, 0).

## Depth

This is the [depth](it's a **decimal** [Vector](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Depth.md).) at which the item in the slot should be drawn. See the "Living Room" furniture in the Example Pack to have examples of layers with depth.  

As a general rule, if you have to create a [Layer](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#layers) for the part of the Furniture where you'll place the spot, then the spot should have the same depth (or lower) as the corresponding layer.

Defaults to `{"Tile": 0, "Sub": 0}`.

Note: if a Slot has the same depth as a Layer, the item in the Slot will be drawn above the Layer, so you can give the Slot the same depth as the Layer it is supposed to rest on.

## Debug Color

This is the name of the color of the rectangle that will be shown if the "Slots Debug" options are enabled in the config, this is just a visual help to know where the Slot's Area is located. See [here](https://learn.microsoft.com/en-us/dotnet/api/system.drawing.color?view=net-8.0#properties) for a list of accepted color names (R, G, B and A are not accepted).

# Example

Here is an example of a table slot in a bigger Furniture (taken from the `living_room` Furniture of the Example Pack). I uses the Depth field.

This is where the slots are in the spritesheet (I removed some stuff for clarity):  
![slots example](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/images/slots_example.png)

This is the definition of the slots:
```json
{
	"Down": [
		{
			"Area": {"X": 48, "Y": 43, "Width": 32, "Height": 13}
		}
	],
	"Right": [
		{
			"Area": {"X": 64, "Y": 41, "Width": 16, "Height": 31},
			"Depth": 1
		}
	],
	"Up": [
		{
			"Area": {"X": 48, "Y": 11, "Width": 32, "Height": 13},
			"Depth": 2
		}
	],
	"Left": [
		{
			"Area": {"X": 0, "Y": 25, "Width": 16, "Height": 31},
			"Depth": 2
		}
	]
}
```