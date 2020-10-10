/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ZaneYork/SDV_Mods
**
*************************************************/

using StardewModdingAPI.Utilities;
using SObject = StardewValley.Object;

namespace CustomCropsDecay
{
    public interface IApi
    {
        SDate getCropHarvestDate(SObject crop);
        int getCropDecayDays(SObject crop);
    }
}
