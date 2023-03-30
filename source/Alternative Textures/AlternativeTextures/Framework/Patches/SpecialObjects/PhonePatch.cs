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
    internal class PhonePatch : PatchTemplate
    {
        private readonly Type _object = typeof(Phone);

        internal PhonePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Phone.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
        }

        private static bool DrawPrefix(Phone __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
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
                var textureOffset = textureModel.GetTextureOffset(textureVariation);

                Vector2 scaleFactor = __instance.getScale();
                scaleFactor *= 4f;
                Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
                Rectangle destination = new Rectangle((int)(position.X - scaleFactor.X / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(position.Y - scaleFactor.Y / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(64f + scaleFactor.X), (int)(128f + scaleFactor.Y / 2f));
                float draw_layer = Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f;
                spriteBatch.Draw(textureModel.GetTexture(textureVariation), destination, new Rectangle(0, textureOffset, textureModel.TextureWidth, textureModel.TextureHeight), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);

                bool ringing = Phone.ringingTimer > 0 && Phone.ringingTimer < 600;
                position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
                destination = new Rectangle((int)position.X + ((ringing || __instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)position.Y + ((ringing || __instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), 64, 128);
                draw_layer = Math.Max(0f, (float)((y + 1) * 64 - 20) / 10000f) + (float)x * 1E-05f;
                spriteBatch.Draw(textureModel.GetTexture(textureVariation), destination, new Rectangle(16, textureOffset, 16, 32), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);

                return false;
            }
            return true;
        }
    }
}
