**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

# Changelog

## 2.13.0
Released May 31st, 2024.

### Fixed
* Support a new 1.6 feature for showing the prices for crafted items.
* Bulk crafting stopping at 20 items when using the Bulk Crafting menu.
* Issue with an upcoming SMAPI update that would cause Better Crafting
  to fail to load due to a constructor signature change within Pintail.

### API
* Added `IEventedInventoryProvider` to allow mods providing inventories
  an alternative to `NetMutex` for establishing a lock for ensuring
  exclusive access for performing write operations to an inventory.


## 2.12.0
Released May 27th, 2024.

### Added
* Setting to enforce some feature flags in multiplayer. Notably, this can
  be used by the multiplayer host to disable the recover trash feature, to
  disable the setting to reveal all gift tastes, to disable recycling items
  with unknown recipes, and to disable recycling recipes with fuzzy items.
* Setting to recycle items of higher-quality than any known recipe produces,
  which is now enabled by default.
* Setting to mark specific storage items as invalid for the purpose of acting
  as sources of items. You can use this to, for example, stop Better Crafting
  from using items that are in Hoppers.

### Changed
* Inventories are now cleaned only once per crafting operation, once the
  crafting has finished, to avoid doing a lot of redundant work.

### Fixed
* Issue where crafting too many items would crash due to a stack exception.
* The crafting menu now protects any items that are in its list of inventories
  from use in crafting, as well as being trashed or removed from the player's
  inventory.

### API
* Added new, simpler event for populating menu containers to allow mods to
  listen to the event while also avoiding needing to include `IBetterCraftingMenu`,
  `IRecipe`, etc. in their copy of the API file.
* Added new event for when a menu closes.
* Added method for casting an `IClickableMenu` to an `IBetterCraftingMenu`,
  if the provided menu is one of our menus.


## 2.11.0
Released May 20th, 2024.

### Added
* Feature to recover trashed items. Just right-click on the trash can
  in your menu (not all menus supported) to open a menu containing items
  you've recently thrown in the trash. Up to 36 items are remembered at
  any given time.
* Ability to invert dynamic rules to exclude recipes from a category,
  rather than including them.

### Compatibility
* Introduced a feature to block harmful Harmony patches from other mods.
  I do not block other mods by default, but I reserve the right to do so
  if another mod causes Better Crafting to break.
* Start logging any mods that have applied Harmony patches to Better Crafting
  at the end of the Game Started event.
* Added `Resource Storage` to the list of mods that are not allowed to
  Harmony patch Better Crafting. This is because Resource Storage attempts
  to apply patches to modify how Better Crafting consumes items, but it
  does so inconsistently and players will be presented with a confusing
  situation where it seems as though they have enough items to perform
  a craft but actually trying to do the craft does nothing.

### Fixed
* Minor text rendering issue where a color would leak outside of a colored
  text segment.


## 2.10.0
Released May 13th, 2024.

### Added
* `Show Unknown Recipes` setting to display unknown recipes in the
  crafting menu, similarly to how they're displayed in the cooking
  menu. That is to say: greyed out.
* `Show Matching Items` setting to display exactly which items should
  be consumed when performing a craft. This *may* be inaccurate but it
  is unlikely. This is disabled by default.

### Changed
* Recipes that create items with a quality greater than low quality
  will now display a quality icon on the crafting menu.

### Fixed
* Integration with SpaceCore Vanilla Asset Expansion crafting recipe
  overrides. I found a way to fix it without needing to wait for Casey
  to merge my pull request.
* Tool-tip caching not working correctly when the crafting menu
  is in category editing mode.

### API
* Much of our configuration is now exposed via our API.
* `CategoryIcon` now has a `Frames` property that can be used to set how
  many frames of animation should be displayed. Note that this is largely
  untested, but it should allow for animated station icons.


## 2.9.0
Released May 5th, 2024.

### Added
* New setting to display the mod that added an item on crafting tool-tips.

### Changed
* Improve integration with SpaceCore Vanilla Asset Extension crafting recipe
  overrides, with support for our ingredient quality features as well as
  item recycling. (Note: This will require an update from SpaceCore before
  it will function correctly.)

### Fixed
* If an error happens in another mod's event handlers for one of our events,
  capture the error properly and log it to minimize disruption to the user.


## 2.8.3
Released May 2nd, 2024.

### Added
* New dynamic rule for matching recipes that a given character likes
  or loves. By default, this only matches discovered gift tastes, but
  you can enable the "Show Undiscovered Gift Tastes" setting to make
  it match everything.

### Changed
* All the dynamic rules for buffs have been combined into one rule,
  with a selection dialog for the specific buff you want. This allows
  for a better user experience by de-cluttering the rule selection
  dialog, as well as showing you how many recipes any particular
  buff actually match.

### Fixed
* The cursor snapping when it shouldn't.
* Issue where Better Crafting was loading `Data/Objects` and `Data/Buffs`
  during GameStarted, which could cache the resources early and cause
  other mods' edits to not apply.
* When drawing the `NEW` label on recipes, use a larger rectangle
  that supports all languages.
* Attempt to fix a sporadic NRE with the temporary loading menu for
  controller users.


## 2.8.2
Released April 20th, 2024.

### Changed
* Improve cursor snapping for controller users when using
  the crafting menu.

### Fixed
* Issue where an error would be printed to the console when
  attempting to load certain textures.
* Update mutexes before attempting to lock them, which should
  improve some issues where mutexes were failing to lock when
  attempting to craft.


## 2.8.1
Released April 19th, 2024.

Sorry for the back to back, but there was a compatibility issue
with other mods causing the menu to not work.

### Fixed
* The temporary menu class used to avoid immediately loading
  the full Better Crafting menu would throw an error when
  the user is using certain mod combinations.


## 2.8.0
Released April 19th, 2024.

### New Features
* You can copy, paste, and delete categories when editing them now,
  making it easier to move a category into or out of your Per-Save
  Data or to share them. Additionally, you can shift-click the
  delete button to reset *all* your categories.
* Better Crafting will attempt to automatically color its UI
  buttons to match your current UI theme.

### Changed
* When opening the game menu, the Better Crafting menu won't
  load until you directly access it, which should improve
  performance somewhat.
* Added some diagnostic logging for performance.

### Fixed
* Workbench connectors not working correctly.

### Translation
* Removed the Turkish translation file, as that is out of
  date and there is an external mod providing it.

### API
* Added a new event to let other mods add extra icons to the
  built-in icon picker.
* Fixed setting item to `null` in pre-craft event causing
  crafting to fail.


## 2.7.0
Released April 17th, 2024.

### New Features
* The mod now uses a global save for categories and favorite recipes.
  This can be toggled on a per-save basis. Saves with existing
  category / favorite customization will automatically start with
  per-save customization enabled.
* Added new dynamic rules for selecting recipes based on item category,
  context tags, and edibility. With these, almost all recipes in the
  base game are categorized using rules rather than manually.

### Changes
* The "Items from Mod" dynamic rule now only displays the count of
  recipes within the current menu mode (cooking vs crafting) when
  you're selecting a mod.
* Update tool-tip rendering in our menu to be more like the base game.

### Fixed
* Unable to translate a certain string from the "Items from Mod"
  selection dialog.
* When using the Kitchen, always add the location's fridge to the
  inventories to craft from, regardless of other discover settings.
* The controls to toggle dynamic rules on, as well as include items
  in the Misc. category, should not appear on the Misc. category itself.
* Change to word wrapping causing some text to not appear on new
  lines when it should.

### API Changes
* When using the trigger/map action to open the menu, there is a new
  syntax that allows for greater flexibility in configuring how
  inventories are discovered.


## 2.6.2
Released April 13th, 2024.

### Changes
* Add a quick setting to hide the Edit Categories button, since
  someone requested it and it's easy.

### Fixed
* Issue where the Use Nearby Chests setting would not have its initial
  value set correctly when opening the settings menu, causing it to
  be reduced over time.
* Issue where the Use Nearby Chests setting would be unable to include
  all chests in its active area due to scanning limitations meant to
  limit performance impacts.
* Improve performance of item comparison slightly by changing how we
  apply our harmony patch for Item.canStackWith.
* Improve word wrapping when dealing with exceptionally long unbroken
  character sequences, which is mainly an issue in
  non-English languages.

### Translation
* Updated Korean language strings. Thanks, wally.


## 2.6.1
Released April 12th, 2024.

### Changes
* Junimo Chests are no longer blacklisted.

### Fixed
* Properly check if the optional background texture is available before
  attempting to load it. Fixes error messages every time the menu opens.
* Fixed an issue where the Transfer to Inventories feature would not
  work correctly when using Better Chests.
* Improve de-duplication logic for inventories.


## 2.6.0
Released April 11th, 2024.

This should be the last release for a while, barring any unexpected bugs.
I've been toying with a few ideas while mindlessly working on this, but
I have what I want thought out for Almanac now.

### New Features
* You can now re-order your categories when editing them.
* There is a new option to open the crafting menu with the full height
  of your screen if opened via a workbench, kitchen, or other means
  that does not place it within the game menu.

### Changes
* When listing the available mods to include items from a mod, we now
  count how many items there are from each mod, and put those without
  items at the end of the list. Further, we sort the list alphabetically
  by mod name.
* When crafting an item that will have attachment slots, show the
  attachment slots in the recipe tool-tip.
* When viewing a recipe with an absurd number of ingredients, try to
  fit it on the screen better by using more columns.

### Fixed
* Apply theme text colors more thoroughly to the crafting menu.
* Spaces appearing near recipes when a recipe would be included both
  by manual selection and because it matched a rule.

### API Changes
* Added the ability for content authors to create custom dynamic rules
  using the game's native item queries feature by modifying the
  target path `Mods/leclair.bettercrafting/Rules`


## 2.5.0
Released April 10th, 2024.

### New Features
* Added the ability to label recipes as new. You can choose to either
  label a recipe as new if you haven't crafted it yet, or if you haven't
  hovered your mouse over it to view its tool-tip yet.

### Changes
* You can now toggle recipes on and off even if a category is using rules.
  Please note that you can only enable extra recipes, you can't hide
  recipes that matched a rule.
* Added a dynamic category rule to match all of a specific mod's items.
* Added dynamic category rules for: floors and paths, fences, furniture.
* Added dynamic category rules for all remaining item buffs.
* Updated how buffs are displayed in recipe tool-tips, to bring them
  more in line with Stardew Valley 1.6.

### Fixed
* When receiving recipes from other C# mods via the API, optional
  interfaces are now detected correctly.
* Changed the logic for loading recipes to not use locking, in case
  that was what's been causing some loading freezes for one user.

### API Changes
* Added the ability to track which specific Items are being consumed
  when crafting a recipe.
* Added a new post-craft event that can be used to modify items after
  its ingredients are consumed, using the tracked items as
  mentioned above.
* Added the ability for mods to register crafting and post-crafting
  events that apply to all recipes.

## 2.4.0
Released April 7th, 2024.

### New Features

* Added a setting to allow crafting from nearby chests. The request was
  made for Convenient Chests compatibility, and it was easier to just
  make a distinct feature for Better Crafting.

### Changes

* Started caching a lot more recipe state in the crafting menu, improving
  performance by over 100% on average.

### Fixed

* Make sure to check that the item is actually an object before checking
  for buff data.
* The menu's exit code not running when closing the game menu while the
  crafting page is not the current page.
* Attempt to handle mutexes that aren't reported as locked but that we
  have obtained a lock for, since apparently that's a thing.
* Fix an issue with the Spooky Action system where locations would be
  considered occupied even after the player closed their menu.

### API Changes

* Added `IDynamicDrawingRecipe` for recipes that should have dynamic
  icons in the crafting menu, along with an API method that wraps
  the recipes to force them to be detected properly.
* The RecipeBuilder can set a drawing function using the same
  format as `IDynamicDrawingRecipe`.
* Data-driven ingredients can now have conditions to control whether
  or not they're displayed and required.
* Data-driven ingredients can now use item query fields to help
  filter items.


## 2.3.0
Released April 6th, 2024.

Yes, I know. Two in one day. Sorry, I just needed to fix a few issues
with the new data-driven crafting recipes. I've also added a bit more
logging for the Better Chests interaction issues.

### Changes

* Added support for rendering smoked fish using my custom item renderer.
* Add extra debug logging for when we are unable to lock inventories.
* Remove useless debug logging from the PFM integration module.
* Vanilla crafting recipes are suppressed if there is a data-driven
  recipe with the same Id.

### Fixed

* Properly check each recipe's `Condition` field.
* Properly support learning data-driven crafting recipes using the mail
  `%item craftingrecipe [recipe]%%` command.

### API Changes

* `CreateBaseIngredient()`, `CreateMatcherIngredient()`, and
  `CreateCurrencyIngredient()` now all have a parameter to set how
  much you get back when performing recycling.
* The `CreateMatcherIngredient()` method now takes a delegate to
  decide which item to return upon recycling, rather than a static
  Item reference.
* Data-driven recipes now have a flag for if they are known by default.
* Data-driven ingredients can now use item spawn fields to specify
  which item they would be recycled into.
* Data-driven ingredients now have a recycle rate field to specify
  how much the player should get back if they recycle it.


## 2.2.0
Released April 6th, 2024.

### New Features

* Added the ability for content pack creators to create more elaborate
  crafting recipes.
* Fixed a major bug.

### Changes

* If, for some reason, Better Crafting is unable to gain access to
  a chest when you try crafting something, it will display a message.
* While Better Crafting is actively processing a craft, the cursor
  will appear as an hourglass.

### Fixes

* Major: The crafting menu will no longer lock up when attempting to
  craft things using chests located in other maps. The issue has been
  fully diagnosed through much effort of myself and the creator
  of Better Chests.
* When using the farmhouse kitchen, mini-fridges would not properly
  have their contents made available.

### API Changes

* `IRecipe` now has a flag for marking that a recipe cannot be
  reversed using recycling.
* Added an entire data structure for making custom recipes through
  Content Patcher by modifying the `Mods/leclair.bettercrafting/Recipes`
  target. Documentation to come.
* Added a `WithInventories()` method to let other mods take advantage
  of Better Crafting's robust mutex handling.


## 2.1.0
Released April 4th, 2024.

### New Features

* Added first-party custom crafting stations for content pack creators.
* Added a fancy label to the menu that appears when using a Kitchen,
  Workbench, etc. as well as the knew custom stations.

### Changes

* Buildings with inventories are now supported, such as Junimo Huts.

### Fixes

* Better null handling when checking for big craftable actions. Stops
  spamming warnings in the log in certain circumstances.

### Mod Compatibility

* Detect storage items added by Expanded Storage and categorize
  them appropriately.

### API Changes

* Added a `[station]` property to the map tile action.
* Added several properties to `IBetterCraftingMenu` to expose more of
  the menu's state.
* Add a flag to the `IPopulateContainersEvent` to let mods disable
  container discovery.
* Add a method to get recipes that should be exclusive to certain
  crafting stations.
* Expose the resource `Mods/leclair.bettercrafting/CraftingStations`
  for loading data on custom crafting stations, as well as reading
  `stations.json` files from content packs owned by Better Crafting.
* Expose the resource `Mods/leclair.bettercrafting/Categories` for
  customizing the default categories with content patcher. Please
  note that editing the defaults in this way is not effective if
  users have customized their categories.


## 2.0.0
Released April 3rd, 2024.

Kept you waiting, huh?

### New Features

* Works on 1.6
* Dynamic rules for foods that buff custom skills added through SpaceCore.
* Separate mod that adds a Magic Workbench, which lets you craft using
  items from within adjacent buildings.

### Changes

* Storage items are no longer included in the Machines dynamic rule. There
  is now a specific Storage rule.

### Fixes

* Various typos, mostly in documentation.
* Improve menu performance a bit by caching some of item tool-tips.
* When displaying a recipe with more than 20 ingredients in the Bulk Crafting
  menu, only show 20 ingredients at a time to support smaller displays. (Also
  who is making recipes like this? Who hurt you?)
* Certain currency ingredients not rendering correctly, if anyone was using
  currency ingredients.

### Removed

* Dynamic Game Assets compatibility code. DGA is dead, long live 1.6.

### API Changes

* Added the `leclair.bettercrafting_OpenMenu [cooking] [includeBuildings]`
  map tile action.
* Added the ability to run map tile actions from big craftables using the
  `leclair.bettercrafting_PerformAction` custom field.
* Added a `leclair.bettercrafting_HAS_WORKBENCH` game state query to check if a
  Workbench has been placed at the farm.
* Renamed IInventory to IBCInventory to avoid collision with vanilla.
* IInventoryProviders have a method now to return a vanilla IInventory
  to allow for future optimizations.
* Removed obsolete API methods.


## 1.5.0
Released January 14th, 2022.

### New Features

* Added new dynamic filter for creating a category with all recipes.
* Added option to include a category's recipes within the Miscellaneous
  category. Usually, only recipes that don't appear in any other categories
  appear within Misc. However, enabling this on a category will allow its
  recipes to still appear within Miscellaneous.
* Added a new feature to recycle (or un-craft) items directly from the crafting
  menu. As this potentially affects game balance, the feature is disabled by
  default. You can also enable it separately for crafting and cooking.

### Changes

* Added a few new icons to the icon picker.

### Fixes

* Do not let users open the settings menu if they're holding an item, as that
  may result in the loss of the item.
* Pressing the menu close button (`E`) may cause the menu to close when it
  should not.

### Translation

* Added Portuguese language support.

### API Changes

* Fix typos in documentation.
* Allow passing a specific item to be returned when recycling a matcher-based
  ingredient. Doing so disables fuzzy-type item matching for the ingredient.


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
