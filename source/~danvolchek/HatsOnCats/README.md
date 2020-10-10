**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/danvolchek/StardewMods**

----

# Hats On Cats


This is mod is unreleased because it's not finished.

## How it works

The mod intercepts calls to `SpriteBatch.Draw`, determines which frame of a sprite of an NPC/animal/monster/etc is being drawn, and overlays a hat onto the thing's head. Unreleased because each frame of every sprite requires configuration of where to put the hat because the head position changes. This is a ton of work and I only ever completed it for Penny. The framework is ready to use though - just missing configuration.