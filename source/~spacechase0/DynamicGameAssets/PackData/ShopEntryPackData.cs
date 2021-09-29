/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.ComponentModel;

namespace DynamicGameAssets.PackData
{
    public class ShopEntryPackData : BasePackData
    {
        /*
        BlueBoat
        TravelingMerchant
        GeMagic
        DesertMerchant
        HatMouse
        IslandMerchant
        ResortBar
        Krobus
        VolcanoShop
        Festival.<date> ie. Festival.summer5
        QiGemShop
        Joja
        IceCreamStand
        Hospital
        Club
        Theater_BoxOffice
        FishShop
        SeedShop
        Sandy
        Saloon
        AdventurerGuild
        Carpenter
        AnimalSupplies
        Blacksmith
        Dwarf
        */
        public string ShopId { get; set; }

        public ItemAbstraction Item { get; set; }

        [DefaultValue(int.MaxValue)]
        public int MaxSold { get; set; } = int.MaxValue;

        public int Cost { get; set; }

        [DefaultValue(null)]
        public string Currency { get; set; }
    }
}
