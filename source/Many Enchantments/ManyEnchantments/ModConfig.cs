/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SlivaStari/ManyEnchantments
**
*************************************************/

using System;
namespace CombineManyRings
{
    public sealed class ModConfig
    {
        public bool AllowSameRing { get; set; } = false;
        public bool BalancedMode { get; set; } = false;
        public bool DestroyRingOnFailure { get; set; } = false;
        public int FailureChancePerExtraRing { get; set; } = 20;
        public int CostPerExtraRing { get; set; } = 100;
    }
}