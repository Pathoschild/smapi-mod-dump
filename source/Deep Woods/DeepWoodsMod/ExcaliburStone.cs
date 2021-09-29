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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepWoodsMod
{
    public class ExcaliburStone : LargeTerrainFeature
    {
        private NetBool swordPulledOut = new NetBool(false);

        public ExcaliburStone()
           : base(false)
        {
            InitNetFields();
        }

        public ExcaliburStone(Vector2 tileLocation)
            : this()
        {
            this.tilePosition.Value = tileLocation;
            InitNetFields();
        }

        private void InitNetFields()
        {
            this.NetFields.AddFields(this.swordPulledOut);
        }

        public override bool isActionable()
        {
            return !swordPulledOut;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 128, 64);
        }

        public override bool isPassable(Character c = null)
        {
            return false;
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            if (this.swordPulledOut.Value)
                return true;

            if (Game1.player.DailyLuck >= 0.25
                && Game1.player.LuckLevel >= DeepWoodsGlobals.MAXIMUM_POSSIBLE_LUCKLEVEL
                && Game1.player.MiningLevel >= 10
                && Game1.player.ForagingLevel >= 10
                && Game1.player.FishingLevel >= 10
                && Game1.player.FarmingLevel >= 10
                && Game1.player.CombatLevel >= 10
                && (Game1.player.timesReachedMineBottom >= 1 || Game1.MasterPlayer.timesReachedMineBottom >= 1)
                && Game1.getFarm().grandpaScore.Value >= 4
                && (!Game1.player.mailReceived.Contains("JojaMember") && !Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
                && (Game1.player.hasCompletedCommunityCenter() || Game1.MasterPlayer.hasCompletedCommunityCenter()))
            {
                location.playSoundAt(Sounds.YOBA, this.tilePosition.Value);
                Game1.player.addItemByMenuIfNecessaryElseHoldUp(Excalibur.GetOne());
                this.swordPulledOut.Value = true;
            }
            else
            {
                location.playSoundAt(Sounds.THUD_STEP, this.tilePosition.Value);
                Game1.showRedMessage("It won't budge.");

                ModEntry.Log($"Excalibur doesn't budge. " +
                    $" DailyLuck is {Game1.player.DailyLuck}, must be >= 0.25. " +
                    $" LuckLevel is {Game1.player.LuckLevel}, must be >= {DeepWoodsGlobals.MAXIMUM_POSSIBLE_LUCKLEVEL}. " +
                    $" MiningLevel is {Game1.player.MiningLevel}, must be >= 10. " +
                    $" ForagingLevel is {Game1.player.ForagingLevel}, must be >= 10. " +
                    $" FishingLevel is {Game1.player.FishingLevel}, must be >= 10. " +
                    $" FarmingLevel is {Game1.player.FarmingLevel}, must be >= 10. " +
                    $" CombatLevel is {Game1.player.CombatLevel}, must be >= 10. " +
                    $" timesReachedMineBottom is {Math.Max(Game1.player.timesReachedMineBottom, Game1.MasterPlayer.timesReachedMineBottom)}, must be >= 1. " +
                    $" grandpaScore is {Game1.getFarm().grandpaScore.Value}, must be >= 4. " +
                    $" JojaMember is {Game1.player.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.mailReceived.Contains("JojaMember")}, must be false. " +
                    $" hasCompletedCommunityCenter is {(Game1.player.hasCompletedCommunityCenter() || Game1.MasterPlayer.hasCompletedCommunityCenter())}, must be true." +
                    $"", StardewModdingAPI.LogLevel.Info);
            }

            return true;
        }

        public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
        {
        }

        public override bool seasonUpdate(bool onLoad)
        {
            return false;
        }

        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override void performPlayerEntryAction(Vector2 tileLocation)
        {
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Vector2 globalPosition = tileLocation * 64f;

            Rectangle bottomSourceRectangle = new Rectangle(0, 16, 32, 16);
            Vector2 globalBottomPosition = new Vector2(globalPosition.X, globalPosition.Y);

            Rectangle topSourceRectangle;
            Vector2 globalTopPosition;
            if (this.swordPulledOut.Value)
            {
                topSourceRectangle = new Rectangle(0, 0, 32, 16);
                globalTopPosition = new Vector2(globalPosition.X, globalPosition.Y - 64);
            }
            else
            {
                topSourceRectangle = new Rectangle(32, 0, 32, 32);
                globalTopPosition = new Vector2(globalPosition.X, globalPosition.Y - 128);
            }

            spriteBatch.Draw(DeepWoodsTextures.Textures.ExcaliburStone, Game1.GlobalToLocal(Game1.viewport, globalTopPosition), topSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
            spriteBatch.Draw(DeepWoodsTextures.Textures.ExcaliburStone, Game1.GlobalToLocal(Game1.viewport, globalBottomPosition), bottomSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
        }
    }
}
