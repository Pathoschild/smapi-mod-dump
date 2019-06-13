To use custom sprites:

Animals must be one of the following types:
- BlueChicken 		(BabyBlueChicken)
- BrownChicken 		(BabyBrownChicken)
- WhiteChicken		(BabyWhiteChicken)
- VoidChicken		(BabyVoidChicken)
- Duck			(BabyDuck)
- Rabbit		(BabyRabbit)
- Dinosaur
- BrownCow		(BabyBrownCow)
- WhiteCow		(BabyWhiteCow)
- Goat			(BabyGoat)
- Sheep			(BabySheep)		(ShearedSheep)
- Pig			(BabyPig)

Pets must be one of the following types:
- Cat
- Dog

Horse must be the following type:
- Horse


** Sprite sheet files must be named in this fashion for Animal Skinner to parse them:

Examples: Dinosaur_1, BabyBlueChicken_1, BlueChicken_1, BabySheep_3, Sheep_3, ShearedSheep_3, etc.

~ Type of animal written exactly as shown in the above type list, no spaces
~ The number following the file name is a unique identifying number
~ Animals that look different as babies or when sheared/harvested from must have a separate sprite sheet, named with the same identifying number, and have this name be proceded by "Baby" or "Sheared" (eg BabySheep_1, Sheep_1, and ShearedSheep_1 all make a single sheep skin set)
~ ANIMAL SKINNER MAY BREAK AND ACT ODDLY when setting skins if an animal type's sprite sheets are not numbered starting at 1 or not numbered continuously (eg, do not have only two skins and have their IDs as 1 and 7. Just do 1 and 2.)
~ All sprite files must be of type .PNG or .XNB
~ All sprite files must be placed in Animal Skinner/assets/skins
~ BFAV-added animals must be named in the same format, with their name being the same as it looks in the BFAV config file.