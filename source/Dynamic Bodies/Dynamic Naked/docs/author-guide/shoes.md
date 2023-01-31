**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ribeena/StardewValleyMods**

----

← [author guide](../author-guide.md)

## Contents
* [Introduction](#introduction)
* [Icons changing colors](#icons-changing-colors)
* [Adding custom boots with JSONAssets](#adding-custom-boots-with-jsonassets)

## Introduction
After creating your [Boots folder and JSON file](../author-guide.md#shoes), that's really all you need to
do for creating custom display boots.

## Icons changing colors
The vanilla game didn't do a very good job of showing what your boots will look like on
your character - so this mod standardises all the icons into 5 colors based on the
Farmer sprite 3 colors. Refer to the [boots sprite](../../assets/Interface/springobjects_boots.png) of
this mod to see the 5 brown colors.

This means that when using the boots stainer/tailoring screens the mod generates a new
icon with the correct colors. The colors come straight from the shoes palette, and it 
calculates the additional 2 colors.

## Adding custom boots with JSONAssets/Dynamic Game Assets
If you are adding new boots with [JSONAssets](https://www.nexusmods.com/stardewvalley/mods/1720)
or [Dynamic Game Assets](https://www.nexusmods.com/stardewvalley/mods/9365)
there's not much you need to do - the name will fuzzy match like above, and the
new color palette you add will work automatically with the stainer provided
you've use the same 5 brown colors.

Refer to the [boots sprite](../../assets/Interface/springobjects_boots.png) of
this mod and create your icon using those 5 colors and it should work fine.