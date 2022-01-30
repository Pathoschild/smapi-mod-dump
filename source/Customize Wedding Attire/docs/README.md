**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/elizabethcd/CustomizeWeddingAttire**

----

# CustomizeWeddingAttire
Stardew Valley mod to let you customize what you wear at weddings.

# What This Mod Does
Normally, in Stardew Valley, when there is a wedding:
* If you selected the "male" body type (whether you're getting married or in the audience), you are put into a tuxedo.
* If you selected the "female" body type, your clothing remains the same.

This mod changes this, so that when there is a wedding:
* If you select the ``Tuxedo`` option in the config, you are put into a tuxedo outfit.
* If you select the ``WeddingDress`` option in the config, you are put into a wedding dress outfit. (Note that this is what you will wear to any wedding, not just your own!)
* If you select the ``NoClothingChange`` option in the config, your clothing is whatever you were wearing right before the wedding.
* If you select the ``GameDefault`` option in the config, your clothing decisions follow the default game logic.

In a multiplayer game, if one of the players wants to wear a different option from the default, the other players will also need to have this mod in order to see them wear the right clothing.

# How To Install
* Install [SMAPI](https://smapi.io), if you don't already have it installed
* Download this mod
* Unzip
* Move into your mods folder
* Optionally, install [Generic Mod Config Menu (GMCM)](https://www.nexusmods.com/stardewvalley/mods/5098) as well for easy configuration
  * If [GMCM Options](https://www.nexusmods.com/stardewvalley/mods/10505) is also installed then the configuration will show a live preview image
* Run the game once to create config.json if you're not using GMCM
* Enjoy!

# Compatibility
* This mod is compatible with multiplayer.
* This mod should be compatible with any mods that do not edit clothing. 
* If you are wearing Fashion Sense clothing when you attend a wedding, Fashion Sense overrides this mod exactly the same way as vanilla clothing. For example, if you're wearing a FS skirt and a vanilla top and get married with the ``Tuxedo`` config option selected, you will wear a tuxedo top and the FS skirt. 
* Get Glam has not been tested with this mod because it's very buggy. 
* Json Assets clothing should work the same as vanilla clothing, but this has not been tested.
* If you have a CP mod that edits the tuxedo outfit or the wedding dress outfit, you will most likely see them edited during the wedding. 

# Credits
* Misha for helping me test.
* ManInBlack, atravita, and Tlitookilakin for tons of help with the C# learning.
* spacechase0 for making GMCM and Pathoschild for making SMAPI.
* CopperSun for complaining about this issue in SDV, and ZoeDoll for making ``Ridgeside Rival Hearts - Jeric and Alex`` which notes this issue.
