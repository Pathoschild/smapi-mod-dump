/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rtrox/LineSprinklersRedux
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using LineSprinklersRedux.Framework.Data;
using System;
using StardewValley.ItemTypeDefinitions;

namespace LineSprinklersRedux.Framework
{
    public static class Sprinkler
    {


        public static bool IsLineSprinkler(SObject obj)
        {
            if (obj == null) return false;
            return obj.HasContextTag(ModConstants.MainContextTag);
        }
        public static IEnumerable<Vector2> GetCoverage(SObject sprinkler)
        {
            var tile = sprinkler.TileLocation;

            int range = Range(sprinkler);

            SprinklerDirection direction = ModData.GetDirection(sprinkler);
            switch (direction)
            {
                case SprinklerDirection.Right:
                    for (int i = 1; i <= range; i++)
                    {
                        yield return new Vector2(tile.X + i, tile.Y);
                    }
                    break;
                case SprinklerDirection.Down:
                    for (int i = 1; i <= range; i++)
                    {
                        yield return new Vector2(tile.X, tile.Y + i);
                    }
                    break;
                case SprinklerDirection.Left:
                    for (int i = 1; i <= range; i++)
                    {
                        yield return new Vector2(tile.X + (i * -1), tile.Y);
                    }
                    break;
                case SprinklerDirection.Up:
                    for (int i = 1; i <= range; i++)
                    {
                        yield return new Vector2(tile.X, tile.Y + (i * -1));
                    }
                    break;

            }
        }

        public static int Range(SObject sprinkler)
        {
            int baseRange = CustomFields.GetRange(sprinkler);
            if (HasPressureNozzle(sprinkler))
            {
                return baseRange * 2;
            }
            return baseRange;
        }
        public static void Rotate(SObject sprinkler)
        {
            var current = ModData.GetDirection(sprinkler);
            ModData.SetDirection(sprinkler, current.Cycle());
            SetSpriteFromRotation(sprinkler);


        }

        public static void SetSpriteFromRotation(SObject sprinkler)
        {
            var dir = ModData.GetDirection(sprinkler);
            int baseSprite = CustomFields.GetBaseSprite(sprinkler);
            sprinkler.ParentSheetIndex = baseSprite + (int)dir;
        }

        public static void ApplySprinklerAnimation(SObject sprinkler)
        {
            ApplySprinklerAnimation(sprinkler, Game1.random.Next(1000));
        }

        public static void ApplySprinklerAnimation(SObject sprinkler, int delayBeforeAnimationStart)
        {
            GameLocation location = sprinkler.Location;
            switch (Range(sprinkler))
            {
                case 4:
                    location.temporarySprites.Add(GetAnimatedSprite(sprinkler, new Rectangle(0, 0, 64, 256), delayBeforeAnimationStart));
                    break;
                case 8:
                    location.temporarySprites.Add(GetAnimatedSprite(sprinkler, new Rectangle(0, 256, 64, 512), delayBeforeAnimationStart));
                    break;
                case 16:
                    location.temporarySprites.Add(GetAnimatedSprite(sprinkler, new Rectangle(0, 768, 64, 1024), delayBeforeAnimationStart));
                    break;
                case 24:
                    location.temporarySprites.Add(GetAnimatedSprite(sprinkler, new Rectangle(256, 0, 64, 1536), delayBeforeAnimationStart));
                    break;
                case 48:
                    location.temporarySprites.Add(GetAnimatedSprite(sprinkler, new Rectangle(512, 0, 64, 3072), delayBeforeAnimationStart));
                    break;
                default:
                    location.temporarySprites.Add(GetAnimatedSprite(sprinkler, new Rectangle(0, 0, 64, 256), delayBeforeAnimationStart));
                    break;
            }

        }

        public static void ApplySprinkler(SObject sprinkler)
        {
            if (sprinkler == null || !IsLineSprinkler(sprinkler)) return;
            foreach (var tile in sprinkler.GetSprinklerTiles())
            {
                sprinkler.ApplySprinkler(tile);
            }
        }



        private static TemporaryAnimatedSprite GetAnimatedSprite(SObject sprinkler, Rectangle sourceRect, int delayBeforeAnimationStart)
        {
            var dir = ModData.GetDirection(sprinkler);
            ModEntry.Mon!.Log($"Direction: {dir}", LogLevel.Debug);
            float rotation = dir switch
            {
                SprinklerDirection.Up => 0f,
                SprinklerDirection.Right => MathF.PI / 2f,
                SprinklerDirection.Down => MathF.PI,
                SprinklerDirection.Left => 4.712389f,
                _ => 0f,
            };
            Vector2 relativePosition = dir switch
            {
                SprinklerDirection.Up => new Vector2(0f, -1f * sourceRect.Height),
                SprinklerDirection.Right => new Vector2(sourceRect.Height / 2f + 32f, (sourceRect.Height / 2f - 26f) * -1),
                SprinklerDirection.Down => new Vector2(0f, 64f),
                SprinklerDirection.Left => new Vector2((sourceRect.Height / 2f + 32f) * -1, (sourceRect.Height / 2f - 26f) * -1),
                _ => new Vector2(),

            };
            return new TemporaryAnimatedSprite(
                $"/Mods/{ModConstants.ModKeySpace}/Animations",
                sourceRect,
                60f,
                4,
                100,
                sprinkler.TileLocation * 64f + relativePosition,
                flicker: false,
                flipped: false)
            {
                color = Color.White * 0.4f,
                delayBeforeAnimationStart = delayBeforeAnimationStart,
                id = (int)sprinkler.TileLocation.X * 4000 + (int)sprinkler.TileLocation.Y,
                rotation = rotation,
            };
        }

        public static void DrawAttachments(SObject sprinkler, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            Microsoft.Xna.Framework.Rectangle boundingBoxAt = sprinkler.GetBoundingBoxAt(x, y);
            if (sprinkler.heldObject.Value != null)
            {
                Vector2 vector3 = Vector2.Zero;
                if (sprinkler.heldObject.Value.QualifiedItemId == "(O)913")
                {
                    vector3 = new Vector2(0f, -20f);
                }

                // Override texture for Pressure Nozzles to be directional.
                ParsedItemData heldItemData = ItemRegistry.GetDataOrErrorItem(sprinkler.heldObject.Value.QualifiedItemId);
                var sourceRect = heldItemData.GetSourceRect(1);
                if (HasPressureNozzle(sprinkler))
                {
                    heldItemData = ItemRegistry.GetDataOrErrorItem(ModConstants.OverlayDummyItemID);
                    sourceRect = heldItemData.GetSourceRect(spriteIndex: (int)ModData.GetDirection(sprinkler));
                }

                spriteBatch.Draw(
                    heldItemData.GetTexture(),
                    Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32 + ((sprinkler.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), y * 64 + 32 + ((sprinkler.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0)) + vector3),
                    sourceRect,
                    Color.White * alpha,
                    0f,
                    new Vector2(8f, 8f),
                    (sprinkler.scale.Y > 1f) ? sprinkler.getScale().Y : 4f,
                    sprinkler.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    (float)(sprinkler.isPassable() ? boundingBoxAt.Top : boundingBoxAt.Bottom) / 10000f + 1E-05f);
            }
            if (sprinkler.SpecialVariable == 999999)
            {
                if (sprinkler.heldObject.Value != null && sprinkler.heldObject.Value.QualifiedItemId == "(O)913")
                {
                    Torch.drawBasicTorch(spriteBatch, (float)(x * 64) - 2f, y * 64 - 32, (float)boundingBoxAt.Bottom / 10000f + 1E-06f);
                }
                else
                {
                    Torch.drawBasicTorch(spriteBatch, (float)(x * 64) - 2f, y * 64 - 32 + 12, (float)(boundingBoxAt.Bottom + 2) / 10000f);
                }
            }
        }

        private static bool HasPressureNozzle(SObject sprinkler)
        {
            if (!IsLineSprinkler(sprinkler)) return false;
            if (sprinkler.heldObject.Value != null)
            {
                return sprinkler.heldObject.Value.QualifiedItemId == "(O)915";
            }
            return false;
        }
    }
}

