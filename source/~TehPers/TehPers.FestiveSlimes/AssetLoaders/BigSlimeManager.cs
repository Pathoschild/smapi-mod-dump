using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.FestiveSlimes.AssetLoaders {
    internal class BigSlimeManager : SlimeManager {
        private readonly Texture2D _winterHat;

        public BigSlimeManager(ICoreApi coreApi) : base(coreApi, "Big Slime") {
            this._winterHat = coreApi.ContentSource.Load<Texture2D>($@"assets\{this.SlimeName}\hats\winter.png");
        }

        protected override bool GetCurrentTexture(SDateTime date, out Texture2D texture) {
            switch (date.Season) {
                case Season.Fall: // Pumpkin slimes
                    texture = this.Mod.Helper.Content.Load<Texture2D>($@"assets\{this.SlimeName}\textures\fall.png");
                    return true;
                default:
                    texture = default;
                    return false;
            }
        }

        protected override void OverrideDraw(IDrawingInfo info) {
            if (SDateTime.Today.Season == Season.Fall) {
                // Remove tint
                info.SetTint(Color.White);
            }
        }

        public override void DrawHat(SDateTime date, SpriteBatch batch, Monster slime) {
            if (date.Season == Season.Winter) {
                batch.Draw(this._winterHat, slime.getLocalPosition(Game1.viewport) + new Vector2(56f, 16 + slime.yJumpOffset), slime.Sprite.SourceRect, Color.White, 0.0f, new Vector2(16f, 16f), 4f * Math.Max(0.2f, slime.Scale), slime.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, slime.drawOnTop ? 0.991f : (float) (slime.getStandingY() / 10000.0 + 2.0 / 1000.0)));
            }
        }

        public override IEnumerable<Item> GetExtraDrops(SDateTime date, Monster monster) {
            // Check season
            if (date.Season == Season.Fall) {
                // Try to add a candy (50% chance)
                if (Game1.random.NextDouble() < 0.5 && ModFestiveSlimes.CoreApi.Items.TryCreate("candy", out Item candy)) {
                    yield return candy;
                }

                // Try to add a candy (25% chance)
                if (Game1.random.NextDouble() < 0.25 && ModFestiveSlimes.CoreApi.Items.TryCreate("candy", out candy)) {
                    yield return candy;
                }
            }
        }
    }
}