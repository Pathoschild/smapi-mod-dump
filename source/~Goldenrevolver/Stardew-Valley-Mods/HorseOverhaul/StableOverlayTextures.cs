/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.Buffers;
using System.Collections.Generic;

namespace HorseOverhaul
{
    internal class StableOverlayTextures
    {
        public static Texture2D GetCurrentStableTexture(HorseOverhaul mod)
        {
            if (mod.UsingMyStableTextures)
            {
                return mod.Helper.ModContent.Load<Texture2D>("assets/stable.png");
            }
            else
            {
                var baseGameTexture = mod.Helper.GameContent.Load<Texture2D>("Buildings/Stable");

                int count = baseGameTexture.Width * baseGameTexture.Height;

                var stableCopy = new Texture2D(Game1.graphics.GraphicsDevice, baseGameTexture.Width, baseGameTexture.Height) { Name = mod.ModManifest.UniqueID + ".StableCopy" };

                var textureData = ArrayPool<Color>.Shared.Rent(count);
                baseGameTexture.GetData(textureData, 0, count);
                stableCopy.SetData(textureData, 0, count);
                ArrayPool<Color>.Shared.Return(textureData);

                return stableCopy;
            }
        }

        internal static Texture2D MergeTextures(IRawTextureData overlay, Texture2D oldTexture)
        {
            if (overlay == null || oldTexture == null)
            {
                return oldTexture;
            }

            if (oldTexture.Width != overlay.Width)
            {
                return oldTexture;
            }

            int pixelCount = oldTexture.Width * oldTexture.Height;
            var newData = overlay.Data;

            var origData = ArrayPool<Color>.Shared.Rent(pixelCount);
            oldTexture.GetData(origData, 0, pixelCount);

            if (newData == null || origData == null)
            {
                ArrayPool<Color>.Shared.Return(origData);
                return oldTexture;
            }

            int pixelOffset = 0;

            // if some mod has a stable texture that is larger in height than the default stable texture, then we ignore the top rows
            if (pixelCount != newData.Length)
            {
                int heightOffset = oldTexture.Height - overlay.Height;

                if (heightOffset >= 0)
                {
                    pixelOffset = heightOffset * oldTexture.Width;
                }
                else
                {
                    ArrayPool<Color>.Shared.Return(origData);
                    return oldTexture;
                }
            }

            for (int i = 0; i < newData.Length; i++)
            {
                ref Color newValue = ref newData[i];

                if (newValue.A != 0)
                {
                    origData[i + pixelOffset] = newValue;
                }
            }

            oldTexture.SetData(origData, 0, pixelCount);

            ArrayPool<Color>.Shared.Return(origData);
            return oldTexture;
        }

        internal static void SetOverlays(HorseOverhaul mod)
        {
            if (mod.Config.SaddleBag && mod.Config.VisibleSaddleBags != SaddleBagOption.Disabled.ToString())
            {
                mod.SaddleBagOverlay = mod.Helper.ModContent.Load<Texture2D>($"assets/saddlebags_{mod.Config.VisibleSaddleBags.ToLower()}.png");
                mod.IsUsingHorsemanship = mod.Helper.ModRegistry.IsLoaded("red.horsemanship");
            }

            // do not check for UsingIncompatibleTextures here
            if (!mod.Config.Water || mod.Config.DisableStableSpriteChanges)
            {
                return;
            }

            mod.SeasonalVersion = SeasonalVersion.None;

            mod.UsingMyStableTextures = false;
            mod.UsingIncompatibleTextures = false;

            mod.FilledTroughOverlay = null;
            mod.EmptyTroughOverlay = null;
            mod.RepairTroughOverlay = null;

            if (mod.Helper.ModRegistry.IsLoaded("sonreirblah.JBuildings"))
            {
                // seasonal overlays are assigned in LateDayStarted
                mod.SeasonalVersion = SeasonalVersion.Sonr;
                return;
            }

            if (mod.Helper.ModRegistry.IsLoaded("leroymilo.USJB"))
            {
                var data = mod.Helper.ModRegistry.Get("leroymilo.USJB");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var dict = mod.ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "Buildings", "Stable" }, data.Manifest.Name, false);

                    if (dict["Buildings"].ToLower() == "true" && dict["Stable"].ToLower() == "true")
                    {
                        // seasonal overlays are assigned in LateDayStarted
                        mod.SeasonalVersion = SeasonalVersion.Sonr;
                        return;
                    }
                }
            }

            if (mod.Helper.ModRegistry.IsLoaded("Oklinq.CleanStable"))
            {
                mod.EmptyTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/overlay_empty_both.png");

                mod.FilledTroughOverlay = GetPreferredFilledTroughOverlay(mod, "assets/overlay_empty_{option}.png");

                return;
            }

            if (mod.Helper.ModRegistry.IsLoaded("Elle.SeasonalBuildings"))
            {
                var data = mod.Helper.ModRegistry.Get("Elle.SeasonalBuildings");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var dict = mod.ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "color palette", "stable" }, data.Manifest.Name, false);

                    if (dict["stable"].ToLower() != "false")
                    {
                        mod.EmptyTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/elle/overlay_empty_{dict["color palette"]}.png");

                        return;
                    }
                }
            }

            if (mod.Helper.ModRegistry.IsLoaded("Elle.SeasonalVanillaBuildings"))
            {
                var data = mod.Helper.ModRegistry.Get("Elle.SeasonalVanillaBuildings");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var dict = mod.ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "stable" }, data.Manifest.Name, false);

                    if (dict["stable"].ToLower() == "true")
                    {
                        mod.FilledTroughOverlay = GetPreferredFilledTroughOverlay(mod, "assets/overlay_tone_empty_{option}.png");
                        mod.EmptyTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/overlay_tone_empty_both.png");

                        mod.RepairTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/overlay_tone_fixed_filled.png");

                        return;
                    }
                }
            }

            if (mod.Helper.ModRegistry.IsLoaded("Gweniaczek.Medieval_stables"))
            {
                IModInfo data = mod.Helper.ModRegistry.Get("Gweniaczek.Medieval_stables");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var dict = mod.ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "stableOption" }, data.Manifest.Name, false);

                    SetupGwenTextures(mod, dict);

                    return;
                }
            }

            if (mod.Helper.ModRegistry.IsLoaded("Gweniaczek.Medieval_buildings"))
            {
                var data = mod.Helper.ModRegistry.Get("Gweniaczek.Medieval_buildings");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var dict = mod.ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "buildingsReplaced", "stableOption" }, data.Manifest.Name, false);

                    if (dict["buildingsReplaced"].Contains("stable"))
                    {
                        SetupGwenTextures(mod, dict);

                        return;
                    }
                }
            }

            if (mod.Helper.ModRegistry.IsLoaded("magimatica.SeasonalVanillaBuildings") || mod.Helper.ModRegistry.IsLoaded("red.HudsonValleyBuildings"))
            {
                mod.EmptyTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/overlay_empty_trough.png");

                mod.SeasonalVersion = SeasonalVersion.Magimatica;

                return;
            }

            if (mod.Helper.ModRegistry.IsLoaded("Lilys.ModularStable"))
            {
                var data = mod.Helper.ModRegistry.Get("Lilys.ModularStable");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var dict = mod.ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "Building Type" }, data.Manifest.Name, false);

                    if (dict.ContainsKey("Building Type"))
                    {
                        SetupLilyTextures(mod, dict);

                        return;
                    }
                }
            }

            if (mod.Helper.ModRegistry.IsLoaded("Rosalie.CuteValley"))
            {
                var data = mod.Helper.ModRegistry.Get("Rosalie.CuteValley");

                var path = data.GetType().GetProperty("DirectoryPath");

                if (path != null && path.GetValue(data) != null)
                {
                    var dict = mod.ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "Stable" }, data.Manifest.Name, false);

                    // check for 'cute valley - blue' and 'disabled' option
                    if (dict.ContainsKey("Stable") && (dict["Stable"].ToLower() == "enabled" || dict["Stable"].ToLower() == "maleha's stable"))
                    {
                        SetupRosieaTextures(mod, dict);

                        return;
                    }
                }
            }

            // no compatible texture mod found, so we will use mine
            mod.UsingMyStableTextures = true;

            mod.EmptyTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/overlay_empty_both.png");
            mod.FilledTroughOverlay = GetPreferredFilledTroughOverlay(mod, "assets/overlay_empty_{option}.png");
        }

        private static void SetupGwenTextures(HorseOverhaul mod, Dictionary<string, string> dict)
        {
            if (dict["stableOption"] == "4")
            {
                mod.FilledTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/gwen/overlay_{dict["stableOption"]}_full.png");
            }

            mod.EmptyTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/gwen/overlay_{dict["stableOption"]}.png");

            mod.SeasonalVersion = SeasonalVersion.Gwen;
            mod.GwenOption = dict["stableOption"];
        }

        private static void SetupLilyTextures(HorseOverhaul mod, Dictionary<string, string> dict)
        {
            if (dict["Building Type"] != null && dict["Building Type"].Trim().ToLower().StartsWith("stable"))
            {
                mod.FilledTroughOverlay = GetPreferredFilledTroughOverlay(mod, "assets/overlay_empty_{option}.png");
                mod.EmptyTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/overlay_empty_both.png");

                mod.RepairTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/overlay_fixed_filled.png");
            }
            else
            {
                mod.UsingIncompatibleTextures = true;
                mod.DebugLog("Horse Overhaul detected Lily's Modular Stable with a non stable option (most likely garage). Horse Overhaul has no overlay for this, so visible water trough overlays are disabled.");

                // TODO for when I get permission to activate them
                //mod.FilledTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/lily/overlay_garage_bucket_filled.png");
                //mod.EmptyTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/lily/overlay_garage_bucket_empty.png");
            }
        }

        private static void SetupRosieaTextures(HorseOverhaul mod, Dictionary<string, string> dict)
        {
            if (dict["Stable"].ToLower() == "enabled")
            {
                mod.EmptyTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/rosiea/overlay_empty_bucket.png");
            }
            else if (dict["Stable"].ToLower() == "maleha's stable")
            {
                mod.EmptyTroughOverlay = mod.Helper.ModContent.Load<IRawTextureData>($"assets/rosiea/overlay_maleha_empty_bucket.png");
            }
        }

        private static IRawTextureData GetPreferredFilledTroughOverlay(HorseOverhaul mod, string path)
        {
            if (mod.Config.PreferredWaterContainer == WaterOption.All.ToString())
            {
                return null;
            }
            else if (mod.Config.PreferredWaterContainer == WaterOption.Bucket.ToString())
            {
                // show bucket -> hide trough
                return mod.Helper.ModContent.Load<IRawTextureData>(path.Replace("{option}", "trough"));
            }
            else
            {
                // show trough (the fallback) -> hide bucket
                return mod.Helper.ModContent.Load<IRawTextureData>(path.Replace("{option}", "bucket"));
            }
        }
    }
}