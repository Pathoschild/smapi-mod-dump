/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

/*
Menu settings for the mod. 
Menu code regurgitated from the game code
Settings loaded from this.Config file and changes saved onto this.Config file.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NPCMapLocations.Framework.Models;
using StardewValley;
using StardewValley.Menus;

namespace NPCMapLocations.Framework.Menus
{
    public class ModMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        private readonly ClickableTextureComponent DownArrow;
        private readonly MapModButton ImmersionButton1;
        private readonly MapModButton ImmersionButton2;
        private readonly MapModButton ImmersionButton3;
        private readonly int MapX;
        private readonly int MapY;
        private readonly ClickableTextureComponent OkButton;
        private readonly List<OptionsElement> Options = new();
        private readonly List<ClickableComponent> OptionSlots = new();
        private readonly ClickableTextureComponent ScrollBar;
        private readonly Rectangle ScrollBarRunner;
        private readonly ClickableTextureComponent UpArrow;
        private bool CanClose;
        private int CurrentItemIndex;
        private int OptionsSlotHeld = -1;
        private bool Scrolling;


        /*********
        ** Public methods
        *********/
        public ModMenu(Dictionary<string, NpcMarker> npcMarkers, Dictionary<string, bool> conditionalNpcs)
            : base(Game1.viewport.Width / 2 - (1000 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 1000 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true)
        {
            var topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(ModEntry.Map.Bounds.Width * Game1.pixelZoom, 180 * Game1.pixelZoom);
            this.MapX = (int)topLeftPositionForCenteringOnScreen.X;
            this.MapY = (int)topLeftPositionForCenteringOnScreen.Y;

            // Most of this mess is straight from the game code just... just give it space
            this.OkButton = new ClickableTextureComponent(
                "OK",
                new Rectangle(this.xPositionOnScreen + this.width - Game1.tileSize * 2, this.yPositionOnScreen + this.height - 7 * Game1.tileSize / 4, Game1.tileSize, Game1.tileSize),
                null,
                null,
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
                1f
            );

            this.UpArrow = new ClickableTextureComponent(
                new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom),
                Game1.mouseCursors,
                new Rectangle(421, 459, 11, 12),
                Game1.pixelZoom
            );

            this.DownArrow = new ClickableTextureComponent(
                new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + this.height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom),
                Game1.mouseCursors,
                new Rectangle(421, 472, 11, 12),
                Game1.pixelZoom
            );

            this.ScrollBar = new ClickableTextureComponent(
                new Rectangle(this.UpArrow.bounds.X + Game1.pixelZoom * 3, this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom),
                Game1.mouseCursors,
                new Rectangle(435, 463, 6, 10),
                Game1.pixelZoom
            );

            this.ScrollBarRunner = new Rectangle(this.ScrollBar.bounds.X, this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, this.ScrollBar.bounds.Width, this.height - Game1.tileSize * 2 - this.UpArrow.bounds.Height - Game1.pixelZoom * 2);

            for (int i = 0; i < 7; i++)
            {
                this.OptionSlots.Add(new ClickableComponent(
                    new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + i * ((this.height - Game1.tileSize * 2) / 7), this.width - Game1.tileSize / 2, (this.height - Game1.tileSize * 2) / 7 + Game1.pixelZoom), string.Concat(i)
                ));
            }

            this.Options.Add(new OptionsElement("NPC Map Locations"));

            var widths = new List<int>();
            for (int i = 0; i < 16; i++)
                widths.Add(75 + i * 15);

            var heights = new List<int>();
            for (int j = 0; j < 10; j++)
                heights.Add(45 + j * 15);

            this.Options.Add(new OptionsElement(I18n.Minimap_Label()));
            this.Options.Add(new ModCheckbox(I18n.Minimap_Enabled(), 0, -1, -1));
            this.Options.Add(new ModCheckbox(I18n.Minimap_Locked(), 5, -1, -1));
            this.Options.Add(new ModPlusMinus(I18n.Minimap_Width(), 1, widths));
            this.Options.Add(new ModPlusMinus(I18n.Minimap_Height(), 2, heights));

            // Translate labels and initialize buttons to handle button press
            this.Options.Add(new OptionsElement(I18n.Immersion_Label()));
            this.ImmersionButton1 = new MapModButton(I18n.Immersion_AlwaysShowVillagers(), 3, -1, -1, -1, -1);
            this.ImmersionButton2 = new MapModButton(I18n.Immersion_OnlyVillagersTalkedTo(), 4, -1, -1, -1, -1);
            this.ImmersionButton3 = new MapModButton(I18n.Immersion_HideVillagersTalkedTo(), 5, -1, -1, -1, -1);
            this.Options.Add(this.ImmersionButton1);
            this.Options.Add(this.ImmersionButton2);
            this.Options.Add(this.ImmersionButton3);

            this.Options.Add(new ModCheckbox(I18n.Immersion_OnlyVillagersInPlayerLocation(), 6, -1, -1));
            this.Options.Add(new ModCheckbox(I18n.Immersion_OnlyVillagersWithinHeartLevel(), 7, -1, -1));
            this.Options.Add(new MapModSlider(I18n.Immersion_MinHeartLevel(), 8, -1, -1, 0, PlayerConfig.MaxPossibleHeartLevel));
            this.Options.Add(new MapModSlider(I18n.Immersion_MaxHeartLevel(), 9, -1, -1, 0, PlayerConfig.MaxPossibleHeartLevel));

            this.Options.Add(new OptionsElement(I18n.Extra_Label()));
            this.Options.Add(new ModCheckbox(I18n.Extra_ShowQuestsOrBirthdays(), 10, -1, -1));
            this.Options.Add(new ModCheckbox(I18n.Extra_ShowHiddenVillagers(), 11, -1, -1));
            this.Options.Add(new ModCheckbox(I18n.Extra_ShowTravelingMerchant(), 12, -1, -1));

            this.Options.Add(new OptionsElement(I18n.Villagers_Label()));

            var orderedMarkers = npcMarkers
              .Where(p => p.Value.Sprite != null && p.Value.Type == CharacterType.Villager)
              .OrderBy(p => p.Value.DisplayName)
              .ToArray();

            int idx = 13;
            foreach (var npcMarker in orderedMarkers)
            {
                if (conditionalNpcs.ContainsKey(npcMarker.Key))
                {
                    if (conditionalNpcs[npcMarker.Key])
                        this.Options.Add(new ModCheckbox(npcMarker.Value.DisplayName, idx++, -1, -1, orderedMarkers));
                    else
                        idx++;
                }
                else
                {
                    this.Options.Add(new ModCheckbox(npcMarker.Value.DisplayName, idx++, -1, -1, orderedMarkers));
                }
            }
        }

        // Override snappy controls on controller
        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return true;
        }

        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose) return;

            base.leftClickHeld(x, y);
            if (this.Scrolling)
            {
                int y2 = this.ScrollBar.bounds.Y;
                this.ScrollBar.bounds.Y = Math.Min(
                    this.yPositionOnScreen + this.height - Game1.tileSize - Game1.pixelZoom * 3 - this.ScrollBar.bounds.Height,
                    Math.Max(y, this.yPositionOnScreen + this.UpArrow.bounds.Height + Game1.pixelZoom * 5)
                );
                float num = (y - this.ScrollBarRunner.Y) / (float)this.ScrollBarRunner.Height;
                this.CurrentItemIndex = Math.Min(this.Options.Count - 7, Math.Max(0, (int)(this.Options.Count * num)));
                this.SetScrollBarToCurrentIndex();
                if (y2 != this.ScrollBar.bounds.Y)
                    Game1.playSound("shiny4");
            }
            else
            {
                if (this.OptionsSlotHeld == -1 || this.OptionsSlotHeld + this.CurrentItemIndex >= this.Options.Count)
                    return;

                this.Options[this.CurrentItemIndex + this.OptionsSlotHeld].leftClickHeld(
                    x - this.OptionSlots[this.OptionsSlotHeld].bounds.X,
                    y - this.OptionSlots[this.OptionsSlotHeld].bounds.Y
                );
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if ((Game1.options.menuButton.Contains(new InputButton(key)) ||
                 Game1.options.doesInputListContain(Game1.options.mapButton, key)) && this.readyToClose() && this.CanClose)
            {
                this.exitThisMenu();
                return;
            }

            if (key.ToString().Equals(ModEntry.Globals.MenuKey) && this.readyToClose() && this.CanClose)
            {
                Game1.exitActiveMenu();
                Game1.activeClickableMenu = new GameMenu();
                (Game1.activeClickableMenu as GameMenu).changeTab(ModConstants.MapTabIndex);
                return;
            }

            this.CanClose = true;
            if (this.OptionsSlotHeld == -1 || this.OptionsSlotHeld + this.CurrentItemIndex >= this.Options.Count)
                return;

            this.Options[this.CurrentItemIndex + this.OptionsSlotHeld].receiveKeyPress(key);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (GameMenu.forcePreventClose) return;

            base.receiveScrollWheelAction(direction);
            if (direction > 0 && this.CurrentItemIndex > 0)
            {
                this.UpArrowPressed();
                Game1.playSound("shiny4");
                return;
            }

            if (direction < 0 && this.CurrentItemIndex < Math.Max(0, this.Options.Count - 7))
            {
                this.DownArrowPressed();
                Game1.playSound("shiny4");
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose) return;

            base.releaseLeftClick(x, y);
            if (this.OptionsSlotHeld != -1 && this.OptionsSlotHeld + this.CurrentItemIndex < this.Options.Count)
            {
                this.Options[this.CurrentItemIndex + this.OptionsSlotHeld].leftClickReleased(
                    x - this.OptionSlots[this.OptionsSlotHeld].bounds.X,
                    y - this.OptionSlots[this.OptionsSlotHeld].bounds.Y
                );
            }

            this.OptionsSlotHeld = -1;
            this.Scrolling = false;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose) return;

            if (this.DownArrow.containsPoint(x, y) && this.CurrentItemIndex < Math.Max(0, this.Options.Count - 7))
            {
                this.DownArrowPressed();
                Game1.playSound("shwip");
            }
            else if (this.UpArrow.containsPoint(x, y) && this.CurrentItemIndex > 0)
            {
                this.UpArrowPressed();
                Game1.playSound("shwip");
            }
            else if (this.ScrollBar.containsPoint(x, y))
            {
                this.Scrolling = true;
            }
            else if (!this.DownArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width &&
                     x < this.xPositionOnScreen + this.width + Game1.tileSize * 2 && y > this.yPositionOnScreen &&
                     y < this.yPositionOnScreen + this.height)
            {
                this.Scrolling = true;
                this.leftClickHeld(x, y);
            }
            else if (this.ImmersionButton1.Rect.Contains(x, y))
            {
                this.ImmersionButton1.receiveLeftClick(x, y);
                this.ImmersionButton2.GreyOut();
                this.ImmersionButton3.GreyOut();
            }
            else if (this.ImmersionButton2.Rect.Contains(x, y))
            {
                this.ImmersionButton2.receiveLeftClick(x, y);
                this.ImmersionButton1.GreyOut();
                this.ImmersionButton3.GreyOut();
            }
            else if (this.ImmersionButton3.Rect.Contains(x, y))
            {
                this.ImmersionButton3.receiveLeftClick(x, y);
                this.ImmersionButton1.GreyOut();
                this.ImmersionButton2.GreyOut();
            }

            if (this.OkButton.containsPoint(x, y))
            {
                this.OkButton.scale -= 0.25f;
                this.OkButton.scale = Math.Max(0.75f, this.OkButton.scale);
                (Game1.activeClickableMenu as ModMenu).exitThisMenu(false);
                Game1.activeClickableMenu = new GameMenu();
                (Game1.activeClickableMenu as GameMenu).changeTab(3);
            }

            y -= 15;
            this.CurrentItemIndex = Math.Max(0, Math.Min(this.Options.Count - 7, this.CurrentItemIndex));
            for (int i = 0; i < this.OptionSlots.Count; i++)
            {
                if (this.OptionSlots[i].bounds.Contains(x, y) && this.CurrentItemIndex + i < this.Options.Count && this.Options[this.CurrentItemIndex + i]
                        .bounds.Contains(x - this.OptionSlots[i].bounds.X, y - this.OptionSlots[i].bounds.Y))
                {
                    this.Options[this.CurrentItemIndex + i].receiveLeftClick(x - this.OptionSlots[i].bounds.X, y - this.OptionSlots[i].bounds.Y);
                    this.OptionsSlotHeld = i;
                    break;
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            if (GameMenu.forcePreventClose) return;

            if (this.OkButton.containsPoint(x, y))
            {
                this.OkButton.scale = Math.Min(this.OkButton.scale + 0.02f, this.OkButton.baseScale + 0.1f);
                return;
            }

            this.OkButton.scale = Math.Max(this.OkButton.scale - 0.02f, this.OkButton.baseScale);
            this.UpArrow.tryHover(x, y);
            this.DownArrow.tryHover(x, y);
            this.ScrollBar.tryHover(x, y);
        }

        // Draw menu
        public override void draw(SpriteBatch b)
        {
            if (Game1.options.showMenuBackground) this.drawBackground(b);

            Game1.drawDialogueBox(this.MapX - Game1.pixelZoom * 8, this.MapY - Game1.pixelZoom * 24,
              (ModEntry.Map.Bounds.Width + 16) * Game1.pixelZoom, 212 * Game1.pixelZoom, false, true);
            b.Draw(ModEntry.Map, new Vector2(this.MapX, this.MapY), new Rectangle(0, 0, 300, 180),
              Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.86f);
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            this.OkButton.draw(b);
            int buttonWidth = (int)Game1.dialogueFont.MeasureString(I18n.Immersion_HideVillagersTalkedTo()).X;
            if (!GameMenu.forcePreventClose)
            {
                this.UpArrow.draw(b);
                this.DownArrow.draw(b);

                if (this.Options.Count > 7)
                {
                    drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.ScrollBarRunner.X,
                        this.ScrollBarRunner.Y, this.ScrollBarRunner.Width, this.ScrollBarRunner.Height, Color.White,
                        Game1.pixelZoom, false);
                    this.ScrollBar.draw(b);
                }

                for (int i = 0; i < this.OptionSlots.Count; i++)
                {
                    int x = this.OptionSlots[i].bounds.X;
                    int y = this.OptionSlots[i].bounds.Y + Game1.tileSize / 4;
                    if (this.CurrentItemIndex >= 0 && this.CurrentItemIndex + i < this.Options.Count)
                    {
                        if (this.Options[this.CurrentItemIndex + i] is MapModButton)
                        {
                            var bounds = new Rectangle(x + 28, y, buttonWidth + Game1.tileSize + 8, Game1.tileSize + 8);
                            switch (this.Options[this.CurrentItemIndex + i].whichOption)
                            {
                                case 3:
                                    this.ImmersionButton1.Rect = bounds;
                                    break;
                                case 4:
                                    this.ImmersionButton2.Rect = bounds;
                                    break;
                                case 5:
                                    this.ImmersionButton3.Rect = bounds;
                                    break;
                            }

                            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), bounds.X, bounds.Y,
                                bounds.Width, bounds.Height, Color.White * (this.Options[this.CurrentItemIndex + i].greyedOut ? 0.33f : 1f),
                                1f, false);
                        }

                        if (this.CurrentItemIndex + i == 0)
                            Utility.drawTextWithShadow(b, "NPC Map Locations", Game1.dialogueFont, new Vector2(x + Game1.tileSize / 2, y + Game1.tileSize / 4), Color.Black);
                        else
                            this.Options[this.CurrentItemIndex + i].draw(b, x, y);
                    }
                }
            }

            if (!Game1.options.hardwareCursor)
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                    Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors,
                        Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero,
                    Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }


        /*********
        ** Private methods
        *********/
        private void SetScrollBarToCurrentIndex()
        {
            if (this.Options.Any())
            {
                this.ScrollBar.bounds.Y = this.ScrollBarRunner.Height / Math.Max(1, this.Options.Count - 7 + 1) * this.CurrentItemIndex + this.UpArrow.bounds.Bottom + Game1.pixelZoom;
                if (this.CurrentItemIndex == this.Options.Count - 7)
                    this.ScrollBar.bounds.Y = this.DownArrow.bounds.Y - this.ScrollBar.bounds.Height - Game1.pixelZoom;
            }
        }

        private void DownArrowPressed()
        {
            this.DownArrow.scale = this.DownArrow.baseScale;
            this.CurrentItemIndex++;
            this.SetScrollBarToCurrentIndex();
        }

        private void UpArrowPressed()
        {
            this.UpArrow.scale = this.UpArrow.baseScale;
            this.CurrentItemIndex--;
            this.SetScrollBarToCurrentIndex();
        }
    }
}
