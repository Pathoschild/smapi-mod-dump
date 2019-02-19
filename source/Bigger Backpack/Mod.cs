using System;
using System.Collections.Generic;
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

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;
            bigBackpack = Helper.Content.Load<Texture2D>("backpack.png");

            helper.Events.Display.MenuChanged += onMenuChanged;
            helper.Events.Display.RenderingHud += onRenderingHud;
            helper.Events.Input.ButtonPressed += onButtonPressed;

            Helper.ConsoleCommands.Add("player_setbackpacksize", "Set the size of the player's backpack.", command);
        }

        private void command( string cmd, string[] args )
        {
            if (args.Length != 1)
            {
                Log.info("Must have one command argument");
                return;
            }

            int newMax = int.Parse(args[0]);
            if (newMax < Game1.player.MaxItems)
            {
                for (int i = Game1.player.MaxItems - 1; i >= newMax; --i)
                    Game1.player.Items.RemoveAt(i);
            }
            else
            {
                for (int i = Game1.player.Items.Count; i < Game1.player.MaxItems; ++i)
                    Game1.player.Items.Add(null);
            }
            Game1.player.MaxItems = int.Parse(args[0]);
        }

        /// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.currentLocation.Name == "SeedShop" && Game1.player.MaxItems == 36)
            {
                e.SpriteBatch.Draw(bigBackpack, Game1.GlobalToLocal(new Vector2(7 * Game1.tileSize + Game1.pixelZoom * 2, 17 * Game1.tileSize)), new Rectangle(0, 0, 12, 14), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, (float)(19.25 * Game1.tileSize / 10000.0));
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button.IsActionButton() && !this.Helper.Input.IsSuppressed(e.Button))
            {
                if (Game1.player.MaxItems == 36 && Game1.currentLocation.Name == "SeedShop" && e.Cursor.Tile.X == 7 && (e.Cursor.Tile.Y == 17 || e.Cursor.Tile.Y == 18) )
                {
                    this.Helper.Input.Suppress(e.Button);
                    Response yes = new Response("Purchase", "Purchase (50,000g)");
                    Response no = new Response("Not", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_ResponseNo"));
                    Response[] resps = new Response[] { yes, no };
                    Game1.currentLocation.createQuestionDialogue("Backpack Upgrade -- 48 slots", resps, "spacechase0.BiggerBackpack");
                }
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // on closed
            if (Context.IsWorldReady && e.OldMenu is DialogueBox)
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

                Helper.Events.GameLoop.UpdateTicked -= watchSelectedResponse;
                prevSelResponse = -1;
            }

            // on new menu
            switch (e.NewMenu)
            {
                case GameMenu gameMenu:
                {
                    var pages = (List<IClickableMenu>)Helper.Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue();
                    var oldInv = pages[GameMenu.inventoryTab];
                    if (oldInv.GetType() == typeof(InventoryPage))
                    {
                        pages[GameMenu.inventoryTab] = new NewInventoryPage(oldInv.xPositionOnScreen, oldInv.yPositionOnScreen, oldInv.width, oldInv.height);
                    }

                    break;
                }

                case MenuWithInventory menuWithInv:
                    menuWithInv.inventory.capacity = 48;
                    menuWithInv.inventory.rows = 4;
                    menuWithInv.height += 64;
                    break;

                case ShopMenu shop:
                    shop.inventory = new InventoryMenu(shop.inventory.xPositionOnScreen, shop.inventory.yPositionOnScreen, false, (List<Item>)null, new InventoryMenu.highlightThisItem(shop.highlightItemToSell), 48, 4, 0, 0, true);
                    break;

                case DialogueBox _:
                    Helper.Events.GameLoop.UpdateTicked += watchSelectedResponse;
                    break;
            }
        }

        int prevSelResponse = -1;

        /// <summary>Raised after the game state is updated (≈60 times per second), while waiting for a dialogue response.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void watchSelectedResponse(object sender, UpdateTickedEventArgs e)
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
