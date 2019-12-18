using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModSettingsTab.Menu;

namespace ModSettingsTab.Framework.Components
{
    public class FilterTextBox
    {
        public enum FilterType
        {
            Mod,
            Options
        }

        private static readonly Rectangle Sl = new Rectangle(0, 200, 14, 36);
        private static readonly Rectangle Sc = new Rectangle(14, 200, 4, 36);
        private static readonly Rectangle Sr = new Rectangle(18, 200, 42, 36);
        private readonly TextBox _textBox;

        private readonly Timer _filterSetTimer = new Timer(1000.0)
        {
            Enabled = false,
            AutoReset = false
        };

        public FilterTextBox(BaseOptionsModPage optionsModPage, FilterType filterType, int x, int y)
        {
            var options = optionsModPage.Options;
            _filterSetTimer.Elapsed += (t, e) =>
            {
                var searchText = _textBox.Text.Trim().ToLower();
                if (searchText.Length < 1 || searchText == string.Empty)
                {
                    optionsModPage.Options = options;
                    optionsModPage.SetScrollBarToCurrentIndex();
                }
                else
                {
                    if (optionsModPage.Options.Count > 0)
                    {
                        optionsModPage.snapToDefaultClickableComponent();
                        optionsModPage.SetScrollBarToCurrentIndex();
                    }

                    switch (filterType)
                    {
                        case FilterType.Mod:
                            optionsModPage.Options =
                                options.Where(o => ModData.ModList[o.ModId].Manifest.Name
                                        .Trim().ToLower().Contains(searchText))
                                    .ToList();
                            break;
                        case FilterType.Options:
                            optionsModPage.Options = options.Where(o =>
                                o.Label.Trim().ToLower().Contains(searchText)).ToList();
                            break;
                    }
                }
            };
            _textBox = new TextBox(_ =>
            {
                _filterSetTimer.Stop();
                _filterSetTimer.Start();
            })
            {
                X = x,
                Y = y,
                TitleText = "",
                Width = 192,
                Height = 36 * 2,
                Text = ""
            };
        }

        public void Update() => _textBox.Update();

        public void Draw(SpriteBatch b, bool drawShadow = true)
        {
            b.Draw(ModData.Tabs, new Rectangle(_textBox.X - 12, _textBox.Y - 12, Sl.Width * 2, Sl.Height * 2), Sl,
                Color.White);
            b.Draw(ModData.Tabs,
                new Rectangle(_textBox.X - 12 + Sl.Width * 2, _textBox.Y - 12, _textBox.Width, Sc.Height * 2), Sc,
                Color.White);
            b.Draw(ModData.Tabs,
                new Rectangle(_textBox.X - 12 + Sl.Width * 2 + _textBox.Width, _textBox.Y - 12, Sr.Width * 2,
                    Sr.Height * 2), Sr, Color.White);
            _textBox.Draw(b);
        }
    }
}