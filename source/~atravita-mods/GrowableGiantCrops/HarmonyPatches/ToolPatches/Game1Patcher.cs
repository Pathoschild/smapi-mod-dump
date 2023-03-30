/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using GrowableGiantCrops.Framework;
using GrowableGiantCrops.Framework.Assets;
using HarmonyLib;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrowableGiantCrops.HarmonyPatches.ToolPatches;

/// <summary>
/// Patches on Game1.
/// </summary>
[HarmonyPatch(typeof(Game1))]
internal static class Game1Patcher
{
    /// <summary>
    /// Draws the shovel.
    /// </summary>
    /// <param name="f">Farmer.</param>
    /// <param name="currentToolIndex">the current tool index.</param>
    /// <returns>true to continue on to the vanilla method, false to skip.</returns>
    [HarmonyPatch(nameof(Game1.drawTool), new[] { typeof(Farmer), typeof(int) })]
    private static bool Prefix(Farmer f, int currentToolIndex)
    {
        if (f.CurrentTool is not ShovelTool shovel)
        {
            return true;
        }

        int xindex = (currentToolIndex * 16) % shovel.GetTexture().Width;
        Rectangle sourceRectangleForTool = new(xindex, shovel.UpgradeLevel * 32, 16, 32);
        Vector2 fPosition = f.getLocalPosition(Game1.viewport) + f.jitter + f.armOffset;
        float tool_draw_layer_offset = 0f;
        if (f.FacingDirection == 0)
        {
            tool_draw_layer_offset = -0.002f;
        }

        if (Game1.pickingTool)
        {
            int yLocation = (int)fPosition.Y - 128;
            Game1.spriteBatch.Draw(
                texture: shovel.GetTexture(),
                new Vector2(fPosition.X, yLocation),
                sourceRectangle: sourceRectangleForTool,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: Math.Max(0f, tool_draw_layer_offset + ((f.getStandingY() + 32) / 10000f)));
            return false;
        }

        shovel.draw(Game1.spriteBatch);

        Vector2 position;
        float rotation = 0f;
        Vector2 origin = new(0f, 16f);
        SpriteEffects effect = SpriteEffects.None;

        switch (f.FacingDirection)
        {
            case Game1.up:
                switch (f.Sprite.currentAnimationIndex)
                {
                    case 0:
                        position = Utility.snapToInt(new Vector2(fPosition.X, fPosition.Y - 64f));
                        break;
                    case 1:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 4f, fPosition.Y - 128f + 40f));
                        break;
                    case 2:
                        position = Utility.snapToInt(new Vector2(fPosition.X, fPosition.Y - 128f + 24f));
                        break;
                    case 3:
                        position = Utility.snapToInt(new Vector2(fPosition.X, fPosition.Y - 128f));
                        break;
                    default:
                        return false;
                }
                break;
            case Game1.right:
                origin = new Vector2(0f, 32f);
                effect = SpriteEffects.FlipHorizontally;
                switch (f.Sprite.currentAnimationIndex)
                {
                    case 0:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 72f, fPosition.Y - 28f));
                        rotation = MathF.PI * 10f / 12f;
                        break;
                    case 1:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 76f, fPosition.Y - 20f));
                        rotation = MathF.PI * 11f / 12f;
                        break;
                    case 2:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 74f, fPosition.Y - 40f));
                        rotation = MathF.PI * 10f / 12f;
                        break;
                    case 3:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 32f + 28f, fPosition.Y - 86f));
                        rotation = MathF.PI * 7f / 12f;
                        break;
                    case 4:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 32f + 28f, fPosition.Y - 64f + 4f));
                        rotation = MathF.PI * 7f / 12f;
                        break;
                    default:
                        return false;
                }
                break;
            case Game1.down:
                switch (f.Sprite.currentAnimationIndex)
                {
                    case 0:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 60f, fPosition.Y + 64f));
                        rotation = MathF.PI;
                        break;
                    case 1:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 60f, fPosition.Y + 56f));
                        rotation = MathF.PI;
                        break;
                    case 2:
                        position = Utility.snapToInt(new Vector2(fPosition.X, fPosition.Y - 44f));
                        break;
                    case 3:
                        position = Utility.snapToInt(new Vector2(fPosition.X, fPosition.Y - 56f));
                        break;
                    case 4:
                        position = Utility.snapToInt(new Vector2(fPosition.X, fPosition.Y - 60f));
                        break;
                    default:
                        return true;
                }
                break;
            case Game1.left:
                origin = new Vector2(0f, 32f);
                switch (f.Sprite.currentAnimationIndex)
                {
                    case 0:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 42f, fPosition.Y + 8f));
                        rotation = -MathF.PI * 10f / 12f;
                        break;
                    case 1:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 54f, fPosition.Y));
                        rotation = -MathF.PI * 11f / 12f;
                        break;
                    case 2:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 54f, fPosition.Y));
                        rotation = -MathF.PI * 10f / 12f;
                        break;
                    case 3:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 34f, fPosition.Y - 21f));
                        rotation = -MathF.PI * 7f / 12f;
                        break;
                    case 4:
                        position = Utility.snapToInt(new Vector2(fPosition.X + 34f, fPosition.Y));
                        rotation = -MathF.PI * 7f / 12f;
                        break;
                    default:
                        return false;
                }
                break;
            default:
                return false;
        }

        Game1.spriteBatch.Draw(
            texture: shovel.GetTexture(),
            position,
            sourceRectangle: sourceRectangleForTool,
            color: Color.White,
            rotation,
            origin,
            scale: 4f,
            effects: effect,
            layerDepth: Math.Max(0f, tool_draw_layer_offset + (f.GetBoundingBox().Bottom / 10000f)));

        return false;
    }
}
