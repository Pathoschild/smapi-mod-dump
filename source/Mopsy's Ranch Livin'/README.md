# Mopsy-Ranch-Livin
**Mopsy's Ranch Livin'** is a [Stardew Valley](http://stardewvalley.net/) mod that lets your change your Farm's suffix to "Ranch", or something else!

## Contents
* [Install](#install)
* [Configure](#configure)
* [Notes](#notes)
* [Changelog](#changelog)

## Install
1. [Install the latest version of SMAPI](https://smapi.io).
2. Install Mopsy's Ranch Livin' from [the Releases section here](https://github.com/mopquill/Mopsys-Ranch-Livin/releases), or the **Files** section of [its Nexus page](https://www.nexusmods.com/stardewvalley/mods/2200).
3. Launch Stardew Valley via SMAPI!

## Configure
1. Load up a save file. If you want your suffix to be "Ranch", you're done!
2. If you want it to be something other than "Ranch", run the command `farm_setsuffix Ranch` in the SMAPI command prompt, where "Ranch" is something else. Don't make it too long or it'll look dumb in some places.

That's it! Running that command saves it to a per-save config file, so whenever you load up a save, it'll remember which suffix you last used for that save! You can find this config file in your Saves folder, and you can also edit it manually!

Suffixes are limited to 10 characters to avoid silly things, but still don't make it too long/wide, or it might look dumb in some places.
 
Romani Ranch, here I come! ;D

## Notes
* Tested with Stardew Valley 1.3.36 and SMAPI 2.11.2.
* Right now, this is only for English. I don't speak any other languages well enough to translate this. If anyone else would like to muck around the various strings/dialogues/events and create translations, let me know. If the keys are not different, I just need the word I need to replace, and with what, and if they keys are also translated, I'd need the files/keys as well.

## Changelog
### 1.0.2
* Updated for the upcoming SMAPI 3.0. (Pathoschild is my personal hero)
* The preferred suffix is now stored in the save file. (Previous players may need to run `farm_setsuffix` again.)

### 1.0.1
* Fixed a bug when entering the command with no arguments, revert to default suffix when exiting save file.

### 1.0.0
* Initial release; compatible with Stardew Valley 1.3.11-beta and SMAPI 2.6-beta.14.
