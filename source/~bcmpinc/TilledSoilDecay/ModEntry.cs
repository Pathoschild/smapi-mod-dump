/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using StardewModdingAPI;
using System.Reflection.Emit;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;

namespace StardewHack.TilledSoilDecay
{
    public class ModConfig
    {
        public class DecayConfig {
            /** Chance that a patch of tilled soil will disappear during the night. */
            public double DryingRate;
            /** Number of days that the patch must have been without water, before it can disappear during the night. */
            public int    Delay;
        }

        /** Chance that tilled soil will disappear each night. Normally this is 0.1 (=10%). */
        public DecayConfig EachNight = new DecayConfig() {
            DryingRate = 0.5,
            Delay = 2
        };

        /** Chance that tilled soil will disappear at the start of a new month. Normally this is 0.8 (=80%). */
        public DecayConfig FirstOfMonth = new DecayConfig() {
            DryingRate = 1.0,
            Delay = 1
        };

        /** Chance that tilled soil will disappear inside the greenhouse. Normally this is 1.0 (=100%). */
        public DecayConfig Greenhouse = new DecayConfig() {
            DryingRate = 1.0,
            Delay = 1
        };

        /** Chance that tilled soil will disappear outside the farm. Normally this is 1.0 (=100%). */
        public DecayConfig NonFarm = new DecayConfig() {
            DryingRate = 1.0,
            Delay = 0
        };
    }

    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        public override void HackEntry(IModHelper helper) {
            Patch((Farm f)=>f.DayUpdate(0), Farm_DayUpdate);
            Patch((HoeDirt hd)=>hd.dayUpdate(null, new Vector2()), HoeDirt_dayUpdate);
            Patch((GameLocation gl)=>gl.DayUpdate(0), GameLocation_DayUpdate);
        }

        protected override void InitializeApi(GenericModConfigMenuAPI api)
        {
            api.RegisterLabel(ModManifest, "Each night", "How soil decays every night");
            api.RegisterClampedOption(ModManifest, "Drying Rate", "Chance that tilled soil will disappear. Normally this is 0.1 (=10%).", () => (float)config.EachNight.DryingRate, (float val) => config.EachNight.DryingRate = val, 0.0f, 1.0f);
            api.RegisterClampedOption(ModManifest, "Delay", "Number of days that the patch must have been without water, before it can disappear during the night.", () => config.EachNight.Delay, (int val) => config.EachNight.Delay = val, 0, 4);
            api.RegisterLabel(ModManifest, "First of Month", "How soil decays at the start of a new month");
            api.RegisterClampedOption(ModManifest, "Drying Rate", "Chance that tilled soil will disappear. Normally this is 0.8 (=80%).", () => (float)config.FirstOfMonth.DryingRate, (float val) => config.FirstOfMonth.DryingRate = val, 0.0f, 1.0f);
            api.RegisterClampedOption(ModManifest, "Delay", "Number of days that the patch must have been without water, before it can disappear during the night.", () => config.FirstOfMonth.Delay, (int val) => config.FirstOfMonth.Delay = val, 0, 4);
            api.RegisterLabel(ModManifest, "Greenhouse", "How soil decays inside the greenhouse");
            api.RegisterClampedOption(ModManifest, "Drying Rate", "Chance that tilled soil will disappear. Normally this is 1.0 (=100%).", () => (float)config.Greenhouse.DryingRate, (float val) => config.Greenhouse.DryingRate = val, 0.0f, 1.0f);
            api.RegisterClampedOption(ModManifest, "Delay", "Number of days that the patch must have been without water, before it can disappear during the night.", () => config.Greenhouse.Delay, (int val) => config.Greenhouse.Delay = val, 0, 4);
            api.RegisterLabel(ModManifest, "Non-farm", "How soil decays outside the farm");
            api.RegisterClampedOption(ModManifest, "Drying Rate", "Chance that tilled soil will disappear. Normally this is 1.0 (=100%).", () => (float)config.NonFarm.DryingRate, (float val) => config.NonFarm.DryingRate = val, 0.0f, 1.0f);
            api.RegisterClampedOption(ModManifest, "Delay", "Number of days that the patch must have been without water, before it can disappear during the night.", () => config.NonFarm.Delay, (int val) => config.NonFarm.Delay = val, 0, 4);
        }

        void Farm_DayUpdate() {
            var DayUpdate = BeginCode();

            var hdv = generator.DeclareLocal(typeof(HoeDirt));

            // There are 2 updates. One for normal day transitions and one for the first of the month.
            for (int i=0; i<2; i++) {
                DayUpdate = DayUpdate.FindNext(
                    // is HoeDirt
                    Instructions.Isinst(typeof(HoeDirt)),
                    // .crop == null
                    Instructions.Callvirt_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                    OpCodes.Brtrue,

                    // Game1.random.NextDouble() <= 0.1
                    Instructions.Ldsfld(typeof(Game1), nameof(Game1.random)),
                    OpCodes.Callvirt,
                    OpCodes.Ldc_R8
                );
                DayUpdate.Replace(
                    DayUpdate[0],

                    // Inject && state <= -delay
                    Instructions.Stloc_S(hdv),
                    Instructions.Ldloc_S(hdv),
                    Instructions.Ldfld(typeof(HoeDirt), nameof(HoeDirt.state)),
                    Instructions.Call_get(typeof(NetInt), nameof(NetInt.Value)),
                    Instructions.Call(GetType(), nameof(getConfig)),
                    Instructions.Ldfld(typeof(ModConfig), i == 0 ? nameof(ModConfig.EachNight) : nameof(ModConfig.FirstOfMonth)),
                    Instructions.Ldfld(typeof(ModConfig.DecayConfig), nameof(ModConfig.DecayConfig.Delay)),
                    Instructions.Neg(),
                    Instructions.Bgt((Label)DayUpdate[2].operand),
                    Instructions.Ldloc_S(hdv),

                    // Continue with && crop == null
                    DayUpdate[1],
                    DayUpdate[2],
                    DayUpdate[3],
                    DayUpdate[4],

                    // Set Decay Rate
                    Instructions.Call(GetType(), nameof(getConfig)),
                    Instructions.Ldfld(typeof(ModConfig), i==0 ? nameof(ModConfig.EachNight) : nameof(ModConfig.FirstOfMonth)),
                    Instructions.Ldfld(typeof(ModConfig.DecayConfig), nameof(ModConfig.DecayConfig.DryingRate))
                );
            }
        }

        // To support the decay delay, we will use the HoeDirt.state variable to track how many days the patch has gone without being watered.
        // For example: 'state == -3' indicates that the tile hasn't been watered in the past 3 days. 
        void HoeDirt_dayUpdate() {
            var setcropvalue = FindCode(
                // state.Value = 0;
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(HoeDirt), nameof(HoeDirt.state)),
                OpCodes.Ldc_I4_0,
                OpCodes.Callvirt
            );
            // Replace with state.Value--;
            setcropvalue.Splice(2,1,
                Instructions.Dup(),
                Instructions.Callvirt_get(typeof(NetInt), nameof(NetInt.Value)),
                Instructions.Ldc_I4_1(),
                Instructions.Sub()
            );
        }
        
        static public void remove_non_farm_hoedirt(GameLocation gl, Vector2 pos) {
            ModConfig.DecayConfig decay = gl.IsGreenhouse ? getConfig().Greenhouse : getConfig().NonFarm;

            var hoedirt = gl.terrainFeatures[pos] as HoeDirt;
            // Apply greenhouse delay
            if (-hoedirt.state.Value < decay.Delay) return;
            // Apply greenhouse decay rate
            if (Game1.random.NextDouble() > decay.DryingRate) return;

            // Remove hoedirt
            gl.terrainFeatures.Remove(pos);
        }

        void GameLocation_DayUpdate() {
            var code = FindCode(
                // terrainFeatures.Remove (collection.ElementAt (num4));
                Instructions.Ldarg_0(), // this
                Instructions.Ldfld(typeof(GameLocation), nameof(GameLocation.terrainFeatures)), // .terrainFeatures
                OpCodes.Ldloc_S,  // collection
                OpCodes.Ldloc_S,  // num4
                OpCodes.Call,     // ElementAt()
                OpCodes.Callvirt, // Remove()
                OpCodes.Pop
            );
            code.Replace(
                code[0],
                code[2],
                code[3],
                code[4],
                Instructions.Call(GetType(), nameof(remove_non_farm_hoedirt), typeof(GameLocation), typeof(Vector2))
            );
        }
    }
}

