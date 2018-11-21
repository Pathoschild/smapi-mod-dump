using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using static StardewValley.Menus.LoadGameMenu;

namespace ServerBookmarker
{
    public class ServerMenuSlot : MenuSlot
    {
        public static event Action<CoopMenu, ServerMenuSlot> DeleteBookmark;

        public string ServerName { get; private set; }
        public string IP { get; private set; }

        private new readonly CoopMenu menu; 
        readonly IReflectionHelper reflection;

        readonly ClickableTextureComponent deleteButton;

        public ServerMenuSlot(CoopMenu menu, IReflectionHelper reflection, string serverName, string ip) : base (menu)
        {
            this.ServerName = serverName;
            this.IP = ip;
            this.reflection = reflection;
            this.menu = menu;

            var menuSlotsField = reflection.GetField<List<MenuSlot>>(menu, "menuSlots");//menuSlots for Join, hostSlots for Host
            var list = menuSlotsField.GetValue();

            deleteButton = new ClickableTextureComponent("",
                new Rectangle(0,0, 48, 48), "", "Delete", Game1.mouseCursors, new Rectangle(322, 498, 12, 12), 3f, false);

            CoopMenuPerformLeftClickPreListenerAndOverrider.PerformLeftClick += (x) =>
            {
                if (deleteButton.bounds.Contains(Game1.getMousePosition()) && 
                    reflection.GetField<CoopMenu.Tab>(menu, "currentTab").GetValue() == CoopMenu.Tab.JOIN_TAB &&
                    reflection.GetField<List<MenuSlot>>(menu, "menuSlots").GetValue() != null)
                {
                    DeleteBookmark?.Invoke(menu, this);

                    x.Override = true;
                }
            };
        }

        public override void Activate()
        {
            Console.WriteLine($"Clicked bookmark: {ServerName} at {IP}");
            CoopMenu menu = TitleMenu.subMenu as CoopMenu;

            //IP in format of 127.0.0.1:1234
            var multiplayer = reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer");
            var setMenuMethod = reflection.GetMethod(menu, "setMenu", true);

            setMenuMethod.Invoke(new FarmhandMenu(multiplayer.GetValue().InitClient(new LidgrenClient(IP))));
        }
        

        public override void Draw(SpriteBatch b, int i)
        {
            //Centre text
            int strWidth = SpriteText.getWidthOfString(ServerName);
            int strHeight = SpriteText.getHeightOfString(ServerName, 999999);
            Rectangle bounds = base.menu.slotButtons[i].bounds;
            int x = bounds.X + (bounds.Width - strWidth) / 2;
            int y = bounds.Y + (bounds.Height - strHeight) / 2;
            SpriteText.drawString(b, ServerName, x, y, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);

            //IP
            int widthOfSomething = 100;
            Vector2 position = new Vector2(menu.slotButtons[i].bounds.X + menu.width - 192 - 60 - widthOfSomething, 
                menu.slotButtons[i].bounds.Y + 64 + 44);
            Utility.drawTextWithShadow(b, IP, Game1.dialogueFont, position, Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            
            //Delete button
            deleteButton.bounds = new Rectangle(menu.slotButtons[i].bounds.X + menu.width - 192+192 - 60+55 - widthOfSomething, menu.slotButtons[i].bounds.Y + 64 + 44 -44 - 32, 
                deleteButton.bounds.Width, deleteButton.bounds.Height);

            deleteButton.draw(b);
        }
    }
}
