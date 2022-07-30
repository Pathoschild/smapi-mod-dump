**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/vgperson/RangedTools**

----

# RangedTools
A mod for Stardew Valley that allows you to extend the range of tools and seed/object placement.

## Description

Playing with mouse controls, but find it annoying walking up to things just to get in range? This mod lets you customize the maximum range of tools and object placement, allowing you to use tools on tiles further away, plant seeds easily, and place objects at range!

Note that this extended range is generally **only applied when mouse-clicking**, and things will otherwise work as usual. This mod is thus mainly intended for **mouse and keyboard** play, where you have a free-moving cursor. I don't currently plan on adding any sort of "controller support."

Also, nothing else about your tools is affected. They still use the same animation, consume the usual amount of stamina, and can be upgraded as normal.

## Installation

1. Install the latest version of SMAPI.
2. Unzip the mod folder into Stardew Valley/Mods.
3. Run the game using SMAPI.

## User Configuration

**NOTE:** Not much is affected under the default settings, so make sure to change these to what you want! Either edit the **config.json** file directly, or if you have [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) installed, open Mod Config from the title screen or options menu.

*Config menu text translated into English, Japanese, Ukrainian (provided by ChulkyBow)*

**AxeRange / PickaxeRange / HoeRange / WateringCanRange (1 or greater, -1 for unlimited range)**

The maximum range of each tool, in tiles.

In the base game (and under default settings), all tools have a range of 1, which means you can reach things 1 tile away, or more specifically, act on any tile within a 3x3 square centered on your farmer. Range 2 is a step further: anything within 2 tiles away, AKA any tile within a 5x5 square centered on your farmer. This number can go as high as you want. If you just want a little more leniency with how close to the "target tile" you have to be, I recommend range 2.

Setting a tool's range to -1 removes all range restrictions, allowing you to target any tile on screen.

**ScytheRange / WeaponRange (1 or greater, -1 for unlimited range)**

Same as the tool ranges above, but for the scythe and melee weapons.

These work a bit differently from the single-tile-target tools above, with a dynamic hitbox that changes over the course of the animation. So instead, this considers the default hitboxes as having a "1 tile" range, and on higher settings, expands that hitbox out in all directions. This does not account for the direction you face, so the hitbox will also be expanded behind the player.

Setting these ranges to -1 removes all range restrictions, hitting all tiles on the map. Note that this may cause some lag if many objects are hit at once.

**SeedRange / ObjectPlaceRange (1 or greater, -1 for unlimited range)**

Same as the tool ranges above, but for seeds/fertilizer and other placeable objects respectively. For instance, setting SeedRange to -1 will basically let you stand still and plant seeds, and setting ObjectPlaceRange to -1 makes it easier to move machines and decorations around.

**Note:** Placing Crab Pots always uses default range (1 tile), as extended range would allow you to place a Crab Pot somewhere that's too far away to retrieve it again. Sorry.

**CenterScytheOnCursor / CenterWeaponOnCursor (true or false)**

Changes the point of origin for scythe/weapon swings from the player to the cursor position. This makes them behave more like single-tile-target tools with extended range, where you can just stand still and click where you want to use it.

**AxeUsableOnPlayerTile / PickaxeUsableOnPlayerTile / HoeUsableOnPlayerTile (true or false)**

In the base game (or with false setting), you cannot use a tool on the tile you're currently standing on, with the exception of the Watering Can. Setting these options to true will allow that tool to be used on the tile you're standing on. Recommended for the Hoe if you find tilling soil unwieldy.

**ToolAlwaysFaceClick / WeaponAlwaysFaceClick (true or false)**

Makes your farmer face toward the cursor when you click to use a tool or melee weapon respectively, even if you click outside the range. You may find this more natural, especially for melee weapons, as you'll at least use the tool/weapon in the general cardinal direction you clicked in.

**ToolHitLocationDisplay (0, 1, or 2)**

Affects how the Tool Hit Location display is determined (only when that game option is enabled).

**0 (original logic):** Indicates the targeted tile according to the base game ranges.

**1 (new logic, default):** Indicates the targeted tile taking into account extended ranges.

**2 (combination):** Indicates both of the above. May be desirable to know what will be targeted should the cursor go beyond the custom range.

**UseHalfTilePositions (true or false)**

Enabled (true) by default, this changes range calculations to feel more equal on all sides no matter where you're standing, rather than feeling lopsided when standing "between" tile units.

Specifically, it rounds the player's position for calculation to the nearest half-tile (rather than just rounding down to a whole tile), then determines what tiles are within reach based on that. For instance, if the current tool has a range of 2 and you're horzontally positioned between two whole tile units, your horizontal range will effectively become "2.5" on both sides - the 2 tiles left of the left tile you're standing on, and the 2 tiles right of the right tile you're standing on.

**AllowRangedChargeEffects (true or false)**

Whether to use the custom range for charged tool use as well. If enabled (true), charge effects will activate starting from the tile the cursor is over when you release the button. (The charge-up animation should indicate the actual tiles that will be affected regardless of Tool Hit Location setting.) If disabled, they will always start from your farmer, like normal.

**AttacksIgnoreObstacles (true or false)**

Whether to let attacks ignore obstacles. Normally, you cannot hit monsters if there are obstructions like rocks or walls in the way, but this can result in awkward behavior when using an increased range for melee weapons (or even with the default range, you might just want to be able to hit monsters through rocks). Setting this to true will remove the obstacle check entirely.

**CustomRangeOnClickOnly (true or false)**

By default (true), this mod only takes effect when pressing a "use tool" button that is a mouse button. If you want it to work even when pressing a keyboard or controller "use tool" button, set this option to false. This will cause it to try and target the current cursor position always, so it's not recommended if you aren't actively moving the mouse around.

## Possible Mod Conflicts

This mod affects a handful of functons, particularly the function that returns what tile a tool acts upon (for the sake of tool range), as well as one of the range-checking functions (for the sake of seed/object placement range). It also overrides drawing of the Tool Hit Location indicator in the player's draw function to follow the user setting. I aimed to make these as unobtrusive as I could.

It's possible that similar tool functionality mods operating in the same area may not work, or cause this mod to not work. Through both general and specific fixes, I've resolved conflicts with mods like Expanded Storage, Pipe Irrigation, and Hoe and Water Direction; other conflicts may or may not be resolvable on a case-by-case basis.

## Download

[Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/6935)
