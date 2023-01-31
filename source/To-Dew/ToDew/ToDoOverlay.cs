/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

// Copyright 2021 Jamie Taylor
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;

namespace ToDew {
    public class OverlayConfig {
        public bool enabled = true;
        public bool clickToMarkDone = true;
        public SButton hotkey = SButton.None;
        public KeybindList hotkeyList = new KeybindList();
        public bool hideAtFestivals = false;
        public int maxWidth = 600;
        public int maxItems = 10;
        public Color backgroundColor = Color.Black * 0.2f;
        public Color textColor = Color.White * 0.8f;
        public int offsetX = 0;
        public int offsetY = 0;
        public bool scaleWithUI = false;
        public static void RegisterConfigMenuOptions(Func<OverlayConfig> getThis, GenericModConfigMenuAPI api, GMCMOptionsAPI? apiExt, IManifest modManifest) {
            api.AddSectionTitle(modManifest, I18n.Config_Overlay, I18n.Config_Overlay_Desc);
            api.AddBoolOption(
                mod: modManifest,
                name: I18n.Config_Overlay_Enabled,
                tooltip: I18n.Config_Overlay_Enabled_Desc,
                getValue: () => getThis().enabled,
                setValue: (bool val) => getThis().enabled = val);
            api.AddBoolOption(
                mod: modManifest,
                name: I18n.Config_Overlay_ClickToMarkDone,
                tooltip: I18n.Config_Overlay_ClickToMarkDone_Desc,
                getValue: () => getThis().clickToMarkDone,
                setValue: (bool val) => getThis().clickToMarkDone = val);
            api.AddKeybind(
                mod: modManifest,
                name: I18n.Config_Overlay_Hotkey,
                tooltip: I18n.Config_Overlay_Hotkey_Desc,
                getValue: () => getThis().hotkey,
                setValue: (SButton val) => getThis().hotkey = val);
            api.AddBoolOption(
                mod: modManifest,
                name: I18n.Config_Overlay_HideAtFestivals,
                tooltip: I18n.Config_Overlay_HideAtFestivals_Desc,
                getValue: () => getThis().hideAtFestivals,
                setValue: (bool val) => getThis().hideAtFestivals = val);
            api.AddNumberOption(
                mod: modManifest,
                name: I18n.Config_Overlay_MaxWidth,
                tooltip: I18n.Config_Overlay_MaxWidth_Desc,
                getValue: () => getThis().maxWidth,
                setValue: (int val) => getThis().maxWidth = val);
            api.AddNumberOption(
                mod: modManifest,
                name: I18n.Config_Overlay_MaxItems,
                tooltip: I18n.Config_Overlay_MaxItems_Desc,
                getValue: () => getThis().maxItems,
                setValue: (int val) => getThis().maxItems = val);
            api.AddNumberOption(
                mod: modManifest,
                name: I18n.Config_Overlay_XOffset,
                tooltip: I18n.Config_Overlay_XOffset_Desc,
                getValue: () => getThis().offsetX,
                setValue: (int val) => getThis().offsetX = val);
            api.AddNumberOption(
                mod: modManifest,
                name: I18n.Config_Overlay_YOffset,
                tooltip: I18n.Config_Overlay_YOffset_Desc,
                getValue: () => getThis().offsetY,
                setValue: (int val) => getThis().offsetY = val);
            api.AddTextOption(
                mod: modManifest,
                name: I18n.Config_Overlay_Zoom,
                tooltip: I18n.Config_Overlay_Zoom_Desc,
                getValue: () => getThis().scaleWithUI ? "OptionsPage_UIScale" : "OptionsPage.cs.11254",
                setValue: (string val) => getThis().scaleWithUI = "OptionsPage_UIScale" == val,
                allowedValues: new[] { "OptionsPage_UIScale", "OptionsPage.cs.11254" },
                formatAllowedValue: (string val) => Game1.content.LoadString("Strings\\StringsFromCSFiles:" + val));
            if (apiExt is not null) {
                apiExt.AddColorOption(
                    mod: modManifest,
                    name: I18n.Config_Overlay_BackgroundColor,
                    tooltip: I18n.Config_Overlay_BackgroundColor_Desc,
                    getValue: () => getThis().backgroundColor,
                    setValue: (c) => getThis().backgroundColor = c,
                    colorPickerStyle: (uint)(GMCMOptionsAPI.ColorPickerStyle.AllStyles | GMCMOptionsAPI.ColorPickerStyle.RadioChooser));
                apiExt.AddColorOption(
                    mod: modManifest,
                    name: I18n.Config_Overlay_TextColor,
                    tooltip: I18n.Config_Overlay_TextColor_Desc,
                    getValue: () => getThis().textColor,
                    setValue: (c) => getThis().textColor = c,
                    colorPickerStyle: (uint)(GMCMOptionsAPI.ColorPickerStyle.AllStyles | GMCMOptionsAPI.ColorPickerStyle.RadioChooser));
            }
        }
    }
    public class ToDoOverlay : IDisposable {
        private readonly ModEntry theMod;
        private readonly ToDoList theList;
        private OverlayConfig config { get => theMod.config.overlay; }
        private string ListHeader = I18n.Overlay_Header();
        private const int marginTop = 5;
        private const int marginLeft = 5;
        private const int marginRight = 5;
        private const int marginBottom = 5;
        private const int lineSpacing = 5;
        private readonly SpriteFont font = Game1.smallFont;
        private readonly Vector2 ListHeaderSize;
        private Rectangle bounds;
        internal record struct Line(ToDoList.ListItem? Item, string Text, bool Bold, float Top, float Height, bool hasDoneButton);
        private List<Line> lines;
        private ToDoList.ListItem? mouseCurrentlyOverDoneButtonForItem;
        private static readonly Rectangle doneButtonSource = new(340, 410, 24, 10);
        public ToDoOverlay(ModEntry theMod, ToDoList theList) {
            this.theMod = theMod;
            this.theList = theList;
            // save "constant" values
            ListHeaderSize = font.MeasureString(ListHeader);
            // initialize rendering callback
            theMod.Helper.Events.Display.RenderedWorld += OnRenderedWorld;
            theMod.Helper.Events.Display.RenderingHud += OnRenderingHud;
            // initialize the list UI and callback
            theList.OnChanged += OnListChanged;
            // initialize the handler for clicking the "done" button
            theMod.Helper.Events.Input.ButtonPressed += OnButtonPressed;
            syncMenuItemList();
        }

        [MemberNotNull(nameof(lines))]
        private void syncMenuItemList() {
            lines = new List<Line>();
            if (theList.Items.Count == 0) return;
            float availableWidth = Math.Max(config.maxWidth - marginLeft - marginRight, ListHeaderSize.X);
            float usedWidth = ListHeaderSize.X;
            float topPx = marginTop + ListHeaderSize.Y;
            foreach (var item in theList.Items) {
                if (item.IsDone || item.HideInOverlay || ! item.IsVisibleToday) continue;
                if (lines.Count >= config.maxItems) {
                    float lineHeight = font.MeasureString("…").Y;
                    lines.Add(new Line(null, "…", Bold: false, topPx, lineHeight, hasDoneButton: false));
                    topPx += lineHeight;
                    break;
                }
                topPx += lineSpacing;
                string itemText = item.IsHeader ? item.Text : ("  " + item.Text);
                var lineSize = font.MeasureString(itemText);
                while (lineSize.X > availableWidth) {
                    if (itemText.Length < 2) {
                        // this really shouldn't happen
                        break;
                    }
                    itemText = itemText.Remove(itemText.Length - 2) + "…";
                    lineSize = font.MeasureString(itemText);
                }
                usedWidth = Math.Max(usedWidth, lineSize.X);
                lines.Add(new Line(item, itemText, Bold: item.IsBold, topPx, lineSize.Y, hasDoneButton: !item.IsHeader));
                topPx += lineSize.Y;
            }
            bounds = new Rectangle(config.offsetX, config.offsetY, (int)(usedWidth + marginLeft + marginRight), (int)topPx + marginBottom);
        }
        private void OnListChanged(object? sender, List<ToDoList.ListItem> e) {
            syncMenuItemList();
        }

        internal void ConfigSaved() {
            bounds.X = config.offsetX;
            bounds.Y = config.offsetY;
        }


        public void Dispose() {
            this.theList.OnChanged -= OnListChanged;
            theMod.Helper.Events.Display.RenderedWorld -= OnRenderedWorld;
            theMod.Helper.Events.Display.RenderingHud -= OnRenderingHud;
            theMod.Helper.Events.Input.ButtonPressed -= OnButtonPressed;
        }

        private void Draw(SpriteBatch spriteBatch) {
            if (lines.Count == 0
                || !config.enabled // shouldn't get this far, but why not check anyway
                || Game1.game1.takingMapScreenshot
                || Game1.eventUp || Game1.farmEvent != null
                || (config.hideAtFestivals && Game1.isFestival()))
            {
                mouseCurrentlyOverDoneButtonForItem = null;
                return;
            }
            Rectangle effectiveBounds = bounds;
            if (Game1.CurrentMineLevel > 0
                || Game1.currentLocation is VolcanoDungeon vd && vd.level.Value > 0
                || Game1.currentLocation is Club)
            {
                int adjust = Game1.uiMode ? (int)MathF.Ceiling(80f * Game1.options.zoomLevel / Game1.options.uiScale) : 80;
                effectiveBounds.Y = Math.Max(effectiveBounds.Y, adjust);
            }
            if (Game1.isOutdoorMapSmallerThanViewport()) {
                effectiveBounds.X = Math.Max(effectiveBounds.X, -Game1.uiViewport.X);
                effectiveBounds.Y = Math.Max(effectiveBounds.Y, -Game1.uiViewport.Y);
            }
            float topPx = effectiveBounds.Y + marginTop;
            float leftPx = effectiveBounds.X + marginLeft;
            spriteBatch.Draw(Game1.fadeToBlackRect, effectiveBounds, config.backgroundColor);
            Utility.drawBoldText(spriteBatch, ListHeader, font, new Vector2(leftPx, topPx), config.textColor);
            topPx += ListHeaderSize.Y;
            spriteBatch.DrawLine(leftPx, topPx, new Vector2(ListHeaderSize.X - 3, 1), config.textColor);
            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();
            ToDoList.ListItem? newMouseCurrentlyOverDoneButtonForItem = null;
            for (int i = 0; i < lines.Count; i++) {
                topPx += lineSpacing;
                if (lines[i].Bold) {
                    Utility.drawBoldText(spriteBatch, lines[i].Text, font, new Vector2(leftPx, topPx), config.textColor);
                } else {
                    spriteBatch.DrawString(font, lines[i].Text, new Vector2(leftPx, topPx), config.textColor);
                }
                if (config.clickToMarkDone
                    && lines[i].hasDoneButton
                    && (effectiveBounds.Left <= mouseX && mouseX < effectiveBounds.Right)
                    && (topPx <= mouseY && mouseY < topPx + lines[i].Height)) {
                    const int doneScale = 2;
                    //int doneLeft = effectiveBounds.Right - marginRight - doneScale * doneButtonSource.Width;
                    int doneLeft = effectiveBounds.Left + marginLeft;
                    Rectangle doneRect = new Rectangle(doneLeft,
                        (int)(topPx + (lines[i].Height - doneButtonSource.Height * doneScale) / 2),
                        doneButtonSource.Width * doneScale,
                        doneButtonSource.Height * doneScale);
                    bool mouseInDoneButton = doneRect.Contains(mouseX, mouseY);
                    newMouseCurrentlyOverDoneButtonForItem = mouseInDoneButton ? lines[i].Item : null;
                    spriteBatch.DrawSprite(Game1.mouseCursors, doneButtonSource, doneRect.Left, doneRect.Top, null, mouseInDoneButton ? 1.2f * (float)doneScale : (float)doneScale);
                }
                topPx += lines[i].Height;
            }
            mouseCurrentlyOverDoneButtonForItem = newMouseCurrentlyOverDoneButtonForItem;
        }

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e) {
            if (!config.scaleWithUI) Draw(e.SpriteBatch);
        }

        private void OnRenderingHud(object? sender, RenderingHudEventArgs e) {
            if (config.scaleWithUI) Draw(e.SpriteBatch);
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e) {
            if (mouseCurrentlyOverDoneButtonForItem is not null && e.Button is SButton.MouseLeft) {
                theList.SetItemDone(mouseCurrentlyOverDoneButtonForItem, true);
                Game1.playSound("coin");
                theMod.Helper.Input.Suppress(SButton.MouseLeft);
            }
        }
    }
}
