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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using DeluxeJournal.Framework;
using DeluxeJournal.Framework.Events;
using DeluxeJournal.Menus.Components;

using static StardewValley.Menus.ClickableComponent;
using static DeluxeJournal.Menus.IOverlay;

namespace DeluxeJournal.Menus
{
    public class OverlaysPage : PageBase
    {
        [Flags]
        private enum DragEdges
        {
            None,
            Top = 1 << 0,
            Bottom = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3
        }

        private readonly struct DragPoint(Point position, Rectangle bounds, DragEdges edges, int resizeBoxIndex, IOverlay overlay)
        {
            /// <summary>Source position of the mouse click.</summary>
            public Point Position { get; } = position;

            /// <summary>Snapshot of the overlay bounds when clicked.</summary>
            public Rectangle BoundsSnapshot { get; } = bounds;

            /// <summary>Edges to be dragged or <see cref="DragEdges.None"/> to drag the entire overlay.</summary>
            public DragEdges Edges { get; } = edges;

            /// <summary>Index of the clicked resize box produced by <see cref="GetOverlayResizeBoxes"/>.</summary>
            public int ResizeBoxIndex { get; } = resizeBoxIndex;

            /// <summary>The target overlay instance.</summary>
            public IOverlay Overlay { get; } = overlay;

            /// <summary><c>true</c> if this is a resize operation or <c>false</c> if this is a move operation.</summary>
            public bool IsResizing => Edges != DragEdges.None;
        }

        private const int LabelWidth = 384;
        private const int ResizeBoxRegion = ResizeBoxSize * 2;
        private const int BorderlessResizeBoxRegion = ResizeBoxRegion - 2;

        private static readonly Color OutlineColor = Color.Cyan;
        private static readonly Color HoverColor = Color.Red;

        public readonly ButtonComponent hotkeySetButton;
        public readonly ButtonComponent editModeButton;
        public readonly ButtonComponent cancelButton;

        private readonly ColorPickerComponent _backgroundColorPicker;

        private readonly ITranslationHelper _translation;
        private readonly Config _config;
        private readonly OverlayManager _overlayManager;
        private readonly List<OverlaySettingsComponent> _overlaySettingsComponents;
        private readonly Rectangle _contentBounds;

        private DragPoint? _dragPoint;
        private bool _dirty;
        private bool _editMode;
        private bool _muteHoverSoundOnce;

        public override bool ParentElementsDisabled => _editMode;

        private bool EditMode
        {
            get => _editMode;

            set
            {
                if (_editMode != (_editMode = value))
                {
                    _overlayManager.SetEditing(value);
                    cancelButton.visible = value;
                    SaveSettings();
                }
            }
        }

        public OverlaysPage(string name, Rectangle bounds, Texture2D tabTexture, ITranslationHelper translation)
            : this(name, translation.Get("ui.tab.overlays"), bounds.X, bounds.Y, bounds.Width, bounds.Height, tabTexture, new Rectangle(48, 0, 16, 16), translation)
        {
        }

        public OverlaysPage(string name, string title, int x, int y, int width, int height, Texture2D tabTexture, Rectangle tabSourceRect, ITranslationHelper translation)
            : base(name, title, x, y, width, height, tabTexture, tabSourceRect)
        {
            if (DeluxeJournalMod.Config is not Config config)
            {
                throw new InvalidOperationException($"{nameof(OverlaysPage)} created before mod entry.");
            }

            if (DeluxeJournalMod.EventManager is not EventManager events)
            {
                throw new InvalidOperationException($"{nameof(OverlaysPage)} created before instantiation of {nameof(EventManager)}");
            }

            if (DeluxeJournalMod.OverlayManager is not OverlayManager overlayManager)
            {
                throw new InvalidOperationException($"{nameof(OverlaysPage)} created before instantiation of {nameof(OverlayManager)}");
            }

            _translation = translation;
            _config = config;
            _overlayManager = overlayManager;
            _overlaySettingsComponents = [];
            _contentBounds = new(x + 32, y + 36, width - 60, height - 56);

            hotkeySetButton = new ButtonComponent(
                new(_contentBounds.Right - 292, _contentBounds.Y, 84, 44),
                Game1.mouseCursors,
                new(294, 428, 21, 11),
                4f,
                true)
            {
                myID = 100,
                downNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                hoverText = translation.Get("ui.overlays.hotkey", new { keybind = _overlayManager.ToggleKeybind.ToString() }),
                SoundCueName = string.Empty,
                OnClick = (self, _) => SetSnappyChildMenu(new InputListenerMenu(events.ModEvents, (Keybind keybind) =>
                {
                    KeybindList hotkey = new KeybindList(keybind);
                    _overlayManager.ToggleKeybind = hotkey;
                    self.hoverText = translation.Get("ui.overlays.hotkey", new { keybind = hotkey.ToString() });
                }))
            };

            _backgroundColorPicker = new ColorPickerComponent(
                _contentBounds.Right - 292,
                _contentBounds.Y + 60,
                101,
                CUSTOM_SNAP_BEHAVIOR)
            {
                AlphaBlendColor = BackgroundColor,
                EnableAlphaSlider = true
            };

            editModeButton = new ButtonComponent(
                new(x + width - 68, y + height, 60, 68),
                DeluxeJournalMod.UiTexture!,
                new(0, 32, 15, 17),
                4f)
            {
                myID = 1000,
                upNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                SoundCueName = "shwip",
                OnClick = (_, _) => EditMode = true
            };

            cancelButton = new ButtonComponent(
                new(Game1.uiViewport.Width - 128, Game1.uiViewport.Height - 128, 64, 64),
                Game1.mouseCursors,
                new(192, 256, 64, 64),
                1f)
            {
                myID = 1001,
                upNeighborID = SNAP_TO_DEFAULT,
                downNeighborID = SNAP_TO_DEFAULT,
                leftNeighborID = SNAP_TO_DEFAULT,
                rightNeighborID = SNAP_TO_DEFAULT,
                fullyImmutable = true,
                visible = false,
                SoundCueName = "bigDeSelect",
                OnClick = (_, _) => EditMode = false
            };

            Rectangle componentBounds = new(x + 16, y + height - 376, width - 32, 0);
            int componentId = 0;

            foreach (string pageId in PageRegistry.PriorityOrderedKeys.Where(overlayManager.Overlays.ContainsKey))
            {
                OverlaySettingsComponent component = new(
                    overlayManager.Overlays[pageId],
                    componentBounds,
                    translation.Get("ui.tab." + pageId),
                    componentId,
                    CUSTOM_SNAP_BEHAVIOR);
                
                _overlaySettingsComponents.Add(component);
                componentBounds.Y += component.Bounds.Height - 4;
                componentId += 10;
            }

            if (_overlaySettingsComponents.Count > 0)
            {
                foreach (var component in _overlaySettingsComponents.First().GetClickableComponents())
                {
                    if (component is not ColorSliderBar || component.name == ColorPickerComponent.HueSliderName)
                    {
                        component.upNeighborID = _backgroundColorPicker.AlphaSliderBar.myID;
                        component.upNeighborImmutable = true;
                    }
                }

                foreach (var component in _overlaySettingsComponents.Last().GetClickableComponents())
                {
                    if (component is not ColorSliderBar || component.name == ColorPickerComponent.ValueSliderName)
                    {
                        component.downNeighborID = editModeButton.myID;
                        component.downNeighborImmutable = true;
                    }
                }
            }
        }

        public override void OnVisible()
        {
            _dirty = true;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            cancelButton.bounds.X = Game1.uiViewport.Width - 128;
            cancelButton.bounds.Y = Game1.uiViewport.Height - 128;
        }

        public override void populateClickableComponentList()
        {
            base.populateClickableComponentList();
            allClickableComponents.AddRange(_backgroundColorPicker.GetClickableComponents());
            
            foreach (var overlayComponent in _overlaySettingsComponents)
            {
                allClickableComponents.AddRange(overlayComponent.GetClickableComponents());
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            if (EditMode)
            {
                currentlySnappedComponent = cancelButton;
            }
            else
            {
                _muteHoverSoundOnce = currentlySnappedComponent == null;
                currentlySnappedComponent = getComponentWithID(0);
            }

            snapCursorToCurrentSnappedComponent();
        }

        /// <summary>Freezes snappy cursor movement if <c>true</c>.</summary>
        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return !EditMode && Game1.options.SnappyMenus && currentlySnappedComponent is ColorSliderBar && Game1.oldPadState.IsButtonDown(Buttons.A);
        }

        public override void applyMovementKey(int direction)
        {
            if (EditMode)
            {
                currentlySnappedComponent = cancelButton;
                snapCursorToCurrentSnappedComponent();
                return;
            }

            base.applyMovementKey(direction);
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            switch (direction)
            {
                case Game1.left:
                    SnapToActiveTabComponent();
                    return;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (EditMode)
            {
                if (cancelButton.containsPoint(x, y))
                {
                    cancelButton.ReceiveLeftClick(x, y, playSound);
                    return;
                }

                Point mousePosition = new(x, y);

                foreach (IOverlay overlay in _overlayManager.Overlays.Values)
                {
                    Rectangle bounds = overlay.Bounds;

                    if (!InflatedBoundsContains(bounds, mousePosition, BorderlessResizeBoxRegion, BorderlessResizeBoxRegion))
                    {
                        continue;
                    }
                    else if (InflatedBoundsContains(bounds, mousePosition, -ResizeBoxRegion, -ResizeBoxRegion))
                    {
                        _dragPoint = new(mousePosition, bounds, DragEdges.None, -1, overlay);
                        return;
                    }
                    else
                    {
                        int resizeBoxIndex = 0;

                        foreach (Rectangle hoverBoxBounds in GetOverlayResizeBoxes(bounds, new(ResizeBoxRegion), 2f))
                        {
                            if (hoverBoxBounds.Contains(mousePosition))
                            {
                                DragEdges edges = DragEdges.None;

                                if (hoverBoxBounds.X < bounds.X)
                                {
                                    edges |= DragEdges.Left;
                                }
                                else if (hoverBoxBounds.X >= bounds.Right - 2)
                                {
                                    edges |= DragEdges.Right;
                                }

                                if (hoverBoxBounds.Y < bounds.Y)
                                {
                                    edges |= DragEdges.Top;
                                }
                                else if (hoverBoxBounds.Y >= bounds.Bottom - 2)
                                {
                                    edges |= DragEdges.Bottom;
                                }

                                if (edges != DragEdges.None)
                                {
                                    _dragPoint = new(mousePosition, bounds, edges, resizeBoxIndex, overlay);
                                    return;
                                }
                            }

                            resizeBoxIndex++;
                        }
                    }
                }
            }
            else
            {
                if (editModeButton.containsPoint(x, y))
                {
                    editModeButton.ReceiveLeftClick(x, y, playSound);
                }
                else if (!isWithinBounds(x, y))
                {
                    ExitJournalMenu(playSound);
                }
                else if (hotkeySetButton.containsPoint(x, y))
                {
                    hotkeySetButton.ReceiveLeftClick(x, y, playSound);
                }
                else if (!Game1.options.SnappyMenus && _backgroundColorPicker.Bounds.Contains(x, y))
                {
                    _backgroundColorPicker.ReceiveLeftClick(x, y, playSound);
                }
                else
                {
                    foreach (var overlayComponent in _overlaySettingsComponents)
                    {
                        if (overlayComponent.Bounds.Contains(x, y))
                        {
                            overlayComponent.ReceiveLeftClick(x, y, playSound);
                            break;
                        }
                    }
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!EditMode)
            {
                _backgroundColorPicker.LeftClickHeld(x, y);

                foreach (var overlayComponent in _overlaySettingsComponents)
                {
                    overlayComponent.LeftClickHeld(x, y);
                }

                return;
            }
            
            if (_dragPoint is not DragPoint dragPoint)
            {
                return;
            }

            if (dragPoint.IsResizing)
            {
                Point size = dragPoint.BoundsSnapshot.Size;
                Point position = dragPoint.BoundsSnapshot.Location;
                int deltaX = x - dragPoint.Position.X; 
                int deltaY = y - dragPoint.Position.Y;

                if (dragPoint.Edges.HasFlag(DragEdges.Left))
                {
                    position.X = Math.Min(Math.Max(position.X + deltaX, 0), dragPoint.BoundsSnapshot.Right - MinWidth);
                    size.X = Math.Max(size.X - deltaX, 0);
                }
                else if (dragPoint.Edges.HasFlag(DragEdges.Right))
                {
                    size.X = Math.Max(size.X + deltaX, 0);
                }

                if (dragPoint.Edges.HasFlag(DragEdges.Top))
                {
                    position.Y = Math.Min(Math.Max(position.Y + deltaY, 0), dragPoint.BoundsSnapshot.Bottom - MinHeight);
                    size.Y = Math.Max(size.Y - deltaY, 0);
                }
                else if (dragPoint.Edges.HasFlag(DragEdges.Bottom))
                {
                    size.Y = Math.Max(size.Y + deltaY, 0);
                }

                Rectangle bounds = new(position, size);

                if (bounds != dragPoint.Overlay.Bounds)
                {
                    bounds.Width = Math.Max(bounds.Width, MinWidth);
                    bounds.Height = Math.Max(bounds.Height, MinHeight);
                    dragPoint.Overlay.Bounds = bounds;
                }
            }
            else
            {
                dragPoint.Overlay.Position = new(
                    Math.Max(Math.Min(dragPoint.BoundsSnapshot.X - dragPoint.Position.X + x, Game1.uiViewport.Width - dragPoint.BoundsSnapshot.Width), 0),
                    Math.Max(Math.Min(dragPoint.BoundsSnapshot.Y - dragPoint.Position.Y + y, Game1.uiViewport.Height - dragPoint.BoundsSnapshot.Height), 0));
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            _dragPoint = null;

            if (!EditMode)
            {
                _backgroundColorPicker.ReleaseLeftClick(x, y);

                foreach (var overlayComponent in _overlaySettingsComponents)
                {
                    overlayComponent.ReleaseLeftClick(x, y);
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);

            if (EditMode && (key == Keys.Escape || (Game1.options.SnappyMenus && Game1.options.doesInputListContain(Game1.options.menuButton, key))))
            {
                EditMode = false;
                Game1.playSound(cancelButton.SoundCueName);
            }
        }

        public override void gamePadButtonHeld(Buttons b)
        {
            base.gamePadButtonHeld(b);

            if (currentlySnappedComponent is ColorSliderBar slider && slider.IsEnabled && Game1.oldPadState.IsButtonDown(Buttons.A))
            {
                switch (b)
                {
                    case Buttons.DPadLeft:
                    case Buttons.LeftThumbstickLeft:
                        slider.ValueInt--;
                        break;
                    case Buttons.DPadRight:
                    case Buttons.RightThumbstickRight:
                        slider.ValueInt++;
                        break;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            if (!EditMode)
            {
                foreach (var overlayComponent in _overlaySettingsComponents)
                {
                    if (overlayComponent.TryHover(x, y) && !overlayComponent.Bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
                    {
                        // Workaround for the sound playing when initially opening the journal on this page.
                        // The mouse position will always be snapped to the default QuestLog component for two
                        // updates due to how SMAPI overrides the game's input system.
                        if (_muteHoverSoundOnce)
                        {
                            _muteHoverSoundOnce = false;
                        }
                        else
                        {
                            Game1.playSound("Cowboy_gunshot");
                        }
                    }
                }

                if (Game1.options.SnappyMenus && currentlySnappedComponent is ColorSliderBar slider && slider.IsEnabled && !Game1.oldPadState.IsButtonDown(Buttons.A))
                {
                    Game1.mouseCursor = Game1.cursor_grab;
                }
            }

            editModeButton.tryHover(x, y, EditMode ? 0f : 0.1f);
            cancelButton.tryHover(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            Utility.drawTextWithShadow(b, hotkeySetButton.hoverText, Game1.dialogueFont, new(_contentBounds.X, _contentBounds.Y), Game1.textColor);
            hotkeySetButton.draw(b);

            Utility.drawTextWithShadow(b, _translation.Get("ui.overlays.backgroundcolor"), Game1.dialogueFont, new(_contentBounds.X, _backgroundColorPicker.Bounds.Y + 8), Game1.textColor);
            _backgroundColorPicker.Draw(b);

            int separatorY = yPositionOnScreen + height - 392;
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + 12, separatorY, 20, 16), new(387, 386, 5, 4), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + width - 32, separatorY, 20, 16), new(394, 386, 5, 4), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + 32, separatorY, width - 64, 16), new(392, 386, 1, 4), Color.White);

            foreach (var overlayComponent in _overlaySettingsComponents)
            {
                overlayComponent.Draw(b);
            }

            editModeButton.draw(b);
            b.Draw(DeluxeJournalMod.UiTexture,
                new Rectangle(editModeButton.bounds.X + 16, editModeButton.bounds.Y + 24, 28, 28),
                new(7, 49, 9, 9),
                Color.White);

            if (EditMode)
            {
                DrawEditMode(b);
            }
        }

        private void DrawEditMode(SpriteBatch b)
        {
            Point mousePosition = new(Game1.getOldMouseX(), Game1.getOldMouseY());

            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.25f);

            foreach (IOverlay overlay in _overlayManager.Overlays.Values)
            {
                if (!overlay.IsVisible)
                {
                    continue;
                }

                Rectangle bounds = overlay.EdgeSnappedBounds;
                bool isDragging = _dragPoint?.Overlay == overlay;
                bool isHovering = isDragging || (_dragPoint == null && InflatedBoundsContains(bounds, mousePosition, BorderlessResizeBoxRegion, BorderlessResizeBoxRegion));
                bool isHoveringMove = isHovering && InflatedBoundsContains(bounds, mousePosition, -ResizeBoxRegion, -ResizeBoxRegion);
                DragEdges dragEdges = isDragging ? _dragPoint!.Value.Edges : DragEdges.None;
                int hoverBoxIndex = 0;

                overlay.DrawInEditMode(b);

                Utility.drawWithShadow(b,
                    DeluxeJournalMod.UiTexture,
                    new(bounds.X + (bounds.Width - 26) / 2, bounds.Y + (bounds.Height - 26) / 2),
                    new(51, 51, 13, 13),
                    isHoveringMove ? HoverColor : OutlineColor,
                    0f,
                    Vector2.Zero,
                    2f);

                drawTextureBox(b,
                    DeluxeJournalMod.UiTexture,
                    OutlineSource,
                    bounds.X,
                    bounds.Y,
                    bounds.Width,
                    bounds.Height,
                    isHoveringMove ? HoverColor : OutlineColor,
                    isHoveringMove ? 4f : 2f,
                    false);

                if (isHovering && !isHoveringMove)
                {
                    foreach (Rectangle hoverBoxBounds in GetOverlayResizeBoxes(bounds, new(ResizeBoxRegion), 2f))
                    {
                        if (isDragging ? _dragPoint!.Value.ResizeBoxIndex == hoverBoxIndex : hoverBoxBounds.Contains(mousePosition))
                        {
                            if (hoverBoxBounds.X < bounds.X || dragEdges.HasFlag(DragEdges.Left))
                            {
                                b.Draw(Game1.staminaRect, new Rectangle(bounds.X, bounds.Y, 4, bounds.Height), HoverColor);
                            }
                            else if (hoverBoxBounds.X >= bounds.Right - 2 || dragEdges.HasFlag(DragEdges.Right))
                            {
                                b.Draw(Game1.staminaRect, new Rectangle(bounds.Right - 4, bounds.Y, 4, bounds.Height), HoverColor);
                            }

                            if (hoverBoxBounds.Y < bounds.Y || dragEdges.HasFlag(DragEdges.Top))
                            {
                                b.Draw(Game1.staminaRect, new Rectangle(bounds.X, bounds.Y, bounds.Width, 4), HoverColor);
                            }
                            else if (hoverBoxBounds.Y >= bounds.Bottom - 2 || dragEdges.HasFlag(DragEdges.Bottom))
                            {
                                b.Draw(Game1.staminaRect, new Rectangle(bounds.X, bounds.Bottom - 4, bounds.Width, 4), HoverColor);
                            }

                            break;
                        }

                        hoverBoxIndex++;
                    }
                }

                foreach (Rectangle boxBounds in GetOverlayResizeBoxes(bounds, new(ResizeBoxSize), 2f))
                {
                    bool isHoveringThisBox = isHovering && !isHoveringMove && hoverBoxIndex-- == 0;

                    if (isHoveringThisBox)
                    {
                        Game1.mouseCursor = Game1.cursor_grab;
                    }

                    drawTextureBox(b,
                        DeluxeJournalMod.UiTexture,
                        OutlineSource,
                        boxBounds.X,
                        boxBounds.Y,
                        boxBounds.Width,
                        boxBounds.Height,
                        isHoveringThisBox ? HoverColor : OutlineColor,
                        isHoveringThisBox ? 4f : 2f,
                        false);
                }

                if (isHoveringMove)
                {
                    Game1.mouseCursor = Game1.cursor_grab;
                }
            }

            cancelButton.draw(b);
        }

        public override bool readyToClose()
        {
            return !EditMode;
        }

        protected override void cleanupBeforeExit()
        {
            if (EditMode)
            {
                EditMode = false;
            }
            else
            {
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            if (_dirty)
            {
                foreach (var overlayComponent in _overlaySettingsComponents)
                {
                    overlayComponent.ApplySettings();
                }

                _overlayManager.SaveSettings();
                _overlayManager.SetBackgroundColor(_backgroundColorPicker.AlphaBlendColor);
            }
        }

        private static IEnumerable<Rectangle> GetOverlayResizeBoxes(Rectangle overlayBounds, Point boxSize, float scale = 1f)
        {
            Rectangle boxBounds = new(Point.Zero, boxSize);

            for (float heightMod = 0f; heightMod <= 1f; heightMod += 0.5f)
            {
                for (float widthMod = 0f; widthMod <= 1f; widthMod += 0.5f)
                {
                    if (heightMod == 0.5f && widthMod == 0.5f)
                    {
                        continue;
                    }

                    boxBounds.X = overlayBounds.X + (int)(overlayBounds.Width * widthMod + (widthMod - 1f) * boxBounds.Width - (widthMod * 2f - 1f) * scale);
                    boxBounds.Y = overlayBounds.Y + (int)(overlayBounds.Height * heightMod + (heightMod - 1f) * boxBounds.Height - (heightMod * 2f - 1f) * scale);

                    yield return boxBounds;
                }
            }
        }

        private static bool InflatedBoundsContains(Rectangle bounds, Point point, int inflateHorizontal, int inflateVertical)
        {
            bounds.Inflate(inflateHorizontal, inflateVertical);
            return bounds.Contains(point);
        }
    }
}
