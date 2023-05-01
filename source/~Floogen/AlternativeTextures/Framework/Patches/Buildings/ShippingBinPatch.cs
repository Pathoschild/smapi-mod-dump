/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System;

namespace AlternativeTextures.Framework.Patches.Buildings
{
    internal class ShippingBinPatch : PatchTemplate
    {
        private readonly Type _entity = typeof(ShippingBin);

        internal ShippingBinPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(ShippingBin.Update), new[] { typeof(GameTime) }), prefix: new HarmonyMethod(GetType(), nameof(UpdatePrefix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(ShippingBin.initLid), null), postfix: new HarmonyMethod(GetType(), nameof(InitLidPostfix)));
        }

        internal static bool UpdatePrefix(ShippingBin __instance, TemporaryAnimatedSprite ___shippingBinLid, Rectangle ___shippingBinLidOpenArea, Vector2 ____lidGenerationPosition, GameTime time)
        {
            if (___shippingBinLid != null && __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_VARIATION]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                var textureOffset = textureModel.GetTextureOffset(textureVariation);

                if (textureModel.GetTexture(textureVariation) != ___shippingBinLid.texture || ___shippingBinLid.sourceRectStartingPos != new Vector2(32, textureOffset))
                {
                    InitLidPostfix(__instance, ___shippingBinLid, ___shippingBinLidOpenArea, ____lidGenerationPosition);
                }

                return true;
            }

            return true;
        }

        internal static void InitLidPostfix(ShippingBin __instance, TemporaryAnimatedSprite ___shippingBinLid, Rectangle ___shippingBinLidOpenArea, Vector2 ____lidGenerationPosition)
        {
            if (___shippingBinLid != null && __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME]);
                if (textureModel is null)
                {
                    return;
                }

                var textureVariation = Int32.Parse(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_VARIATION]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return;
                }
                var textureOffset = textureModel.GetTextureOffset(textureVariation);

                ___shippingBinLid.texture = textureModel.GetTexture(textureVariation);
                ___shippingBinLid.currentParentTileIndex = 0;
                ___shippingBinLid.sourceRect = new Rectangle(32, textureOffset, 30, 25);
                ___shippingBinLid.sourceRectStartingPos = new Vector2(32, textureOffset);
            }

            return;
        }
    }
}
