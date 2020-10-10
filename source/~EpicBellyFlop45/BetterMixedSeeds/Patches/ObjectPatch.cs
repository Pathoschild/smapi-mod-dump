/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using System;

namespace BetterMixedSeeds.Patches
{
    /// <summary>Contains patches for patching game code in the StardewValley.Object class.</summary>
    internal static class ObjectPatch
    {
        /// <summary>This is the code that will replace some game code, this is ran whenever the player is about to cut some weeds. Used for recalculating the drop chance for mixed seeds.</summary>
        /// <param name="who">The current farmer who is cutting the weeds.</param>
        /// <param name="__instance">The weeds object that is being cut.</param>
        /// <returns>Always return false as this patch includes the original game code. (Returning false means to not run the original method).</returns>
        internal static bool CutWeedPrefix(Farmer who, StardewValley.Object __instance)
        {
            // Custom added code for mixed seeds drop chance
            double upperBound = Math.Min(1, ModEntry.ModConfig.PercentDropChanceForMixedSeedsWhenNotFiber / 100f);
            double mixedSeedDropChance = Math.Round(Math.Max(0, upperBound), 3);

            int parentSheetIndex = -1;

            if (Game1.random.NextDouble() > 0.5)
            {
                parentSheetIndex = 771;
            }
            else if (Game1.random.NextDouble() < mixedSeedDropChance)
            {
                parentSheetIndex = 770;
            }

            // Default game method
            Color color = Color.Green;
            string audioName = "cut";
            int rowInAnimationTexture = 50;
            __instance.fragility.Value = 2;

            switch (__instance.parentSheetIndex)
            {
                case 313:
                case 314:
                case 315:
                    color = new Color(84, 101, 27);
                    break;
                case 316:
                case 317:
                case 318:
                    color = new Color(109, 49, 196);
                    break;
                case 319:
                    color = new Color(30, 216, (int)byte.MaxValue);
                    audioName = "breakingGlass";
                    rowInAnimationTexture = 47;
                    who.currentLocation.playSound("drumkit2", NetAudio.SoundContext.Default);
                    parentSheetIndex = -1;
                    break;
                case 320:
                    color = new Color(175, 143, (int)byte.MaxValue);
                    audioName = "breakingGlass";
                    rowInAnimationTexture = 47;
                    who.currentLocation.playSound("drumkit2", NetAudio.SoundContext.Default);
                    parentSheetIndex = -1;
                    break;
                case 321:
                    color = new Color(73, (int)byte.MaxValue, 158);
                    audioName = "breakingGlass";
                    rowInAnimationTexture = 47;
                    who.currentLocation.playSound("drumkit2", NetAudio.SoundContext.Default);
                    parentSheetIndex = -1;
                    break;
                case 678:
                    color = new Color(228, 109, 159);
                    break;
                case 679:
                    color = new Color(253, 191, 46);
                    break;
                case 792:
                case 793:
                case 794:
                    parentSheetIndex = 770;
                    break;
            }

            if (audioName.Equals("breakingGlass") && Game1.random.NextDouble() < 1.0 / 400.0)
            {
                parentSheetIndex = 338;
            }

            who.currentLocation.playSound(audioName, NetAudio.SoundContext.Default);

            StardewValley.Multiplayer multiplayer = ModEntry.ModHelper.Reflection.GetField<StardewValley.Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(rowInAnimationTexture, __instance.tileLocation.Value * 64f, color, 8, false, 100f, 0, -1, -1f, -1, 0));
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(rowInAnimationTexture, __instance.tileLocation.Value * 64f + new Vector2((float)Game1.random.Next(-16, 16), (float)Game1.random.Next(-48, 48)), color * 0.75f, 8, false, 100f, 0, -1, -1f, -1, 0)
            {
                scale = 0.75f,
                flipped = true
            });
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(rowInAnimationTexture, __instance.tileLocation.Value * 64f + new Vector2((float)Game1.random.Next(-16, 16), (float)Game1.random.Next(-48, 48)), color * 0.75f, 8, false, 100f, 0, -1, -1f, -1, 0)
            {
                scale = 0.75f,
                delayBeforeAnimationStart = 50
            });
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(rowInAnimationTexture, __instance.tileLocation.Value * 64f + new Vector2((float)Game1.random.Next(-16, 16), (float)Game1.random.Next(-48, 48)), color * 0.75f, 8, false, 100f, 0, -1, -1f, -1, 0)
            {
                scale = 0.75f,
                flipped = true,
                delayBeforeAnimationStart = 100
            });

            if (!audioName.Equals("breakingGlass") && Game1.random.NextDouble() < 1E-05)
            {
                who.currentLocation.debris.Add(new Debris((Item)new Hat(40), __instance.tileLocation.Value * 64f + new Vector2(32f, 32f)));
            }

            if (parentSheetIndex != -1)
            {
                who.currentLocation.debris.Add(new Debris((Item)new StardewValley.Object(parentSheetIndex, 1, false, -1, 0), __instance.tileLocation.Value * 64f + new Vector2(32f, 32f)));
            }

            if (Game1.random.NextDouble() < 0.02)
            {
                who.currentLocation.addJumperFrog(__instance.tileLocation);
            }

            if (!who.hasMagnifyingGlass || Game1.random.NextDouble() >= 0.009)
            {
                return false;
            }

            StardewValley.Object unseenSecretNote = who.currentLocation.tryToCreateUnseenSecretNote(who);

            if (unseenSecretNote == null)
            {
                return false;
            }

            Game1.createItemDebris((Item)unseenSecretNote, new Vector2(__instance.tileLocation.X + 0.5f, __instance.tileLocation.Y + 0.75f) * 64f, (int)Game1.player.facingDirection, who.currentLocation, -1);

            return false;
        }
    }
}
