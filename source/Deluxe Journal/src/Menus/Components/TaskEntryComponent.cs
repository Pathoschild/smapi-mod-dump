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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Task;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>A component that encapsulates a task entry in the TasksPage.</summary>
    public class TaskEntryComponent : ClickableComponent
    {
        public readonly ClickableTextureComponent checkbox;
        public readonly ClickableTextureComponent removeButton;
        public readonly ProgressBar progressBar;

        private readonly ITranslationHelper _translation;
        private readonly SpriteFont _font;
        private readonly Dictionary<char, SpriteFont.Glyph> _glyphs;
        private readonly int _halfLineSpacing;
        private readonly int _ellipsisWidth;
        private readonly int _centerY;

        private double _hoverStartTime;
        private bool _hovering;
        private bool _nameTruncated;

        public TaskEntryComponent(Rectangle bounds, string name, ITranslationHelper translation)
            : base(bounds, name)
        {
            if (DeluxeJournalMod.UiTexture == null)
            {
                throw new InvalidOperationException("TaskEntryComponent created before assigning UI Texture");
            }

            _translation = translation;
            _font = Game1.dialogueFont;
            _glyphs = _font.GetGlyphs();
            _halfLineSpacing = (_font.LineSpacing / 2) - (_font.LineSpacing / 2 % 4);
            _ellipsisWidth = (int)_font.MeasureString("...").X;
            _centerY = bounds.Y + (bounds.Height / 2);
            _hoverStartTime = 0;
            _hovering = false;
            _nameTruncated = false;

            checkbox = new ClickableTextureComponent(
                new Rectangle(bounds.X + 24, _centerY - 20, 36, 36),
                DeluxeJournalMod.UiTexture,
                new Rectangle(16, 16, 9, 9),
                4f);

            removeButton = new ClickableTextureComponent(
                new Rectangle(bounds.Right - 60, _centerY - 22, 40, 40),
                DeluxeJournalMod.UiTexture,
                new Rectangle(61, 16, 10, 10),
                4f);

            progressBar = new ProgressBar(new Rectangle(bounds.Right - 260, _centerY - 30, 248, 56), 0)
            {
                texture = DeluxeJournalMod.UiTexture,
                barLeftSourceRect = new Rectangle(0, 64, 5, 14),
                barMiddleSourceRect = new Rectangle(5, 64, 37, 14),
                barRightSourceRect = new Rectangle(42, 64, 5, 14),
                notchSourceRect = new Rectangle(47, 64, 1, 14),
                AlignText = ProgressBar.TextAlignment.Center,
                TextMargin = new Vector2(80, 28 - _halfLineSpacing)
            };
        }

        public double TimeHovering()
        {
            return _hovering ? Game1.currentGameTime.TotalGameTime.TotalSeconds - _hoverStartTime : 0;
        }

        public bool TryHover(int x, int y)
        {
            bool hovering = containsPoint(x, y);
            removeButton.tryHover(x, y, 0.4f);

            if (hovering && !_hovering)
            {
                _hoverStartTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;
            }

            _hovering = hovering;
            return _hovering;
        }

        public bool IsNameTruncated()
        {
            return _nameTruncated;
        }

        public void Draw(SpriteBatch b, ITask task, bool popOut = false)
        {
            string name = task.Name;
            bool complete = task.Complete;
            bool pulse = !task.HasBeenViewed();
            int nameWidth = bounds.Width - 76;
            Rectangle scaledBounds = bounds;
            Color textColor;

            if (popOut)
            {
                scaledBounds.Inflate(2, 2);
            }

            IClickableMenu.drawTextureBox(b,
                Game1.mouseCursors,
                new Rectangle(384, 396, 15, 15),
                scaledBounds.X,
                scaledBounds.Y - 4,
                scaledBounds.Width,
                scaledBounds.Height + 4,
                _hovering ? Color.Wheat : Color.White,
                4f,
                drawShadow: popOut);

            if (task.Active)
            {
                textColor = Game1.textColor;
                checkbox.sourceRect.X = (complete && !pulse) ? 25 : 16;
                checkbox.draw(b);

                if (complete && pulse)
                {
                    Utility.drawWithShadow(b,
                        DeluxeJournalMod.UiTexture,
                        new Vector2(checkbox.bounds.X + 20, checkbox.bounds.Y + 20),
                        new Rectangle(26, 25, 7, 7),
                        Color.White,
                        0,
                        new Vector2(4f),
                        checkbox.baseScale + Game1.dialogueButtonScale * 0.1f);
                }

                if (task.ShouldShowProgress())
                {
                    int count = task.Count;
                    int maxCount = task.MaxCount;

                    nameWidth -= 260;

                    if (complete && count < maxCount)
                    {
                        progressBar.Draw(b, _font, Color.DarkBlue, Color.Gray * 0.6f, count, maxCount);
                    }
                    else
                    {
                        progressBar.Draw(b, _font, Color.DarkBlue, count, maxCount);
                    }
                }
                else if (task.ShouldShowCustomStatus())
                {
                    Translation status = _translation.Get(task.GetCustomStatusKey()).UsePlaceholder(false);

                    if (status.HasValue())
                    {
                        string text = status.ToString();
                        int emojiEnd = text.StartsWith('[') ? text.IndexOf(']') : -1;
                        int textOffset = 0;

                        nameWidth -= 260;

                        if (emojiEnd > 0 && int.TryParse(text[1..emojiEnd], out int emojiIndex))
                        {
                            text = text[(emojiEnd + 1)..].TrimStart();
                            textOffset = 48;

                            Utility.drawWithShadow(b,
                                ChatBox.emojiTexture,
                                new Vector2(progressBar.bounds.X, _centerY - 22),
                                new Rectangle(emojiIndex * 9 % ChatBox.emojiTexture.Width, emojiIndex * 9 / ChatBox.emojiTexture.Width * 9, 9, 9),
                                Color.White,
                                0f,
                                Vector2.Zero,
                                4f);
                        }

                        Utility.drawTextWithShadow(b,
                            text,
                            _font,
                            new Vector2(progressBar.bounds.X + textOffset, _centerY - _halfLineSpacing - 6),
                            Color.DarkBlue);
                    }
                }
            }
            else
            {
                int daysRemaining = task.DaysRemaining();
                string daysRemainingKey = (daysRemaining == 1) ? "ui.tasks.renew.day" : "ui.tasks.renew.days";

                nameWidth -= 260;
                textColor = Game1.unselectedOptionColor;
                checkbox.sourceRect.X = checkbox.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 43 : 34;
                checkbox.draw(b);

                Utility.drawWithShadow(b,
                    DeluxeJournalMod.UiTexture,
                    new Vector2(progressBar.bounds.X, _centerY - 22),
                    new Rectangle(52, 16, 9, 9),
                    Color.White,
                    0,
                    Vector2.Zero,
                    4f);

                Utility.drawTextWithShadow(b,
                    _translation.Get(daysRemainingKey, new { count = daysRemaining }),
                    _font,
                    new Vector2(progressBar.bounds.X + 48, _centerY - _halfLineSpacing - 6),
                    Color.DarkBlue);
            }

            Utility.drawTextWithShadow(b,
                TruncateString(name, nameWidth),
                _font,
                new Vector2(bounds.X + 68, _centerY - _halfLineSpacing - 6),
                textColor);

            if (_hovering && (!Game1.options.SnappyMenus || popOut))
            {
                removeButton.draw(b);
            }
        }

        private string TruncateString(string text, float width)
        {
            float currentWidth = 0;
            int overflow = 0;
            int i;

            _nameTruncated = false;

            for (i = 0; i < text.Length; i++)
            {
                char c = text[i];
                SpriteFont.Glyph glyph;

                if (_glyphs.ContainsKey(c))
                {
                    glyph = _glyphs[c];
                }
                else if (_font.DefaultCharacter != null)
                {
                    glyph = _glyphs[(char)_font.DefaultCharacter];
                }
                else
                {
                    continue;
                }

                currentWidth += glyph.LeftSideBearing + glyph.Width;

                if (currentWidth > width)
                {
                    _nameTruncated = true;
                    return text[0..(i - overflow)] + "...";
                }
                else if (currentWidth > width - _ellipsisWidth)
                {
                    overflow++;
                }

                currentWidth += glyph.RightSideBearing + _font.Spacing;
            }

            return text;
        }
    }
}
