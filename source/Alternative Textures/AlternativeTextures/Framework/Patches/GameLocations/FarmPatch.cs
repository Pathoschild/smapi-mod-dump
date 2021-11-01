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
using AlternativeTextures.Framework.Patches.Buildings;
using AlternativeTextures.Framework.Utilities.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.GameLocations
{
    internal class FarmPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Farm);

        internal FarmPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Farm.ApplyHousePaint), null), postfix: new HarmonyMethod(GetType(), nameof(ApplyHousePaintPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Farm.DayUpdate), new[] { typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(DayUpdatePostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Farm.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));

            harmony.CreateReversePatcher(AccessTools.Method(typeof(BuildableGameLocation), nameof(BuildableGameLocation.draw), new[] { typeof(SpriteBatch) }), new HarmonyMethod(GetType(), nameof(BaseDrawReversePatch))).Patch();
        }

        private static void ApplyHousePaintPostfix(Farm __instance)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return;
                }

                if (__instance.paintedHouseTexture != null)
                {
                    __instance.paintedHouseTexture.Dispose();
                    __instance.paintedHouseTexture = null;
                }

                var targetedBuilding = new Building();
                targetedBuilding.buildingType.Value = $"Farmhouse_{Game1.MasterPlayer.HouseUpgradeLevel}";
                targetedBuilding.netBuildingPaintColor = __instance.housePaintColor;
                targetedBuilding.tileX.Value = __instance.GetHouseRect().X;
                targetedBuilding.tileY.Value = __instance.GetHouseRect().Y;
                targetedBuilding.tilesWide.Value = __instance.GetHouseRect().Width + 1;
                targetedBuilding.tilesHigh.Value = __instance.GetHouseRect().Height + 1;

                __instance.paintedHouseTexture = BuildingPatch.GetBuildingTextureWithPaint(targetedBuilding, textureModel, textureVariation, true);
            }
        }

        private static void DayUpdatePostfix(Farm __instance, int dayOfMonth)
        {
            ApplyHousePaintPostfix(__instance);
        }

        private static bool DrawPrefix(Farm __instance, TemporaryAnimatedSprite ___shippingBinLid, SpriteBatch b)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
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

                // Initial vanilla logic
                BaseDrawReversePatch(__instance, b);
                foreach (KeyValuePair<long, FarmAnimal> pair in __instance.animals.Pairs)
                {
                    pair.Value.draw(b);
                }
                Point entry_position_tile = __instance.GetMainFarmHouseEntry();
                Vector2 entry_position_world = Utility.PointToVector2(entry_position_tile) * 64f;
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(entry_position_tile.X - 5, entry_position_tile.Y + 2) * 64f), Building.leftShadow, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                for (int x = 1; x < 8; x++)
                {
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(entry_position_tile.X - 5 + x, entry_position_tile.Y + 2) * 64f), Building.middleShadow, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                }
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(entry_position_tile.X + 3, entry_position_tile.Y + 2) * 64f), Building.rightShadow, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);

                // Custom farmhouse logic
                Color house_draw_color = Color.White;
                if (__instance.frameHouseColor.HasValue)
                {
                    house_draw_color = __instance.frameHouseColor.Value;
                    __instance.frameHouseColor = null;
                }

                Texture2D house_texture = textureModel.GetTexture(textureVariation);
                if (__instance.paintedHouseTexture != null)
                {
                    house_texture = __instance.paintedHouseTexture;
                }

                Vector2 house_draw_position = new Vector2(entry_position_world.X - 384f, entry_position_world.Y - 440f);
                b.Draw(house_texture, Game1.GlobalToLocal(Game1.viewport, house_draw_position), new Rectangle(0, 0, 160, 144), house_draw_color, 0f, Vector2.Zero, 4f, SpriteEffects.None, (house_draw_position.Y + 230f) / 10000f);

                // Do rest of vanilla logic
                if (Game1.mailbox.Count > 0)
                {
                    float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                    Point mailbox_position = Game1.player.getMailboxPosition();
                    float draw_layer = (float)((mailbox_position.X + 1) * 64) / 10000f + (float)(mailbox_position.Y * 64) / 10000f;
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(mailbox_position.X * 64, (float)(mailbox_position.Y * 64 - 96 - 48) + yOffset)), new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, draw_layer + 1E-06f);
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(mailbox_position.X * 64 + 32 + 4, (float)(mailbox_position.Y * 64 - 64 - 24 - 8) + yOffset)), new Microsoft.Xna.Framework.Rectangle(189, 423, 15, 13), Color.White, 0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, draw_layer + 1E-05f);
                }
                if (___shippingBinLid != null)
                {
                    ___shippingBinLid.draw(b);
                }
                if (!__instance.hasSeenGrandpaNote)
                {
                    Point grandpa_shrine = __instance.GetGrandpaShrinePosition();
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((grandpa_shrine.X + 1) * 64, grandpa_shrine.Y * 64)), new Microsoft.Xna.Framework.Rectangle(575, 1972, 11, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0448009968f);
                }
                return false;
            }

            return true;
        }
        public static void BaseDrawReversePatch(BuildableGameLocation __instance, SpriteBatch b)
        {
            new NotImplementedException("It's a stub!");
        }
    }
}