all: RecoverStamina.dll stardew_valley

stardew_valley:
	$(error Please symlink your stardew valley game folder to ./stardew_valley)
RecoverStamina.dll: src/RecoverEndurance.cs stardew_valley/StardewValley.exe stardew_valley/MonoGame.Framework.dll stardew_valley/StardewModdingAPI.exe
	mcs -target:library -out:RecoverStamina.dll -r:stardew_valley/StardewValley.exe -r:stardew_valley/MonoGame.Framework.dll -r:stardew_valley/StardewModdingAPI.exe src/*.cs


