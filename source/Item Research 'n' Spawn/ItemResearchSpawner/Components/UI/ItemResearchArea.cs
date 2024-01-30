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
    internal class ItemResearchArea
    {
        private readonly ClickableComponent _researchArea;
        private readonly ClickableTextureComponent _researchButton;
        
        private readonly Texture2D _researchTexture;
        private readonly Texture2D _sellTexture;
        private readonly Texture2D _combinedTexture;

        private Item _researchItem;

        private Item _lastItem;
        private string _itemProgression;

        public ItemResearchArea(IContentHelper content, IMonitor monitor, int x, int y)
        {
            // _researchTexture = content.Load<Texture2D>("assets/images/search-button.png");
            // _sellTexture = content.Load<Texture2D>("assets/images/sell-button.png");
            
            _researchTexture = content.Load<Texture2D>(Path.Combine("assets", "images", "search-button"));
            _sellTexture = content.Load<Texture2D>(Path.Combine("assets", "images", "sell-button.png"));
            _combinedTexture = content.Load<Texture2D>(Path.Combine("assets", "images", "combined-button.png"));

            _researchArea = new ClickableComponent(new Rectangle(x, y, Game1.tileSize + 60, Game1.tileSize + 50), "");

            _researchButton = new ClickableTextureComponent(
                new Rectangle(
                    RenderHelpers.GetChildCenterPosition(x, _researchArea.bounds.Width + 2 * UIConstants.BorderWidth,
                        _researchTexture.Width),
                    _researchArea.bounds.Height + 38 + y, _researchTexture.Width,
                    _researchTexture.Height), _researchTexture,
                new Rectangle(0, 0, _researchTexture.Width, _researchTexture.Height), 1f);

            ProgressionManager.OnStackChanged += OnStackChanged;
        }

        public Rectangle Bounds => _researchArea.bounds;

        public Rectangle ButtonBounds => _researchButton.bounds;

        public Item ResearchItem => _researchItem;

        public bool TrySetItem(Item item)
        {
            if (_researchItem != null) return false;

            _researchItem = item;

            return true;
        }

        public Item ReturnItem()
        {
            var item = _researchItem;
            _researchItem = null;
            _lastItem = null;

            return item;
        }

        public void HandleResearch()
        {
            if (_researchItem != null)
            {
                if (ModManager.Instance.ModMode == ModMode.Combined)
                {
                    ModManager.Instance.SellItem(_researchItem);
                }

                if (ModManager.Instance.ModMode == ModMode.BuySell)
                {
                    ModManager.Instance.SellItem(_researchItem); 
                }
                
                ProgressionManager.Instance.ResearchItem(_researchItem);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RenderHelpers.DrawMenuBox(_researchArea.bounds.X, _researchArea.bounds.Y,
                _researchArea.bounds.Width, _researchArea.bounds.Height, out var areaInnerAnchors);

            var researchItemCellX = areaInnerAnchors.X + _researchArea.bounds.Width / 2f - Game1.tileSize / 2f;
            RenderHelpers.DrawItemBox((int) researchItemCellX, (int) areaInnerAnchors.Y + 10, Game1.tileSize,
                Game1.tileSize,
                out _);

            var researchProgressString = GetItemProgression();

            var progressFont = Game1.smallFont;
            var progressPositionX = areaInnerAnchors.X + _researchArea.bounds.Width / 2f -
                                    progressFont.MeasureString(researchProgressString).X / 2f;

            spriteBatch.DrawString(progressFont, researchProgressString,
                new Vector2(progressPositionX, areaInnerAnchors.Y + Game1.tileSize + 10), Color.Black);


            var buttonTexture = ModManager.Instance.ModMode switch
            {
                ModMode.BuySell => _sellTexture,
                ModMode.Combined => _combinedTexture,
                _ => _researchTexture
            };



            spriteBatch.Draw(buttonTexture, _researchButton.bounds, _researchButton.sourceRect, Color.White);

            _researchItem?.drawInMenu(spriteBatch, new Vector2(researchItemCellX, areaInnerAnchors.Y + 10), 1f);
        }

        public void PrepareToBeKilled()
        {
            ProgressionManager.OnStackChanged -= OnStackChanged;
        }

        private string GetItemProgression()
        {
            if (_researchItem == null)
            {
                return "(0 / 0)";
            }

            if (_lastItem == null || !_lastItem.Equals(_researchItem))
            {
                _itemProgression = ProgressionManager.Instance.GetItemProgression(_researchItem, true);
                _lastItem = _researchItem;
            }

            return _itemProgression;
        }

        private void OnStackChanged(int newCount)
        {
            _lastItem = null; //update cached progression string

/*            if (ModManager.Instance.ModMode == ModMode.BuySell && _researchItem != null)
            {
                var amount = _researchItem.Stack - newCount;
            }*/

            if (newCount <= 0)
            {
                _researchItem = null;
            }
            else if (_researchItem != null)
            {
                _researchItem.Stack = newCount % _researchItem.maximumStackSize();
            }
        }
    }
}