/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

How to use custom sprite animations:

Paste to this folder for the mod to detect.
The default sprites are not included since I don't want to be
redistributing the games assets. The mod automatically detects
if you're using custom prites for the animations.

SmokeSprite.png (properties defined in SmokeAnimation.json)
-the default sprite is from LooseSprites\\Cursors, (372, 1956, 10, 10)
-has to be SpriteSizeY in height and SpriteSizeX in width

FireAnimation.png (properties defined in FireAnimation.json)
-the default sprite is from TileSheets\\animations, (0, 1920, 256, 64)
-has to be SpriteSizeY in height and (SpriteSizeX * AnimationLength) in width


How to use seasonal textures:

Paste the textures to this folder. They should be named season_IndustrialFurnaceOn
and season_IndustrialFurnaceOff. So for every season you need 2 textures.

For example the winter textures would be named
winter_IndustrialFurnaceOn.png
winter_IndustrialFurnaceOff.png

If the mod doesn't detect seasonal textures, it looks for textures named
IndustrialFurnaceOn.png
IndustrialFurnaceOff.png