/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Objects.Items.Tools;
using FishingTrawler.Framework.Utilities;
using FishingTrawler.Patches;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;

namespace FishingTrawler.Framework.Patches.Objects
{
    internal class ToolPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Tool);

        public ToolPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "get_DisplayName", null), postfix: new HarmonyMethod(GetType(), nameof(GetNamePostfix)));
            harmony.Patch(AccessTools.Method(_object, "get_description", null), postfix: new HarmonyMethod(GetType(), nameof(GetDescriptionPostfix)));
            harmony.Patch(AccessTools.Method(typeof(Item), nameof(Item.canBeTrashed), null), postfix: new HarmonyMethod(GetType(), nameof(CanBeTrashedPostfix)));

            harmony.Patch(AccessTools.Method(_object, nameof(Tool.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.beginUsing), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(BeginUsingPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.tickUpdate), new[] { typeof(GameTime), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(TickUpdatePrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.DoFunction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(DoFunctionPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.endUsing), new[] { typeof(GameLocation), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(EndUsingPrefix)));
        }

        private static void GetNamePostfix(Tool __instance, ref string __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_KEY))
            {
                __result = _helper.Translation.Get("item.bailing_bucket.name");
                return;
            }
            else if (LostFishingCharm.IsValid(__instance))
            {
                __result = _helper.Translation.Get("item.lost_fishing_charm.name");
                return;
            }
            else if (Trident.IsValid(__instance))
            {
                __result = _helper.Translation.Get("item.trident_tool.name");
                return;
            }
        }

        private static void GetDescriptionPostfix(Tool __instance, ref string __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_KEY) && new BailingBucket(__instance) is BailingBucket bucket && bucket.IsValid)
            {
                __result = bucket.ContainsWater ? _helper.Translation.Get("item.bailing_bucket.description_full") : _helper.Translation.Get("item.bailing_bucket.description_empty");
                return;
            }
            else if (LostFishingCharm.IsValid(__instance))
            {
                __result = _helper.Translation.Get("item.lost_fishing_charm.description");
                return;
            }
            else if (Trident.IsValid(__instance))
            {
                __result = _helper.Translation.Get("item.trident_tool.description");
                return;
            }
        }

        private static void CanBeTrashedPostfix(Tool __instance, ref bool __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_KEY))
            {
                __result = FishingTrawler.IsPlayerOnTrawler() is false;
                return;
            }
            else if (LostFishingCharm.IsValid(__instance))
            {
                __result = true;
                return;
            }
            else if (Trident.IsValid(__instance))
            {
                __result = true;
                return;
            }
        }

        private static bool DrawInMenuPrefix(Tool __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_KEY) && new BailingBucket(__instance) is BailingBucket bucket && bucket.IsValid)
            {
                int spriteOffset = bucket.ContainsWater ? 16 : 0;
                spriteBatch.Draw(FishingTrawler.assetManager.bucketTexture, location + new Vector2(32f, 32f), new Rectangle(spriteOffset, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f), 4f * (scaleSize + bucket.Scale), SpriteEffects.None, layerDepth);

                return false;
            }
            else if (LostFishingCharm.IsValid(__instance))
            {
                spriteBatch.Draw(FishingTrawler.assetManager.lostFishingCharmTexture, location + new Vector2(34f, 32f) * scaleSize, new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);

                return false;
            }
            else if (Trident.IsValid(__instance))
            {
                spriteBatch.Draw(FishingTrawler.assetManager.tridentTexture, location + new Vector2(34f, 32f) * scaleSize, new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);

                return false;
            }

            return true;
        }

        private static bool BeginUsingPrefix(Tool __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_KEY) && who == Game1.player && new BailingBucket(__instance) is BailingBucket bucket && bucket.IsValid)
            {
                __result = true;
                return bucket.Use(location, x, y, who);
            }
            else if (LostFishingCharm.IsValid(__instance) && who == Game1.player)
            {
                __result = true;
                return LostFishingCharm.Use(location, x, y, who);
            }
            else if (Trident.IsValid(__instance) && who == Game1.player)
            {
                __result = true;
                return Trident.Use(location, x, y, who);
            }

            return true;
        }

        private static bool TickUpdatePrefix(Tool __instance, ref Farmer ___lastUser, GameTime time, Farmer who)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_KEY) && who == Game1.player && new BailingBucket(__instance) is BailingBucket bucket && bucket.IsValid)
            {
                if (bucket.Scale > 0f)
                {
                    bucket.Scale -= 0.01f;
                    bucket.SaveData();
                }

                return false;
            }
            else if (Trident.IsValid(__instance) && who.UsingTool is true && who.CurrentTool == __instance)
            {
                Trident.Update(__instance, time, who);
            }

            return true;
        }

        private static bool DoFunctionPrefix(Tool __instance, ref Farmer ___lastUser, GameLocation location, int x, int y, int power, Farmer who)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_KEY) && who == Game1.player && new BailingBucket(__instance) is BailingBucket bucket && bucket.IsValid)
            {
                return false;
            }
            else if (LostFishingCharm.IsValid(__instance))
            {
                return false;
            }
            else if (Trident.IsValid(__instance))
            {
                return false;
            }

            return true;
        }

        private static bool EndUsingPrefix(Tool __instance, GameLocation location, Farmer who)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_KEY) && who == Game1.player && new BailingBucket(__instance) is BailingBucket bucket && bucket.IsValid)
            {
                return false;
            }
            else if (LostFishingCharm.IsValid(__instance))
            {
                return false;
            }
            else if (Trident.IsValid(__instance))
            {
                who.forceCanMove();
                return false;
            }

            return true;
        }
    }
}
