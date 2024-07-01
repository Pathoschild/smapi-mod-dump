/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
//using StardewValley.BellsAndWhistles;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace ichortower_HatMouseLacey
{
    internal class HatRegistryMenu : IClickableMenu
    {
        private static string _MenuOpenSound = "shwip";
        private static string _PageTurnSound = "shwip";
        private static int _InnerPadding = 4;
        private static int _OuterPadding = 8;
        private static int _BorderWidth = 16;
        private static int _RoomForTitle = 72;

        private static int _Columns = 8;
        private static int _Rows = 6;

        private static int _DefaultWidth = (20*4)*_Columns + _InnerPadding*7 +
                _OuterPadding*2 + _BorderWidth*2;
        private static int _DefaultHeight = _RoomForTitle + (20*4)*_Rows +
                _InnerPadding*5 + _OuterPadding*2 + _BorderWidth*2;

        private Rectangle[] _FrameNineslice = new Rectangle[9] {
            new(16, 16, 28, 28),
            new(44, 16, 28, 28),
            new(212, 16, 28, 28),
            new(16, 44, 28, 28),
            Rectangle.Empty,
            new(212, 44, 28, 28),
            new(16, 212, 28, 28),
            new(44, 212, 28, 28),
            new(212, 212, 28, 28),
        };
        private Rectangle _FrameBg = new(64, 128, 64, 64);

        private List<List<ClickableTextureComponent>> _Pages = new();
        private int _CurrentPage = 0;
        public int CurrentPage {
            get {
                return _CurrentPage;
            }
            set {
                _CurrentPage = Math.Max(0, Math.Min(value, _Pages.Count-1));
            }
        }
        public ClickableTextureComponent BackButton;
        public ClickableTextureComponent ForwardButton;
        private string _HoverText;

        public HatRegistryMenu(bool playSound = true)
            : base(Game1.uiViewport.Width/2 - _DefaultWidth/2,
                Game1.uiViewport.Height/2 - _DefaultHeight/2,
                _DefaultWidth, _DefaultHeight, true)
        {
            Dictionary<string, string> hatData = DataLoader.Hats(Game1.content);
            BackButton = new ClickableTextureComponent(new Rectangle(
                        xPositionOnScreen + _BorderWidth + _OuterPadding*2,
                        yPositionOnScreen + _BorderWidth + _OuterPadding*2, 48, 44),
                    texture: Game1.mouseCursors,
                    sourceRect: new Rectangle(352, 495, 12, 11),
                    scale: 4f);
            ForwardButton = new ClickableTextureComponent(new Rectangle(
                        xPositionOnScreen + width - _BorderWidth - _OuterPadding*2 - 48,
                        yPositionOnScreen + _BorderWidth + _OuterPadding*2, 48, 44),
                    texture: Game1.mouseCursors,
                    sourceRect: new Rectangle(365, 495, 12, 11),
                    scale: 4f);
            int count = 0;
            int baseX = xPositionOnScreen + _BorderWidth + _OuterPadding;
            int baseY = yPositionOnScreen + _BorderWidth + _OuterPadding + _RoomForTitle;
            int pagesize = _Columns * _Rows;
            foreach (var kvp in hatData) {
                int subcount = count % 48;
                if (subcount == 0) {
                    _Pages.Add(new List<ClickableTextureComponent>());
                }
                int xpos = baseX + (subcount%_Columns)*(20*4 + _InnerPadding);
                int ypos = baseY + (subcount/_Columns)*(20*4 + _InnerPadding);
                ParsedItemData pid = ItemRegistry.GetDataOrErrorItem($"(H){kvp.Key}");
                int spriteIndex = pid.SpriteIndex;
                Texture2D texture = pid.GetTexture();
                Rectangle sourceRect = new(spriteIndex * 20 % texture.Width,
                        spriteIndex * 20 / texture.Width * 20 * 4, 20, 20);
                bool hatWasShown = MakeHoverText(pid, out string hoverText);
                ClickableTextureComponent obj = new(
                        $"{pid.QualifiedItemId} {hatWasShown}",
                        bounds: new Rectangle(xpos, ypos, 80, 80),
                        label: null,
                        hoverText: hoverText,
                        texture: texture,
                        sourceRect: sourceRect,
                        scale: 4f,
                        drawShadow: false);
                _Pages[_Pages.Count-1].Add(obj);
                ++count;
            }
            if (playSound) {
                Game1.playSound(_MenuOpenSound);
            }
        }

        private bool MakeHoverText(ParsedItemData pid, out string hoverText)
        {
            Hat h = (Hat)ItemRegistry.Create(pid.QualifiedItemId);
            string hatString = LCHatString.HatIdCollapse(
                    LCHatString.GetItemHatString(h));
            if (!LCModData.HasShownHat(hatString)) {
                hoverText = "???^" + HML.ModHelper.Translation.Get(
                        "hatreactions.menu.NotYetShown");
                return false;
            }
            string reactionKey = hatString.Replace(" ", "")
                    .Replace("'", "").Replace("|", ".");
            NPC Lacey = Game1.getCharacterFromName(HML.LaceyInternalName);
            Dialogue d = Dialogue.TryGetDialogue(Lacey,
                    $"{LCHatString.ReactionsAsset}:{reactionKey}");
            if (d is null) {
                d = Dialogue.FromTranslation(Lacey, $"{LCHatString.ReactionsAsset}:404");
            }
            string continued = d.dialogues.Count > 1 ? " (...)" : "";
            hoverText = $"{pid.DisplayName}^{d.dialogues[0].Text}{continued}";
            return true;
        }

        public override void draw(SpriteBatch b)
        {
            this.drawFrame(b);
            base.draw(b);
            string title = HML.ModHelper.Translation.Get("hatreactions.menu.HatRegistry")
                    .ToString().Replace("@", Game1.player.Name);
            Vector2 titleSize = Game1.dialogueFont.MeasureString(title);
            Vector2 titlePos = new(xPositionOnScreen + width/2 - titleSize.X/2,
                    yPositionOnScreen + _BorderWidth + _OuterPadding);
            Utility.drawTextWithShadow(b, title, Game1.dialogueFont,
                    titlePos, Game1.textColor);
            foreach (var obj in _Pages[CurrentPage]) {
                string[] split = ArgUtility.SplitBySpace(obj.name);
                ArgUtility.TryGetOptionalBool(split, split.Length-1, out bool seen, out string err);
                Color drawColor = seen ? Color.White : Color.Black * 0.15f;
                obj.draw(b, drawColor, 0.86f);
            }
            if (CurrentPage > 0) {
                BackButton.draw(b);
            }
            if (CurrentPage < _Pages.Count - 1) {
                ForwardButton.draw(b);
            }
            if (!String.IsNullOrEmpty(_HoverText)) {
                string[] split = _HoverText.Split("^");
                string lines = Game1.parseText((split.Length > 1 ? split[1] : split[0]),
                        Game1.smallFont, 380);
                string hoverTitle = (split.Length > 1 ? split[0] : null);
                IClickableMenu.drawHoverText(b, font: Game1.smallFont,
                        text: lines, boldTitleText: hoverTitle);
            }
            base.drawMouse(b);
        }

        private void drawFrame(SpriteBatch b)
        {
            Rectangle bounds = new(xPositionOnScreen, yPositionOnScreen, width, height);
            Rectangle[] dests = nineslice(bounds, 28, 28);
            Texture2D tex = Game1.menuTexture;
            b.Draw(tex, color: Color.White,
                    sourceRectangle: _FrameBg,
                    destinationRectangle: new Rectangle(bounds.X+_BorderWidth, bounds.Y+_BorderWidth, bounds.Width-_BorderWidth*2, bounds.Height-_BorderWidth*2));
            for (int i = 0; i < _FrameNineslice.Length; ++i) {
                Rectangle r = _FrameNineslice[i];
                if (!r.Equals(Rectangle.Empty)) {
                    b.Draw(tex, color: Color.White,
                            sourceRectangle: r,
                            destinationRectangle: dests[i]);
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            _HoverText = "";
            foreach (var obj in _Pages[CurrentPage]) {
                if (obj.containsPoint(x, y)) {
                    obj.scale = Math.Min(obj.scale + 0.04f, obj.baseScale + 0.4f);
                    _HoverText = obj.hoverText;
                }
                else {
                    obj.scale = Math.Max(obj.scale - 0.04f, obj.baseScale);
                }
            }
            ForwardButton.tryHover(x, y, 0.5f);
            BackButton.tryHover(x, y, 0.5f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (CurrentPage > 0 && BackButton.containsPoint(x, y)) {
                --CurrentPage;
                if (playSound) {
                    Game1.playSound(_PageTurnSound);
                }
            }
            if (CurrentPage < _Pages.Count - 1 && ForwardButton.containsPoint(x, y)) {
                ++CurrentPage;
                if (playSound) {
                    Game1.playSound(_PageTurnSound);
                }
            }
            foreach (var obj in _Pages[CurrentPage]) {
                if (obj.containsPoint(x, y)) {
                    ReplayHatDialogue(obj, playSound);
                    return;
                }
            }
            base.receiveLeftClick(x, y, playSound);
        }

        private void ReplayHatDialogue(ClickableTextureComponent obj, bool playSound = true)
        {
            string[] split = ArgUtility.SplitBySpace(obj.name);
            Hat h = (Hat)ItemRegistry.Create(split[0]);
            string hatString = LCHatString.HatIdCollapse(
                    LCHatString.GetItemHatString(h));
            if (!LCModData.HasShownHat(hatString)) {
                return;
            }
            _HoverText = "";
            string reactionKey = hatString.Replace(" ", "")
                    .Replace("'", "").Replace("|", ".");
            NPC Lacey = Game1.getCharacterFromName(HML.LaceyInternalName);
            Dialogue d = Dialogue.TryGetDialogue(Lacey,
                    $"{LCHatString.ReactionsAsset}:{reactionKey}");
            if (d is null) {
                d = Dialogue.FromTranslation(Lacey, $"{LCHatString.ReactionsAsset}:404");
            }
            DialogueBox child = new(d);
            var parent = this;
            SetChildMenu(child);
            Game1.afterDialogues = delegate {
                parent.SetChildMenu(null);
            };
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            xPositionOnScreen = newBounds.Width/2 - _DefaultWidth/2;
            yPositionOnScreen = newBounds.Height/2 - _DefaultHeight/2;
        }

        public static Rectangle[] nineslice(Rectangle source, int cornerX, int cornerY)
        {
            int[] xval = {
                source.X,
                source.X + cornerX,
                source.X + source.Width - cornerX
            };
            int[] yval = {
                source.Y,
                source.Y + cornerY,
                source.Y + source.Height - cornerY
            };
            int[] wval = {
                cornerX,
                source.Width - 2 * cornerX,
                cornerX
            };
            int[] hval = {
                cornerY,
                source.Height - 2 * cornerY,
                cornerY
            };
            var ret = new List<Rectangle>();
            for (int c = 0; c < 3; ++c) {
                for (int r = 0; r < 3; ++r) {
                    ret.Add(new Rectangle(xval[r], yval[c], wval[r], hval[c]));
                }
            }
            return ret.ToArray();
        }
    }
}
