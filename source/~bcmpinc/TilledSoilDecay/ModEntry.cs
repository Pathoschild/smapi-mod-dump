using StardewModdingAPI;
using System;
using System.Reflection.Emit;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewHack.TilledSoilDecay
{
    public class ModConfig
    {
        /** Amount of tilled soil that will disappear. Normally this is 0.1 (=10%). */
        public double DryDecayRate = 0.5;

        /** Amount of tilled soil that will disappear at the start of a new month. Normally this is 0.8 (=80%). */
        public double DryDecayRateFirstOfMonth = 1;

        /** Number of days that the patch must have been without water, before it can disappear during the night. */
        public int DecayDelay = 2;

        /** Number of days that the patch must have been without water, before it can disappear during the night at the end of the month. */
        public int DecayDelayFirstOfMonth = 1;
    }

    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        [BytecodePatch("StardewValley.Farm::DayUpdate")]
        void Farm_DayUpdate() {
            //var crop = GetField("StardewValley.TerrainFeatures.HoeDirt::crop");
            //var state = GetField("StardewValley.TerrainFeatures.HoeDirt::state");
            var DayUpdate = BeginCode();

            var hdv = generator.DeclareLocal(typeof(HoeDirt));

            // There are 2 updates. One for normal day transitions and one for the first of the month.
            for (int i=0; i<2; i++) {
                DayUpdate = DayUpdate.FindNext(
                    Instructions.Isinst(typeof(HoeDirt)),
                    Instructions.Callvirt_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                    OpCodes.Brtrue,
                    Instructions.Ldsfld(typeof(Game1), nameof(Game1.random)),
                    OpCodes.Callvirt,
                    OpCodes.Ldc_R8
                );
                // Set Decay Rate
                DayUpdate[5].operand = (i==0 ? config.DryDecayRate : config.DryDecayRateFirstOfMonth);

                int delay = (i==0 ? config.DecayDelay : config.DecayDelayFirstOfMonth);
                if (delay > 0) {
                    DayUpdate.Insert(1,
                        // Inject && state <= delay
                        Instructions.Stloc_S(hdv),
                        Instructions.Ldloc_S(hdv),
                        Instructions.Ldfld(typeof(HoeDirt), nameof(HoeDirt.state)),
                        Instructions.Call_get(typeof(NetInt), nameof(NetInt.Value)),
                        Instructions.Ldc_I4(-delay),
                        Instructions.Bgt((Label)DayUpdate[2].operand),
                        Instructions.Ldloc_S(hdv)
                        // Continue with && crop == null
                    );
                }
            }
        }

        public bool UsesDelays() {
            return config.DecayDelay > 0 || config.DecayDelayFirstOfMonth > 0;
        }

        // To support the decay delay, we will use the HoeDirt.state variable to track how many days the patch has gone without being watered.
        // For example: 'state == -3' indicates that the tile hasn't been watered in the past 3 days. 
        [BytecodePatch("StardewValley.TerrainFeatures.HoeDirt::dayUpdate", "UsesDelays")]
        void HoeDirt_dayUpdate() {
            // Find: if (crop != null)
            var dayUpdate = BeginCode();
            var begin = AttachLabel(dayUpdate[0]);
            dayUpdate.Prepend(
                // if (crop == null 
                Instructions.Ldarg_0(),
                Instructions.Callvirt_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                Instructions.Brtrue(begin),
                //   && state <= 0) {
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(HoeDirt), nameof(HoeDirt.state)),
                Instructions.Call_get(typeof(NetInt), nameof(NetInt.Value)),
                Instructions.Ldc_I4_0(),
                Instructions.Bgt(begin),
                //   state--;
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(HoeDirt), nameof(HoeDirt.state)),
                Instructions.Dup(),
                Instructions.Call_get(typeof(NetInt), nameof(NetInt.Value)),
                Instructions.Ldc_I4_1(),
                Instructions.Sub(),
                Instructions.Call_set(typeof(NetInt), nameof(NetInt.Value)),
                //   return;
                Instructions.Ret()
                // }
            );
        }
    }
}

