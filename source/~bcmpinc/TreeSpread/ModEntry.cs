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
using GenericModConfigMenu;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.WildTrees;
using StardewValley.TerrainFeatures;
using static HarmonyLib.Code;

namespace StardewHack.TreeSpread
{
    public class ModConfig
    {
        /** Chance that a tree will have a seed is multiplied by this value. Normally this is 5%. */
        public float SeedChanceMultiplier = 3.0f;
        /** Whether only tapped trees are prevented from spreading. */
        public bool OnlyPreventTapped = false;
        /** Whether the tree should keep its seed during the night, to compensate for trees not spreading. Vanilla SDV removes seeds during the night. */
        public bool RetainSeed = true;
    }

    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        public override void HackEntry(IModHelper helper) {
            I18n.Init(helper.Translation);
            Patch((Tree t) => t.dayUpdate(), Tree_DayUpdate);
        }

        protected override void InitializeApi(IGenericModConfigMenuApi api)
        {
            api.AddNumberOption(mod: ModManifest, name: I18n.SeedChanceMultiplierName,  tooltip: I18n.SeedChanceMultiplierTooltip, getValue: () => config.SeedChanceMultiplier, setValue: (float val) => config.SeedChanceMultiplier = val, min: 1, max: 10);
            api.AddBoolOption  (mod: ModManifest, name: I18n.OnlyPreventTappedName,     tooltip: I18n.OnlyPreventTappedTooltip,    getValue: () => config.OnlyPreventTapped,    setValue: (bool val)  => config.OnlyPreventTapped    = val);
            api.AddBoolOption  (mod: ModManifest, name: I18n.RetainSeedName,            tooltip: I18n.RetainSeedTooltip,           getValue: () => config.RetainSeed,           setValue: (bool val)  => config.RetainSeed           = val);
        }

        static float getSeedChanceMultiplier() => getInstance().config.SeedChanceMultiplier;
        static bool isOnlyPreventTapped() => getInstance().config.OnlyPreventTapped;
        static bool isRetainSeed() => getInstance().config.RetainSeed;

        void Tree_DayUpdate() {
            // Erase code related to tree spreading.
            var spread = FindCode(
                // Game1.random.NextBool(data?.SeedSpreadChance ?? 0.15f)
                Instructions.Ldsfld(typeof(Game1), nameof(Game1.random)),
                OpCodes.Ldloc_1,
                OpCodes.Brtrue_S,
                OpCodes.Ldc_R4,
                OpCodes.Br_S,
                OpCodes.Ldloc_1,
                Instructions.Ldfld(typeof(WildTreeData), nameof(WildTreeData.SeedSpreadChance)),
                OpCodes.Call,
                OpCodes.Brfalse
            );

            var skip = (Label)spread.End[-1].operand;

            spread.Prepend(
                // if (!tapped && config.OnlyPreventTapped)
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(Tree), nameof(Tree.tapped)),
                Instructions.Call_get(typeof(NetBool), nameof(NetBool.Value)),
                Instructions.Brtrue(skip),

                Instructions.Call(typeof(ModEntry), nameof(isOnlyPreventTapped)),
                Instructions.Brfalse(skip)
            );

            var seed = FindCode(
                // Game1.random.NextBool(data.SeedOnShakeChance)
                Instructions.Ldsfld(typeof(Game1), nameof(Game1.random)),
                OpCodes.Ldloc_1,
                Instructions.Ldfld(typeof(WildTreeData), nameof(WildTreeData.SeedOnShakeChance)),
                OpCodes.Call,
                OpCodes.Br_S,
                // false
                OpCodes.Ldc_I4_0,
                // this.hasSeed.Value = 
                OpCodes.Callvirt
            );

            var pop = Instructions.Pop();

            seed.Replace(
                // if (!config.RetainSeed)
                Instructions.Call(typeof(ModEntry), nameof(isRetainSeed)),
                Instructions.Brfalse(AttachLabel(pop)),
                
                // if (!environment is Farm)
                Instructions.Ldloc_0(),
                Instructions.Isinst(typeof(Farm)),
                Instructions.Brtrue(AttachLabel(pop)),

                // data.SeedOnShakeChance
                seed[0],
                seed[1],
                seed[2],

                // Increase chance that tree has a seed.
                Instructions.Call(typeof(ModEntry), nameof(getSeedChanceMultiplier)),
                Instructions.Mul(),

                // Game1.random.NextBool(*)
                seed[3],
                seed[4],

                // Add instructions for not changing seed.
                pop,
                Instructions.Br(AttachLabel(seed.End[0])),

                // false
                seed[5],
                // this.hasSeed.Value = 
                seed[6]
            );

        }
    }
}

