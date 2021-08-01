/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System.IO;
using ItemResearchSpawner.Models.Enums;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components.UI
{
    internal class ItemQualitySelectorTab
    {
        private readonly Texture2D _emptyQualityTexture;

        private readonly ClickableComponent _qualityButton;
        
        public ItemQualitySelectorTab(IContentHelper content, IMonitor monitor, int x, int y)
        {
            // _emptyQualityTexture = content.Load<Texture2D>("assets/images/empty-quality-icon.png");
            _emptyQualityTexture = content.Load<Texture2D>(Path.Combine("assets", "images", "empty-quality-icon.png"));

            _qualityButton =
                new ClickableComponent(
                    new Rectangle(x, y, 36 + UIConstants.BorderWidth, 36 + UIConstants.BorderWidth - 2), "");
        }

        public Rectangle Bounds => _qualityButton.bounds;

        public void HandleLeftClick()
        {
            ModManager.Instance.Quality = ModManager.Instance.Quality.GetNext();
        }

        public void HandleRightClick()
        {
            ModManager.Instance.Quality = ModManager.Instance.Quality.GetPrevious();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            GetCurrentQualityIcon(out var texture, out var sourceRect, out var color);

            RenderHelpers.DrawMenuBox(_qualityButton.bounds.X, _qualityButton.bounds.Y,
                _qualityButton.bounds.Width - UIConstants.BorderWidth,
                _qualityButton.bounds.Height - UIConstants.BorderWidth, out var qualityIconPos);

            spriteBatch.Draw(texture, new Vector2(qualityIconPos.X, qualityIconPos.Y), sourceRect, color, 0,
                Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
        }

        private void GetCurrentQualityIcon(out Texture2D texture, out Rectangle sourceRect, out Color color)
        {
            texture = Game1.mouseCursors;
            color = Color.White;

            switch (ModManager.Instance.Quality)
            {
                case ItemQuality.Normal:
                    texture = _emptyQualityTexture;
                    sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
                    color *= 0.65f;
                    break;

                case ItemQuality.Silver:
                    sourceRect = CursorSprites.SilverStarQuality;
                    break;

                case ItemQuality.Gold:
                    sourceRect = CursorSprites.GoldStarQuality;
                    break;

                default:
                    sourceRect = CursorSprites.IridiumStarQuality;
                    break;
            }
        }
    }
}