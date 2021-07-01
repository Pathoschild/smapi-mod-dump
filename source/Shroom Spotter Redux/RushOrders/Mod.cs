/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SpaceShared;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Tools;

namespace RushOrders
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        public static RushOrdersConfig ModConfig { get; private set; }
        private static Api api;
        private static bool hadDialogue = false;
        private static int prevMoney = 0;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry( IModHelper helper )
        {
            instance = this;
            Log.Monitor = Monitor;

            Log.info("Loading Config");
            ModConfig = Helper.ReadConfig<RushOrdersConfig>();

            helper.Events.GameLoop.GameLaunched += onGameLaunched;
            helper.Events.Display.MenuChanged += onMenuChanged;
            helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
        }

        public override object GetApi()
        {
            return (api = new Api());
        }

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var capi = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (capi != null)
            {
                capi.RegisterModConfig(ModManifest, () => ModConfig = new RushOrdersConfig(), () => Helper.WriteConfig(ModConfig));
                capi.RegisterSimpleOption(ModManifest, "Price: Tool - One day", "The price multiplier for a one-day tool upgrade.", () => (float)ModConfig.PriceFactor.Tool.Rush, (float val) => ModConfig.PriceFactor.Tool.Rush = val);
                capi.RegisterSimpleOption(ModManifest, "Price: Tool - Instant", "The price multiplier for an instant upgrade.", () => (float)ModConfig.PriceFactor.Tool.Rush, (float val) => ModConfig.PriceFactor.Tool.Now = val);
                capi.RegisterSimpleOption(ModManifest, "Price: Building - Accelerate", "The price multiplier to accelerate building construction by one day.", () => (float)ModConfig.PriceFactor.Building.RushOneDay, (float val) => ModConfig.PriceFactor.Building.RushOneDay = val);
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void onMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // add rush option to menu
            switch (Game1.activeClickableMenu)
            {
                case ShopMenu shop:
                {
                    switch (shop.portraitPerson?.Name)
                    {
                        case "Clint":
                            addToolRushOrders(shop);
                            break;

                        case "Robin":
                            if (this.HasBuildingToRush() && !(e.OldMenu is RushConstructionMenu))
                                doRushBuildingDialogue();
                            break;
                    }
                    break;
                }

                case DialogueBox diagBox:
                {
                    var diag = instance.Helper.Reflection.GetField<Dialogue>(diagBox, "characterDialogue").GetValue();
                    if (diag?.speaker != null && diag.speaker.Name == "Robin" && this.HasBuildingToRush() && !(e.OldMenu is RushConstructionMenu))
                        doRushBuildingDialogue();

                    break;
                }
            }
        }

        private static void addToolRushOrders( ShopMenu shop )
        {
            Dictionary<ISalable, int[]> toAddStock = new Dictionary<ISalable, int[]>();
            Dictionary<ISalable, int[]> stock = shop.itemPriceAndStock;
            List<ISalable> toAddItems = new List<ISalable>();
            List<ISalable> items = shop.forSale;
            foreach ( KeyValuePair<ISalable, int[] > entry in stock )
            {
                if (!(entry.Key is Tool)) continue;
                Tool tool = entry.Key as Tool;

                if (!(tool is Axe || tool is Pickaxe || tool is Hoe || tool is WateringCan))
                    continue;

                // I'm going to edit the description, and I don't want to affect the original shop entry
                Tool toolRush = null, toolNow = null;
                if (tool is Axe)
                {
                    toolRush = new Axe();
                    toolNow = new Axe();
                }
                else if (tool is Pickaxe)
                {
                    toolRush = new Pickaxe();
                    toolNow = new Pickaxe();
                }
                else if (tool is Hoe)
                {
                    toolRush = new Hoe();
                    toolNow = new Hoe();
                }
                else if (tool is WateringCan)
                {
                    toolRush = new WateringCan();
                    toolNow = new WateringCan();
                }
                toolRush.UpgradeLevel = tool.UpgradeLevel;
                toolNow.UpgradeLevel = tool.UpgradeLevel;
                toolRush.DisplayName = tool.DisplayName + "         *RUSH*";
                toolNow.DisplayName  = tool.DisplayName + "       =INSTANT=";
                toolRush.description = "The tool will take one day to upgrade." + Environment.NewLine + Environment.NewLine + tool.description;
                toolNow.description = "The tool will be immediately upgraded." + Environment.NewLine + Environment.NewLine + tool.description;
                
                int price = getToolUpgradePrice(tool.UpgradeLevel);
                if (entry.Value[0] == price)
                {
                    int[] entryDataRush = (int[])entry.Value.Clone();
                    int[] entryDataNow = (int[])entry.Value.Clone();
                    entryDataRush[0] = (int)(entry.Value[ 0 ] * ModConfig.PriceFactor.Tool.Rush);
                    entryDataNow[0] = (int)(entry.Value[ 0 ] * ModConfig.PriceFactor.Tool.Now);

                    if (entryDataRush[0] != entry.Value[0] && ModConfig.PriceFactor.Tool.Rush > 0)
                    {
                        toAddStock.Add(toolRush, entryDataRush);
                        toAddItems.Add(toolRush);
                    }
                    if (entryDataNow[0] != entry.Value[0] && ModConfig.PriceFactor.Tool.Now > 0)
                    {
                        toAddStock.Add(toolNow, entryDataNow);
                        toAddItems.Add(toolNow);
                    }
                }
            }
            foreach (var elem in toAddStock)
                stock.Add(elem.Key, elem.Value);
            foreach (var elem in toAddItems)
                items.Add(elem);

            shop.forSale = items.OrderBy(i => i.Name).ToList();
        }

        /// <summary>Get whether there's a building being upgraded or constructed that can be rushed.</summary>
        private bool HasBuildingToRush()
        {
            // farmhouse upgrade
            if (Game1.player.daysUntilHouseUpgrade.Value > 1)
                return true;

            // building
            Building constructing = Game1.getFarm().getBuildingUnderConstruction();
            return constructing != null && (constructing.daysOfConstructionLeft.Value > 1 || constructing.daysUntilUpgrade.Value > 1);
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void onUpdateTicked(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // add tool rush order
            NPC clint = Game1.getCharacterFromName("Clint");
            bool hasDialog = clint?.CurrentDialogue.Count > 0 && clint.CurrentDialogue.Peek().getCurrentDialogue() == Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14317");
            if ( hasDialog && !hadDialogue && Game1.player.daysLeftForToolUpgrade.Value == 2 && Game1.player.toolBeingUpgraded.Value != null )
            {
                int curPrice = getToolUpgradePrice(Game1.player.toolBeingUpgraded.Value.UpgradeLevel);
                int diff = prevMoney - Game1.player.Money;

                if (diff == (int)(curPrice * ModConfig.PriceFactor.Tool.Now))
                {
                    Game1.player.daysLeftForToolUpgrade.Value = 0;
                    clint.CurrentDialogue.Pop();
                    Game1.drawDialogue(clint, "Thanks. I'll get started right away. It should be ready in a few minutes.");
                    api.InvokeToolRushed(Game1.player.toolBeingUpgraded.Value);
                }
                else if ( diff == ( int )( curPrice * ModConfig.PriceFactor.Tool.Rush) )
                {
                    Game1.player.daysLeftForToolUpgrade.Value = 1;
                    clint.CurrentDialogue.Pop();
                    Game1.drawDialogue(clint, "Thanks. I'll get started right away. It should be ready tomorrow.");
                    api.InvokeToolRushed(Game1.player.toolBeingUpgraded.Value);
                }
            }
            hadDialogue = hasDialog;
            prevMoney = Game1.player.Money;
        }

        private static void doRushBuildingDialogue()
        {
            Game1.activeClickableMenu = new RushConstructionMenu( Game1.activeClickableMenu );
        }

        private static MethodInfo getToolUpgradePriceInfo;
        public static int getToolUpgradePrice( int level )
        {
            if (getToolUpgradePriceInfo == null)
            {
                getToolUpgradePriceInfo = typeof(Utility).GetMethod("priceForToolUpgradeLevel", BindingFlags.NonPublic | BindingFlags.Static);
            }
            return (int) getToolUpgradePriceInfo.Invoke(null, new object[] { level });
        }

        public static void rushBuilding()
        {
            if (Game1.player.daysUntilHouseUpgrade.Value > 0)
                Game1.player.daysUntilHouseUpgrade.Value--;
            else if (Game1.getFarm().getBuildingUnderConstruction() != null)
            {
                Building building = Game1.getFarm().getBuildingUnderConstruction();
                if (building.daysOfConstructionLeft.Value > 0)
                    building.daysOfConstructionLeft.Value--;
                else if (building.daysUntilUpgrade.Value > 0)
                    building.daysUntilUpgrade.Value--;
            }
            api.InvokeBuildingRushed();
        }

        public static int getBuildingDaysLeft()
        {
            if (Game1.player.daysUntilHouseUpgrade.Value > 0)
                return Game1.player.daysUntilHouseUpgrade.Value;
            else if (Game1.getFarm().getBuildingUnderConstruction() != null)
            {
                Building building = Game1.getFarm().getBuildingUnderConstruction();
                if (building.daysOfConstructionLeft.Value > 0)
                    return building.daysOfConstructionLeft.Value;
                else if (building.daysUntilUpgrade.Value > 0)
                    return building.daysUntilUpgrade.Value;
            }

            return -1;
        }

        public static int getBuildingRushPrice()
        {
            int num = 0;
            if (Game1.player.daysUntilHouseUpgrade.Value > 0)
            {
                if (Game1.player.HouseUpgradeLevel == 0)
                    num = 10000;
                else if (Game1.player.HouseUpgradeLevel == 1)
                    num = 50000;
                else if (Game1.player.HouseUpgradeLevel == 2)
                    num = 100000;
            }
            else if ( Game1.getFarm().getBuildingUnderConstruction() != null )
            {
                Building building = Game1.getFarm().getBuildingUnderConstruction();
                if (building.daysOfConstructionLeft.Value > 0)
                {
                    BluePrint bp = new BluePrint(building.buildingType.Value);
                    num = bp.moneyRequired;
                }
                else if (building.daysUntilUpgrade.Value > 0)
                {
                    BluePrint bp = new BluePrint(building.getNameOfNextUpgrade());
                    num = bp.moneyRequired;
                }
            }

            return (int)(num * Mod.ModConfig.PriceFactor.Building.RushOneDay);
        }
    }
}
