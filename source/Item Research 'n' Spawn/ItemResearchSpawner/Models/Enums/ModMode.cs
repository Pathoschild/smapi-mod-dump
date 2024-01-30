/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

namespace ItemResearchSpawner.Models.Enums
{
    public enum ModMode
    {
        Research,
        BuySell, 
        Combined,
    }
    
    internal static class ModModeExtensions{
        
        public static string GetString(this ModMode current)
        {
            return current switch
            {
                ModMode.Research => "Research (Spawn) mode",
                ModMode.BuySell => "Buy/Sell mode",
                ModMode.Combined => "Combined mode",
                _ => "???"
            };
        }
    }
}