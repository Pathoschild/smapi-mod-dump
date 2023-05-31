/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGiftDialogue.Data
{
    public class NpcGiftData
    {
        public class GiftData
        {
            public string Id { get; set; }
            public string Condition { get; set; }
            public string Items { get; set; }
            public string DialogueKey { get; set; }
        }
        
        public double GiveChance { get; set; }
        public List<GiftData> Gifts { get; set; }
    }
}
