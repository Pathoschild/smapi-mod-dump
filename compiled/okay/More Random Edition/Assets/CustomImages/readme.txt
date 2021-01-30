Custom Images Reference
-----------------------
This document is for anyone looking to add to/modify the randomizer's custom images. If you're not looking at doing so, just note that you can disable the randomized images by turning off the appropriate "UseCustomImages" settings in the config.json file located at the mod's root folder (generated the first time you launch Stardew Valley with the mod installed).

Feel free to post to the mod page with any additional questions:
https://www.nexusmods.com/stardewvalley/mods/5311/


In general
----------
The image randomizer works by randomly choosing images in specific directories, and piecing together a spritesheet to use in place of the existing one. Here are some general rules for each custom image:

- Try to avoid using indexed color palettes, as they cause crashes on Macs
- Each image should be a png, and should end in ".png", else it won't be picked up as a possible choice
- DO NOT delete any image in the directory that is a spritesheet - this is likely the base file that's used; deleting these will cause errors
- Most issues will be detected by the randomizer when first loading your farm, so it's a good idea to scroll through the console output to locate any errors if you make any changes
- Most errors would result in default graphics being used instead of the custom ones
- The randomizer will create a "randomizedImage.png" in these directories; if you're not running the game, it's safe to delete these, but they're cleared when the game is started anyway, so it doesn't do much

The following sections are about each folder located in the CustomImages directory, and specifics about them.


Bundles
-------
These images are all linked with a specific randomized bundle name. If you with to use your own, you must replace the existing image with the one you wish to use instead. Any extra files placed in here will never be used.

- Any replacement must be 32 x 32px
- DO NOT delete the JunimoNote images, as these are the base images used for the UI
 - If you plan on replacing it, note that the bundle image section will be overwritten by the randomizer, and that you only really need to replace the one that matches the language you're using (the English version is the one with no suffix)


CropGrowth
----------
These images are the sprites used when crops are growing on your farm. These images consist of eight 16 x 32px sprites (meaning the entire image must be 128 x 32px). The order of the sprites depends on the folder. DO NOT delete the crops.png image here, as it is the base image used.

You MUST have the appropriate matching images in the SpringObjects directory, or you will see errors. Specificaly, the Seeds, Crops, and Flowers subdirectories. See the SpringObjects section for more details.

Try not to have the following in your image name unless otherwise noted to prevent unexpected behaviors:
- -4.png
- -5.png
- -NoHue.png

Here's an overview of all the folders and what the sprites mean:


Flowers:
Two versions of each flower must be placed here. One is for flowers with hues, and one is for flowers without hues. The image to use for the flower without hues should end with "-NoHue.png". The following is an example of two file names that will work:
- MyFlower.png
- MyFlower-NoHue.png

Here's an outline of how the rest of the sprites work:
- 1, 2: Two variations of when the seed is first planted
- 3, 4, 5: Three of the growth stages of the flower
- 6: The stem of the final growth stage of the flower
- 7: (Only for flowers with hues, unused otherwise): The top of the flower - a hue is applied to this flower, depending on the flower being replaced


NormalCrops:
These can have either 4 or 5 growth stages. There MUST be two images per crop in this directory, one ending in "-4.png", and one in "-5.png". The appropriate set of sprites will be used depending on the number of growth stages the crop has.

- 1, 2: Two variations of when the seed is first planted
- The rest, excluding the end are the intermediate growth stages of the crop
- The last is the stage when the crop is ready for harvest (would be the 6th or 7th sprite)


RegrowingCrops and TrellisCrops:
- 1, 2: Two variations of when the seed is first planted
- 3, 4, 5, 6: The intermediate growth stages of the crop
- 7: The crop when ready for harvest
- 8: The crop after harvest, before it has regrown


SpringObjects
-------------
This directory consists of replacements to be made in the springobjects.png spritesheet. This is the name of the spritesheet for most of the items in Stardew Valley.

DO NOT delete the springobjects.png image, as it's used as the base image for these replacements.

Fruit tree sapling sprites are replaced when the appropriate setting is on. The image to be used for that is directly in the SpringObjects directory, and is called "fruitTreeSprites.png". This consists of six 16 x 16px fruit tree sapling sprites in the following order:
- cherry, apricot, orange, peach, pomegranate, apple

The following are details about each folder:


Boots and Fish:
- Each image must be 16 x 16px


Crops: 
- Each image must be 16 x 16px
- For each crop growth in the following folders, there has to be one crop image with the same name:
 - CustomImages/CropGrowth/NormalCrops (without the -4 or -5 in the name)
 - CustomImages/CropGrowth/RegrowingCrops
 - CustomImages/CropGrowth/TrellisCrops


Flowers:
- Each image must be 32 x 16px - the first 16 x 16px image being the flower that's used if no custom hue is applied. The second image is overlayed onto the first image if the flower is using hues.
- One image with a matching name must exist for the corresponding crop growth image under CustomImages/CropGrowth/Flowers


Seeds:
- Each image must be 16 x 16px
- For each crop growth in the following folders, there has to be one crop image with the same name:
 - CustomImages/CropGrowth/NormalCrops (without the -4 or -5 in the name)
 - CustomImages/CropGrowth/RegrowingCrops
 - CustomImages/CropGrowth/TrellisCrops
 - CustomImages/CropGrowth/Flowers


Weapons:
The weapons are divided among four different folders, to represent the different weapon types. Each directory has the following rules, with the exception of the Slingshots directory, which is currently unused:

- Each image must be 16 x 16px
- Having less than 60 images could result in default images being used

DO NOT delete the weapons.png image in the Weapons directory, as it's used as the base image.