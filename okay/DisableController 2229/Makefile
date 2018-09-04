all: DisableController.dll stardew_valley

stardew_valley:
	$(error Please symlink your stardew valley game folder to ./stardew_valley)
DisableController.dll: src/DisableControllerMod.cs src/DisableController.cs stardew_valley/StardewValley.exe stardew_valley/MonoGame.Framework.dll stardew_valley/StardewModdingAPI.exe
	mcs -target:library -out:DisableController.dll -r:stardew_valley/StardewValley.exe -r:stardew_valley/MonoGame.Framework.dll -r:stardew_valley/StardewModdingAPI.exe src/*.cs -r:stardew_valley/0Harmony.dll


