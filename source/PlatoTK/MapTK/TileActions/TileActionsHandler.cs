/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatoTK;
using PlatoTK.Events;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile.ObjectModel;
using xTile.Tiles;

namespace MapTK.TileActions
{
    internal class ShopSaveData
    {
        public Dictionary<string, List<BoughtItem>> BoughtItems { get; set; } = new Dictionary<string, List<BoughtItem>>();
    }

    internal class TileActionsHandler
    {
        internal readonly IPlatoHelper Plato;
        internal const string LuaScriptRepository = @"Data/MapTK/LuaScripts";
        internal const string ShopSaveData = @"MapTK.SaveData.Shops";
        internal string LastShop { get; set; } = "";

        public TileActionsHandler(IModHelper helper)
        {
            Plato = helper.GetPlatoHelper();
            Plato.Events.CallingTileAction += Events_CallingTileActionLua;
            Plato.Events.CallingTileAction += Events_CallingTileActionReloadMap;
            Plato.Events.CallingTileAction += Events_CallingTileActionFlag;
            Plato.Events.CallingTileAction += Events_CallingTileActionDropIn;
            Plato.Events.CallingTileAction += Events_CallingTileActionShops;

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            Plato.Content.Injections.InjectLoad(LuaScriptRepository, new Dictionary<string, string>());
            Plato.Content.Injections.InjectLoad(MapTKShop.ShopDataAsset, new Dictionary<string, MapTKShop>());
            Plato.Content.Injections.InjectLoad(MapTKInventory.InventoryDataAsset, new Dictionary<string, MapTKInventory>());

            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.Saving += GameLoop_Saving;

            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            helper.ConsoleCommands.Add("MapTKShop", "Start a MapTK shop by id: MapTKShop IdOfTheShop", (command, parameter) =>
             {
                 if(parameter.Length > 0)
                     OpenMapTKShop(parameter[0]);
             });
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            foreach (var shopid in MapTKShop.Bought.Keys)
                if (MapTKShop.GetShop(Plato.ModHelper, shopid) is MapTKShop shop 
                    && (shop.Restock == "Daily" || (shop.Restock == "Monthly" && Game1.dayOfMonth == 1) || (shop.Restock == "Yearly" && Game1.dayOfMonth == 1 && Game1.currentSeason.ToLower() == "spring")))
                    MapTKShop.Bought.Remove(shopid);
        }

        private void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            var svd = new ShopSaveData();
            foreach (var s in MapTKShop.Bought)
                svd.BoughtItems.Add(s.Key, s.Value);

            Plato.ModHelper.Data.WriteSaveData(ShopSaveData, svd);
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            if(Plato.ModHelper.Data.ReadSaveData<ShopSaveData>(ShopSaveData) is ShopSaveData svd)
            {
                MapTKShop.Bought.Clear();
                foreach (var s in svd.BoughtItems)
                    MapTKShop.Bought.Add(s.Key, s.Value);
            }
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            var api = Plato.ModHelper.ModRegistry.GetApi<PlatoTK.APIs.IContentPatcher>("Pathoschild.ContentPatcher");
            api.RegisterToken(Plato.ModHelper.ModRegistry.Get(Plato.ModHelper.ModRegistry.ModID).Manifest, "Shops", new ShopToken());
            api.RegisterToken(Plato.ModHelper.ModRegistry.Get(Plato.ModHelper.ModRegistry.ModID).Manifest, "ShopInventories", new ShopInventoryToken());
            api.RegisterToken(Plato.ModHelper.ModRegistry.Get(Plato.ModHelper.ModRegistry.ModID).Manifest, "ShopPortraits", new ShopPortraitsToken());
            api.RegisterToken(Plato.ModHelper.ModRegistry.Get(Plato.ModHelper.ModRegistry.ModID).Manifest, "LuaScripts", new LuaScriptsToken());
        }

        private bool SoldItemInCustomShop(ISalable item, Farmer who, int stock)
        {
            if (LastShop == "")
                return false;

            if (!MapTKShop.Bought.ContainsKey(LastShop))
                MapTKShop.Bought.Add(LastShop, new List<BoughtItem>());

            if (MapTKShop.Bought[LastShop].FirstOrDefault(b => b.ItemName == item.Name) is BoughtItem bi)
                bi.Stock += stock;
            else
                MapTKShop.Bought[LastShop].Add(new BoughtItem(item,stock));

            return false;
        }

        private void Events_CallingTileActionShops(object sender, PlatoTK.Events.ICallingTileActionEventArgs e)
        {
            if(e.Trigger == "MapTKShop" && e.Parameter.Length > 0)
                OpenMapTKShop(e.Parameter[0]);
        }

        private void OpenMapTKShop(string shopId)
        {
            if (!string.IsNullOrEmpty(shopId) && MapTKShop.GetShop(Plato.ModHelper, shopId) is MapTKShop shop)
            {
                LastShop = shopId;

                var shopMenu = new ShopMenu(MapTKShop.GetInventory(LastShop, Plato.ModHelper), shop.Currency);

                if (shop.Restock != "Always")
                    shopMenu.onPurchase = SoldItemInCustomShop;

                try
                {
                    if (shop.ShopKeeper != "")
                    {
                        if (Game1.getCharacterFromName(shop.ShopKeeper) is NPC shopkeeper)
                        {
                            shopMenu.setUpShopOwner(shop.ShopKeeper);
                            shopMenu.portraitPerson = shopkeeper;
                        }
                        else
                        {
                            var npc = new NPC(null, Vector2.Zero, "Town", 0, shop.ShopKeeper.Split('.')[0], false, null, Plato.ModHelper.GameContent.Load<Texture2D>($"{ShopPortraitsToken.ShopPortraitsPrefix}/{shop.ShopKeeper}"));
                            shopMenu.portraitPerson = npc;
                            Game1.removeThisCharacterFromAllLocations(npc);
                        }

                    }
                }
                catch
                {

                }

                if (shop.Message != "")
                    shopMenu.potraitPersonDialogue = Game1.parseText(shop.Message, Game1.dialogueFont, 304);

                Game1.activeClickableMenu = shopMenu;
            }
        }

        private void Events_CallingTileActionDropIn(object sender, PlatoTK.Events.ICallingTileActionEventArgs e)
        {
            Farmer who = e.Caller ?? Game1.player;
            GameLocation location = e.Location ?? Game1.currentLocation;

            if (e.Trigger == "DropIn"
                && (who?.IsLocalPlayer ?? false))
            {
                if (who.ActiveObject is StardewValley.Object o)
                {
                    Func<StardewValley.Object, bool> validate;

                    int stack = 1;

                    if (int.TryParse(e.Parameter[0], out int s))
                        stack = s;

                    if (e.Parameter[1] == "L#")
                        validate = (obj) => (bool)CallLua(e, obj, 2).Globals["result"];
                    else
                        validate = (obj) => e.Parameter[1] == obj.Name || 
                        (int.TryParse(e.Parameter[1], out int index) && (index == obj.ParentSheetIndex || obj.Category == index));

                    if (who.ActiveObject.Stack >= stack && validate(o))
                    {
                        who.ActiveObject.Stack -= s;
                        if (who.ActiveObject.Stack <= 0)
                        {
                            who.removeItemFromInventory(who.ActiveObject);
                            who.ActiveObject = null;

                            if (e.Tile != null && e.Tile.Properties.TryGetValue("@DropIn_Success", out PropertyValue value))
                                location?.performAction(value.ToString(), who, new xTile.Dimensions.Location(e.Position.X, e.Position.Y));
                        }
                        else if (e.Tile != null && e.Tile.Properties.TryGetValue("@DropIn_Failure", out PropertyValue value))
                            location?.performAction(value.ToString(), who, new xTile.Dimensions.Location(e.Position.X, e.Position.Y));

                    }
                }
                else if(e.Tile != null && e.Tile.Properties.TryGetValue("@DropIn_Default", out PropertyValue value))
                    (e.Location ?? Game1.currentLocation).performAction(value.ToString(), who, new xTile.Dimensions.Location(e.Position.X, e.Position.Y));
            }
        }

        private void Events_CallingTileActionFlag(object sender, PlatoTK.Events.ICallingTileActionEventArgs e)
        {
            if (Game1.MasterPlayer is Farmer who)
                if (e.Trigger == "SetFlag" && !who.hasOrWillReceiveMail(e.Parameter[0]))
                    who.mailReceived.Add(e.Parameter[0]);
                else if (e.Trigger == "UnsetFlag" && who.hasOrWillReceiveMail(e.Parameter[0]))
                    who.mailReceived.Remove(e.Parameter[0]);
        }

        private void Events_CallingTileActionReloadMap(object sender, PlatoTK.Events.ICallingTileActionEventArgs e)
        {
            if(e.Trigger == "ReloadMap")
            {
                Plato.Utilities.ReloadMap(e.Location);
                e.TakeOver(true);
            }
        }

        private void Events_CallingTileActionLua(object sender, PlatoTK.Events.ICallingTileActionEventArgs e)
        {
            if(e.Trigger == "L#")
            {
                CallLua(e);
                e.TakeOver(true);
            }
        }

        private MoonSharp.Interpreter.Script CallLua(PlatoTK.Events.ICallingTileActionEventArgs e, StardewValley.Object input = null, int skipParams = 0)
        {
            string[] parameter = e.Parameter.Skip(skipParams).ToArray();
            string codeId = parameter[0];
            Tile tile = e.Tile;
            string code;
            string function = null;

            if (codeId == "this")
            {
                string propertyId = parameter[1];
                code = tile.Properties[$"@Lua_{propertyId}"].ToString();
                

                if (parameter.Length > 2)
                    function = parameter[2].ToString();
                else
                {
                    code = $"function callthis(location,tile,layer) {code} end";
                    function = "callthis";
                }
            }
            else
            {
                code = Plato.ModHelper.GameContent.Load<Dictionary<string, string>>(LuaScriptRepository)[codeId];
                if (parameter.Length > 1)
                    function = parameter[1];
            }
            Dictionary<string, object> inputDict = null;

            if (input != null)
                inputDict = new Dictionary<string, object>() { { "input", input } };

            var lua = Plato.Lua.LoadLuaCode(code, inputDict);

            if (!string.IsNullOrEmpty(function))
                lua.Call(lua.Globals[function], e.Location,Utility.PointToVector2(e.Position), e.Layer.Id,e);

            return lua;
        }
    }
}
