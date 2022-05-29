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
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Extensions;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Extensions;
using TehPers.FishingOverhaul.Extensions.Drawing;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal sealed class FishingHudRenderer : ISetup, IDisposable
    {
        private readonly IModHelper helper;
        private readonly IFishingApi fishingApi;
        private readonly HudConfig hudConfig;
        private readonly INamespaceRegistry namespaceRegistry;

        private readonly Texture2D whitePixel;

        public FishingHudRenderer(
            IModHelper helper,
            IFishingApi fishingApi,
            HudConfig hudConfig,
            INamespaceRegistry namespaceRegistry
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));
            this.hudConfig = hudConfig ?? throw new ArgumentNullException(nameof(hudConfig));
            this.namespaceRegistry = namespaceRegistry;
            this.whitePixel = new(Game1.graphics.GraphicsDevice, 1, 1);
            this.whitePixel.SetData(new[] {Color.White});
        }

        public void Setup()
        {
            this.helper.Events.Display.RenderedHud += this.RenderFishingHud;
        }

        public void Dispose()
        {
            this.helper.Events.Display.RenderedHud -= this.RenderFishingHud;
        }

        private void RenderFishingHud(object? sender, RenderedHudEventArgs e)
        {
            // Check if HUD should be rendered
            var farmer = Game1.player;
            if (!this.hudConfig.ShowFishingHud
                || Game1.eventUp
                || farmer.CurrentTool is not FishingRod)
            {
                return;
            }

            // Draw the fishing GUI to the screen
            var normalTextColor = Color.White;
            var fishTextColor = Color.White;
            var trashTextColor = Color.Gray;
            var font = Game1.smallFont;
            var boxWidth = 0f;
            var lineHeight = (float)font.LineSpacing;
            var boxTopLeft = new Vector2(this.hudConfig.TopLeftX, this.hudConfig.TopLeftY);
            var boxBottomLeft = boxTopLeft;
            var fishingInfo = this.fishingApi.CreateDefaultFishingInfo(farmer);
            var fishChances = this.fishingApi.GetFishChances(fishingInfo)
                .ToWeighted(value => value.Weight, value => value.Value.FishKey)
                .Condense()
                .OrderByDescending(x => x.Weight)
                .ToList();
            var trashChances = this.fishingApi.GetTrashChances(fishingInfo)
                .ToWeighted(value => value.Weight, value => value.Value.ItemKey)
                .Condense()
                .OrderByDescending(x => x.Weight)
                .ToList();

            // Setup the sprite batch
            e.SpriteBatch.End();
            e.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Draw streak chances
            var streakText = this.helper.Translation.Get(
                "text.streak",
                new {streak = this.fishingApi.GetStreak(farmer)}
            );
            e.SpriteBatch.DrawStringWithShadow(
                font,
                streakText,
                boxBottomLeft,
                normalTextColor,
                1F
            );
            boxWidth = Math.Max(boxWidth, font.MeasureString(streakText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Draw treasure chances
            var treasureChance = this.fishingApi.GetChanceForTreasure(fishingInfo);
            var treasureText = this.helper.Translation.Get(
                "text.treasure",
                new {chance = $"{treasureChance:P2}"}
            );
            e.SpriteBatch.DrawStringWithShadow(
                font,
                treasureText,
                boxBottomLeft,
                normalTextColor,
                1F
            );
            boxWidth = Math.Max(boxWidth, font.MeasureString(treasureText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Draw trash chances
            var chanceForFish = this.fishingApi.GetChanceForFish(fishingInfo);
            var trashChance = fishChances.Count == 0 ? 1.0 : 1.0 - chanceForFish;
            var trashText = this.helper.Translation.Get(
                "text.trash",
                new {chance = $"{trashChance:P2}"}
            );
            e.SpriteBatch.DrawStringWithShadow(font, trashText, boxBottomLeft, normalTextColor, 1F);
            boxWidth = Math.Max(boxWidth, font.MeasureString(trashText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Draw entries
            var maxDisplayedFish = this.hudConfig.MaxFishTypes;
            var displayedEntries = fishChances.ToWeighted(
                x => x.Weight,
                x => (entry: x.Value, textColor: fishTextColor)
            );
            if (this.hudConfig.ShowTrash)
            {
                displayedEntries = displayedEntries.Normalize(chanceForFish)
                    .Concat(
                        trashChances.ToWeighted(
                                x => x.Weight,
                                x => (entry: x.Value, textColor: trashTextColor)
                            )
                            .Normalize(1 - chanceForFish)
                    );
            }

            displayedEntries = displayedEntries.Normalize()
                .Where(x => x.Weight > 0d)
                .OrderByDescending(x => x.Weight);

            foreach (var displayedEntry in displayedEntries.Take(maxDisplayedFish))
            {
                var (entryKey, textColor) = displayedEntry.Value;
                var chance = displayedEntry.Weight;

                // Draw fish icon
                var lineWidth = 0f;
                var fishName = this.helper.Translation.Get(
                        "text.fish.unknownName",
                        new {key = entryKey.ToString()}
                    )
                    .ToString();
                if (this.namespaceRegistry.TryGetItemFactory(entryKey, out var factory))
                {
                    var fishItem = factory.Create();
                    fishName = fishItem.DisplayName;

                    const float iconScale = 0.5f;
                    const float iconSize = 64f * iconScale;
                    fishItem.DrawInMenuCorrected(
                        e.SpriteBatch,
                        boxBottomLeft,
                        iconScale,
                        1F,
                        0.9F,
                        StackDrawType.Hide,
                        Color.White,
                        false,
                        new TopLeftDrawOrigin()
                    );

                    lineWidth += iconSize;
                    lineHeight = Math.Max(lineHeight, iconSize);
                }

                // Draw chance
                var fishText = this.helper.Translation.Get(
                    "text.fish",
                    new
                    {
                        name = fishName,
                        chance = $"{chance * 100.0:F2}"
                    }
                );
                e.SpriteBatch.DrawStringWithShadow(
                    font,
                    fishText,
                    boxBottomLeft + new Vector2(lineWidth, 0),
                    textColor,
                    1F
                );
                var (textWidth, textHeight) = font.MeasureString(fishText);
                lineWidth += textWidth;
                lineHeight = Math.Max(lineHeight, textHeight);

                // Update background box
                boxWidth = Math.Max(boxWidth, lineWidth);
                boxBottomLeft += new Vector2(0, lineHeight);
            }

            // Draw 'more fish' text
            if (fishChances.Count > maxDisplayedFish)
            {
                var moreFishText = this.helper.Translation.Get(
                        "text.fish.more",
                        new {quantity = fishChances.Count - maxDisplayedFish}
                    )
                    .ToString();
                e.SpriteBatch.DrawStringWithShadow(
                    font,
                    moreFishText,
                    boxBottomLeft,
                    normalTextColor,
                    1F
                );
                boxWidth = Math.Max(boxWidth, font.MeasureString(moreFishText).X);
                boxBottomLeft += new Vector2(0, lineHeight);
            }

            // Draw the background rectangle
            // TODO: use a nicer background
            e.SpriteBatch.Draw(
                this.whitePixel,
                new((int)boxTopLeft.X, (int)boxTopLeft.Y, (int)boxWidth, (int)boxBottomLeft.Y),
                null,
                new(0, 0, 0, 0.25F),
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.85F
            );

            // Reset sprite batch
            e.SpriteBatch.End();
            e.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise
            );
        }
    }
}
