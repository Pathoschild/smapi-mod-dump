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
using AlternativeTextures.Framework.Utilities.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.Buildings
{
    internal class StablePatch : PatchTemplate
    {
        private readonly Type _entity = typeof(Stable);
        private const int TRACTOR_GARAGE_ID = -794739;

        internal StablePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(Stable.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
        }

        internal static bool DrawPrefix(Stable __instance, NetFloat ___alpha, SpriteBatch b)
        {
            if (__instance.maxOccupants == TRACTOR_GARAGE_ID && __instance.modData.ContainsKey("AlternativeTextureName"))
            {
                if (__instance.isMoving || __instance.daysOfConstructionLeft > 0)
                {
                    return true;
                }

                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                var paintedTexture = BuildingPatch.GetBuildingTextureWithPaint(__instance, textureModel, textureVariation);

                __instance.drawShadow(b);
                b.Draw(paintedTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX * 64, (int)__instance.tileY * 64 + (int)__instance.tilesHigh * 64)), paintedTexture.Bounds, __instance.color.Value * ___alpha, 0f, new Vector2(0f, __instance.texture.Value.Bounds.Height), 4f, SpriteEffects.None, (float)(((int)__instance.tileY + (int)__instance.tilesHigh - 1) * 64) / 10000f);
                
                return false;
            }

            return true;
        }
    }
}
