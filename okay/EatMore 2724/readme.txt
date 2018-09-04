Stardew Valley - Eat More! v0.2.2 by crc-fehler

What about?
This mod forces you to eating more by reducing the stamina regained by sleeping.

How to install?
1. Install SMAPI? (if not allready done)
2. download this mod
3. extract its content into your Stardew Valley mod-folder ( ...\Steam\steamapps\common\Stardew Valley\Mods )
4. create a backup from your savegames befor launching the game (can be found here: "%appdata%\stardewvalley" )

How to deinstall?
Just delete the "EatMore" folder from your Stardew Valley mod-folder.
Savegames should still work after removing this mod.

Does it work in multiplayer?
Jep.
If a Client does not have this mod installed, its stamina-regeneration will be unaffected.

How it works?
If you go to sleep, you will only get 25% stamina back. (default value)
If you had a meal 2h befor you going to sleep, you will regenerate up to 50% stamina. (default value)
This mod also limit the amount of stamina you can regenerate in multiplayer by just laying in your bed.

Can i change some values?
Yes, open the config.json inside the mod-folder ( ...\Steam\steamapps\common\Stardew Valley\Mods\EatMore ) with your prefered text-editor.
There you can change follwing values:

	 StaminaRegeneration:		a value between 0 and 1 (0 = 0% and 1 = 100%) i.e. if you want to regenerate 50% stamina per sleep, change it to 0.5
	 StaminaRegenerationWellFeeded:	same as "StaminaRegeneration", is used instead if you had a meal 2h befor sleeping.

This mod offers translations for the following languages:
- englisch
- german
- spanish
- Portuguese

- Changelog -
0.2:	
	- Added: Allowing to change some values via config.json
0.2.1:	
	- Added: spanish translation
	- Fixed: Hint on consuming food does not show the correct percentage-value
0.2.2:
	- Added: portuguese translation