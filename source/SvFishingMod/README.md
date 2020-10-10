# SvFishingMod
Small and simple **StardewValley Mod** which allows user customization of the Fishing mechanics included in the game, with features like *difficulty, rewards and condition tweaks*, as well as *auto-reeling* the hooked fish and overriding fish types regardless of the area where the player is located.

# Requirements
- StardewValley v1.4.5 or greater.
- SMAPI v3.6.1 or greater.

```
The mod could work on previous versions, but it was not tested. Your mileage may vary.
```

# Downloads
For mod downloads, please refer to the following sections.

- [Releases](https://github.com/KDERazorback/SvFishingMod/releases) section on Github

# Installation

- Decompress the downloaded zip file.
- Copy and paste the descompressed files in a new directory named `SvFishingMod`. The directory structure should be as follows:
    ```
    + SvFishingMod
        - SvFishingMod.dll
        - SvFishingMod.pdb
        - manifest.json
        - svfishmod.json (optional)
    ```
- Move the `SvFishingMod` folder to the `Mods` directory of your StardewValley installation. If you dont have a `Mods` directory, please ensure you have already downloaded and installed **SMAPI** from the [SMAPI Homepage](https://smapi.io/) and have launched the game at least once after its installation.

# Usage
When first run, the mod includes a default configuration that completely skips the Fishing minigame when a fish is hooked, automatically reeling it up the player's inventory. No other customizations are made, fish types that can be reeled on specific map areas are unchanged, as well as fish quality and treasure conditions.

To further tweak the fishing mechanics, you need to edit the Mod configuration file (`svfishmod.json`) located in the mod's directory. This file is created automatically once the game has been launched at least once with the mod installed. Please refer to the **Configuration** section for details on how to further tweak the mod.

# Remarks
- This mod doesnt affect the achievements you can get through Steam, fished counts will still increment as normal when a fish is reeled.
- The mod is designed to be compatible with the **Multiplayer/Coop mode** of the game. **This is still being tested**, so your mileage may vary. Please comment or open new Issues if you encounter something weird during your Online adventures when using this mod.
- Since 0.1.3, support for Server enforced settings in multiplayer sessions has been added. This should work if both the server and clients have at least this mod version installed. **This is currently being tested.**

# Configuration
The `svfishmod.json` configuration file is located on the mod's folder under the StardewValley installation directory, and its automatically created the first time the Game is run with the mod installed.

This file is an JSON-formatted file which can be edited with any normal text editor or with the help of JSON online tools like [ACE's Online JSON Editor](https://jsoneditoronline.org). 
- Navigate to the page, and select `Open from disk` on the right panel (right panel is easier).
- Select the `svfishmod.json` file from the mod's directory. _Remember this file is created the first time you launch the game with the mod installed._
- Tweak settings as desired.
- When done, choose `Save to disk` on the same panel you used for editing the file and save it back to the mod's directory, replacing the old configuration file.
- Ensure the configuration file is named `svfishmod.json` exactly.

**Note:** If you dont have your file explorer set to display file extensions by default, you will only see a single file with name `svfishmod`. That's the configuration file, just follow the previous steps ignoring the `.json` file extension. Take note that the configuration file is named `svfishmod` instead of `svfishingmod` by purpose.

## Configuration sections
### AlwaysCatchDoubleFish
**Possible values:** `true | false`

```
Forces the game to give you two quantities of a hooked fish when reeled out. 

Requires "AutoReelFish: true"
```

### AlwaysCatchTreasure
**Possible values:** `true | false`

```
Forces the game to give you a treasure everytime you reel out a fish. Treasure contents is still determined by the game itself.
```

### AlwaysPerfectCatch
**Possible values:** `true | false`

```
Forces every fish attempt to be perfect if the fish is successfully reeled out. This affects mostly the size and quality of the fish, but have effect on the StardewValley Fair event too.
```

### AutoReelFish
**Possible values:** `true | false`

```
Disables the Fishing minigame, and automatically reels-out the fish out of the water once hooked. Other configuration settings still apply to the fish quality, size and type.
```

### DisableMod
**Possible values:** `true | false`

```
Globally Disables the mod entirely, restoring the vanilla fishing mechanics of the game.
```

### DistanceFromCatchingOverride
**Possible values:** `number from 0.0 through 1.0`

```
Sets thei nitial position of the "Fishing progress bar" when the fishing minigame is started, requiring less time for the fish to be reeled-out once hooked, but still requiring you to complete the Minigame. Take note that the progress can still drop to zero if the fish is not on the green area of the fishing rod. 

Requires "AutoReelFish: false" since this feature has no effect when the minigame is skipped.
```

### OverrideBarHeight
**Possible values:** `number from 0 through 568`

```
Sets the size of the green area of your fishing rod while on the fishing minigame. A value of 0 means that no green area is displayed (impossible to complete) and a value of 568 means the bar completely fill all the available space on the fishing minigame.

Recommended values are: 60-300
Requires "AutoReelFish: false" since this feature has no effect when the minigame is skipped.
```

### RemoveBiteDelay
**Possible values:** `true | false`

```
When enabled, the fish will instantly bite the bait as soon as it touches the water, removing the waiting time. If this value is false, the normal game behaviour will be used and fishes will take a little while for biting.
```

### OverrideFishQuality
**Possible values:** `0 | 1 | 2 | 4`

```
Determines the quality of the hooked fish once reeled-out.
Quality appears to be 0: Normal, 1: Silver, 2: Gold, 4: Purple.

Use this option with caution since the quality values are not completely tested.
```

### OverrideFishType
**Possible values:** `number`

```
Changes the type of fish reeled-out to the specified id after every fishing action regardless of the area where the player is located. This allows you to fish any type of fish you want, anywhere and anytime. See the "console commands" section for more information about how to obtain a fish id from its name. A value of "-1" restores the original game functionality and produces a fish based on the original game conditions.
```

### ReelFishCycling
**Possible values:** `true | false`

```
Cycles through all possible fish (and trash) types everytime you reel-out your fishing rod, ignoring the area, time, season and rod conditions set by the game. This allows you to catch every type of fish in the game, just by throwing your rod continuously. The fish type only changes when the currently selected fish is actually reeled-out from the water.
```

### EnforceMultiplayerSettings
**Possible values:** `true | false`

```
Introduced on v0.1.3. When enabled, the host player on multiplayer sessions will send its mod settings to every other player upon connecting. This allows the host to enforce custom settings that will be applied to all players on the session. Players must have at least v0.1.3 of the mod installed for this feature to work.
```

# Console Commands
This mods includes an small set of commands that can be used from the **SMAPI Console**. Please take note that these commands **CANNOT** be used from within the game's chat window.
Most settings can be changed while the game is running by using the SMAPI console. Just type `help` to get started.


## sv_fishing_debug
**Possible values:** `0 | 1`

```
Enables the output of debug messages to the SMAPI console. Usefull for diagnose purposes only and not intended to be used by actual players.
```

## sv_fishing_enabled
**Possible values:** `0 | 1`

```
Enables or disables the mod globally. See "DisableMod" configuration entry for more details.
```

## sv_fishing_autoreel
**Possible values:** `0 | 1`

```
Enables or disables the Auto reel functionality. See "AutoReelFish" configuration entry for more details.
```

## sv_fishing_bitedelay
**Possible values:** `0 | 1`

```
Enables or disables the bite delay for fishes once the rod has been casted into the water. A value of 1 means that fishes will bite as soon as the hook touches the water, a value of 0 will restore the original game functionality.
```

## sv_fishing_reload

```
Reloads the "svfishmod.cfg" configuration file from disk. Usefull for refreshing the mod configuration without the need of restarting the game.
```

## sv_fishing_fishcycling
**Possible values:** `0 | 1`

```
Enables or disables the Auto Fish Cycling functionality. See "ReelFishCycling" configuration entry for more details.
```

## sv_fishing_search
**Possible values:** `keywords`

```
Searches the list of all possible fishes in the game for those that contains ANY of the specified keywords, and outputs the results to the SMAPI console. This is useful to search the ID of any given fish based on its name.

Searchs are performed based on the Fish's name in both English AND the current StardewValley language, as selected on the Settings window, so feel free to type in the name of the fish as you actually see it on the game.

For example: Typing "sv_fishing_search puff tuna" will return results for the "Pufferfish" and every other fish which includes the word "Tuna" on its name.

Before every fish name, you will see its internal ID which can be used with the "sv_fishing_setfish" command to force the next reeled-out fish to be of the desired type.
```

## sv_fishing_setfish
**Possible values:** `number`

```
Forces the next reeled-out fishes to always give the a fish with the specified internal ID. Use the "sv_fishing_search" command to get the internal ID of a fish from its name.

A value of "-1" restores the original Game functionality and gives the player a fish based on the usual Game conditions.
```

## sv_fishing_status
**Possible values:** `none`

```
Displays the current working status of the mod to the SMAPI console.
```

# Open-Source commitment
This mod is **Open-Source**, that means its code is public, freely-available and covered by an open-source license.

**Dont ever download and use a mod which is NOT OpenSource.**

For more details about why OpenSourcing is important on StardewValley mods see the Open-Source Wiki entry on [StardewValleyWiki](https://stardewvalleywiki.com/Modding:Open_source).

# Licensing
This mod is licensed under the LGPL-2.1 License. For more information see [LGPL License details](https://github.com/KDERazorback/SvFishingMod/blob/master/LICENSE)