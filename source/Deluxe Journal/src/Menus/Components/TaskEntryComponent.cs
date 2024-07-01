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
using DeluxeJournal.Util;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>A component that encapsulates a task entry in the TasksPage.</summary>
    public class TaskEntryComponent : ClickableComponent
    {
        public readonly ClickableTextureComponent checkbox;
        public readonly ClickableTextureComponent removeButton;
        public readonly ProgressBar progressBar;

        private readonly ITranslationHelper _translation;
        private readonly SpriteFontTools _fontTools;
        private readonly int _halfLineSpacing;
        private readonly int _centerY;

        private double _hoverStartTime;
        private bool _hovering;

        public bool IsNameTruncated { get; private set; }

        public TaskEntryComponent(Rectangle bounds, string name, ITranslationHelper translation)
            : base(bounds, name)
        {
            if (DeluxeJournalMod.UiTexture == null)
            {
                throw new InvalidOperationException("TaskEntryComponent created before assigning UI Texture");
            }

            _translation = translation;
            _fontTools = new(Game1.dialogueFont);
            _halfLineSpacing = (_fontTools.LineSpacing / 2) - (_fontTools.LineSpacing / 2 % 4);
            _centerY = bounds.Y + (bounds.Height / 2);
            _hoverStartTime = 0;
            _hovering = false;

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
                barLeftSourceRect = new Rectangle(0, 64, 6, 14),
                barMiddleSourceRect = new Rectangle(6, 64, 34, 14),
                barRightSourceRect = new Rectangle(40, 64, 6, 14),
                notchSourceRect = new Rectangle(46, 67, 1, 8),
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
            removeButton.tryHover(x, y, 0.4f);

            if (!_hovering & (_hovering = containsPoint(x, y)))
            {
                _hoverStartTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;
            }

            return _hovering;
        }

        public void Draw(SpriteBatch b, ITask task, ColorSchema colorSchema, bool popOut = false)
        {
            string name = task.Name;
            bool complete = task.Complete;
            bool pulse = !task.HasBeenViewed();
            float nameWidth = bounds.Width - 76f;
            Rectangle borderBounds = new(bounds.X, bounds.Y - 4, bounds.Width, bounds.Height + 4);
            Rectangle contentBounds = new(bounds.X + 4, bounds.Y, bounds.Width - 8, bounds.Height - 4);
            Color shadowColor = colorSchema.Shadow;
            Color textColor = Game1.textColor;
            Color fillColor = task.IsHeader ? colorSchema.Header : (_hovering ? colorSchema.Hover : colorSchema.Main);
            Color borderColor = DeluxeJournalMod.TaskBorderColor;

            if (popOut)
            {
                borderBounds.Inflate(4, 4);
                contentBounds.Inflate(4, 4);
            }

            DrawCornerlessBox(b, borderBounds, borderColor);
            b.Draw(Game1.staminaRect, contentBounds, colorSchema.Corner);
            DrawCornerlessBox(b, contentBounds, colorSchema.Padding);
            b.Draw(Game1.staminaRect, new Rectangle(contentBounds.X + 4, contentBounds.Y + 4, contentBounds.Width - 8, contentBounds.Height - 8), fillColor);

            IClickableMenu.drawTextureBox(b,
                DeluxeJournalMod.ColoredTaskMask,
                new(0, 0, 15, 15),
                borderBounds.X,
                borderBounds.Y,
                borderBounds.Width,
                borderBounds.Height,
                colorSchema.Accent,
                4f,
                false);

            IClickableMenu.drawTextureBox(b,
                DeluxeJournalMod.ColoredTaskMask,
                new(16, 0, 15, 15),
                borderBounds.X,
                borderBounds.Y,
                borderBounds.Width,
                borderBounds.Height,
                colorSchema.Shadow,
                4f,
                false);

            if (!task.IsHeader)
            {
                if (task.Active)
                {
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

                        nameWidth -= 260f;

                        if (complete && count < maxCount)
                        {
                            progressBar.Draw(b, _fontTools.Font, Color.DarkBlue, Color.Gray * 0.6f, colorSchema, count, maxCount);
                        }
                        else
                        {
                            progressBar.Draw(b, _fontTools.Font, Color.DarkBlue, colorSchema, count, maxCount);
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

                            nameWidth -= 260f;

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

                            Utility.drawTextWithColoredShadow(b,
                                text,
                                _fontTools.Font,
                                new Vector2(progressBar.bounds.X + textOffset, _centerY - _halfLineSpacing - 6),
                                Color.DarkBlue,
                                shadowColor);
                        }
                    }
                }
                else
                {
                    int daysRemaining = task.DaysRemaining();
                    string daysRemainingKey = (daysRemaining == 1) ? "ui.tasks.renew.day" : "ui.tasks.renew.days";

                    nameWidth -= 260f;
                    textColor = Game1.unselectedOptionColor * 0.9f;
                    shadowColor *= 0.9f;
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

                    Utility.drawTextWithColoredShadow(b,
                        _translation.Get(daysRemainingKey, new { count = daysRemaining }),
                        _fontTools.Font,
                        new Vector2(progressBar.bounds.X + 48, _centerY - _halfLineSpacing - 6),
                        Color.DarkBlue,
                        shadowColor);
                }
            }

            Vector2 namePosition;
            IsNameTruncated = _fontTools.Truncate(name, nameWidth, out name);

            if (task.IsHeader)
            {
                nameWidth = _fontTools.Font.MeasureString(name).X;
                namePosition = new Vector2(bounds.X + (bounds.Width - nameWidth - 8f) / 2f, _centerY - _halfLineSpacing - 6);

                IClickableMenu.drawTextureBox(b,
                    DeluxeJournalMod.ColoredTaskMask,
                    new(32, 0, 15, 15),
                    (int)namePosition.X - 20,
                    contentBounds.Y + 8,
                    (int)nameWidth + 36,
                    contentBounds.Height - 16,
                    colorSchema.Main,
                    4f,
                    false);
            }
            else
            {
                namePosition = new Vector2(bounds.X + 68, _centerY - _halfLineSpacing - 6);
            }

            Utility.drawTextWithColoredShadow(b, name, _fontTools.Font, namePosition, textColor, shadowColor);

            if (_hovering && (!Game1.options.SnappyMenus || popOut))
            {
                removeButton.draw(b);
            }
        }

        private static void DrawCornerlessBox(SpriteBatch b, Rectangle bounds, Color color, int cornerSize = 4)
        {
            b.Draw(Game1.staminaRect, new Rectangle(bounds.X, bounds.Y + cornerSize, bounds.Width, bounds.Height - cornerSize * 2), color);
            b.Draw(Game1.staminaRect, new Rectangle(bounds.X + cornerSize, bounds.Y, bounds.Width - cornerSize * 2, cornerSize), color);
            b.Draw(Game1.staminaRect, new Rectangle(bounds.X + cornerSize, bounds.Bottom - cornerSize, bounds.Width - cornerSize * 2, cornerSize), color);
        }
    }
}
