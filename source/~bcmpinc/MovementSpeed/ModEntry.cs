using Harmony;
using StardewModdingAPI;
using System;
using System.Reflection;
using System.Reflection.Emit;
using StardewValley;

namespace StardewHack.MovementSpeed
{
    
    public class ModConfig {
        /** The movement speed is multiplied by this amount. The default is 1.5, meaning 50% faster movement. */
        public double MovementSpeedMultiplier = 1.5;
        /** Time required for charging the hoe or watering can in ms. Normally this is 600ms. The default is 600/1.5 = 400, meaning 50% faster charging. */
        public int ToolChargeDelay = 400;
    }
    
    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        public bool ChangesMovementSpeed () {
            return Math.Abs(config.MovementSpeedMultiplier - 1) > 1e-3;
        }

        // Add a multiplier to the movement speed.
        [BytecodePatch("StardewValley.Farmer::getMovementSpeed", "ChangesMovementSpeed")]
        void Farmer_getMovementSpeed() {
            FindCodeLast(
                OpCodes.Ret
            ).Prepend(
                Instructions.Ldc_R8(config.MovementSpeedMultiplier),
                Instructions.Mul()
            );
        }

        public bool ChangesToolChargeDelay() {
            return config.ToolChargeDelay != 600;
        }

        // Change (reduce) the time it takses to charge tools (hoe & water can).
        [BytecodePatch("StardewValley.Game1::UpdateControlInput", "ChangesToolChargeDelay")]
        void Game1_UpdateControlInput() {
            // StardewModdingAPI changed this method and moved its original code into a delegate, hence the chain patching.
            MethodInfo method = (MethodInfo)FindCode(
                OpCodes.Ldftn
            )[0].operand;
            ChainPatch(method, AccessTools.Method(typeof(ModEntry), nameof(Game1_UpdateControlInput_Chain)));
        }

        void Game1_UpdateControlInput_Chain() {
            FindCode(
                Instructions.Ldc_I4(600),
                Instructions.Stfld(typeof(Farmer), nameof(Farmer.toolHold))
            )[0].operand = config.ToolChargeDelay;
        }
    }
}

