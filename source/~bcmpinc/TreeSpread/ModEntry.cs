using System.Reflection.Emit;

namespace StardewHack.TreeSpread
{
     public class ModConfig
    {
        /** Chance that a tree will have a seed. Normally this is 0.05 (=5%). */
        public double SeedChance = 0.15;
        /** Whether only tapped trees are prevented from dropping seeds. */
        public bool OnlyPreventTapped = false;
    }

    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        [BytecodePatch("StardewValley.TerrainFeatures.Tree::dayUpdate")]
        void Tree_DayUpdate() {
            // Erase code related to trea spreading.
            var spread = FindCode(
                // if ((int)growthStage >= 5 
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(StardewValley.TerrainFeatures.Tree), "growthStage"),
                OpCodes.Call,
                OpCodes.Ldc_I4_5,
                OpCodes.Blt,
                // && environment is Farm ...
                OpCodes.Ldarg_1,
                Instructions.Isinst(typeof(StardewValley.Farm)),
                OpCodes.Brfalse
            );
            spread.Extend(
                // hasSeed.Value = false;
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(StardewValley.TerrainFeatures.Tree), "hasSeed"),
                OpCodes.Ldc_I4_0,
                OpCodes.Callvirt
            );
            spread.length -= 4;
            if (config.OnlyPreventTapped) {
                spread.Prepend(
                    // if (!tapped)
                    Instructions.Ldarg_0(),
                    Instructions.Ldfld(typeof(StardewValley.TerrainFeatures.Tree), "tapped"),
                    Instructions.Call_get(typeof(Netcode.NetBool), "Value"),
                    Instructions.Brtrue(AttachLabel(spread.End[0]))
                );
                spread = spread.End;
            } else {
                spread.Remove();
                
                // Don't remove seeds when on the farm
                // if (!environment is Farm)
                spread.Append(
                    Instructions.Ldarg_1(),
                    Instructions.Isinst(typeof(StardewValley.Farm)),
                    Instructions.Brtrue(AttachLabel(spread[4]))
                );
            }
            
            // Increase chance that tree has a seed.
            var seed = spread.FindNext(
                OpCodes.Ldsfld,
                OpCodes.Callvirt,
                OpCodes.Ldc_R8,
                OpCodes.Bge_Un
            );
            seed[2].operand = config.SeedChance;
        }
    }
}

