**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Achtuur/StardewTravelSkill**

----

# Hover Labels

This mod lets you hover over things and shows you a neat little label with some information about it.

![Example label showing tilled dirt](./docs/images//examplelabel.png)
# Features

Types of labels:

* Crops
* Trees
* Fruit Trees
* All types of nodes (Ore, Geode, Mineral etc.)
* Buildings:
  * Barn
  * Coop
  * Fish pond
  * Greenhouse
  * Junimo hut
  * Mill
  * Silo
  * Slime hutch
* Objects (any plaed object will have its name shown):
  * Chests
  * Casks
  * Machines (any placed object that is processing items, furnaces for example)
  * Scarecrows
  * Sprinklers

# Changelog

## Planned

### New/Changes to existing content

#### ambitious changes
* Add additional box to label for "show more" types of things.
* Add option for smaller borders (similar to ones you get when hovering over items in inventory)

#### extra labels
* (slime) incubator

#### small label changes

* disable label during events?
* labels with a duration that is longer than a day should show date
  

## 1.1.0
* New/Changed
  * Added label for all types of nodes
  * Greenhouse now switches between crop/trees when pressing shift
  * Sprinklers label text updated
  * text for which key to press now looks a bit nicer
  * New labels:
    * Added generic label for buildings that shows the name
    * Added label for fish ponds, showing number of fish and quests
    * Added label for silos, showing hay
    * Added label to Barn and Coop showing their animals
  	* Added label to slime hutch
    * Added label to mill
  * Updated labels:
    * Casks now show next quality AND iridium quality date

* Fixes
  * Fixed nullrefexception when hovering over Greenhouse
  * Chest label now shows correct number of items
  * Crop label text fixed (missing word added)
  * Fixed date string showing dates with a 0 in them
  * Fixed regrowable crops not properly showing as fully grown sometimes

## 1.0.0

* Initial release

