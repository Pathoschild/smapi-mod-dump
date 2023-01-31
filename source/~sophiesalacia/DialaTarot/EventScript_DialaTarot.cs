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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DialaTarotCSharp
{
    internal class EventScriptDialaTarot : ICustomEventScript
    {
        private readonly Vector2 ScreenCenterPosition;
        private readonly Texture2D CardBackTexture;

        private const int CardWidth = 92;
        private const int CardHeight = 139;

        private const int SpacerWidth = 40;

        private Vector2 Card1Pos;
        private Vector2 Card2Pos;
        private Vector2 Card3Pos;

        private readonly Vector2 TextPos;

        private float Card1SquishFactor;
        private float Card2SquishFactor;
        private float Card3SquishFactor;

        private int PhaseTimer;
        private int Phase;

        private readonly TarotCard Card1;
        private readonly TarotCard Card2;
        private readonly TarotCard Card3;

        private float TextOpacity = 0f;
        private float BgOpacity = 0f;

        public EventScriptDialaTarot()
        {
            ScreenCenterPosition = new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width / 2f,
                Game1.graphics.GraphicsDevice.Viewport.Height / 2f);
            //e.aboveMapSprites.Add(new TemporaryAnimatedSprite());
            CardBackTexture = Game1.content.Load<Texture2D>("sophie.DialaTarot/CardBack");

            Card2Pos = ScreenCenterPosition + new Vector2(0f - CardWidth * 2, -1200f);
            Card1Pos = Card2Pos + new Vector2(0f - SpacerWidth - CardWidth * 4, 0f);
            Card3Pos = Card2Pos + new Vector2(SpacerWidth + CardWidth * 4, 0f);

            TextPos = ScreenCenterPosition + new Vector2(-6 * CardWidth, CardHeight * 2 + SpacerWidth);

            Card1SquishFactor = 4f;
            Card2SquishFactor = 4f;
            Card3SquishFactor = 4f;

            Phase = 0;
            PhaseTimer = 3000;

            List<int> cards = new() {2, 3, 4, 5, 7, 8, 9, 10};
            if (Game1.player.isMarried())
            {
                cards.Add(1);
                cards.Add(6);
            }
            else if (Game1.player.isEngaged())
            {
                cards.Add(1);
            }

            Random rand = new();
            int randNum = rand.Next(0, cards.Count);
            Card1 = new TarotCard(cards[randNum]);
            cards.RemoveAt(randNum);
            randNum = rand.Next(0, cards.Count);
            Card2 = new TarotCard(cards[randNum]);
            cards.RemoveAt(randNum);
            randNum = rand.Next(0, cards.Count);
            Card3 = new TarotCard(cards[randNum]);

            Card1.Buff.Invoke();
            Card2.Buff.Invoke();
            Card3.Buff.Invoke();
        }

        public void drawAboveAlwaysFront(SpriteBatch b)
        {
            // black background
            b.Draw(
                Game1.staminaRect,
                new Rectangle(0, 0, 1920, 1080),
                Game1.staminaRect.Bounds,
                Color.Black,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0f
            );

            b.Draw(
                Game1.mouseCursors,
                Vector2.Zero,
                new Rectangle(0, 1453, 638, 195),
                Color.White * BgOpacity,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                0f
            );

            b.Draw(
                Game1.mouseCursors,
                new Vector2(0f, 781),
                new Rectangle(0, 1453, 638, 195),
                Color.White * BgOpacity,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                0f
            );

            b.Draw(
                CardBackTexture,
                Card1Pos,
                new Rectangle(0, 0, 92, 139),
                Color.White,
                0f,
                new Vector2(0f, 0f),
                new Vector2(Card1SquishFactor, 4f),
                SpriteEffects.None,
                layerDepth: 1f
            );

            if (Phase is >= 5 and <= 19)
            {
                b.Draw(
                    Card1.Texture,
                    Card1Pos,
                    new Rectangle(0, 0, 92, 139),
                    Color.White,
                    0f,
                    new Vector2(0f, 0f),
                    new Vector2(Card1SquishFactor, 4f),
                    SpriteEffects.None,
                    layerDepth: 2f
                );
            }

            b.Draw(
                CardBackTexture,
                Card2Pos,
                null,
                Color.White,
                0f,
                new Vector2(0f, 0f),
                new Vector2(Card2SquishFactor, 4f),
                SpriteEffects.None,
                layerDepth: 1f
            );

            if (Phase is >= 10 and <= 19)
            {
                b.Draw(
                    Card2.Texture,
                    Card2Pos,
                    new Rectangle(0, 0, 92, 139),
                    Color.White,
                    0f,
                    new Vector2(0f, 0f),
                    new Vector2(Card2SquishFactor, 4f),
                    SpriteEffects.None,
                    layerDepth: 2f
                );
            }

            b.Draw(
                CardBackTexture,
                Card3Pos,
                null,
                Color.White,
                0f,
                new Vector2(0f, 0f),
                new Vector2(Card3SquishFactor, 4f),
                SpriteEffects.None,
                layerDepth: 1f
            );

            if (Phase is >= 15 and <= 19)
            {
                b.Draw(
                    Card3.Texture,
                    Card3Pos,
                    new Rectangle(0, 0, 92, 139),
                    Color.White,
                    0f,
                    new Vector2(0f, 0f),
                    new Vector2(Card3SquishFactor, 4f),
                    SpriteEffects.None,
                    layerDepth: 2f
                );
            }

            if (Phase is 6 or 7 or 8)
            {
                b.DrawString(
                    Game1.dialogueFont,
                    Game1.parseText($"{Card1.Name}: {Card1.Description}", Game1.dialogueFont, CardWidth * 12),
                    TextPos,
                    Color.White * (TextOpacity)
                );

                
            }

            if (Phase is 11 or 12 or 13)
            {
                b.DrawString(
                    Game1.dialogueFont,
                    Game1.parseText($"{Card2.Name}: {Card2.Description}", Game1.dialogueFont, CardWidth * 12),
                TextPos,
                    Color.White * (TextOpacity)
                );
            }

            if (Phase is 16 or 17 or 18)
            {
                b.DrawString(
                    Game1.dialogueFont,
                    Game1.parseText($"{Card3.Name}: {Card3.Description}", Game1.dialogueFont, CardWidth * 12),
                    //screenCenterPosition + new Vector2(-(Game1.dialogueFont.MeasureString(card3.Description).X / 2), cardHeight * 2 + spacerWidth),
                    TextPos,
                    Color.White * (TextOpacity)
                );
            }
        }

        public void draw(SpriteBatch b)
        {
        }

        public bool update(GameTime time, Event e)
        {
            switch (Phase)
            {
                case 0:
                {
                    PhaseTimer -= time.ElapsedGameTime.Milliseconds;
                    if (PhaseTimer <= 0)
                    {
                        Phase = 1;
                        BgOpacity = 1f;
                    }
                    else
                    {
                        BgOpacity = (3000 - PhaseTimer) / 3000f;
                    }
                    return false;
                }

                case 1:
                {
                    if (Card1Pos.Y >= ScreenCenterPosition.Y - CardHeight * 2)
                    {
                        Card1Pos.Y = ScreenCenterPosition.Y - CardHeight * 2;
                        Phase = 2;
                    }
                    else
                    {
                        Card1Pos.Y += 10f;
                    }
                    return false;
                }

                case 2:
                {
                    if (Card2Pos.Y >= ScreenCenterPosition.Y - CardHeight * 2)
                    {
                        Card2Pos.Y = ScreenCenterPosition.Y - CardHeight * 2;
                        Phase = 3;
                    }
                    else
                    {
                        Card2Pos.Y += 10f;
                    }
                    return false;
                }

                case 3:
                {
                    if (Card3Pos.Y >= ScreenCenterPosition.Y - CardHeight * 2)
                    {
                        Card3Pos.Y = ScreenCenterPosition.Y - CardHeight * 2;
                        Phase = 4;
                    }
                    else
                    {
                        Card3Pos.Y += 10f;
                    }
                    return false;
                }

                case 4:
                {
                    if (Card1SquishFactor <= 0)
                    {
                        Card1SquishFactor = 0;
                        Phase = 5;
                    }
                    else
                    {
                        Card1SquishFactor -= 0.2f;
                        Card1Pos.X += 10f;
                    }
                    return false;
                }

                case 5:
                {
                    
                    if (Card1SquishFactor >= 4)
                    {
                        Card1SquishFactor = 4;
                        Phase = 6;
                        PhaseTimer = 500;
                        TextOpacity = 0f;
                    }
                    else
                    {
                        Card1SquishFactor += 0.2f;
                        Card1Pos.X -= 10f;
                    }
                    return false;
                }

                case 6:
                {
                    PhaseTimer -= time.ElapsedGameTime.Milliseconds;
                    if (PhaseTimer <= 0)
                    {
                        Phase = 7;
                        PhaseTimer = 5000;
                        TextOpacity = 1f;
                    }
                    else
                    {
                        TextOpacity = (500 - PhaseTimer) / 500f;
                    }
                    return false;
                }

                case 7:
                {
                    PhaseTimer -= time.ElapsedGameTime.Milliseconds;
                    if (PhaseTimer <= 0)
                    {
                        Phase = 8;
                        PhaseTimer = 500;
                    }
                    return false;
                }

                case 8:
                {
                    PhaseTimer -= time.ElapsedGameTime.Milliseconds;
                    if (PhaseTimer <= 0)
                    {
                        Phase = 9;
                    }
                    else
                    {
                        TextOpacity = PhaseTimer / 500f;
                    }
                    return false;
                }

                case 9:
                {
                    if (Card2SquishFactor <= 0)
                    {
                        Card2SquishFactor = 0;
                        Phase = 10;
                    }
                    else
                    {
                        Card2SquishFactor -= 0.2f;
                        Card2Pos.X += 10f;
                    }
                    return false;
                }

                case 10:
                {
                    
                    if (Card2SquishFactor >= 4)
                    {
                        Card2SquishFactor = 4;
                        Phase = 11;
                        PhaseTimer = 0;
                        TextOpacity = 0f;
                    }
                    else
                    {
                        Card2SquishFactor += 0.2f;
                        Card2Pos.X -= 10f;
                    }
                    return false;
                }

                case 11:
                {
                    PhaseTimer -= time.ElapsedGameTime.Milliseconds;
                    if (PhaseTimer <= 0)
                    {
                        Phase = 12;
                        PhaseTimer = 5000;
                        TextOpacity = 1f;
                    }
                    else
                    {
                        TextOpacity = (500 - PhaseTimer) / 500f;
                    }
                    return false;
                }

                case 12:
                {
                    PhaseTimer -= time.ElapsedGameTime.Milliseconds;
                    if (PhaseTimer <= 0)
                    {
                        Phase = 13;
                        PhaseTimer = 500;
                    }
                    return false;
                }

                case 13:
                {
                    PhaseTimer -= time.ElapsedGameTime.Milliseconds;
                    if (PhaseTimer <= 0)
                    {
                        Phase = 14;
                    }
                    else
                    {
                        TextOpacity = PhaseTimer / 500f;
                    }
                    return false;
                }

                case 14:
                {
                    if (Card3SquishFactor <= 0)
                    {
                        Card3SquishFactor = 0;
                        Phase = 15;
                    }
                    else
                    {
                        Card3SquishFactor -= 0.2f;
                        Card3Pos.X += 10f;
                    }
                    return false;
                }

                case 15:
                {
                    
                    if (Card3SquishFactor >= 4)
                    {
                        Card3SquishFactor = 4;
                        Phase = 16;
                        PhaseTimer = 500;
                        TextOpacity = 0f;
                    }
                    else
                    {
                        Card3SquishFactor += 0.2f;
                        Card3Pos.X -= 10f;
                    }
                    return false;
                }

                case 16:
                {
                    PhaseTimer -= time.ElapsedGameTime.Milliseconds;
                    if (PhaseTimer <= 0)
                    {
                        Phase = 17;
                        PhaseTimer = 5000;
                        TextOpacity = 1f;
                    }
                    else
                    {
                        TextOpacity = (500 - PhaseTimer) / 500f;
                    }
                    return false;
                }

                case 17:
                {
                    PhaseTimer -= time.ElapsedGameTime.Milliseconds;
                    if (PhaseTimer <= 0)
                    {
                        Phase = 18;
                        PhaseTimer = 500;
                    }
                    return false;
                }

                case 18:
                {
                    PhaseTimer -= time.ElapsedGameTime.Milliseconds;
                    if (PhaseTimer <= 0)
                    {
                        Phase = 19;
                    }
                    else
                    {
                        TextOpacity = PhaseTimer / 500f;
                    }
                    return false;
                }

                default:
                    return true;
            }
        }
    }
}
