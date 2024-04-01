/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Models;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace AlternativeTextures.Framework.Patches.StandardObjects
{
    internal class HoeDirtPatch : PatchTemplate
    {
        private readonly Type _object = typeof(HoeDirt);

        internal HoeDirtPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(HoeDirt.plant), new[] { typeof(string), typeof(Farmer), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(PlantPostfix)));
        }

        private static void PlantPostfix(HoeDirt __instance, string itemId, Farmer who, bool isFertilizer)
        {
            var instanceName = Game1.objectData.ContainsKey(itemId) ? Game1.objectData[itemId].Name : String.Empty;
            instanceName = $"{AlternativeTextureModel.TextureType.Crop}_{instanceName}";
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.Location)}";

            if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName) && AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
            {
                var result = Game1.random.Next(2) > 0 ? AssignModData(__instance, instanceSeasonName, true) : AssignModData(__instance, instanceName, false);
                return;
            }
            else
            {
                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName))
                {
                    AssignModData(__instance, instanceName, false);
                    return;
                }

                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
                {
                    AssignModData(__instance, instanceSeasonName, true);
                    return;
                }
            }

            AssignDefaultModData(__instance, instanceSeasonName, true);
        }
    }
}
