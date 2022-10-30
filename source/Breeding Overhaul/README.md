**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/StarAmy/BreedingOverhaul**

----

****This mod is in ALPHA release stage. It has no currently known issues, but is likely to contain bugs, extraneous code, etc, and the documentation is under constant improvement. Please only download if you're willing to be patient as problems crop up, and I encourage early downloaders to let me know of anything you'd like to see adjusted or rebalanced. ****
---------------------------------
Breeding Overhaul is a Stardew Valley mod that aims to revamp the vanilla breeding system when used in conjunction with Animal Husbandry Mod (formerly ButcherMod) and Better Farm Animal Variety. Json Assets (DGA version will be out at some point), Production Framework Mod, and Content Patcher are all also required for full function. It consists of the following sections:

1. Breeding Overhaul, the main project code. It makes the following changes to the base game / Animal Husbandry Mod behavior:

 		Disables normal incubator function, requiring the correct type of 'fertilized' egg instead 
		(the item required for each type of animal can be manually reconfigured in the config file 
		'Incubator Data').
        
		Disables normal pregnancy/insemination function, requiring the correct type of 'DNA' in the AHM 
		insemination syringe instead (the item required for inseminating each type of animal and each 
		parent:offspring possibility can be manually reconfigured in the config file 'Pregnancy Data').

		Disables spontaneous birthing events.
    
2. [BFAV] Vanilla Males adds male variants to every category of vanilla livestock via Better Farm Animal Variety. Males drop manure as a common drop, and a species-specific item as a rare drop. 
        
        	Male Cow = Manure, Cow Horn
        	Male Goat = Manure, Goat Horn    
        	Male Sheep = Manure, Sheep Horn     
        	Male Pig = Manure, Pig Tusk     
        	Male Chicken = Manure, Chicken Feather  
        	Male Duck = Manure, Duck Feather (the same item as in vanilla) 
        	Male Rabbit = Manure, Rabbit Foot (the same item as in vanilla)
        	Male Ostrich = Manure, Ostrich Feather
        	Male Dinosaur = Manure, Dinosaur Scale

3. [AHM] Vanilla Male Rules adds rules for Animal Husbandry Mod behavior, including number and type of meat drops, for the new male variants. 

        Males drop the same quantity/type of meat and have the same treat preferences as their vanilla/female counterparts.

4. [CP] Vanilla Livestock Tweaks makes some changes to the pre-existing livestock to make them feel more like females and fit in better with this mod, including some configurable texture changes, and 'large egg' production for several of the poultry types. The production for the 'female' (vanilla) variants is now as follows:
        
        	White Cow = Milk, Large Milk
        	Brown Cow = Milk, Large Milk
        	Goat = Goat Milk, Large Goat Milk
        	Sheep = Wool
        	Pig  = Truffle
        	White Chicken = White Egg, Large White Egg
        	Brown Chicken = Brown Egg, Large Brown Egg 
        	Blue Chicken = Blue Egg, Large Blue Egg
        	Void Chicken = Void Egg, Large Void Egg
        	Golden Chicken = Golden Egg, Large Golden Egg
        	Duck = Duck Egg, Large Duck Egg
        	Rabbit = Wool
        	Ostrich = Ostrich Egg, Large Ostrich Egg    
        	Dinosaur = Dinosaur Egg, Large Dinosaur Egg
        
5. [JA] Breeding Overhaul Objects adds all the new objects dropped by 'male' and 'female' variants, as well as the DNA and fertilized eggs they can be crafted or processed into.
        
        	Manure, which can be processed into quality fertilizer. 
			It’s not worth very much before, and no one likes getting this out of the blue!
     
        	Cow Horn, which can be processed into Cow DNA. 
        	Goat Horn, which can be processed into Goat DNA.
        	Sheep Horn, which can be processed into Sheep DNA.
			Pig Tusk, which can be processed into Pig DNA.
			Chicken Feather, which can be processed into Chicken DNA. Chicken DNA can 
			be used with ALL chicken variants, even blue, golden, and void. 
			Ostrich Feather, which can be processed into Ostrich DNA. 
			Dinosaur Scale, which can be processed into Dinosaur DNA.
             
        	Blue Egg, a new produce drop from blue chickens.
			Large Blue Egg, a new deluxe produce drop from blue chickens.
			Large Duck Egg, a new deluxe produce drop from ducks.
			Large Golden Egg, a new deluxe produce drop from golden chickens.
			Large Void Egg, a new deluxe produce drop from void chickens.
			Large Ostrich Egg, a new deluxe produce drop from ostriches. 
			Large Dinosaur Egg, a new deluxe produce drop from dinosaurs. 
        
        	Fertilized White Egg, which hatch 1 new baby chicken (brown, white, or male)
			Fertilized Brown Egg, which hatch 1 new baby chicken (brown, white, or male)
			Fertilized Blue Egg, which hatch 1 new baby chicken (blue or male)
			Fertilized Void Egg, which hatch 1 new baby chicken (void or male)
			Fertilized Golden Egg, which hatch 1 new baby chicken (golden or male)

			Fertilized Duck Egg, which hatch 1 new baby duck (normal or male)
			Fertilized Ostrich Egg, which hatch 1 new baby ostrich (normal or male)
			Fertilized Dinosaur Egg, which hatch 1 new dinosaur (normal or male)
        
        	Fertilized Large White Egg, which hatch 1-2 new baby chickens (brown, white, or male)*
			Fertilized Large Brown Egg, which hatch 1-1 new baby chickens (brown, white, or male)*
			Fertilized Large Blue Egg, which hatch 1-2 new baby chickens (blue or male)*
			Fertilized Large Void Egg, which hatch 1-2 new baby chickens (void or male)*
			Fertilized Large Golden Egg, which hatch 1-2 new baby chickens (golden or male)*
			Fertilized Large Duck Egg, which hatch 1-2 new baby ducks (normal or male)*
			Fertilized Large Ostrich Egg, which hatch 1-2 new baby ostrich (normal or male)*
			Fertilized Large Dinosaur Egg, which hatch 1-2 new dinosaurs (normal or male)*
			
        * In development; probably going to be a 1.1.0 feature
	
        	Blue Mayonnaise, a new artisan good made from blue eggs and large blue eggs.
			Golden Mayonnaise, a new artisan good made from golden eggs and large golden eggs.
			Ostrich Mayonnaise, a new artisan good made from ostrich eggs and large ostrich eggs.
        
6. [PFM] Breeding Overhaul Object Rules adds processing rules for all the new objects. 

    The recycling machine now produces:
       
        	Manure = Quality Fertilizer
        	Cow Horn = Cow DNA     
        	Goat Horn = Goat DNA
        	Sheep Horn = Sheep DNA
        	Pig Tusk = Pig DNA
        	Chicken Feather = Chicken DNA
        	Duck Feather = Duck DNA
        	Rabbit Foot = Rabbit DNA
        	Ostrich Feather = Ostrich DNA
        	Dinosaur Scale = Dinosaur DNA
        
     The mayonnaise machine now produces:
        
        	Blue Egg, Large Blue Egg = Blue Mayonnaise
        	Golden Egg, Large Golden Egg = Golden Mayonnaise
			Large Dinosaur Egg = Dinosaur Mayonnaise

So, what does this mean for gameplay? 

Breeding Egg-Laying Animals In-Game (assuming you use default configuration for the incubatordata file):

                1. Obtain DNA for the correct species - you can buy DNA from Marnie, or process 
		your male animal's unique drops into it at the recycling machine. 
                2. Craft a fertile egg using an egg and a DNA, or a large fertile egg using a large egg and a DNA.
                3. Drop the fertile egg in the incubator just like normal, and wait for it to hatch (now it has a 
		chance to be born male, too). Large fertile eggs have a chance to hatch two new babies if you have space for them.

Breeding Live-Birth Animals In-Game (assuming you use default configuration for the pregnancydata file):

                1. Obtain DNA for the correct species - you can buy DNA from Marnie, or process your male animal's 
		unique drops into it at the recycling machine. 
                2. Use the DNA in the Animal Husbandry Mod insemination syringe instead of the vanilla drops (milk, etc). 
                3. Wait for your animal to be born through the normal Animal Husbandry Mod process 
		(now it has a chance to be born male, too). 

A New Quality Fertilizer Source:

                With a handful of males producing manure you now have a steady supply of quality fertilizer. 

You Need a Few Males For Community Center Bundle:

               Duck Feather and Rabbit Foot items are now dropped by male ducks and male rabbits respectively, 
	       instead of their vanilla ('female') counterparts, so you will need to plan ahead and get male 
	       chicks / kits from Marnie to complete the original, unmixed community center bundle.

Adopt & Skin Compatible 

		Paritee’s Better Farm Animal Variety and Animal Husbandry Mod  (required dependencies for this mod) 
		mostly play nice with Adopt & Skin, which will let you further customize and randomize your animals. 
		DO NOT try to enter modded animals into the AHM Animal Shows, though.

Includes a Content Pack Framework

		Feel like using Paritee’s awesome males or another set instead of mine? Have some BFAV animals you want 
		to add breeding functionality for? Just add their data with an extra content pack (example HERE), 
		configure it however you want, and voila! If a modded animal is not included on the config list, 
		breeding behavior reverts back to 'normal' AHM breeding behavior (inseminate animals with their own produce). 
. 
A New Terminal Command

		Type list_animals into your terminal (or your chat bar if you’re using BLANK mod) to get a full list of animal 
		names and their types. This can be handy if you’re using animal skins with few visual differences between genders/types.


