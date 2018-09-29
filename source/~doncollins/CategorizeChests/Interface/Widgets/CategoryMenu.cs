using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValleyMods.CategorizeChests.Framework;

namespace StardewValleyMods.CategorizeChests.Interface.Widgets
{
    class CategoryMenu : Widget
    {
        public event Action OnClose;

        // Dependencies
        private readonly IItemDataManager ItemDataManager;

        private readonly ITooltipManager TooltipManager;
        private readonly ChestData ChestData;

        // Styling settings
        private SpriteFont HeaderFont => Game1.dialogueFont;

        private const int MaxItemColumns = 12;
        private int Padding => 2 * Game1.pixelZoom;

        // Widgets
        private Widget Body;

        private Widget TopRow;
        private LabeledCheckbox SelectAllButton;
        private SpriteButton CloseButton;
        private Background Background;
        private Label CategoryLabel;
        private WrapBag ToggleBag;
        private SpriteButton PrevButton;
        private SpriteButton NextButton;

        private IEnumerable<ItemToggle> ItemToggles => from child in ToggleBag.Children
            where child is ItemToggle
            select child as ItemToggle;

        private List<string> AvailableCategories;
        private string SelectedCategory;

        public CategoryMenu(ChestData chestData, IItemDataManager itemDataManager, ITooltipManager tooltipManager)
        {
            ItemDataManager = itemDataManager;
            TooltipManager = tooltipManager;

            ChestData = chestData;

            AvailableCategories = ItemDataManager.Categories.Keys.ToList();
            AvailableCategories.Sort();

            BuildWidgets();

            SetCategory(AvailableCategories.First());
        }

        private void BuildWidgets()
        {
            Background = AddChild(new Background(Sprites.MenuBackground));
            Body = AddChild(new Widget());
            TopRow = Body.AddChild(new Widget());
            ToggleBag = Body.AddChild(new WrapBag(MaxItemColumns * Game1.tileSize));

            NextButton = TopRow.AddChild(new SpriteButton(Sprites.RightArrow));
            PrevButton = TopRow.AddChild(new SpriteButton(Sprites.LeftArrow));
            NextButton.OnPress += () => CycleCategory(1);
            PrevButton.OnPress += () => CycleCategory(-1);

            SelectAllButton = TopRow.AddChild(new LabeledCheckbox("All"));
            SelectAllButton.OnChange += OnToggleSelectAll;

            CloseButton = AddChild(new SpriteButton(Sprites.ExitButton));
            CloseButton.OnPress += () => OnClose?.Invoke();

            CategoryLabel = TopRow.AddChild(new Label("", Color.Black, HeaderFont));
        }

        private void PositionElements()
        {
            Body.Position = new Point(Background.Graphic.LeftBorderThickness, Background.Graphic.RightBorderThickness);

            // Figure out width

            Body.Width = ToggleBag.Width;
            TopRow.Width = Body.Width;
            Width = Body.Width + Background.Graphic.LeftBorderThickness + Background.Graphic.RightBorderThickness +
                    Padding * 2;

            // Build the top row

            var HeaderWidth = (int) HeaderFont.MeasureString(" Animal Product ").X; // TODO
            NextButton.X = TopRow.Width / 2 + HeaderWidth / 2;
            PrevButton.X = TopRow.Width / 2 - HeaderWidth / 2 - PrevButton.Width;

            SelectAllButton.X = Padding;

            CategoryLabel.CenterHorizontally();

            TopRow.Height = TopRow.Children.Max(c => c.Height);

            foreach (var child in TopRow.Children)
                child.Y = TopRow.Height / 2 - child.Height / 2;

            // Figure out height and vertical positioning

            ToggleBag.Y = TopRow.Y + TopRow.Height + Padding;
            Body.Height = ToggleBag.Y + ToggleBag.Height;
            Height = Body.Height + Background.Graphic.TopBorderThickness + Background.Graphic.BottomBorderThickness +
                     Padding * 2;

            Background.Width = Width;
            Background.Height = Height;

            CloseButton.Position = new Point(Width - CloseButton.Width, 0);
        }

        private void OnToggleSelectAll(bool on)
        {
            if (on)
                SelectAll();
            else
                SelectNone();
        }

        private void SelectAll()
        {
            foreach (var toggle in ItemToggles)
            {
                if (!toggle.Active)
                    toggle.Toggle();
            }
        }

        private void SelectNone()
        {
            foreach (var toggle in ItemToggles)
            {
                if (toggle.Active)
                    toggle.Toggle();
            }
        }

        private void CycleCategory(int offset)
        {
            var index = AvailableCategories.FindIndex(c => c == SelectedCategory);
            var newCategory = AvailableCategories[Utility.Mod(index + offset, AvailableCategories.Count)];
            SetCategory(newCategory);
        }

        private void SetCategory(string category)
        {
            SelectedCategory = category;

            CategoryLabel.Text = category;

            RecreateItemToggles();

            SelectAllButton.Checked = AreAllSelected();

            PositionElements();
        }

        private void RecreateItemToggles()
        {
            ToggleBag.RemoveChildren();

            var itemKeys = ItemDataManager.Categories[SelectedCategory];

            foreach (var itemKey in itemKeys)
            {
                var toggle =
                    ToggleBag.AddChild(new ItemToggle(ItemDataManager, TooltipManager, itemKey, ChestData.Accepts(itemKey)));
                toggle.OnToggle += () => ToggleItem(itemKey);
            }
        }

        private void ToggleItem(ItemKey itemKey)
        {
            ChestData.Toggle(itemKey);
            SelectAllButton.Checked = AreAllSelected();
        }

        private bool AreAllSelected()
        {
            return ItemToggles.Count(t => !t.Active) == 0;
        }

        public override bool ReceiveLeftClick(Point point)
        {
            PropagateLeftClick(point);
            return true;
        }

        public override bool ReceiveScrollWheelAction(int amount)
        {
            CycleCategory(amount > 1 ? -1 : 1);
            return true;
        }
    }
}