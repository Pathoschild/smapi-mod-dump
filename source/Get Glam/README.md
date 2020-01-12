# GetGlam
Get Glam is a Stardew Valley mod that allows different player customizations to be added to the game, based off of GetDressed/Kisekae.

## Contents
* [Install](#install)
* [Config](#config)
* [Features](#features)
	* [Overview](#overview)
	* [Accessories](#accessories)
	* [Bases](#bases)
	* [Dresser](#dresser)
	* [Faces and Noses](#faceandnose)
	* [Hairstyles](#hairstyles)
	* [Shoes](#shoes)
	* [Skin Colors](#skincolor)
* [Todo](#todo)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. Install the mod from [Nexus](https://www.nexusmods.com/stardewvalley/mods/5044).
3. Unzip any Get Glam content packs into the `Mods` folder to install them.
4. Run the game using SMAPI.

## Config
Get Glam has a config that allows different options to be changed, the options include:

option                    | purpose
--------------------------|--------
`OpenGlamMenuKey`         | Default: `C`. Description: The key used to open the custimization menu.
`NewHatsIgnoreHair`       | Default: `false`. Decription: Any new hats acquired automatically ignore the hairstyle.
`DresserTableLocationX`   | Default: `0`. Description: Change the X coordinate of the dresser in the Farmhouse.
`DresserTableLocationY`   | Default: `0`. Description: Chnage the Y coordinate of the dresser in the Farmhouse.
`PatchDresserInFarmHouse` | Default: `true`. Description: Places the dresser in the FarmHouse.
`StoveInCorner`           | Default: `false`. Description: Whether the player has a mod that has a stove in the corner.

## Features
### Overview
Get Glam is a rewritten GetDressed/Kisekae for Stardew Valley 1.4.
The base mod includes the dresser and all the face and nose options from Get Dressed.
This mod differs from previous iterations since it adds content pack support.

There are seven folders that you are likely to see when downloading a Get Glam content pack:
* Accessories
* Base
* Dresser
* FaceAndNose
* Hairstyles
* Shoes
* SkinColors

Each of these subfolders will contain `.png` image(s) and a `.json` in the `Hairstyles`, `Accessories` and `FaceAndNose` folder.

### Accessories
The player can wear accessories including facial hair and earnings, all contained within one image.

Accessory Size: 16 x 32

SpriteSheet Size: 128 x Number of accesories added

To add accessories to a content pack you need to:
* Create a folder in the pack named `Accessories`.
* Add in the custom image named `accessories.png` to the newly created `Accessories` folder. 
* Add in a `accessories.json` with one field.

field			      | purpose
----------------------|--------
`NumberOfAccessories` | The number of accessories added by the content pack.

All accessories need to be added to the same `accessories.png`, Get Glam only supports one `accessories.png` per content pack.

Note: Get Glam can add up to 1024 accessories including default accessories from different Get Glam content packs.

### Bases
The player can swap between farmer bases that content packs provide.

Size: 288 x 672

To add a base to a content pack you need to:
* Create a folder in the pack named `Base`.
* Add in the custom image `farmer_base.png` for the male farmers base to the `Base` folder.
* Add in the custom image `farmer_base_bald.png` for the male farmers bald base to the `Base` folder.
* Add in the custom image `farmer_girl_base.png` for the female farmers base to the `Base` folder.
* Add in the custom image `farmer_girl_base_bald.png` for the female farmers bald base to the `Base` folder.

The bald option NEEDS to be added to the `Base` folder in order for the content pack to work correctly with the mod.
Currently, Get Glam only supports one `farmer_base.png`, `farmer_girl_base.png`, `farmer_base_bald.png` and `farmer_girl_base_bald.png` in a content pack.

### Dresser
The dresser comes as the default from Get Dressed/Kisekae and can be changed with content packs.

Dresser Size: 16 x 32

SpriteSheet Size: 16 x Number of dressers added

To add dressers to a content pack you need to:
* Create a folder in the pack named `Dresser`.
* Add in the custom dresser image `dresser.png` to the newly created `Dresser` folder.

All dressers need to be added to the same `dressers.png`, Get Glam only supports one `dressers.png` per content pack.

Note: Get Glam can add up to 128 dressers including the default dresser from different Get Glam content packs.

### Faces and Noses
Get Glam adds the option for the farmer to be able to swap between faces and noses for a particular base.

Size: 96 x 672

To add faces and noses for your base, you need to:
* Create a folder in the pack named `FaceAndNose`.
* Add in the custom face and nose images `"gender"_face"FaceNumber"_nose"NoseNumber".png` to the `FaceAndNose` folder.
* The naming convention for the Face and Nose number need to be sequential. Ex: First face and nose needs to be named `male_face0_nose0.png`, second `male_face0_nose1.png`...and so on.
* Add in a `count.json` to the `FaceAndNose` folder.

field			      | purpose
----------------------|--------
`NumberOfMaleFaces`   | The number of male faces added by the content pack.
`NumerOfMaleNoses`    | The number of male noses added by the content pack.
`NumerOfFemaleFaces`  | The number of female faces added by the content pack.
`NumerOfFemaleNoses`  | The number of female noses added by the content pack.

### Hairstyles
The player can change their hairstyle, all contained within one image.

Hairstyle Size: 16 x 96

Spritesheet Size: 128 x Number of hairstyles added

To add hair to a content pack you need to:
* Create a folder in the pack named `Hairstyles`.
* Add in the custom image named `hairstyles.png` to the newly created `Hairstyles` folder.
* Add in a `hairstyles.json` with one field.

field			      | purpose
----------------------|--------
`NumberOfHairstyles`  | The number of hairstyles added by the content pack.

All hairstyles need to be added to the same `hairstyles.png`, Get Glam only supports one `hairstyles.png` per content pack.

Note: Get Glam can add up to 336 hairstyles including default hairstyles from different Get Glam content packs. SpaceCore can make it unlimited.

### Shoes
The player can change their shoes for any base provided with content packs.

Size: 92 x 672

To add shoes to a content pack you need to:
* Create a folder in the pack named `Shoes`.
* Add in the custom images named `"gender"_shoes{ShoeNumber}.png` to the newly created `Shoes` folder.

### Skin Color
The player can change their skin color to different colors provided by content packs.

Skin Color Size: 3 x 1

Spritesheet Size: 3 x Number of skin colors

To add skin colors to a content pack you need to:
* Create a folder in the pack named `SkinColor`.
* Add in the custom image name `skinColors.png` to the newly created `SkinColor` folder.

All skin colors need to be added to the same `skinColors.png`. Get Glam only supports one `skinColors.png` per content pack.

Note: Get Glam can add up to 4096 skin colors including default skin colors from different Get Glam content packs.

## TODO
* Make the dresser JA compatible for easy moving.
* Fix bugs.