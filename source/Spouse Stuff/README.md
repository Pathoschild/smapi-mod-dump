# SpouseStuff - A Stardew Valley mod
As the title suggests, this mod adds interactions to your spouse's stuff in the farmhouse of Stardew Valley. If you're married, of course.

## How to get it
First install and set up [SMAPI, the Stardew Valley Modding API](https://smapi.io) if you have not already done so.

When you have SMAPI working, download the mod from [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/4651) and unzip it into your StardewValley/Mods folder. Then you are ready to run Stardew Valley with SMAPI and enjoy the mod!

## How to work with it
First install the SMAPI extension in Visual Studio 2019 so you are able to develop and build Stardew Valley mods.

When you have the SMAPI extension up and running, clone this project with Visual Studio to start hacking! 

ModEntry.cs contains a Dictionary with each spouse name and a corresponding ISpouseRoom object, as well as a button press event listener. To modify/add interactions for a specific spouse, edit their corresponding class in the Spouses folder. You can get the tileX, tileY and faceDirection values from the SMAPI console whenever you're in the farmhouse and use the action button somewhere.

## Support and compatibility
All twelve possible spouses in the vanilla game are supported. At present this mod will not work correctly with mods that modify the layout of the spouse rooms, but support for the most popular of those is planned for a future release.