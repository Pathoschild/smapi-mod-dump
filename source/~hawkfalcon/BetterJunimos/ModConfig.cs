/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace BetterJunimos {
    internal class ModConfig {
        public Dictionary<string, bool> JunimoAbilites { get; set; } = new Dictionary<string, bool>();

        public HutSettings JunimoHuts { get; set; } = new HutSettings();
        internal class HutSettings {
            public int MaxJunimos { get; set; } = 3;
            public int MaxRadius { get; set; } = 8;
            public bool AvailibleAfterCommunityCenterComplete { get; set; } = true;
            public bool AvailibleImmediately { get; set; } = false;
            public bool ReducedCostToConstruct { get; set; } = true;
            public bool FreeToConstruct { get; set; } = false;
        }

        public JunimoImprovement JunimoImprovements { get; set; } = new JunimoImprovement();
        internal class JunimoImprovement {
            public bool CanWorkInRain { get; set; } = true;
            public bool CanWorkInWinter { get; set; } = false;
            public bool CanWorkInEvenings { get; set; } = false;
            public bool WorkFaster { get; set; } = false;
            public bool AvoidHarvestingFlowers { get; set; } = true;
            public bool AvoidPlantingCoffee { get; set; } = true;
        }

        public JunimoPayments JunimoPayment { get; set; } = new JunimoPayments();
        internal class JunimoPayments {
            public bool WorkForWages { get; set; } = false;
            public PaymentAmount DailyWage { get; set; } = new PaymentAmount();
            internal class PaymentAmount {
                public int ForagedItems { get; set; } = 1;
                public int Flowers { get; set; } = 0;
                public int Fruit { get; set; } = 0;
                public int Wine { get; set; } = 0;
            }
        }

        public FunSettings FunChanges { get; set; } = new FunSettings();
        internal class FunSettings {
            public float RainyJunimoSpiritFactor = 0.7f;
            public bool JunimosAlwaysHaveLeafUmbrellas { get; set; } = false;
            public bool MoreColorfulLeafUmbrellas { get; set; } = false;
            public bool InfiniteJunimoInventory { get; set; } = false;
        }

        public OtherSettings Other { get; set; } = new OtherSettings();
        internal class OtherSettings {
            public SButton SpawnJunimoKeybind { get; set; } = SButton.J;
            public bool ReceiveMessages { get; set; } = true;
        }

        public JunimoProgression Progression { get; set; } = new JunimoProgression();
        internal class JunimoProgression {
            public bool Enabled { get; set; } = true;

            public MoreJunimosPT MoreJunimos { get; set; } = new MoreJunimosPT();
            internal class MoreJunimosPT {
                public int Item { get; set; } = 268; // starfruit
                public int Stack { get; set; } = 3;
            }
            public UnlimitedJunimosPT UnlimitedJunimos { get; set; } = new UnlimitedJunimosPT();
            internal class UnlimitedJunimosPT {
                public int Item { get; set; } = 268; // starfruit
                public int Stack { get; set; } = 5;
            }
            public WorkInRainPT CanWorkInRain { get; set; } = new WorkInRainPT();
            internal class WorkInRainPT {
                public int Item { get; set; } = 771; // fiber
                public int Stack { get; set; } = 40;
            }
            public WorkInWinterPT CanWorkInWinter { get; set; } = new WorkInWinterPT();
            internal class WorkInWinterPT {
                public int Item { get; set; } = 440; // wool
                public int Stack { get; set; } = 6;
            }
            public WorkInEveningsPT CanWorkInEvenings { get; set; } = new WorkInEveningsPT();
            internal class WorkInEveningsPT {
                public int Item { get; set; } = 768; // solar essence
                public int Stack { get; set; } = 2;
            }
            public WorkFasterPT WorkFaster { get; set; } = new WorkFasterPT();
            internal class WorkFasterPT {
                public int Item { get; set; } = 771; // fiber
                public int Stack { get; set; } = 40;
            }
            public ReducedCostToConstructPT ReducedCostToConstruct { get; set; } = new ReducedCostToConstructPT();
            internal class ReducedCostToConstructPT {
                public int Item { get; set; } = 336; // gold bar
                public int Stack { get; set; } = 5;
            }
            public PlantPT PlantCrops { get; set; } = new PlantPT();
            internal class PlantPT {
                public int Item { get; set; } = 335; // iron bar
                public int Stack { get; set; } = 5;
            }
            public WaterPT Water { get; set; } = new WaterPT();
            internal class WaterPT {
                public int Item { get; set; } = 88; // coconut
                public int Stack { get; set; } = 10;
            }
            public FertilizePT Fertilize { get; set; } = new FertilizePT();
            internal class FertilizePT {
                public int Item { get; set; } = 330; // clay
                public int Stack { get; set; } = 20;
            }
            public HarvestForageCropsPT HarvestForageCrops { get; set; } = new HarvestForageCropsPT();
            internal class HarvestForageCropsPT {
                public int Item { get; set; } = 372; // clam
                public int Stack { get; set; } = 6;
            }
            public HarvestBushesPT HarvestBushes { get; set; } = new HarvestBushesPT();
            internal class HarvestBushesPT {
                public int Item { get; set; } = 709; // hardwood
                public int Stack { get; set; } = 6;
            }
            public ClearDeadCropsPT ClearDeadCrops { get; set; } = new ClearDeadCropsPT();
            internal class ClearDeadCropsPT {
                public int Item { get; set; } = 769; // void essence
                public int Stack { get; set; } = 2;
            }
        }
    }
}
