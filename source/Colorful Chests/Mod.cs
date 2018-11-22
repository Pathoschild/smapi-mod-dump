using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ColorfulChests
{
    public class Mod : StardewModdingAPI.Mod
    {
        private Texture2D hsl;

        private IClickableMenu activeMenu;
        private Chest activeChest;

        public override void Entry(IModHelper helper)
        {
            hsl = Helper.Content.Load<Texture2D>("hsl.png");
            MenuEvents.MenuChanged += onMenuChanged;
        }

        private void onMenuChanged(object sender, EventArgsClickableMenuChanged args)
        {
            activeMenu = null;
            activeChest = null;
            GraphicsEvents.OnPostRenderGuiEvent -= onPostRenderGui;
            ControlEvents.MouseChanged -= onMouseChanged;
            
            if ( args.NewMenu is ItemGrabMenu menu && Helper.Reflection.GetField<Item>(menu, "sourceItem").GetValue() is Chest chest )
            {
                activeMenu = menu;
                activeChest = chest;
                menu.chestColorPicker = null;
                menu.colorPickerToggleButton = null;
                GraphicsEvents.OnPostRenderGuiEvent += onPostRenderGui;
                ControlEvents.MouseChanged += onMouseChanged;
            }
        }

        private void onPostRenderGui(object sender, EventArgs args)
        {
            if (activeMenu == null)
                return;

            SpriteBatch sb = Game1.spriteBatch;
            //sb.Begin();
            sb.Draw(hsl, new Microsoft.Xna.Framework.Vector2((Game1.viewport.Width - hsl.Width) / 2, 32), Color.White);
            activeMenu.drawMouse(sb);
            //sb.End();
        }

        private void onMouseChanged( object sender, EventArgsMouseStateChanged args)
        {
            if ( args.NewState.LeftButton == ButtonState.Pressed && args.PriorState.LeftButton == ButtonState.Released )
            {
                int x = ( Game1.viewport.Width - hsl.Width ) / 2, y = 32;
                if ( args.NewPosition.X >= x && args.NewPosition.Y >= y && 
                     args.NewPosition.X <= x + hsl.Width && args.NewPosition.Y <= y + hsl.Height )
                {
                    var pos = new Point(args.NewPosition.X - x, args.NewPosition.Y - y);

                    var cols = new Color[hsl.Width * hsl.Height];
                    hsl.GetData<Color>(cols);
                    activeChest.playerChoiceColor.Value = cols[ pos.X + pos.Y * hsl.Width ];
                }
            }
        }
    }
}
