**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/taggartaa/AutoAnimalDoors**

----

# AutoAnimalDoors
This is a mod created for StardewValley which will automate the animal door opening/closing process for all your barns and coops.

I appreciate all the support this mod has received! If you'd like to further support my work please use this link to [donate](http://paypal.me/JustKiddingStudios)

~Thanks :) Aaron

## Android Users:

I don't have any way of testing android, however thanks to Puadii09 (from Nexus Mods), this should work:

Mod Version: 2.3.0
Smapi Version: 3.7 (android)
Stardew Valley Version: 1.4.5

Note: I have not been actively developing for android so it will be missing the newer features.

## Features
### Opening animal doors automatically
Your animal doors will open automatically at the time you specified in the config file no matter where your character is in the game world (you will hear the door opening sound). 

If that time never occurs during the day (for instance, you specified 730AM but you used some other mod to set the time to 900AM) the doors will still open unless it is past your specified doors close time. Note: the doors will only ever auto-open once per day.

Doors will not open in the Winter or on Rainy/Thunder Stormy days by defualt, this can be changed in the config file.

You can disable the animal doors opening automatically with a setting in the config file if you would like your doors to auto-close, but not auto-open.

### Closing animal doors automatically
Your animal doors will close automatically at the time you specified in the config file if all your animals are inside. If they are not yet inside, the doors will wait for all the animals to enter their homes before closing any doors. 

If you go to bed before your animal door close time, your animal doors will be closed, and all your animals will be warped back to their homes to keep them safe from attacks ;)

Note: The current implementation closes the animal doors and warps the animals back as soon as you get into bed (even if you select "No" from the "Go to sleep now?" dialog). It is difficult (impossible?) to warp your animals back to safety on time once "Yes" is pressed.

### Other Mods Support
I added the option `UnrecognizedAnimalBuildingsEnabled` to allow you to enable auto opening of animal buildings that are added by other mods (example: [Aviary Mod](https://www.nexusmods.com/stardewvalley/mods/13492)). Keep in mind that I can't test every mod out there, and I can't control other mods door animations or sounds, so they will likely not follow your `DoorSoundSetting`. For this reason, I have disabled the option by default.

### Upgrade Level Requirement
You can set the required upgrade level for barns or coops before the auto open/close feature is enabled via the CoopRequiredUpgradeLevel and BarnRequiredUpgradeLevel respectively. The number corresponds to the minimim upgrade level required for the feature to be enabled. If you want to only have the doors automatically open/close on the last upgrade level (to correspond with the auto feeder system) you can set these values to 3. 

Consequently, you can disable this mod for barns or coops by setting the value to anything 4 or over, since you can never have a coop/barn with that high of an upgrade level.

## Config File

Hey, you no longer have to use a config file, I integrated with [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098). Download and install that mod and configure all these settings in the game! :)

Oh, you'd rather use the config file? That is okay too. The config file is where all the options for this mod can be set. It will be automatically created the first time the game is launched with this mod installed.

You can find it in the mods install directory.

### Values

| Name                                  | Type      | Default | Description                                                                      |
|:--------------------------------------|:--------- |:------- |:-------------------------------------------------------------------------------- |
| **AnimalDoorOpenTime**                | *integer* | 730     | The time animal doors are scheduled to open (730 = 7:30 am, 1310 = 1:10 pm)      |
| **AnimalDoorCloseTime**               | *integer* | 1800    | The time animal doors are scheduled to close (730 = 7:30 am, 1310 = 1:10 pm)     |
| **CoopRequiredUpgradeLevel**          | *integer* | 1       | The coop upgrade level required for auto open/close (1=base, 2=big, 3=deluxe)    |
| **BarnRequiredUpgradeLevel**          | *integer* | 1       | The barn upgrade level required for auto open/close (1=base, 2=big, 3=deluxe)    |
| **UnrecognizedAnimalBuildingsEnabled**| *boolean* | false   | true if animal bulidings from other mods should auto open/close, false if not    |
| **AutoOpenEnabled**                   | *boolean* | true    | true if doors should automatically open, false if not                            |
| **DoorSoundSetting**                  | *string*  | "ONLY_ON_FARM" | Sets when you hear the door sound openning and closing. Possible values: ("ONLY_ON_FARM", "ALWAYS_ON", "ALWAYS_OFF") |
| **OpenDoorsWhenRaining**              | *boolean* | false   | true if doors should open even when raining/lightning, false if not              |
| **OpenDoorsDuringWinter**             | *boolean* | false   | true if doors should open even during winter, false if not                       |

  
### Example config.json

```json
{

  "AnimalDoorOpenTime": 730,
  "AnimalDoorCloseTime": 1800,
  "CoopRequiredUpgradeLevel": 1,
  "BarnRequiredUpgradeLevel": 1,
  "UnrecognizedAnimalBuildingsEnabled": false,
  "AutoOpenEnabled": true,
  "DoorSoundSetting": "ONLY_ON_FARM",
  "OpenDoorsWhenRaining": false,
  "OpenDoorsDuringWinter": false
}
```

## Building the Code
This mod ustilizes the SMAPI API. So if you get any issues regarding not being able to resolve StardewModdingAPI. You likely need to do some of the setup described here: http://canimod.com/for-devs/creating-a-smapi-mod
