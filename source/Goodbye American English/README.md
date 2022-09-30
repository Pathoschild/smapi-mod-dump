**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TheMightyAmondee/GoodbyeAmericanEnglish**

----

# Goodbye American English #

Changes American spelling to British spelling, this includes changing the season fall to autumn.

Also changes the game units to the metric system

I've tried to be as thorough as I can but I may have missed some changes, let me know if you spot anything and I'll fix it up.

## To install: ##
- Ensure SMAPI is installed
- Simply unzip the download file into your Mods folder

### NameReplacer ###
Version 1.4.0 introduced the ability to change the display names of any concession snack or object using the included NameReplacer.json file, think of this as an internal content pack. When entering name replacements make sure to put a comma at the end of each entry (except the last one). 

You can safely delete this file if you don't want to change any names. If you want to replace names but have deleted the file, simply create a new JSON file with the name "NameReplacer"

In Version 1.4.1, the NameReplacer.json will generate once the mod is run. Just ignore the file if you don't want to use it.

In Version 1.5.0, the NameReplacer has become more advanced, thanks to Harmony! Now preserve and honey names can be independently or generically edited. These edits are discussed in the Advanced NameReplacer section. 

To turn Harmony patching off, set AllowAdvancedNameReplacer to false in the config. This stops some replacements from occurring but can prevent issues if the game is experiencing problems.

The format has also been updated for concessions to fix some recognition issues. For concessions the form is now "SnackID_C":"Name/NameToReplaceWith"

Input name replacements for objects are in the form "ObjectID":"O/Name/NameToReplaceWith" (first field is a capital O) e.g "272":"O/Eggplant/Aubergine". See https://stardewcommunitywiki.com/Modding:Object_data for object IDs

In 1.4.0 and 1.4.1 name replacements for concession snacks are in the form "SnackID":"C/Name/NameToReplaceWith" (first field is a capital C) e.g "0":"C/Cotton Candy/Fairy Floss"

In 1.5.0 name replacements for concession snacks are in the form "SnackID_C":"Name/NameToReplaceWith" e.g "0_C":"Cotton Candy/Fairy Floss"

The table below shows the SnackID for each concession:
Concession | SnackID
-----------|--------
Cotton Candy | 0
Jasmine Tea | 1
Joja Cola | 2
Sour Slimes | 3
Personal Pizza | 4
Nachos | 5
Salmon Burger | 6
Ice Cream Sandwich | 7
Popcorn | 8
Fries | 9
Chocolate Popcorn | 10
Black Licorice | 11
Star Cookie | 12
Jawbreaker | 13
Salted Peanuts | 14
Hummus Snack Pack | 15
Kale Smoothie | 16
Apple Slices | 17
Panzanella Salad | 18
Truffle Popcorn | 19
Cappuccino Mousse Cake | 20
Joja Corn | 21
Stardrop Sorbet | 22
Rock Candy | 23

## Advanced NameReplacer ##

Editing preserve names works a little differently.

#### Generic edits ####

Universal edits to all preserve names i.e all Jelly becomes Jam are in the form "PreserveType":"PP/EditType/NameToReplaceWith".

To break it down:

The PreserveType is the preserve to change. Either: "Juice", "Wine", "Pickles", "Jelly", "Roe", "Wild Honey" or "Honey". Edits for "Wild Honey" replaces the entire object name.

The EditType can be one of "prefix" or "suffix". Basically, put the preserve word before "prefix" or after "suffix" the object name. Either will replace the entire name for "Wild Honey".

E.g "Jelly": "PP/suffix/Jam"

#### Independent edits ####

Unique preserve edits can be done when AllowAdvancedNameReplacer is true (which is the default) in the config. 

Unique edits are in the form "ObjectID_PreserveType":"P/EditType/NameToReplaceWith".

To break it down:
The ObjectID refers to the object ID of the item to replace the preserve name for. i.e Potato's ID to change Potato Juice to something else.

The PreserveType refers to the preserve to change. Either: "Juice", "Wine", "Pickles", "Jelly", "Roe" or "Honey".

The EditType can be one of "prefix", "suffix" or "replace". Basically, put the preserve word before "prefix" or after "suffix" the object name. "replace" will replace the entire name with something else. For "replace" using {0} will insert the object name in it's place.

E.g "192_Juice": "P/replace/Vodka" or "376_Honey": "P/replace/Wild {0} Nectar" or "266_Pickles": "P/suffix/Sauerkraut"

Generic  edits are mutually exclusive (can only prefix or suffix, not both). Unique edits will override generic edits.



