/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using System;
using System.Reflection.Emit;
using GenericModConfigMenu;
using StardewValley;

namespace StardewHack.GrassGrowth
{
    public class ModConfig {
        /** Whether grass growth & spreading should be suppressed entirely.*/
        public bool DisableGrowth = false;
        /** Whether grass spreads almost everywhere. If false, grass spreading is limited to tillable tiles.*/
        public bool GrowEverywhere = true;
        /** The chance that grass grows or spreads.*/
        public float GrowthChance = 0.65f;
        /** The chance for each neighbouring tile that the grass will spread there.*/
        public float SpreadChance = 0.25f;
        /** The number of iterations that grass growth is applied per day (max=10).*/
        public int DailyGrowth = 1;
        /** Additional iterations that grass growth is applied at the start of each month (max=100).*/
        public int MonthlyGrowth = 40;
    }

    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        public override void HackEntry(StardewModdingAPI.IModHelper helper) {
            I18n.Init(helper.Translation);

            // Sanitize config.
            if (config.GrowthChance < 1e-6 || (config.DailyGrowth==0 && config.MonthlyGrowth==0)) {
                config.DisableGrowth = true;
            }
            if (config.DailyGrowth > 10) config.DailyGrowth = 10;
            if (config.MonthlyGrowth > 100) config.MonthlyGrowth = 100;
            
            Patch((GameLocation gl) => gl.HandleGrassGrowth(0), GameLocation_HandleGrassGrowth);
            Patch((GameLocation gl) => gl.growWeedGrass(0), GameLocation_growWeedGrass);
        }

        protected override void InitializeApi(IGenericModConfigMenuApi api)
        {
            api.AddBoolOption(mod: ModManifest, name: I18n.DisableGrowthName,  tooltip: I18n.DisableGrowthTooltip,  getValue: () => config.DisableGrowth,  setValue: (bool val)  => config.DisableGrowth  = val);
            api.AddBoolOption(mod: ModManifest, name: I18n.GrowEverywhereName, tooltip: I18n.GrowEverywhereTooltip, getValue: () => config.GrowEverywhere, setValue: (bool val)  => config.GrowEverywhere = val);
            api.AddNumberOption(mod: ModManifest, name: I18n.GrowthChanceName,  tooltip: I18n.GrowthChanceTooltip,  getValue: () => config.GrowthChance,   setValue: (float val) => config.GrowthChance   = val, min:0, max:1);
            api.AddNumberOption(mod: ModManifest, name: I18n.SpreadChanceName,  tooltip: I18n.SpreadChanceTooltip,  getValue: () => config.SpreadChance,   setValue: (float val) => config.SpreadChance   = val, min:0, max:1);
            api.AddNumberOption(mod: ModManifest, name: I18n.DailyGrowthName,   tooltip: I18n.DailyGrowthTooltip,   getValue: () => config.DailyGrowth,    setValue: (int val)   => config.DailyGrowth    = val, min:0, max:10);
            api.AddNumberOption(mod: ModManifest, name: I18n.MonthlyGrowthName, tooltip: I18n.MonthlyGrowthTooltip, getValue: () => config.MonthlyGrowth,  setValue: (int val)   => config.MonthlyGrowth  = val, min:0, max:100);
        }

        static int getMonthlyGrowth() => getInstance().config.MonthlyGrowth;
        static int getDailyGrowth() => getInstance().config.DailyGrowth;

        // Change the rate at which new grass spawns during the night. 
        void GameLocation_HandleGrassGrowth() {
            var code = FindCode(
                // growWeedGrass(40);
                OpCodes.Ldarg_0,
                OpCodes.Ldc_I4_S,
                Instructions.Call(typeof(GameLocation), nameof(GameLocation.growWeedGrass), typeof(int))
            );
            code[1] = Instructions.Call(GetType(), nameof(getMonthlyGrowth));
            code = code.FindNext(        
                // growWeedGrass(1);
                OpCodes.Ldarg_0,
                OpCodes.Ldc_I4_1,
                Instructions.Call(typeof(GameLocation), nameof(GameLocation.growWeedGrass), typeof(int)),
                OpCodes.Ret
            );
            code[1] = Instructions.Call(GetType(), nameof(getDailyGrowth));
        }
        
        static bool getDisableGrowth() => getInstance().config.DisableGrowth;
        static bool getGrowEverywhere() => getInstance().config.GrowEverywhere;
        static double getGrowthChance() => getInstance().config.GrowthChance;
        static double getSpreadChance() => getInstance().config.SpreadChance;
        
        // Change the behavior of the grass growth & spreading. 
        void GameLocation_growWeedGrass() {
            // Stop grass from growing & spreading.
            AllCode().Prepend(
                Instructions.Call(GetType(), nameof(getDisableGrowth)),
                Instructions.Brfalse(AttachLabel(AllCode()[0])),
                Instructions.Ret()
            );
            
            // Change grass growth to spread mostly everywhere.
            var growWeedGrass = BeginCode();
            // For each of the 4 directions
            for (int i=0; i<4; i++) {
                growWeedGrass = growWeedGrass.FindNext(
                    OpCodes.Ldarg_0,
                    null,
                    null,
                    null,
                    null,
                    Instructions.Ldstr("Diggable"),
                    Instructions.Ldstr("Back"),
                    Instructions.Call(typeof(GameLocation), nameof(GameLocation.doesTileHaveProperty), typeof(int), typeof(int), typeof(string), typeof(string)),
                    OpCodes.Brfalse
                );
                growWeedGrass.Prepend(
                    Instructions.Call(GetType(), nameof(getGrowEverywhere)),
                    Instructions.Brtrue(AttachLabel(growWeedGrass.End[0]))
                );
            }
            
            // Growth chance
            FindCode(
                Instructions.Ldc_R8(0.65),
                OpCodes.Bge_Un,
                OpCodes.Ldloca_S
            )[0] = Instructions.Call(GetType(), nameof(getGrowthChance));
            
            // Spread
            var spreadGrass = BeginCode();
            // For each of the 4 directions
            for (int i=0; i<4; i++) {
                spreadGrass = spreadGrass.FindNext(
                    Instructions.Ldc_R8(0.25),
                    OpCodes.Bge_Un,
                    OpCodes.Ldarg_0
                );
                spreadGrass[0] = Instructions.Call(GetType(), nameof(getSpreadChance));
            }
        }
    }
}

