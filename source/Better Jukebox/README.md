**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Gaphodil/BetterJukebox**

----

**Better Jukebox** is a [Stardew Valley](http://stardewvalley.net/) mod that redesigns the menu
that pops up when you use either the jukebox in the Saloon or a Mini-Jukebox.
Also includes optional settings to change the list of tracks, in-menu or permanently.

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/6017).
3. Run the game using SMAPI.

## Configure
The mod creates a `config.json` file in its mod folder the first time you run it. You can open that
file in a text editor to configure the mod.

These are the available settings:

| General settings      | what it does
| ---                   | ---
| `ShowMenu`            | Default `true`. Show the menu. When disabled, only permanent list settings will apply.

| List settings		    | what it does
| ---					| --- 
| `AmbientTracks`		| Default `1`. Addresses ambient and other non-musical tracks normally hidden from the player. `0` performs no removal, `1` uses the default removal, and `2` removes additional non-music items not removed normally, whether due to bugs or mod interference. Applied after adding unheard tracks.
| `Blacklist`           | Default `""`. Comma-separated list of music cues to hide from the list in addition to the above.
| `Whitelist`           | Default `""`. Comma-separated list of music cues to unhide from the list in addition to the above. Does not add tracks, only applies if heard or added with `ShowUnheardTracks`.
| `ShowLockedSongs`		| Default `false`. Shows locked icons in place of unheard songs from the soundtrack. Ignored if `ShowUnheardTracks` is enabled. Due to mutually exclusive tracks, you may not be able to hear them all.
| `ShowUnheardTracks`	| Default `false`. Songs not yet heard on the current save file can be found in the jukebox. WARNING: Will permanently add songs to the farmer's "heard songs" list when played in the Saloon.
| `UnheardSoundtrack`	| Default `true`. If `ShowUnheardTracks` is enabled, adds unheard Steam soundtrack tracks (plus a few extra).
| `UnheardNamed`		| Default `true`. If `ShowUnheardTracks` is enabled, adds unheard named, non-soundtrack tracks.
| `UnheardRandom`		| Default `false`. If `ShowUnheardTracks` is enabled, adds situational random-choice tracks.
| `UnheardMisc`			| Default `false`. If `ShowUnheardTracks` is enabled, adds unheard ambient tracks, unnamed unused songs, and a couple musical sound effects.
| `UnheardDupes`		| Default `false`. If `ShowUnheardTracks` is enabled, adds unused duplicate cues.
| `UnheardMusical`      | Default `false`. If `ShowUnheardTracks` is enabled, adds a subset of Unheard Named and Misc tracks that could be considered music. Adding both and setting `AmbientTracks` to `2` will display the same tracks.
| `PermanentUnheard`    | Default `false`. If `ShowUnheardTracks` is enabled, any selected unheard tracks are added to the farmer's "heard songs" list. This will be permanent after the game is saved.
| `PermanentBlacklist`  | Default `""`. Comma-separated list of music cues to remove from the farmer's "heard songs" list. Applied on loading a save. May be undone by hearing the song again.

| Functional settings   | what it does
| ---                   | ---
| `TrueRandom`			| Default `false`. Replace random function with menu items (including shown ambient tracks) instead of only heard songs, but lose automatic shuffle on warp.
| `ShowAlternateSorts`	| Default `true`. Shows and enables use of alternate sorts.

| Visual settings		| what it does
| ---					| --- 
| `ShowInternalID`		| Default `false`. Shows the internal reference for music tracks next to the actual name.
| `ShowBandcampNames`	| Default `true`. Replaces (sometimes incorrect) track names with the full Bandcamp names.

Note: the vanilla random function uses the farmer's "heard songs" list.

Note: you can utilize the blacklist/whitelist features more easily with
[this list of music ids](https://docs.google.com/spreadsheets/d/1lJS6UQRpMbIWlZafU0pd982Bc2g5bDjqwm3F_UOA7pY)
and [the included preset lists](Framework/BetterJukeboxHelper.cs).

## See also
* [release notes](release-notes.md)
* [Nexus page](https://www.nexusmods.com/stardewvalley/mods/6017)
* [Moddrop page](https://www.moddrop.com/stardew-valley/mods/984775-better-jukebox)
* [planned features](planned-features.md)
