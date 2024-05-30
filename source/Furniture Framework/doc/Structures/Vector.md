**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# Vector Format

A vector is a general term that is used for multiple things in this documentation:
- Point
- Offset (distances)
- Speed

A Vector has 2 fields: "X" and "Y", representing its horizontal and vertical components. When encountering a Vector field in this documentation, take good notice if it is "integer" (whole numbers) or "decimal", this is very important.

Here's an example of a valid **integer** Vector:
```json
{
	"X": 16, "Y": 32
}
```

Keep in mind that negative Y is up and positive Y is down.