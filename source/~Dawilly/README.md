**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Dawilly/SAAT**

----

# SAAT - Stardew Valley Audio API &amp; Toolkit

Audio API & Audio Management for SMAPI, without any utilization of HarmonyLib.

---

## Current Functionality:
Allow brand new music and sound effects to be added into Stardew Valley
Audio tracks that are defined as `music` should appear on the Jukebox, once the player character has heard them.
Allow music to loop continuously.
Allow modders to add audio tracks to the jukebox on the start of a new game.

## Not Yet Implemented:

#### Audio Track Replacement / Overrides
An individual can replace replace existing audio tracks with another, overriding vanilla assets in a non-destructable manner.

#### Audio Signal Processing (APS or DPS)
Upcoming feature that will allow for audio to undergo low-pass, high-pass, altered pitch, effects, and more without the need to create another track.

#### Proper memory management
This is to be the prime feature of the API. As of now, Stardew Valley relies on XNA / MonoGame to handle memory management for audio. The memory management is poor in design, as it loads all or nothing. Typically, audio is chunked into memory rather than retailing it all at once. This creates a very large consumption of memory.

In example, the winter music tracks are loaded into memory at the beginning of the game. Even if its spring. You won't hear the audio track for 3/4th of the game, yet it is still consuming memory.

Note: This subset of the API / Mod will be written in C. Native is required to ensure performance.

#### Packed Audio Format
In conjunction with proper memory management, audio is to be compacted into binary files on a per mod basis. This allows for faster loading / unloading / reloading operating.

It will **not be required**, but it will optimizes performance. Especially for larger audio files.

#### Toolkit for Packed Audio Format
To assist and encourage content creators in optimizing audio for performance, a tool will be provided to create the packed audio format mentioned above.

---

## Documentation

#### Structure: SAAT.API vs SAAT.Mod

SAAT comes in the form of two, individual SMAPI mods: SAAT.API and SAAT.Mod. This seperation allows for other mods to interact with the backend operations of SAAT while allowing content creators to not have to be concerned with any heavy technicalities.

- **SAAT.API**: Does the actual work load / operations relating to audio, such as I/O management, DSP, etc.
- **SAAT.Mod**: Allows content creator to add/modify audio tracks with ease, as if it was an extention of Content Patcher.

Naturally, SAAT.Mod depends on SAAT.API.

#### Adding an audio track

**The Basics**

You can add your own audio track to the game via SAAT.Mod. The structure is similiar to a Content Patcher mod.

You will need to create a SMAPI Mod, label the mod as a Content Pack for `ZeroMeters.SAAT.Mod`, add the audio files and a `tracks.json` file. The structure of `tracks.json` is as followed:

```
[
    {
        "Id": "Audio Tracks Displaying and Systematic ID",
        "Filepath": "Relative file path to the audio track",
        "Category": "A Valid SDV Audio Category"
    }
]
```

For every track entry there must be three fields: `Id`, `Filepath`, and `Category`.
- `Id`: The unique identification of the audio track. **Please note this identification will be the name displayed in the juke box**
- `Filepath`: The relative path (that is, from the mod folder) to the audio file.
- `Category`: A valid Audio Category

Additionally, there is a fourth and optional field: `Settings`. More on that below.

`Filepath` can point to a `.wav` or a `.ogg`. Only these two file formats are supported. Additional file formats can be implemented if there is a demand for them.

`Category` can be one of the following values:
- `Music`
- `Sound`
- `Ambient`
- `Footsteps`

Setting the category allows for the the proper volumne settings to apply to the audio track. That is, if an audio track is set to `Category: Ambient`, the volumne settings for Ambient will apply.

Jukebox Functionality: Any audio track that is set to `Category: Music`, will appear in the jukebox once heard by the player.

**Additional Settings**

You can add the fourth and optional field `Settings` to specify certain behaviors. At this time (SAAT 1.1), there are two possible options:

- `Loop`: Has the audio track continuously loop until explicitly stopped by another mod or the game.
- `AddToJukebox`: Adds the audio track to the Jukebox **on the creation of a new game**.

**Examples**

An example of a `tracks.json` file loading three audio tracks.

```
[
    {
        "Id": "JericTheme",
        "Filepath": "Music/JericTheme.ogg",
        "Category": "Music",
        "Settings": {
            "Loop": true,
            "AddToJukebox": false
        }
    },
    {
        "Id": "CrimsonSands",
        "Filepath": "CrimsonSands.wav",
        "Category": "Music"
    },
    {
        "Id": "ZCCC_Crowd_Ambience",
        "Filepath": "ZCCC_Crowd_Ambience.wav",
        "Category": "Ambient"
    }
]

```

You can also generate an example `tracks.json` by starting SMAPI with SAAT.Mod and SAAT.API installed and typing `tracktemplate` into the console.
The file will be written out in the SAAT.Mod folder.

#### Migrating from Custom Music

The conversion process is pretty straightforward in regards to migrate from Custom Music. 

After you convert your content.json to a tracks.json, in your map files and event script files, remove all `cm:` prefixes from the audio IDs.

#### Debugging & Visibility

You can observe which audio tracks are loaded by typing `audioallocs` into the SMAPI console. This command only requires SAAT.API to be installed. You can also print out a detailed listing of a specific audio track by typing `audioallocs [id]`, where `[id]` is a valid audio id / name.

You can determine if the audio tracks loaded in properly by forcibily playing them in a playlist style operation. With SAAT.API and SAAT.Mod installed, typing `audiodebug true` into the SMAPI console enables the debug feature. Once activated, you can use the following keys to cycle through the loaded tracks:

| Key | Function                |
|-----|-------------------------|
|  1  | Stops the current track |
|  2  | Play the next track     |
|  3  | Play the previous track |

