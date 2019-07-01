using System;
using System.Reflection.Emit;

namespace StardewHack.GrassGrowth
{
    public class ModConfig {
        /** Whether grass growth & spreading should be suppressed entirely.*/
        public bool DisableGrowth = false;
        /** Whether grass spreads almost everywhere. If false, grass spreading is limited to digable tiles.*/
        public bool GrowEverywhere = true;
        /** The chance that grass growth or spreads.*/
        public double GrowthChance = 0.65;
        /** The chance for each neighbouring tile that the grass will spreads there.*/
        public double SpreadChance = 0.25;
        /** The number of iterations that grass growth is applied per day (max=10).*/
        public int DailyGrowth = 1;
        /** Additional iterations that grass growth is applied at the start of each month (max=100).*/
        public int MonthlyGrowth = 40;
    }

    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        public override void Entry(StardewModdingAPI.IModHelper helper) {
            base.Entry(helper);

            // Sanitize config.
            if (config.SpreadChance < 1e-6 || config.DisableGrowth) {
                config.GrowEverywhere = false;
            }
            if (config.GrowthChance < 1e-6 || (config.DailyGrowth==0 && config.MonthlyGrowth==0)) {
                config.DisableGrowth = true;
            }
            if (config.DailyGrowth > 10) config.DailyGrowth = 10;
            if (config.MonthlyGrowth > 100) config.MonthlyGrowth = 100;
        }
    
        public bool IterationsChanged() {
            return config.DailyGrowth != 1 || config.MonthlyGrowth != 40;
        }
    
        // Change the rate at which new grass spawns during the night. 
        [BytecodePatch("StardewValley.Farm::DayUpdate", "IterationsChanged")]
        void Farm_DayUpdate() {
            var code = FindCode(
                OpCodes.Ldarg_0,
                OpCodes.Ldc_I4_S,
                Instructions.Call(typeof(StardewValley.GameLocation), "growWeedGrass", typeof(int)),
                OpCodes.Ldarg_0,
                OpCodes.Ldc_I4_1,
                Instructions.Call(typeof(StardewValley.GameLocation), "growWeedGrass", typeof(int)),
                OpCodes.Ret
            );
            if (config.MonthlyGrowth != 40) {
                code[1] = Instructions.Ldc_I4(config.MonthlyGrowth);
            }
            if (config.DailyGrowth != 1) {
                code[4] = Instructions.Ldc_I4(config.DailyGrowth);
            }
        }
        
        // Change the behavior of the grass growth & spreading. 
        [BytecodePatch("StardewValley.GameLocation::growWeedGrass")]
        void GameLocation_growWeedGrass() {
            // Stop grass from growing & spreading.
            if (config.DisableGrowth) {
                AllCode().Replace(
                    Instructions.Ret()
                );
                return;
            } 
            
            // Change grass growth to spread mostly everywhere.
            if (config.GrowEverywhere) {
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
                        Instructions.Call(typeof(StardewValley.GameLocation), "doesTileHaveProperty", typeof(int), typeof(int), typeof(string), typeof(string)),
                        OpCodes.Brfalse
                    );
                    growWeedGrass.Remove();
                }
            }
            
            // Growth chance
            if (Math.Abs(config.GrowthChance - 0.65) < 1e-6) {
                FindCode(
                    Instructions.Ldc_R8(0.65),
                    OpCodes.Bge_Un,
                    OpCodes.Ldloca_S
                )[0].operand = config.GrowthChance;
            }
            
            // Spread
            if (Math.Abs(config.SpreadChance - 0.25) < 1e-6) {
                var spreadGrass = BeginCode();
                // For each of the 4 directions
                for (int i=0; i<4; i++) {
                    spreadGrass = spreadGrass.FindNext(
                        Instructions.Ldc_R8(0.25),
                        OpCodes.Bge_Un,
                        OpCodes.Ldarg_0
                    );
                    spreadGrass[0].operand = config.SpreadChance;
                }
            }
        }
    }
}

