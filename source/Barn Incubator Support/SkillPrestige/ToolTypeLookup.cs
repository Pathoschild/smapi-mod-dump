/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley.Tools;

namespace SkillPrestige
{
    /// <summary>
    /// Tool Type lookup that ties items in the ToolType enum to their respective Types in Stardew Valley.
    /// </summary>
    public static class ToolTypeLookup
    {
        public static IDictionary<ToolType, Type> ToolTypes => new Dictionary<ToolType, Type>
        {
            {ToolType.Axe, typeof(Axe)},
            {ToolType.FishingRod, typeof(FishingRod) },
            {ToolType.Hoe, typeof(Hoe) },
            {ToolType.Pickaxe, typeof(Pickaxe) },
            {ToolType.WateringCan, typeof(WateringCan) }
        };
    }
}