Using Generic Mod Config Menu is highly recommended for in-depth explanation of each position, otherwise run the game once to generate the config.json for this mod:

Config (without GMCM, or manual), you can save the config.json file and press F5 while in game, to update the changes in game without restarting/closing it.

I recommend enabling Sprite Preview mode here and playing the game in windowed mode - so you can edit settings here and see the previews after pressing F5 in game.

format:
    ExampleValue: min-max (default)

This mod allows you to offset your spouse/roommate's generic starting positions, as well as spruce them up with animations. The spouse room has two modes: Auto and Manual, everything else is Manual.
Auto will pick a random tile in the room, and turn the spouse towards the Facing Tile. By default only Spouse Room (auto) is enabled. Hover over variable names for more info.

    SpritePreviewName: text ("")
    	Will show previews of the specified Manual profiles in the world (and the automation's Facing Tile), allowing you to fine-tune spouse offsets.
		 Leave blank during regular gameplay to avoid performance impacts. Valid entries: "All" (for all profiles), or a list of profile names (like "Default, Sam").
		 If installed, Custom Spouse Patio will assign the correct spouse preview to the correct patio.
		 If installed, Multiple Spouses will show a preview for each room, however the preview won't be able to determine the correct room (will show one in each), but the rest of the mod will. 

Spouse Room (Auto): 
    SpouseRoom_Auto_Blacklist: text ("")
		If you wish to exclude a specific spouse profile from automation. Valid entries: "All" (same as setting Chance to 0), or a list of profile names (like "Default, Sam"). 
    
	SpouseRoom_Auto_Chance: 0-100 (100)
		The chance of automation being picked instead of a manual profile. 100% for only Automation, 0% for only Manual, something in between for a mix. 
    
	SpouseRoom_Auto_PerformanceMode: true/false (false)
		When enabled the automation will only check if the room tile is empty, instead of checking if it can be navigated to.
		 The navigation check is quite expensive, though without it there's a tiny chance the spouse might end up behind some furniture (if there's a free tile there). 
    
	SpouseRoom_Auto_MapChairs_DownOnly_Blacklist: text ("") 
		Spouses not on this list can try to sit on any (player placed) sittable furniture facing up (North), due to lack of sitting spites it's recommended to use chairs/sofas with non-see-through backs.
		 Larger furniture might be destroyed without the 'Non Desctructive NPCs' mod.
		This blacklist will disable this behaviour for the specific profiles. Valid entries: All, or a list of profile names (like Default, Sam). 
		 
    SpouseRoom_Auto_FurnitureChairs_UpOnly_Blacklist: text ("")
		Spouses not on this list can try to sit on any map chairs (ones the player can sit on) facing down (South), due to lack of sitting spites it can look a bit silly in some cases.
		 If time doesn't change from 8:20am to 8:30am naturally (time skip), the spouse can get stuck by the chair for the rest of the day.
		This blacklist will disable this behaviour for the specific profiles. Valid entries: All, or a list of profile names (like Default, Sam). 

    SpouseRoom_Auto_Facing_TileOffset: X, Y offset ("-1.1, 0")
		This is the 'interesting' tile used by Automation to determine where the spouse will be looking towards upon spawning, when not sitting. By default it's offset to 1.1 tile left of their spawn point.
		"Default" is the profile is used by all spouses without a named profile. It uses your main spouse (or Emily) for preview.
		 The "sebastianFrog" profile is a special hardcoded case - ignore it IF you have Multiple Spouses installed.
		Sebastian's spawn point changes a bit after a certain event - the interesting tile in his room is then around the middle. 



Manual Mode - same instructions for all of below, see further down (with extra for patio).
Manual offsets do not do any checks to prevent them from spawning inside a wall, on top of furniture, or on top of another NPC, it's all up to you to decide.
Keep in mind that making them spawn somewhere with 0 empty adjacent tiles might cause issues. 

"Default" is the profile is used by all spouses without a named profile. It uses your main spouse (or Emily) for preview. If a profile has multiple entries (example below), it picks one at random each day.

"Abigail": [ //this profile has 2 Key-Value entries (2 tiles + sprites) to pick from
  {
	"Key": "Down",		//facing down, 5.1 tile above default loc
	"Value": "0, -5.1"
  },
  {
	"Key": "40",		//sprite 40, default loc
	"Value": "0, 0"
  }
],


    SpouseRoom_Manual_TileOffsets: Key = Sprite (or Sequence), Value = X, Y Offset ("Key": "Up",   "Value": "0, 0")
	
		This is triggered when the spouse spawns on the standard spouse room tile. Manual tile offsets are not limited to their room, and allow you to offset the location to anywhere in the house. 
		The "sebastianFrog" profile is a special hardcoded case - ignore it IF you have Multiple Spouses installed. Sebastian's spawn point changes a bit after a certain event.
			
	
    Kitchen_Manual_TileOffsets:    Key = Sprite (or Sequence), Value = X, Y Offset ("Key": "Down", "Value": "0, 0")
	
		This is triggered when the spouse spawns on the standard tile in the kitchen. Manual tile offsets are not limited to the kitchen, and allow you to offset the location to anywhere in the house. 
	
	
    Patio_Manual_TileOffsets:      Key =            Sequence , Value = X, Y Offset ("Key": "Down", "Value": "0, 0")
		
		This is triggered when the spouse spawns within their patio area (outdoor spouse area). Manual tile offsets are not limited to the patio, and allow you to offset the location to anywhere on the farm.
		Important: You can't replace their Patio animation with a single sprite. You can replace it with a Sequence. If you only wish to change their offset (and not the animation), set the Sprite to Down. 
	
	
    Porch_Manual_TileOffsets:      Key = Sprite (or Sequence), Value = X, Y Offset ("Key": "Down", "Value": "0, 0")
	
		This is triggered when the spouse spawns on the porch (outside, by the door). Manual tile offsets are not limited to the porch, and allow you to offset the location to anywhere on the farm. 
 
"ProfileName": [
	{//entry 1
		"Key":		1 Sprite:	Up/Down/Left/Right, or specific sprite index (like 20). Patio only accepts Sequences - if you don't want to change the animation, use "Down".
	OR 	"Key":		Sequence:	Comma separated list of SpriteIndex:Seconds (:f after SpriteIndex flips the sprite).
		"Value":	Offset:		X, Y tile offset from their regular position, like: 0, 2.5 Entries with Value offset of -999,-999 are ignored.
	},
	{
	//entry2
	}
]

Three Entry examples (spaces between symbols are ignored):

	 "Key": "2", 
	 "Value": "1,1"								= sprite index 2, 1 tile to right and 1 lower than default spawn.
	 
	 "Key": "0:1, 4:1, 8:2.5, 12:0.5", 
	 "Value": "1, 1.5"							= sprite 0 (for 1 second) > 4 (1s) > 8 (0.5s) > 12 (0.5s), 1 tile to right and 1.5 lower than default spawn.
	 
	 "Key": "40:f:0.5, 41:0.5, 42:f:1", 
	 "Value": "0,0"								= sprite 40 (flipped, for 0.5 second) > 41 (0.5s) > 42 (flipped, for 1s), same place as default spawn.
	 