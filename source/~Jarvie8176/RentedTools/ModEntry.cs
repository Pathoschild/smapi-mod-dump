using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace RentedTools
{
    // NOTE: one might want to implement a static class `bool IsRentedTool(Item)` so some code can be reused

    public class ModEntry : Mod
    {
        private bool inited;
        private Farmer player;
        private NPC blacksmithNpc;
        private bool shouldCreateFailedToRentTools;
        private bool shouldCreateSucceededToRentTools;
        private bool rentedToolsOffered;
        private bool recycleOffered;

        private Dictionary<Tuple<List<Item>, int>, Item> rentedToolRefs;

        private ITranslationHelper i18n;


        private List<Vector2> blackSmithCounterTiles = new List<Vector2>();

        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += this.Bootstrap;
            MenuEvents.MenuClosed += this.MenuCloseHandler;

            this.i18n = helper.Translation;
        }

        private void Bootstrap(object sender, EventArgs e)
        {
            // params reset
            this.inited = false;
            this.player = null;
            this.blacksmithNpc = null;

            this.shouldCreateFailedToRentTools = false;
            this.shouldCreateSucceededToRentTools = false;
            this.rentedToolsOffered = false;
            this.recycleOffered = false;

            this.rentedToolRefs = new Dictionary<Tuple<List<Item>, int>, Item>();
            this.blackSmithCounterTiles = new List<Vector2>();

            // params init
            this.player = Game1.player;
            this.blackSmithCounterTiles.Add(new Vector2(3f, 15f));
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc.Name == "Clint")
                {
                    this.blacksmithNpc = npc;
                    break;
                }
            }

            if (this.blacksmithNpc == null)
            {
                Monitor.Log("blacksmith NPC not found", LogLevel.Info);
            }

            // init done
            this.inited = true;
        }

        private Tool GetToolBeingUpgraded(Farmer who)
        {
            return who.toolBeingUpgraded.Value;
        }

        private void MenuCloseHandler(object sender, EventArgsClickableMenuClosed e)
        {
            if (this.shouldCreateFailedToRentTools)
            {
                this.SetupFailedToRentDialog(this.player);
                this.shouldCreateFailedToRentTools = false;
                return;
            }

            if (this.shouldCreateSucceededToRentTools)
            {
                this.SetupSucceededToRentDialog(this.player);
                this.shouldCreateSucceededToRentTools = false;
                return;
            }

            if (this.rentedToolsOffered)
            {
                this.rentedToolsOffered = false;
                return;
            }

            if (this.recycleOffered)
            {
                this.recycleOffered = false;
                return;
            }

            if (this.inited && this.IsPlayerAtCounter(this.player))
            {
                if (this.player.toolBeingUpgraded.Value == null && this.HasRentedTools(this.player))
                {
                    this.SetupRentToolsRemovalDialog(this.player);
                }
                else if (this.ShouldOfferTools(this.player))
                {
                    this.SetupRentToolsOfferDialog(this.player);
                }
            }
        }

        private bool IsPlayerAtCounter(Farmer who)
        {
            return who.currentLocation.Name == "Blacksmith" && this.blackSmithCounterTiles.Contains(who.getTileLocation());
        }

        private bool HasRentedTools(Farmer who)
        {
            // Should recycle if:
            // (there's no tool being upgraded) and (there are tools of the same type)
            bool result = false;

            IList<Item> inventory = who.Items;
            List<Tool> tools = inventory
                .Where(tool => tool is Axe || tool is Pickaxe || tool is WateringCan || tool is Hoe)
                .OfType<Tool>()
                .ToList();

            if (GetToolBeingUpgraded(who) != null)
            {
                result = tools.Exists(item => item.GetType().IsInstanceOfType(this.GetToolBeingUpgraded(who)));
            }
            else
            {
                foreach (Tool tool in tools)
                {
                    if (tools.Exists(item => item.GetType().IsInstanceOfType(tool) && item.UpgradeLevel < tool.UpgradeLevel))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        private bool ShouldOfferTools(Farmer who)
        {
            return (GetToolBeingUpgraded(who) != null && !this.HasRentedTools(who));
        }

        private void SetupRentToolsRemovalDialog(Farmer who)
        {
            who.currentLocation.createQuestionDialogue(
                i18n.Get("Blacksmith_RecycleTools_Menu"),
                new Response[2]
                {
                    new Response("Confirm", i18n.Get("Blacksmith_RecycleToolsMenu_Confirm")),
                    new Response("Leave", i18n.Get("Blacksmith_RecycleToolsMenu_Leave")),
                },
                (Farmer whoInCallback, string whichAnswer) =>
                {
                    switch (whichAnswer)
                    {
                        case "Confirm":
                            this.RecycleTempTools(whoInCallback);
                            break;
                        case "Leave":
                            // do nothing
                            break;
                    }
                    return;
                },
                this.blacksmithNpc
            );
            this.recycleOffered = true;
        }

        private void SetupRentToolsOfferDialog(Farmer who)
        {
            who.currentLocation.createQuestionDialogue(
                i18n.Get("Blacksmith_OfferTools_Menu",
                new
                {
                    oldToolName = GetRentedToolByTool(GetToolBeingUpgraded(who))?.DisplayName,
                    newToolName = GetToolBeingUpgraded(who)?.DisplayName
                }),
                new Response[2]
                {
                    new Response("Confirm", i18n.Get("Blacksmith_OfferToolsMenu_Confirm")),
                    new Response("Leave", i18n.Get("Blacksmith_OfferToolsMenu_Leave")),
                },
                (Farmer whoInCallback, string whichAnswer) =>
                {
                    switch (whichAnswer)
                    {
                        case "Confirm":
                            this.BuyTempTool(whoInCallback);
                            break;
                        case "Leave":
                            // do nothing
                            break;
                    }
                    return;
                },
                this.blacksmithNpc
            );
            rentedToolsOffered = true;
        }

        private void SetupSucceededToRentDialog(Farmer who)
        {
            i18n.Get("Blacksmith_HowToReturn");
        }

        private void SetupFailedToRentDialog(Farmer who)
        {
            if (who.freeSpotsInInventory() <= 0)
            {
                Game1.drawObjectDialogue(i18n.Get("Blacksmith_NoInventorySpace"));
            }
            else
            {
                Game1.drawObjectDialogue(i18n.Get("Blacksmith_InsufficientFundsToRentTool"));
            }
        }

        private Tool GetRentedToolByTool(Item tool)
        {
            if (tool is Axe)
            {
                return new Axe();
            }
            else if (tool is Pickaxe)
            {
                return new Pickaxe();
            }
            else if (tool is WateringCan)
            {
                return new WateringCan();
            }
            else if (tool is Hoe)
            {
                return new Hoe();
            }
            else
            {
                Monitor.Log($"unsupported upgradable tool: {tool?.ToString()}");
                return null;
            }
        }

        private void BuyTempTool(Farmer who)
        {
            // NOTE: there's no thread safe method for money transactions, so I suppose the game doesn't care about it as well?

            // TODO: handle upgradeLevel so rented tool is not always the cheapest

            Item toolToBuy = this.GetRentedToolByTool(GetToolBeingUpgraded(who));

            if (toolToBuy == null)
            {
                return;
            }

            int toolCost = this.GetToolCost(toolToBuy);

            if (who.money >= toolCost && who.freeSpotsInInventory() > 0)
            {
                ShopMenu.chargePlayer(who, 0, toolCost);
                who.addItemToInventory(toolToBuy);
                this.shouldCreateSucceededToRentTools = true;
            }
            else
            {
                this.shouldCreateFailedToRentTools = true;
            }

        }

        private void RecycleTempTools(Farmer who)
        {
            // recycle all rented tools

            IList<Item> inventory = who.Items;
            List<Tool> tools = inventory
                .Where(tool => tool is Axe || tool is Pickaxe || tool is WateringCan || tool is Hoe)
                .OfType<Tool>()
                .ToList();

            foreach (Tool tool in tools)
            {
                if (tools.Exists(item => tool.GetType().IsInstanceOfType(item) && tool.UpgradeLevel < item.UpgradeLevel))
                {
                    who.removeItemFromInventory(tool);
                }
            }

            return;


            // NOTE: not using custom type anymore

            /*

            while (who.items.Any(item => item is IRentedTool))
            {
                foreach (Item item in who.items)
                {
                    if (item is IRentedTool)
                    {
                        who.removeItemFromInventory(item);
                        break;
                    }
                }
            }

            */
        }

        private int GetToolCost(Item tool)
        {
            // TODO: this function is subject to change
            return 200;
        }

        // NOTE: not using the custom type anymore

        /* 
        private void BeforeSaveHandler(object sender, EventArgs e)
        {
            rentedToolRefs.Clear();

            // NOTE: Farmer.addItemToInventory()'s implementation uses raw mapping of item position and index in Farmer.items
            // i.e. items[10] means the 10th item in player's inventory, I suppose it's subject to change unless the game provides
            // and uses a method that would return index of item after inserting.

            // get rented tool references

            for (int i = 0; i < this.player.items.Count; i++)
            {
                if (this.player.items[i] is IRentedTool)
                {
                    this.rentedToolRefs.Add(new Tuple<List<Item>, int>(this.player.items, i), this.player.items[i]);
                    Monitor.Log($"rented tool found: {this.player.items[i]} in player's inventory");
                }
            }

            // loop through all chests
            foreach (GameLocation loc in Game1.locations)
            {
                foreach (var key in loc.objects.Keys)
                {
                    if (loc.objects[key] is StardewValley.Objects.Chest)
                    {
                        // loop through each chest
                        List<Item> currItems = ((StardewValley.Objects.Chest)loc.objects[key]).items;
                        for (int i = 0; i < currItems.Count(); i++)
                        {
                            if (currItems[i] is IRentedTool)
                            {
                                this.rentedToolRefs.Add(new Tuple<List<Item>, int>(currItems, i), currItems[i]);
                                Monitor.Log($"rented tool found: {currItems[i].ToString()} in {loc.objects[key]}");
                            }
                        }
                    }
                }
            }

            // remove rented tools from all inventories

            foreach(KeyValuePair<Tuple<List<Item>, int>, Item> pair in this.rentedToolRefs.Reverse())
            {
                Monitor.Log($"removing {pair.Value.ToString()} in {pair.Key.Item1.ToString()} at index {pair.Key.Item2}");
                pair.Key.Item1.RemoveAt(pair.Key.Item2);
            }
        }

        private void AfterSaveHandler(object sender, EventArgs e)
        {
            // load rented tools back to player's inventory

            foreach (KeyValuePair<Tuple<List<Item>, int>, Item> pair in this.rentedToolRefs)
            {
                Monitor.Log($"addint {pair.Value.ToString()} to {pair.Key.Item1.ToString()} at index {pair.Key.Item2}");
                pair.Key.Item1.Insert(pair.Key.Item2, pair.Value);
            }

            rentedToolRefs.Clear();
        }

        */
    }
}
