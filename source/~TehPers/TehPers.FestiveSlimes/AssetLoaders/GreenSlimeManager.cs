/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Structs;
using Object = System.Object;
using SObject = StardewValley.Object;

namespace TehPers.FestiveSlimes.AssetLoaders {
    internal class GreenSlimeManager : SlimeManager {
        private readonly Texture2D _winterHat;

        public GreenSlimeManager(ICoreApi coreApi) : base(coreApi, "Green Slime") {
            this._winterHat = coreApi.ContentSource.Load<Texture2D>($@"assets\{this.SlimeName}\hats\winter.png");
        }

        protected override bool GetCurrentTexture(SDateTime date, out Texture2D texture) {
            switch (date.Season) {
                case Season.Fall: // Pumpkin slimes
                    texture = this.Mod.Helper.Content.Load<Texture2D>($@"assets\{this.SlimeName}\textures\fall.png");
                    return true;
                case Season.Winter when date.DayOfSeason == 11: // Pufferchick slimes
                    texture = this.Mod.Helper.Content.Load<Texture2D>($@"assets\{this.SlimeName}\textures\pufferchick.png");
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
            } else if (SDateTime.Today.Season == Season.Winter && SDateTime.Today.DayOfSeason == 11) {
                // Remove tint
                info.SetTint(Color.White);
            }
        }

        public override void DrawHat(SDateTime date, SpriteBatch batch, Monster slime) {
            if (!(slime is GreenSlime greenSlime)) {
                return;
            }

            if (date.Season == Season.Winter && date.DayOfSeason != 11) {
                batch.Draw(this._winterHat, greenSlime.getLocalPosition(Game1.viewport) + new Vector2(32f, greenSlime.GetBoundingBox().Height / 2f + greenSlime.yOffset), greenSlime.Sprite.SourceRect, Color.White, 0.0f, new Vector2(8f, 16f), 4f * Math.Max(0.2f, greenSlime.Scale - (float) (0.400000005960464 * (greenSlime.ageUntilFullGrown.Value / 120000.0))), SpriteEffects.None, Math.Max(0.0f, greenSlime.drawOnTop ? 0.991f : greenSlime.getStandingY() / 10000f + 5f / 10000f));
            }
        }

        public override IEnumerable<Item> GetExtraDrops(SDateTime date, Monster monster) {
            switch (date.Season) {
                case Season.Fall when Game1.random.NextDouble() < 0.25 && ModFestiveSlimes.CoreApi.Items.TryCreate("candy", out Item candy):
                    return candy.Yield();
                case Season.Winter when date.DayOfSeason == 11 && Game1.random.NextDouble() < 0.05:
                    return new SObject(Vector2.Zero, Objects.VoidEssence, 1).Yield();
            }

            return Enumerable.Empty<Item>();
        }
    }
}