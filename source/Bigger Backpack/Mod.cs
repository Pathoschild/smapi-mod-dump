using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

namespace BiggerBackpack
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;

        private Texture2D bigBackpack;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            bigBackpack = Helper.Content.Load<Texture2D>("backpack.png");

            MenuEvents.MenuClosed += menuClosed;
            MenuEvents.MenuChanged += menuChanged;
            GraphicsEvents.OnPreRenderHudEvent += draw;
            InputEvents.ButtonPressed += inputPressed;

            Helper.ConsoleCommands.Add("player_setbackpacksize", "Set the size of the player's backpack.", command);
        }

        private void command( string cmd, string[] args )
        {
            if (args.Length != 1)
            {
                Log.info("Must have one command argument");
                return;
            }

            Game1.player.MaxItems = int.Parse(args[0]);
        }

        private void draw(object sender, EventArgs args)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.currentLocation.Name == "SeedShop" && Game1.player.MaxItems == 36)
            {
                Game1.spriteBatch.Draw(bigBackpack, Game1.GlobalToLocal(new Vector2((float)(7 * Game1.tileSize + Game1.pixelZoom * 2), (float)(17 * Game1.tileSize))), new Rectangle?(new Rectangle(0, 0, 12, 14)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)(19.25 * (double)Game1.tileSize / 10000.0));
            }
        }

        private void inputPressed(object sender, EventArgsInput input)
        {
            if (!Context.IsWorldReady)
                return;

            if (input.IsActionButton && !input.IsSuppressed)
            {
                if (Game1.player.MaxItems == 36 && Game1.currentLocation.Name == "SeedShop" && input.Cursor.Tile.X == 7 && (input.Cursor.Tile.Y == 17 || input.Cursor.Tile.Y == 18) )
                {
                    input.SuppressButton();
                    Response yes = new Response("Purchase", "Purchase (50,000g)");
                    Response no = new Response("Not", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_ResponseNo"));
                    Response[] resps = new Response[] { yes, no };
                    Game1.currentLocation.createQuestionDialogue("Backpack Upgrade -- 48 slots", resps, "spacechase0.BiggerBackpack");
                }
            }
        }

        private void menuClosed(object sender, EventArgsClickableMenuClosed args)
        {
            if (!Context.IsWorldReady)
                return;
            
            if ( args.PriorMenu is DialogueBox db )
            {
                if (Game1.currentLocation.lastQuestionKey == "spacechase0.BiggerBackpack" && prevSelResponse == 0)
                {
                    if (Game1.player.Money >= 50000)
                    {
                        Game1.player.Money -= 50000;
                        Game1.player.MaxItems += 12;
                        for (int index = 0; index < Game1.player.MaxItems; ++index)
                        {
                            if (Game1.player.Items.Count <= index)
                                Game1.player.Items.Add((Item)null);
                        }
                        Game1.player.holdUpItemThenMessage((Item)new SpecialItem(99, "Premium Pack"), true);
                    }
                    else
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney2"));
                }

                GameEvents.UpdateTick -= watchSelectedResponse;
                prevSelResponse = -1;
            }
        }

        private void menuChanged(object sender, EventArgsClickableMenuChanged args)
        {
            if (args.NewMenu == null)
                return;

            if (args.NewMenu is GameMenu gameMenu)
            {
                var pages = (List<IClickableMenu>)Helper.Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue();
                var oldInv = pages[GameMenu.inventoryTab];
                if (oldInv.GetType() == typeof(InventoryPage))
                {
                    pages[GameMenu.inventoryTab] = new NewInventoryPage(oldInv.xPositionOnScreen, oldInv.yPositionOnScreen, oldInv.width, oldInv.height);
                }
            }
            else if (args.NewMenu is MenuWithInventory menuWithInv)
            {
                menuWithInv.inventory.capacity = 48;
                menuWithInv.inventory.rows = 4;
                menuWithInv.height += 64;
            }
            else if ( args.NewMenu is ShopMenu shop )
            {
                shop.inventory = new InventoryMenu(shop.inventory.xPositionOnScreen, shop.inventory.yPositionOnScreen, false, (List<Item>)null, new InventoryMenu.highlightThisItem(shop.highlightItemToSell), 48, 4, 0, 0, true);
            }
            else if ( args.NewMenu is DialogueBox )
            {
                GameEvents.UpdateTick += watchSelectedResponse;
            }
        }

        int prevSelResponse = -1;
        private void watchSelectedResponse(object sender, EventArgs args)
        {
            if (Game1.activeClickableMenu is DialogueBox db)
            {
                int sel = Helper.Reflection.GetField<int>(db, "selectedResponse").GetValue();
                if (sel != -1)
                    prevSelResponse = sel;
            }
        }
    }
}
