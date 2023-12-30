/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/


using DeepWoodsMod.API.Impl;
using DeepWoodsMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod.Stuff
{
    public class DeepWoodsOrbStone : LargeTerrainFeature
    {
        public static readonly Color[] SavedColors = {
            Color.Red,
            Color.Orange,
            Color.Green,
            Color.Blue,
            Color.Purple
        };

        // only local
        private bool didHaveOrb = false;

        private NetBool hasOrb = new NetBool(false);
        private NetColor orbColor = new NetColor(Color.White);
        private NetInt orbIndex = new NetInt(-1);

        public bool HasOrb
        {
            get
            {
                return hasOrb.Value;
            }
            set
            {
                hasOrb.Value = value;
            }
        }

        public Color OrbColor
        {
            get
            {
                return orbColor.Value;
            }
            set
            {
                orbColor.Value = value;
            }
        }

        public DeepWoodsOrbStone()
           : base(false)
        {
            InitNetFields();
        }

        public DeepWoodsOrbStone(Vector2 tileLocation, int orbIndex = -1)
            : this()
        {
            this.tilePosition.Value = tileLocation;
            this.orbIndex.Value = orbIndex;
        }

        private void InitNetFields()
        {
            this.NetFields.AddFields(this.hasOrb, this.orbColor, this.orbIndex);
        }

        private bool IsMainOrbStone(GameLocation location)
        {
            if (orbIndex.Value < 0 || orbIndex.Value >= SavedColors.Length)
                return false;

            if (location is not DeepWoods deepWoods)
                return false;

            if (deepWoods.Level != 1)
                return false;

            return true;
        }

        public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
        {
            if (IsMainOrbStone(location))
            {
                if (Game1.IsMasterGame)
                {
                    if (orbIndex.Value < DeepWoodsState.OrbStonesSaved)
                    {
                        if (hasOrb.Value == false)
                        {
                            hasOrb.Value = true;
                            orbColor.Value = SavedColors[orbIndex.Value];
                        }
                    }
                }

                if (hasOrb.Value == true && didHaveOrb == false)
                {
                    if (Game1.player.currentLocation == location)
                    {
                        // audible feedback
                        Game1.playSound(Sounds.YOBA);
                    }
                    (location as DeepWoods).lightSources.Add(new LightSource(LightSource.sconceLight, tileLocation - new Vector2(0, 2), 6, new Color(1f, 0f, 0f)));
                    didHaveOrb = true;
                }
            }

            return base.tickUpdate(time, tileLocation, location);
        }

        public override bool isActionable()
        {
            return true;
        }

        public override bool isPassable(Character c = null)
        {
            return false;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 128, 64);
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            DeepWoodsQuestMenu.OpenQuestMenu(I18N.OrbStoneTouchQuestion, new Response[2]
            {
                new Response("Yes", I18N.OrbStoneTouchYes).SetHotKey(Keys.Y),
                new Response("No", I18N.OrbStoneTouchNope).SetHotKey(Keys.Escape)
            }, orbTouchResponse);
            return true;
        }

        private void orbTouchResponse(Farmer who, string whichAnswer)
        {
            if (whichAnswer == "Yes")
            {
                if (hasOrb.Value)
                {
                    DeepWoodsQuestMenu.OpenQuestMenu(I18N.OrbStoneTouchMessage, new Response[1]
                    {
                        new Response("No", I18N.MessageBoxClose).SetHotKey(Keys.Escape)
                    });
                }
                else
                {
                    DeepWoodsQuestMenu.OpenQuestMenu(I18N.OrbStoneTouchMessageNoOrb, new Response[1]
                    {
                        new Response("No", I18N.MessageBoxClose).SetHotKey(Keys.Escape)
                    });
                }
            }
        }

        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Vector2 globalPosition = tileLocation * 64f;

            Rectangle bottomSourceRectangle = new Rectangle(0, 48, 32, 16);
            Vector2 globalBottomPosition = new Vector2(globalPosition.X, globalPosition.Y);

            Rectangle topSourceRectangle;
            Vector2 globalTopPosition;
            topSourceRectangle = new Rectangle(0, 0, 32, 48);
            globalTopPosition = new Vector2(globalPosition.X, globalPosition.Y - 192);

            spriteBatch.Draw(DeepWoodsTextures.Textures.OrbStone, Game1.GlobalToLocal(Game1.viewport, globalTopPosition), topSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
            spriteBatch.Draw(DeepWoodsTextures.Textures.OrbStone, Game1.GlobalToLocal(Game1.viewport, globalBottomPosition), bottomSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));

            if (hasOrb.Value)
            {
                int timeIndex = (int)((long)Math.Round(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 300.0) % 5);
                topSourceRectangle.X += 32 * timeIndex;
                bottomSourceRectangle.X += 32 * timeIndex;
                spriteBatch.Draw(DeepWoodsTextures.Textures.OrbStoneOrb, Game1.GlobalToLocal(Game1.viewport, globalTopPosition), topSourceRectangle, this.orbColor.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f + 1));
                spriteBatch.Draw(DeepWoodsTextures.Textures.OrbStoneOrb, Game1.GlobalToLocal(Game1.viewport, globalBottomPosition), bottomSourceRectangle, this.orbColor.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f + 1));
            }
        }
    }
}
