/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures;
using AlternativeTextures.Framework.Models;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.StandardObjects
{
    internal class GrassPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Grass);
        private const string NAME_PREFIX = "Grass";

        public GrassPatch(IMonitor modMonitor) : base(modMonitor)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Grass.draw), new[] { typeof(SpriteBatch), typeof(Vector2) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Grass.seasonUpdate), new[] { typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(SeasonUpdatePostfix)));
            harmony.Patch(AccessTools.Constructor(typeof(Grass)), postfix: new HarmonyMethod(GetType(), nameof(GrassPostfix)));
        }

        private static bool DrawPrefix(Grass __instance, int[] ___offset1, int[] ___offset2, int[] ___offset3, int[] ___offset4, int[] ___whichWeed, float ___shakeRotation, double[] ___shakeRandom, bool[] ___flip, SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1)
                {
                    return true;
                }

                for (int i = 0; i < (int)__instance.numberOfWeeds; i++)
                {
                    Vector2 pos = ((i != 4) ? (tileLocation * 64f + new Vector2((float)(i % 2 * 64 / 2 + ___offset3[i] * 4 - 4) + 30f, i / 2 * 64 / 2 + ___offset4[i] * 4 + 40)) : (tileLocation * 64f + new Vector2((float)(16 + ___offset1[i] * 4 - 4) + 30f, 16 + ___offset2[i] * 4 + 40)));
                    spriteBatch.Draw(textureModel.Texture, Game1.GlobalToLocal(Game1.viewport, pos), new Rectangle(0, textureVariation * textureModel.TextureHeight, 15, 20), Color.White, ___shakeRotation / (float)(___shakeRandom[i] + 1.0), new Vector2(7.5f, 17.5f), 4f, ___flip[i] ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (pos.Y + 16f - 20f) / 10000f + pos.X / 1E+07f);
                }
                return false;
            }
            return true;
        }

        private static void SeasonUpdatePostfix(Grass __instance, bool onLoad)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName") && __instance.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(__instance.modData["AlternativeTextureSeason"]))
            {
                __instance.modData["AlternativeTextureSeason"] = Game1.GetSeasonForLocation(__instance.currentLocation);
                __instance.modData["AlternativeTextureName"] = String.Concat(__instance.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Grass}_{NAME_PREFIX}_{__instance.modData["AlternativeTextureSeason"]}");
            }
        }

        private static void GrassPostfix(Grass __instance)
        {
            var instanceName = $"{AlternativeTextureModel.TextureType.Grass}_{NAME_PREFIX}";
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.currentLocation)}";

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
