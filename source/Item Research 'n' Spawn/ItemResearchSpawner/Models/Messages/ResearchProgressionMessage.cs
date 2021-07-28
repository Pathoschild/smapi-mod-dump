/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System.Collections.Generic;

namespace ItemResearchSpawner.Models.Messages
{
    public class ResearchProgressionMessage
    {
        public string PlayerID { get; set; }
        public Dictionary<string, ResearchProgression> Progression { get; set; }
    }
}