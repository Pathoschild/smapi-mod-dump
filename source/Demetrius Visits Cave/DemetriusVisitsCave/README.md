Demetrius puts in so much effort turning that empty farm cave into a controlled environment for his experiment and then… never comes back. Dima, c'mon man, get down here and collect some data!

This mod brings him over once a week to check on his research project. That's once a week when you don't have to run all the way up the mountain if you're trying to increase friendsh-- uh, once a week encouraging your local scientist to gather important scientific data.

#### Installation  

1. Install [SMAPI](http://smapi.io)
2. Download [the zip file of this mod from Nexus](https://www.nexusmods.com/stardewvalley/mods/5477) and unzip it into your mods folder
3. Run the game using SMAPI

#### Configuration/Options

Run the game with the mod installed at least once to generate config.json

* `day` (default `"Tuesday"`) -- day of the week for Demetrius to visit the cave. This will override his normal schedule but not specific dates (ie if it's set to Tuesday he'll skip going to the cave in favor of the Night Market on Winter 16 and if it's set to Thursday he'll still go to his doctor's appointment on Summer 25). It's basically all-or-nothing, he either follows his normal schedule or he's in your cave *all. day. long.* I recommend Tuesday or Thursday to fit in with the days Maru works at the clinic but if you're intent on maximizing cave visits they're also the days when he's going to miss at least one.
* `avoidRain` (default `true`) -- whether Demetrius stays home and sticks to his normal rainy-day schedule if it's raining
* `modDialog` (default `true`) -- whether to use a lil custom dialog if you talk to Demetrius while he's in the cave. Currently available in English, Chinese, and Spanish.
* `locx` (default `3`) -- Demetrius' location in the cave (x-axis)
* `locy` (default `7`) -- Demetrius' location in the cave (y-axis)
* `facing` (default `"right"`) -- direction Demetrius is facing 

#### Compatibility with other mods

Default settings have been tested with SVE and the IF2R map and should work with other mods which affect the farm cave, though a drastic enough change in the map or enough extra items in the cave might require tweaking the location to keep him from conflicting with anything else. The mod dialog is location-specific so unless you have another mod sending Demetrius to the cave there won't be any conflicts. Scheduling might be a bit quirkier but if you know when another mod is changing his schedule you can work around it by setting the cave day to something else.

#### Changelog
* 1.0.2 -- Direction added to config
* 1.0.1 -- Chinese and Spanish translations added

#### Credits

* Chinese translation by yyj1996
* Spanish translation by [sandrasorey](url=https://www.nexusmods.com/stardewvalley/users/44879567)
