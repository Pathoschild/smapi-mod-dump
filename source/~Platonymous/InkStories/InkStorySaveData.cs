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
    public class InkStorySaveData
    {
        public string Id { get; set; }
        public string LastState { get; set; }
        public SharedStoryData SharedData { get; set; } = new SharedStoryData();
    }
}
