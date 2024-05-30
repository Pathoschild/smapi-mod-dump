**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# What are Directional Fields?

Directional Fields is a type of field that *can* depend on the rotation of the Furniture.

When a field is directional, its value can either be itself, or a dictionary with rotation names as keys and the actual data fields as values, the rotation names being defined in the Furniture's [Rotations field](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#rotations).

Let's take the Furniture's [Source Rect field](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#source-rect) as an example. If you choose to have the same Source Rectangle for every rotation, the field will look like this:
```json
"Source Rect": {"X": 0, "Y": 0, "Width": 16, "Height": 32}
```
But if you want each rotation to use a different part of the spritesheet (which you often do), you need to use the directional variant of this field.  
For this example, we'll take the "Chair Test" Furniture of the Example Pack, it has `"Rotation": 4`, if you read about the Furniture Rotations field, you'll now that its rotation keys are "Down", "Right", "Up" and "Left". This is how its Source Rectangle looks like:
```json
"Source Rect": {
	"Down":		{"X": 0,  "Y": 0, "Width": 16, "Height": 32},
	"Right":	{"X": 16, "Y": 0, "Width": 16, "Height": 32},
	"Up":		{"X": 32, "Y": 0, "Width": 16, "Height": 32},
	"Left":		{"X": 48, "Y": 0, "Width": 16, "Height": 32}
}
```

For the Source Rect field, a value for all possible rotations are required, because the Furniture Framework need to know how to display the Furniture at all possible rotations. This is not true for other Directional Fields, most of them don't need to have values for all rotations.

A directional field can also be an array instead of an object. This works the same way, let's take a Furniture's [Layers field](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#layers) as an example. Here what it looks like when it is non-directional:
```json
"Layers": [
	// My layers applied to all directions
]
```

And what it looks like when directional:
```json
"Layers": {
	"Down": [
		// My layers applied when the rotation is "Down"
	],
	"Right": [
		// My layers applied when the rotation is "Right"
	],
	"Up": [
		// My layers applied when the rotation is "Up"
	],
	"Left": [
		// My layers applied when the rotation is "Left"
	]
}
```

As said earlier, the "Layers" field does not require a value for every direction, if your Furniture only has Layers when it's facing Right or Left, but not when it's facing Up or Down, you can reduce the field to this:
```json
"Layers": {
	"Right": [
		// My layers applied when the rotation is "Right"
	],
	"Left": [
		// My layers applied when the rotation is "Left"
	]
}
```
