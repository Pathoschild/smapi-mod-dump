/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/captncraig/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace HardcoreBundles
{
    public class GameState
    {
        public static GameState Current;

        public bool Activated { get; set; }
        public bool Declined { get; set; }

        public IList<int> Level5PerksAdded = new List<int>();


    }
}
