**Stardew Symphony Remastered** is a [Stardew Valley](http://stardewvalley.net/) mod that lets you
add music packs to Stardew Valley and play them without editing the game's default sound files. 

## Contents
* [For players](#for-players)
  * [Install](#install)
  * [Use](#use)
  * [Configure](#configure)
  * [Compatibility](#compatibility)
* [For content pack creators](#for-content-pack-creators)
* [For SMAPI mod creators](#for-smapi-mod-creators)
* [See also](#see-also)

## For players
### Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. [Install Stardust Core](https://www.nexusmods.com/stardewvalley/mods/2341).
2. [Install this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/1100).
3. Run the game using SMAPI.

### Use
1. Unzip any content packs you want to use into `Mods`.
2. Press `L` in-game to open the music menu, which will show icons representing the loaded music
   albums. (You can click the left/right arrows to scroll if you have more than seven albums loaded.)
3. Choose an album icon to see a list of available songs on the right.
4. Click a song to select it. You can click the play/stop button to preview the song.
5. Next configure when the music will play by choosing options on the right. You can play it during
   a specified season, festival, event, date, weather, time of day, location, or menu. The
   conditions can be very broad (like 'play in winter') or very specific (like 'play during snowy
   winter nights at the Saloon on Wednesdays'). Note that when two songs can be applied, the one
  with the more specific conditions will be played.
6. **Make sure you click 'Add' when you're done!**

Your music options are saved when the game saves, so they'll be lost if you exit without saving.

### Configure
The mod creates a `config.json` file in its mod folder the first time you run it. You can open that
file in a text editor to configure the mod.

These are the available settings:

setting    | what it affects
---------- | -------------------
`EnableDebugLog` | Default false. Whether to show debug logs in the SMAPI console.
`MinimumDelayBetweenSongsInMilliseconds` | Default 5000. The minimum delay between songs in milliseconds.
`MaximumDelayBetweenSongsInMilliseconds` | Default 60000. The maximum delay between songs in milliseconds.
`KeyBinding`                 | Default `L`. The key binding to open the menu music.
`WriteAllConfigMusicOptions` | Default false. Whether to write a JSON file for every possible option for a music pack. Use at your own risk!
`DisableStardewMusic`        | Default false. Whether to completely disable the Stardew Valley OST.

### Compatibility
Stardew Symphony Remastered is compatible with Stardew Valley 1.3+ on Linux/Mac/Windows, both
single-player and multiplayer. There are no known issues in multiplayer (even if other players
don't have it installed), but it will only affect you.

## For content pack creators
To create a content pack:

1. [Create a standard content pack](https://stardewvalleywiki.com/Modding:Content_packs), using `Omegasis.StardewSymphonyRemastered` as the 'content pack for' ID.
2. Add an `icon.png` image, which is the album logo to show in-game.
2. Create a `songs` subfolder containing the `.mp3`, `.ogg`, or `.wav` files to include. The file
   names (without extensions) will be shown in-game as the song names.

## For SMAPI mod creators
You can reference Stardew Symphony in your own SMAPI mods in order to add new events, festivals,
locations, and menus for music selection.

1. Reference the Stardew Symphony DLL directly (see [how to reference a DLL](https://stackoverflow.com/questions/12992286/how-to-add-a-dll-reference-to-a-project-in-visual-studio)).
2. Add `Omegasis.StardewSymphonyRemastered` as a dependency (see [manifest dependencies](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest#Dependencies)).
3. Now you can access the Stardew Symphony API to add music contexts:

    ```cs
    using System;
    using Microsoft.Xna.Framework;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewSymphonyRemastered;

    namespace YourProjectName
    {
        /// <summary>The mod entry point.</summary>
        public class ModEntry : Mod
        {
            public override void Entry(IModHelper helper)
            {
                StardewSymphonyRemaster.SongSpecifics.addLocation(“NameOfLocation”);
                StardewSymphonyRemaster.SongSpecifics.addEvent(“UniqueEventID”);
                StardewSymphonyRemaster.SongSpecifics.addFestival(“FestivalName”);
                StardewSymphonyRemaster.SongSpecifics.addMenu(typeOf(MyMenu));
            }
        }
    }
    ```

Happy modding!

## See also
* [Nexus mod](http://www.nexusmods.com/stardewvalley/mods/425)
* [Discussion thread](https://community.playstarbound.com/threads/stardew-symphony-add-music-packs-to-stardew-valley.115686/)
