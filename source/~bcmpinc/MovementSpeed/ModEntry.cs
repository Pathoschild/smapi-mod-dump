/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using System;
using System.Reflection;
using System.Reflection.Emit;
using StardewValley;

namespace StardewHack.MovementSpeed
{
    
    public class ModConfig {
        /** The movement speed is multiplied by this amount. The mod's default is 1.5, meaning 50% faster movement. Set this to 1 to disable the increase in movement speed. */
        public float MovementSpeedMultiplier = 1.5f;
        /** Time required for charging the hoe or watering can in ms. Normally this is 600ms. The default is 600/1.5 = 400, meaning 50% faster charging. Set this to 600 to disable faster tool charging. */
        public int ToolChargeDelay = 400;
    }
    
    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        public override void HackEntry(IModHelper helper) {
            // If movement speed is different than the game's default.
            if (Math.Abs(config.MovementSpeedMultiplier - 1) > 1e-3) {
                Patch((Farmer f)=>f.getMovementSpeed(), Farmer_getMovementSpeed);
            }
            
            // If the configured charge time is different than the game's default.
            if (config.ToolChargeDelay != 600) {
                Patch(typeof(Game1), "UpdateControlInput", Game1_UpdateControlInput);
            }
        }

        protected override void InitializeApi(GenericModConfigMenuAPI api)
        {
            api.RegisterClampedOption(ModManifest, "Movement Speed Multiplier", "The movement speed is multiplied by this amount. The mod's default is 1.5, meaning 50% faster movement. Set this to 1 to disable the increase in movement speed.", () => config.MovementSpeedMultiplier, (float val) => config.MovementSpeedMultiplier = val, 0, 5);
            api.RegisterClampedOption(ModManifest, "Tool Charge Delay", "Time required for charging the hoe or watering can in ms. Normally this is 600ms. The default is 600/1.5 = 400, meaning 50% faster charging. Set this to 600 to disable faster tool charging.", () => config.ToolChargeDelay, (int val) => config.ToolChargeDelay = val, 100, 600);
        }

        static float getMovementSpeedMultiplier() => getInstance().config.MovementSpeedMultiplier;
        static int getToolChargeDelay() => getInstance().config.ToolChargeDelay;

        // Add a multiplier to the movement speed.
        void Farmer_getMovementSpeed() {
            var code = FindCode(
                // movementMultiplier = 0.066f;
                OpCodes.Ldarg_0,
                OpCodes.Ldc_R4,
                Instructions.Stfld(typeof(Farmer), nameof(Farmer.movementMultiplier))
            );
            code.Insert(2,
                Instructions.Call(GetType(), nameof(getMovementSpeedMultiplier)),
                Instructions.Mul()
            );
        }

        // Change (reduce) the time it takses to charge tools (hoe & water can).
        void Game1_UpdateControlInput() {
            try {
                Game1_UpdateControlInput_Chain();
            } catch (Exception err) {
                LogException(err, LogLevel.Trace);
                
                // The PC version of StardewModdingAPI changed this method and moved its original code into a delegate, hence the chain patching.
                MethodInfo method = (MethodInfo)FindCode(
                    OpCodes.Ldftn
                )[0].operand;
                ChainPatch(method, AccessTools.Method(typeof(ModEntry), nameof(Game1_UpdateControlInput_Chain)));
            }
        }

        void Game1_UpdateControlInput_Chain() {
            // Game1.player.toolHold = (int)(600f * num4);
            FindCode(
                Instructions.Call_get(typeof(StardewValley.Game1), nameof(StardewValley.Game1.player)),
                Instructions.Ldc_R4(600f),
                OpCodes.Ldloc_S,
                OpCodes.Mul,
                OpCodes.Conv_I4,
                Instructions.Stfld(typeof(Farmer), nameof(Farmer.toolHold))
            )[1] = Instructions.Call(GetType(), nameof(getToolChargeDelay));
        }
    }
}

