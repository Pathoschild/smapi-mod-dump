/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/huancz/SDV-TidyOrchard
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace TidyOrchard
{
    internal sealed class ModEntry : StardewModdingAPI.Mod
    {
        public static IModHelper modHelper { get; private set; } = null!;
        public static ModEntry instance { get; private set; } = null!;

        public static string ModDataKey = null!;

        public override void Entry(IModHelper helper)
        {
            modHelper = helper;
            instance = this;
            ModDataKey = $"{ModEntry.instance.ModManifest.UniqueID}_sapling";
            InitMod();
        }

        private void InitMod()
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: typeof(FruitTree).GetMethod(nameof(FruitTree.performToolAction)),
                postfix: new HarmonyMethod(typeof(HarmonyStuff).GetMethod(nameof(HarmonyStuff.FruitTree_performToolAction)))
            );
            harmony.Patch(
                original: typeof(StardewValley.Object).GetMethod(nameof(StardewValley.Object.placementAction)),
                postfix: new HarmonyMethod(typeof(HarmonyStuff).GetMethod(nameof(HarmonyStuff.Sapling_placementAction)))
                );
            harmony.Patch(
                original: typeof(StardewValley.Object).GetMethod(nameof(StardewValley.Object.maximumStackSize)),
                prefix: new HarmonyMethod(typeof(HarmonyStuff).GetMethod(nameof(HarmonyStuff.Sapling_MaximumStackSize)))
                );
        }
    }

    internal class HarmonyStuff
    {
        private class ModDataModel
        {
            public int daysUntilMature { get; set; }
            public int growthRate { get; set; }
            public int growthStage { get; set; }
            public float health { get; set; }
        }

        public static void FruitTree_performToolAction(Tool t, int explosion, Vector2 tileLocation, FruitTree __instance, bool __result)
        {
            try
            {
                _FruitTree_performToolAction(t, explosion, tileLocation, __instance, __result);
            }
            catch (Exception e)
            {
                ModEntry.instance.Monitor.Log($"{nameof(FruitTree_performToolAction)}\n{e}", LogLevel.Error);
            }
        }

        private static void _FruitTree_performToolAction(Tool t, int explosion, Vector2 tileLocation, FruitTree __instance, bool __result)
        {
            if (t == null
                || t is not Hoe
                || __instance.health.Value <= 0.0
                || __instance.treeId.Value == null
                || __instance.fruit.Count > 0
                || __instance.struckByLightningCountdown.Value > 0
                || __instance.stump.Value
                || __instance.growthStage.Value < 3 // stage 3 trees require axe, lower stages react to hoe too => creating special sapling would add duplicate
                || __result == true
                || explosion != 0
                )
            {
                return;
            }

            var data = System.Text.Json.JsonSerializer.Serialize(new ModDataModel
            {
                daysUntilMature = __instance.daysUntilMature.Value,
                growthRate = __instance.growthRate.Value,
                growthStage = __instance.growthStage.Value,
                health = __instance.health.Value,
            });
            __instance.Location.terrainFeatures.Remove(tileLocation);

            var sapling = ItemRegistry.Create("(O)" + __instance.treeId.Value, quality: __instance.GetQuality());
            sapling.modData[ModEntry.ModDataKey] = data;
            Game1.createItemDebris(sapling, tileLocation * 64f, 2, __instance.Location);
        }

        public static bool Sapling_MaximumStackSize(StardewValley.Object __instance, ref int __result)
        {
            try
            {
                return _Sapling_MaximumStackSize(__instance, ref __result);
            }
            catch (Exception e)
            {
                ModEntry.instance.Monitor.Log($"{nameof(Sapling_MaximumStackSize)}\n{e}", LogLevel.Error);
                return true;
            }
        }

        private static bool _Sapling_MaximumStackSize(StardewValley.Object __instance, ref int __result)
        {
            if (__instance.IsFruitTreeSapling() && __instance.modData.ContainsKey(ModEntry.ModDataKey))
            {
                __result = 1;
                return false;
            }
            return true;
        }

        public static void Sapling_placementAction(GameLocation location, int x, int y, Farmer who, StardewValley.Object __instance, ref bool __result)
        {
            try
            {
                _Sapling_placementAction(location, x, y, who, __instance, ref __result);
            }
            catch (Exception e)
            {
                ModEntry.instance.Monitor.Log($"{nameof(Sapling_placementAction)}\n{e}", LogLevel.Error);
            }
        }

        private static void _Sapling_placementAction(GameLocation location, int x, int y, Farmer who, StardewValley.Object __instance, ref bool __result)
        {
            if (!__result)
            {
                return;
            }
            if (!__instance.IsFruitTreeSapling())
            {
                return;
            }

            string stringData = null!;
            if (!__instance.modData.TryGetValue(ModEntry.ModDataKey, out stringData))
            {
                return;
            }

            TerrainFeature treeFeature;
            if (!location.terrainFeatures.TryGetValue(new Vector2((float)(x / 64), (float)(y / 64)), out treeFeature) || treeFeature is not FruitTree)
            {
                ModEntry.instance.Monitor.Log($"planted tree not found / not a fruit tree at [{x / 64}, {y / 64}]", LogLevel.Warn);
                return;
            }

            var data = System.Text.Json.JsonSerializer.Deserialize<ModDataModel>(stringData)!;
            var tree = (FruitTree)treeFeature;
            tree.daysUntilMature.Value = data.daysUntilMature;
            tree.growthRate.Value = data.growthRate;
            tree.growthStage.Value = data.growthStage;
            tree.health.Value = data.health;

            ModEntry.instance.Monitor.Log($"updated planted sapling from mod data: {stringData}", LogLevel.Trace);
        }
    }
}
