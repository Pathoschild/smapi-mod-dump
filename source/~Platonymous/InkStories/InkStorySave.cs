/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkStories
{
    public class InkStorySave
    {
        public List<InkStorySaveData> Data { get; set; } = new List<InkStorySaveData>();
        public Dictionary<string, List<string>> InksForNextDay { get; set; }
    }
}
