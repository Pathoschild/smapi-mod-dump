/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace MoreGrass.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Grass"/> class.</summary>
    internal class GrassPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="Grass.reduceBy(int, Vector2, bool)"/> method.</summary>
        /// <param name="tileLocation">The tile location of the grass.</param>
        /// <param name="showDebris">Whether debris should be drawn.</param>
        /// <param name="__instance">The current <see cref="Grass"/> instance that is being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise <see langword="false"/> (depending on the mod configuration).</returns>
        /// <remarks>This is used so animals won't eat grass if the configuration forbids them from it.</remarks>
        internal static bool ReduceByPrefix(Vector2 tileLocation, bool showDebris, Grass __instance, ref bool __result)
        {
            // ensure animals aren't allowed to eat grass
            if (ModEntry.Instance.Config.CanAnimalsEatGrass)
                return true;

            // reimplement the method and force the return value to be false (meaning the grass won't get destroyed)
            if (showDebris)
                Game1.createRadialDebris(Game1.currentLocation, __instance.textureName(), new Rectangle(2, 8, 8, 8), 1, ((int)tileLocation.X + 1) * 64, ((int)tileLocation.Y + 1) * 64, Game1.random.Next(6, 14), (int)tileLocation.Y + 1, Color.White, 4);
            __result = false;
            return false;
        }

        /// <summary>The prefix for the <see cref="Grass.seasonUpdate(bool)"/> method.</summary>
        /// <param name="__instance">The current <see cref="Grass"/> instance that is being patched.</param>
        /// <param name="__result">Whether all the grass should be killed (this is the return value of the original method).</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This is used to determine if grass should get killed at the beginning of a new season based on the mod configuration.</remarks>
        internal static bool SeasonUpdatePrefix(Grass __instance, ref bool __result)
        {
            switch (Game1.currentSeason)
            {
                case "spring": __result = !ModEntry.Instance.Config.CanGrassLiveInSpring; break;
                case "summer": __result = !ModEntry.Instance.Config.CanGrassLiveInSummer; break;
                case "fall": __result = !ModEntry.Instance.Config.CanGrassLiveInFall; break;
                case "winter": __result = !ModEntry.Instance.Config.CanGrassLiveInWinter; break;
            }

            // recalculate the new textures for grass
            __instance.loadSprite(); 
            __instance.setUpRandom(__instance.currentTileLocation);

            return false;
        }

        /// <summary>The post fix for the <see cref="Grass.loadSprite"/> method.</summary>
        /// <param name="__instance">The current <see cref="Grass"/> instance that is being patched.</param>
        /// <remarks>This is used to load the custom grass sprite.</remarks>
        internal static void LoadSpritePostFix(Grass __instance)
        {
            Texture2D grassTexture = null;
            switch (Game1.currentSeason)
            {
                case "spring": grassTexture = ModEntry.Instance.SpringSpritePool.GetRandomSprite(__instance.currentLocation?.Name); break;
                case "summer": grassTexture = ModEntry.Instance.SummerSpritePool.GetRandomSprite(__instance.currentLocation?.Name); break;
                case "fall": grassTexture = ModEntry.Instance.FallSpritePool.GetRandomSprite(__instance.currentLocation?.Name); break;
                case "winter": grassTexture = ModEntry.Instance.WinterSpritePool.GetRandomSprite(__instance.currentLocation?.Name); break;
            }

            __instance.texture = new Lazy<Texture2D>(() => grassTexture);
            __instance.grassSourceOffset.Value = 0;
        }

        /// <summary>The transpiler for the <see cref="Grass.performToolAction(Tool, int, Vector2, GameLocation)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to change the colour of the break animation of grass in winter.</remarks>
        internal static IEnumerable<CodeInstruction> PerformToolActionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Call && instruction.operand == typeof(Color).GetMethod("get_Green", BindingFlags.Public | BindingFlags.Static))
                    instruction.operand = typeof(Color).GetMethod("get_DarkTurquoise", BindingFlags.Public | BindingFlags.Static);

                yield return instruction;
            }
        }

        /// <summary>The post fix for the <see cref="Grass.setUpRandom(Vector2)"/> method.</summary>
        /// <param name="__instance">The current <see cref="Grass"/> instance that is being patched.</param>
        /// <remarks>This is used for setting the 'whichWeed' member which ensures the custom sprite is drawn correctly.</remarks>
        internal static void SetupRandomPostFix(Vector2 tileLocation, Grass __instance)
        {
            var random = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed / 28 + (int)tileLocation.X * 7 + (int)tileLocation.Y * 11);

            // calculate the 'whichWeed' value
            var newWhichWeed = new int[4];
            for (int i = 0; i < 4; i++)
            {
                switch (Game1.currentSeason)
                {
                    case "spring": newWhichWeed[i] = random.Next(ModEntry.Instance.SpringSpritePool.GetSprites(__instance.currentLocation.Name).Count); break;
                    case "summer": newWhichWeed[i] = random.Next(ModEntry.Instance.SummerSpritePool.GetSprites(__instance.currentLocation.Name).Count); break;
                    case "fall": newWhichWeed[i] = random.Next(ModEntry.Instance.FallSpritePool.GetSprites(__instance.currentLocation.Name).Count); break;
                    case "winter": newWhichWeed[i] = random.Next(ModEntry.Instance.WinterSpritePool.GetSprites(__instance.currentLocation.Name).Count); break;
                }
            }

            // update the member on the grass instance
            typeof(Grass).GetField("whichWeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, newWhichWeed);
        }

        /// <summary>The prefix for the <see cref="Grass.draw(SpriteBatch, Vector2)(bool)"/> method.</summary>
        /// <param name="spriteBatch">The sprite batch to draw the grass to.</param>
        /// <param name="tileLocation">The tile location of the current grass being drawn.</param>
        /// <param name="__instance">The current <see cref="Grass"/> instance that is being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This is used to draw the grass sprites.</remarks>
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

            // cache the textures
            var defaultTextures = new List<Texture2D>();
            var textures = new List<Texture2D>();
            switch (Game1.currentSeason)
            {
                case "spring": textures = ModEntry.Instance.SpringSpritePool.GetSprites(__instance.currentLocation.Name); defaultTextures = ModEntry.Instance.SpringSpritePool.DefaultSprites; break;
                case "summer": textures = ModEntry.Instance.SummerSpritePool.GetSprites(__instance.currentLocation.Name); defaultTextures = ModEntry.Instance.SummerSpritePool.DefaultSprites; break;
                case "fall":   textures = ModEntry.Instance.FallSpritePool.GetSprites(__instance.currentLocation.Name);   defaultTextures = ModEntry.Instance.FallSpritePool.DefaultSprites;   break;
                case "winter": textures = ModEntry.Instance.WinterSpritePool.GetSprites(__instance.currentLocation.Name); defaultTextures = ModEntry.Instance.WinterSpritePool.DefaultSprites; break;
            }

            // force default grass based on user specified white/black list configuration
            var forceDefaultGrass = Utilities.ShouldForceGrassToDefault(__instance.currentLocation.Name);

            // draw the grass
            var random = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed / 28 + (int)tileLocation.X * 7 * (int)tileLocation.Y * 11);
            for (int i = 0; i < __instance.numberOfWeeds; i++)
            {
                var grassId = random.Next(textures.Count);

                // force default grass based on coverage configuration
                if (random.NextDouble() < (ModEntry.Instance.Config.PercentConverageOfDefaultGrass / 100f)
                    || forceDefaultGrass)
                    grassId = random.Next(defaultTextures.Count);

                // draw grass
                var globalPosition = i != 4
                    ? tileLocation * 64f + new Vector2(x: i % 2 * 64 / 2 + offset3[i] * 4 - 4 + 30, y: i / 2 * 64 / 2 + offset4[i] * 4 + 40)
                    : tileLocation * 64f + new Vector2(x: 16 + offset1[i] * 4 - 4 + 30, y: 16 + offset2[i] * 4 + 40);
                
                spriteBatch.Draw(
                    texture: forceDefaultGrass ? defaultTextures[grassId] : textures[grassId],
                    position: Game1.GlobalToLocal(Game1.viewport, globalPosition),
                    sourceRectangle: new Rectangle(0, 0, 15, 20),
                    color: Color.White,
                    rotation: shakeRotation / (float)(shakeRandom[i] + 1),
                    origin: new Vector2(7.5f, 17.5f),
                    scale: 4f,
                    effects: flip[i] ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    layerDepth: (globalPosition.Y + 16 - 20) / 10000f + globalPosition.X / 10000000f
                );
            }

            return false;
        }
    }
}
