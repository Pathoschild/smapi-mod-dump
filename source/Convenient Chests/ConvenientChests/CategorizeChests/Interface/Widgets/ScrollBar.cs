/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ConvenientChests.CategorizeChests.Interface.Widgets {
    public class ScrollBar : Widget {
        private ScrollBarRunner Runner;

        private int _scrollPosition;

        public int ScrollPosition {
            get => _scrollPosition;
            set {
                _scrollPosition = value;
                UpdateScroller();
            }
        }


        private int scrollMax;

        public int ScrollMax {
            get => scrollMax;
            set {
                scrollMax = value;
                UpdateScroller();
            }
        }

        public int Step { get; set; } = 1;

        public bool Visible { get; set; } = true;

        private SpriteButton ScrollUpButton   { get; }
        private SpriteButton ScrollDownButton { get; }
        private Rectangle    ScrollBackground;

        public ScrollBar() {
            ScrollUpButton   = new SpriteButton(Sprites.UpArrow);
            ScrollDownButton = new SpriteButton(Sprites.DownArrow);
            Runner           = new ScrollBarRunner {Width = 24};

            AddChild(ScrollUpButton);
            AddChild(ScrollDownButton);
            AddChild(Runner);

            PositionElements();

            ScrollUpButton.OnPress   += () => Scroll(-1);
            ScrollDownButton.OnPress += () => Scroll(+1);

            ModEntry.StaticHelper.Events.Input.ButtonReleased  += InputOnButtonReleased;
            ModEntry.StaticHelper.Events.GameLoop.UpdateTicked += GameLoopOnUpdateTicked;
        }

        public override void Dispose() {
            ModEntry.StaticHelper.Events.Input.ButtonReleased -= InputOnButtonReleased;
            ModEntry.StaticHelper.Events.GameLoop.UpdateTicked -= GameLoopOnUpdateTicked;

            base.Dispose();
        }

        protected override void OnDimensionsChanged() {
            if (Width != 64) {
                Width = 64;
                return;
            }

            base.OnDimensionsChanged();
            PositionElements();
        }

        private void PositionElements() {
            if (ScrollDownButton == null)
                return;

            ScrollUpButton.X   = 0;
            ScrollDownButton.X = 0;
            ScrollDownButton.Y = Height - ScrollDownButton.Height;

            ScrollBackground.X      = 20;
            ScrollBackground.Y      = ScrollUpButton.Height                                    - 4;
            ScrollBackground.Height = Height - ScrollUpButton.Height - ScrollDownButton.Height + 8;
            ScrollBackground.Width  = Runner.Width;
            Runner.X                = ScrollBackground.X;


            ScrollBackground.Location = Globalize(ScrollBackground.Location);
            UpdateScroller();
        }

        private void UpdateScroller() {
            if (Step == 0)
                return;

            Runner.Height = (int) (ScrollBackground.Height * (Step / (float) ScrollMax));
            Runner.Y      = 60 + (int) ((ScrollBackground.Height - Runner.Height) * Math.Min(1, (ScrollPosition / (float) (ScrollMax - Step))));
        }


        public override void Draw(SpriteBatch batch) {
            if (!Visible)
                return;

            // draw background
            IClickableMenu.drawTextureBox(batch, Game1.mouseCursors,
                                          new Rectangle(403, 383, 6, 6),
                                          ScrollBackground.X, ScrollBackground.Y, ScrollBackground.Width, ScrollBackground.Height,
                                          Color.White, 4f, false);

            base.Draw(batch);
        }

        public void Scroll(int direction) {
            if (ScrollMax == 0)
                return;

            ScrollPosition = Math.Max(0, Math.Min(ScrollMax, ScrollPosition + direction * Step));
            OnScroll?.Invoke(this, new ScrollBarEventArgs(ScrollPosition, direction));
            UpdateScroller();
        }


        protected bool _scrolling = false;

        public override bool ReceiveLeftClick(Point point) {
            var localPoint = new Point(point.X - Runner.Position.X, point.Y - Runner.Position.Y);
            if (Runner.LocalBounds.Contains(localPoint))
                _scrolling = true;

            return true;
        }

        /// <summary>
        /// Update ScrollRunner position and dispatch scrolling events
        /// </summary>
        private void GameLoopOnUpdateTicked(object sender, UpdateTickedEventArgs e) {
            if (!_scrolling)
                return;

            // check if scroll buttons are still active
            var buttons = new List<SButton> { SButton.MouseLeft }
               .Concat(Game1.options.useToolButton.Select(SButtonExtensions.ToSButton));

            if (!buttons.Any(b => ModEntry.StaticHelper.Input.IsDown(b) || ModEntry.StaticHelper.Input.IsSuppressed(b))) {
                _scrolling = false;
                return;
            }

            var mouseY   = Game1.getMouseY(true);
            var progress = Math.Min(Math.Max(0f, mouseY - ScrollBackground.Y) / (Height), 1);
            ScrollPosition = (int) (progress * ScrollMax);

            OnScroll?.Invoke(this, new ScrollBarEventArgs(ScrollPosition, mouseY < GlobalBounds.Y ? -1 : 1));
        }

        /// <summary>
        /// Cancel scrolling on button release
        /// </summary>
        private void InputOnButtonReleased(object sender, ButtonReleasedEventArgs e) {
            if (!_scrolling || e.IsSuppressed())
                // also check if the released button was suppressed
                return;

            _scrolling = false;
        }


        public override bool ReceiveScrollWheelAction(int amount) => base.ReceiveScrollWheelAction(amount);

        public event EventHandler<ScrollBarEventArgs> OnScroll;

        public class ScrollBarEventArgs : EventArgs {
            public ScrollBarEventArgs(int position, int direction) {
                Position  = position;
                Direction = direction;
            }

            public int Position  { get; set; }
            public int Direction { get; set; }
        }

        protected class ScrollBarRunner : Widget {
            private static readonly TextureRegion TextureTop = new TextureRegion(Game1.mouseCursors, new Rectangle(435, 463, 6, 3), true);
            private static readonly TextureRegion TextureMid = new TextureRegion(Game1.mouseCursors, new Rectangle(435, 466, 6, 4), true);
            private static readonly TextureRegion TextureBot = new TextureRegion(Game1.mouseCursors, new Rectangle(435, 470, 6, 3), true);

            public bool Visible { get; set; } = true;

            public override void Draw(SpriteBatch batch) {
                if (!Visible)
                    return;

                base.Draw(batch);

                var rect = GlobalBounds;
                batch.Draw(TextureMid, rect.X, rect.Y,                          rect.Width,       rect.Height);
                batch.Draw(TextureTop, rect.X, rect.Y,                          TextureTop.Width, TextureTop.Height);
                batch.Draw(TextureBot, rect.X, rect.Bottom - TextureBot.Height, TextureBot.Width, TextureBot.Height);
            }
        }
    }
}