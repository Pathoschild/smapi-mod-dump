"Don't Eat That!" 1.1 by Pyrohead37
	Allows you to specify which items and item categories you do and don't want to be prompted to eat.


!-- REQUIREMENTS --!
	SMAPI 2.0 or newer


!-- INSTRUCTIONS --!
	1)	Install the latest version of SMAPI.
	2)	Download this mod and unzip it into Stardew Valley/Mods.
	3)	Change config.json to your liking (see below). Default settings include animal products, crops, and harmful items that drain energy.
	4)	Run the game using SMAPI.


!-- CONFIGURATION --!
	• Open config.json in a text editor like Notepad++. 
	• Change "dontEatItemNames" to list the specific items YOU DON'T want to eat. This is empty by default.
		Example:
		{
		  "dontEatItemNames": 
		  [
			"Corn",
			"Hops",
			"Egg"
		  ],
		  "dontEatItemCategories": [],
		  "allowedToEatItemNames": []
		}
	• Change "dontEatItemCategories" to list the broad item categories that YOU DON'T want to eat from. This contains the example values below by default.
		Example:
		{
		  "dontEatItemNames": [],
		  "dontEatItemCategories": 
		  [
			"Animal Product",
			"Crop",
			"Harmful"
		  ],
		  "allowedToEatItemNames": []
		}
	• These are all the category values that you can use (data taken from https://stardewvalleywiki.com/Category:Items. "Harmful" is every item with a negative energy effect):
		"All",
		"Animal Product",
		"Artisan Good",
		"Cooking",
		"Crafting",
		"Crop",
		"Fish",
		"Flower",
		"Forage",
		"Harmful"
	• You can also specify items that YOU DO want to eat. This overrides items the previous two lists. That way, you can be generally restrictive but still eat what you want.
	Example:
		{
		  "dontEatItemNames": [],
		  "dontEatItemCategories": 
		  [
			"All"
		  ],
		  "allowedToEatItemNames": 
		  [
			"Spring Onion",
			"Salmonberry",
			"Cheese"
		  ]
		}