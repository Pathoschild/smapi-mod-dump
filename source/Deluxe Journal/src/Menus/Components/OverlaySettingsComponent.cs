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
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus.Components
{
    public class OverlaySettingsComponent : IClickableComponentSupplier
    {
        private readonly ButtonComponent _visibleCheckBox;
        private readonly ButtonComponent _visibilityLockButton;
        private readonly ButtonComponent _colorCheckBox;
        private readonly ColorPickerComponent _colorPicker;

        private Rectangle _bounds;
        private bool _hovering;

        public Rectangle Bounds => _bounds;

        public IOverlay Overlay { get; set; }

        public string Label { get; set; }

        public OverlaySettingsComponent(IOverlay overlay, Rectangle bounds, string label, int myId, int leftNeighborId = -1, int rightNeighborId = -1)
        {
            _bounds = bounds;
            Overlay = overlay;
            Label = label;

            _visibleCheckBox = new ButtonComponent(
                new(bounds.X + 24, bounds.Y + 32, 36, 36),
                DeluxeJournalMod.UiTexture!,
                new(16, 16, 9, 9),
                4f)
            {
                myID = myId,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = leftNeighborId,
                rightNeighborID = SNAP_AUTOMATIC,
                Selected = Overlay.IsVisible,
                SoundCueName = "tinyWhip",
                SoundPitch = (self) => self.Selected ? 1000 : 2000,
                OnClick = (self, _) => Overlay.IsVisible = self.Toggle()
            };

            _visibilityLockButton = new ButtonComponent(
                new(bounds.X + 68, bounds.Y + 28, 28, 40),
                DeluxeJournalMod.UiTexture!,
                new(71, 16, 7, 10),
                4f)
            {
                myID = myId + 1,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC,
                rightNeighborID = SNAP_AUTOMATIC,
                Selected = Overlay.IsVisibilityLocked,
                SoundCueName = "tinyWhip",
                SoundPitch = (self) => self.Selected ? 1000 : 2000,
                OnClick = (self, _) => Overlay.IsVisibilityLocked = self.Toggle()
            };

            _colorPicker = new ColorPickerComponent(bounds.Right - 304, bounds.Y + 20, myId + 3, SNAP_AUTOMATIC, rightNeighborId)
            {
                AlphaBlendColor = Overlay.CustomColor,
                IsEnabled = Overlay.IsColorSelected
            };

            _colorCheckBox = new ButtonComponent(
                new(bounds.Right - 348, bounds.Y + 32, 36, 36),
                DeluxeJournalMod.UiTexture!,
                new(16, 16, 9, 9),
                4f)
            {
                myID = myId + 2,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC,
                rightNeighborID = SNAP_AUTOMATIC,
                visible = Overlay.IsColorOptional,
                Selected = Overlay.IsColorSelected,
                SoundCueName = "tinyWhip",
                SoundPitch = (self) => self.Selected ? 1000 : 2000,
                OnClick = (self, _) => _colorPicker.IsEnabled = Overlay.IsColorSelected = self.Toggle()
            };

            _bounds.Height = _colorPicker.Bounds.Height + 40;
        }

        public IEnumerable<ClickableComponent> GetClickableComponents()
        {
            yield return _visibleCheckBox;
            yield return _visibilityLockButton;
            yield return _colorCheckBox;

            foreach (var slider in _colorPicker.GetClickableComponents().Take(3))
            {
                yield return slider;
            }
        }

        public void ApplySettings()
        {
            Overlay.CustomColor = _colorPicker.AlphaBlendColor;
        }

        public void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            if (_visibleCheckBox.containsPoint(x, y))
            {
                _visibleCheckBox.ReceiveLeftClick(x, y, playSound);
            }
            else if (_visibilityLockButton.containsPoint(x, y))
            {
                _visibilityLockButton.ReceiveLeftClick(x, y, playSound);
            }
            else if (_colorCheckBox.containsPoint(x, y))
            {
                _colorCheckBox.ReceiveLeftClick(x, y, playSound);
            }
            else if (!Game1.options.SnappyMenus && _colorPicker.IsEnabled && _colorPicker.Bounds.Contains(x, y))
            {
                _colorPicker.ReceiveLeftClick(x, y, playSound);
            }
        }

        public void LeftClickHeld(int x, int y)
        {
            _colorPicker.LeftClickHeld(x, y);
        }

        public void ReleaseLeftClick(int x, int y)
        {
            _colorPicker.ReleaseLeftClick(x, y);
        }

        public bool TryHover(int x, int y)
        {
            return _hovering = Bounds.Contains(x, y);
        }

        public void Draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b,
                Game1.mouseCursors,
                new(384, 396, 15, 15),
                _bounds.X,
                _bounds.Y,
                _bounds.Width,
                _bounds.Height,
                _hovering ? Color.Wheat : Color.White,
                4f,
                false);

            SpriteText.drawString(b, Label, _visibilityLockButton.bounds.Right + 32, _visibilityLockButton.bounds.Y);

            _visibleCheckBox.draw(b, Color.White, 0.86f, _visibleCheckBox.Selected ? 1 : 0);
            _visibilityLockButton.draw(b, Color.White, 0.86f, _visibilityLockButton.Selected ? 1 : 0);
            _colorCheckBox.draw(b, Color.White, 0.86f, _colorCheckBox.Selected ? 1 : 0);

            if (Overlay.IsColorSelected || Overlay.IsColorOptional)
            {
                _colorPicker.Draw(b);
            }
        }
    }
}
