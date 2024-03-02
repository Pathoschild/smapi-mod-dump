/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using SAML.Elements;
using SAML.Menus;
using SAML;
using StardewValley;
using SAML.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SAML.Utilities;
using StardewModdingAPI;

namespace SAMLTest
{
    internal class TestMenu : Menu
    {
        private Label titleLabel;
        private StackPanel checkboxPanel;
        private WrapPanel itemBoxPanel;
        private ItemBox itemBox1;
        private ItemBox itemBox2;
        private ItemBox itemBox3;
        private ItemBox itemBox4;
        private TextBox inputBox;
        private MenuBox menuBox;
        private CheckBox visibilityCheckbox;

        public TestMenu()
        {
            Width = 800;
            Height = 600;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            titleLabel = new()
            {
                Text = "SAML",
                Font = Game1.dialogueFont,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            checkboxPanel = new()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new(16, 0, 0, 64)
            };

            checkboxPanel.Elements.Add(new Label()
            {
                Text = "Checkbox 1",
                Font = Game1.dialogueFont
            });
            checkboxPanel.Elements.Add(new CheckBox()
            {
                IsChecked = false,
                Margin = new(0, 0, 0, 128)
            });
            checkboxPanel.Elements.Add(new Label()
            {
                Text = "Checkbox 2",
                Font = Game1.dialogueFont
            });
            checkboxPanel.Elements.Add(new CheckBox()
            {
                IsChecked = true
            });

            itemBoxPanel = new()
            {
                Orientation = Orientation.Horizontal,
                MaxWidth = 160,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new(0, 32, 0, 32)
            };

            itemBoxPanel.Elements.AddRange(new[]
            {
                itemBox1 = new()
                {
                    Item = ItemRegistry.Create("(O)74", 77, 4),
                    Margin = new(0, 16, 0, 16)
                },
                itemBox2 = new()
                {
                    Item = ItemRegistry.Create("(O)74", 13, 4),
                    Margin = new(0, 16, 0, 16)
                },
                itemBox3 = new()
                {
                    Item = ItemRegistry.Create("(O)74", 56, 2),
                    Margin = new(0, 16, 0, 0)
                },
                itemBox4 = new()
                {
                    Item = ItemRegistry.Create("(O)122", 23, 0),
                    Margin = new(0, 16, 0, 0)
                },
            });

            inputBox = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "SAML"
            };

            visibilityCheckbox = new()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new(32, 0, 32, 0),
                IsChecked = true
            };

            menuBox = new(Width, Height);

            /*menuBox.Columns.AddRange(new GridElement[]
            {
                new() { EdgeDecoration = DecorationStyle.Smooth },
                new() { EdgeDecoration = DecorationStyle.Bauble },
                new() { EdgeDecoration = DecorationStyle.Smooth },
                new()
            });*/

            Elements.Add(titleLabel);
            Elements.Add(checkboxPanel);
            Elements.Add(itemBoxPanel);
            Elements.Add(inputBox);
            Elements.Add(visibilityCheckbox);

            inputBox.PropertyChanged += onTextBoxPropertyChanged;
            visibilityCheckbox.Toggle += onVisibilityToggle;
        }

        private void onVisibilityToggle(object sender, ToggleEventArgs e) => itemBox1.Visible = e.IsToggled;

        private void onTextBoxPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(TextBox.Text))
                return;
            titleLabel.Text = inputBox.Text;
            titleLabel.HorizontalAlignment = titleLabel.HorizontalAlignment;
        }

        public override void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.OnGameWindowSizeChanged(oldBounds, newBounds);
            menuBox.OnGameWindowSizeChanged();
        }

        public override void OnBeforeDraw(SpriteBatch b)
        {
            base.OnBeforeDraw(b);
            menuBox.Draw(b, X, Y);
        }
    }
}
