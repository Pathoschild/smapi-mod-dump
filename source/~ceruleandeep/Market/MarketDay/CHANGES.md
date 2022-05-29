**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ceruleandeep/CeruleanStardewMods**

----

# Changes

1.1.3-beta.4
* Ensure that players get shops when the market is shrunk

1.1.3-beta.3
* Configure chests to be ignored by Better Chests

1.1.3-beta.2
* Remove hard dependency on DGA
  
1.1.3-beta.1
* Fix item handling when buying hats, tools, and weapons 
* Move GrangeShops.cs to #nullable

1.1.2 RELEASE
* Fix DGA item sales
* Fix sign summary in Chinese

1.1.1 RELEASE
* Fix town map change tracking in Chinese

1.1.0 RELEASE
* Forbid Krobus from leaving the house, their sprite can't handle it
* Scale prize difficulty if player is using a reduced profit margin

1.1.0-beta.2
* Add dynamic signs for shops without custom art
* Change sync strategery againnn
* Add DGA item support

1.1.0-beta.1
* Fixes some issues with changing market configuration in-game
* George's mailbox is back where it should be
* Reorganised GMCM screen

1.0.9 RELEASE

1.0.9-beta.9
* Re-design state model to respond to Maps/Town reloads
  * (if your market opens and closes through the day, this is why, let me know!)
* Move shop stock caching around to let JA finish

* Add Chinese translations from Becks723
* Add plain version of shop
* Add CP player option to have rugs/plain shops

1.0.9-beta.8
* Persist mails into modData
* Add visit from Lewis to do initial setup
* Cap bonus multiplier at each level of challenge mode

1.0.9-beta.7
* Load challenge mode data earlier, fix item check
* Disable Shop 0 when Paisley's Bridal is open

1.0.9-beta.6
* Add challenge mode, send progress and prize emails with MFM
* Stop chests appearing in Chests Anywhere

1.0.9-beta.5
* schedule time for spouses and shop owners to run their shops
* owners much less likely to buy their own stuff
* NPCs take a more sensible path through the market
* another try at getting the map patching just right

1.0.9-beta.4
  * Fix ordering of NPC gift preferences when buying stuff

1.0.9-beta.3: Multi-shop
  * Add one shop for each player
  * Pay farmers separately when separate money in use
  * Patch paths to make room for one more shop
  * Add 10-shops option
  * Track sales and gold in modData
  * Sync market when GMCM options are changed
  
1.0.9-beta.1
  * Wrap another NPC gift tastes call in a try/catch block
  * Fix some ordering stuff that prevented shop en/dis from being saved
  
1.0.8 RELEASE
  * Add shops for Wizard, Haley, Elliott, Sam, Jas+Vincent, Maru
  * Catch exceptions caused by other lazy mods not doing gift preference
  * Move all market-opening settings to C# out of CP
  * Make GMM compat optional and smarter
  * Option to disable shops in config.json
  * Dump mod state to log each day
  * Optionally open when raining or snowing
  * Fix alternative currency/item-payment support
  * Display total sales value in sign message
  * ShopTiles is now a property so get set for mayhem
  
1.0.7 RELEASE
  * fix exploit where shop could be opened after hours via sign
  * remove click handler so we don't react to STF tile props
  * remove SVE's picnic set more thoroughly
  
1.0.5 RELEASE: New shops, new art, item quality
  * New shops: Caroline, Gunther, Pierre, Demetrius
  * Support item quality in NPC shops
  * Custom open and closed signs for shops
  * Load random items from a category
  * Fix annoying log spam when a not-ours chest is clicked on
