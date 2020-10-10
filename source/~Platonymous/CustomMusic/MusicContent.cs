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
    public class MusicContent
    {
        public List<MusicItem> Music { get; set; } = new List<MusicItem>();

        public List<SoundItem> Sounds { get; set; } = new List<SoundItem>();

        public List<SoundEmitter> Emitters { get; set; } = new List<SoundEmitter>();
        public List<LocationItem> Locations { get; set; } = new List<LocationItem>();
    }
}
