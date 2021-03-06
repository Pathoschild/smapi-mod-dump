/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MultipleMiniObelisks
**
*************************************************/

// StardewValley.Menus.NamingMenu
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MultipleMiniObelisks.Multiplayer;
using MultipleMiniObelisks.Objects;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MultipleMiniObelisks.UI
{
    public class RenameMenu : IClickableMenu
    {
        public const int region_okButton = 101;

        public const int region_doneNamingButton = 102;

        public const int region_namingBox = 104;

        public ClickableTextureComponent doneNamingButton;

        protected TextBox textBox;

        public ClickableComponent textBoxCC;

        private TextBoxEvent e;

        private string title;

        protected int minLength = 1;

        private MiniObelisk obelisk;

        private TeleportMenu parentMenu;

        public RenameMenu(TeleportMenu parentMenu, string title, MiniObelisk obelisk)
        {
            this.obelisk = obelisk;
            this.parentMenu = parentMenu;

            base.xPositionOnScreen = 0;
            base.yPositionOnScreen = 0;
            base.width = Game1.uiViewport.Width;
            base.height = Game1.uiViewport.Height;
            this.title = title;

            this.textBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor);
            this.textBox.X = Game1.uiViewport.Width / 2 - ((Game1.uiViewport.Width / 6) / 2);
            this.textBox.Y = Game1.uiViewport.Height / 2;
            this.textBox.Width = Game1.uiViewport.Width / 6;
            this.textBox.Height = 192;
            this.e = textBoxEnter;
            this.textBox.OnEnterPressed += this.e;
            Game1.keyboardDispatcher.Subscriber = this.textBox;
            this.textBox.Text = obelisk.CustomName;
            this.textBox.Selected = true;

            this.doneNamingButton = new ClickableTextureComponent(new Rectangle(this.textBox.X + this.textBox.Width + 32 + 4, Game1.uiViewport.Height / 2 - 8, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 102,
                rightNeighborID = 103,
                leftNeighborID = 104
            };
            this.textBoxCC = new ClickableComponent(new Rectangle(this.textBox.X, this.textBox.Y, textBox.Width, textBox.Height), "")
            {
                myID = 104,
                rightNeighborID = 102
            };
            if (Game1.options.SnappyMenus)
            {
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            base.currentlySnappedComponent = base.getComponentWithID(104);
            this.snapCursorToCurrentSnappedComponent();
        }

        public void textBoxEnter(TextBox sender)
        {
            if (sender.Text.Length >= this.minLength)
            {
                if (this.textBox.Selected == true)
                {
                    this.textBox.Selected = false;
                }
                else
                {
                    this.obelisk.CustomName = this.textBox.Text;

                    if (Context.IsMainPlayer)
                    {
                        ModEntry.UpdateObeliskCustomName(this.obelisk);
                    }
                    else
                    {
                        // Notify the MasterPlayer of name change
                        var updateMessage = new ObeliskUpdateMessage(this.obelisk);
                        ModEntry.helper.Multiplayer.SendMessage(updateMessage, nameof(ObeliskUpdateMessage), modIDs: new[] { ModEntry.manifest.UniqueID });
                    }
                    this.exitThisMenu();

                    // Paginate the obelisks to order them with the new name
                    this.parentMenu.PaginateObelisks();
                    Game1.activeClickableMenu = this.parentMenu;
                }
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (this.textBox.Selected)
            {
                switch (b)
                {
                    case Buttons.DPadUp:
                    case Buttons.DPadDown:
                    case Buttons.DPadLeft:
                    case Buttons.DPadRight:
                    case Buttons.LeftThumbstickLeft:
                    case Buttons.LeftThumbstickUp:
                    case Buttons.LeftThumbstickDown:
                    case Buttons.LeftThumbstickRight:
                        this.textBox.Selected = false;
                        break;
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!this.textBox.Selected && !Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                base.receiveKeyPress(key);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (this.doneNamingButton != null)
            {
                if (this.doneNamingButton.containsPoint(x, y))
                {
                    this.doneNamingButton.scale = Math.Min(1.1f, this.doneNamingButton.scale + 0.05f);
                }
                else
                {
                    this.doneNamingButton.scale = Math.Max(1f, this.doneNamingButton.scale - 0.05f);
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            this.textBox.Update();

            if (this.doneNamingButton.containsPoint(x, y))
            {
                this.textBoxEnter(this.textBox);
                Game1.playSound("smallSelect");
            }
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, this.title, Game1.uiViewport.Width / 2, Game1.uiViewport.Height / 2 - 128, this.title);
            this.textBox.Draw(b);
            this.doneNamingButton.draw(b);
            base.drawMouse(b);
        }
    }
}