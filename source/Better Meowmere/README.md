**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TheMightyAmondee/BetterMeowmere**

----

# Better Meowmere #

Compared to the original in Terraria, the meowmere is a little lackluster in Stardew Valley. Better Meowmere aims to rectify this problem by improving the behaviour of the sword in [Stardew Valley](https://www.stardewvalley.net/) to be more similar to Terraria (along with all the annoying sound effects).

Better meowmere adds a rainbow cat projectile (altered to look a little more stardew-y) that it shot straight ahead when using the secondary attack. The attack of the sword can also be buffed to be as overpowered as it is in Terraria (the projectile does do less damage than the sword).

A few config options are also included to customise your meowmere experience. Listed below:

- ProjectileSounds (default is All): All: all sound effects, Some: no sound effects when bouncing off walls, None: No sound effects.
- ProjectileIsSecondaryAttack (default is true): Projectile is shot using the secondary attack, set this to false if you want the projectile to be shot using the primary attack instead (this would be more in line with how the weapon functions in Terraria)
- BuffAttack (default is false): Set this to true to make the sword deal more damage (and make the sword Terraria level overpowered). 

Changes to buff attack will only be applied to newly acquired meowmeres and to any meowmeres stored in the player inventory upon saving. Ensure the meowmere is in your inventory when saving if the BuffAttack setting is changed to ensure changes apply.

Better Meowmere supports Generic Mod Config Menu to change the config settings in game.

### Installation: ###

1. Install SMAPI 4.0.0 or newer
2. Download the mod
3. Extract (unzip) the contents of the mod download into your Mods Folder.
4. The config.json file will generate in the mod folder after the game has been run once with the mod installed.

### Uninstalling: ###

If you wish to reset the meowmere to it's default attack strength before uninstalling, a few special actions are needed. Otherwise, the mod can simply be removed from the Mods folder to uninstall!

Failing to complete the special actions will result in a permanently buffed meowmere. Not game breaking, but probably not intended!

To reset the meowmere damage:

1. Set BuffAttack to false in the config.json or by using GMCM.
2. Sleep for one night with the meowmere in your inventory.
3. The sword should now have it's default attack. Remove the mod from your Mods folder to uninstall the mod.

### Compatibility: ###

Should be fully compatible with multiplayer although this has not been tested. Let me know if you encounter any issues!

### Version History: ###
1.0.0 - initial release
