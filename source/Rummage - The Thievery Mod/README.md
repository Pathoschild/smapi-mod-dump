**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ApryllForever/Rummage**

----

# Rummage
Stardew Valley Rummage

This mod allows the farmer to rummage around and find items! Get things from kitchens! Loot rooms! Be careful, if people catch you stealing, they will be angry! This mod is built off of my Rest Stop Trash Can Code, which is largely the Ridgeside Garbage Can code, Thanks Shekurika!!!

For Players:

This mod adds tiles to places throughout the game where you can right click and get things, when you see the action white glove hand cursor. There are places in the stores, in houses, and such. Look around; every major character has one in their rooms, except: 

The Witch (the wizard in vanilla... but I like trans Rasmodia better!!!), because she would smite you!!!

Linus: Only a complete sleaze would steal from Linus.

Jas and Vinni: Do not steal from the children!!! That is terrible!!!

Granny and Grampa: You can steal from their kitchen and living room, but their room? That's just too far!!!

Leo: He does not have anything to steal.



Like the garbage cans, people will become angry if you are within seven tiles!!! You will lose a lot of friendship. However, you might get food, cool items, or perhaps all you can find is garbage!!! 

For Modders:

This mod allows you to add rummage tiles to your maps, where you can allow the farmer to get any vanilla object from the object list which You put the ID for (I don't know how to get weapons or other things yet). To add the tile to a custom map, on the Objects Buildings Layer, add a Object named TileData, with the custom property of Action RS.Rummage XX (XX being whatever number you assign to it. Must be an integer), for example, RS.Rummage 33. There must be something in the buildings layer on that tile for the action to work. To define the number (with any map), set it in [CP] Rummage/Data/Extras/RummageData.JSON. I have a whole lot of different items with their ID numbers listed in the file, hopefully it will prove helpful to you to be able to add tiles you would like or edit what is already found! 

After you add the tile information, add the EditMap to your mod's ContentPatcher file. This is where you will say what maps and tiles are affected for vanilla maps, In the future, I would like to make this compatable with JSON Assets and Dynamic Game Assets (This is simply the first form of my release of the mod. I wanted to gain experience releasing mods so that I might be prepared to release a major mod I am making called Rest Stop). You also will need to add EditData to ExtraDialogue, as shown below (you can make the people say whatever you want. The $a means Angry Face, the $h means happy face. The kids think it's funny that you are rummaging aroud, but you still lose friendship, because they know to not trust your thieving butt!)

Examples: 

For Vanilla Maps: (or adding to any map instead of editing the TMX/TBIN file)


{

{
	"Action": "EditMap",
	
	"Target": "Maps/SeedShop", //////Abigail's Dresser
	
	"Update": "OnLocationChange",
	
	"MapTiles": [
	
		{
		
        "Position": { "X": 12, "Y": 4 },
	
        "Layer": "Buildings",
	
        "SetProperties": {
	
        "Action": "RS.Rummage 30",
	
      }
      
    }
    
  ]
  
},

{

	"Action": "EditData",
	
	"Target": "Data/ExtraDialogue",
	
	"Entries": {
	
		"RummageComment_Child": "Why are you digging around in there???$h",
		
		"RummageComment_Teen": "Get the fuck out of that!!!$a",
		
		"RummageComment_Adult": "Hey!!! Get out of that!!! Stop theiving around!!!$a",
		
		},
	},

}


The RummageData.JSON uses this format: "weight ItemID NumberofItems/weight ItemID NumberofItems/weight ItemID NumberofItems/...",

//////Abigail
	"30" : "4 879 1/4 431 3/4 241 1/4 93 2/4 92 2/4 874 3/4 288 1/4 287 2/4 286 4/4 253 2/4 403 2/4 311 2/2 432 1/4 272 1/4 766 3/4 349 1/1 534 1/",  ////Monster Musk, Sunflower Seeds, etc...

This way, you can use this to add rummageable tiles anywhere there is a tile in the buildings layer!

So, to use this for your mod you are releasing, you will add the information to your content patcher and to the RummageData.JSON file, and use the mod files with your release. You will also need to edit the source code to change the tile action from RS.Rummage to YourModName.Rummage, to avoid incompatibility with any other mod.. Please credit me if you are releasing it using my files. Cheers!
  
  
  
  
  
