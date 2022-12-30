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
using GenericModConfigMenu;

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

        /** How soil decays on the farm (and maps that have the `ClearEmptyDirtOnNewMonth` property) during nights between seasons. This is in addition to the normal decay. Normally this is 0.8 (=80%). */
        public DecayConfig EachSeason = new DecayConfig() {
            DryingRate = 1.0,
            Delay = 1
        };

        /** Chance that tilled soil will disappear inside the greenhouse. Normally this is 1.0 (=100%). */
        public DecayConfig Greenhouse = new DecayConfig() {
            DryingRate = 1.0,
            Delay = 1
        };

        /** Chance that tilled soil will disappear on ginger island. Normally this is 0.1 (=10%). */
        public DecayConfig Island = new DecayConfig() {
            DryingRate = 0.5,
            Delay = 2
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
            Patch((Farm f) => f.DayUpdate(0), Farm_DayUpdate);
            Patch((GameLocation gl) => gl.HandleGrassGrowth(1), GameLocation_HandleGrassGrowth);
            Patch((StardewValley.Locations.IslandWest iw) => iw.DayUpdate(0), IslandWest_DayUpdate);
            Patch((HoeDirt hd) => hd.dayUpdate(null, new Vector2()), HoeDirt_dayUpdate);
            Patch((GameLocation gl) => gl.DayUpdate(0), GameLocation_DayUpdate);
        }

        protected override void InitializeApi(IGenericModConfigMenuApi api)
        {
            api.AddSectionTitle(mod: ModManifest, text: () => "Each night",  tooltip: () => "How soil decays every night");
            api.AddNumberOption(mod: ModManifest, name: () => "Drying Rate", tooltip: () => "Chance that tilled soil will disappear. Normally this is 0.1 (=10%).", getValue: () => (float)config.EachNight.DryingRate, setValue: (float val) => config.EachNight.DryingRate = val, min: 0.0f, max: 1.0f);
            api.AddNumberOption(mod: ModManifest, name: () => "Delay",       tooltip: () => "Number of consecutive days that the patch must have been without water, before it can disappear during the night.", getValue: () => config.EachNight.Delay, setValue: (int val) => config.EachNight.Delay = val, min: 0, max: 4);
            api.AddSectionTitle(mod: ModManifest, text: () => "Each Season", tooltip: () => "How soil decays on the farm (and maps that have the `ClearEmptyDirtOnNewMonth` property) during nights between seasons");
            api.AddNumberOption(mod: ModManifest, name: () => "Drying Rate", tooltip: () => "Chance that tilled soil will disappear. Normally this is 0.8 (=80%).", getValue: () => (float)config.EachSeason.DryingRate, setValue: (float val) => config.EachSeason.DryingRate = val, min: 0.0f, max: 1.0f);
            api.AddNumberOption(mod: ModManifest, name: () => "Delay",       tooltip: () => "Number of consecutive days that the patch must have been without water, before it can disappear during the night.", getValue: () => config.EachSeason.Delay, setValue: (int val) => config.EachSeason.Delay = val, min: 0, max: 4);
            api.AddSectionTitle(mod: ModManifest, text: () => "Greenhouse",  tooltip: () => "How soil decays inside the greenhouse");
            api.AddNumberOption(mod: ModManifest, name: () => "Drying Rate", tooltip: () => "Chance that tilled soil will disappear. Normally this is 1.0 (=100%).", getValue: () => (float)config.Greenhouse.DryingRate, setValue: (float val) => config.Greenhouse.DryingRate = val, min: 0.0f, max: 1.0f);
            api.AddNumberOption(mod: ModManifest, name: () => "Delay",       tooltip: () => "Number of consecutive days that the patch must have been without water, before it can disappear during the night.", getValue: () => config.Greenhouse.Delay, setValue: (int val) => config.Greenhouse.Delay = val, min: 0, max: 4);
            api.AddSectionTitle(mod: ModManifest, text: () => "Island",      tooltip: () => "How soil decays on Ginger Island");
            api.AddNumberOption(mod: ModManifest, name: () => "Drying Rate", tooltip: () => "Chance that tilled soil will disappear. Normally this is 0.1 (=10%).", getValue: () => (float)config.Island.DryingRate, setValue: (float val) => config.Island.DryingRate = val, min: 0.0f, max: 1.0f);
            api.AddNumberOption(mod: ModManifest, name: () => "Delay",       tooltip: () => "Number of consecutive days that the patch must have been without water, before it can disappear during the night.", getValue: () => config.Island.Delay, setValue: (int val) => config.Island.Delay = val, min: 0, max: 4);
            api.AddSectionTitle(mod: ModManifest, text: () => "Non-farm",    tooltip: () => "How soil decays outside the farm");
            api.AddNumberOption(mod: ModManifest, name: () => "Drying Rate", tooltip: () => "Chance that tilled soil will disappear. Normally this is 1.0 (=100%).", getValue: () => (float)config.NonFarm.DryingRate, setValue: (float val) => config.NonFarm.DryingRate = val, min: 0.0f, max: 1.0f);
            api.AddNumberOption(mod: ModManifest, name: () => "Delay",       tooltip: () => "Number of consecutive days that the patch must have been without water, before it can disappear during the night.", getValue: () => config.NonFarm.Delay, setValue: (int val) => config.NonFarm.Delay = val, min: 0, max: 4);
        }

        void Farm_DayUpdate() {
            var hdv = generator.DeclareLocal(typeof(HoeDirt));

            // Inject code to check if soil is sufficiently dried out.
            var DayUpdate = FindCode(
                // if (!terrainFeatures[key] is HoeDirt)
                OpCodes.Ldarg_0,
                OpCodes.Ldfld, //Instructions.Ldfld(typeof(GameLocation), nameof(GameLocation.terrainFeatures)),
                OpCodes.Ldloc_S,
                OpCodes.Callvirt,
                Instructions.Isinst(typeof(HoeDirt)),
                OpCodes.Brfalse
            );

            DayUpdate.Replace(
                // if (terrainFeatures[key] as HoeDirt).state > -ModConfig.DecayConfig.getFarmConfig(dayOfMonth).DryingDelay
                DayUpdate[0],
                DayUpdate[1],
                DayUpdate[2],
                DayUpdate[3],
                DayUpdate[4],
                Instructions.Stloc_S(hdv),
                Instructions.Ldloc_S(hdv),
                DayUpdate[5],

                Instructions.Ldloc_S(hdv),
                Instructions.Ldfld(typeof(HoeDirt), nameof(HoeDirt.state)),
                Instructions.Call_get(typeof(NetInt), nameof(NetInt.Value)),
                Instructions.Call(GetType(), nameof(getConfig)),
                Instructions.Ldfld(typeof(ModConfig), nameof(ModConfig.EachNight)),
                Instructions.Ldfld(typeof(ModConfig.DecayConfig), nameof(ModConfig.DecayConfig.Delay)),
                Instructions.Neg(),
                Instructions.Bgt((Label)DayUpdate[5].operand)
            );

            DayUpdate = DayUpdate.FindNext(
                // Game1.random.NextDouble() <= 0.1
                Instructions.Ldsfld(typeof(Game1), nameof(Game1.random)),
                OpCodes.Callvirt,
                OpCodes.Ldc_R8
            );
            DayUpdate.Replace(
                // Game1.random.NextDouble() <= ModConfig.DecayConfig.getFarmConfig(dayOfMonth).DryingRate
                DayUpdate[0],
                DayUpdate[1],

                // Set Decay Rate
                Instructions.Call(GetType(), nameof(getConfig)),
                Instructions.Ldfld(typeof(ModConfig), nameof(ModConfig.EachNight)),
                Instructions.Ldfld(typeof(ModConfig.DecayConfig), nameof(ModConfig.DecayConfig.DryingRate))
            );
        }

        void GameLocation_HandleGrassGrowth() {
            var hdv = generator.DeclareLocal(typeof(HoeDirt));
            var DayUpdate = FindCode(
                // is HoeDirt
                Instructions.Isinst(typeof(HoeDirt)),
                // .crop == null
                Instructions.Callvirt_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                OpCodes.Brtrue,

                // Game1.random.NextDouble() <= 0.8
                Instructions.Ldsfld(typeof(Game1), nameof(Game1.random)),
                OpCodes.Callvirt,
                OpCodes.Ldc_R8
            );
            DayUpdate.Replace(
                DayUpdate[0],
                Instructions.Stloc_S(hdv),
                Instructions.Ldloc_S(hdv),

                // Inject && state <= -delay
                Instructions.Ldfld(typeof(HoeDirt), nameof(HoeDirt.state)),
                Instructions.Call_get(typeof(NetInt), nameof(NetInt.Value)),
                Instructions.Call(GetType(), nameof(getConfig)),
                Instructions.Ldfld(typeof(ModConfig), nameof(ModConfig.EachSeason)),
                Instructions.Ldfld(typeof(ModConfig.DecayConfig), nameof(ModConfig.DecayConfig.Delay)),
                Instructions.Neg(),
                Instructions.Bgt((Label)DayUpdate[2].operand),

                Instructions.Ldloc_S(hdv),
                DayUpdate[1],
                DayUpdate[2],

                // Game1.random.NextDouble() <= getConfig().EachSeason.DryingRate
                DayUpdate[3],
                DayUpdate[4],
                // Set Decay Rate
                Instructions.Call(GetType(), nameof(getConfig)),
                Instructions.Ldfld(typeof(ModConfig), nameof(ModConfig.EachSeason)),
                Instructions.Ldfld(typeof(ModConfig.DecayConfig), nameof(ModConfig.DecayConfig.DryingRate))
            );
        }

        void IslandWest_DayUpdate() {
            var hdv = generator.DeclareLocal(typeof(HoeDirt));

            var DayUpdate = FindCode(
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
                Instructions.Ldfld(typeof(ModConfig), nameof(ModConfig.Island)),
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
                Instructions.Ldfld(typeof(ModConfig), nameof(ModConfig.Island)),
                Instructions.Ldfld(typeof(ModConfig.DecayConfig), nameof(ModConfig.DecayConfig.DryingRate))
            );
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

