**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# How to define a custom Bed?

You'll only need to define one new field for a bed to work:


## Bed Spot

The Bed Spot determines where the player will appear when waking up. It is an integer [Vector](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Structures/Vector.md) mesured in tiles, starting from the top left of the Bounding Box.

## Other Info

The bounding box of the area where the game will prompt you to sleep is the rectangle inside the bed, one tile removed from the actual bounding box. For example, a 6x4 Bed will have a 4x2 centered area where the game will ask if you want to sleep.

If you want more customization for Bed Furniture, make a post on the Nexus page or ping me on Discord (on the Stardew Valley server).