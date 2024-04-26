/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Models.Appearances.Pants;
using FashionSense.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;

namespace FashionSense.Framework.Patches.Entities
{
    internal class FarmerPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Farmer);

        internal FarmerPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Farmer.DrawShadow), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawShadowPrefix)));
        }

        private static bool DrawShadowPrefix(Farmer __instance, SpriteBatch b)
        {
            if (__instance is not null && AppearanceHelpers.GetCurrentlyEquippedModels(__instance, __instance.FacingDirection).Count > 0)
            {
                // Hide the player's shadow if required
                if (__instance.modData.ContainsKey(ModDataKeys.CUSTOM_PANTS_ID) && FashionSense.textureManager.GetSpecificAppearanceModel<PantsContentPack>(__instance.modData[ModDataKeys.CUSTOM_PANTS_ID]) is PantsContentPack pPack && pPack != null)
                {
                    PantsModel pantsModel = pPack.GetPantsFromFacingDirection(__instance.FacingDirection);

                    if (pantsModel is not null && pantsModel.HideShadow)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
