using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MoreGrass.Patches
{
    /// <summary>Contains patches for patching game code in the StardewValley.TerrainFeatures.Grass class.</summary>
    internal class GrassPatch
    {
        /// <summary>This is code that will replace some game code, this is ran whenever the season gets updated. Used for ensuring grass doesn't get killing in winter.</summary>
        /// <param name="__instance">The current grass instance that is being patched.</param>
        /// <param name="__result">Always return false, this means the grass won't get killed.</param>
        /// <returns>False meaning the original method will never get ran.</returns>
        internal static bool SeasonUpdatePreFix(Grass __instance, ref bool __result)
        {
            // this will ensure the grass doesn't get killed in winter
            __result = false;
            __instance.loadSprite();

            return false;
        }

        /// <summary>This is code that will run after some game code, this is ran whenever the grass sprite gets loaded. Used for setting a custom sprite.</summary>
        /// <param name="__instance">The current grass instance that is being patched.</param>
        internal static void LoadSpritePostFix(Grass __instance)
        {
            FieldInfo texture = typeof(Grass).GetField("texture", BindingFlags.NonPublic | BindingFlags.Instance);

            Texture2D grassTexture = null;
            switch (Game1.currentSeason)
            {
                case "spring":
                    {
                        grassTexture = ModEntry.SpringGrassSprites[Game1.random.Next(ModEntry.SpringGrassSprites.Count)];
                        break;
                    }
                case "summer":
                    {
                        grassTexture = ModEntry.SummerGrassSprites[Game1.random.Next(ModEntry.SummerGrassSprites.Count)];
                        break;
                    }
                case "fall":
                    {
                        grassTexture = ModEntry.FallGrassSprites[Game1.random.Next(ModEntry.FallGrassSprites.Count)];
                        break;
                    }
                case "winter":
                    {
                        grassTexture = ModEntry.WinterGrassSprites[Game1.random.Next(ModEntry.WinterGrassSprites.Count)];
                        break;
                    }
            }

            texture.SetValue(__instance, new Lazy<Texture2D>(() => grassTexture));
            __instance.grassSourceOffset.Value = 0;
        }

        /// <summary>This is code that will run after some game code, this is used for setting 'whichWeed' which ensures the custom sprite is drawn correctly.</summary>
        /// <param name="__instance">The current grass instance that is being patched.</param>
        internal static void SetupRandomPostFix(Vector2 tileLocation, Grass __instance)
        {
            Random random = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed / 28 + (int)tileLocation.X * 7 + (int)tileLocation.Y * 11);

            FieldInfo whichWeed = typeof(Grass).GetField("whichWeed", BindingFlags.NonPublic | BindingFlags.Instance);

            int[] newWhichWeed = new int[4];
            for (int i  = 0; i < 4; i++)
            {
                switch (Game1.currentSeason)
                {
                    case "spring":
                        {
                            newWhichWeed[i] = random.Next(ModEntry.SpringGrassSprites.Count);
                            break;
                        }
                    case "summer":
                        {
                            newWhichWeed[i] = random.Next(ModEntry.SummerGrassSprites.Count);
                            break;
                        }
                    case "fall":
                        {
                            newWhichWeed[i] = random.Next(ModEntry.FallGrassSprites.Count);
                            break;
                        }
                    case "winter":
                        {
                            newWhichWeed[i] = random.Next(ModEntry.WinterGrassSprites.Count);
                            break;
                        }
                }
            }

            whichWeed.SetValue(__instance, newWhichWeed);
        }

        /// <summary>This is code that will run after some game code, this is used to draw the sprites correctly.</summary>
        /// <param name="spriteBatch">The sprite batch to draw the grass to.</param>
        /// <param name="tileLocation">The tile location of the current grass being drawn.</param>
        /// <param name="__instance">The grass instance to draw.</param>
        /// <returns>False meaning the original method will never get ran.</returns>
        internal static bool DrawPrefix(SpriteBatch spriteBatch, Vector2 tileLocation, Grass __instance)
        {
            // load all private fields that will be needed
            var offset1 = (int[])typeof(Grass).GetField("offset1", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var offset2 = (int[])typeof(Grass).GetField("offset2", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var offset3 = (int[])typeof(Grass).GetField("offset3", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var offset4 = (int[])typeof(Grass).GetField("offset4", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var whichWeed = (int[])typeof(Grass).GetField("whichWeed", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var shakeRotation = (float)typeof(Grass).GetField("shakeRotation", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var shakeRandom = (double[])typeof(Grass).GetField("shakeRandom", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var flip = (bool[])typeof(Grass).GetField("flip", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // cache the textures so they dont need to be fetched 4x more
            var textures = new List<Texture2D>();
            switch (Game1.currentSeason)
            {
                case "spring":
                    {
                        textures = ModEntry.SpringGrassSprites;
                        break;
                    }
                case "summer":
                    {
                        textures = ModEntry.SummerGrassSprites;
                        break;
                    }
                case "fall":
                    {
                        textures = ModEntry.FallGrassSprites;
                        break;
                    }
                case "winter":
                    {
                        textures = ModEntry.WinterGrassSprites;
                        break;
                    }
            }

            for (int i = 0; i < __instance.numberOfWeeds; i++)
            {
                Vector2 globalPosition = i != 4 
                    ? tileLocation * 64f + new Vector2((float)(i % 2 * 64 / 2 + offset3[i] * 4 - 4) + 30f, (float)(i / 2 * 64 / 2 + offset4[i] * 4 + 40)) 
                    : tileLocation * 64f + new Vector2((float)(16 + offset1[i] * 4 - 4) + 30f, (float)(16 + offset2[i] * 4 + 40));
                
                spriteBatch.Draw(
                    texture: textures[whichWeed[i]], 
                    position: Game1.GlobalToLocal(Game1.viewport, globalPosition), 
                    sourceRectangle: new Rectangle(0, 0, 15, 20), 
                    color: Color.White, 
                    rotation: shakeRotation / (float)(shakeRandom[i] + 1.0), 
                    origin: new Vector2(7.5f, 17.5f), 
                    scale: 4f, 
                    effects: flip[i] ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 
                    layerDepth: (float)(((double)globalPosition.Y + 16.0 - 20.0) / 10000.0 + (double)globalPosition.X / 10000000.0));
            }

            return false;
        }
    }
}
