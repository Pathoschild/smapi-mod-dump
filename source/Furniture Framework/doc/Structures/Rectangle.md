**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# Rectangle Format

If you are familiar with Content Patcher, it is the same data model used in the fields `ToArea` and `FromArea` of an `EditImage` patch.

A Rectangle has 4 properties, all the mesurements are ***in image pixels*** and must be integers:
- "X": the horizontal position of the top-left of the rectangle
- "Y": the vertical position of the top-right of the rectangle
- "Width": the width of the rectangle
- "Height": the height of the rectangle

A proper Rectangle would look like this:
```json
{"X": 64, "Y": 32, "Width": 32, "Height": 32}
```
or, if spread on multiple lines, like this:
```json
{
	"X": 64,
	"Y": 32,
	"Width": 32,
	"Height": 32
}
```

Keep in mind that negative Y is up and positive Y is down.