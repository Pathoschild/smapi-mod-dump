/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MiguelLucas/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustableBuildingCosts.Framework
{
    class ModConfig
    {
        public Dictionary<string, BlueprintCost> Buildings { get; set; } = new Dictionary<string, BlueprintCost>();

        public ModConfig()
        {
            BlueprintCost coop = new BlueprintCost();
            coop.GoldCost = 4000;
            coop.Items.Add(new ItemAmount((int) ItemID.WOOD, 300));
            coop.Items.Add(new ItemAmount((int) ItemID.STONE, 100));

            BlueprintCost bigCoop = new BlueprintCost();
            bigCoop.GoldCost = 10000;
            bigCoop.Items.Add(new ItemAmount((int) ItemID.WOOD, 400));
            bigCoop.Items.Add(new ItemAmount((int) ItemID.STONE, 150));

            BlueprintCost deluxeCoop = new BlueprintCost();
            deluxeCoop.GoldCost = 20000;
            deluxeCoop.Items.Add(new ItemAmount((int) ItemID.WOOD, 500));
            deluxeCoop.Items.Add(new ItemAmount((int) ItemID.STONE, 200));

            BlueprintCost barn = new BlueprintCost();
            barn.GoldCost = 6000;
            barn.Items.Add(new ItemAmount((int) ItemID.WOOD, 350));
            barn.Items.Add(new ItemAmount((int) ItemID.STONE, 150));

            BlueprintCost bigBarn = new BlueprintCost();
            bigBarn.GoldCost = 12000;
            bigBarn.Items.Add(new ItemAmount((int) ItemID.WOOD, 450));
            bigBarn.Items.Add(new ItemAmount((int) ItemID.STONE, 200));

            BlueprintCost deluxeBarn = new BlueprintCost();
            deluxeBarn.GoldCost = 25000;
            deluxeBarn.Items.Add(new ItemAmount((int) ItemID.WOOD, 550));
            deluxeBarn.Items.Add(new ItemAmount((int) ItemID.STONE, 300));

            BlueprintCost shed = new BlueprintCost();
            shed.GoldCost = 15000;
            shed.Items.Add(new ItemAmount((int) ItemID.WOOD, 300));

            BlueprintCost bigShed = new BlueprintCost();
            bigShed.GoldCost = 20000;
            bigShed.Items.Add(new ItemAmount((int) ItemID.WOOD, 550));
            bigShed.Items.Add(new ItemAmount((int) ItemID.STONE, 300));

            BlueprintCost silo = new BlueprintCost();
            silo.GoldCost = 1000;
            silo.Items.Add(new ItemAmount((int) ItemID.STONE, 100));
            silo.Items.Add(new ItemAmount((int) ItemID.CLAY, 10));
            silo.Items.Add(new ItemAmount((int) ItemID.COPPER_BAR, 5));

            BlueprintCost mill = new BlueprintCost();
            mill.GoldCost = 2500;
            mill.Items.Add(new ItemAmount((int) ItemID.WOOD, 150));
            mill.Items.Add(new ItemAmount((int) ItemID.STONE, 50));
            mill.Items.Add(new ItemAmount((int) ItemID.CLOTH, 4));

            BlueprintCost well = new BlueprintCost();
            well.GoldCost = 1000;
            well.Items.Add(new ItemAmount((int) ItemID.STONE, 75));

            BlueprintCost fishPond = new BlueprintCost();
            fishPond.GoldCost = 5000;
            fishPond.Items.Add(new ItemAmount((int) ItemID.STONE, 200));
            fishPond.Items.Add(new ItemAmount((int) ItemID.SEAWEED, 5));
            fishPond.Items.Add(new ItemAmount((int) ItemID.GREEN_ALGAE, 5));

            BlueprintCost stable = new BlueprintCost();
            stable.GoldCost = 10000;
            stable.Items.Add(new ItemAmount((int) ItemID.HARDWOOD, 100));
            stable.Items.Add(new ItemAmount((int) ItemID.IRON_BAR, 5));

            BlueprintCost slimeHutch = new BlueprintCost();
            slimeHutch.GoldCost = 10000;
            slimeHutch.Items.Add(new ItemAmount((int) ItemID.STONE, 500));
            slimeHutch.Items.Add(new ItemAmount((int) ItemID.REFINED_QUARTZ, 10));
            slimeHutch.Items.Add(new ItemAmount((int) ItemID.IRIDIUM_BAR, 1));

            BlueprintCost shippingBin = new BlueprintCost();
            shippingBin.GoldCost = 250;
            shippingBin.Items.Add(new ItemAmount((int) ItemID.WOOD, 150));

            BlueprintCost goldClock = new BlueprintCost();
            goldClock.GoldCost = 10000000;

            BlueprintCost junimoHut = new BlueprintCost();
            junimoHut.GoldCost = 20000;
            junimoHut.Items.Add(new ItemAmount((int) ItemID.STONE, 200));
            junimoHut.Items.Add(new ItemAmount((int) ItemID.FIBER, 100));
            junimoHut.Items.Add(new ItemAmount((int) ItemID.STARFRUIT, 9));

            BlueprintCost desertObelisk = new BlueprintCost();
            desertObelisk.GoldCost = 1000000;
            desertObelisk.Items.Add(new ItemAmount((int) ItemID.IRIDIUM_BAR, 20));
            desertObelisk.Items.Add(new ItemAmount((int) ItemID.COCONUT, 10));
            desertObelisk.Items.Add(new ItemAmount((int) ItemID.CACTUS_FRUIT, 10));

            BlueprintCost earthObelisk = new BlueprintCost();
            earthObelisk.GoldCost = 500000;
            earthObelisk.Items.Add(new ItemAmount((int) ItemID.IRIDIUM_BAR, 10));
            earthObelisk.Items.Add(new ItemAmount((int) ItemID.EARTH_CRYSTAL, 10));

            BlueprintCost islandObelisk = new BlueprintCost();
            islandObelisk.GoldCost = 1000000;
            islandObelisk.Items.Add(new ItemAmount((int) ItemID.IRIDIUM_BAR, 10));
            islandObelisk.Items.Add(new ItemAmount((int) ItemID.BANANA, 10));
            islandObelisk.Items.Add(new ItemAmount((int) ItemID.DRAGON_TOOTH, 10));

            BlueprintCost waterObelisk = new BlueprintCost();
            waterObelisk.GoldCost = 500000;
            waterObelisk.Items.Add(new ItemAmount((int) ItemID.IRIDIUM_BAR, 5));
            waterObelisk.Items.Add(new ItemAmount((int) ItemID.CLAM, 10));
            waterObelisk.Items.Add(new ItemAmount((int) ItemID.CORAL, 10));


            Buildings.Add("Coop", coop);
            Buildings.Add("Big Coop", bigCoop);
            Buildings.Add("Deluxe Coop", deluxeCoop);
            Buildings.Add("Barn", barn);
            Buildings.Add("Big Barn", bigBarn);
            Buildings.Add("Deluxe Barn", deluxeBarn);
            Buildings.Add("Shed", shed);
            Buildings.Add("Big Shed", bigShed);
            Buildings.Add("Silo", silo);
            Buildings.Add("Mill", mill);
            Buildings.Add("Well", well);
            Buildings.Add("Fish Pond", fishPond);
            Buildings.Add("Stable", stable);
            Buildings.Add("Slime Hutch", slimeHutch);
            Buildings.Add("Shipping Bin", shippingBin);

            Buildings.Add("Gold Clock", goldClock);
            Buildings.Add("Junimo Hut", junimoHut);
            Buildings.Add("Desert Obelisk", desertObelisk);
            Buildings.Add("Island Obelisk", islandObelisk);
            Buildings.Add("Earth Obelisk", earthObelisk);
            Buildings.Add("Water Obelisk", waterObelisk);
        }
    }
}
