NOTE: This is for people who want to create custom animations or textures.

How to use custom sprite animations:
(The old way of dropping the sprites in the mod folder no longer works)

You can now use Content Patcher to the sprite animations.
The target names for the sprites are:
-The smoke sprite: "Traktori.IndustrialFurnace/SmokeSprite"
-The fire sprite: "Traktori.IndustrialFurnace/FireSprite"

You'll also need to edit the animation data files to toggle custom sprites on.
The target names for the data files are:
-The smoke data: "Traktori.IndustrialFurnace/SmokeAnimationData"
-The fire data: "Traktori.IndustrialFurnace/FireAnimationData"

You'll need to set
"UseCustomSprite": true
with CP if you want to change the sprites at all since by default the mod loads them from
-smoke: LooseSprites\\Cursors, (372, 1956, 10, 10)
-fire: TileSheets\\animations, (0, 1920, 256, 64)
The sprites' sizes must match those defined in the animation data.
For the smoke sprite that means it has to be SpriteSizeX in width and
SpriteSizeY in height. The fire sprite has to be
(SpriteSizeX * AnimationLength) in width and SpriteSizeY in height.


How to use seasonal textures:
(This approach technically still works but the preferred way is through Content Patcher)

Paste the textures to this folder. They should be named season_IndustrialFurnaceOn
and season_IndustrialFurnaceOff. So for every season you need 2 textures.

For example the winter textures would be named
winter_IndustrialFurnaceOn.png
winter_IndustrialFurnaceOff.png

If the mod doesn't detect seasonal textures, it looks for textures named
IndustrialFurnaceOn.png
IndustrialFurnaceOff.png

Content Patcher way:

Target names are
"Buildings/Industrial Furnace" - The off texture
"Traktori.IndustrialFurnace/FurnaceOn" - The on texture