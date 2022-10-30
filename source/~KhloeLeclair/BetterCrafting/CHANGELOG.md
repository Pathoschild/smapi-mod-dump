**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

# Changelog

## 1.4.0
Released October 28th, 2022.

### General

* Update to use SMAPI 3.17. Older versions of SMAPI are no longer supported.
* Improve performance slightly by reducing iteration when determining if a
  given recipe has sufficient ingredients to be crafted.

### Fixes

* When clicking on UI elements when editing a category that is not rule based,
  do not send click events to the rule editors.
* When performing inventory manipulation (such as crafting) using a chest
  located on another map, manually update the mutex to ensure the action
  doesn't hang.
* When the "Crafting Skill" mod is enabled, do not use the incorrect ingredient
  consumption code. This should prevent any odd behavior, especially related
  to limit quality crafting and/or using lower quality ingredients first.

### API Changes

* Each mod now receives a separate API instance, which will allow for better
  tracking of which mod provided which data in the future.


## 1.3.1
Released September 21st, 2022.

### Fixes

* Larger grid items in the Better Crafting menu were having their sizes
  calculated incorrectly.

### API Changes

* Added a new recipe builder to make it easier for other C# mods to
  manipulate recipes.
* Added a new method to count items, which is compatible with Better Crafting's
  quality settings and aware of the mod "Stack Quality".
* Change `CreateMatcherIngredient` to use methods for setting a display name
  and texture, to improve performance and i18n support.


## 1.3.0
Released September 20th, 2022.

### New Features

* Categories can now have their items selected using dynamic filters, rather
  than being picked manually. This is now the default behavior for all
  categories in the cooking menu.
* Cookout Kits can now function as Workbenches, but for cooking! When enabled,
  using a Cookout Kit will let you use items from nearby chests. Additionally,
  Cookout Kits can be made longer lasting so that they don't vanish overnight
  or when you break them.

### Changes

* The maximum number of visible tabs along the left side of the crafting menu
  is now calculated based on the height of the menu, rather than being
  hard coded.
* Use two columns for displaying a recipe's ingredients if the recipe has more
  than five ingredients.
* Add support for more category ingredients with names and icons.

### Fixes

* When opening a Workbench, return that the action succeeded so that they game
  won't try performing another action immediately.
* Do not crash in the menu handler when replacing a crafting menu with no
  material containers list.
* The crafting menu handles it better when the game window changes size, though
  it may still act a bit odd in some cases.
* Catch an error if one is thrown while getting a list of all NPCs in the
  game for the purpose of displaying likes/loves.
* Do not scroll the recipe list when the mouse is over the inventory display,
  to improve compatibility with certain mods that may modify the inventory.
* Un-cache recipes whenever the recipe data assets are invalidated.
* Do not crash in the cooking menu when hovering over a recipe that creates an
  item with no objectInformation entry.
* Do not crash in the crafting menu if more than one recipe is registered with
  the same name.
* Remove duplicate inventories from the inventories list after discovery to
  avoid displaying inaccurate ingredient counts.

### Mod Compatibility

* Added support for [Stack Quality](https://www.nexusmods.com/stardewvalley/mods/13724)
  when it comes to crafting. It will appropriately detect the number of items
  in a stack of a given quality, particularly when limiting crafting by quality.
* Added support for [Custom Backpack Framework](https://www.nexusmods.com/stardewvalley/mods/13826)
  (and potentially other backpack mods) by allowing the crafting menu to expand
  as necessary to display more inventory rows when there are more than 3 rows.

### API Changes

* Added method to register new dynamic rule handlers. Configuring rules is
  still a work in progress, but a simple text input is supported.
* The method to add a new default category now allows you to use rules.
* Removed a couple deprecated methods from the API interface. They still work,
  but new implementations shouldn't use them.


## 1.2.1
Released September 3rd, 2022.

### New Features

* Display character heads for gift tastes by default, rather than names. This
  should take up quite a bit less space for items that are liked and/or loved
  by many characters. There is an option to display names instead.

### Fixes

* Do not error if an error is thrown when trying to determine if an NPC likes
  a given item.

### API Changes

* Added a property to `IBetterCraftingMenu` for accessing the active `IRecipe`,
  allowing other mods to more easily perform actions based on the current recipe.


## 1.2.0
Released September 2nd, 2022.

### New Features

* The crafting menu now displays NPC gift tastes in tool-tips, and allows
  searching for items that specific NPCs like or love. By default, this only
  displays gift tastes that you have already discovered in-game, as well as
  only appearing on tool-tips while Shift is being held.

### Fixes

* Do not ignore containers with a `null` location when performing crafting.
* Do not ignore the existing container list when replacing the crafting menu
  contained within GameMenu, in case other mods have added containers.

### Mod Compatibility

* Added an option to change the priority of Better Crafting's menu event
  handler, potentially allowing for increased compatibility with some mods that
  access the game's default crafting menu.
* Added built-in support for SpaceCore and Dynamic Game Assets. The extra file
  "SpaceCore Support" is no longer necessary.

### API Changes

* Added an event to allow other mods to easily add custom containers to any
  Better Crafting menu, including the menu embedded in the GameMenu.
* Added a method for getting a reference to the currently active Better Crafting
  menu, in case other mods need it for some reason.


## 1.1.1
Released May 25th, 2022.

### Fixes

* Whenever closing the menu, make sure that we release all inventory locks so
  that no chests / workbenches / etc. are left in a state where they only
  function for one player in multiplayer scenarios.


## 1.1.0
Released May 23rd, 2022.

### Fixes

* Add translation support for currency ingredients.
* Do not write empty category arrays to a user's saved categories if the
  data has not been initialized yet.
* Fix support for inventory providers that don't have require mutexes, as
  well as providers for inventories without a physical location.

### API Changes

* Add translation support for default categories added through the API.


## 1.0.0
Released April 26th, 2022.

### New Features

* Added a button to transfer items from your inventory into all the
  chests connected to a workbench / kitchen. This is like a chest's
  "Add to Existing Stacks" button, but for a lot of chests at once.

### Fixes

* Improve error checking for recipes, since a lot of mods unfortunately
  introduce recipes with data errors.
* Stop repositioning the mouse cursor when using a gamepad and exiting
  the menu.
* Draw better tooltips when a recipe doesn't have ingredients.

### Mod Compatibility

* Added built-in support for Vintage Interface 1.

### API Changes

* Improve support for custom recipes not based on an existing vanilla
  `CraftingRecipe`, including support for recipes that don't produce
  items at all.
* The API now provides convenience methods for creating simple ingredients
  so that external mods don't need to reinvent the wheel for basic tasks.
* The API now provides convenience methods for creating simple recipes that
  implement existing `CraftingRecipe`s with custom ingredients.
* The API now lets mods create new default categories and add recipes to them.
* Consolidate all interfaces used by the API into a single `.cs` file for easy
  inclusion in other mods once SMAPI 3.14 is available.
* Basically, a lot of API stuff happened and will be cool once 3.14 is out.

### Optional Add-Ons

* Created a new add-on for Better Crafting that lets you craft buildings via
  the crafting menu. Buildings constructed this way are finished instantly.
  Due to the somewhat cheaty nature of the add-on, it is not included by
  default but available as an optional download.

### Maintenance

* All Better Crafting code, including the API, now uses nullable annotations
  and file-scoped namespaces.
* General code cleanup.


## 0.15.0
Released March 15th, 2022.

### Mod Compatibility

* Added built-in support for Vintage Interface 2, Overgrown Flowery Interface,
  and Starry Sky Interface.

### API Changes

* Added a `bc_theme` command to list all themes, or change the current theme
  if called with an argument. `bc_theme reload` will reload all themes,
  including the active theme. `bc_retheme` can be used as a shortcut for that.
* We now have support for themes. Themes allow you to replace the buttons
  texture, as well as set custom colors for a few things in the menu. See
  the Vintage Interface 2 theme for an idea. Content packs can add a theme
  by including a theme.json.


## 0.14.1
Released March 14th, 2022.

### Fixes

* When opening settings from the game menu and closing it, the menu could
  appear with a strange offset in some situations.

### Translation

* Added Chinese language support.


## 0.14.0
Released March 12th, 2022.

### General

* Added support for scrolling the category tabs.
* Removed the limit on the number of categories.
* Added a console command to clear the recipe cache.
* Add a warning in the console when DynamicGameAssets is detected but
  our compatibility mod is not.

### Fixes

* Use separate recipe caches for each player in split-screen, as certain
  recipe adjustments may only affect one player.
* Clear the recipe cache at the start of every new day in case recipes
  have been adjusted due to levelling up, etc.
* Use integer positioning for all UI elements to avoid odd rendering of
  text and sprites.

## 0.13.2
Released March 7th, 2022.

### General

* Fixed issue where custom categories for cooking recipes would not be saved.
* Do not display the GMCM Settings button on the crafting menu for split-screen
  players. The GMCM menu is not controller accessible and opening it may
  soft lock that player.
* Play a sound when opening the icon picker.
* Make it easier to navigate to the category name and icon picker controls when
  using a controller for input.

### Translation

* Improved Spanish language support.


## 0.13.1
Released March 6th, 2022.

### General

* Add support for localizations overriding the ingredient search character.
* Fix the search pop-up's tip not having localization support.
* Improve tool-tip positioning logic to avoid covering the cursor.

### Translation

* Improved Russian language support.


## 0.13.0
Released March 6th, 2022.

### General

* Added a search button to the crafting menu! Search by recipe name, description,
  and optionally its ingredients.
* When searching, the matching bits of text in tool-tips are highlighted.
* Added an option to sort recipes alphabetically. I might need to change the way
  this is handled to make sense in other languages. Let me know if so!
* When editing a category, you can now hold shift to view an item's full tooltip.
  This should make it easier to double check where you want to put something,
  especially when categorizing foods.
* Center parts of the Bulk Crafting menu to make it look nicer.


## 0.12.0
Released March 4th, 2022.

### General

* Added "Bulk Crafting"! This changes the default action performed when you use
  the Use Tool key (Right-Click) on a recipe.
* Added configuration for how the `Use Tool` and `Action` keys behave.
* Added separate key bindings for `Favorite` and `Open Bulk Crafting`.
* Added icon picker for categories for setting icons unrelated to icons.

### Mod Compatibility

* Disable the theoretical StackSplit Redux support for bulk crafting recipes,
  since the new Bulk Crafting interface works much better for that. Still waiting
  on SSR to accept my pull request though, so it's not like anyone is missing out.

### Other Stuff

* Add an update key for ModDrop to hopefully make SMAPI stop saying there's
  an update when the update is for the SpaceCore support optional file.


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
