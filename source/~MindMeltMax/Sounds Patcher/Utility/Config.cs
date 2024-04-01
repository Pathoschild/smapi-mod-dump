/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace SoundsPatcher.Utility
{
    public class Config
    {
        public Dictionary<string, bool> Sounds { get; set; }

        public Dictionary<string, bool> Songs { get; set; }

        public Dictionary<string, bool> UnknownSounds { get; set; }

        public KeybindList MenuKeys { get; set; } = KeybindList.Parse("O, RightStick");

        public Config() { }

        public Config(Dictionary<string, bool> sounds, Dictionary<string, bool> songs, Dictionary<string, bool> unknownSounds, KeybindList key)
        {
            Sounds = sounds;
            Songs = songs;
            UnknownSounds = unknownSounds;
            MenuKeys = key;
        }
    }
}
