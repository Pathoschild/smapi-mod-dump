Font Settings:
	A mod to change Stardew Valley in-game fonts.


Installation:
	1. Install SMAPI (website: smapi.io)
	2. Put unzipped mod file into Mods folder.
	3. Done!


Compatibility:
	Lastest version works with Windows, MacOS, Linux.


Usage:
	To open Font Settings menu, choose one in following:
	1. Click the font button at left-bottom corner of the title menu. (the one with an uppercase 'A')
	2. HotKey. (default LeftAlt + F)
	
	In the menu, configure the fonts, then click OK, your font is set!
	Settings:
		General
			- Enabled: Whether to enable custom font. Check to enable, otherwise keep vanilla.
			- Font: Select the font you like. Those fonts are all from your computer.
			- Font Size: Configure the font size.

		Advanced
			- Spacing: Configure the horizontal spacing between two adjacent characters.
			- Line Spacing: Configure the vertical spacing between two adjacent lines.
			- X-offset: Configure the horizontal offset of each character.
			- Y-offset: Configure the vertical offset of each character.
			- Pixel Zoom (Dialogue Font only): Configure the factor by which to multiply the font size.

		Preset
			You can optionally store your configs as a preset, for convenivence. 
			To create a new preset, press Save current as...
			To edit a preset, switch to it and press save.
			To remove a preset, switch to it and press delete.

	About Font File:
		All the fonts available are from your computer. Supported types are TrueType (.ttf, .ttc), OpenType (.otf, .otc, .ttf, .ttc).

	About in-game Font Types:
		In game there're mainly three font types: Small, Medium, Dialogue fonts. You need to configure them seperately.

	About Latin Language:
		For those languages only contains latin characters, 
		they just use game's default font as Dialogue Font, which is hardcoded in a spritesheet, 
		so you might not set Dialogue Font for now.
		If you have a solution, let me know!


Configure:
	These are mod configs, not font configs above. All of these are supported in GenericModConfigMenu.

	- ExampleText               Text for font samples. Keep it empty and mod will use built-in text. Otherwise set your own.
    - OpenFontSettingsMenu      Keybind to open font menu, default LeftAlt + F.
    - DisableTextShadow         Miscellaneous option. Whether to close text shadow, default false.
	- MinFontSize				Min value of the font size option, default 5.
	- MaxFontSize				Max value of the font size option, default 75.
    - MinSpacing				Min value of the spacing option, default -10.
    - MaxSpacing				Max value of the spacing option, default 10.
    - MinLineSpacing			Min value of the line spacing option, default 5.
    - MaxLineSpacing			Max value of the line spacing option, default 75.
    - MinCharOffsetX			Min value of the x-offset option, default -10.
    - MaxCharOffsetX			Max value of the x-offset option, default 10.
    - MinCharOffsetY			Min value of the y-offset option, default -10.
    - MaxCharOffsetY			Max value of the y-offset option, default 10.
	- MinPixelZoom			    Min value of the pixel zoom option, default 0.5.
    - MaxPixelZoom			    Max value of the pixel zoom option, default 5.
    - FontSettingsInGameMenu    Legacy option, don't touch it.


FAQ:
	Q: How do I set dialogue font? There's no dialogue font tab in the menu.
	A: For those languages only contains latin characters, 
		they just use game's default font as Dialogue Font, which is hardcoded in a spritesheet, 
		so you might not set Dialogue Font for now.

	Q: I just want to set font size with vanilla font. But it seems not working when I select "Keep Original".
	A: You need to download additional files. Find vanilla font for your language in modpage optional files.


Help & Feedback:
	Where to feedback/askforhelp:
		1. At Nexus modpage POSTS: https://www.nexusmods.com/stardewvalley/mods/12467?tab=posts
		2. On Stardew Valley Official Discord: https://discord.gg/stardewvalley
			Ping me @Becks723#7620 anytime. I won't be always around but I'll check.

	Report a bug:
		1. At Nexus modpage BUGS: https://www.nexusmods.com/stardewvalley/mods/12467?tab=bugs
		2. On Github: https://github.com/Becks723/StardewMods/issues

	Thirst for feedback about this mod!


Release Notes:
0.7.3 - 2023-03-01
	- Fix dialogue font's characters incomplete/clipped render.
	- Fix in some case cannot click ok button.

0.7.2 - 2023-02-20
	- Adds a font button into title menu. You can find it at lb corner, appear as an uppercase 'A'.
    - Now supports custom language.
	- Bugfixes.

0.7.1 - 2023-02-14
	- Slim mod file. Move those vanilla font file (assets/fonts) to optional mod file.
	- Improve font change effects when you select "keep original". (all languages now support, except hu, ru, tr)
	- Bugfixes.

0.7.0 - 2023-02-06
- Improvements:
	- Display UI sliders' value.
	- Improve font change effects when you select "keep original". (current cjk)

- Bugfixes:
	- Fix a major bug where all fonts get lost after returning to title (or anyone invalidating it).
	- Fix a bug where everytime game launches, 1. ExampleText gets cleared, 2. PixelZoom is set to 1.0.
	- Add back the refresh button.

- Compatibility:
	- Drop 0.2 migration.

0.6.2 - 2023-01-05
- Fix a bug where Dialogue font failed to set when 'Enabled' is not checked.

0.6.1 - 2023-01-03
- Hotfix: In English no effect changing fonts.