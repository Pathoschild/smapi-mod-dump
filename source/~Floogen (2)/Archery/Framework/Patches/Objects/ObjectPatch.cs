/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects;
using Archery.Framework.Objects.Items;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace Archery.Framework.Patches.Objects
{
    internal class ObjectPatch : PatchTemplate
    {
        private readonly System.Type _object = typeof(Object);

        public ObjectPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "get_DisplayName", null), postfix: new HarmonyMethod(GetType(), nameof(GetNamePostfix)));
            harmony.Patch(AccessTools.Method(_object, "getDescription", null), postfix: new HarmonyMethod(GetType(), nameof(GetDescriptionPostfix)));

            harmony.Patch(AccessTools.Method(_object, nameof(Object.addToStack), new[] { typeof(Item) }), prefix: new HarmonyMethod(GetType(), nameof(AddToStackPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Object.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
        }

        private static void GetNamePostfix(Object __instance, ref string __result)
        {
            if (Arrow.IsValid(__instance))
            {
                __result = Arrow.GetName(__instance);
                return;
            }
        }

        private static void GetDescriptionPostfix(Object __instance, ref string __result)
        {
            if (Arrow.IsValid(__instance))
            {
                __result = Arrow.GetDescription(__instance);
                return;
            }
        }

        private static bool AddToStackPrefix(Object __instance, ref int __result, Item otherStack)
        {
            if (Arrow.IsValid(__instance))
            {
                if (Arrow.IsValid(otherStack) && Arrow.GetInternalId(__instance) == Arrow.GetInternalId(otherStack))
                {
                    return true;
                }

                __result = otherStack.Stack;
                return false;
            }

            return true;
        }

        private static bool DrawInMenuPrefix(Object __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, ref Color color, bool drawShadow)
        {
            if (Arrow.GetModel<AmmoModel>(__instance) is AmmoModel arrowModel && arrowModel is not null)
            {
                bool isRecipe = InstancedObject.IsRecipe(__instance);

                if (isRecipe)
                {
                    transparency = 0.5f;
                    scaleSize *= 0.75f;
                }

                var arrowIcon = arrowModel.GetIcon(Game1.player);
                if (arrowIcon is null)
                {
                    return false;
                }

                spriteBatch.Draw(arrowModel.Texture, location + (new Vector2(32f, 32f) + arrowIcon.Offset) * scaleSize, arrowIcon.Source, color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, arrowIcon.Scale, arrowIcon.GetSpriteEffects(), layerDepth);

                if (drawStackNumber != 0 && __instance.Stack > 0 && __instance.Stack != int.MaxValue && isRecipe is false)
                {
                    Utility.drawTinyDigits(__instance.Stack, spriteBatch, location + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(__instance.Stack, 3f * scaleSize)) + 3f * scaleSize, 64f - 18f * scaleSize + 2f), 3f * scaleSize, 1f, Color.White);
                }

                if (isRecipe)
                {
                    spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(16f, 16f), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16, 16), color, 0f, Vector2.Zero, 3f, SpriteEffects.None, layerDepth + 0.0001f);
                }
                return false;
            }

            return true;
        }
    }
}