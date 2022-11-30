/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale.Patches
{
    class HitShaker : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(FarmerRenderer), "draw",

            new Type[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) });

        private static readonly Dictionary<long, int> hitShakeTimers = new();

        public static bool IsPlayerFlashing(long playerID) => hitShakeTimers.ContainsKey(playerID);

        private static bool AllowOneMethodCall = false;
        public static bool Prefix(FarmerRenderer __instance, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect, Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who)
        {
            if (AllowOneMethodCall)
            {
                AllowOneMethodCall = false;
                return true;
            }

            try
            {
                if (who != null && hitShakeTimers.ContainsKey(who.UniqueMultiplayerID))
                {
                    if (hitShakeTimers[who.UniqueMultiplayerID] % 100 < 50)
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return true;
            }

            AllowOneMethodCall = true;
            try
            {
                __instance.draw(b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who);
            }
            catch (Exception) { }
            AllowOneMethodCall = false;
            return false;
        }

        public static void SetHitShakeTimer(long playerID, int milliseconds)
        {
            if (!hitShakeTimers.ContainsKey(playerID))
                hitShakeTimers[playerID] = milliseconds;
        }

        public static void Update(GameTime gameTime)
        {
            foreach (long playerID in hitShakeTimers.Keys.ToList())
            {
                hitShakeTimers[playerID] = hitShakeTimers[playerID] - gameTime.ElapsedGameTime.Milliseconds;
                if (hitShakeTimers[playerID] <= 0)
                    hitShakeTimers.Remove(playerID);
            }
        }
    }
}
