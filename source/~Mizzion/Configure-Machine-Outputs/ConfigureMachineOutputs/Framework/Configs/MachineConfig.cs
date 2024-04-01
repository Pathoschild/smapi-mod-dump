/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using ConfigureMachineOutputs.Framework.Configs.Machines;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Machines;
using System.Collections.Generic;
using System.Linq;


namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class MachineConfig
    {
        /*
        public SeedMakerConfig SeedMaker = new SeedMakerConfig();
        public KegConfig Keg = new KegConfig();
        public PreservesJarConfig PreservesJar = new PreservesJarConfig();
        public CheesePressConfig CheesePress = new CheesePressConfig();
        public MayoConfig Mayonnaise = new MayoConfig();
        public LoomConfig Loom = new LoomConfig();
        public OilMakerConfig OilMaker = new OilMakerConfig();
        public CrystalariumConfig Crystalarium = new CrystalariumConfig();
        public RecyclingConfig Recycling = new RecyclingConfig();
        public FurnaceConfig Furnace = new FurnaceConfig();
        public CharcoalConfig Charcoal = new CharcoalConfig();
        public SlimeEggPressConfig SlimeEggPress = new SlimeEggPressConfig();
        public WormBinConfig WormBin = new WormBinConfig();
        public LightningRodConfig LightningRod = new LightningRodConfig();
        public SolarPanelConfig SolarPanel = new SolarPanelConfig();
        public TapperConfig Tapper = new TapperConfig();
        public HeavyTapperConfig HeavyTapper = new HeavyTapperConfig();
        //public IncubatorConfig Incubator = new IncubatorConfig();
        //public OstrichIncubatorConfig OstrichIncubator = new OstrichIncubatorConfig();
        public BeeHouseConfig BeeHouse = new BeeHouseConfig();
        //public SlimeIncubatorConfig SlimeIncubator = new SlimeIncubatorConfig();
        public DeconstructorConfig Deconstructor = new DeconstructorConfig();
        public GeodeCrusherConfig GeodeCrusher = new GeodeCrusherConfig();
        public BoneMillConfig BoneMill = new BoneMillConfig();
        public WoodChipperConfig WoodChipper = new WoodChipperConfig();
        //public CoffeeMakerConfig CoffeeMaker = new CoffeeMakerConfig();

        */
        public Dictionary<string, MachineConfigData> Machines { get; set; }

        public MachineConfig() 
        {
            /*
            var mach = StardewValley.DataLoader.Machines(Game1.content);
            var bigCraftables = Game1.bigCraftableData;

            foreach(var machine in mach)
            {
                if (Machines.ContainsKey(machine.Key))
                    continue;
                var machineName = bigCraftables is null ? "Not Found" : bigCraftables[machine.Key].DisplayName;
                Machines.TryAdd(machine.Key, new MachineConfigData()
                {
                    Enabled = true,
                    MachineName = machineName,
                    QualityId = MachineConfigData.SetId(machine.Key),
                    OutPut = machine.Value.OutputRules
                });            
            }
            */
            /*
            if (Machines.Any())
                return;*/
            Machines = new()
            {
                /*
                { "(BC)9", new MachineConfigData(){ 
                    Enabled = true, 
                    MachineName = "", 
                    QualityId = "(BC)9", 
                    OutPut = new List<MachineOutputRule>{ } 
                } },*/
                { "(BC)10", new MachineConfigData(){ 
                    Enabled = true,
                    MachineName = "",
                    QualityId = "",
                    OutPut = new List<MachineOutputRule>{}

                } },
                { "(BC)12", new MachineConfigData(){ } },
                { "(BC)13", new MachineConfigData(){ } },
                { "(BC)15", new MachineConfigData(){ } },
                { "(BC)16", new MachineConfigData(){ } },
                { "(BC)17", new MachineConfigData(){ } },
                { "(BC)19", new MachineConfigData(){ } },
                { "(BC)20", new MachineConfigData(){ } },
                { "(BC)21", new MachineConfigData(){ } },
                { "(BC)24", new MachineConfigData(){ } },
                { "(BC)25", new MachineConfigData(){ } },
                { "(BC)90", new MachineConfigData(){ } },
                { "(BC)101", new MachineConfigData(){ } },
                { "(BC)105", new MachineConfigData(){ } },
                { "(BC)114", new MachineConfigData(){ } },
                { "(BC)117", new MachineConfigData(){ } },
                { "(BC)127", new MachineConfigData(){ } },
                { "(BC)128", new MachineConfigData(){ } },
                { "(BC)154", new MachineConfigData(){ } },
                { "(BC)156", new MachineConfigData(){ } },
                { "(BC)158", new MachineConfigData(){ } },
                { "(BC)160", new MachineConfigData(){ } },
                { "(BC)163", new MachineConfigData(){ } },
                { "(BC)182", new MachineConfigData(){ } },
                { "(BC)211", new MachineConfigData(){ } },
                { "(BC)231", new MachineConfigData(){ } },
                { "(BC)246", new MachineConfigData(){ } },
                { "(BC)254", new MachineConfigData(){ } },
                { "(BC)264", new MachineConfigData(){ } },
                { "(BC)265", new MachineConfigData(){ } },
                { "(BC)280",new MachineConfigData(){ }  }
            };
            

        }
    }
}
