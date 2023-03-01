**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/urbanyeti/stardew-better-ranching**

----

# Stardew Valley Mod - Better Ranching
Stardew Valley mod that prevents failing milking/shearing attempts and adds an indicator when animals can be petted, milked, or sheared: https://www.nexusmods.com/stardewvalley/mods/859/

Compatible with Stardew Valley 1.5.5+ on Linux, Mac, and Windows. Requires SMAPI 3.13.0 or later.

**New Update 1.9.2 (2/1/23):** Oh wow! [Profittroll](https://github.com/profitrollgame) added a Ukranian translation for the config settings menu and prompts!

![Preview image](/better_ranching_preview.gif)

<h2>How to Install:</h2>
    Install the latest version of SMAPI.
    Unzip the mod folder into Stardew Valley/Mods.
    Run the game using SMAPI.


<h3>Versions:</h3>

* 1.0:
    * Initial version
    * Added speech bubbles to indicate which animals can be milked
    
* 1.1:
    * Added override for milk pail left-click to prevent failed attempt animation and sound effect
    * Added "Milking Failed" warning message
    * Fixed bug causing speech bubbles to cover up heart animation
* 1.2:
    * Added support for sheep shearing
    * Added Gamepad support
    * Fixed bug causing thought bubbles to arbitrarily disappear
* 1.3:
    * Fixed a bug that made it hard to click on the toolbar when the milk pail or shears were selected
* 1.4:
    * Added indicator for when animals can be petted
* 1.5:
    * Added indicator when pigs can be petted
    * Fixed bug causing error when milking in the FarmHouse
* 1.6:
    * Added config json file
    * Fixed bug causing player to fail valid milk attempts
* 1.7:
    * vaindil updated to work for the latest version of 1.3 and SMAPI 2.6-beta.15
* 1.7.1:
    * Updates the code for SMAPI 2.8 and Stardew Valley 1.3.31.
    * Adds update keys to the manifest.json and standardises the version format.
* 1.7.2:
    * Updated the code for SMAPI 3.0
* 1.7.3:
    * minervamaga made a fix for an error that was spamming the console
* 1.7.4:
    * Fixed a bug causing dogs/cats to keep their heart indicators even if the setting was turned off in the config
* 1.7.5:
    * Improve error handling
* 1.7.6:
    * floatingatoll fixed a bug causing bubbles to not be hidden during cutscenes
* 1.8.0:
    * Made compatible with SDV 1.5
    * Fixed UI / Zoom scaling issues
* 1.8.1:
    * Added API for better compatibility with other mods
    * Fixed bubbles showing up during cutscenes
    * Cleaned up / refactored code
* 1.9.0:
    * Add support for Generic Mod Config Menu
    * Clarify "Milking/Shearing Failed" messages
    * Improve reliability of harvest detection
    * Add config options to individually control hearts for farm animals and pets
    * Add config option to hide hearts for max level animal friends
    * Update to SMAPI 3.13.1 and .NET5
* 1.9.1:
    * Clean-up code styling
    * Added i18n support 
    * Added Ukranian translation for config menus and prompts
<h2>Contributors</h2>

* Urbanyeti -- https://github.com/urbanyeti
* vaindil -- https://github.com/vaindil
* Pathoschild -- https://github.com/Pathoschild
* minervamaga -- https://github.com/minervamaga
* floatingatoll -- https://github.com/floatingatoll
* corrin -- https://github.com/corrinr
* Profitroll -- https://github.com/profitrollgame
