/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SmartBuilding.UI;
using SmartBuilding.Utilities;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;

namespace SmartBuilding
{
    public class ConsoleCommand
    {
        private Logger logger;
        private bool commandRunOnce = false;
        private Texture2D texture;

        public ConsoleCommand(Logger logger, Texture2D texture)
        {
            this.logger = logger;
            this.texture = texture;
        }

        /// <summary>
        /// Both arguments are ignored.
        /// </summary>
        /// <param name="command">Ignored.</param>
        /// <param name="args">Ignored.</param>
        public void BindingUI(string command, string[] args)
        {
            Rectangle viewport = Game1.uiViewport;
            BindingUi binding = new BindingUi(viewport.X, viewport.Y, viewport.Width, viewport.Height, texture, true);

            Game1.activeClickableMenu = binding;
        }

        public void DebugCommand(string command, string[] args)
        {
            if (!commandRunOnce)
            {
                logger.Log("THIS IS YOUR ONE AND ONLY WARNING. This command is purely for testing, may wipe your player inventory, the farm, " +
                            "spawn buildings on the farm, and perform other acts of chaos. After this warning, you will be able to use " +
                            "the command.", LogLevel.Alert);
                commandRunOnce = true;

                return;
            }

            string subCommand = "";

            if (args.Length <= 0)
            {
                logger.Log("You forgot an argument.");

                return;
            }

            Farmer player = Game1.player;

            // We only care about the first argument, and can ignore the rest.
            subCommand = args[0];

            // Wipe the player's inventory with the vanilla "debug clear" command.
            Game1.game1.parseDebugInput("clear");

            // Our item list
            Item[] items;

            // And figure out which subcommand we want.
            switch (subCommand)
            {
                case "regression":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Auto-Grabber")
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "identification":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Small FishTank"),
                        Utility.fuzzyItemSearch("Dresser"),
                        Utility.fuzzyItemSearch("Bed"),
                        Utility.fuzzyItemSearch("TV"),
                        Utility.fuzzyItemSearch("Oak Table"),
                        Utility.fuzzyItemSearch("Weathered Floor", 10),
                        Utility.fuzzyItemSearch("Chest", 10),
                        Utility.fuzzyItemSearch("Stone Chest", 10),
                        Utility.fuzzyItemSearch("Junimo Chest", 10),
                        Utility.fuzzyItemSearch("Hardwood Fence", 10),
                        Utility.fuzzyItemSearch("Iron Fence", 10),
                        Utility.fuzzyItemSearch("Grass Starter", 10),
                        Utility.fuzzyItemSearch("Crab Pot"),
                        Utility.fuzzyItemSearch("Parsnip Seed"),
                        Utility.fuzzyItemSearch("Eerie Seed"),
                        Utility.fuzzyItemSearch("Apple Sapling"),
                        Utility.fuzzyItemSearch("Acorn"),
                        Utility.fuzzyItemSearch("Tree Fertilizer")
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "flooring":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Weathered Floor", 100),
                        Utility.fuzzyItemSearch("Straw Floor", 100),
                        Utility.fuzzyItemSearch("Brick Floor", 100),
                        Utility.fuzzyItemSearch("Stone Floor", 100),
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "furniture":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Dresser"),
                        Utility.fuzzyItemSearch("Oak Table"),
                        Utility.fuzzyItemSearch("Chair")
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "tv":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Floor TV"),
                        Utility.fuzzyItemSearch("Budget TV"),
                        Utility.fuzzyItemSearch("Plasma TV"),
                        Utility.fuzzyItemSearch("Tropical TV"),
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "storage":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Birch Dresser"),
                        Utility.fuzzyItemSearch("Oak Dresser"),
                        Utility.fuzzyItemSearch("Walnut Dresser"),
                        Utility.fuzzyItemSearch("Mahogany Dresser"),
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "bed":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Single Bed"),
                        Utility.fuzzyItemSearch("Double Bed"),
                        Utility.fuzzyItemSearch("Fisher Double Bed"),
                        Utility.fuzzyItemSearch("Wild Double Bed"),
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "chests":
                case "chest":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Chest"),
                        Utility.fuzzyItemSearch("Stone Chest"),
                        Utility.fuzzyItemSearch("Junimo Chest")
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "tree":
                case "trees":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Apple Sapling", 10),
                        Utility.fuzzyItemSearch("Apricot Sapling", 10),
                        Utility.fuzzyItemSearch("Banana Sapling", 10),
                        Utility.fuzzyItemSearch("Cherry Sapling", 10),
                        Utility.fuzzyItemSearch("Maple Seed", 10),
                        Utility.fuzzyItemSearch("Acorn", 10),
                        Utility.fuzzyItemSearch("Pine Cone", 10),
                        Utility.fuzzyItemSearch("Mahogany Seed", 10),
                        Utility.fuzzyItemSearch("Tapper", 10),
                        Utility.fuzzyItemSearch("Heavy Tapper", 10),
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "crop":
                case "crops":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Eerie Seed", 100),
                        Utility.fuzzyItemSearch("Parsnip Seed", 100),
                        Utility.fuzzyItemSearch("Bison Seed", 100),
                        Utility.fuzzyItemSearch("Cactus Flower Seed", 100),
                        Utility.fuzzyItemSearch("Cabbage Seed", 100),
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "fence":
                case "fences":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Wood Fence", 100),
                        Utility.fuzzyItemSearch("Stone Fence", 100),
                        Utility.fuzzyItemSearch("Iron Fence", 100),
                        Utility.fuzzyItemSearch("Hardwood Fence", 100)
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "modded":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Cream Soda Maker", 100),
                        Utility.fuzzyItemSearch("Artisanal Soda Maker", 100),
                        Utility.fuzzyItemSearch("Alembic", 100),
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                case "insertion":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Furnace", 100),
                        Utility.fuzzyItemSearch("Copper Ore", 100),
                        Utility.fuzzyItemSearch("Coal", 999),
                        
                        Utility.fuzzyItemSearch("Keg", 100),
                        Utility.fuzzyItemSearch("Hops", 100),

                        Utility.fuzzyItemSearch("Cream Soda Maker", 100),
                        Utility.fuzzyItemSearch("Artisanal Soda Maker", 100),
                        Utility.fuzzyItemSearch("Carbonator", 100),
                        
                        Utility.fuzzyItemSearch("Alembic", 100),
                        Utility.fuzzyItemSearch("Vanilla", 100),

                        Utility.fuzzyItemSearch("Strawberry", 100),
                        Utility.fuzzyItemSearch("Sugar", 100),
                        Utility.fuzzyItemSearch("Fresh Water", 100),
                        Utility.fuzzyItemSearch("Sparkling Water", 100),
                    };

                    foreach (Item item in items)
                    {
                        if (item != null)
                            player.addItemToInventory(item);
                    }

                    break;
                default:
                    break;

            }
        }
    }
}