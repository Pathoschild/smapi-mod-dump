**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/BleakCodex/SpritesInDetail**

----

Note: This is a Modding Resource, while possibly required by other Content Packs, it does not have an effect by itself.

Sprites in Detail


Installation
Extract ZIP to your Stardew Valley/Mods directory


Requirements
SMAPI
Content Patcher


What is Sprites in Detail?

Sprites in Detail is a SMAPI Modding Resource akin to ContentPatcher allowing the Visual replacement of sprites with higher resolution versions.
Multiplayer should work, but all changes are client-side and, if mods differ, will not be visible to other players.
﻿
Unmodded, sprites are limited to a 16x32 rectangle:


With this mod, the sprites are instead given 64x128 pixels, however, when drawn into Stardew, they will be scaled so that the sprite will have 32x64 pixels to occupy the normal space.


In effect, this grants the ability to draw higher resolution sprites, and also provides additional height and width to extend the original sprite.
                                      
*Abi is feeling a little fuzzy*﻿




How does Sprites in Detail work?

Sprites in Detail follows a similar render-focused approach as other high-compatibility Sprite alteration libraries such as HDSprites, or SpriteMaster.

SiD textures are loaded but do not interact with Stardew's logic until the actual Draw operation. During a Draw, SiD will instead draw the texture with the specifications desired.


The benefit of this approach is that no sizing, bounds, or other logic is impacted by a SiD texture. This maintains a very high level of compatibility with Events, Pathing, and bounding calculations.

The largest negative is collision boxes of the NPCs remain exactly as they were.
If you make a particularly wide sprite, the character will still be able to get exactly as close as before:

*Gus thinks you might be a little too close*




How do I use Sprites In Detail? 
For Mod Authors, usage is similar to other SMAPI Content Packs:
1.) Create your higher resolution SpriteSheet
       Here is how you can do it in Aseprite:
1. Increase the size of the spritesheet to 400%
﻿2. Resize down the sprites to 32x64
﻿They should be centered and located at the bottom of sprite area
﻿* One tip is that if you hold SHIFT while resizing height, it will keep the ratio and positioning



2.) Create a content.json
﻿The format of the JSON as minimum will need to contain:
﻿{
﻿﻿"Sprites": [
﻿﻿﻿{
﻿﻿﻿"Target": "REQUIRED {The Target Asset, this will be the directory/name for base-game assets, or the chosen name for other mod's assets}",
﻿﻿﻿"FromFile": "REQUIRED {The location of your replacement asset}",﻿
﻿﻿﻿"BreathType": "{Male|Female|None - Used to determine which pixels will be used to 'breathe'}",
﻿﻿﻿"When": {
﻿﻿﻿﻿OPTIONAL - ContentPatcher Conditional Token Object﻿
﻿﻿﻿}
﻿﻿﻿},
﻿﻿...
﻿﻿]
﻿}
﻿


3.) Determine if you want to handle the breathing texture
    Stardew Valley will draw a second texture for any 'Breather' NPCs.
﻿SiD exposes three basic options:
﻿﻿1.) Male - Will use the same sprite locations and dimensions as vanilla Stardew for Adult Male NPCs (i.e. (24, 98) as the upper left point of a 16x16 rectangle)
﻿﻿2.) Female - Will use the same sprite locations and dimensions as vanilla Stardew for Adult Female NPCs (i.e. (24, 100) as the upper left point of a 16x8 rectangle)
﻿﻿3.) None - Will Disable the breathing texture completely


Alternatively, you can specify the bounds of this texture manually, which is useful you've increased the height or width of the sprite:
ChestSourceX - The position relative to the upper left corner of the Sprite for the X position of the breath texture (24 for an unaltered Male SiD Sprite)
ChestSourceY - The position relative to the upper left corner of the Sprite for the Y position of the breath texture (98 for an unaltered Male SiD Sprite)
ChestSourceWidth - The width of the rectangle to be used (16 for an unaltered Male SiD Sprite)
ChestSourceHeight- The height of the rectangle to be used (16 for an unaltered Male SiD Sprite)
ChestAdjustX - A value representing the number of pixels to adjust the breath texture left/right. Higher values move the texture to the right.
ChestAdjustY - A value representing the number of pixels to adjust the breath texture up/down. Higher values move the texture down.

*ChestAdjustX and ChestAdjustY are slightly difficult to determine. They impact the origin of the breath texture (its center point) and are relative to the default center of the Chest rectangle (31,106). 
In addition the Y value needs to be multiplied by 2.

For example, if you increase the height of your sprite and decide you don't want them breathing from the groin:
﻿

You could configure the Sprite:
        {
            "Target": "Characters/Lewis",
            "FromFile": "assets/Lewis.png",
            "ChestSourceX":24,
            "ChestSourceY":89,
            "ChestSourceWidth": 16,
            "ChestSourceHeight": 20,
            "ChestAdjustX": 0,
            "ChestAdjustY": -14
        }


Which would result in a much more normally breathing Lewis:



4.) Conditional Sprites
SiD supports ContentPatcher's conditional tokens/queries.
To use, simply specify a When attribute in the Sprite following ContentPacher's format.
"When":
  {
     "Weather": "rain"
  } ﻿
The sprite will only show on Rainy days.

In addition, any config values specified in the config.json will be accessible in this area as well:
config.json
{
  "Enabled": "True",
  "GusSprite": "SiD"
}

content.json
﻿{
﻿﻿"Sprites": [
{
"Target": "Characters/Gus",
"FromFile": "assets/gus.png",﻿
"BreathType": "Male",
"When": {
﻿﻿﻿﻿﻿"GusSprite":"SiD"﻿
}
}
﻿﻿]
﻿}

Any defined config.json values will appear in the Mod Config Menu if Generic Mod Config Menu is installed

If you'd like to use a config token in a Content Patcher Query, you will have to refer to it with your ContentPack's UniqueId:
"When":
    {
       "Query: '{{Weather}}' = 'storm' AND {{BleakCodex.CrazyMustacheGus/EnableDuringStorms}} = 'true'": true
    }


4.) Finally, create manifest.json
Follow the SMAPI Manifest Format
You will need a ContentPackFor entry pointed to SiD:﻿
"ContentPackFor": {
"UniqueID": "BleakCodex.SpritesInDetail"
},

In addition, if you would like to use the Content Patcher Token/Conditionals (the When attribute), you must add a required Dependency to ContentPatcher pointing to version 1.22.0 or later:
"Dependencies": [
{
"UniqueID": "Pathoschild.ContentPatcher",
"MinimumVersion": "1.22.0"
}
]


Example
Please refer to the "Giant Crazy Mustache Gus" for an example of the mod setup


Compatibility
While SiD should be highly compatible, I have only tested on the Windows version of Stardew with the latest SMAPI version. 
Please feel free to notify me if you find this mod compatible with another version.
Windows﻿﻿✅
Linux﻿       ❔
Android﻿   ❔
Mac﻿﻿❔
Split-Screen    ❔



Feature Requests
If desired, I can add additional sprite resolutions.
Please reach out here or on GitHub if you need additional functionality around the Conditional/Token integration.
Otherwise, I intend to keep this as lean as possible to make the transition to any future version of SMAPI minor.

Source



*In addition to the Permissions specified, I give permission for anyone to distribute these files alongside another mod or modpack, so long as the mod/modpack is distributed for free.
