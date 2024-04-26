**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/HeyImAmethyst/SkinToneLoader**

----

# Skin Tone Loader
Skin Tone Loader is a Stardew Valley mod that adds the ability to add custom skin tones to your farmer.

The functionality for this is an updated and modified version of the custom skin tone code from the mod [Get Glam](https://www.nexusmods.com/stardewvalley/mods/5044) by MartyrPher.

# Install

- Make sure the game is updated to at least version 1.6.4, otherwise it won't work.
- Install the latest version of [SMAPI](https://smapi.io/).
- Install the mod from Nexus and add it to your mods folder.
- Unzip any Skin Tone Loader content packs into your Mods folder to install them.
- Run the game using SMAPI.

# Uninstall
- Simply delete the mod from your mods folder (Note that deleting this mod then loading the game afterward will reset the skin tone to the first default one).

**Creating a skin tone texture**

The size of one skin tone is a 1 x 3 texture (1 row of 3 pixels)

If you want to add multiple skin tones in a spritesheet your spritesheet will need to be of the size: number of skin tones x 3

The process for adding skin tone content packs for this mod is the same for Get Glam except that some things have been renamed.

**Adding skin tones to a content pack**

- Create a folder in the pack named SkinTone.
- Add in the skin tone textures with the name skinTones.png to the SkinTone folder.

Just like with Get Glam, Skin Tone Loader only supports one skinTones.png per content pack and only adds up to 4096 skin tones including default skin colors from different content packs