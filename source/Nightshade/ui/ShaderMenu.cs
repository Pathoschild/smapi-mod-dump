/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/Nightshade
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace ichortower.ui
{
    public class ShaderMenu : IClickableMenu
    {
        public static int defaultWidth = 420;
        public static int defaultHeight = 720 - 64;
        public static int defaultX = 32;
        public static int defaultY = 32;

        public static Texture2D IconTexture = null;

        private List<Widget> children = new();
        // references to the two child widgets that have interop
        private Checkbox bySeasonToggle = null;
        private TabBar seasonSwitcher = null;

        private Widget heldChild = null;
        private Widget keyedChild = null;

        private ColorizerPreset[] ColorizerInitialStates = new ColorizerPreset[4] {
            new(), new(), new(), new()
        };
        private ColorizerPreset[] ColorizerActiveStates = new ColorizerPreset[4] {
            new(), new(), new(), new()
        };

        private ColorizerPreset CopyPasteBuffer = null;

        public ShaderMenu()
            : this(defaultX, defaultY)
        {
        }

        public ShaderMenu(int x, int y)
            : base(x, y, defaultWidth, defaultHeight, true)
        {
            LoadIcons();
            AddChildWidgets();
            LoadCurrentSettings();
            exitFunction = delegate {
                Nightshade.instance.ApplyConfig(Nightshade.Config);
            };
        }

        public static void LoadIcons()
        {
            IconTexture = Game1.content.Load<Texture2D>($"Mods/{Nightshade.ModId}/Icons");
        }

        private void AddChildWidgets()
        {
            int y = 20;
            int x = 20;
            // give labels the same height (27) as checkboxes so they line up
            // vertically (default valign is center)
            var lbl_colorizer = new Label(this,
                    new Rectangle(x, y, 0, 27),
                    text: TR.Get("menu.Colorizer.Text"));
            x += lbl_colorizer.Bounds.Width + 16;
            var chk_colorizeWorld = new Checkbox(this, x, y, "ColorizeWorld");
            x += chk_colorizeWorld.Bounds.Width + 8;
            var lbl_colorizeWorld = new Label(this,
                    new Rectangle(x, y, 0, 27),
                    text: TR.Get("menu.ColorizeWorld.Text"),
                    hoverText: TR.Get("menu.ColorizeWorld.Hover"),
                    activate: chk_colorizeWorld);
            x += lbl_colorizeWorld.Bounds.Width + 16;
            var chk_colorizeUI = new Checkbox(this, x, y, "ColorizeUI");
            x += chk_colorizeUI.Bounds.Width + 8;
            var lbl_colorizeUI = new Label(this,
                    new Rectangle(x, y, 0, 27),
                    text: TR.Get("menu.ColorizeUI.Text"),
                    hoverText: TR.Get("menu.ColorizeUI.Hover"),
                    activate: chk_colorizeUI);
            y += lbl_colorizer.Bounds.Height + 8;
            x = 20;
            var chk_colorBySeason = new Checkbox(this, x, y, "ColorizeBySeason");
            bySeasonToggle = chk_colorBySeason;
            x += chk_colorBySeason.Bounds.Width + 8;
            var lbl_colorBySeason = new Label(this,
                    new Rectangle(x, y, 0, 27),
                    text: TR.Get("menu.ColorizeBySeason.Text"),
                    hoverText: TR.Get("menu.ColorizeBySeason.Hover"),
                    activate: chk_colorBySeason);
            y += chk_colorBySeason.Bounds.Height + 16;

            var tbr_profiles = new TabBar(
                    new Rectangle(4, y, defaultWidth-8, 39),
                    new string[] {" 1 ", " 2 ", " 3 ", " 4 "},
                    parent: this);
            seasonSwitcher = tbr_profiles;
            y += tbr_profiles.Bounds.Height + 16;
            // same as before, give labels the same height (20) as the sliders.
            // this makes the labels render "too high" but it lines up
            var lbl_saturation = new Label(this,
                    new Rectangle(20, y, 128, 20),
                    text: TR.Get("menu.Saturation.Text"));
            var sld_saturation = new Slider(this, 156, y, name: "Saturation");
            y += lbl_saturation.Bounds.Height + 8;
            var lbl_lightness = new Label(this,
                    new Rectangle(20, y, 128, 20),
                    text: TR.Get("menu.Lightness.Text"));
            var sld_lightness = new Slider(this, 156, y, name: "Lightness");
            sld_lightness.Range = new int[]{-80, 80};
            y += lbl_lightness.Bounds.Height + 8;
            var lbl_contrast = new Label(this,
                    new Rectangle(20, y, 128, 20),
                    text: TR.Get("menu.Contrast.Text"));
            var sld_contrast = new Slider(this, 156, y, name: "Contrast");
            sld_contrast.Range = new int[]{-80, 80};
            y += lbl_contrast.Bounds.Height + 16;

            int buttonY = y;

            var colorBalance = TR.Get("menu.ColorBalance.Hover");
            var lbl_cyan = new Label(this, new Rectangle(20, y, 24, 60),
                    text: "C", hoverText: colorBalance);
            var lbl_red = new Label(this, new Rectangle(308, y, 24, 60),
                    text: "R", hoverText: colorBalance);
            var sld_redShadow = new Slider(this, 48, y, name: "ShadowR");
            var sld_redMidtone = new Slider(this, 48, y+20, name: "MidtoneR");
            var sld_redHighlight = new Slider(this, 48, y+40, name: "HighlightR");
            y += 60 + 8;
            var lbl_magenta = new Label(this, new Rectangle(20, y, 24, 60),
                    text: "M", hoverText: colorBalance);
            var lbl_green = new Label(this, new Rectangle(308, y, 24, 60),
                    text: "G", hoverText: colorBalance);
            var sld_greenShadow = new Slider(this, 48, y, name: "ShadowG");
            var sld_greenMidtone = new Slider(this, 48, y+20, name: "MidtoneG");
            var sld_greenHighlight = new Slider(this, 48, y+40, name: "HighlightG");
            y += 60 + 8;
            var lbl_yellow = new Label(this, new Rectangle(20, y, 24, 60),
                    text: "Y", hoverText: colorBalance);
            var lbl_blue = new Label(this, new Rectangle(308, y, 24, 60),
                    text: "B", hoverText: colorBalance);
            var sld_blueShadow = new Slider(this, 48, y, name: "ShadowB");
            var sld_blueMidtone = new Slider(this, 48, y+20, name: "MidtoneB");
            var sld_blueHighlight = new Slider(this, 48, y+40, name: "HighlightB");
            y += 60 + 16;
            var tbr_separator = new TabBar(
                    new Rectangle(4, y, defaultWidth-8, 2),
                    new string[] {}, parent: this);
            y += 2 + 16;

            // magic centering for when the future fifth button is added
            buttonY += 7;
            var btn_revert = new IconButton(this, defaultWidth-50, buttonY,
                    iconIndex: 0, hoverText: TR.Get("menu.RevertButton.Hover"),
                    onClick: RevertCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;
            var btn_clear = new IconButton(this, defaultWidth-50, buttonY,
                    iconIndex: 1, hoverText: TR.Get("menu.ClearButton.Hover"),
                    onClick: ClearCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;
            var btn_copy = new IconButton(this, defaultWidth-50, buttonY,
                    iconIndex: 2, hoverText: TR.Get("menu.CopyButton.Hover"),
                    onClick: CopyCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;
            var btn_paste = new IconButton(this, defaultWidth-50, buttonY,
                    iconIndex: 3, hoverText: TR.Get("menu.PasteButton.Hover"),
                    onClick: PasteCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;

            var chk_enableDepthOfField = new Checkbox(this, 20, y, "DepthOfFieldEnabled");
            var lbl_enableDepthOfField = new Label(this,
                    new Rectangle(56, y, 0, 27),
                    text: TR.Get("menu.EnableDepthOfField.Text"),
                    activate: chk_enableDepthOfField);
            y += chk_enableDepthOfField.Bounds.Height + 16;
            var lbl_field = new Label(this,
                    new Rectangle(20, y, 96, 20),
                    text: TR.Get("menu.Field.Text"),
                    hoverText: TR.Get("menu.Field.Hover"));
            var sld_field = new Slider(this, new Rectangle(126, y, 201, 20),
                    name: "Field", range: new int[]{0, 100});
            sld_field.ValueDelegate = sld_field.FloatRenderer(denom:100f);
            y += lbl_field.Bounds.Height + 8;
            var lbl_intensity = new Label(this,
                    new Rectangle(20, y, 96, 20),
                    text: TR.Get("menu.Intensity.Text"),
                    hoverText: TR.Get("menu.Intensity.Hover"));
            var sld_intensity = new Slider(this, new Rectangle(126, y, 201, 20),
                    name: "Intensity", range: new int[]{0, 100});
            sld_intensity.ValueDelegate = sld_intensity.FloatRenderer(denom:10f);
            y += lbl_intensity.Bounds.Height + 16;

            var btn_save = new TextButton(this, 0, 0,
                    text: TR.Get("menu.Save.Text"), onClick: SaveSettings);
            btn_save.Bounds.X = defaultWidth/2 - btn_save.Bounds.Width/2;
            btn_save.Bounds.Y = defaultHeight - btn_save.Bounds.Height - 8;

            this.children.AddRange(new List<Widget>() {
                lbl_colorizer, lbl_colorizeWorld, chk_colorizeWorld,
                lbl_colorizeUI, chk_colorizeUI,
                lbl_colorBySeason, chk_colorBySeason,
                tbr_profiles,
                lbl_saturation, sld_saturation,
                lbl_lightness, sld_lightness,
                lbl_contrast, sld_contrast,
                lbl_cyan, lbl_red,
                sld_redShadow, sld_redMidtone, sld_redHighlight,
                lbl_magenta, lbl_green,
                sld_greenShadow, sld_greenMidtone, sld_greenHighlight,
                lbl_yellow, lbl_blue,
                sld_blueShadow, sld_blueMidtone, sld_blueHighlight,
                btn_revert, btn_clear, btn_copy, btn_paste,
                tbr_separator,
                lbl_enableDepthOfField, chk_enableDepthOfField,
                lbl_field, sld_field,
                lbl_intensity, sld_intensity,
                btn_save,
            });
        }

        public void LoadCurrentSettings()
        {
            foreach (var child in this.children) {
                if (child is Checkbox ch) {
                    switch (ch.Name) {
                    case "ColorizeWorld":
                        ch.Value = Nightshade.Config.ColorizeWorld;
                        break;
                    case "ColorizeUI":
                        ch.Value = Nightshade.Config.ColorizeUI;
                        break;
                    case "ColorizeBySeason":
                        ch.Value = Nightshade.Config.ColorizeBySeason;
                        break;
                    case "DepthOfFieldEnabled":
                        ch.Value = Nightshade.Config.DepthOfFieldEnabled;
                        break;

                    }
                }
            }
            for (int i = 0; i < ColorizerInitialStates.Length; ++i) {
                ColorizerInitialStates[i] = Nightshade.Config.ColorizerProfiles[i].Clone();
                ColorizerActiveStates[i] = ColorizerInitialStates[i].Clone();
            }
            setSwitcherLabels();
            if (bySeasonToggle.Value) {
                seasonSwitcher.FocusedIndex = Game1.seasonIndex;
            }
            else {
                seasonSwitcher.FocusedIndex = Nightshade.Config.ColorizerActiveProfile;
            }
            LoadColorizerPreset(ColorizerInitialStates[seasonSwitcher.FocusedIndex]);
            LoadDepthOfFieldPreset(Nightshade.Config.DepthOfFieldSettings);

        }

        public void LoadColorizerPreset(ColorizerPreset set)
        {
            foreach (var child in this.children) {
                if (child is Slider ch) {
                    switch (ch.Name) {
                    case "Saturation":
                        ch.Value = (int)(set.Saturation * 100);
                        break;
                    case "Lightness":
                        ch.Value = (int)(set.Lightness * 100);
                        break;
                    case "Contrast":
                        ch.Value = (int)(set.Contrast * 100);
                        break;
                    case "ShadowR":
                        ch.Value = (int)(set.ShadowR * 100);
                        break;
                    case "ShadowG":
                        ch.Value = (int)(set.ShadowG * 100);
                        break;
                    case "ShadowB":
                        ch.Value = (int)(set.ShadowB * 100);
                        break;
                    case "MidtoneR":
                        ch.Value = (int)(set.MidtoneR * 100);
                        break;
                    case "MidtoneG":
                        ch.Value = (int)(set.MidtoneG * 100);
                        break;
                    case "MidtoneB":
                        ch.Value = (int)(set.MidtoneB * 100);
                        break;
                    case "HighlightR":
                        ch.Value = (int)(set.HighlightR * 100);
                        break;
                    case "HighlightG":
                        ch.Value = (int)(set.HighlightG * 100);
                        break;
                    case "HighlightB":
                        ch.Value = (int)(set.HighlightB * 100);
                        break;
                    }
                }
            }
        }

        public void LoadDepthOfFieldPreset(DepthOfFieldPreset set)
        {
            foreach (var child in this.children) {
                if ((child is Slider ch)) {
                    switch (ch.Name) {
                    case "Field":
                        ch.Value = (int)(set.Field * 100);
                        break;
                    case "Intensity":
                        ch.Value = (int)(set.Intensity * 10);
                        break;
                    }
                }
            }
        }

        public void SaveSettings()
        {
            ModConfig built = new();
            built.MenuKeybind = Nightshade.Config.MenuKeybind;
            foreach (var ch in this.children) {
                switch (ch.Name) {
                case "ColorizeWorld":
                    built.ColorizeWorld = (ch as Checkbox).Value;
                    break;
                case "ColorizeUI":
                    built.ColorizeUI = (ch as Checkbox).Value;
                    break;
                case "DepthOfFieldEnabled":
                    built.DepthOfFieldEnabled = (ch as Checkbox).Value;
                    break;
                case "Field":
                    built.DepthOfFieldSettings.Field = (float)(ch as Slider).Value / 100f;
                    break;
                case "Intensity":
                    built.DepthOfFieldSettings.Intensity = (float)(ch as Slider).Value / 10f;
                    break;
                }
            }
            for (int i = 0; i < ColorizerActiveStates.Length; ++i) {
                built.ColorizerProfiles[i] = ColorizerActiveStates[i].Clone();
            }
            built.ColorizeBySeason = bySeasonToggle.Value;
            built.ColorizerActiveProfile = seasonSwitcher.FocusedIndex;
            Nightshade.Config = built;
            Nightshade.instance.Helper.WriteConfig(Nightshade.Config);
            // toast "saved"
        }

        public override void draw(SpriteBatch b)
        {
            this.drawFrame(b, xPositionOnScreen, yPositionOnScreen,
                    width, height);
            base.draw(b);
            foreach (var child in this.children) {
                child.draw(b);
            }
            // only apply hover states and tooltips if not dragging a slider
            if (this.heldChild is null) {
                int modx = Game1.getMouseX() - this.xPositionOnScreen;
                int mody = Game1.getMouseY() - this.yPositionOnScreen;
                foreach (var child in this.children) {
                    child.InHoverState = child.Bounds.Contains(modx, mody);
                    if (child.InHoverState) {
                        if (child.HoverText?.Length > 0) {
                            var split = child.HoverText.Split("^");
                            string lines = Game1.parseText((split.Length > 1 ? split[1] : split[0]),
                                    Game1.smallFont, 280);
                            string title = (split.Length > 1 ? split[0] : null);
                            IClickableMenu.drawHoverText(b, font: Game1.smallFont,
                                    text: lines, boldTitleText: title);
                        }
                    }
                    else {
                        child.InActiveState = false;
                    }
                }
            }
            base.drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            int modx = x - this.xPositionOnScreen;
            int mody = y - this.yPositionOnScreen;
            foreach (var child in this.children) {
                if (child.Bounds.Contains(modx, mody)) {
                    child.InActiveState = true;
                    child.click(modx, mody, playSound);
                    if (child is Slider) {
                        this.heldChild = child;
                    }
                    this.keyedChild = child;
                    break;
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            if (this.heldChild != null) {
                int modx = x - this.xPositionOnScreen;
                int mody = y - this.yPositionOnScreen;
                this.heldChild.clickHold(modx, mody);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            this.heldChild = null;
            foreach (var child in this.children) {
                child.InActiveState = false;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (this.keyedChild != null) {
                this.keyedChild.keyPress(key);
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            int modx = Game1.getMouseX() - xPositionOnScreen;
            int mody = Game1.getMouseY() - yPositionOnScreen;
            foreach (var child in children) {
                if (child.Bounds.Contains(modx, mody)) {
                    child.scrollWheel(direction);
                    break;
                }
            }
        }

        public void onChildChange(Widget child)
        {
            if (child == bySeasonToggle) {
                setSwitcherLabels();
                return;
            }
            if (child == seasonSwitcher) {
                LoadColorizerPreset(ColorizerActiveStates[seasonSwitcher.FocusedIndex]);
            }
            ColorizerPreset current = ColorizerActiveStates[seasonSwitcher.FocusedIndex];
            if (child is Slider sl) {
                switch (sl.Name) {
                case "Saturation":
                    current.Saturation = sl.Value / 100f;
                    break;
                case "Lightness":
                    current.Lightness = sl.Value / 100f;
                    break;
                case "Contrast":
                    current.Contrast = sl.Value / 100f;
                    break;
                case "ShadowR":
                    current.ShadowR = sl.Value / 100f;
                    break;
                case "ShadowG":
                    current.ShadowG = sl.Value / 100f;
                    break;
                case "ShadowB":
                    current.ShadowB = sl.Value / 100f;
                    break;
                case "MidtoneR":
                    current.MidtoneR = sl.Value / 100f;
                    break;
                case "MidtoneG":
                    current.MidtoneG = sl.Value / 100f;
                    break;
                case "MidtoneB":
                    current.MidtoneB = sl.Value / 100f;
                    break;
                case "HighlightR":
                    current.HighlightR = sl.Value / 100f;
                    break;
                case "HighlightG":
                    current.HighlightG = sl.Value / 100f;
                    break;
                case "HighlightB":
                    current.HighlightB = sl.Value / 100f;
                    break;
                }
            }
            ModConfig built = new();
            foreach (var ch in this.children) {
                switch (ch.Name) {
                case "ColorizeWorld":
                    built.ColorizeWorld = (ch as Checkbox).Value;
                    break;
                case "ColorizeUI":
                    built.ColorizeUI = (ch as Checkbox).Value;
                    break;
                case "DepthOfFieldEnabled":
                    built.DepthOfFieldEnabled = (ch as Checkbox).Value;
                    break;
                case "Field":
                    built.DepthOfFieldSettings.Field = (float)(ch as Slider).Value / 100f;
                    break;
                case "Intensity":
                    built.DepthOfFieldSettings.Intensity = (float)(ch as Slider).Value / 10f;
                    break;
                }
            }
            // TODO use initial state if missing toggle is off
            for (int i = 0; i < ColorizerActiveStates.Length; ++i) {
                built.ColorizerProfiles[i] = ColorizerActiveStates[i].Clone();
            }
            // because this is controlling the live preview, we don't care if
            // colorize by season is on. force it off so we see the current
            // profile.
            built.ColorizeBySeason = false;
            built.ColorizerActiveProfile = seasonSwitcher.FocusedIndex;
            Nightshade.instance.ApplyConfig(built);
        }

        private void setSwitcherLabels()
        {
            if (bySeasonToggle.Value) {
                string pf = "Strings\\StringsFromCSFiles";
                seasonSwitcher.Labels = new string[] {
                    Utility.capitalizeFirstLetter(Game1.content.LoadString(pf+":spring")),
                    Utility.capitalizeFirstLetter(Game1.content.LoadString(pf+":summer")),
                    Utility.capitalizeFirstLetter(Game1.content.LoadString(pf+":fall")),
                    Utility.capitalizeFirstLetter(Game1.content.LoadString(pf+":winter")),
                };
            }
            else {
                seasonSwitcher.Labels = new string[] {" 1 ", " 2 ", " 3 ", " 4 "};
            }
        }

        public void drawFrame(SpriteBatch b, int x, int y, int w, int h)
        {
            Texture2D tex = Game1.menuTexture;
            Rectangle[] sources = Widget.nineslice(new Rectangle(64, 320, 60, 60), 8, 8);
            Rectangle[] dests = Widget.nineslice(new Rectangle(x, y, w, h), 8, 8);
            for (int i = 0; i < sources.Length; ++i) {
                b.Draw(tex, color: Color.White,
                        sourceRectangle: sources[i],
                        destinationRectangle: dests[i]);
            }
        }

        public void RevertCurrentProfile()
        {
            ref ColorizerPreset current = ref ColorizerActiveStates[seasonSwitcher.FocusedIndex];
            ColorizerPreset rev = ColorizerInitialStates[seasonSwitcher.FocusedIndex];
            current = rev.Clone();
            LoadColorizerPreset(current);
            onChildChange(null);
        }

        public void ClearCurrentProfile()
        {
            ref ColorizerPreset current = ref ColorizerActiveStates[seasonSwitcher.FocusedIndex];
            current = new();
            LoadColorizerPreset(current);
            onChildChange(null);
        }

        public void CopyCurrentProfile()
        {
            ColorizerPreset current = ColorizerActiveStates[seasonSwitcher.FocusedIndex];
            CopyPasteBuffer = current.Clone();
        }

        public void PasteCurrentProfile()
        {
            if (CopyPasteBuffer != null) {
                ref ColorizerPreset current = ref ColorizerActiveStates[seasonSwitcher.FocusedIndex];
                current = CopyPasteBuffer.Clone();
                LoadColorizerPreset(current);
                onChildChange(null);
            }
        }

    }
}
