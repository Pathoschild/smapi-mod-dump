/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework.Graphics;
using SkillfulClothes.Effects;
using SkillfulClothes.Types;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes
{
    class HarmonyPatches
    {
        static HatDrawWrapper hatDrawWrapper = new HatDrawWrapper();
        static ClothingDrawWrapper clothingDrawWrapper = new ClothingDrawWrapper();

        public static void Apply(String modId)
        {
            HarmonyInstance harmony = HarmonyInstance.Create(modId);

            harmony.Patch(
                  original: AccessTools.Method(typeof(Item), "getDescriptionWidth"),
                  postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.getDescriptionWidth))
               );

            // patch IClickableMenu.drawToolTip, since Harmony (1.2.0.1) is unable to patch Item.getExtraSpaceNeededForTooltipSpecialIcons() correctly (as it returns a struct)
            // see https://github.com/pardeike/Harmony/issues/159 and https://github.com/pardeike/Harmony/issues/77
            // seems to be fixed in Harmony 2.0.4.0
            harmony.Patch(
                  original: typeof(IClickableMenu).GetMethod(nameof(IClickableMenu.drawToolTip), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public),
                  prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.drawToolTip))
                );            
        }

        static void drawToolTip(ref Item hoveredItem)
        {
            // replace the hoveredItem with a wrapper class which allows us to
            // control Item.getExtraSpaceNeededForTooltipSpecialIcons
            if (PredefinedEffects.GetEffect(hoveredItem, out IEffect effect))
            {
                if (hoveredItem is Clothing clothing)
                {
                    clothingDrawWrapper.Assign(clothing, effect);
                    hoveredItem = clothingDrawWrapper;
                } else if (hoveredItem is StardewValley.Objects.Hat hat)
                {
                    hatDrawWrapper.Assign(hat, effect);
                    hoveredItem = hatDrawWrapper;
                }
            }
        }

        static int getDescriptionWidth(int __result, Item __instance)
        {
            // increase the width so that effect descriptions stay on one line and do not break
            if (PredefinedEffects.GetEffect(__instance, out IEffect effect))
            {
                return Math.Max(__result, EffectHelper.getDescriptionWidth(effect));
            }

            return __result;
        }

    }
}
