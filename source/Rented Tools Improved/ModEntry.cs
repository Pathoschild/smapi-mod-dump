/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SolusCleansing/RentedToolsImproved
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;

namespace RentedToolsImproved
{
   
    public class ModEntry : Mod
    {
        
        private bool inited;
        private Farmer player;
        private NPC blacksmithNPC;
        private bool shouldCreateFailedToRentTools;
        private bool shouldCreateSucceededToRentTools;
        private bool rentedToolsOffered;
        private bool recycleOffered;

        private Dictionary<Tuple<List<Item>, int>, Item> rentedToolRefs;
        private ITranslationHelper i18n;
        private List<Vector2> blacksmithCounterTiles = new List<Vector2>();

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.Bootstrap;
            helper.Events.Display.MenuChanged += this.MenuCloseHandler;

            this.i18n = Helper.Translation;
        }

        private void Bootstrap(object sender, EventArgs e)
        {
            
            this.inited = false;
            this.player = null;
            this.blacksmithNPC = null;

            this.shouldCreateFailedToRentTools = false;
            this.shouldCreateSucceededToRentTools = false;
            this.rentedToolsOffered = false;
            this.recycleOffered = false;

            this.rentedToolRefs = new Dictionary<Tuple<List<Item>, int>, Item>();
            this.blacksmithCounterTiles = new List<Vector2>();

            
            this.player = Game1.player;
            this.blacksmithCounterTiles.Add(new Vector2(3f, 15f));
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc.Name == "Clint")
                {
                    this.blacksmithNPC = npc;
                    break;
                }
            }

            if (this.blacksmithNPC == null)
            {
                Monitor.Log("blacksmith NPC not found", LogLevel.Info);
            }

            
            this.inited = true;
        }

        private Tool GetToolBeingUpgraded(Farmer who)
        {
            if (who.toolBeingUpgraded.Value != null)
            {
                if (who.toolBeingUpgraded.Value is Axe || who.toolBeingUpgraded.Value is Pickaxe || who.toolBeingUpgraded.Value is Hoe || who.toolBeingUpgraded.Value is WateringCan)
                {
                    return who.toolBeingUpgraded.Value;
                }
                else if (who.toolBeingUpgraded.Value is GenericTool)
                {
                    return null;
                }
            }
            return null;
            
        }

        private void MenuCloseHandler(object sender, EventArgs e)
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
            return who.currentLocation.Name == "Blacksmith" && this.blacksmithCounterTiles.Contains(who.getTileLocation());
        }

        private bool HasRentedTools(Farmer who)
        {
            
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
            if (GetToolBeingUpgraded(who) != null )
            {
                return (GetToolBeingUpgraded(who) != null && !this.HasRentedTools(who));
            }
            if (GetToolBeingUpgraded(who) == null)
            {
                return false;
            }
            return false;
        }

        private void SetupRentToolsRemovalDialog(Farmer who)
        {
            who.currentLocation.createQuestionDialogue(
                i18n.Get("Blacksmith_RecycleTools_Menu"),
                new Response[1]
                {
                    new Response("Confirm", i18n.Get("Blacksmith_RecycleToolsMenu_Confirm")),
                },
                (Farmer whoInCallback, string whichAnswer) =>
                {
                    switch (whichAnswer)
                    {
                        case "Confirm":
                            this.RecycleTempTools(whoInCallback);
                            break;
                    }
                    return;
                },
                this.blacksmithNPC
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
                this.blacksmithNPC
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
            //Get tool that is gonna be upgraded
            Item toolToBuy = this.GetRentedToolByTool(GetToolBeingUpgraded(who));
            if (toolToBuy == null)
            {
                return;
            }
            //Sets rental tool quality to the quality of the current tool
            else if (toolToBuy is Tool actual)
            {
                actual.UpgradeLevel = GetToolBeingUpgraded(who).upgradeLevel - 1;
            }


            int toolCost = this.GetToolCost(toolToBuy);
            
            if (who.Money >= toolCost && who.freeSpotsInInventory() > 0)
            {
                
                ShopMenu.chargePlayer(who, 0, toolCost);
                Item item = who.addItemToInventory(toolToBuy);
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
        }

        private int GetToolCost(Item tool)
        {
            //Rental is free so toolcost = 0
            return 0;
        }
    }
}
    
