**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/dmcrider/LastDayToPlant**

----

# Release History
v2.0.2 - Fixed crop names not being translated correctly.

v2.0.1 - Added support for other mods.

v1.2.0 - Automatically check for Agriculturist skill (determined by the main player in multiplayer). Added flag for base crops in preparation for mod compatibility (coming soon).

v1.1.0 - Added configuration settings. Will now show messages when it is the last day to use each type of Speed Gro.

v1.0.0 - Initial Release.

# Install
- Download from NexusMods: https://www.nexusmods.com/stardewvalley/mods/7917 (recommended) or from the latest release: https://github.com/dmcrider/LastDayToPlant/releases/latest
- Requires SMAPI
- Extract the ZIP to `\Stardew Valley\Mods\`
- Start the game to create the `config.json` file

# Configuration
The configuration options work like this: In addition to a notification on Day 16 of Spring for Cauliflower, if you enable all options, you will also get a notification on Days 18, 19, and 20 for Speed Gro, Deluxe Speed Gro, and Hyper Speed Gro, respectively, for Cauliflower. As another example, you could also set `ShowBaseCrops` to `false` and set `ShowSpeedGro` to `true` to only recieve notifications on the last day that Speed Gro can be used.

The `config.json` file looks like this:

    {
      "IncludeBaseGameCrops": true,
      "ShowBaseCrops": true,
      "ShowSpeedGro": false,
      "ShowDeluxeSpeedGro": false,
      "ShowHyperSpeedGro": false,
      "PPJAFruitsAndVeggiesPath": "",
      "PPJAFantasyCropsPath": "",
      "PPJAAncientCropsPath": "",
      "PPJACannabisKitPath": "",
      "BonstersFruitAndVeggiesPath": "",
    }

Any option that ends with "Path" expects a full path to the root folder of that mod. For example, `"PPJAFruitsAndVeggiesPath": "D:/Steam/steamapps/common/Stardew Valley/Mods/Fruits and Veggies"`. If you have any issues getting the path right, let me know via GitHub: https://github.com/dmcrider/LastDayToPlant/issues.

# Running into issues?
If something is going wrong, please let me know by submitting an issue on GitHub: https://github.com/dmcrider/LastDayToPlant/issues/new, or email me directly at mods@dayloncrider.com. I try my best to test multiple scenarios, but I can't test everything.

# Supported Languages
- English
- Spanish
- Brazilian Portuguese
- Russian

I am aware that some translations are not getting the translated crop name. I am looking into this and hope to have a fix soon.

# Special Thanks
@Duckexza (https://github.com/Duckexza) - Brazilian translation
@4x4cheesecake (https://github.com/4x4cheesecake) - Fix for this issue: https://github.com/dmcrider/LastDayToPlant/issues/16

[@4x4cheesecake](https://github.com/4x4cheesecake) - Fix for [#16](https://github.com/dmcrider/LastDayToPlant/issues/16)

# Like what I do?
If you enjoy the content I create, a donation is greatly appreciated (though NEVER required!). You can make a donation [on my website](https://www.dayloncrider.com/donations).
