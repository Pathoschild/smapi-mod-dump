/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/urbanyeti/stardew-better-friendship
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace BetterFriendship
{
    public class BubbleDrawer
    {
        private Vector2 BubbleOffset { get; set; }
        private readonly ModConfig _config;
        private readonly Dictionary<string, (int item, double time)> _lastCycled = new();
        private readonly Rectangle _giftSourceRectangle = new(167, 175, 12, 11);
        private readonly Rectangle _talkSourceRectangle = new(181, 175, 12, 11);
        private readonly Texture2D _emojiTexture2D;

        internal BubbleDrawer(ModConfig config)
        {
            _config = config;
            BubbleOffset = new Vector2(-6, 120);
            _emojiTexture2D = Game1.content.Load<Texture2D>("LooseSprites\\emojis");
        }

        public void UpdateOffset(Vector2 offset)
        {
            BubbleOffset += offset;
        }

        public void DrawBubble(SpriteBatch spriteBatch, Character character, List<(Object item, int taste)> bestItems,
            bool displayGift, bool displayTalk)
        {
            if (!displayGift && !displayTalk)
            {
                return;
            }

            character.Position.Deconstruct(out var xPosition, out var yPosition);

            var hoverVal = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2)) -
                           (Game1.tileSize / 2);

            var selectedItem = displayGift ? SelectIem(character.Name, bestItems) : -1;

            // Icon(s)
            switch (displayGift)
            {
                case true when displayTalk:
                    if (selectedItem == -1)
                    {
                        if (!_config.DisplayGenericGiftPrompts && !character.ShouldOverrideForSpouse(_config))
                        {
                            break;
                        }

                        DrawThoughtBubble(spriteBatch, xPosition, yPosition + hoverVal);

                        spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(
                                xPosition + BubbleOffset.X + 6,
                                yPosition - BubbleOffset.Y + hoverVal + 10)),
                            _giftSourceRectangle,
                            Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                    }
                    else
                    {
                        DrawThoughtBubble(spriteBatch, xPosition, yPosition + hoverVal);

                        var itemData = ItemRegistry.GetData(bestItems[selectedItem].item.QualifiedItemId);

                        spriteBatch.Draw(itemData.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(
                                xPosition + BubbleOffset.X + 6,
                                yPosition - BubbleOffset.Y + hoverVal + 10)),
                            itemData.GetSourceRect(),
                            Color.White * 0.75f, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1);
                    }

                    break;
                case true:
                    if (selectedItem == -1)
                    {
                        if (!_config.DisplayGenericGiftPrompts && !character.ShouldOverrideForSpouse(_config))
                        {
                            break;
                        }

                        DrawThoughtBubble(spriteBatch, xPosition, yPosition + hoverVal);

                        spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(
                                xPosition + BubbleOffset.X + 16,
                                yPosition - BubbleOffset.Y + hoverVal + 16)),
                            _giftSourceRectangle,
                            Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                        break;
                    }
                    else
                    {
                        DrawThoughtBubble(spriteBatch, xPosition, yPosition + hoverVal);

                        var itemData = ItemRegistry.GetData(bestItems[selectedItem].item.QualifiedItemId);

                        spriteBatch.Draw(itemData.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(
                                xPosition + BubbleOffset.X + 8,
                                yPosition - BubbleOffset.Y + hoverVal + 8)),
                            itemData.GetSourceRect(),
                            Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
                            1);

                        var smileySourceRect = GetSmileySourceRect(bestItems[selectedItem].taste);

                        spriteBatch.Draw(_emojiTexture2D, Game1.GlobalToLocal(Game1.viewport, new Vector2(
                                xPosition + BubbleOffset.X + 50,
                                yPosition - BubbleOffset.Y + hoverVal + 60)),
                            smileySourceRect,
                            Color.White * 0.75f, 0.0f, Vector2.Zero, 3f, SpriteEffects.None,
                            2);

                        if (bestItems[selectedItem].item.Quality is 0) return;

                        var qualitySourceRect = GetQualitySourceRect(bestItems[selectedItem].item.Quality);

                        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(
                                xPosition + BubbleOffset.X + 3,
                                yPosition - BubbleOffset.Y + hoverVal + 61)),
                            qualitySourceRect,
                            Color.White * 0.75f, 0.0f, Vector2.Zero, 3f, SpriteEffects.None,
                            2);

                        break;
                    }
            }

            switch (displayTalk)
            {
                case true when displayGift && (_config.DisplayGenericGiftPrompts || selectedItem != -1 ||
                                               character.ShouldOverrideForSpouse(_config)):
                    spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(
                            xPosition + BubbleOffset.X + 22,
                            yPosition - BubbleOffset.Y + 30 + hoverVal)),
                        _talkSourceRectangle,
                        Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
                        2);
                    break;
                case true:
                    DrawThoughtBubble(spriteBatch, xPosition, yPosition + hoverVal);

                    spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(
                            xPosition + BubbleOffset.X + 16,
                            yPosition - BubbleOffset.Y + 16 + hoverVal)),
                        _talkSourceRectangle,
                        Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
                        1);
                    break;
            }
        }

        private static Rectangle GetQualitySourceRect(int quality)
        {
            return quality < 4 ? new Rectangle(338 + (quality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8);
        }

        private static Rectangle GetSmileySourceRect(int taste) =>
            taste switch
            {
                0 => new Rectangle(9, 27, 9, 9),
                2 => new Rectangle(0, 0, 9, 9),
                not 4 or 6 => new Rectangle(18, 9, 9, 9),
                _ => new Rectangle(0, 0, 0, 0)
            };

        private int SelectIem(string npcName, IReadOnlyCollection<(Object item, int taste)> bestItems)
        {
            if (bestItems == null || !bestItems.Any() || !_lastCycled.ContainsKey(npcName) ||
                _lastCycled[npcName].item >= bestItems.Count)
            {
                _lastCycled[npcName] = (0, 0);
                return -1;
            }

            if (!(Game1.currentGameTime.TotalGameTime.TotalMilliseconds >=
                  _lastCycled[npcName].time + _config.GiftCycleDelay))
                return _lastCycled[npcName].item;
            _lastCycled[npcName] = ((_lastCycled[npcName].item + 1) % bestItems.Count,
                Game1.currentGameTime.TotalGameTime.TotalMilliseconds);

            return _lastCycled[npcName].item;
        }

        private void DrawThoughtBubble(SpriteBatch spriteBatch, float xPosition, float yPosition)
        {
            spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(
                    xPosition + BubbleOffset.X,
                    yPosition - BubbleOffset.Y)),
                new Rectangle(141, 465, 20, 24),
                Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0);
        }
    }
}