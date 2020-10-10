/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/CookingSkill
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.IO;

namespace CookingSkill
{
    public class MultiplayerSaveData
    {
        public static string FilePath => Path.Combine(Constants.CurrentSavePath, "cooking-skill-mp.json");

        public Dictionary<long, int> Experience { get; set; } = new Dictionary<long, int>();
    }
}