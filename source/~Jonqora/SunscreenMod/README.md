<details>
  <summary><i>TRANSLATION INFO:</i></summary>
  
  - No translations for this mod are available yet.
</details>

---

# UV Index (Sunscreen Mod)
*Apply sunscreen on sunny days... or you'll get sunburnt and the villagers will laugh at you!*

## Features
This mod adds functional sunscreen to the game that protects you from sunburn. If you spend too long outdoors without sunscreen on a sunny day, you'll get sunburnt... but it won't show up until the next day!

Sunburn will temporarily change your character's skin color. **NPC villagers who see your tomato-red skin will react in shock!** Each villager has a unique set of randomized reactions.

Active sunburn damage lowers your starting health and energy for the day and gives you a slight speed debuff. More severe sunburn damage will take longer to heal (up to 3 days without treatment). 
> *NOTE: If you only want the skin color changes, you can configure this mod to disable any debuffs.*

Watch the weather channel on TV to be notified of sun conditions for the next day. Higher UV index means greater sunburn risk and faster burn development. UV intensity is semi-randomly determined and is influenced by season, weather conditions, and time of day. Early mornings or evenings spent outside should be very low-risk even on the worst days. But during midday and summer afternoons... *watch out*.

You can buy protective Sunscreen lotion from Harvey's shop for 100g. You can also buy Aloe Vera Gel (500g), which you can use to treat a sunburn to speed your healing process and reverse *some* of the damage.

(If you uninstall this mod while still sunburnt, your character's skin color and everything else will return to normal.)

### Translation Support
This mod is designed to support built-in translations. If you would like to help translate this mod to other languages, *watch for a link coming soon!* Submitted translations will be included in future mod updates.


## User Information
### COMPATIBILITY
- Stardew Valley v1.4 or later;
- Linux, Mac, Windows, and Android.
- Single-player and multiplayer. Can be installed by some OR all players - see Multiplayer section for details.

### INSTALLATION
- [Install the latest version of SMAPI.](https://smapi.io/)
- Install the latest version of [Json Assets.](https://www.nexusmods.com/stardewvalley/mods/1720)
- Download this mod from [Nexus](https://www.nexusmods.com/stardewvalley/mods/6676) or the [GitHub Releases](https://github.com/Jonqora/StardewMods/releases) list.
- Unzip the mod and place the `UVIndex` folder inside your `Mods` folder.
- Run the game using SMAPI.

### USING THE MOD
The mod should be active as soon as you've installed it correctly. You can check the TV weather channel to see the UV forecast, and you can buy SPF60 Sunscreen and Aloe Vera Gel lotions from Harvey's clinic. When you take sun damage, it will show up as a sunburn **the following day**. Then you can run around town as red as a lobster and see all the villager reactions. ;)


## Multiplayer
UV Index mod should be fully multiplayer-compatible. (If you find bugs, please report them in the [nexusmods comments](https://www.nexusmods.com/stardewvalley/mods/6676/?tab=posts)!) All features of this mod should work even if you are the only person who uses it! This mod only affects gameplay for players who install it.

To make sure the skin colors used for sunburned players are synced in multiplayer, use the `BurnSkinColorIndex` config setting. Choose three unique skin color values (ranging from 1-24); be sure to choose skin color choices that are not already being used for any player character! Make sure all players who are using the mod have the same skin color values in their UV Index config, arranged in the same order. Then you should be able to see each others' sunburns properly.

If you want to see and poke fun at other players' sunburns but don't want to get sunburnt yourself, you can install this mod and set `EnableSunburn` in the config to `false`.


## Config Settings
After running SMAPI at least once with UV Index installed, a `config.json` file will be created inside the `UVIndex` mod folder. Open it in any text editor to change your config settings for UV Index.

**Optional:** UV Index mod includes [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (GMCM) support. If you download this optional mod, you can use a settings button in the Stardew Valley menu screen to change your UV Index config while the game is running.


- **EnableSunburn:** Enables all sunburn effects for the current player. Defaults to `true`. (Turn this off if you're in multiplayer and want to see other players' sunburns but never get sunburnt yourself.)

- **SunburnSeasons:** Enables or disables sunburn chance during certain times of year. Defaults to `"AllSeasons"`.
  - `"SummerOnly"` - You can only get sunburnt on summer days.
  - `"SpringSummerFall"` - You can get sunburnt during spring, summer, and fall; sunburn risk is highest in summer.
  - `"AllSeasons"` You can get sunburnt in any season, but sunburn risk in winter is very low.

- **WeatherReport:** Reports tomorrow's maximum UV Index on the TV weather channel. Defaults to `true`.

- **SkinColorChange:** Changes player skin display color to an appropriate shade of red when sunburnt. Defaults to `true`.

- **VillagerReactions:** Villagers react in shock when they see a sunburnt player. Defaults to `true`.


- **SunscreenDuration:** How long sunscreen protection will last after applying. Default is `3` in-game hours. Should work as intended even with other mods like TimeSpeed.

- **HealthLossPerLevel:** Loss in new day starting health per level of sunburn damage (moderate sunburn is x2, severe sunburn is x3). Defaults to `20`.

- **EnergyLossPerLevel:** Loss in new day starting energy per level of sunburn damage (moderate sunburn is x2, severe sunburn is x3). Defaults to `50`.

- **SunburnSpeedDebuff:** Active sunburn (no matter what severity) gives a `-1` debuff to movement speed. Defaults to `true`.


- **BurnSkinColorIndex:** **(Only used in multiplayer.)** Specifies which skin color options to replace in-game and use for sunburnt players. Defaults to `[ 19, 20, 21 ]`. (The original colors for the default values are red, purple, and yellow - UV Index mod changes them to red, dark red, and even darker red.)


- **DebugMode:** Logs noisy info messages to the SMAPI console for Jonqora to stress over. Defaults to `false`.


## Notes
### ACKNOWLEDGEMENTS
* *Created for the Stardew Valley Discord Summer 2020 modding contest!*
* Many thanks to [Hime](https://twitter.com/himearts) for volunteering to create the new sunscreen and aloe gel sprites!
* Thanks to [Sakorona](https://www.nexusmods.com/stardewvalley/users/2920064) for sharing some helpful time-wrangling C# code!
* Much gratitude to ConcernedApe and [Pathoschild](https://www.nexusmods.com/stardewvalley/users/1552317?tab=user+files)!
* Thanks to those who provided help and support in the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) #making-mods channel.

### SEE ALSO
* Source code on [GitHub](https://github.com/Jonqora/StardewMods/tree/master/SunscreenMod)
* Check out [my other mods](https://www.nexusmods.com/users/88107803?tab=user+files)!