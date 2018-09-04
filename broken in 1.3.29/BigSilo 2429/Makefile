all: BigSilo.dll stardew_valley

stardew_valley:
	$(error Please symlink your stardew valley game folder to ./stardew_valley)
BigSilo.dll: src/BigSilo.cs stardew_valley/StardewValley.exe stardew_valley/MonoGame.Framework.dll stardew_valley/StardewModdingAPI.exe
	mcs -target:library -out:BigSilo.dll -r:stardew_valley/StardewValley.exe -r:stardew_valley/MonoGame.Framework.dll -r:stardew_valley/StardewModdingAPI.exe -r:./0Harmony.dll src/*.cs


