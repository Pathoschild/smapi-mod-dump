using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using static StardewValley.Menus.LoadGameMenu;

namespace ServerBookmarker
{
    class Manager
    {
        private readonly IReflectionHelper reflection;

        private readonly BookmarksDataModel bookmarks;

        private ClickableComponent addServerButton = null;

        private readonly Action<BookmarksDataModel, string> writeData;

        public Manager(IReflectionHelper reflection, Func<string, BookmarksDataModel> readData, Action<BookmarksDataModel, string> writeData)
        {
            this.reflection = reflection;
            this.writeData = writeData;

            bookmarks = readData("Bookmarks.json");
            if (bookmarks == null)
            {
                bookmarks = new BookmarksDataModel();
                writeData(bookmarks, "Bookmarks.json");
            }

            StartListPopulationListener.StartPopulation += FillUIElements;

            CoopMenuDrawExtraListener.DrawExtra += DrawExtra;
            CoopMenuLeftClickListener.LeftClick += LeftClick;
            CoopMenuPerformHoverListener.PerformHover += PerformHover;

            ServerMenuSlot.DeleteBookmark += DeleteBookmark;
        }
        
        public void FillUIElements(CoopMenu menu)
        {
            //Add server buttons
            string text = "Add server";
            int width = (int)Game1.dialogueFont.MeasureString(text).X + 64;
            Vector2 pos = new Vector2(menu.backButton.bounds.Right - width, menu.backButton.bounds.Y - 128 - 150);
            addServerButton = new ClickableComponent(new Rectangle((int)pos.X, (int)pos.Y, width, 96), "", text);
            
            //Servers
            var menuSlotsField = reflection.GetField<List<MenuSlot>>(menu, "menuSlots");//menuSlots for Join, hostSlots for Host
            var list = menuSlotsField.GetValue();
            list.RemoveAll((x) => x is ServerMenuSlot);//So there are no duplicates
            
            foreach (var x in bookmarks.Bookmarks)
            {
                list.Add(new ServerMenuSlot(menu, reflection, x.Key, x.Value));
            }            
        }

        private void DeleteBookmark(CoopMenu menu, ServerMenuSlot slot)
        {
            var menuSlotsField = reflection.GetField<List<MenuSlot>>(menu, "menuSlots");//menuSlots for Join, hostSlots for Host
            var list = menuSlotsField.GetValue();
            if (list != null)
            {
                Console.WriteLine($"Deleting bookmark {slot.ServerName}, ip {slot.IP}");
                list.Remove(slot);

                bookmarks.Bookmarks.Remove(slot.ServerName);
                writeData(bookmarks, "Bookmarks.json");
            }
        }

        private void LeftClick(CoopMenu menu, int x, int y)
        {
            if (addServerButton != null && addServerButton.containsPoint(x, y))
            {
                Game1.playSound("bigDeSelect");
                
                Game1.activeClickableMenu = new TextMenu("Server name?", (serverName) =>
                {
                    Game1.activeClickableMenu = new TextMenu("IP address?", (ipAddress) =>
                    {
                        bookmarks.Bookmarks.Remove(serverName);
                        bookmarks.Bookmarks.Add(serverName, ipAddress);
                        writeData(bookmarks, "Bookmarks.json");

                        var title = new TitleMenu();
                        title.skipToTitleButtons();
                        TitleMenu.subMenu = new CoopMenu();
                        Game1.activeClickableMenu = title;
                    });
                });
            }
        }

        private void DrawExtra(CoopMenu menu, SpriteBatch spriteBatch)
        {
            if (addServerButton != null)
            {
                IClickableMenu.drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), addServerButton.bounds.X, addServerButton.bounds.Y, addServerButton.bounds.Width, addServerButton.bounds.Height, (addServerButton.scale > 0f) ? Color.Wheat : Color.White, 4f, true);
                Utility.drawTextWithShadow(spriteBatch, addServerButton.label, Game1.dialogueFont, new Vector2(addServerButton.bounds.Center.X, addServerButton.bounds.Center.Y + 4) - Game1.dialogueFont.MeasureString(addServerButton.label) / 2f, Game1.textColor, 1f, -1f, -1, -1, 0f, 3);
            }
        }

        private void PerformHover(CoopMenu menu, int x, int y)
        {
            if (addServerButton != null)
            {
                addServerButton.scale = addServerButton.visible && addServerButton.containsPoint(x, y) ? 1f : 0f;
            }
        }
    }
}
