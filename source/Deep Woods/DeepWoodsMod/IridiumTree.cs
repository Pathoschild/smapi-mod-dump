using DeepWoodsMod.API.Impl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod
{
    class IridiumTree : ResourceClump
    {
        private const int TREE_TOP_TILE_INDEX = 0;
        private const int TREE_TRUNK_TILE_INDEX = 26;

        private NetFloat nextSpawnIridiumOreHealth = new NetFloat();

        public IridiumTree()
            : base()
        {
            InitNetFields();
        }

        public IridiumTree(Vector2 tile)
            : base(ResourceClump.meteoriteIndex, 2, 1, tile)
        {
            InitNetFields();
            this.health.Value = Settings.Objects.IridiumTree.Health;
            this.nextSpawnIridiumOreHealth.Value = Settings.Objects.IridiumTree.Health - Settings.Objects.IridiumTree.DamageIntervalForOreDrop;
        }

        private void InitNetFields()
        {
            this.NetFields.AddFields(this.nextSpawnIridiumOreHealth);
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Vector2 globalPosition = this.tile.Value * 64f;
            if (this.shakeTimer > 0)
            {
                globalPosition.X += (float)Math.Sin(2.0 * Math.PI / this.shakeTimer) * 4f;
            }

            Rectangle treeTopSourceRectangle = Game1.getSourceRectForStandardTileSheet(DeepWoodsTextures.Textures.IridiumTree, TREE_TOP_TILE_INDEX, 16, 16);
            treeTopSourceRectangle.Width = 16 * 6;
            treeTopSourceRectangle.Height = 16 * 4;
            Rectangle trunkSourceRectangle = Game1.getSourceRectForStandardTileSheet(DeepWoodsTextures.Textures.IridiumTree, TREE_TRUNK_TILE_INDEX, 16, 16);
            trunkSourceRectangle.Width = 16 * 2;
            trunkSourceRectangle.Height = 16 * 3;

            Vector2 treeTopPosition = globalPosition;
            treeTopPosition.X -= 2 * 64;
            treeTopPosition.Y -= 6 * 64;

            Vector2 trunkPosition = globalPosition;
            trunkPosition.Y -= 2 * 64;

            spriteBatch.Draw(DeepWoodsTextures.Textures.IridiumTree, Game1.GlobalToLocal(Game1.viewport, treeTopPosition), treeTopSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((this.tile.Y + 1f) * 64f / 10000f + this.tile.X / 100000f));
            spriteBatch.Draw(DeepWoodsTextures.Textures.IridiumTree, Game1.GlobalToLocal(Game1.viewport, trunkPosition), trunkSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((this.tile.Y + 1f) * 64f / 10000f + this.tile.X / 100000f));
        }

        public override bool performToolAction(Tool t, int damage, Vector2 tileLocation, GameLocation location)
        {
            if (t == null && damage > 0)
            {
                // explosion
                this.shakeTimer = 100f;
                return false;
            }

            if (!(t is Axe))
                return false;

            if (t.UpgradeLevel < Settings.Objects.IridiumTree.MinimumAxeLevel)
            {
                location.playSound("axe");
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13948"));
                Game1.player.jitterStrength = 1f;
                return false;
            }

            location.playSound("axchop");
            Game1.createRadialDebris(Game1.currentLocation, Debris.iridiumDebris, (int)this.tile.X + Game1.random.Next(0, 2), (int)this.tile.Y + Game1.random.Next(0, 2), Game1.random.Next(4, 9), false, -1, false, -1);
            this.health.Value -= Math.Max(1f, (t.UpgradeLevel + 1) * 0.75f);

            if (this.health.Value > 0)
            {
                if (this.health.Value <= this.nextSpawnIridiumOreHealth.Value)
                {
                    location.playSound("stumpCrack");

                    t.getLastFarmerToUse().gainExperience(Farmer.foragingSkill, 10);
                    t.getLastFarmerToUse().gainExperience(Farmer.miningSkill, 10);

                    SpawnIridiumOre(t, (int)tileLocation.X, (int)tileLocation.Y);

                    this.nextSpawnIridiumOreHealth.Value = this.health.Value - Settings.Objects.IridiumTree.DamageIntervalForOreDrop;
                }

                this.shakeTimer = 100f;
                return false;
            }

            PlayDestroyedSounds(location);

            for (int x = 0; x < this.width.Value; x++)
            {
                for (int y = 0; y < this.height.Value; y++)
                {
                    SpawnIridiumOres(t, (int)(this.tile.X + x), (int)(this.tile.Y + y));
                    Game1.createRadialDebris(Game1.currentLocation, Debris.iridiumDebris, (int)(this.tile.X + x), (int)(this.tile.Y + y), Game1.random.Next(4, 9), false, -1, false, -1);
                }
            }

            return true;
        }

        private void PlayDestroyedSounds(GameLocation location)
        {
            DelayedAction.playSoundAfterDelay("stumpCrack", 0, location);
            DelayedAction.playSoundAfterDelay("boulderBreak", 10, location);
            DelayedAction.playSoundAfterDelay("stumpCrack", 50, location);
            DelayedAction.playSoundAfterDelay("boulderBreak", 60, location);
            DelayedAction.playSoundAfterDelay("boulderBreak", 110, location);
            DelayedAction.playSoundAfterDelay("boulderBreak", 160, location);
        }

        private void SpawnIridiumOres(Tool t, int x, int y)
        {
            for (int i = 0, n = Game1.random.Next(2, 7); i < n; i++)
            {
                SpawnIridiumOre(t, x, y);
            }
        }

        private void SpawnIridiumOre(Tool t, int x, int y)
        {
            if (Game1.IsMultiplayer)
                Game1.createMultipleObjectDebris(386, x, y, 1, t.getLastFarmerToUse().UniqueMultiplayerID);
            else
                Game1.createMultipleObjectDebris(386, x, y, 1);
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            return true;
        }
    }
}
