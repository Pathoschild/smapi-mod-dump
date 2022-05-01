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
        public Dictionary<string, bool> JunimoAbilities { get; set; } = new();

        public HutSettings JunimoHuts { get; set; } = new();
        internal class HutSettings {
            public int MaxJunimos { get; set; } = 3;
            public int MaxRadius { get; set; } = 8;
            public bool AvailableAfterCommunityCenterComplete { get; set; } = true;
            public bool AvailableImmediately { get; set; }
            public bool ReducedCostToConstruct { get; set; } = true;
            public bool FreeToConstruct { get; set; }
        }

        public JunimoImprovement JunimoImprovements { get; set; } = new();
        internal class JunimoImprovement {
            public bool CanWorkInRain { get; set; } = true;
            public bool CanWorkInWinter { get; set; } = true;
            public bool CanWorkInEvenings { get; set; } = true;
            public bool CanWorkInGreenhouse { get; set; } = true;
            public bool WorkFaster { get; set; } = true;
            public bool WorkRidiculouslyFast { get; set; }
            public bool AvoidHarvestingFlowers { get; set; } = true;
            public bool AvoidHarvestingGiants { get; set; } = true;
            public bool HarvestEverythingOn28th { get; set; } = true;
            public bool AvoidPlantingCoffee { get; set; } = true;
            public bool AvoidPlantingOutOfSeason { get; set; } = true;
        }

        public JunimoPayments JunimoPayment { get; set; } = new();
        internal class JunimoPayments {
            public bool WorkForWages { get; set; }
            public PaymentAmount DailyWage { get; set; } = new();
            internal class PaymentAmount {
                public int ForagedItems { get; set; } = 1;
                public int Flowers { get; set; }
                public int Fruit { get; set; }
                public int Wine { get; set; }
            }
            
            public bool GiveExperience { get; set; }

        }

        public FunSettings FunChanges { get; set; } = new();
        internal class FunSettings {
            public float RainyJunimoSpiritFactor = 0.7f;
            public bool JunimosAlwaysHaveLeafUmbrellas { get; set; } 
            public bool MoreColorfulLeafUmbrellas { get; set; }
            public bool InfiniteJunimoInventory { get; set; }
        }

        public OtherSettings Other { get; set; } = new();
        internal class OtherSettings {
            public SButton SpawnJunimoKeybind { get; set; } = SButton.J;
            public bool ReceiveMessages { get; set; } = true;
            public bool HutClickEnabled { get; set; } = true;
            public SButton HutMenuKeybind { get; set; } = SButton.None;
        }

        public JunimoProgression Progression { get; set; } = new();
        internal class JunimoProgression {
            public bool Enabled { get; set; } = true;
        }
    }
}
