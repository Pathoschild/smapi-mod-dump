**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Prism-99/DailyTaskReportPlus**

----

# DailyTaskReportPlus
A Stardew Valley SMAPI mod.

Added:
    SDV 1.6 compatibility
    
    Re-wrote the code to search all locations and not just the Farm for Crops, Animals and Machines
    Added checking ponds for wanted items or items to collect
    Added warning for end-of-the-month crop harvesting
    
Changes:

    The default open report key is now 'P'. (The 'Y' key hides the emote icons)
    The default bubble toggle key is now '*'
    The default open settings menu is now '-'

Settings

    The mod settings are done by a mod provided menu.  The default  key to open the settings is '-'.  
    I am looking at how to revamp the settings menus to accommodate dynamic Crops and Machines.

Press P to open your report or click the button below the quests button. It shows (with detailed information about each item):

    If The Queen of Sauce has a new recipe for you
    If it's someone's birthday
    If the Travelling merchant is in town
    Unwatered crops (Anywhere)
    Crops ready to harvest (Anywhere)
    Dead crops
    If you did not pet your pet
    If you did not fill your pet's bowl
    Animals that were not petted
    Animals products ready to collect
    Empty hay spots on feedings benches
    If there are fruits/mushrooms to be collected in your farm cave.
    Crab pots ready to be collected
    Crab pots that were not baited
    Machines ready to collect (full list of machines in the configuration section)
    Ponds that have items to collect
    Ponds that have requests
    End of the month warning to harvest flowers and crops


The mod can draw bubbles for (there is a configurable keybinding to turn them on/off without entering any menu):

    Unwatered crops
    Crops ready to harvest
    Dead crops
    Pet wants to be petted
    Animals that were not petted
    Animals that have produce
    If there is animal produce inside a building
    If there are hay spots missing in the animal house
    Truffles
    Crab pots that were not baited
    Casks with a minimum item quality (set in the configuration)


Included Languages

    English
    German - Deutsch (Thanks Th3Tob1)


Future Releases

Currently the animal product types and the machine types are hard coded into the mod and only check for items that are part of the non-modded game.  Future releases will add the ability to check for any machine and any type of animal product based upon the mods installed.
