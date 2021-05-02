Adding new NPCs:

1. Add an entry to config.json
2. Add at least one entry to locations.json
3. Add 2 entries to items.json
4. Add 4 pngs to the assets folder

config.json:

"Aspen": {
	"DisplayName": "Aspen",                 // set to the same as the NPC name key
	"Enabled": 1,
	"IsFemale": 1,
	"CrossDress": 0,
	"HomeSpots": 1,
	"BathSpots": 1,
	"OtherSpots": 1
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
 
items.json:

{
    "NPC": "Aspen",
    "Variant": 1,
    "Description": "%Pronoun%'s looking for these.",
    "Price": 41
},

pronoun inserts for items.json:

%pronoun%: she/he
%obj_pronoun%: her/him
%pos_pronoun%: her/his
%Pronoun%: She/He
%Obj_pronoun%: Her/Him
%Pos_pronoun%: Her/His


Assets folder naming:

px[short name]f1.png
px[short name]f2.png
px[short name]m1.png
px[short name]m2.png

If you want to maintain support for gender-swaps and cross-dressing you need to provide all 4 assets.

Short name:

* your NPC name
* in lower case
* with the letters "aeiouy" removed
* and with any duplicate letters removed

e.g.
Abigail -> bgl -> pxbglf1.png
Clint -> clnt -> pxclntm1.png
Demetrius -> dmtrs -> pxdmtrsm1.png

If you can't figure it out, look in the SMAPI log for what image asset the mod is trying to load
