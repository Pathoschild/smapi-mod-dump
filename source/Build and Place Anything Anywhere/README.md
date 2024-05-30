**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Smoked-Fish/AnythingAnywhere**

----

[![Build](https://github.com/Smoked-Fish/AnythingAnywhere/actions/workflows/build.yml/badge.svg)](https://github.com/Smoked-Fish/AnythingAnywhere/actions/workflows/build.yml)
# Anything Anywhere Config List


## Table of Contents
- [Placing](#placing)
- [Building](#building)
- [Farming](#farming)
- [Miscellaneous](#miscellaneous)
- [Debug Commands](#debug-commands)

## Placing

| Setting Name           | Description    |
|------------------------|----------------|
| Reset Page Button      | This button, located at the top of every page, resets the configuration for only that page. |
| Enable Placing         | Toggle to enable the placement portion of the mod. Allows playing objects (chests, mini-fridges, fences) and furniture on any free tile. Enables infinite reach when placing furniture in all locations. |
| Place Anywhere         | Toggle to enable placing objects and furniture inside walls or any other locked tile, with exceptions: cannot place objects on top of another object or inside the mines. |
| Rug Removal Bypass     | Toggle to disable the check ensuring rugs don't have anything on top of them when picked up. Intended for use if unable to pick up a rug, disabling the checks. |
| Wall Furniture Indoors | Toggle to enable placing wall furniture anywhere indoors. By default off for easier indoor decorating, but can be enabled for unconventional placements like a window on your floor. |


## Building

| Setting Name           | Description    |
|------------------------|----------------|
| Enable Building          | Toggle to enable all the building and menu features of the mod. |
| Build Anywhere           | Toggle to make all tiles buildable, allowing building in walls or having buildings intersect each other. |
| Build Free & Instantly   | Toggle to set the costs of all blueprints to be free and make buildings build instantly. |
| Build Anywhere Menu      | Opens up the carpenter menu anywhere, with the viewport centered on the player. |
| Wizard Build Menu        | Opens up the Wizard's build menu anywhere, with the viewport centered on the player. |
| Building Modifier Key    | Holding down this key while building, upgrading, or destroying buildings prevents being kicked out of the menu, enabling fast building or destruction of multiple buildings. |
| Enable Greenhouse        | Toggle to add the Greenhouse as a blueprint that can be built. Requires unlocking the Greenhouse in the Community Center or Joja path. Blueprint costs: 150,000 Gold, 100 Hardwood, 20 Refined Quartz, 10 Iridium Bars. |
| Remove Build Conditions  | Toggle to remove build conditions on all blueprints, allowing building things like the Island Obelisk without visiting the island. Compatible with mods adding buildings locked behind certain events. |
| Build Indoors            | Toggle to enable building structures indoors. Note: Can lead to errors, such as soft locking the game when entering a building built inside the coop. Works for buildings inside the farmhouse and greenhouse. If using the Farmhouse Fixes mod, ensure enabling "Non-Hardcoded Warps". |
| Magic Ink Bypass         | Toggle to skip the check for magic ink when opening the Wizard's build menu. Only works for this mod's Wizard's build menu; magic ink is still required to open the Wizard's menu at the tower. |
| Hide Location            | Button to add the current player location to a list to not display in Robin's build menu. Sets map properties "AlwaysActive" and "CanBuildHere" to true for the current location. To remove non-blacklisted locations with no buildings from Robin's menu, reload your save. |


## Farming
| Setting Name           | Description    |
|------------------------|----------------|
| Enable Farming     | Toggle to mark all locations as plantable and make most dirt tiles hoeable. Enables farming in all locations with dirt. Note: Some dirt tiles added in 1.6 aren't labeled as dirt and cannot be hoed unless "Hoe Anything" is enabled. |
| Hoe Anything       | Toggle to allow the player to hoe any tile, making farming on stone or other non-dirt tiles possible. |
| Fruit Tree Tweaks  | Toggle to remove placement and growth restrictions on fruit trees, allowing planting them as close together as desired without growth being blocked by other trees or walls. |
| Wild Tree Tweaks   | Toggle to remove placement and growth restrictions on wild trees, such as acorn, pine, maple, and mahogany seeds. Enabling this may cause wild trees to quickly grow out of control, leading to forest takeover. |


## Miscellaneous

| Setting Name           | Description    |
|------------------------|----------------|
| Animal Relocation       | Toggle to enable the animal relocation menu, allowing animals to be relocated to any location with a building for them. If disabled, animals can only be relocated to the main farm. |
| Cask Tweaks             | Toggle to allow the player to use the cask outside of the cellar. Disabled by default due to potential perception as 'cheating' by some players. |
| Jukebox Tweaks          | Toggle to enable using the jukebox in all locations it can be placed. Note: The jukebox does not work outside while it is raining. |
| Gold Clock Tweaks       | Toggle to make the gold clock building work in any location, rather than just the farm. |
| Multiple Mini-Obelisk   | Toggle to allow placing more than two mini-obelisks per location. Having more than two per location may cause them not to work as intended but can be used as decoration. If using the Multiple Mini-Obelisk mod, this toggle will be enabled by default; otherwise, it is disabled by default. |



## Debug Commands



| Command              | Description                                                            | Parameters                  |
|----------------------|------------------------------------------------------------------------|-----------------------------|
| `aa_remove_objects`  | Removes all objects with a specified ID at a specified location.       | `[LOCATION] [OBJECT_ID]`    |
| `aa_remove_furniture`| Removes all furniture with a specified ID at a specified location.     | `[LOCATION] [FURNITURE_ID]` |
| `aa_active`          | Prints a list of all the locations that are set to AlwaysActive.       | None                        |


