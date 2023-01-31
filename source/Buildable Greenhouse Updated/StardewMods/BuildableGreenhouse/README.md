**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Yariazen/YariazenMods**

----

# Description
Adds the greenhouse to the carpenter's shop.
<br>
# Update
Updating from a version before 1.0.0 to a version after 1.0.0 causes loss of progress. It's best to wait until starting a new save before updating.
<br>
# Config
StartWithGreenhouse (default true):
- true: Greenhouse is buildable from the start
- false: Greenhouse is buildable once it is unlocked

BuildCost (default 100000):
- This is the cost to construct a greenhouse

BuildDays (default 3):
- This is the number of days to construct a greenhouse

BuildDifficulty (default 2):
- 0: Greenhouse requires no materials
- 1:
  - Stone 500
  - Wood 100
  - Ironbars 5
- 2:
  - Stone 500
  - Hardwood 100
  - ﻿Iridiumbars 5
<br>
# Use a Different Indoor Map
This guide will use Ellie's Ideal Greenhouse as the example
Find the content.json of the patch the content of which which should look something like the following

	{
		"Format": "1.19.0",
		"ConfigSchema": {
			"GreenhouseType": {"AllowValues": "Modest, Expanded, Spacious",
			"Default": "Modest"}
		},
	
		"Changes": [
    		{
				"Action": "Load",
				"Target": "Maps/Greenhouse",
				"FromFile": "assets/{{GreenhouseType}} Greenhouse.tmx"
			},
		]
	}
<br>
Modify the value of "Target" to "Maps/GreenhouseMap" <br>
The final result should look something like the following

	{
		"Format": "1.19.0",
		"ConfigSchema": {
			"GreenhouseType": {"AllowValues": "Modest, Expanded, Spacious",
			"Default": "Modest"}
		},
	
		"Changes": [
    		{
				"Action": "Load",
				"Target": "Maps/GreenhouseMap",
				"FromFile": "assets/{{GreenhouseType}} Greenhouse.tmx"
			},
		]
	}
<br>
# Roadmap
- Better Junimos Compatibility
- Greenhouse Sprinklers Compatibility (may implement similar features in this mod rather than compatibility)
- Greenhouse Upgrades Compatibility (may implement similar features in this mod rather than compatibility)
<br>
# Link to Source: [Github](https://github.com/Yariazen/StardewValleyMods/tree/main/BuildableGreenhouse)