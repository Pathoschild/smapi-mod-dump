/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Objects.Weapons;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Archery.Framework.Patches.Renderer
{
    internal class DrawPatch : PatchTemplate
    {
        private readonly Type _object = typeof(FarmerRenderer);

        public DrawPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(FarmerRenderer.draw), new[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }), transpiler: new HarmonyMethod(GetType(), nameof(DrawTranspiler)));

            harmony.Patch(AccessTools.Method(_object, "executeRecolorActions", new[] { typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(ExecuteRecolorActionsPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(FarmerRenderer.draw), new[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(DrawPostfix)));
        }

        private static void DrawPostfix(FarmerRenderer __instance, LocalizedContentManager ___farmerTextureManager, Texture2D ___baseTexture, NetInt ___skin, ref Rectangle ___hairstyleSourceRect, ref Rectangle ___shirtSourceRect, ref Rectangle ___accessorySourceRect, ref Rectangle ___hatSourceRect, ref Vector2 ___positionOffset, ref Vector2 ___rotationAdjustment, ref bool ____sickFrame, ref bool ____shirtDirty, ref bool ____spriteDirty, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect, Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who)
        {
            if (Archery.apiManager.IsFashionSenseLoaded() is true)
            {
                return;
            }

            if (Game1.player.UsingTool is true && Bow.IsValid(Game1.player.CurrentTool) is true)
            {
                Bow.Draw(who, b, Game1.player.FacingDirection == Game1.up ? 0f : layerDepth, position, ___positionOffset, origin, rotation, scale, overrideColor);
            }
        }

        private static bool ExecuteRecolorActionsPrefix(FarmerRenderer __instance, bool ____spriteDirty, Farmer farmer)
        {
            if (Archery.apiManager.IsFashionSenseLoaded() is false && ____spriteDirty)
            {
                Archery.RecolorSleeves();
            }

            return true;
        }

        private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var list = instructions.ToList();
                object returnLabel = null;

                // Get the indices to insert at
                List<int> indices = new List<int>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].opcode == OpCodes.Brfalse && list[i - 1].opcode == OpCodes.Ldfld && list[i - 1].operand.ToString().Contains("Slingshot", StringComparison.OrdinalIgnoreCase))
                    {
                        returnLabel = list[i].operand;
                        continue;
                    }

                    if (list[i].opcode == OpCodes.Call && list[i].operand is not null && list[i].operand.ToString().Contains("drawHairAndAccesories", StringComparison.OrdinalIgnoreCase))
                    {
                        indices.Add(i);
                    }
                }

                // Insert the changes at the specified indices
                list.Insert(indices.Last() + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DrawPatch), nameof(ShouldHideSleeves))));
                list.Insert(indices.Last() + 2, new CodeInstruction(OpCodes.Brtrue_S, returnLabel));

                return list;
            }
            catch (Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for StardewValley.FarmerRenderer.draw: {e}", LogLevel.Error);
                return instructions;
            }
        }

        private static bool ShouldHideSleeves()
        {
            if (Archery.apiManager.IsFashionSenseLoaded() is true || Game1.player.UsingTool is false || Bow.IsValid(Game1.player.CurrentTool) is false)
            {
                return false;
            }

            return true;
        }
    }
}