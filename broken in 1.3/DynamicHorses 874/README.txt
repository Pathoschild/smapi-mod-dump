Dynamic Horses
A Mod by Bpendragon
Contact on the Stardew Vallew Discord Server

Requirements:
Stardew Valley 1.11
SMAPI 1.7

###Purpose:
To allow players to have differently textured horses per save. Texture is loaded on Horse named, and file loaded

This is a precursor to a much more in depth (and upcoming) DynamicAnimals mod that will do the same thing but for farm animals on an individual basis.

###Current Limitations: 

Multiple saves that have the same named horse will have the same texture

###Contained in this mod

DynamicHorses
	Horses
	DynamicHorses.dll
	DynamicHorses.pdb
	manifest.json
	config.json

###Installation:

1. Drop the DynamicHorses folder that this document is located in into your StarDewValley/Mods folder.
2. Place any horse texture replacements in the Horses Folder. (Can also be done at any time in the future)


###Usage:

config.json is a simple JSON file consisting of a list of Names and associated XNB files. 

Name: The name of the horse you wish to change, case insensitive. "grover" would match "Grover" "grover" "GrOvEr" etc.
XNBName: the name of the xnb (without extension) file you wish to associate with the horse (located in the DynamicHorses/Horses directory) CASE SENSITIVE.

###Current Expected Behavior:

A Horse named "Epona" or "Test" will leave an error in the SMAPI Console because there currently is no epona.xnb or test.xnb files, will leave default texture (these can be had from old versions of the mod).
A Horse named with any other name will load the default texture.