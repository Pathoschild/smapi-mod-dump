# Stardew Valley Farm Automation Mods

#### Quick Links:
[Download](https://github.com/oranisagu/SDV-FarmAutomation/releases/latest)
[F.A.Q](#FAQ)
[Setup](#Setup)
[Item Collector](#ItemCollector)
[Automated Barn Doors](#BarnDoorAutomation)
[Feedback](#Feedback)
[History](#History)

## Introduction
The purpose of these mods is to save you time doing tedious task without being completely unfair/cheating. Whether something is an unfair advantage or not is of course subjective, which is why you can disable various features. The goal is to make the game more enjoyable for you by removing aspects you find tedious or unfun.

### Disclaimer
These Mods are still in early development and might have bugs or unintended behaviour. Make sure you backup your safefiles before using them!
If you have problems, please give feedback over at the [Forums](http://community.playstarbound.com/threads/alpha-farmautomation-mods-for-smapi-0-39-2.111931)

### Requirements
You'll need SMAPI v0.39.6 or higher for these mods to work, see their [GitHub Page](https://github.com/ClxS/SMAPI) for more information.

Always update both mods to their current version, even if nothing changed for one of the mods. The reason being they use a DLL with common functions and .NET only ever loads one. This unfortunately means, if you update one mod but not the other, things may lead to problems like this:
```
[ERROR] an error occured with the automation mod: System.TypeLoadException: Could not load type 'FarmAutomation.Common.SoundHelper' from assembly 'FarmAutomation.Common, Version=0.1.4.0, Culture=neutral, PublicKeyToken=null'.
```
I'm looking into finding a definitive solution for this issue, but for now, if you use both mods, always use the most current version of both, otherwise it might lead to issues.

### Download
You can download the compiled mods under [latest releases](https://github.com/oranisagu/SDV-FarmAutomation/releases/latest).

<a name="FAQ" />
### Frequently asked Questions
**Q:** Machine X does not take any items from a chest

**A:** the pathfinding to the next chest is complicated and most of the times people have accidentally connected multiple chests to the same machine and the mod uses another chest than the expected one. Try to keep your connected machines and their chest isolated from other machines. For example, create a keg farm with no floor tiles around them leading to the rest of the farm, just add one chest and test it again.
___
**Q:** How do I disable the collection from animals?

**A:** This part of the mod only works if there is a chest in the animals home. Don't place a chest inside their home, and the animals will not be collected from.
___
**Q:** How do I add other locations to look for machines?

**A:** The configuration file now contains a full list of locations. You can add any location you'd like, but be careful, if you place a machine or chest where a NPC walks through, the machine will get destroyed (this has nothing to do with this mod and is just the games normal behaviour)
___
**Q:** I'm using the ChestPooling mod and weird things happen.

**A:** These mods are unfortunately incompatible for the time being. 
___
**Q:** When will the mod be out of Beta? Why is it only at 0.1.X?

**A:** Once no more bugs are being reported, I will release the current version as 1.0. In parallel, I'm working on Version 2, which will have a few new features and better configurability (and which will then stay in beta for a while as well until no more bugs get reported)
___
**Q:** I've updated a mod but now it doesn't work at all anymore

**A:** If you use both mods but only update one to the newest version, it's very likely the new one won't work anymore. I'm looking into the issue, but for now, always update both mods if you use both. If you only use one mod, this shouldn't be a problem.
___
**Q:** The mod doesn't work.

**A:** Explain your situation with as much detail as possible (just saying it doesn't do anything is really not helpful) either in the issues here on GitHub or on the [Forum Thread](http://community.playstarbound.com/threads/beta-farmautomation-mods-for-smapi-0-39-6.111931). What did you try, what did you expect? Attach Screenshots or your safefile.

## The Mods
The following mods are currently part of this project:

<a name="ItemCollector" />
### ItemCollector
This is the main mod of this project. it's purpose is to collect items from your barns, coops and machines (in various locations) and put them in nearby chests. If the chest contains appropriate raw materials (for example for Kegs it would be Fruit or Wheat or Hops), it will also immediately refill the machine with the first matching item in the chest. if, for example you have a keg farm somewhere, just put enough fruit into the chest and it should automatically create wine for you. This alleviates the tiresome running around refilling your machines.

*So how does it find the chests?* Simply put: if a machine touches a chest, it will use it. If not, but that machine touches another machine and that one touches a chest, it will use it. As long as there's another machine connected will keep looking. You can create as large rows (or fields, it works in all 4 directions) of machines as you want. In addition, it's now possible to use pathways like wood floor or cobblestone paths as well by adding the appropriate number into your configuration file (the default configuration file contains a list of the various paths and their numbers and only enables the wood floor by default).

You will want to shut off flooring tiles until you've setup your farm appropriately - otherwise it's possible everything connects to everything and uses the first available chest (don't worry if it's full, it won't deposit the items then, but it might take items for further processing you didn't want to process).

#### Installation
the installation is simple, download the zipfile and extract it into your mods folder (either %AppData%\StardewValley\Mods\ or the games installation directory). check the configuration file and read it carefully first, to avoid unexpected behaviour.

#### Configuration (Important!)
read the configuration file first and disable any machines or floorings you might not want. if you're unsure, don't run the mod without making a backup until it's in a more stable state.
<a name="Setup" />
#### Setup
The easiest setup imaginable is just placing a chest near a machine:

![Basic Connection](https://raw.githubusercontent.com/oranisagu/SDV-FarmAutomation/master/Help/Basic%20Connection.gif)

As you can see, if you put materials into the chest, the machine next to it will use those materials and return the finished items, in this case a Copper Bar, back into the Chest.
The Machine will continue to work as long as there are materials in the chest.

But usually you don't just have one machine. You can place as many machines and connect them to one of your chests. Even different machines.
The following setup for example shows how you can setup multiple machines and connect them with a Wood Path to your chest.

![Simple Connection](https://raw.githubusercontent.com/oranisagu/SDV-FarmAutomation/master/Help/Simple%20connection%20setup.png)

In action:

![Simple Connection in Action](https://raw.githubusercontent.com/oranisagu/SDV-FarmAutomation/master/Help/Floor%20Connection.gif)

<a name="BarnDoorAutomation" />
### BarnDoorAutomation
This mod simply opens and closes the barn doors at configurable times. there is already another mod doing this so unless you have problems with that one, this probably won't help you much.

#### Installation
the installation is simple, download the zipfile and extract it into your mods folder (either %AppData%\StardewValley\Mods\ or the games installation directory) and start SMAPI. There are some configuration options, though you can probably leave those at their defaults.


<a name="Feedback" />
## Feedback
Feedback is very appreciated as there are so many situations I can't account for. If you have problems, consider sending me your savefile so I can look into it.

There's also my [Forum Post](http://community.playstarbound.com/threads/alpha-farmautomation-mods-for-smapi-0-39-2.111931), where you can leave suggestions or bug reports.

<a name="History" />
## Version History
#### [0.2.4](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.2.4)
* Custom Farm extensions are now supported for collecting from animals as well as machines inside buildings on those new locations (outside already worked)

#### [0.2.3](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.2.3)
* The Seedmaker now supports all crops in the games crop directory (mainly a fix for gemberries, but it should also fix newly added crops as long as they've been put into the Data\Crops file)
* If an invalid location name is added to 'LocationsToSearch', the game will no longer crash and instead log an error.
* After loading a game the mod will output a full list of all known locations, so the exact names of custom locations can be easier found. 

#### [0.2.2](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.2.2)
* Crab Pots got stuck after being connected (until at least one day passed without being connected)
* Crab Pots took bait when the player already had the Luremaster profession.

#### [0.2.1](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.2.1)
* Bugfix for compatibility with the Chest Label Mod.
* Added option to mute sound when processing machines. Read the comment in the config file carefully, as there are some downsides.
* Fixed Statue Of Perfection
* Added Slime Egg-Press to supported machines

#### [0.2.0](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.2.0)
* Added compatibility with the Chest Label Mod as chests were ignored if they had a custom label.
* Various improvements to the comments in the config file
* Added an option to allow diagonal connections.

#### [0.1.9](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.1.9)
* General Bugfix where NullReference Exceptions were logged.
* The above mentioned bugfix solves the problem where only one Crab Pot at a time would be collected from
* Added support for the Slime Hutch
* Flavoured Honey can be collected from Bee Houses (it will no longer turn into Wild Honey)
* Improved config file comments and included possible Locations

#### [0.1.8](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.1.8)
* Spelling of Rabbit's Foot should now be correct and pick it up automatically
* Charcoal Kilns used 10 (as of 1.0.7) but required the chest to have at least 20 logs. 
* Update to the ghost farmer to hopefully be a bit more stable and added a reset to it if something goes wrong.
* If a chest is full, the mod won't try to add any items from machines. 
* If an item still can't be placed into a chest, it will be added to the players inventory instead as a last resort.

#### [0.1.7](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.1.7)
only the item collector mod is affected.
* Bugfix: chests corrupted by 0.1.5 are fixed automatically without having to pickup/replace them manually.
* If a machine is ready for harvest but the connected chest is full, the mod won't try to take the item anymore. added log message for this case (like for animals)
* As a last resort, if the mod does take an item from a machine but can't put it into the chest, it will try to put it in the players inventory, to avoid losing items

#### [0.1.6](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.1.6)
* Updated logging to the SMAPI 0.39.6 variant.
* added Mushroom Box & Lightning Rod
* Bugfix where some items didn't get placed in the chests anymore.

#### [0.1.5](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.1.5)
* Added Statue of Perfection, Charcoal Kiln and Crab Pots to the supported machines
* Dinosaur Eggs should now be collected as well
* Added a Configuration option for muting animals in the morning. Mutes by default
* various Bugfixes with machines which would not be filled but produced after a set time (Tappers, Statues, Crab Pots etc). They should now work correctly (before they only started working again after picking them up or sleeping)

#### [0.1.4](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.1.4)
* Seedmaker and Furnace are now supported
* Barn animals which are outside their homes now get sheared/milked
* added lots of logging for easier debugging
* if a machine or chest was on a floortile (which was configured as a connector) the mod stopped working
* updated names for floortiles in the configuration comments

#### [0.1.3](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.1.3)
* items at the outer bounds of buildings lead to CTDs because the games internal method for determining if a tile is on the map simply doesn't work. had to rewrite it. this should hopefully fix a lot of the CTDs people had.
* added the 'Statue of Endless Fortune' to the default machines to collect from
* if animals leave their house before the first time update per day they were ignored due to no longer being in the building. now all animals, in buildings or on the farm are petted. if an animal sleeps outside, it won't be ignored either anymore.

#### [0.1.2](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.1.2)
* Collection of Void Eggs
* Opening/Closing times for Barn/Coop doors were inverted
* animals get petted even if no chest is present in the building

#### [0.1.1](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.1.1)
the same as 0.1.0-alpha, only updated assembly versions and added the version to the zip files.

#### [0.1.0-alpha](https://github.com/oranisagu/SDV-FarmAutomation/releases/tag/v0.1.0-alpha)
* floor tiles can now be used as connectors
* animals in coops are getting petted as well
* improved caching and performance
 
#### before GitHub
the first release was only posted on the community forums and offered basic functionality.
