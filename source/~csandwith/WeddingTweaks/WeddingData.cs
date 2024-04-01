/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace WeddingTweaks
{
    public class WeddingData
    {
        public List<string> witnesses = new List<string>();
        public List<string> witnessAcceptDialogue = new List<string>();
        public List<string> witnessDeclineDialogue = new List<string>();
        public int witnessFrame = -1;
        public int witnessAcceptChance = -1;
    }
}