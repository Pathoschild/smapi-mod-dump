"Did You Water Your Crops?" by Nishtra

Changes the look of crops depending on whether they were watered.




Installation
1. Install the latest version of SMAPI
2. Unpack the downloaded zip file and place it's contents into the Mods folder
3. Tweak the configs to your liking. If there is no config file or you want to reset all settings to default, delete existing config file and the next time you load the game a new config will be created.
4. (Optional) If you're using mods that replace \Content\TileSheets\crops.xnb file, the mod will generate a new dry crops tilesheet, but the algorithm used for this is very basic, so if you want to create crops' sprites yourself, read further.
  4.1 Run the game and enter "export_crops" in the SMAPI console. This will give you the original crops tilesheet to work with.
  4.2 Make a copy of crops tilesheet. This will be a base for creating dry version. to be "dry version" tilesheet and edit it as you like. You can also edit normal tilesheet if you want. Giant crops are better left with their original look
  4.3 Edit dry crops tilesheet to as you like (like lowering it's brightness/saturation or completely redrawing the crops' sprites). You can also edit normal tilesheet if you want (giant crops are better left with their original look though).
  4.4 Put both files into \Did You Water Your Crops\Data folder (if they aren't there yet) and add their filenames to config settings "cropsTextureFilename" and "dryCropsTextureFilename".
5. Play the game.  

!Important: due to how the mod and the game function the height of tilesheets shouldn't be more than 2048px. Which means that maximum 86 new crops can be added.



Config settings:

  > "cropsTextureFilename" - the name of the file (including the extension) containing normal (watered) crops tilesheet. The file must be placed in "Did You Water Your Crops\Data" folder and have .png extension.  By default "vanilla crops.png". This setting is used mainly if you want to alter the normal appearance of the crops. If empty, the mod will not affect the appearence of watered crops.

  > "dryCropsTextureFilename" - the name of the file (including the extension) containing dry (NOT watered) crops tilesheet. The file must be placed in "Did You Water Your Crops\Data" folder and have .png extension.  By default "vanilla crops (dry).png". If empty the mod will try to automatically generate a desaturated crops' textures. If the referenced image file contains fewer crops than the game, missing crops' sprites will be automatically generated.
*Note: if you're using mods tha add new crops to the game, leave this field empty or use "vanilla crops (dry).png".

  > "DebugMessages": default FALSE. Set to TRUE if you want messages about crops' sprites changing be logged to console.


Console commands:

"export_combined_tilesheet" - save the combined normal+dry crops tilesheet to the mod's Data folder in .png format. There should be no reason for you to use this command.
"export_crops" - save the unaltered crops tilesheet to the mod's Data folder in .png format.
"export_drycrops" - save the dryCrops tilesheet to the mod's Data folder in .png format.



Requires SMAPI!