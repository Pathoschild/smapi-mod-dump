/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Umbrellas;

[HarmonyPatch]
class HarmonyPatches
{
    internal static readonly ConditionalWeakTable<string, UmbrellaData> CachedUmbrellaData = new();

    [HarmonyPatch(typeof(NPC), nameof(NPC.draw))]
    [HarmonyPostfix]
    private static void PostDraw(NPC __instance, SpriteBatch b, int ___shakeTimer)
    {
        if (!AssetManager.Inclusions.ContainsKey(__instance.Name) || !AssetManager.Inclusions[__instance.Name]) return;
        if (Game1.eventUp || !Globals.UmbrellaNeeded) return;

        int frame = __instance.Sprite.CurrentFrame;
        if (frame > 15) return;

        Texture2D umbrellaTexture;

        if (CachedUmbrellaData.TryGetValue(__instance.Name, out UmbrellaData umbrellaData))
        {
            umbrellaTexture = umbrellaData.UmbrellaTexture;
        }
        // if not cached, get and add to cache
        else
        {
            if (AssetManager.UmbrellaData.ContainsKey(__instance.Name))
            {
                umbrellaData = AssetManager.UmbrellaData[__instance.Name];
                umbrellaTexture = umbrellaData.UmbrellaTexture;
                CachedUmbrellaData.AddOrUpdate(__instance.Name, umbrellaData);
            }
            else
            {
                umbrellaData = AssetManager.UmbrellaData["Default"];
                umbrellaTexture = umbrellaData.UmbrellaTexture;
                CachedUmbrellaData.AddOrUpdate(__instance.Name, umbrellaData);
            }
        }

        Vector2 shakeOffset = (___shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero;

        int facingDirection = __instance.getFacingDirection();

        bool foregroundHand = facingDirection is 2 || (facingDirection is 1 && !umbrellaData.LeftHanded) || (facingDirection is 3 && umbrellaData.LeftHanded);

        Vector2 handPositionOffset = umbrellaData.FrameOffsets[frame];
        Vector2 positionOffset = new Vector2(-48 + handPositionOffset.X * 4, -176 + handPositionOffset.Y * 4);
        Vector2 umbrellaOffset = new Vector2(umbrellaData.UmbrellaOffset.X, umbrellaData.UmbrellaOffset.Y * 4 - 12);

        Vector2 drawPosition = __instance.getLocalPosition(Game1.viewport) + positionOffset + shakeOffset;

        Rectangle handleSourceRect = new Rectangle(24 * facingDirection, 0, 24, 32);
        Rectangle umbrellaSourceRect = new Rectangle(24 * facingDirection, 32, 24, 32);

        float standingY = __instance.getStandingY();

        // breathing overlay gets drawn at __instance.getStandingY() / 10000f + 0.001f so to avoid z-fighting go up a tiny bit more
        float depth = foregroundHand ? standingY / 10000f + 0.0011f : standingY / 10000f - 0.0009f;

        b.Draw(
            umbrellaTexture,
            drawPosition,
			handleSourceRect,
            Color.White,
            0f,
            new Vector2(0, 0),
            new Vector2(4f, 4f),
            SpriteEffects.None,
            depth
        );

        b.Draw(
            umbrellaTexture,
            drawPosition + umbrellaOffset,
            umbrellaSourceRect,
            Color.White,
            0f,
            new Vector2(0, 0),
            new Vector2(4f, 4f),
            SpriteEffects.None,
            // resolve z-fighting between umbrellas on same y-coord by adding small x-coord offset to depth
            standingY / 10000f + __instance.getStandingX() / 1000000f + 0.0012f
        );
    }
}
