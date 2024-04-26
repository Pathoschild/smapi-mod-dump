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

namespace InkStories
{
    public class SharedStoryData
    {
        public string Id { get; set; }
        public List<SharedStoryDataEntry> Data { get; set; } = new List<SharedStoryDataEntry>();
        public List<SharedStoryNumberEntry> Numbers { get; set; } = new List<SharedStoryNumberEntry>();
    }
}
