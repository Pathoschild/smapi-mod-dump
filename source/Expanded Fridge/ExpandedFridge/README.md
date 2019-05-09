This is a replacement project for my old Stardew Valley SMAPI mod Expanded Fridge. I created the new structure in a short time while I was experimenting with the SMAPI functionality and came up with this implementation. My goal was to create a better UI for managing large amounts of items in the fridge while also eliminating the risk of players losing their items if the mod ever breaks in the future. The system is not perfect and could be improved, but we'll see if I have the time for that. If anyone wants to use this code as a base for developing a better mod that expands the fridge they are free to do so.

## Note of Warning
As of today (2019-04-28) I have not had the time to test this mod intensively. I have tested the basic functionality but not under many different conditions. If you are using this mod be aware that there might be some bugs. I will try to fix at least more critical reported bugs if I have the time. Multiplayer is completely untested but could work. I don't know the netcode for Stardew Valley very much but I have tried to adjust for multiplayer in my implementation.

## Disclosure 2019
I originally created this mod about two years ago. It was just a fun side project and I didn't expect there to be many people that would like to use it. Coming back and realizing that there were comments asking for it to be updated surprised me to say the least. Originally I didnt think I would have time to remake it since I lost the source code and had to come up with a new way to modify the fridge. Also it would be boring to just try and make the same implementation again. But then I got curious and dove into SMAPI and two days later I had an idea for how I could do it differently. Another weekend and I had an untested new version for Expanded Fridge ready to be uploaded.

So here we are with an update for the mod. I really didn't think I would do it because of my final exam but I guess I was just really inexperienced back at the time (even though I still think I'm writing crap code today).

I'll try to fix bugs that are reported but I can't promise anything. The source code for this mod is on git so if I never return again anyone who wants can take a look and see if they want to make a new mod based on this.

## Expanding the Fridge
Simply increases the fridge inventory with additional inventory tabs. The fridge can be upgraded by using the fridge from the right side.

## Notes:
* If you want to unlock max storage and all upgrades, look under the cheat section.
* You can have a maximum number of 12 inventory tabs.
* Compatible with Stardew Valley 1.3
* Multiplayer might not work but all players will need to have the mod installed.
* Should work with all mods that dosnt modify the fridge or accesses the fridge inventory.
* If you can't unzip the file you might need to install winrar.

## Requirements:
* SMAPI 2.2+
* Stardew Valley 1.3+

## In-Game Instructions:
* Simply open the fridge and access it normally.
* Click on the tabs to switch between inventories.
* Open the fridge from the right side to access the upgrade system.
* Use fridge button in your inventory menu to access fridge remotely (upgrade).

## Install Instructions:
* Download and extract the "Expanded Fridge Files" zip file to a new folder.
* Copy the "ExpandedFridge" folder from inside the new folder into your Stardew Valley mods folder.
* Done!
Note: If you leave items in the fridge before installing the mod you can't access them while the mod is installed. Any items left in the fridge will be there when the mod is uninstalled.

## Uninstall Instructions:
* Go to your Stardew Valley mods folder.
* Delete the "ExpandedFridge" folder.
* Done! 
Note: All inventory tabs inside the fridge will be moved into new chests placed inside your house. I recommend you destroy them after receiving the items inside them.

## Cheats:
If you would like to increase the base storage of the fridge or start with all upgrades you can edit the config.json file inside the "ExpandedFridge" folder.

## Known Bugs:
* Multiplayer
I have no idea if this works in multiplayer. I know that some things will break or possibly crash the game unless all players have the mod installed.

* Stack Merging
If you put the same type of item inside different tabs they will stack together into one of them when the fridge is closed. This will probably never be fixed since it is required from how we connect all the items to cooking.

* Gamepad Remote Access
The fridge cannot be remotely accessed with a gamepad since there is no shortcut and no way to get to the remote fridge button without moving the mouse.

* Gamepad Tab Switching
It is not possible to switch between tabs in the fridge using a gamepad.

## Change log:
2.0 - Complete rework of entire mod.
1.2.8 - Changed settings for choosing fridge version to SMAPI friendly.
1.2.7 - Updated SMAPI code and added shift + right-click to transfer half stack.
1.2.6 - Fixed bug on mac with new settings file not being read.
1.2.5 - Escape key can be used to close the fridge and new system for choosing fridge version.
1.2.4 - Extra, Added Extra Large Fridge version.
1.2.4 - Added simple support for navigating the menu with a controller.
1.2.3 - Added an option for a scrolling menu.
1.2.2 - Fixed bug where fridge would not work if it was empty.
1.2.1 - Clicking and opening/closing now has sounds.
1.2 - Reworked menu manipulation. Should be more stable and work better with Chests Anywhere.
1.1 - Right clicking now works.
