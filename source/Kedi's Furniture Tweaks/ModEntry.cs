/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/FurnitureTweaks
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using HarmonyLib;
namespace FurnitureTweaks
{
    public class ModEntry : Mod
    {
        internal static Dictionary<string, Option> Configuration = new();
        internal IGMCM GMCM;
        internal Harmony Harmony = new("KediDili.FurnitureTweaks.DLL");
        public override void Entry(IModHelper helper)
        {
            Configuration = Helper.ModContent.Load<Dictionary<string, Option>>("options.json");

            var dict = Helper.ReadConfig<Dictionary<string, bool>>();
            if (dict is not null && dict != new Dictionary<string, bool>() && dict.Count > 0)
                foreach (var item in Configuration)
                    item.Value.Enabled = dict[item.Key];
            else
            {
                foreach (var item in Configuration)
                    dict.Add(item.Key, item.Value.Enabled);

                Helper.WriteConfig(dict);
            }
            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.LerpPosition)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.LerpPositionPostfix))
            );
            Harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.StopSitting)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.StopSitting_Prefix))
            );
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            GMCM = Helper.ModRegistry.GetApi<IGMCM>("spacechase0.GenericModConfigMenu");
            if (GMCM is null)
            {
                Monitor.Log("Generic Mod Config Menu API couldn't be gathered. The GMCM menu wont be there.", LogLevel.Info);
                return;
            }
            GMCM.Register(ModManifest, () => { ResetConfig(); } , () => { SaveConfig(); }, false);
            foreach (var item in Configuration)
            {
                GMCM.AddBoolOption(ModManifest, () => item.Value.Enabled, v => { item.Value.Enabled = v; }, () => Helper.Translation.Get(item.Key), () => Helper.Translation.Get("Generic"));
            }
        }
        public static void LerpPositionPostfix(Farmer __instance)
        {
            static Vector2 OffsetAmount(string name)
            {
                Vector2 vector2 = new(0, 0);
                if (name.Contains(" Armchair"))
                    vector2.X = 40;
                else if (name.StartsWith("Wizard") || name.StartsWith("Woodsy"))
                    vector2.X = 20;
                else
                    vector2.X = 20;

                return vector2;
            }
            if (__instance.sittingFurniture is Furniture && __instance.isSitting.Value && !__instance.isStopSitting)
            {
                if (Configuration.TryGetValue((__instance.sittingFurniture as Furniture).Name, out Option Value) && Value.Enabled)
                {
                    Vector2 vector = OffsetAmount((__instance.sittingFurniture as Furniture).Name);
                    if ((__instance.sittingFurniture as Furniture).GetSittingDirection() is 1)
                    {
                        __instance.lerpEndPosition -= vector;
                        __instance.Position -= vector;
                    }
                    else if ((__instance.sittingFurniture as Furniture).GetSittingDirection() is 3)
                    {
                        __instance.lerpEndPosition += vector;
                        __instance.Position += vector;
                    }
                }
            }
        }
        public static void StopSitting_Prefix(Farmer __instance)
        {
            if (__instance.sittingFurniture is not null && __instance.IsSitting())
            {
                if (__instance.sittingFurniture is Furniture)
                {
                    if (Configuration.ContainsKey((__instance.sittingFurniture as Furniture).Name))
                    {
                        __instance.isSitting.Value = false;
                        __instance.isStopSitting = true;
                    }
                }
            }
        }
        private void ResetConfig()
        {
            Dictionary<string, bool> dict = new();

            foreach (var item in Configuration)
                dict.Add(item.Key, true);
            Helper.WriteConfig(dict);
            if (dict is not null && dict != new Dictionary<string, bool>())
                foreach (var item in Configuration)
                    item.Value.Enabled = dict[item.Key];
        }
        private void SaveConfig()
        {
            Dictionary<string, bool> dict = new();

            foreach (var item in Configuration)
                dict.Add(item.Key, item.Value.Enabled);
            Helper.WriteConfig(dict);
        }
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("TileSheets/furniture") || e.NameWithoutLocale.IsEquivalentTo("TileSheets/furnitureFront"))
            {
                foreach (var item in Configuration)
                {
                    if (item.Value.Enabled)
                    {
                        IRawTextureData sittableTex = Helper.ModContent.Load<IRawTextureData>("assets/" + item.Key.Replace(" ", "") + (e.NameWithoutLocale.IsEquivalentTo("TileSheets/furnitureFront") ? "Front" : "") + ".png");
                        e.Edit(arg =>
                        {
                            var editor = arg.AsImage();
                            editor.PatchImage(sittableTex, targetArea: new Rectangle((int)item.Value.ToArea.X, (int)item.Value.ToArea.Y, sittableTex.Width, sittableTex.Height));
                        });
                    }
                }
            }
        }
    }
}