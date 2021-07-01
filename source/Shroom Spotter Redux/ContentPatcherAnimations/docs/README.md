**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/strobel1ght/StardewValleyMods**

----

# Content Patcher Animation
This lets you animate textures in Content Patcher with the "EditImage" Action.

It adds two new properties - "AnimationFrameTime" which is the amount of ticks between each frame
(in game ticks, which is 60 per second), and "AnimationFrameCount" which is how many frames there
are.

All frames must be next to each other horizontally in the same "FromFile" image.

See [this CP mod](https://spacechase0.com/files/sdvmod/ContentPatcherAnimationTest.zip) for an
example.

Each patch must have a unique "LogName".

![](screenshot.gif)

## See also
* [Release notes](release-notes.md)
