Using Generic Mod Config Menu is highly recommended for in-depth explanation of each position, otherwise run the game once to generate the config.json for this mod:

Config (without GMCM, or manual), you can save the config.json file and press F5 while in game, to update the changes in game without restarting/closing it.


format:
    ExampleValue: min-max (default) - per screen = split screen array

Most of the variables are arrays of 4 - representing each split-screen screen, GMCM has a proper setup for this. Manual example:
"ExampleValue": [
    2,	- This is screen 1: Split-screen host, or any non split-screen player.
    0,	- This is screen 2: Split-screen (top) right. Non split-screen players can ignore this and the next 2 values.
    1,	- Screen 3.
    0	- Screen 4.
  ]


Options for the alternative Fishing Minigames. Here you can pick your minigame style, or completely disable a minigame part.
The fish size and quality is determined by the combined score of Start and End minigames, depending on which ones are enabled.

    VoiceVolume: 0-100 (100)
	Allows fine-tuning of the "Here Fishy!" max volume. The actual volume increases a bit with fishing level, starting at 80% of this value.

    VoicePitch: -100-100 (random) - per screen
	Changes the pitch of your "Here Fishy!" voice. Starts randomized for multiplayer diversity. The original is right in the middle (0).

    KeyBinds: "" ("MouseLeft, Space, ControllerX") - per screen
	Button(s) used to trigger the minigame. If the same as Tool Use buttons, the Vanilla Fishing Minigame will be overwritten by this.
	Supports Mouse, Keyboard and Controller. 
	Individual keys are separated by commas, combos like \"LeftShift + K\" are NOT supported for Keybind minigames, but work otherwise.
	Example of three keys: MouseRight, LeftShift + K, ControllerX.
	For key names, see: https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings

    FreeAim: true/false (false) - per screen
	False: You can aim in front of you on a grid by holding your keybind, and control which tile with WASD, Arrow Keys, or DPad.
	True:  Allows you to simply click any tile you can reach (within your fishing level) with the cursor.
	Fishable tiles will be highlighted in both modes.
	Free Aim might not be very controller friendly, as the cursor position resets when accessing menus.

    StartMinigameStyle: 0-3 (1) - per screen
	This minigame determines whether you catch the fish at all (non-fish catches skip this part), and contributes to total score.
	0: Disabled - Start portion is skipped.
	1: The classic rhythm game where you match arrow directions with your Movement buttons as they reach a specific area. 
	2: WIP
	3: WIP

    EndMinigameStyle: 0-3 (2) - per screen
    	This minigame determines whether you take damage (if failed) when scooping up any fishable item/fish, and contributes to total score.
	0: Disabled - End portion is skipped.
	1: Minigame Keybind - At the right time, press the button you use for fishing, the keybind must be single button (no + combos).
	2: Matching Arrow   - At the right time, match the direction of the displayed arrow with WASD, Arrow Keys, or DPad.
	3: Matching Letter  - At the right time, press the displayed letter.
				Keyboard only, will use a letter from the fish/item name, if it can find any English ones - otherwise defaults to Arrows.

    EndLoseTreasureIfFailed: true/false (true) - per screen
	true: If you fail the End Minigame, you'll lose any potential treasure.
		This is mainly recommended if you don't have a Start Minigame enabled, as you'll otherwise receive treasure for nothing.
    
    EndMinigameDamage: 0.0-2.0 - (1.0) - per screen
	If you fail the end minigame, you'll get damaged based on the fish/item difficulty, size, and this multiplier.
	This is a straight-up multiplier, so 0.0 means no damage, less than 1.0 means less, more means more.

    MinigameDifficulty: 0.1-3.0 (1.0) - per screen
	Minigame difficulty (timers) is based on factors like your equipment, fish difficulty and size. This is the final value multiplier, the higher, the harder.

    TutorialSkip: true/false (true) - per screen
    	Setting to true allows you to skip the Start Minigame tutorial (slow mode). It's set automatically once you've completed a Start Minigame. You can also set this to false see it again.

    StartMinigameScale: 0.1-10.0 (1.0)
	The Start Minigame scales along with the zoom level, and can be fine-tuned here.

    ConvertToMetric: true/false (false)
	Only matters for English language, other languages apply this conversion automatically.
	WARNING: If using together with 'Inches To Metric System' Mod, delete all fish data from its 'content.json' (line 4-74), or they'll be double converted.
	WARNING: This will convert fish sizes as they're caught, and previous records won't be reversed to inches if you disable this.
	Does not affect Festival Minigames, or regular fishing minigame - as their mechanics are based on fish size. Will work with modded fish caught via this mod.
	If you read the above, are playing in English, and want metric, enable with: true

    RealisticSizes: true/false (true)
	true: The fish shown off at the end will have a realistic size VS player size, assuming the player holds them at a perspective that makes them a tiny bit bigger.
		Keep in mind that most of the time the fish are pretty tiny, around hand size.
	false: Vanilla sprite sizes.

    FishTankHoldSprites: true/false (true)
    	true: The fish shown off at the end will use the Fish Tank sprites, for fish that have them. These sprites tend to be more consistent than item sprites.
	However if you don't like them, or have a mod that animates the fish, you can switch to item sprites by setting this to false.

This only matters for Fall 16 and Winter 8 festivals, as they have fishing minigames that happen inside events.
Meaning that the animations can look a little glitchy, and full lenght minigames might be hard to balance. So here are the options for those.

    FestivalMode: 0-3 (3) - per screen
	0: Vanilla      - Use the regular fishing minigame for both festivals.
	1: Simple       - Only does the End Minigame, only fish count. A bit janky animations.
	2: Perfect Only - Only does the End Minigame (if enabled), only "perfect catch" fish count (better balance). A bit janky animations.
	3: Start Only:  - Plays the Start Minigame portion only, this one should be the most balanced in terms of points for effort.

    MinigameColor: An RGB value (don't change the A). GMCM has a color picker, you can google one. The letters might be in a different order.
	R: 0-255 (0)
	G: 0-255 (255)
	B: 0-255 (255)

    BossTransparency: true/false (true)
    	true: Boss fish in the Start Minigame will have semi-transparent arrows every couple arrows. You can set this to false if it's hard to follow.


    These are item specific effects for rods (nets), baits, and tackles (megaphones).
    A few items have hardcoded effects that aren't listed here, but are mentioned in the item's description.
    Setting an effect to 0 will disable it, you can also remove the effect line completely, but be careful with the structure.
    EXTRA, LIFE, QUALITY, TREASURE, and UNBREAKING are additive, the rest are multipliers.
    I recommend the "Everlasting Baits and Unbreakable Tackles" Mod instead of UNBREAKING for better immersion.

    The item names must be the original English names (item.Name, not the translated item.DisplayName).
    Effect names must also be in English, and all-caps.

	Effects:
    		"AREA": 	% multiplier	Expands the perfect area by {x}%
    		"DAMAGE": 	% multiplier	Damage taken reduced by {x}%
    		"DIFFICULTY": 	% multiplier	Minigame difficulty reduced by {x}%
    		"EXTRA_MAX":	n additive	Can attract up to {MAX} similar, additional fish, {CHANCE}% chance each.
    		"EXTRA_CHANCE":	% additive	^^
    		"LIFE": 	n additive	Might fix a misplay, up to {x} times per cast. Tackle (megaphone) only!
    		"QUALITY":	n additive	Attracts fish of {x} higher quality than normal.
    		"SIZE": 	% multiplier	Attracts {x}% larger fish.
    		"SPEED":	% multiplier	Decreases minigame speed by {x}%
    		"TREASURE":	% additive	Adds {x}% to treasure chance.
    		"UNBREAKING": 	%		{x}% chance of not being consumed. Bait/tackle only, specific to that item. Not needed if using "Everlasing Baits..." mod.

    SeeInfoForBelowData: {
	"Quality Bobber": {	= Original English (code) item name
		"SIZE": 20,	= 20% bigger fish size (so size * 1.2 internally)
		"QUALITY": 1	= +1 to quality
	}
    }