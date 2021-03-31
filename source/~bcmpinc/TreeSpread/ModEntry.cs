/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using System.Reflection.Emit;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewHack.TreeSpread
{
    public class ModConfig
    {
        /** Chance that a tree will have a seed. Normally this is 0.05 (=5%). */
        public float SeedChance = 0.15f;
        /** Whether only tapped trees are prevented from spreading. */
        public bool OnlyPreventTapped = false;
        /** Whether the tree should keep its seed during the night, to compensate for trees not spreading. Vanilla SDV removes seeds during the night. */
        public bool RetainSeed = true;
    }

    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        public override void HackEntry(IModHelper helper) {
            Patch((Tree t) => t.dayUpdate(null, new Microsoft.Xna.Framework.Vector2()), Tree_DayUpdate);
        }

        protected override void InitializeApi(GenericModConfigMenuAPI api)
        {
            api.RegisterClampedOption(ModManifest, "Seed Chance", "Chance that a tree will have a seed. Normally this is 0.05 (=5%).", () => config.SeedChance, (float val) => config.SeedChance = val, 0, 1);
            api.RegisterSimpleOption(ModManifest, "Only Prevent Tapped", "Whether only tapped trees are prevented from spreading.", () => config.OnlyPreventTapped, (bool val) => config.OnlyPreventTapped = val);
            api.RegisterSimpleOption(ModManifest, "Retain Seed", "Whether the tree should keep its seed during the night, to compensate for trees not spreading. Vanilla SDV removes seeds during the night.", () => config.RetainSeed, (bool val) => config.RetainSeed = val);
        }

        static float getSeedChance() => getInstance().config.SeedChance;
        static bool isOnlyPreventTapped() => getInstance().config.OnlyPreventTapped;
        static bool isRetainSeed() => getInstance().config.RetainSeed;

        void Tree_DayUpdate() {
            // Erase code related to tree spreading.
            var spread = FindCode(
                // if ((int)growthStage >= 5 
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(Tree), nameof(Tree.growthStage)),
                OpCodes.Call,
                OpCodes.Ldc_I4_5,
                OpCodes.Blt,
                // && environment is Farm ...
                OpCodes.Ldarg_1,
                Instructions.Isinst(typeof(Farm)),
                OpCodes.Brfalse
            );
            spread.Extend(
                // hasSeed.Value = false;
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(Tree), nameof(Tree.hasSeed)),
                OpCodes.Ldc_I4_0,
                OpCodes.Callvirt
            );
            spread.length -= 4;

            spread.Prepend(
                // if (!tapped && !config.OnlyPreventTapped)
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(Tree), nameof(Tree.tapped)),
                Instructions.Call_get(typeof(NetBool), nameof(NetBool.Value)),
                Instructions.Brtrue(AttachLabel(spread.End[0])),
                Instructions.Call(GetType(), nameof(isOnlyPreventTapped)),
                Instructions.Brtrue(AttachLabel(spread.End[0]))
            );
            spread.ReplaceJump(6, spread[0]);

            // if RetainSeed: Don't remove seeds when on the farm 
            var remove_seed = spread.End;
            remove_seed.length += 4;

            // if (!config.RetainSeed || !environment is Farm)
            var lbl = generator.DefineLabel();
            remove_seed.Prepend(
                Instructions.Call(GetType(), nameof(isRetainSeed)),
                Instructions.Brfalse(lbl),
                Instructions.Ldarg_1(),
                Instructions.Isinst(typeof(Farm)),
                Instructions.Brtrue(AttachLabel(remove_seed.End[0]))
            );
            remove_seed.ReplaceJump(5, remove_seed[0]);
            remove_seed[5].labels.Add(lbl);

            // Increase chance that tree has a seed.
            // float num3 = 0.05f;
            var seed = remove_seed.FindNext(
                Instructions.Ldc_R4(0.05f),
                OpCodes.Stloc_1
            );
            seed[0] = Instructions.Call(GetType(), nameof(getSeedChance));
        }
    }
}

