/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Menus;
using MaddUtil;
using StardewValley.Locations;
using SObject = StardewValley.Object;
using ChestPreview.Framework;
using ChestPreview.Framework.APIs;

namespace ChestPreview
{
    public class ModEntry : Mod
    {
        public static IModHelper helper;
        public static ModConfig config;
        public static Size CurrentSize { get; set; }
        public static IDynamicGameAssetsApi DGAAPI { get; set; }

        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;
            Printer.SetMonitor(this.Monitor);
            Helpers.SetModHelper(helper);

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.Display.WindowResized += this.OnWindowResized;
        }


        private void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            Printer.Trace($"Old screen size: {e.OldSize}");
            Printer.Trace($"New screen size: {e.NewSize}");
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            config = null;
            try
            {
                config = ModEntry.helper.ReadConfig<ModConfig>();
                if (config == null)
                {
                    Printer.Error($"The config file seems to be empty or invalid.");
                }
            }
            catch (Exception ex)
            {
                Printer.Error($"The config file seems to be missing or invalid.\n{ex}");
            }
            config.RegisterModConfigMenu(helper, this.ModManifest);
            CurrentSize = Conversor.GetSizeFromConfigInt(config.Size);
            if (this.Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
            {
                DGAAPI = helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");
                if (DGAAPI != null)
                {
                    Printer.Debug("DGA API loaded");
                }
                else
                {
                    Printer.Debug("DGA API not loaded");
                }
            }
        }

        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (config.Enabled 
                && Context.IsWorldReady 
                && Game1.activeClickableMenu == null 
                && (!config.EnableKey || (config.EnableKey && Helper.Input.IsDown(config.Key))) 
                && (!config.EnableMouse || (config.EnableMouse && Helper.Input.IsDown(Conversor.GetMouseButton(config.Mouse)))))
            {
                Vector2 tile = Game1.currentCursorTile;
                Vector2 downTile = new Vector2(tile.X, tile.Y+1);
                if (Game1.currentLocation is FarmHouse
                    && (Game1.currentLocation as FarmHouse).fridgePosition.Equals(tile.ToPoint()))
                {
                    int yOffset = (int)(-94 * Game1.options.zoomLevel);
                    InventoryMenu menu = CreatePreviewMenu(tile, (Game1.currentLocation as FarmHouse).fridge.First().items.ToList(), 36, yOffset, 3);
                    menu.draw(e.SpriteBatch);
                }
                else if ((Game1.currentLocation.Objects.ContainsKey(tile)
                    && Game1.currentLocation.Objects[tile] != null
                    && Game1.currentLocation.Objects[tile] is Chest)
                    && (config.Range <= 0 ||(config.Range > 0
                    && Utility.tileWithinRadiusOfPlayer((int)tile.X, (int)tile.Y, config.Range, Game1.player))))
                {
                    DrawPreview(tile, e.SpriteBatch);
                }
                else if 
                    ((Game1.currentLocation.Objects.ContainsKey(downTile)
                    && Game1.currentLocation.Objects[downTile] != null
                    && Game1.currentLocation.Objects[downTile] is Chest)
                    && (config.Range <= 0 || (config.Range > 0
                    && Utility.tileWithinRadiusOfPlayer((int)downTile.X, (int)downTile.Y, config.Range, Game1.player))))
                {
                    DrawPreview(downTile, e.SpriteBatch);
                }
            }
        }

        public void DrawPreview(Vector2 tile, SpriteBatch b)
        {
            Chest chest = Game1.currentLocation.Objects[tile] as Chest;
            int slotsPerRow = 12;
            int maxRows = 8;
            int yOffset = GetSpriteYOffset(chest);
            int capacity = chest.GetActualCapacity();
            int rows = capacity / slotsPerRow;
            if(capacity >= slotsPerRow * maxRows)
            {
                capacity = slotsPerRow * maxRows;
                rows = capacity / slotsPerRow;
            }
            InventoryMenu menu = CreatePreviewMenu(tile, chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID).ToList(), capacity, yOffset, rows);
            menu.draw(b);
        }

        public InventoryMenu CreatePreviewMenu(Vector2 tile, List<Item> items, int capacity, int yOffset, int rows)
        {
            Vector2 position = new Vector2(
                (tile.X * Game1.tileSize) - Game1.viewport.X + Game1.tileSize / 2,
                (tile.Y * Game1.tileSize) - Game1.viewport.Y);
            position = Utility.ModifyCoordinatesForUIScale(position);
            HoverMenu menu = new HoverMenu((int)position.X, (int)position.Y, yOffset, false, items, null, capacity, rows);
            menu.populateClickableComponentList();
            return menu;
        }

        public int GetSpriteYOffset(Item item)
        {
            int offset = 0;
            if (item is Chest)
            {
                if ((item as Chest).SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
                {
                    offset = -32;
                }
                else if ((item as Chest).SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
                {
                    offset = -50;
                }
                else if ((item as Chest).SpecialChestType == Chest.SpecialChestTypes.Enricher)
                {

                }
                else if ((item as Chest).SpecialChestType == Chest.SpecialChestTypes.AutoLoader)
                {

                }
                else if ((item as Chest).fridge)
                {
                    offset = -74;
                }
                else
                {
                    offset = -42;
                }
            }
            offset = (int)(offset * Game1.options.zoomLevel);
            return offset;
        }

        public static int UpdateSize(float value)
        {
            int size = (int)value;
            CurrentSize = Conversor.GetSizeFromConfigInt(size);
            if(!(config.Size == (int)value))
            {
                Printer.Debug($"Size changed from {Conversor.GetSizeName(config.Size)} to {CurrentSize}");
            }
            return size;
        }

        public override object GetApi()
        {
            return new ChestPreviewAPI();
        }
    }
}
