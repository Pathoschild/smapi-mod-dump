**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# How to define a Custom TV?

To make a TV work, you only need 2 new fields:
- "Screen Position"
- "Screen Scale"

```json
"Screen Position": {"X": 6, "Y": 0},
"Screen Scale": 2
```

The screen position is an integer [Vector](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Vector.md) in pixels, starting from the top left of the sprite.

The screen scale might not be very intuitive: most TVs have a screen scale of 4, except the Plasma TV and the Tropical TV with a scale of 2, so you can base your TV's screen scale from these values.