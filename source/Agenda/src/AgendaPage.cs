/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/StardewValley-Agenda
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using StardewValley;
using Microsoft.Xna.Framework.Input;

namespace MyAgenda
{
    public class AgendaPage : IClickableMenu
    {
        public string festival, birthday, title, subsituteTitle, note, note_back, title_back;
        public int season, day, selected, ticks = 0;
        public static Texture2D pageTexture;
        public static Rectangle[] bounds = new Rectangle[4];
        public static IMonitor monitor;
        public static IModHelper helper;
        public static TextBox tbox;

        public AgendaPage(IModHelper helper) : base()
        {
            Game1.mouseCursorTransparency = 1f;
            if (Game1.gameMode == 3 && Game1.player != null && !Game1.eventUp)
            {
                Game1.player.Halt();
            }
            if (Game1.player != null && !Game1.player.UsingTool && !Game1.eventUp)
            {
                Game1.player.forceCanMove();
            }

            pageTexture = helper.ModContent.Load<Texture2D>("assets\\page");
            tbox = new TextBox(null, null, Game1.dialogueFont, Color.Black);
            tbox.Text = "0";
            tbox.X = 100000000;
            tbox.Y = 100000000;
            tbox.Width = 114514;
            tbox.Height = 114514;
            tbox.OnEnterPressed += textBoxEnter;
            tbox.OnBackspacePressed += textBoxback;
            resize();
            AgendaPage.helper = helper;
        }

        public void textBoxback(TextBox sender)
        {
            if (selected == 1)
            {
                //monitor.Log("title", LogLevel.Info);
                if (title.Length > 0) title = title.Substring(0, title.Length - 1);
            }
            else if (selected == 2)
            {
                //monitor.Log("note", LogLevel.Info);
                if (note.Length > 0) note = note.Substring(0, note.Length - 1);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (bounds[0].Contains(x, y))
            {
                if(selected == 0)
                {
                    tbox.SelectMe();
                    tbox.Text = "0";
                }
                if(selected == 2)
                {
                    note += note_back;
                    note_back = "";
                }
                selected = 1;
                return;
            }
            if (bounds[3].Contains(x, y))
            {
                if (selected == 0)
                {
                    tbox.SelectMe();
                    tbox.Text = "0";
                }
                if(selected == 1)
                {
                    title += title_back;
                    title_back = "";
                }
                selected = 2;
                return;
            }
            if(selected != 0)
            {
                selected = 0;
                tbox.Selected = false;
            }
            exitThisMenu();
            Game1.activeClickableMenu = Agenda.Instance;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            resize();
        }

        public void resize()
        {
            width = 200 * 4;
            height = 238 * 4;
            Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            xPositionOnScreen = (int)topLeftPositionForCenteringOnScreen.X;
            yPositionOnScreen = (int)topLeftPositionForCenteringOnScreen.Y;
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
            bounds[0] = new Rectangle(xPositionOnScreen + 52 * 4, yPositionOnScreen + 20 * 4, 130 * 4, 80);
            bounds[1] = new Rectangle(xPositionOnScreen + 52 * 4, yPositionOnScreen + 35 * 4, 130 * 4, 80);
            bounds[2] = new Rectangle(xPositionOnScreen + 52 * 4, yPositionOnScreen + 48 * 4, 130 * 4, 80);
            bounds[3] = new Rectangle(xPositionOnScreen + 52 * 4, yPositionOnScreen + 70 * 4, 130 * 4, 160 * 4);
        }

        public override void draw(SpriteBatch b)
        {
            ticks++;
            ticks %= 60;
            if(selected == 1)
            {
                title += tbox.Text.Substring(1);
            }
            else if(selected == 2)
            {
                note += tbox.Text.Substring(1);
            }

            if(selected != 0)
            {
                tbox.Text = "0";
            }

            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            b.Draw(pageTexture, new Vector2(xPositionOnScreen, yPositionOnScreen), new Rectangle(0, 0, 200, 238), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            Util.drawStr(b, getSuitableTitle(), bounds[0], Game1.dialogueFont);

            Util.drawStr(b, helper.Translation.Get("festival") + (festival == "" ? helper.Translation.Get("none") : festival), bounds[1], Game1.dialogueFont);
            Util.drawStr(b, helper.Translation.Get("birthday_page") + (birthday == "" ? helper.Translation.Get("none") : birthday), bounds[2], Game1.dialogueFont);

            /*
            if(selected == 2 && ticks >= 30)
            {
                Util.drawStr(b, note + "|" + note_back, bounds[3], Game1.smallFont);
            }
            else
            {
                Util.drawStr(b, note + " " + note_back, bounds[3], Game1.smallFont);
            }*/
            Util.drawStr(b, getSuitableNote(), bounds[3], Game1.smallFont);
            
            base.draw(b);
            tbox.Draw(b);
            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (selected == 1)
            {
                if (key == Keys.Left && title.Length > 0)
                {
                    title_back = title[title.Length - 1] + title_back;
                    title = title.Substring(0, title.Length - 1);
                }
                else if (key == Keys.Right && title_back.Length > 0)
                {
                    title += title_back[0];
                    title_back = title_back.Substring(1);
                }
            }
            else if (selected == 2)
            {
                if (key == Keys.Left && note.Length > 0)
                {
                    note_back = note[note.Length - 1] + note_back;
                    note = note.Substring(0, note.Length - 1);
                }
                else if (key == Keys.Right && note_back.Length > 0)
                {
                    note += note_back[0];
                    note_back = note_back.Substring(1);
                }
            }
            if (!tbox.Selected && !Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                base.receiveKeyPress(key);
            }
        }
        public string getSuitableTitle()
        {
            if(selected == 1)
            {
                return title + (ticks > 30 ? " " : "|") + title_back;
            }

            if(title != "")
            {
                return title + title_back;
            }

            if(subsituteTitle != "")
            {
                return subsituteTitle;
            }

            return helper.Translation.Get("subsitute");
        }

        public string getSuitableNote()
        {
            if (selected == 2)
            {
                return note + (ticks > 30 ? " " : "|") + note_back;
            }

            if (note != "")
            {
                return note + note_back;
            }

            return helper.Translation.Get("subsitute_note");
        }

        public void textBoxEnter(TextBox sender)
        {
            sender.Text += "\n";
        }
    }
}