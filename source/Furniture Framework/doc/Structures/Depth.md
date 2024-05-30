**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# Depth Format

Every depth is split into 2 values:

## Tile

The "Tile" depth is required, it represents the full depth of a tile.  
Tile Depth = 0 is the tile at the top of the bounding box, bigger Tile Depth will push the depth down (south) of this. Negative Tile Depth will put the depth behind the base Sprite, Tile Depth bigger than Collisions.Height will put the depth in front of the Furniture (will overlap with stuff placed in front of the Furniture).

## Sub

The "Sub" depth is optional, it goes from 0 to 1000 (included) and allows for fine tuning in a tile. 0 will be at the top of the tile, and 1000 at the bottom.  
Because of a quirk in the game's code, there's a gap between `"Depth": {"Tile": N, "Sub": 1000}` and `"Depth": {"Tile": N+1, "Sub": 0}` to avoid layering issues with other objects placed behind and in front of the Furniture.

## Simple Depth

If you don't need to define a Sub Depth (defaults to 0), you can replace the Depth object with the Tile Depth itself.  
For example, this:
```json
"Depth": {"Tile": 2, "Sub": 0}
```
Is equivalent to this:
```json
"Depth": 2
```

# Visual explanation

// TODO