/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace SelfServe
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool FishShop { get; set; } = true;
        public bool SeedShop { get; set; } = true;
        public bool SandyShop { get; set; } = true;
        public bool IceCreamShop { get; set; } = true;
        public bool AnimalShop { get; set; } = true;
        public bool HospitalShop { get; set; } = true;
        public bool CarpenterShop { get; set; } = true;
        public bool SaloonShop { get; set; } = true;
        public bool SmithShop { get; set; } = true;
    }
}
