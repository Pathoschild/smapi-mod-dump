**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# Templates

This is a file with templates reproducing Vanilla Furniture, so that you can have a strating point when making your own Furniture.  
All images referenced are in the assets of the Example Pack.

## Contents

- [Basic Furniture](#basic-furniture)
- [Seasonal Furniture](#seasonal-furniture)
- [Sittable Furniture](#sittable-furniture)
	- [Chair](#chair)
	- [Couch](#couch)
- [Table Furniture](#table-furniture)
- [Catalogue](#catalogue)
- [Cauldron](#cauldron)
- [Complex Furniture](#complex-furniture)
	- [Corner Couch](#corner-couch)
	- [Living Room](#living-room)

## Basic Furniture

This can be adapted for any Furniture having no rotations.

```json
{
	"Display Name": "Basic Furniture",

	"Rotations": 1,
	"Collisions": {
		"Width": 1,
		"Height": 1
	},	// in tiles

	"Placement Restriction": 1,	// outdoors only

	"Source Image": "assets/simple.png",
	"Source Rect": {"X": 0, "Y": 0, "Width": 16, "Height": 48},	// in pixels
}
```

## Seasonal Furniture

This can be adapted for any Seasonal Furniture having no rotations.

```json
{
	"Display Name": "Seasonal Bush",
	"Rotations": 1,
	"Collisions": {
		"Width": 2,
		"Height": 1
	},

	"Placement Restriction": 1,	// outdoors only

	"Seasonal": true,
	"Source Image": "assets/bush.png",
	"Source Rect": {"X": 0, "Y": 0, "Width": 32, "Height": 32},	// in pixels
}
```
Make sure that all the seasonal tile-sheets are located where the `Source Image` path points to:
- `assets/bush_spring.png`
- `assets/bush_summer.png`
- `assets/bush_fall.png`
- `assets/bush_winter.png`

## Sittable Furniture

### Chair

This can be adapted for any chair-like Furniture with 4 rotations and a simple front layer.

```json
{

	"Display Name": "Chair Furniture",
	"Price": 0,

	"Rotations": 4,
	"Collisions": {
		"Width": 1,
		"Height": 1
		// in tiles
	},

	"Placement Restriction": 2,	// indoors & outdoors

	"Source Image": "assets/chair.png",
	"Source Rect": {
		"Down":		{"X": 0,  "Y": 0, "Width": 16, "Height": 32},
		"Right":	{"X": 16, "Y": 0, "Width": 16, "Height": 32},
		"Up":		{"X": 32, "Y": 0, "Width": 16, "Height": 32},
		"Left":		{"X": 48, "Y": 0, "Width": 16, "Height": 32}
		// in pixels
		// must have all directions
	},
	"Layers": {
		"Up": [
			{
				"Source Rect": {"X": 32, "Y": 32, "Width": 16, "Height": 32}
				// in pixels
			}
		]
		// Only the "Up" rotation needs to have LayerData
		// because it's the only direction where the chair
		// is in front of the Player sitting in it.
	},

	"Seats": [
		// positions are from the top left of the Bounding Box (not the texture!)
		{
			"Position": {"X": 0, "Y": 0},		// in tiles, can be decimal
			"Player Direction": {
				"Up": 0,
				"Right": 1,
				"Down": 2,
				"Left": 3
			}
			// The direction the player is facing when sitting on this seat:
			// 0 means Up, 1 means Right, 2 means Down and 3 means Left.
		}
	]
}
```

### Couch

This can be adapted for more complex Sittable Furniture, like a couch.

```json
{

	"Display Name": "Couch Test",

	"Rotations": 4,
	"Collisions": {
		"Down": 	{"Width": 3, "Height": 1},
		"Right": 	{"Width": 2, "Height": 2},
		"Up": 		{"Width": 3, "Height": 1},
		"Left": 	{"Width": 2, "Height": 2}
		// in tiles
		// each rotation has its own Bounding Box Size.
	},

	"Placement Restriction": 2,	// indoors & outdoors

	"Source Image": "assets/couch.png",
	"Source Rect": {
		"Down":		{"X": 0,	"Y": 0, "Width": 48, "Height": 32},
		"Right":	{"X": 48,	"Y": 0, "Width": 32, "Height": 48},
		"Up":		{"X": 80,	"Y": 0, "Width": 48, "Height": 32},
		"Left":		{"X": 128,	"Y": 0, "Width": 32, "Height": 48}
		// in pixels
		// must have all directions
	},

	"Layers": {
		// Layer for Down is transparent, can be omitted
		"Right": [
			{
				"Source Rect": {"X": 48, "Y": 48, "Width": 32, "Height": 48},
				"Depth": {"Tile": 1, "Sub": 1000}
			}
		],
		"Up": [
			{
				"Source Rect": {"X": 80, "Y": 48, "Width": 48, "Height": 32},
				"Depth": {"Tile": 0, "Sub": 1000}
			}
		],
		"Left": [
			{
				"Source Rect": {"X": 128, "Y": 48, "Width": 32, "Height": 48},
				"Depth": {"Tile": 1, "Sub": 1000}
			}
		]
	},

	"Seats": {
		"Up": [
			{
				// Left seat
				"Position": {"X": 0.5, "Y": 0},
				"Player Direction": 0
			},
			{
				// Right seat
				"Position": {"X": 1.5, "Y": 0},
				"Player Direction": 0
			}
		],
		"Right": [
			{
				// Top seat
				"Position": {"X": 1, "Y": 0},
				"Player Direction": 1
			},
			{
				// Bottom seat
				"Position": {"X": 1, "Y": 1},
				"Player Direction": 1
			}
		],
		"Down": [
			{
				// Left seat
				"Position": {"X": 0.5, "Y": 0},
				"Player Direction": 2
			},
			{
				// Right seat
				"Position": {"X": 1.5, "Y": 0},
				"Player Direction": 2
			}
		],
		"Left": [
			{
				// Top seat
				"Position": {"X": 0, "Y": 0},
				"Player Direction": 3
			},
			{
				// Bottom seat
				"Position": {"X": 0, "Y": 1},
				"Player Direction": 3
			}
		]
	},
}
```

## Table Furniture

This can be adapted for any Furniture with slots to place objects on it. This example has 2 slots.

```json
{
	"Display Name": "Table Furniture",

	"Rotations": 2,
	"Collisions": {
		"Horizontal": 	{"Width": 2, "Height": 1},
		"Vertical": 	{"Width": 1, "Height": 2}
		// in tiles
	},

	"Placement Restriction": 2,	// indoors & outdoors

	"Source Image": "assets/table.png",
	"Source Rect": {
		"Horizontal":	{"X": 0,	"Y": 0, "Width": 32, "Height": 32},
		"Vertical":		{"X": 32,	"Y": 0, "Width": 16, "Height": 48}
		// in pixels
	},

	"Slots": {
		"Horizontal": [
			{
				"Area": {"X": 0, "Y": 10, "Width": 16, "Height": 16}
			},
			{
				"Area": {"X": 16, "Y": 10, "Width": 16, "Height": 16}
			}
		],
		"Vertical": [
			{
				"Area": {"X": 0, "Y": 8, "Width": 16, "Height": 16}
			},
			{
				"Area": {"X": 0, "Y": 24, "Width": 16, "Height": 16},
				"Depth": 1
			}
		]
		// The Area rectangle is in pixels, and is relative to the sprite for each rotation.
	}
}
```
Be carefull, the Area of a Slot is relative to the sprite for this rotation, not to the whole sprite-sheet. In this example, since the Vertical Source Rect starts at (32, 0), the Vertical Area starting at (0, 8) is actually starting at (32, 8) on the sprite-sheet.

## Catalogue

This is an example of how to make a custom catalogue throught the Furniture Framework.

```json
{
	"Display Name": "Custom Catalogue",

	"Rotations": 1,
	"Collisions": {
		"Width": 1,
		"Height": 1
		// in tiles
	},

	"Placement Restriction": 2,	// indoors & outdoors

	"Source Image": "assets/catalogue.png",
	"Source Rect": {"X": 0, "Y": 0, "Width": 16, "Height": 32},	// in pixels

	"Shop Id": "{{ModID}}.custom_catalogue",
	// To create a Shop. It is strongly recommended to use the {{ModID}}.
	"Shows in Shops": ["Carpenter"]
	// Adding the Custom Catalogue to Robin's Shop
}
```

If you want other Furniture to show up in this Catalogue, you have to add the field `"Shows in Shops": ["{{ModID}}.custom_catalogue"]` to their definition.  
You can also define a Shop in more details by using Content Patcher to patch Data/Shops, see [the wiki](https://stardewvalleywiki.com/Modding:Shops) for more info about custom Shops. Shops with an ID that already exists should be attached to the Furniture without having their definition modified. The same way goes for Shop Items: if you define a Shop Item in a CP Patch, it won't be overwritten by the Furniture Framework. If you have any issue with this feature, ping me in the [modding channel of the Stardew Valley Discord server](https://discord.com/channels/137344473976799233/156109690059751424) so that I can help you.

## Cauldron

A cauldron has 3 specific properties:
- it is [Toggleable](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#toggle)
- it has a [Layer](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#layers)
- it has [Particles](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#particles)

This template can be easily reused, but make sure to understand each field before modifying it.

```json
{
	"Display Name": "Custom Cauldron",

	"Rotations": 1,
	"Collisions": {
		"Width": 2,
		"Height": 1
	},	// in tiles

	"Indoors": true,
	"Outdoors": true,

	"Source Image": "assets/cauldron.png",
	"Source Rect": {"X": 0, "Y": 0, "Width": 32, "Height": 32},	// in pixels

	"Layers": [
		{
			"Source Rect": {"X": 0, "Y": 32, "Width": 32, "Height": 32}
		}
	],

	"Sounds": [
		{
			"Mode": "on_turn_on",
			"Name": "bubbles"
		},
		{
			"Mode": "on_turn_on",
			"Name": "fireball"
		}
	],

	"Particles": [
		{
			"Source Image": "assets/smoke.png",
			"Source Rect": {"X": 0, "Y": 0, "Width": 10, "Height": 10},
			"Emission Interval": 500,

			"Spawn Rect": {"X": 12, "Y": 15, "Width": 8, "Height": 4},
			"Depths": [0.1, 0.3, 0.5, 0.7, 0.9],
			"Speed": {"X": 0, "Y": -0.5},

			"Rotations": [],
			"Rotation Speeds": [
				-0.061, -0.049, -0.037, -0.025, -0.012,
				0, 0.012, 0.025, 0.037, 0.049, 0.061
			],

			"Scale": 3,
			"Scale Change": 0.01,

			"Color": "Lime",
			"Alpha": 0.75,
			"Alpha Fade": 0.0027,

			"Frame Count": 1,
			"Frame Duration": 5000,
			"Hold Last Frame": false,
			"Flicker": false,

			"Emit When On": true,
			"Emit When Off": false,
			"Burst": true
		}
	],

	"Toggle": true
}
```

## Complex Furniture

Before trying to use one of these templates, it is strongly recommended to read about all of the features they use in the [documentation](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md).

### Living Room

This is an example of a Furniture with:
- 4 Rotations
- Collision Maps
- Seats
- Layers
- Slots

```json
{

	"Display Name": "Living Room",

	"Rotations": 4,
	"Collisions": {
		"Down": 	{
			"Width": 8, "Height": 3,
			"Map": "..XXXX../XX....XX/...XX.XX"
		},
		"Right": 	{
			"Width": 5, "Height": 5,
			"Map": "..XXX/XX.../XX..X/XX..X/..XX."
		},
		"Up": 		{
			"Width": 8, "Height": 3,
			"Map": "XX.XX.../XX....XX/..XXXX.."
		},
		"Left": 	{
			"Width": 5, "Height": 5,
			"Map": ".XX../X..XX/X..XX/...XX/XXX.."
		}
	},

	"Placement Restriction": 2,

	"Source Image": "assets/living_room.png",
	"Source Rect": {
		"Down":		{"X": 0, "Y": 0, "Width": 128, "Height": 64},
		"Right":	{"X": 0, "Y": 128, "Width": 80, "Height": 96},
		"Up":		{"X": 0, "Y": 64, "Width": 128, "Height": 64},
		"Left":		{"X": 80, "Y": 128, "Width": 80, "Height": 96}
	},

	"Layers": {
		"Up": [
			{
				// Back of Long Couch, facing Up
				"Source Rect": {"X": 80, "Y": 224, "Width": 64, "Height": 32},
				"Draw Pos": {"X": 32, "Y": 32},
				"Depth": {"Tile": 2, "Sub": 1000}
			},
			{
				// Lower Arm of Couch facing Right
				"Source Rect": {"X": 160, "Y": 128, "Width": 32, "Height": 48},
				"Draw Pos": {"X": 0, "Y": 0},
				"Depth": {"Tile": 1, "Sub": 1000}
			},
			{
				// Lower Arm of Armchair facing Left
				"Source Rect": {"X": 192, "Y": 64, "Width": 32, "Height": 32},
				"Draw Pos": {"X": 96, "Y": 16},
				"Depth": {"Tile": 1, "Sub": 1000}
			},
			{
				// Upper Arm of Armchair facing Left
				"Source Rect": {"X": 192, "Y": 96, "Width": 32, "Height": 32},
				"Draw Pos": {"X": 96, "Y": 16},
				"Depth": 1
			}
			// Upper Arm of Couch facing Right & Table are already drawn by the base sprite 
		],
		"Right": [
			{
				// Back of Armchair facing Up
				"Source Rect": {"X": 0, "Y": 224, "Width": 32, "Height": 32},
				"Draw Pos": {"X": 32, "Y": 64},
				"Depth": {"Tile": 4, "Sub": 1000}
			},
			{
				// Lower Arm of Long Couch facing Right
				"Source Rect": {"X": 128, "Y": 0, "Width": 32, "Height": 64},
				"Draw Pos": {"X": 0, "Y": 16},
				"Depth": {"Tile": 3, "Sub": 1000}
			},
			{
				// Table
				"Source Rect": {"X": 176, "Y": 208, "Width": 16, "Height": 48},
				"Draw Pos": {"X": 64, "Y": 32},
				"Depth": 2
			},
			{
				// Upper Arm of Long Couch facing Right
				"Source Rect": {"X": 128, "Y": 64, "Width": 32, "Height": 64},
				"Draw Pos": {"X": 0, "Y": 16},
				"Depth": 1
			}
		],
		"Down": [
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
		],
		"Left": [
			{
				// Back of Couch facing Up
				"Source Rect": {"X": 32, "Y": 224, "Width": 48, "Height": 32},
				"Draw Pos": {"X": 0, "Y": 64},
				"Depth": {"Tile": 4, "Sub": 1000}
			},
			{
				// Lower Arm of Long Couch facing Left
				"Source Rect": {"X": 160, "Y": 0, "Width": 32, "Height": 64},
				"Draw Pos": {"X": 48, "Y": 16},
				"Depth": {"Tile": 3, "Sub": 1000}
			},
			{
				// Table
				"Source Rect": {"X": 176, "Y": 208, "Width": 16, "Height": 48},
				"Draw Pos": {"X": 0, "Y": 16},
				"Depth": 1
			},
			{
				// Upper Arm of Long Couch facing Left
				"Source Rect": {"X": 160, "Y": 64, "Width": 32, "Height": 64},
				"Draw Pos": {"X": 48, "Y": 16},
				"Depth": 1
			}
		]
	},

	"Seats": {
		"Down": [
			{
				// Left seat of Long Couch
				"Position": {"X": 2.5, "Y": 0},
				"Player Direction": 2
			},
			{
				// Center seat of Long Couch
				"Position": {"X": 3.5, "Y": 0},
				"Player Direction": 2
			},
			{
				// Right seat of Long Couch
				"Position": {"X": 4.5, "Y": 0},
				"Player Direction": 2
			},
			{
				// Seat of Armchair
				"Position": {"X": 1, "Y": 1},
				"Player Direction": 1
			},
			{
				// Top seat of Couch
				"Position": {"X": 6, "Y": 1},
				"Player Direction": 3
			},
			{
				// Bottom seat of Couch
				"Position": {"X": 6, "Y": 2},
				"Player Direction": 3
			}
		],
		"Right": [
			{
				// Left seat of Couch
				"Position": {"X": 2.5, "Y": 0},
				"Player Direction": 2
			},
			{
				// Right seat of Couch
				"Position": {"X": 3.5, "Y": 0},
				"Player Direction": 2
			},
			{
				// Top seat of Long Couch
				"Position": {"X": 1, "Y": 1},
				"Player Direction": 1
			},
			{
				// Center seat of Long Couch
				"Position": {"X": 1, "Y": 2},
				"Player Direction": 1
			},
			{
				// Bottom seat of Long Couch
				"Position": {"X": 1, "Y": 3},
				"Player Direction": 1
			},
			{
				// Seat of Armchair
				"Position": {"X": 2.5, "Y": 4},
				"Player Direction": 0
			}
		],
		"Up": [
			{
				// Top seat of Couch
				"Position": {"X": 1, "Y": 0},
				"Player Direction": 1
			},
			{
				// Bottom seat of Couch
				"Position": {"X": 1, "Y": 1},
				"Player Direction": 1
			},
			{
				// Seat of Armchair
				"Position": {"X": 6, "Y": 1},
				"Player Direction": 3
			},
			{
				// Left seat of Long Couch
				"Position": {"X": 2.5, "Y": 2},
				"Player Direction": 0
			},
			{
				// Center seat of Long Couch
				"Position": {"X": 3.5, "Y": 2},
				"Player Direction": 0
			},
			{
				// Right seat of Long Couch
				"Position": {"X": 4.5, "Y": 2},
				"Player Direction": 0
			}
		],
		"Left": [
			{
				// Seat of Armchair
				"Position": {"X": 1.5, "Y": 0},
				"Player Direction": 2
			},
			{
				// Top seat of Long Couch
				"Position": {"X": 3, "Y": 1},
				"Player Direction": 3
			},
			{
				// Center seat of Long Couch
				"Position": {"X": 3, "Y": 2},
				"Player Direction": 3
			},
			{
				// Bottom seat of Long Couch
				"Position": {"X": 3, "Y": 3},
				"Player Direction": 3
			},
			{
				// Left seat of Couch
				"Position": {"X": 0.5, "Y": 4},
				"Player Direction": 0
			},
			{
				// Right seat of Couch
				"Position": {"X": 1.5, "Y": 4},
				"Player Direction": 0
			}
		]
	},

	"Slots": {
		// The Area rectangle is in pixels, and is relative to the sprite for each rotation.
		"Down": [
			{
				"Area": {"X": 48, "Y": 43, "Width": 32, "Height": 13},
				"Depth": 2
			}
		],
		"Right": [
			{
				"Area": {"X": 64, "Y": 41, "Width": 16, "Height": 31},
				"Depth": 2
			}
		],
		"Up": [
			{
				"Area": {"X": 48, "Y": 11, "Width": 32, "Height": 13},
				"Depth": 0
			}
		],
		"Left": [
			{
				"Area": {"X": 0, "Y": 25, "Width": 16, "Height": 31},
				"Depth": 1
			}
		]
	}
}
```