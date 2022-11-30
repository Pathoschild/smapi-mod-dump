/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Linq;

namespace StardewRoguelike.TerrainFeatures
{
    internal class SpeedPad : TerrainFeature
    {
        private string textureName = "TerrainFeatures\\SpeedPad";

        private Lazy<Texture2D> texture = null!;

        private Rectangle SourceRect = new(0, 0, 32, 16);

        private static readonly int FrameDuration = 200;

        private NetBool IsSlowPad { get; } = new();

        private int currentFrame = 0;

        private double frameCounter = 0;

        public SpeedPad() : base(true)
        {
            ResetTexture();
            NetFields.AddField(IsSlowPad);
        }

        public SpeedPad(bool isSlowPad) : this()
        {
            IsSlowPad.Value = isSlowPad;
        }

        protected void ResetTexture()
        {
            texture = new(new Func<Texture2D>(LoadTexture));
        }

        protected Texture2D LoadTexture()
        {
            return Game1.content.Load<Texture2D>(textureName);
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64 * 2, 64);
        }

        public override void doCollisionAction(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who, GameLocation location)
        {
            if (who != Game1.player)
                return;

            int buffId = IsSlowPad.Value ? 88998 : 88997;
            int speedAdjustment = IsSlowPad.Value ? -3 : 3;
            string soundEffect = IsSlowPad.Value ? "debuffHit" : "cowboy_powerup";
            int duration = IsSlowPad.Value ? 750 : 1500;

            Buff? buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
            if (buff is null)
            {
                Game1.buffsDisplay.addOtherBuff(
                    buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: speedAdjustment, 0, 0, minutesDuration: 1, source: "Roguelike", displaySource: "Speedpad") { which = buffId }
                );
                Game1.playSound(soundEffect);
            }
            buff.millisecondsDuration = duration;
        }

        public override bool isPassable(Character? c = null)
        {
            return true;
        }

        public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
        {
            if (IsSlowPad.Value && SourceRect.X != 32)
                SourceRect.X = 32;

            if (frameCounter >= FrameDuration)
            {
                currentFrame++;
                currentFrame %= 3;
                frameCounter = 0;

                SourceRect.Y = currentFrame * 16;
            }
            else
                frameCounter += time.ElapsedGameTime.TotalMilliseconds;

            return false;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            spriteBatch.Draw(texture.Value, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f), SourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 64f / 10000f);
        }
    }
}
