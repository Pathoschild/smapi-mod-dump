/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

(If viewing from the Nexus, click 'View as plain text' on the top-right of the Docs tab to fix formatting)

======================================================================================================================================
========	Overview	======================================================================================================
======================================================================================================================================
This mod adds several new "Bag" items that are capable of holding other items inside of them while only taking up a single inventory slot.

-There are 4 different kinds of bags with drastically different uses:
	-Standard Bags
		-Capable of storing specific pre-defined items
		-Comes in several different subtypes
			-Gem Bags
			-Smithing Bags
			-Mineral Bags
			-Resource Bags
			-Construction Bags
			-Tree Bags
			-Animal Products Bags
			-Recycling Bags
			-Loot Bags
			-Foraging Bags
			-Artifact Bags
			-Seed Bags
			-Fish Bags
				-There are also less expensive sub-types of Fish Bags
					-Ocean Fish Bags
					-River Fish Bags
					-Lake Fish Bags
					-Miscellaneous Fish Bags
			-Farmer's Bags
			-Food Bags
			-Crop Bags
	-Bundle Bag
		-Capable of storing items that are required by incomplete community center bundles
	-Rucksack
		-Capable of storing almost any non-modded stackable items that you want, like having an inventory inside of your inventory
	-Omni Bag
		-Capable of storing other bags inside of it
-Most bags come in 5 different sizes:
	-Small
	-Medium
	-Large
	-Giant
	-Massive
-Larger sizes can store more items, and higher quantities of each item.


======================================================================================================================================
========	Compatibility	     =================================================================================================
======================================================================================================================================
-Requires SMAPI
	-For PC users: SMAPI 3.0.1+
	-For Android users: SMAPI 3.2.0.3+
-Compatible with Windows, Linux, and Android
-Multiplayer support was added in v1.3.1 and has not yet been thoroughly tested.
-Known mod compatibility issues:
	-SaveAnywhere
		-Requires ItemBags 1.2.3+ or else you'll crash when saving
		-You may need to exit to title and re-load your game after saving to get your bags back


======================================================================================================================================
========	Bag Interface	     =================================================================================================
======================================================================================================================================
-Opening/Closing bags:
	-Left-click or right-click a bag on your toolbar
	-Right-click or double-click a bag that is in your inventory
	-Right-click or middle-click a bag that is inside of a chest
	-If you already have a bag opened, you can:
		-Right-click a different bag to instantly switch to it
		-Right-click the currently-opened bag to close it
-Transferring items:
	-Left-click: Transfer entire stack (or as many as possible)
	-Right-click: Transfers 1
		-Hold right-click to repeat transfer action ~12 times/second
	-Modifier keys:
		-Shift: Transfers several (Usually 5, but increases to 25 if quantity is > 999)
		-Control: Transfers half
-Bags inside chests:
	-If you open a bag that is inside of a chest, you'll be transferring items between the chest and the bag
	-If you open a bag that is inside of your inventory, you'll be transferring items between your inventory and the bag


======================================================================================================================================
========	Standard Bags	     =================================================================================================
======================================================================================================================================
-All standard bags are available in all 5 sizes.
-Sold by several different shops. Note that some of the larger sized bags may only be available at a certain shop.
	-Pierre
		-Resource Bags (Up to Medium)
		-Tree Bags (Up to Medium)
		-Foraging Bags (Up to Large)
		-Seed Bags (Up to Large)
		-Crop Bags (Up to Medium)
	-Clint
		-Gem Bags (Up to Giant)
		-Smithing Bags (All sizes)
		-Mineral Bags (All sizes)
		-Mining Bags (Up to Medium)
	-Robin
		-Resource Bags (All sizes)
		-Construction Bags (All sizes)
		-Tree Bags (All sizes)
	-Willy
		-Recycling Bags (Up to Giant)
		-Fish Bags (All sizes)
		-Ocean Fish Bags (All sizes)
		-River Fish Bags (All sizes)
		-Lake Fish Bags (All sizes)
		-Misscellaneous Fish Bags (All sizes)
	-Marnie
		-Animal Products Bags (All sizes)
		-Farmer's Bags (All sizes)
	-Krobus
		-Recycling Bags (All sizes)
		-Artifact Bags (All sizes)
	-Dwarf
		-Gem Bags (All sizes)
		-Mineral Bags (All sizes)
		-Mining Bags (All sizes)
		-Loot Bags (Up to Medium)
		-Artifact Bags (Up to Large)
	-Marlon
		-Loots Bags (All sizes)
	-Sandy
		-Foraging Bags (All sizes)
		-Seed Bags (All sizes)
		-Crop Bags (All sizes)
	-Gus
		-Food Bags (All sizes)


======================================================================================================================================
========	Bundle Bags	   ===================================================================================================
======================================================================================================================================
-Comes in 2 sizes
	-Massive
		-Stores ALL items required by incomplete community center bundles
	-Large
		-Stores everything except items required by the "Bulletin Board" and the "Abandoned Joja Mart" rooms
-Sold by the Travelling Merchant in the forest
-Can only store exactly as many of the item that you still need for INCOMPLETE bundles


======================================================================================================================================
========	Rucksacks	 =====================================================================================================	
======================================================================================================================================
-Available in all 5 sizes
	-Small	- 6 slots - Holds 30 quantity per slot
	-Medium	- 12 slots - Holds 99 quantity per slot
	-Large	- 24 slots - Holds 300 quantity per slot
	-Giant	- 36 slots - Holds 999 quantity per slot
	-Massive- 72 slots - Holds 9999 quantity per slot
-Sold by Pierre
-Can only store non-modded stackable items
	-Can store things like:
		-Fish, Vegetables, Fruits, Craftables, Resources, Seeds, Artifacts, Minerals, Gems, Foods, Animal Products, Floors/Paths etc
	-Cannot store things like:
		-Equippables (Rings, Hats, Clothings)
		-Weapons
		-Fishing Tackles
		-Tools
		-Bags


======================================================================================================================================
========	Omni Bags	 =====================================================================================================
======================================================================================================================================
-Available in all 5 sizes
-Can hold up to 1 of every other bag
	-1 of each standard bag
	-1 bundle bag
	-1 rucksack
-The size of the omni bag determines the maximum size of the bags it can store
	-For example, a Large Omni Bag can only store up to a Large Rucksack. Cannot store a Giant or Massive Rucksack
-Sold by Pierre


======================================================================================================================================
========	Autofilling	     =================================================================================================
======================================================================================================================================
-"Autofill" is a feature that allows bags to automatically be filled with items that you pick up
-Autofilling only takes place when no menus are active. 
	-It won't impact items you buy from shops, or items given to you through the mail etc.
-Each bag has its own autofill toggle. Open the bag and click the "Autofill" icon near the bottom-left of the menu to toggle it.
-Only affects NEW stacks of items that are added to your inventory
	-For example, if you have a Resource Bag set to be Autofilled with picked up wood:
		-If you have 998 wood in your inventory, the next wood you pick up would just stack with it: Now 999
		-Then if you picked up another wood, it would attempt to create a new stack of wood
			-The Resource Bag will detect this and put the new stack inside of it
-Rucksacks can only be autofilled with items that it already has an existing stack of
--Autofill Priority:
	-If multiple bags can be autofilled with the picked up item, the priority is as follows:
		-1. Bundle Bag
		-2. Rucksack that already has a stack of the item in it, and has AutofillPriority set to "High"
		-3. Standard Bag that already has a stack of the item in it
		-4. Standard Bag that does NOT have an existing stack of the item in it
		-5. Rucksack that already has a stack of the item in it, and has AutofillPriority set to "Low"
		-6. Any bags inside of Omni Bags
	-In the case of a tie, the mod will choose the first bag it finds while scanning your inventory left-to-right, top-to-bottom
		-So a bag in the top-left of the inventory has highest priority


======================================================================================================================================
========	Config Files	    ==================================================================================================
======================================================================================================================================
-This mod has 2 different config files. 
	-You will need to run Stardew Valley at least once with the mod installed for default config files to be generated
	-"config.json"
		-Located in the ItemBags mod installation folder
			- <StardewValleyInstallPath>\Mods\ItemBags\config.json
		-Settings:
			-GlobalPriceModifier - A multiplier that affects the price of every type of bag
			-GlobalCapacityModifier - A multiplier that affects how many of each item your bags can store
				-Does not affect Bundle Bags
			-StandardBagSettings
			-BundleBagSettings
			-RucksackSettings
				-MenuColumns 
					- The # of columns to use on the Rucksack menu. Recommended: 12
					- Note that the # of rows is automatically determined based on the # of Slots and the MenuColumns
				-MenuSlotSize
					- The size, in pixels, to use for each slot on the Rucksack menu. Recommended: 64
				-If you wanted your Rucksacks to be able to store tons of items, you could use something like:
					- Slots = 360, MenuColumns = 30, MenuSlotSize = 48
			-OmniBagSettings
			-Shop options
				-HideSmallBagsFromShops, HideMediumBagsFromShops, HideLargeBagsFromShops, HideGiantBagsFromShops, HideMassiveBagsFromShops
				-HideObsoleteBagsFromShops - If "true", shops will no longer sell bags for which you already own at least one copy of a larger size
	-"bagconfig.json"
		-Located in the ItemBags mod installation folder
			- <StardewValleyInstallPath>\Mods\ItemBags\bagconfig.json
			-Prior to version 3.0.5, this file was located in your SMAPI global mod data folders
				- Typically this is "<AppData>\Roaming\StardewValley\.smapi\mod-data\SlayerDharok.Item_Bags\bagconfig.json
		-Not recommended to edit unless you're an advanced user
		-Defines all of the standard bags
			-Each standard bag is composed of:
				-Basic settings like a GUID (Globally Unique Identifier) and an icon to draw on the bag
				-Settings for each size that the bag is available in. Each size configuration is composed of:
					-MenuOptions (How to display the bag's menu)
						-GroupByQuality - If true, items that are available in multiple different qualities will be grouped together
							-The grouped items will be arranged according to GroupedLayoutOptions
						-InventoryColumns - The # of columns to use when drawing your inventory (below the bag's contents). Recommended: 12
						-InventorySlotSize - The size, in pixels, to use for each slot of your inventory (below the bag's contents). Recommended: 64
					-Price - the price before any additional modifiers are applied from the config.json settings
					-Sellers - the names of shops that will sell this size of this bag
					-Items - The items that this bag can store
						-Id - required - You can generally find the Item Id here:
							-https://docs.google.com/spreadsheets/d/1CpDrw23peQiq-C7F2FjYOMePaYe0Rc9BwQsj3h6sjyo/edit#gid=1082266757
							-Note that the Id is the index of the Item in it's parent spritesheet
								-For regular objects, this is the index within <GameInstallPath>\Content\Maps\springObjects.xnb
								-For craftable items, this is the index within <GameInstallPath>\Content\TileSheets\Craftables.xnb
						-HasQualities - required - true if the item is available in multiple qualities (Silver/Gold/Iridium)
						-IsBigCraftable - optional - assumed to be false if not specified
							-If true, it is assumed that the Id refers to TileSheets\Craftables.xnb
							-This is for placeable items like Seed Maker, Scarecrows, Furances, Bee Hives, Tappers etc
						-For example, if you specified Id=16
							-This would refer to Wild Horseradish if IsBigCraftable=false, or Cheese Press if IsBigCraftable=true
			-It is possible to create your own bags by editing this json file. But not recommended and would be very time consuming
	-If you manually edit a config file and mess something up, just delete it. It will be re-created with default settings the next time you launch Stardew Valley.
		-But what if I edited the items that a bag can store, and my bag now contains items it's no longer able to store?
			-There is a built-in fail-safe. Everytime you open a bag, any "invalid" items will be automatically moved to your inventory if there is space.	


======================================================================================================================================
========	Source Code	    ==================================================================================================
======================================================================================================================================
-The source code is available at:
	-https://github.com/Videogamers0/SDV-ItemBags/tree/master/ItemBags
