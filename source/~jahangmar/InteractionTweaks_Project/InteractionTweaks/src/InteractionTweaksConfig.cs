/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

using System;
namespace InteractionTweaks
{
    public class InteractionTweaksConfig
    {
        public bool EatingFeature { get; set; } = true;
        public bool EatingWithoutWaste { get; set; } = false;
        public bool EatingTopHealth { get; set; } = true;
        public bool EatingTopStamina { get; set; } = true;
        //public bool ToolsFeature { get; set; } = true;
        //public bool SlingshotFeature { get; set; } = true;
        public bool AdventurersGuildShopFeature { get; set; } = true;
        //public bool WeaponBlockingFeature { get; set; } = true;
        public bool CarpenterMenuFeature { get; set; } = true;
        //public bool FishingRodFeature { get; set; } = true;
        public bool SellableItemsFeature { get; set; } = true;
    }
}
