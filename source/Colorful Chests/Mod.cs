using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ColorfulChests
{
    public class Mod : StardewModdingAPI.Mod
    {
        private Texture2D hsl;

        private ItemGrabMenu activeMenu;
        private Chest activeChest;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            hsl = Helper.Content.Load<Texture2D>("hsl.png");
            helper.Events.Display.MenuChanged += onMenuChanged;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onMenuChanged(object sender, MenuChangedEventArgs e)
        {
            activeMenu = null;
            activeChest = null;
            Helper.Events.Display.RenderedActiveMenu -= onRenderedActiveMenu;
            Helper.Events.Input.ButtonPressed -= onButtonPressed;
            
            if ( e.NewMenu is ItemGrabMenu menu && Helper.Reflection.GetField<Item>(menu, "sourceItem").GetValue() is Chest chest )
            {
                activeMenu = menu;
                activeChest = chest;
                menu.chestColorPicker = null;
                menu.colorPickerToggleButton = null;
                Helper.Events.Display.RenderedActiveMenu += onRenderedActiveMenu;
                Helper.Events.Input.ButtonPressed += onButtonPressed;
            }
        }

        /// <summary>When a menu is open (<see cref="Game1.activeClickableMenu"/> isn't null), raised after that menu is drawn to the sprite batch but before it's rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (activeMenu == null)
                return;

            e.SpriteBatch.Draw(hsl, new Vector2((Game1.viewport.Width - hsl.Width) / 2, 32), Color.White);
            activeMenu.drawMouse(e.SpriteBatch);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onButtonPressed( object sender, ButtonPressedEventArgs e )
        {
            if (activeMenu == null)
                return;

            if ( e.Button == SButton.MouseLeft )
            {
                Vector2 screenPos = e.Cursor.ScreenPixels;
                int x = ( Game1.viewport.Width - hsl.Width ) / 2, y = 32;
                if (screenPos.X >= x && screenPos.Y >= y && screenPos.X <= x + hsl.Width && screenPos.Y <= y + hsl.Height )
                {
                    var pos = new Point((int)screenPos.X - x, (int)screenPos.Y - y);

                    var cols = new Color[hsl.Width * hsl.Height];
                    hsl.GetData<Color>(cols);
                    activeChest.playerChoiceColor.Value = cols[ pos.X + pos.Y * hsl.Width ];
                }
            }
        }
    }
}
