**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/alanperrow/StardewModding**

----

TO DO:
 - Add new opt-in config option that adds a left-to-right warp with the gamepad cursor in inventory page.
 - Add new opt-in config option that allows quick stacking of items without checking quality.
 - Support compatibility with "Vertical Toolbar" mod.
   - If possible, extend compatibility for all mods that modify the player's inventory size, even if in (what I'm presuming to be) irregular ways like this one.
 - Support compatibility with mods that add storage objects, such as
   - Mega Storage
   - Expanded Storage
   - Storage Variety
 - Re-investigate feasability of preventing favorited items from being placed in the shipping bin.
 - tails618: "community center bundles in the inventory is gone... am I missing something?". Validate if this is true.

 DONE:
 - Fix assets directory issue due to capitalization.
 - Add hotkey for quick stack via opt-in config option.
 - (X) Add hotkey for "Organize" button via opt-in config option.
   - No need for this, as base game already has a hotkey! When in your inventory menu, press the controller's "back" button to organize inventory.

 ---

NexusMods link: https://www.nexusmods.com/stardewvalley/mods/10384

# Convenient Inventory
Adds convenience features to the player's inventory, such as quick stack to nearby chests and favorited items.

## Preview
One of the most tedious parts of any game is inventory management, and this is especially true in Stardew Valley. Every day, whether it be from farming produce, mining rewards, or freshly caught fish, you will be emptying out your backpack to store items into chests on your farm. After a while, this process becomes all too familiar: open chest, deposit items, close chest, open chest, deposit items, close chest, open chest, deposit items, close chest, ... etc.

What if you could stow away all the items taking up space in your backpack, instantly, while keeping the important ones?

![](https://imgur.com/R4QWKVI.gif)

## Features
#### Quick Stack to Nearby Chests
Click the new "Quick Stack to Nearby Chests" button in the player's inventory UI to quickly deposit items into chests within a nearby range.

#### Favorite Items
Hold the favorite-hotkey (Left Alt, by default) and select items in the player's inventory to favorite them.

Favorited items are prevented from:
 - Being quick stacked
 - Being trashed
 - Being dropped
 - Being considered when using the "Organize" button in the player's inventory
 - Being considered when using the "Add to Existing Stacks" button in a chest

## Config
 - **IsEnableQuickStack**: If enabled, adds a "Quick Stack To Nearby Chests" button to your inventory menu. Pressing this button will stack items from your inventory to any nearby chests which contain that item.
 - **QuickStackRange**: How many tiles away from the player to search for nearby chests.
 - **IsEnableQuickStackHotkey**: If enabled, pressing either of the quick stack hotkeys specified below will quick stack your items, even outside of your inventory menu.
 - **QuickStackKeyboardHotkey**: Press this key to quick stack your items.
 - **QuickStackControllerHotkey**: Press this button to quick stack your items.
 - **IsQuickStackIntoBuildingsWithInventories**: If enabled, nearby buildings with inventories (such as Mills or Junimo Huts) will also be checked when quick stacking.
 - **IsQuickStackOverflowItems**: If enabled, quick stack will place as many items as possible into chests which contain that item, rather than just a single stack.
 - **IsQuickStackTooltipDrawNearbyChests**: If enabled, hovering over the quick stack button will show a preview of all nearby chests, ordered by distance.
 - **IsEnableFavoriteItems**: If enabled, items in your inventory can be favorited. Favorited items will be ignored when stacking into chests.
 - **FavoriteItemsHighlightTextureChoice**: Choose your preferred texture style for highlighting favorited items in your inventory.
   - ( 0: ![](https://i.imgur.com/fTMl0FT.png),  1: ![](https://i.imgur.com/NTlia1R.png),  2: ![](https://i.imgur.com/QGztt8Q.png),  3: ![](https://i.imgur.com/MBG2A6e.png),  4: ![](https://i.imgur.com/rZqklnN.png),  5: ![](https://i.imgur.com/FvKpyZV.png) )
 - **FavoriteItemsKeyboardHotkey**: Hold this key when selecting an item to favorite it.
 - **FavoriteItemsControllerHotkey**: Hold this button when selecting an item to favorite it. (For controller support)

## Compatibility
 - Supports single player, split-screen local multiplayer, and online multiplayer.
 - Supports controllers by using the left-stick button (configurable) for favoriting.
 - Supports [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) for in-game config editing.
 - Supports [Bigger Backpack](https://www.nexusmods.com/stardewvalley/mods/1845) and other inventory expansion mods (as of version 1.1.0).
