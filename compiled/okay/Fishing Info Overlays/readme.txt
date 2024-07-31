This mod only displays information about the fish (and fishable items) available in your area, as well as a preview during the fishing minigame.
It does not alter any mechanics and has been coded to be as compatible with other fish-related mods as possible.


Installation:

    Download and Install the latest version of SMAPI. Made for 4.0.0+, earlier versions might not work.
    [Optional] Download and install Generic Mod Config Menu (GMCM). If you want an easy way to customise the overlays on the fly, with in-depth explanations.
    Download this mod from Files, and unzip the FishingInfoOverlays folder into your Mods folder.


Config (without GMCM, or manual):

From version 1.1.0, most of the variables are arrays of 4 - representing each split-screen screen, GMCM has a proper setup for this. Manual example:
"BarIconMode": [
    2,	- This is screen 1: Split-screen host, or any non split-screen player.
    0,	- This is screen 2: Split-screen (top) right. Non split-screen players can ignore this and the next 2 values.
    1,	- Screen 3.
    0	- Screen 4.
  ]

If you're using GMCM it has detailed ingame explanations of each position, otherwise run the game once to generate the config.json for this mod:

    BarSonarMode (0-3): Defines what doesn't function without Sonar Bobber: 0= Whole mod , 1= Minigame preview, 2= Shift Aim, 3= Nothing.
    BarIconMode (0-3): Direction the icons will travel, before eventually swapping row/column. 0= Horisontal Icons, 1= Vertical Icons, 2= Vertical Icons + Text, 3= Disabled (no location bar for rods/pots, just minigame preview).
    BarTopLeftLocationX (0-X): X coordinate of top left bar corner.
    BarTopLeftLocationY (0-X): Y coordinate of top left bar corner.
    BarScale (0.1-10): The bar already scales along with the UI, this allows you to fine-tune it.
    BarMaxIcons (4-500): Maximum amount of icons displayed at the same time.
    BarMaxIconsPerRow (4-500): Maximum amount of icons displayed before they change row/column.
    BarBackgroundMode (0-2): Dark transparent background(s). 0= Circles behind each icon, 1= Single rectangle behind all icons, 2= Off.
    BarBackgroundColorRGBA: Colour of background in Integer RGBA, so 0-255 x4. Google a colour picker, the alpha value is also 0-255 (not 0-1).
    BarTextColorRGBA: Colour of text in Integer RGBA, so 0-255 x4, like [255, 255, 255, 255].
    BarShowBaitAndTackleInfo: true= Displays bait and tackle icons + counts in the bar (if equipped). false= off.
    BarShowPercent: true= Displays percentage chance of catching a fish under its icon. Accuracy based on ExtraCheckFrequency, false= off.
    BarSortMode (0-2): 0= Icons are sorted by name. 1= Icons are sorted by catch chance percentage (heavily based on ExtraCheckFrequency, will likely jump), 2= Off (Trash, then IDs, then extras).
    BarExtraCheckFrequency (0-22): Generic (when 0) OR Simulated, runs the fishing minigame value*10 times per UI Update (higher is more accurate, but can impact performance). Generic is less accurate percentage-wise, but performs better.
    BarScanRadius (1-50): Scans X tiles around player for water, and only shows fish info for the nearest tile found (if any).
    BarLegendaryMode (0-2) (pre 1.0.5): If BarExtraCheckFrequency is above 0, this is ignored. 0= Vanilla Legendary Fish logic, 1= Vanilla + Always Show (cool if you have a mod that only makes them re-catchable), 2= Never Show (cool if you don't want to see them, or have a mod that alters them).
    BarCrabPotEnabled: true= Enables the above preview for Crab Pots. false= Off.
    UncaughtFishAreDark: Applies to both the above bar and the minigame. true= Uncaught fish will appear darker (like in collections tab) and will be named "???" if text mode enabled. false= Fish will always show full colour and their names.
    OnlyFish: Only show things from the official fish list (and trash), skipping things like furniture or notes. Keep in mind that there's a tiny chance that some mod didn't add its fish to this list, though those wouldn't really count as fish.
    MinigamePreviewMode (0-3): Fishing minigame style. 0= Full, copies 6 layers for best visuals. 1= Simple, copies 1 for best performance. 2= Only appears in the above bar (also part of mode 0 and 1). 3= Off, no minigame preview.