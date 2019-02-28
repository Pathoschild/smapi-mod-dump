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
    }
}
