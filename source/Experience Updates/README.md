**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Quipex/ExperienceUpdates**

----

**Experience Updates** is a [Stardew Walley](http://stardewvalley.net/) visual mod that shows you when you gain the experience. Available on [NexusMods](https://www.nexusmods.com/stardewvalley/mods/7581)

This is how it shows without [experience bars mod](https://github.com/spacechase0/ExperienceBars) installed (pay attention to the top left corner) <br>
![no_bars_mod](media/preview_no_bars.gif)

This is with the [mod](https://github.com/spacechase0/ExperienceBars)<br>
![with_bars_mod](media/preview_with_bars.gif)

## Configuration
Mod configuration is file-based. To change configuration simply change the values in config.json with any text editor. It requires a game restart in order to apply the config changes.

Key | Value  | Description
--- | ---    | ---
X   | number (positive/negative) (295 default) | The horizontal offset from the left side of the screen if positive (or from the right side of the screen if negative).
Y   | number (positive/negative) (20 default) | The vertical offset from the top of the screen if positive (or from the bottom of the screen if negative)
TextDurationMS | number (4000 default) | The time in milliseconds for the numbers animation to perform (4000 = 4 seconds)

Credits to [spacechase0](https://github.com/spacechase0) and his [experience bars](https://github.com/spacechase0/ExperienceBars) for the inspiration and some code references.
