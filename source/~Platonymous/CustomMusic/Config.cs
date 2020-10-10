/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace CustomMusic
{
    public class Config
    {
        public bool Convert { get; set; } = false;
        public bool Debug { get; set; } = false;
        public Dictionary<string, string> Presets { get; set; } = new Dictionary<string, string>();

        public float SoundVolume { get; set; } = 0.3f;

        public float MusicVolume { get; set; } = 0.5f;
    }
}
