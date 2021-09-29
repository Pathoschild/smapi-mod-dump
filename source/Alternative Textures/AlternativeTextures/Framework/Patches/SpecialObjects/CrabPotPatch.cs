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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.SpecialObjects
{
    internal class CrabPotPatch : PatchTemplate
    {
        private readonly Type _object = typeof(CrabPot);

        internal CrabPotPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(CrabPot.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
        }

        private static bool DrawPrefix(CrabPot __instance, float ___yBob, Vector2 ___shake, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
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
                var textureOffset = textureVariation * textureModel.TextureHeight;

                if (__instance.heldObject.Value != null)
                {
                    __instance.tileIndexToShow = 714;
                }
                else if (__instance.tileIndexToShow == 0)
                {
                    __instance.tileIndexToShow = __instance.parentSheetIndex;
                }

                ___yBob = (float)(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500.0 + (double)(x * 64)) * 8.0 + 8.0);
                if (___yBob <= 0.001f)
                {
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64, 64), 150f, 8, 0, __instance.directionOffset + new Vector2(x * 64 + 4, y * 64 + 32), flicker: false, Game1.random.NextDouble() < 0.5, 0.001f, 0.01f, Color.White, 0.75f, 0.003f, 0f, 0f));
                }

                spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, __instance.directionOffset + new Vector2(x * 64, y * 64 + (int)___yBob)) + ___shake, new Rectangle((__instance.tileIndexToShow - 710) * textureModel.TextureWidth, textureOffset, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64) + __instance.directionOffset.Y + (float)(x % 4)) / 10000f);
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, __instance.directionOffset + new Vector2(x * 64 + 4, y * 64 + 48)) + ___shake, new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2112 + (((x + y) % 2 != 0) ? ((!Game1.currentLocation.waterTileFlip) ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 128 : 0)), 56, 16 + (int)___yBob), Game1.currentLocation.waterColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, ((float)(y * 64) + __instance.directionOffset.Y + (float)(x % 4)) / 9999f);

                if ((bool)__instance.readyForHarvest && __instance.heldObject.Value != null)
                {
                    float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, __instance.directionOffset + new Vector2(x * 64 - 8, (float)(y * 64 - 96 - 16) + yOffset)), new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((y + 1) * 64) / 10000f + 1E-06f + __instance.tileLocation.X / 10000f);
                    spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, __instance.directionOffset + new Vector2(x * 64 + 32, (float)(y * 64 - 64 - 8) + yOffset)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, __instance.heldObject.Value.parentSheetIndex, 16, 16), Color.White * 0.75f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float)((y + 1) * 64) / 10000f + 1E-05f + __instance.tileLocation.X / 10000f);
                }

                return false;
            }
            return true;
        }
    }
}
