/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.IO;
using StardewModdingAPI;

namespace CookingSkill
{
    public class SaveData
    {
        public static string FilePath => Path.Combine(Constants.CurrentSavePath, "cooking-skill.json");
        
        public int experience = 0;
    }
}
