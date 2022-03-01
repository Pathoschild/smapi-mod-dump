**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

# Changelog

## 0.11.0
Released February 26th, 2022.

### General

* Added "Extended Workbench" feature. When enabled, Better Crafting will detect
  chests connected to the Workbench or kitchen and make their inventories
  available for crafting.
* Add support for re-opening the pause menu to its crafting page after closing
  Better Crafting's settings in GMCM.
* Added support for using Ctrl+Shift for bulk crafting.
* Fixed the fridge in the island farmhouse not being detected properly.
* Fixed overly-strict rules for validating chests without a set game location.
* Fixed bug where ingredients may not be consumed in some cases.

### Mod Compatibility

* Improve support for CustomCraftingStations using a Harmony patch to get
  direct access to the CCS restricted recipe list.
* Improve support for CustomCraftingStations by replacing its crafting menu
  with a Better Crafting menu when possible.

### API Changes

* Added more signatures for `OpenCraftingMenu()` adding support for more paramters.
* Added `RegisterInventoryProvider()` method that uses `Func` and `Action` to
  allow consumers to add custom inventory providers without consuming
  Better Crafting's interfaces.
* Added a separate IInventoryProvider registration method.
* Added a method to unregister inventory providers.


## 0.10.0
Released February 24th, 2022.

### General

* Added option to use lower quality ingredients first when crafting.
* Added option to limit crafting materials based on quality.
* Added notice to tooltips when a recipe added with the API does not support
  quality limits.
* Added settings button to the Better Crafting menu when the
  Generic Mod Config Menu is installed.
* Fixed ingredient check calls not passing a list of inventories along, which
  would break SpaceCore support.
* Fixed rendering order issue with certain menu components that would cause
  tooltips to draw under the menu components.
* Fixed bug with my menu closing code that might store a reference to a stale
  cleanup method.

### Mod Compatibility Changes

* Implement support for a potential new StackSplitRedux API. Awaiting the
  acceptance of a [pull request](https://github.com/pepoluan/StackSplitRedux/pull/1).
* Add slightly better support for CustomCraftingStations. The crafting menu in
  the game menu now supports limiting the listed crafting recipes. However,
  opening the crafting menu from a workbench or the kitchen will still fail to
  limit the listed crafting recipes.

### API Changes

* Added `OpenCraftingMenu()` call that can be used to open the Better Crafting
  menu. It supports a few useful paramters, including the ability to limit
  the menu to only display certain recipes.
* Added `GetMenuType()` call to get a reference to the BetterCraftingMenu class
  for doing evil things that might be better handled with a feature request.
* Added `RemoveRecipeProvider()` call.
* Added `InvalidateRecipeCache()` call.
* Added `GetRecipes()` call to get an iterator over all our known recipes.
* IRecipeProvider instances can now provide additional recipes that might have
  not otherwise been discovered.


## 0.9.1
Released February 23rd, 2022.

### General

* Add crafted counts to recipe tooltips.

### Mod Compatibility Fixes

* Add support for mods that expand the player's inventory beyond the default
  three rows. Works with Bigger Backpack, and should work with other mods.
* Add support for the Cooking Skill mod. This isn't heavily tested, and might
  not work for some cases. I'm trying to duplicate the behavior of the default
  CraftingPage when the mod is present.
* Attempt to register our menu with StackSplitRedux. This doesn't do anything
  yet. They'll need to add support for Better Crafting in that mod directly, or
  else expand their API so that we can register our child inventory menu and,
  preferrably, trigger the pop-up on demand so we can integrate it into crafting.
* Add support for chests that are in other maps. This is not heavily tested.
* Add stubs to our crafting menu so that Custom Crafting Stations no longer
  throws an exception in an event handler. Not sure how to handle limiting
  the available recipes. I need to find some content packs using CCS so I
  can experiment.


## 0.9.0
Released February 22nd, 2022.

* Initial release.
