using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using StardewValleyMods.CategorizeChests.Framework;

namespace StardewValleyMods.CategorizeChests.Interface.Widgets
{
    /// <summary>
    /// A toggle button corresponding to a given kind of item, appearing as
    /// the icon for that item with an appropriate tooltip.
    /// </summary>
    class ItemToggle : Widget
    {
        private readonly IItemDataManager ItemDataManager;
        private readonly ITooltipManager TooltipManager;
        private readonly ItemKey ItemKey;

        public event Action OnToggle;

        public bool Active;

        private Widget Tooltip;

        public ItemToggle(IItemDataManager itemDataManager, ITooltipManager tooltipManager, ItemKey itemKey,
            bool active)
        {
            ItemDataManager = itemDataManager;
            TooltipManager = tooltipManager;

            ItemKey = itemKey;
            Active = active;

            Width = Game1.tileSize;
            Height = Game1.tileSize;

            var itemName = ItemDataManager.GetItem(ItemKey).Name;
            Tooltip = new ItemTooltip(itemName);
        }

        public override void Draw(SpriteBatch batch)
        {
            var proto = ItemDataManager.GetItem(ItemKey);
            var alpha = Active ? 1.0f : 0.25f;

            proto.drawInMenu(batch, new Vector2(GlobalPosition.X, GlobalPosition.Y), 1, alpha, 1, false);

            if (GlobalBounds.Contains(Game1.getMousePosition()))
                TooltipManager.ShowTooltipThisFrame(Tooltip);
        }

        public void Toggle()
        {
            Active = !Active;
            OnToggle?.Invoke();
        }

        public override bool ReceiveLeftClick(Point point)
        {
            Toggle();
            return true;
        }
    }
}