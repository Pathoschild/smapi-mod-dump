Context Sensitive Gift Cursor
by Vanguard3000

Installation
To install, simply unzip the "ContextSensitiveGiftCursor" folder into your Stardew Valley/Mods folder and run with SMAPI.

Configuration
The "config.json" file in the mod folder can be opened with a text editor, and contains the option to allow the disposition icons to show even with unrevealed gift tastes (i.e. gifts you haven't previously given to a Villager. In the sirit of the vanilla game, this is off by default, but to enable it, find the line
	"AlwaysShowGiftTaste": false
and change it to "true". Save the file, then run the app with SMAPI.
PLEASE NOTE: If you haven't met a Villager, you will have to introduce yourself before  these icons will show.

Changelog
v1.2.0 - Aug 5, 2020:
- Changed things so that gift tastes will show even if the person has been given max gifts. Previously, this would just show a red X. Now, it'll show a red slash over an already known gift taste.
- Fixed a logic bug regarding birthday gifts. Birthdays can be given even if max gifts has been given, but the X would appear before it was given. Now it will show after the gift is given.
- For those getting annoyed at all the updates: This should be the last one for now, barring any emergent bugs... I hope.
v1.1.1 - Aug 4, 2020:
- Added a check to limit the quest icon to Item Delivery quests. This should address issues with stuttering and crashing with Fishing and Mining quests.
v1.1.0 - Aug 1, 2020:
- Per a few requests (thanks, Donirexian and ColossusX13!), added a config option to see the disposition icons for items even if you haven't given them before, allowing Farmers to gauge their reaction without resorting to experimentation.
v1.0.0 - Jul 30, 2020:
- Initial upload.