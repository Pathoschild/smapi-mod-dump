/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/YariazenMods
**
*************************************************/


using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;

namespace SeasonalSeeds
{
    public class ModEntry : Mod
    {
        IModHelper Helper;
        public override void Entry(IModHelper helper)
        {
            Helper = helper;

        }
    }
}