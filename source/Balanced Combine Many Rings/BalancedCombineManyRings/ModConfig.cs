/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/arruda/BalancedCombineManyRings
**
*************************************************/

using System;
namespace BalancedCombineManyRings
{
    public class ModConfig
    {
        public bool DestroyRingOnFailure { get; set; } = false;
        public int FailureChancePerExtraRing { get; set; } = 20;
        public int CostPerExtraRing { get; set; } = 100;
    }
}
