/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace MoreGiantCrops
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        public static Dictionary<int, Texture2D> sprites = new Dictionary<int, Texture2D>();

        public override void Entry(IModHelper helper)
        {
            instance = this;
            SpaceShared.Log.Monitor = Monitor;

            Directory.CreateDirectory(Path.Combine(Helper.DirectoryPath, "assets"));

            Log.trace("Finding giant crop images");
            foreach ( var path in Directory.EnumerateFiles(Path.Combine(Helper.DirectoryPath, "assets"), "*.png") )
            {
                string filename = Path.GetFileName(path);
                if (!int.TryParse(filename.Split('.')[0], out int id))
                {
                    Log.error("Bad PNG: " + filename);
                    continue;
                }
                Log.trace("Found PNG: " + filename);
                var tex = helper.Content.Load<Texture2D>($"assets/{filename}");
                sprites.Add(id, tex);
            }

            if (!sprites.Any())
            {
                Log.error("You must install an asset pack to use this mod.");
                return;
            }

            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Crop), nameof(Crop.newDay)),
                transpiler: new HarmonyMethod(typeof(CropPatches), nameof(CropPatches.NewDay_Transpiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(GiantCrop), nameof(GiantCrop.draw)),
                prefix: new HarmonyMethod(typeof(GiantCropPatches), nameof(GiantCropPatches.Draw_Prefix))
            );
        }
    }
}
