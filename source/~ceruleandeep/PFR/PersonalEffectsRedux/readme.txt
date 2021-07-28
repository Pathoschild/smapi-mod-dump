/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

Adding new NPCs:

1. Add an entry to config.json
2. Add at least one entry to locations.json
3. Add 4 objects to the JA folder; copy the folder structure and object.json from one of the existing NPCs

config.json:

"Aspen": {
	"DisplayName": "Aspen",     // set to the same as the NPC name key. I haven't tested what happens if an NPC's internal name and display name differ
	"Enabled": 1,
	"IsFemale": 1,
	"CrossDress": 0,
	"HomeSpots": 1,             // do items spawn at the NPC's home?
	"BathSpots": 1,             // do items spawn at the bath?
	"OtherSpots": 1             // do items spawn in the mystery spots?
},

locations.json:

{
    "NPC": "Aspen",
    "Location": "FarmHouse",
    "LocationType": "Home",                 // allowed values: Home Bath Other
    "LocationGender": "Any",                // allowed values: Any Female Male
    "Description": "Test location",         // not used by the code, just to remind you of where the location is
    "X": 3,
    "Y": 6,
    "Rarity": "normal"                      // allowed values: normal rare very_rare always never
},

Assets folder naming:

Objects
    Aspen F 1
        object.json
        object.png
    Aspen F 2 
        object.json
        object.png
    Aspen M 1  
        object.json
        object.png
    Aspen M 2 
        object.json
        object.png

If you want to maintain support for gender-swaps and cross-dressing you need to provide all 4 assets.

object.json notes:

Your objects must be named like so (for F1, F2, M1, M2):
Aspen's Panties, Aspen's Delicates, Aspen's Underpants, Aspen's Underwear
Can you have Aspen's Bra? No! 
  