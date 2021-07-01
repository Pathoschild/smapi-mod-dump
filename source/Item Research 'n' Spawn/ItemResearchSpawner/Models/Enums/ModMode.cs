/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;

namespace ItemResearchSpawner.Models
{
    public enum ModMode
    {
        Spawn,
        Buy
    }
    
    internal static class ModModeExtensions{
        
        public static string GetString(this ModMode current)
        {
            return current switch
            {
                ModMode.Spawn => "Spawn mode",
                ModMode.Buy => "Buy/Sell mode",
                _ => "???"
            };
        }
    }
}