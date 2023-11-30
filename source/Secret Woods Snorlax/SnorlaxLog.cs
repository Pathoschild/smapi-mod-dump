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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;

namespace ichortower.SecretWoodsSnorlax
{
    internal class SnorlaxLog : StardewValley.TerrainFeatures.ResourceClump
    {
        public static string SpriteSheetName = "Maps\\SecretWoodsSnorlax";
        public static Texture2D SpriteSheet = null!;

        public float yJumpOffset = 0f;
        public float yJumpVelocity = 0f;
        public float yJumpGravity = -0.5f;
        public int jumpTicks = -1;
        public float xJumpMove = 0f;
        public float yJumpMove = 0f;

        public NetEvent1Field<int, NetInt> mpJumpEvent =
                new NetEvent1Field<int, NetInt>();

        public SnorlaxLog(float x, float y)
            : base()
        {
            this.width.Value = 3;
            this.height.Value = 3;
            this.parentSheetIndex.Value = 0;
            this.tile.Value = new Vector2(x, y);
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
                this.jumpTicks = 0;
                this.xJumpMove = Constants.vec_MovedPosition.X -
                        Constants.vec_BlockingPosition.X;
                this.yJumpMove = Constants.vec_MovedPosition.Y -
                        Constants.vec_BlockingPosition.Y;
            }
            else if (which == 2) {
                this.parentSheetIndex.Value = 2;
                this.yJumpVelocity = 8f;
                this.jumpTicks = 0;
                this.xJumpMove = 0f;
                this.yJumpMove = 0f;
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
            if (Game1.player.mailReceived.Contains(Constants.mail_SnorlaxMoved)) {
                return true;
            }
            return this.tile.Value == Constants.vec_MovedPosition;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(
                    SnorlaxLog.SpriteSheet, this.parentSheetIndex.Value,
                    this.width.Value * 16, this.height.Value * 16);
            Vector2 position = this.tile.Value * 64f;
            position.Y -= yJumpOffset;
            if (jumpTicks > 0) {
                position.X += xJumpMove * jumpTicks;
                position.Y += yJumpMove * jumpTicks;
            }
            spriteBatch.Draw(SnorlaxLog.SpriteSheet,
                    Game1.GlobalToLocal(Game1.viewport, position),
                    sourceRect, Color.White, 0f, Vector2.Zero, 4f,
                    SpriteEffects.None,
                    (this.tile.Y + 1f) * 64f / 10000f + this.tile.X / 100000f);
        }

        public override bool tickUpdate(GameTime time,
                Vector2 tileLocation, GameLocation location)
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
                this.tile.Value = Constants.vec_MovedPosition;
                location.playSoundAt("clubSmash", this.tile.Value);
                location.playSoundAt("treethud", this.tile.Value);
            }
            return base.tickUpdate(time, tileLocation, location);
        }

        public override bool performUseAction(Vector2 tileLocation,
                GameLocation location)
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
                Vector2 tileLocation, GameLocation location)
        {
            if (t is null) {
                return false;
            }
            if (!HasMoved()) {
                string key = $"tool.noEffect.{Game1.random.Next(0,4)}";
                string str = ModEntry.HELPER.Translation.Get(key);
                if (t is Axe) {
                    location.playSound("woodyHit");
                    Game1.player.jitterStrength = 1f;
                    Game1.drawObjectDialogue(str);
                    enableHints();
                }
                else if (t is Pickaxe) {
                    location.playSound("woodyHit");
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
            Game1.addMailForTomorrow(Constants.mail_SnorlaxHints, true, true);
        }
    }

}
