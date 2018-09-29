using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using SFarmer = StardewValley.Farmer;
using System;
using TehPers.Stardew.CombatOverhaul.Natures;

namespace TehPers.Stardew.CombatOverhaul {
    public class JunimoRod : Tool {
        private Nature _activeNature = null;
        public Nature ActiveNature {
            get {
                return _activeNature;
            }
            set {
                _activeNature = value;
                _charges = 10;
                updateVisuals();
            }
        }

        private int _charges = 0;
        public int Charges {
            get {
                return _charges;
            }
            set {
                _charges = value;
                if (_charges <= 0)
                    _activeNature = null;
                updateVisuals();
            }
        }

        public JunimoRod() : base("Junimo Rod", 0, 2, 2, false, 0) {
            this.ActiveNature = null;
            this.upgradeLevel = 0;
            this.CurrentParentTileIndex = this.indexOfMenuItemView;
            this.instantUse = true;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, SFarmer who) {
            who.completelyStopAnimatingOrDoingAction();
            this.indexOfMenuItemView = 2;
            this.CurrentParentTileIndex = 2;
            if (who.IsMainPlayer) executeMagic(location, x, y, power, who);
            this.CurrentParentTileIndex = this.indexOfMenuItemView;
        }

        public override bool actionWhenPurchased() {
            // Todo: Probably some event that introduces the rod
            return base.actionWhenPurchased();
        }

        public void executeMagic(GameLocation location, int x, int y, int power, SFarmer who) {
            if (Charges > 0 && ActiveNature != null && ActiveNature.activate(location, x, y, power, who)) {
                if (ActiveNature.playWandSound()) Game1.playSound("wand");
                Charges--;
            } else {
                Game1.playSound("wand");
                Game1.showGlobalMessage("The rod shimmers, but nothing seems to happen.");
            }
        }

        private void wandWarpForReal() {
            Game1.warpFarmer("Farm", 64, 15, false);
            if (!Game1.isStartingToGetDarkOut())
                Game1.playMorningSong();
            else
                Game1.changeMusicTrack("none");
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.player.temporarilyInvincible = false;
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
        }

        public void updateVisuals() {
            if (ActiveNature == null) {
                this.name = "Junimo Rod";
                this.description = "Mysterious natural magic flows through this wand.";
            } else {
                this.name = string.Format("Junimo Rod - {0} ({1})", ActiveNature.getName(), Charges);
                this.description = ActiveNature.getDescription();
            }
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber) {
            if (ActiveNature == null) {
                Rectangle rect = Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, Game1.tileSize / 4, Game1.tileSize / 4, this.indexOfMenuItemView);
                spriteBatch.Draw(Game1.toolSpriteSheet, location + new Vector2(Game1.tileSize / 2, Game1.tileSize / 2), new Rectangle?(rect), Color.White * transparency, 0.0f, new Vector2(Game1.tileSize / 4 / 2, Game1.tileSize / 4 / 2), Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
            } else {
                float correctionScale = (Game1.tileSize / 4f) / ActiveNature.ScepterTexture.Bounds.Width;
                Vector2 origin = new Vector2(ActiveNature.ScepterTexture.Width / 2f, ActiveNature.ScepterTexture.Height / 2f);
                Vector2 loc = location + new Vector2(Game1.tileSize / 2, Game1.tileSize / 2);
                spriteBatch.Draw(ActiveNature.ScepterTexture, loc, null, Color.White * transparency, 0f, origin, Game1.pixelZoom * scaleSize * correctionScale, SpriteEffects.None, layerDepth);
            }
        }

        protected override string loadDisplayName() {
            throw new NotImplementedException();
        }

        protected override string loadDescription() {
            throw new NotImplementedException();
        }
    }
}
