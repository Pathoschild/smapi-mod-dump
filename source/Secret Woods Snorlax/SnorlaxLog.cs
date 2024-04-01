/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/SecretWoodsSnorlax
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Tools;
using System;

namespace ichortower.SecretWoodsSnorlax
{
    internal class SnorlaxLog : StardewValley.TerrainFeatures.ResourceClump
    {
        public static Texture2D SpriteSheet = null!;

        public float yJumpOffset = 0f;
        public float yJumpVelocity = 0f;
        public float yJumpGravity = -0.5f;
        public float xJumpMove = 0f;
        public float yJumpMove = 0f;
        public int jumpTicks = -1;

        public NetEvent1Field<int, NetInt> mpJumpEvent =
                new NetEvent1Field<int, NetInt>();

        public SnorlaxLog(float x, float y)
            : base()
        {
            this.width.Value = 3;
            this.height.Value = 3;
            this.parentSheetIndex.Value = 0;
            this.Tile = new Vector2(x, y);
            this.health.Value = 160; // snorlax's base HP stat
            if (SnorlaxLog.SpriteSheet is null) {
                SnorlaxLog.SpriteSheet = ModEntry.HELPER.ModContent
                        .Load<Texture2D>("assets/map_snorlax.png");
            }
            base.NetFields.AddField(this.mpJumpEvent);
            this.mpJumpEvent.onEvent += performJump;
        }

        public SnorlaxLog(Vector2 pos)
            : this(pos.X, pos.Y)
        {
        }

        public SnorlaxLog()
            : this(Constants.vec_BlockingPosition)
        {
        }

        public void performJump(int which)
        {
            if (which == 1) {
                this.parentSheetIndex.Value = 2;
                this.yJumpVelocity = 16f;
                /*
                 * the move amounts are so simple because at velocity 16f, the
                 * jump takes exactly 64 frames to complete (quadratic formula).
                 * tile dist * 64 pixels / 64 frames
                 */
                this.xJumpMove = Constants.vec_MovedPosition.X -
                        Constants.vec_BlockingPosition.X;
                this.yJumpMove = Constants.vec_MovedPosition.Y -
                        Constants.vec_BlockingPosition.Y;
                this.jumpTicks = 0;
            }
            else if (which == 2) {
                this.parentSheetIndex.Value = 2;
                this.yJumpVelocity = 8f;
                this.xJumpMove = 0f;
                this.yJumpMove = 0f;
                this.jumpTicks = 0;
            }
        }

        public void broadcastJump(int which)
        {
            this.mpJumpEvent.Fire(which);
            this.mpJumpEvent.Poll();
        }

        public void JumpAside()
        {
            broadcastJump(1);
        }

        public void JumpInPlace()
        {
            broadcastJump(2);
        }

        public bool HasMoved()
        {
            if (Game1.player.mailReceived.Contains(Constants.mail_Moved)) {
                return true;
            }
            return this.Tile == Constants.vec_MovedPosition;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(
                    SnorlaxLog.SpriteSheet, this.parentSheetIndex.Value,
                    this.width.Value * 16, this.height.Value * 16);
            Vector2 position = this.Tile * 64f;
            position.Y -= yJumpOffset;
            if (jumpTicks > 0) {
                position.X += xJumpMove * jumpTicks;
                position.Y += yJumpMove * jumpTicks;
            }
            spriteBatch.Draw(SnorlaxLog.SpriteSheet,
                    Game1.GlobalToLocal(Game1.viewport, position),
                    sourceRect, Color.White, 0f, Vector2.Zero, 4f,
                    SpriteEffects.None,
                    (this.Tile.Y + 1f) * 64f / 10000f + this.Tile.X / 100000f);
        }

        public override bool tickUpdate(GameTime time)
        {
            mpJumpEvent.Poll();
            if (jumpTicks >= 0) {
                ++jumpTicks;
            }
            float prevOffset = yJumpOffset;
            if (yJumpVelocity != 0f) {
                yJumpOffset = Math.Max(0f, yJumpOffset + yJumpVelocity);
            }
            if (yJumpOffset > 0f) {
                yJumpVelocity += yJumpGravity;
            }
            if (prevOffset > 0f && yJumpOffset == 0f) {
                this.parentSheetIndex.Value = 0;
                this.jumpTicks = -1;
                this.Tile = Constants.vec_MovedPosition;
                GameLocation location = this.Location;
                location.playSound("clubSmash", this.Tile);
                location.playSound("treethud", this.Tile);
            }
            return base.tickUpdate(time);
        }

        public override bool performUseAction(Vector2 tileLocation)
        {
            if (!Game1.didPlayerJustRightClick(true)) {
                Game1.haltAfterCheck = false;
                return false;
            }
            string key = HasMoved() ? "inspect.moved" : "inspect.unmoved";
            string text = ModEntry.HELPER.Translation.Get(key);
            Game1.drawObjectDialogue(Game1.parseText(text));
            enableHints();
            return true;
        }

        public override bool performToolAction(Tool t, int damage,
                Vector2 tileLocation)
        {
            if (t is null) {
                return false;
            }
            GameLocation location = this.Location;
            if (!HasMoved()) {
                string key = $"tool.noEffect.{Game1.random.Next(0,4)}";
                string str = ModEntry.HELPER.Translation.Get(key);
                if (t is Axe) {
                    location.playSound("woodyHit", this.Tile);
                    Game1.player.jitterStrength = 1f;
                    Game1.drawObjectDialogue(str);
                    enableHints();
                }
                else if (t is Pickaxe) {
                    location.playSound("woodyHit", this.Tile);
                    Game1.player.jitterStrength = 1f;
                    Game1.drawObjectDialogue(str);
                    enableHints();
                }
            }
            return false;
        }

        private void enableHints()
        {
            // no letter, send to everyone. all players will get their own
            // CTs, so they can work together getting hints.
            // (only the main player can get the flute event)
            Game1.addMailForTomorrow($"{Constants.mail_Hints}Active", true, true);
        }
    }

}
