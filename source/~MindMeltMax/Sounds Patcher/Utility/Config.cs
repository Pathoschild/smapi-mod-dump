/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sounds_Patcher.Utility
{
    public class Config
    {
        public Dictionary<string, bool> Sounds { get; set; } 

        public Dictionary<string, bool> Songs { get; set; }

        public string MenuKey { get; set; } = "o";

        public Config() { }

        public Config(Dictionary<string, bool> sounds, Dictionary<string, bool> songs, string key)
        {
            Sounds = sounds;
            Songs = songs;
            MenuKey = key;
        }
    }
}
