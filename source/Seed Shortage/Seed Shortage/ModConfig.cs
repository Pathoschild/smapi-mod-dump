/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xynerorias/Seed-Shortage
**
*************************************************/

namespace SeedShortage
{
    public class ModConfig
    {
        public string[] VendorsWithoutSeeds { get; set; } =
        {
            "Pierre",
            "Sandy",
        };
        public string[] VendorsPrice { get; set; } =
        {
            "Joja",
            "Magic Boat"
        };
        public string PriceIncrease { get; set; } = "10x";
        public string[] CropExceptions { get; set; } =
        {
            "Parsnip Seeds",
            "Pink Cat Seeds"
        };
    }
}
