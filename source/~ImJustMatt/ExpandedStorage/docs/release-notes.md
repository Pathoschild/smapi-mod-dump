**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ImJustMatt/StardewMods**

----

# Release Notes

## 1.6.10
- Update to SMAPI 3.12

## 1.6.9
- Fixed chest opening on host from farmhands in same location
- Removed configurable controls for carry/access carried chests

## 1.6.8
- Fixed farmhand being unable to access carried chest
- Improve chest matching (fixes Volcano Dungeon chests)
- Restructure codebase
- Refactor patches
- Preliminary 64-bit support

## 1.6.7
- Split off Automate compatibility into its own mod [XSAutomate](https://github.com/ImJustMatt/StardewMods/tree/master/XSAutomate)

## 1.6.6
- Added support for Auto-Grabber
- Fixed certain chests such as Junimo Chest could not be opened
- Fixed Automate only connecting to top-left corner of Bigger Expanded Storages

## 1.6.5

- Added Animation support
- Added Open when Farmer nearby
- Added Config option to enable HSL Color Picker
- Added Config to change Log Level when loading storages
- Updated Generic Mod Config Menu
- Fixed Bigger Storages breaking between saves
- Fixed Purchased Storages not working until they are dropped

## 1.6.4

- Fixed Mini-Fridge not animating correctly
- Fixed divide by zero errors

## 1.6.3

- Added indestructible chests option
- Added option to disable player config for storages
- Optimized Debris patch when no Vacuum chests
- Optimized Overlay patch to use RenderedActiveMenu event

## 1.6.2

- Optimized image caching
- Fixed Mini-Shipping Bin not opening
- Fixed chests with less than 12 item slots not loading
- Fixed crash when GMCM is not installed
- Fixed crash when search bar is disabled
- Prevent right-click on Carryable chests in inventory

## 1.6.1

- Added Content Patcher support for SpriteSheets and Tab Images
- Fixed Null reference exception
- Fixed storage config not saving

## 1.6.0

- Added Resize Inventory Menu to Generic Mod Config Menu
- Draw a hologram when placing Bigger Expanded Storages
- Fix depth issue with bigger expanded storages
- Patch Chests Anywhere so that shipping bin will refresh on item grab
- Added a summary of Default Storage config to logs

## 1.6.0-rc.1

- Added Bigger Expanded Storages
- Added Advanced Color Picker
- Added ExpandedStorageAPI methods
- Added default config for unknown storages
- Optimized texture loading for chest tabs
- Updated content/config format

## 1.5.3

- Fixed error when loading chest without tabs

## 1.5.2

- Added all Furniture to category_furniture tag
- Added category_artifact tab
- Added donate_museum tag
- Added donate_bundle tag
- Fixed compatibility with Remote Fridge Storage

## 1.5.1

- Fixed compatibility with CJBItemSpawner 

## 1.5.0

- Added vacuum items to chest
- Added chest to chest menu
- Added craft from carried chest
- Added new config options
- Added additional options for content packs
- Added item count in menu for carried chests
- Animate held chests and chests in menu
- Color held chests and chests in menu
- Optimize searching chests for vacuum items
- Reduced some logging
- Fixed controller resetting position
- Fixed items not refreshing
- Fixed stackable carried chests

## 1.4.2

- Added non-placeable storage
- Support Generic Mod Config Menu
- Support Special Chest Types
- Fixed config loading duplicate data after Return to Title
- Fixed error when config is missing

## 1.3.1

- Added chest tabs
- Fixed scrolling desync

## 1.2.2

- Support vanilla chests
- Support colored chests

## 1.1.0

- Added patching system
- Added resizable chest menu
- Added controller config

## 1.0.4

- Added Overlay for scrolling inventory
- Added controller support
- Added Carry Chest
- Fixed Chests Anywhere compatibility
- Remove dependency on StardewHack

## 1.0.0

- Initial Release