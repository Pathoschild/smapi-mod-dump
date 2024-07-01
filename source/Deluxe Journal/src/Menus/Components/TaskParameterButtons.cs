/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Task;

namespace DeluxeJournal.Menus.Components
{
    public class TaskParameterButtons : ClickableComponent, ITaskParameterComponent
    {
        private ButtonComponent? _selectedButton;
        private Rectangle _selectedButtonBounds;
        private bool _collapsed;

        public IList<ButtonComponent> Buttons { get; }

        public ClickableComponent ClickableComponent => this;

        public TaskParameter Parameter { get; set; }

        public string Label { get; set; } = string.Empty;

        public int Margin { get; set; }

        public ButtonComponent? SelectedButton
        {
            get => _selectedButton;

            set
            {
                if (_selectedButton != null)
                {
                    _selectedButton.Selected = false;
                }

                if ((_selectedButton = value) is ButtonComponent button)
                {
                    _selectedButton.Selected = true;
                    _selectedButtonBounds = new(
                        bounds.X - button.bounds.Width - Margin - 4,
                        bounds.Y,
                        button.bounds.Width,
                        button.bounds.Height);
                }
            }
        }

        public bool Collapsed
        {
            get => _collapsed;

            set
            {
                _collapsed = value;
                RecalculateBounds();
            }
        }

        public TaskParameterButtons(TaskParameter parameter, IEnumerable<ButtonComponent> buttons, Rectangle bounds, int margin = 0, bool collapsed = false)
            : base(bounds, parameter.Attribute.Name)
        {
            Buttons = buttons.ToList();
            Parameter = parameter;
            Margin = margin;
            Collapsed = collapsed;
            visible = false;

            if (parameter.Value is not int value)
            {
                throw new ArgumentException($"{nameof(TaskParameterButtons)} must have a parameter of type int.");
            }
            else
            {
                foreach (var button in buttons)
                {
                    if (button.Value == value)
                    {
                        SelectedButton = button;
                        break;
                    }
                }
            }

            RecalculateBounds();
        }

        public void RecalculateBounds()
        {
            int x = bounds.X;
            int y = bounds.Y;
            int rows = 0;
            int rowWidth = 0;
            int rowHeight = 0;

            for (int i = 0; i < Buttons.Count; i++)
            {
                ButtonComponent<int> button = Buttons[i];
                int buttonWidth = button.bounds.Width;
                int buttonHeight = button.bounds.Height;

                if (buttonHeight > rowHeight)
                {
                    rowHeight = buttonHeight;
                }
                
                if (rowWidth + buttonWidth > bounds.Width)
                {
                    if (Collapsed)
                    {
                        for (; i < Buttons.Count; i++)
                        {
                            Buttons[i].visible = false;
                        }
                        break;
                    }

                    y += rowHeight;
                    rowWidth = 0;
                    rowHeight = buttonHeight;
                    rows++;
                }

                button.visible = true;
                button.bounds = new Rectangle(x + rowWidth, y + Margin * rows, buttonWidth, buttonHeight);
                rowWidth += buttonWidth + Margin;
            }

            bounds.Height = y - bounds.Y + rowHeight + Margin * rows;
            RemapButtonNeighbors();
        }

        public void RemapButtonNeighbors()
        {
            int bottomRowY = Buttons.LastOrDefault()?.bounds.Y ?? 0;
            int i;

            if (Collapsed)
            {
                for (i = 0; i < Buttons.Count; i++)
                {
                    if (Buttons[i].bounds.Y > bounds.Y)
                    {
                        break;
                    }
                }
            }
            else
            {
                i = Buttons.Count;
            }

            for (i--; i >= 0; i--)
            {
                var button = Buttons[i];
                bool isBottomRow = button.bounds.Y >= bottomRowY;

                button.upNeighborID = button.bounds.Y <= bounds.Y ? upNeighborID : SNAP_AUTOMATIC;
                button.downNeighborID = isBottomRow ? downNeighborID : SNAP_AUTOMATIC;
                button.rightNeighborID = (i == Buttons.Count - 1 || Buttons[i + 1].bounds.X <= bounds.X) ? rightNeighborID : SNAP_AUTOMATIC;
                button.leftNeighborID = button.bounds.X <= bounds.X ? leftNeighborID : SNAP_AUTOMATIC;
            }
        }

        public IEnumerable<ClickableComponent> GetClickableComponents()
        {
            foreach (var button in Buttons)
            {
                yield return button;
            }

            yield return ClickableComponent;
        }

        public override bool containsPoint(int x, int y)
        {
            if (bounds.Contains(x, y))
            {
                Game1.SetFreeCursorDrag();
                return true;
            }

            return false;
        }

        public void TryHover(int x, int y)
        {
            foreach (var button in Buttons)
            {
                button.tryHover(x, y);
            }
        }

        public void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (var button in Buttons)
            {
                if (button.containsPoint(x, y))
                {
                    button.ReceiveLeftClick(x, y, playSound);
                    Parameter.TrySetValue((SelectedButton = button).Value);
                    return;
                }
            }
        }

        public void Draw(SpriteBatch b)
        {
            if (SelectedButton != null)
            {
                Rectangle listBounds = SelectedButton.bounds;
                bool isVisible = SelectedButton.visible;
                SelectedButton.bounds = _selectedButtonBounds;
                SelectedButton.visible = true;

                b.Draw(Game1.staminaRect, new Rectangle(_selectedButtonBounds.Right + Margin / 2, _selectedButtonBounds.Y, 4, _selectedButtonBounds.Height), Color.Black * 0.35f);
                SelectedButton.draw(b);

                SelectedButton.bounds = listBounds;
                SelectedButton.visible = isVisible;
            }

            foreach (var button in Buttons)
            {
                if (button.visible)
                {
                    button.draw(b);
                }
            }
        }
    }
}
