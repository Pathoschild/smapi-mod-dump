using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Mods.CustomLocalization.Rewrites;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace CustomToolEffect
{
    public class ModEntry : Mod
    {
        public static ModConfig ModConfig;
        public override void Entry(IModHelper helper)
        {
            ModConfig = helper.ReadConfig<ModConfig>();
            helper.WriteConfig(ModConfig);
            HarmonyInstance harmony = HarmonyInstance.Create("zaneyork.CustomToolEffect");
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Tree), "performToolAction"),
                prefix: new HarmonyMethod(typeof(TreeRewrites.PerformToolActionRewrite), nameof(TreeRewrites.PerformToolActionRewrite.Prefix))
            );
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(FruitTree), "performToolAction"),
                prefix: new HarmonyMethod(typeof(FruitTreeRewrites.PerformToolActionRewrite), nameof(FruitTreeRewrites.PerformToolActionRewrite.Prefix))
            );
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(ResourceClump), "performToolAction"),
                prefix: new HarmonyMethod(typeof(ResourceClumpRewrites.PerformToolActionRewrite), nameof(ResourceClumpRewrites.PerformToolActionRewrite.Prefix))
            );
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Pickaxe), "DoFunction"),
                prefix: new HarmonyMethod(typeof(PickaxeRewrites.DoFunctionRewrite), nameof(PickaxeRewrites.DoFunctionRewrite.Prefix))
            );
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Tool), "tilesAffected"),
                prefix: new HarmonyMethod(typeof(ToolRewrites.TilesAffectedRewrite), nameof(ToolRewrites.TilesAffectedRewrite.Prefix))
            );
            AccessTools.GetDeclaredConstructors(typeof(TemporaryAnimatedSprite)).ForEach(ctor =>
            {
                harmony.Patch(
                    original: ctor,
                    postfix: new HarmonyMethod(typeof(ToolRewrites.TilesAffectedRewrite), nameof(TemporaryAnimatedSpriteRewrites.ConstructorRewrite.Postfix))
                );
            });
        }
    }
}
