#### NEWS

* Updated for SV multiplayer beta! There are some bugs related to letters being sent repeatably but discounts should work properly.
* Players will no longer receive letters if a discount is 0% or less (price increase) because of custom config.

#### WHAT THIS DOES

Adds discounts to all shops if you have a good relationship with the owner. Discounts come in three levels, 10% at three hearts, 25% at seven hearts and 50% at ten hearts. Should work for all regular shops, including Robin's buildings (except for the house upgrade, planned for a later release). Robin will also offer resource discounts for buildings.

Details on whose shops are currently affected:

* Robin (buildings, ie. Coops, Silos, Barns, and so on)
* Robin (regular shop, ie. furniture etc.)
* Marnie (animals and supplies)
* Pierre
* Harvey
* Gus
* Clint (regular shop and tool upgrades)
* Sandy
* Willy
* Krobus
* Dwarf
* Wizard

#### COMPATIBILITY

Should be compatible with anything that adds new shop content no problem, since this mod only updates existing prices with a discount percentage. Definitely works with the Longevity mod.

#### CONFIGURATION

You can change the discounts and heart levels at which they are triggered in the config.json file. They correspond to each other, ie. heartLevel1 = 3, disc1 = 10 means the player receives a 10% discount from 3 hearts upwards. Change to what you feel is fair.

They must be in ascending order, ie. heartLevel1 and disc1 have to be the lowest values, 3 the highest. You can also use this configuration to increase prices beyond their base value, ie. heartLevel1 = 0, disc1 = 125 means the player has to pay 125% of the original price if he has 0 hearts with the store owners.

You can add the names of custom NPCs with shops to the config file. Just replace the place holder strings from the customNPCs list with the names of your shop owners or add new ones (format: "NPC1", "NPC2", "NPC3").
Important: The shop owner will of course have to be an NPC with which you can earn hearts.

#### THANKS TO

* An, who has been patient with me while I dug myself into coding with SMAPI and being frustrated at how things work during the two days I created the initial version.
* Pathoschild, who put me on the right track for accessing the shops and of course for his work on SMAPI in the first place.

#### TO DO
* Add custom dialogue so the player is informed in-game once he triggers discount levels
* Add discount for house upgrades at Robin's
* Possibly add various other rewards for better relationships with NPCs, ie. make Clint upgrade your tools faster etc.
