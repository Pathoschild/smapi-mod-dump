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
using StardewValley.TerrainFeatures;

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

            if (Game1.player.DailyLuck >= DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MinimumDailyLuck
                && Game1.player.LuckLevel >= DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MinimumLuckLevel
                && Game1.player.MiningLevel >= DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MinimumMiningLevel
                && Game1.player.ForagingLevel >= DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MinimumForagingLevel
                && Game1.player.FishingLevel >= DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MinimumFishingLevel
                && Game1.player.FarmingLevel >= DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MinimumFarmingLevel
                && Game1.player.CombatLevel >= DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MinimumCombatLevel
                && (Game1.player.timesReachedMineBottom >= 1 || Game1.MasterPlayer.timesReachedMineBottom >= 1 || !DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MustHaveReachedMineBottom)
                && Game1.getFarm().grandpaScore.Value >= DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MinimumGrandpaScore
                && (!Game1.player.mailReceived.Contains("JojaMember") && !Game1.MasterPlayer.mailReceived.Contains("JojaMember") || !DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MustNotBeJojaMember)
                && (Game1.player.hasCompletedCommunityCenter() || Game1.MasterPlayer.hasCompletedCommunityCenter() || !DeepWoodsSettings.Settings.Objects.Excalibur.Worthiness.MustHaveCompletedCommunityCenter))
            {
                location.playSoundAt(Sounds.YOBA, this.tilePosition.Value);
                Game1.player.addItemByMenuIfNecessaryElseHoldUp(Excalibur.GetOne());
                this.swordPulledOut.Value = true;
            }
            else
            {
                location.playSoundAt(Sounds.THUD_STEP, this.tilePosition.Value);
                Game1.showRedMessage(I18N.ExcaliburNopeMessage);
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
