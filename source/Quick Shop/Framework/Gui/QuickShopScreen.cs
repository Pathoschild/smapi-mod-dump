/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/QuickShop
**
*************************************************/

using System;
using System.Collections.Generic;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley.Util;
using Object = StardewValley.Object;

namespace QuickShop.Framework.Gui
{
    public class QuickShopScreen : ScreenGui
    {
        public QuickShopScreen()
        {
            var buttonTitle = GetTranslation("quickShop.button");
            var gameLocation = Game1.game1.instanceGameLocation;

            var pierreShopTitle = $"{buttonTitle} {GetButtonTranslation("pierreShop")}";
            AddElement(new Button(pierreShopTitle, pierreShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu =
                        new ShopMenu(new List<ISalable>(Utility.getShopStock(true)), who: "Pierre");
                }
            });

            var harveyShopTitle = $"{buttonTitle} {GetButtonTranslation("harveyShop")}";

            AddElement(new Button(harveyShopTitle, harveyShopTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new ShopMenu(Utility.getHospitalStock()); }
            });
            var gusShopTitle = $"{buttonTitle} {GetButtonTranslation("gusShop")}";
            AddElement(new Button(gusShopTitle, gusShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(Utility.getSaloonStock(), who: "Gus");
                }
            });

            var robinShopTitle = $"{buttonTitle} {GetButtonTranslation("robinShop")}";
            AddElement(new Button(robinShopTitle, robinShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), who: "Robin");
                }
            });

            var carpenterBuildingTitle = $"{buttonTitle} {GetButtonTranslation("carpenterBuilding")}";
            var carpenterBuildingLocation = "";
            var carpenterBuildingTileX = 0;
            var carpenterBuildingTileY = 0;
            var carpenterBuilding = false;
            AddElement(new Button(carpenterBuildingTitle, carpenterBuildingTitle)
            {
                OnLeftClicked = () =>
                {
                    carpenterBuildingTileX = Game1.player.getTileX();
                    carpenterBuildingTileY = Game1.player.getTileY();
                    carpenterBuildingLocation = Game1.player.currentLocation.Name;
                    carpenterBuilding = true;
                    Game1.activeClickableMenu = new CarpenterMenu();
                }
            });

            ModEntry.GetInstance().Helper.Events.GameLoop.UpdateTicked += (sender, args) =>
            {
                if (!carpenterBuilding) return;
                if (Game1.activeClickableMenu is CarpenterMenu) return;
                if (carpenterBuildingLocation != Game1.player.currentLocation.Name)
                {
                    Game1.warpFarmer(carpenterBuildingLocation, carpenterBuildingTileX, carpenterBuildingTileY,
                        Game1.player.facingDirection);
                }

                carpenterBuilding = false;
            };

            var willyShopTitle = $"{buttonTitle} {GetButtonTranslation("willyShop")}";
            AddElement(new Button(willyShopTitle, willyShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(Utility.getFishShopStock(Game1.player), who: "Willy");
                }
            });

            var krobusShopTitle = $"{buttonTitle} {GetButtonTranslation("krobusShop")}";
            AddElement(new Button(krobusShopTitle, krobusShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(new Sewer().getShadowShopStock(), who: "Krobus");
                }
            });

            var marnieShopTitle = $"{buttonTitle} {GetButtonTranslation("marnieShop")}";
            AddElement(new Button(marnieShopTitle, marnieShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(Utility.getAnimalShopStock(), who: "Marnie");
                }
            });

            var animalShopTitle = $"{buttonTitle} {GetButtonTranslation("animalShop")}";
            var animalShopLocation = "";
            var animalShopTileX = 0;
            var animalShopTileY = 0;
            var animalShop = false;
            AddElement(new Button(animalShopTitle, animalShopTitle)
            {
                OnLeftClicked = () =>
                {
                    animalShopTileX = Game1.player.getTileX();
                    animalShopTileY = Game1.player.getTileY();
                    animalShopLocation = Game1.player.currentLocation.Name;
                    animalShop = true;
                    Game1.activeClickableMenu = new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock());
                }
            });

            ModEntry.GetInstance().Helper.Events.GameLoop.UpdateTicked += (sender, args) =>
            {
                if (!animalShop) return;
                if (Game1.activeClickableMenu is PurchaseAnimalsMenu) return;
                if (animalShopLocation != Game1.player.currentLocation.Name)
                {
                    Game1.warpFarmer(animalShopLocation, animalShopTileX, animalShopTileY,
                        Game1.player.facingDirection);
                }

                animalShop = false;
            };

            var merchantShopTitle = $"{buttonTitle} {GetButtonTranslation("merchantShop")}";
            AddElement(new Button(merchantShopTitle, merchantShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(
                        Utility.getTravelingMerchantStock((int) ((long) Game1.uniqueIDForThisGame +
                                                                 Game1.stats.DaysPlayed)), who: "TravelerNightMarket",
                        on_purchase: Utility.onTravelingMerchantShopPurchase);
                }
            });

            var magicShopBoatTitle = $"{buttonTitle} {GetButtonTranslation("magicShopBoat")}";
            AddElement(new Button(magicShopBoatTitle, magicShopBoatTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(new BeachNightMarket().geMagicShopStock(),
                        who: "magicBoatShop");
                }
            });

            var decorationBoatShopTitle = $"{buttonTitle} {GetButtonTranslation("decorationBoatShop")}";
            AddElement(new Button(decorationBoatShopTitle, decorationBoatShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu =
                        new ShopMenu(new BeachNightMarket().getBlueBoatStock(), who: "BlueBoat");
                }
            });

            var renovationTitle = $"{buttonTitle} {GetButtonTranslation("renovation")}";
            AddElement(new Button(renovationTitle, renovationTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu =
                        new ShopMenu(HouseRenovation.GetAvailableRenovations(),
                            on_purchase: HouseRenovation.OnPurchaseRenovation);
                }
            });

            var clintShopTitle = $"{buttonTitle} {GetButtonTranslation("clintShop")}";
            AddElement(new Button(clintShopTitle, clintShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu =
                        new ShopMenu(Utility.getBlacksmithStock(), who: "Clint");
                }
            });

            if (Game1.player.toolBeingUpgraded.Value == null &&
                Utility.getBlacksmithUpgradeStock(Game1.player).Values.Count != 0)
            {
                var upgradeTitle = $"{buttonTitle} {GetButtonTranslation("upgrade")}";
                AddElement(new Button(upgradeTitle, upgradeTitle)
                {
                    OnLeftClicked = () =>
                    {
                        Game1.activeClickableMenu =
                            new ShopMenu(Utility.getBlacksmithUpgradeStock(Game1.player));
                    }
                });
            }

            var geodeTitle = $"{buttonTitle} {GetButtonTranslation("geode")}";
            AddElement(new Button(geodeTitle, geodeTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new GeodeMenu(); }
            });

            var mailboxTitle = $"{buttonTitle} {GetButtonTranslation("mailbox")}";
            AddElement(new Button(mailboxTitle, mailboxTitle)
            {
                OnLeftClicked = () => { gameLocation.mailbox(); }
            });

            var calendarTitle = $"{buttonTitle} {GetButtonTranslation("calendar")}";
            AddElement(new Button(calendarTitle, calendarTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new Billboard(); }
            });

            var helpWantedTitle = $"{buttonTitle} {GetButtonTranslation("helpWanted")}";
            AddElement(new Button(helpWantedTitle, helpWantedTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new Billboard(true); }
            });

            var specialOrdersBoardTitle = $"{buttonTitle} {GetButtonTranslation("specialOrdersBoard")}";
            AddElement(new Button(specialOrdersBoardTitle, specialOrdersBoardTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new SpecialOrdersBoard(); }
            });

            var morrisShopTitle = $"{buttonTitle} {GetButtonTranslation("morrisShop")}";
            AddElement(new Button(morrisShopTitle, morrisShopTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new ShopMenu(Utility.getJojaStock()); }
            });

            var dwarfShopTitle = $"{buttonTitle} {GetButtonTranslation("dwarfShop")}";
            AddElement(new Button(dwarfShopTitle, dwarfShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(Utility.getDwarfShopStock(), who: "Dwarf");
                }
            });

            var volcanoDungeonShopTitle = $"{buttonTitle} {GetButtonTranslation("volcanoDungeonShop")}";
            Dictionary<ISalable, int[]> dictionary = new Dictionary<ISalable, int[]>
            {
                {new Boots(853), new[] {0, int.MaxValue, 848, 100}}
            };
            AddElement(new Button(volcanoDungeonShopTitle, volcanoDungeonShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Random random =
                        new Random((int) (Game1.stats.DaysPlayed + 898U + (long) Game1.uniqueIDForThisGame));
                    Utility.AddStock(dictionary, new Object(Vector2.Zero, 286, int.MaxValue), 150);
                    Utility.AddStock(dictionary, new Object(Vector2.Zero, 287, int.MaxValue), 300);
                    Utility.AddStock(dictionary, new Object(Vector2.Zero, 288, int.MaxValue), 500);
                    Utility.AddStock(dictionary,
                        random.NextDouble() < 0.5
                            ? new Object(Vector2.Zero, 244, int.MaxValue)
                            : new Object(Vector2.Zero, 237, int.MaxValue), 600);
                    if (random.NextDouble() < 0.25)
                        Utility.AddStock(dictionary, new Hat(77), 5000);
                    if (!Game1.player.craftingRecipes.ContainsKey("Warp Totem: Island"))
                        Utility.AddStock(dictionary, new Object(886, 1, true), 5000);
                    if (!Game1.player.cookingRecipes.ContainsKey("Ginger Ale"))
                        Utility.AddStock(dictionary, new Object(903, 1, true), 500);
                    Game1.activeClickableMenu = new ShopMenu(dictionary, who: "VolcanoShop", context: "VolcanoShop");
                }
            });

            var marlonShopTitle = $"{buttonTitle} {GetButtonTranslation("marlonShop")}";
            AddElement(new Button(marlonShopTitle, marlonShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(Utility.getAdventureShopStock(), who: "Marlon");
                }
            });

            var hatShopTitle = $"{buttonTitle} {GetButtonTranslation("hatShop")}";
            AddElement(new Button(hatShopTitle, hatShopTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new ShopMenu(Utility.getHatStock()); }
            });

            var movieTheaterShopTitle = $"{buttonTitle} {GetButtonTranslation("movieTheaterShop")}";
            AddElement(new Button(movieTheaterShopTitle, movieTheaterShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Object @object = new Object(809, 1);
                    Game1.activeClickableMenu = new ShopMenu(new Dictionary<ISalable, int[]>
                    {
                        {
                            @object,
                            new[] {@object.salePrice(), int.MaxValue}
                        }
                    }, who: "boxOffice");
                }
            });


            var casinoShopTitle = $"{buttonTitle} {GetButtonTranslation("casinoShop")}";
            AddElement(new Button(casinoShopTitle, casinoShopTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new ShopMenu(Utility.getQiShopStock(), 2); }
            });

            var qiShopTitle = $"{buttonTitle} {GetButtonTranslation("qiShop")}";
            AddElement(new Button(qiShopTitle, qiShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.playSound("qi_shop");
                    Game1.activeClickableMenu = new ShopMenu(Utility.GetQiChallengeRewardStock(Game1.player), 4,
                        context: "QiGemShop");
                }
            });

            var qiSpecialOrdersBoardTitle = $"{buttonTitle} {GetButtonTranslation("qiSpecialOrdersBoard")}";
            AddElement(new Button(qiSpecialOrdersBoardTitle, qiSpecialOrdersBoardTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new SpecialOrdersBoard("Qi"); }
            });

            var sandyShopTitle = $"{buttonTitle} {GetButtonTranslation("sandyShop")}";
            AddElement(new Button(sandyShopTitle, sandyShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu =
                        new ShopMenu(
                            GetMethod(gameLocation, "sandyShopStock")
                                .Invoke<Dictionary<ISalable, int[]>>(), who: "Sandy",
                            on_purchase: OnSandyShopPurchase);
                }
            });

            var desertShopTitle = $"{buttonTitle} {GetButtonTranslation("desertShop")}";
            AddElement(new Button(desertShopTitle, desertShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(Desert.getDesertMerchantTradeStock(Game1.player));
                }
            });

            var islandTradeTitle = $"{buttonTitle} {GetButtonTranslation("islandTrade")}";
            AddElement(new Button(islandTradeTitle, islandTradeTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(IslandNorth.getIslandMerchantTradeStock(Game1.player),
                        who: "IslandTrade");
                }
            });

            var resortBarTitle = $"{buttonTitle} {GetButtonTranslation("resortBar")}";
            AddElement(new Button(resortBarTitle, resortBarTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(dictionary, who: "Gus", context: "ResortBar");
                }
            });


            if (Game1.player.mailReceived.Contains("JojaMember"))
            {
                var joJaCdTitle = $"{buttonTitle} {GetButtonTranslation("joJaCD")}";
                AddElement(new Button(joJaCdTitle, joJaCdTitle)
                {
                    OnLeftClicked = () =>
                    {
                        Game1.activeClickableMenu =
                            new JojaCDMenu(Game1.temporaryContent.Load<Texture2D>("LooseSprites\\JojaCDForm"));
                    }
                });
            }

            var iceCreamStandTitle = $"{buttonTitle} {GetButtonTranslation("iceCreamStand")}";
            AddElement(new Button(iceCreamStandTitle, iceCreamStandTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new ShopMenu(new Dictionary<ISalable, int[]>
                    {
                        {
                            new Object(233, 1),
                            new[] {250, int.MaxValue}
                        }
                    });
                }
            });

            var wizardBuildingTitle = $"{buttonTitle} {GetButtonTranslation("wizardBuilding")}";
            var wizardBuildingLocation = "";
            var wizardBuildingTileX = 0;
            var wizardBuildingTileY = 0;
            var wizardBuilding = false;
            AddElement(new Button(wizardBuildingTitle, wizardBuildingTitle)
            {
                OnLeftClicked = () =>
                {
                    wizardBuildingTileX = Game1.player.getTileX();
                    wizardBuildingTileY = Game1.player.getTileY();
                    wizardBuildingLocation = Game1.player.currentLocation.Name;
                    wizardBuilding = true;
                    Game1.activeClickableMenu = new CarpenterMenu(true);
                }
            });

            ModEntry.GetInstance().Helper.Events.GameLoop.UpdateTicked += (sender, args) =>
            {
                if (!wizardBuilding) return;
                if (Game1.activeClickableMenu is CarpenterMenu) return;
                if (wizardBuildingLocation != Game1.player.currentLocation.Name)
                {
                    Game1.warpFarmer(wizardBuildingLocation, wizardBuildingTileX, wizardBuildingTileY,
                        Game1.player.facingDirection);
                }

                wizardBuilding = false;
            };


            var changeAppearanceTitle = $"{buttonTitle} {GetButtonTranslation("changeAppearance")}";
            AddElement(new Button(changeAppearanceTitle, changeAppearanceTitle)
            {
                OnLeftClicked = () =>
                {
                    gameLocation.createQuestionDialogue(
                        Game1.content.LoadString("Strings\\Locations:WizardTower_WizardShrine").Replace('\n', '^'),
                        gameLocation.createYesNoResponses(), "WizardShrine");
                }
            });

            if (!Game1.player.mailReceived.Contains("JojaMember"))
            {
                var bundlesTitle = $"{buttonTitle} {GetButtonTranslation("bundles")}";
                AddElement(new Button(bundlesTitle, bundlesTitle)
                {
                    OnLeftClicked = () => { Game1.activeClickableMenu = new JunimoNoteMenu(true); }
                });
            }

            var sewingTitle = $"{buttonTitle} {GetButtonTranslation("sewing")}";
            AddElement(new Button(sewingTitle, sewingTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new TailoringMenu(); }
            });

            var dyeTitle = $"{buttonTitle} {GetButtonTranslation("dye")}";
            AddElement(new Button(dyeTitle, dyeTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new DyeMenu(); }
            });

            var forgeTitle = $"{buttonTitle} {GetButtonTranslation("forge")}";
            AddElement(new Button(forgeTitle, forgeTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new ForgeMenu(); }
            });

            var minesTitle = $"{buttonTitle} {GetButtonTranslation("mines")}";
            AddElement(new Button(minesTitle, minesTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new MineElevatorMenu(); }
            });

            var shipTitle = $"{buttonTitle} {GetButtonTranslation("ship")}";
            AddElement(new Button(shipTitle, shipTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = ShippingBin(); }
            });

            if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade <= 0)
            {
                AddElement(new Button(GetTranslation("quickShop.button.getUpgradedTool"),
                    GetTranslation("quickShop.button.getUpgradedTool"))
                {
                    OnLeftClicked = () =>
                    {
                        if (Game1.player.freeSpotsInInventory() > 0 ||
                            Game1.player.toolBeingUpgraded.Value is GenericTool)
                        {
                            Tool tool = Game1.player.toolBeingUpgraded.Value;
                            Game1.player.toolBeingUpgraded.Value = null;
                            Game1.player.hasReceivedToolUpgradeMessageYet = false;
                            Game1.player.holdUpItemThenMessage(tool);
                            if (tool is GenericTool)
                            {
                                tool.actionWhenClaimed();
                            }
                            else
                            {
                                Game1.player.addItemToInventoryBool(tool);
                            }

                            Game1.exitActiveMenu();
                        }
                        else
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Clint"),
                                Game1.content.LoadString("Data\\ExtraDialogue:Clint_NoInventorySpace",
                                    Game1.player.toolBeingUpgraded.Value.DisplayName));
                        }
                    }
                });
            }

            if (Game1.player.maxItems < 36)
            {
                AddElement(new Button(GetTranslation("quickShop.button.backpackUpgrade"),
                    GetTranslation("quickShop.button.backpackUpgrade"))
                {
                    OnLeftClicked = () => { gameLocation.answerDialogueAction("Backpack_Purchase", null); }
                });
            }

            if (Game1.player.daysUntilHouseUpgrade < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
            {
                if (Game1.player.HouseUpgradeLevel < 3)
                {
                    AddElement(new Button(GetTranslation("quickShop.button.houseUpgrade"),
                        GetTranslation("quickShop.button.houseUpgrade"))
                    {
                        OnLeftClicked = () => { GetMethod(gameLocation, "houseUpgradeAccept").Invoke(); }
                    });
                }
                else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") ||
                          Game1.MasterPlayer.mailReceived.Contains("JojaMember") ||
                          Game1.MasterPlayer.hasCompletedCommunityCenter()) &&
                         new Town().daysUntilCommunityUpgrade <= 0 &&
                         !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                {
                    AddElement(new Button(GetTranslation("quickShop.button.houseUpgrade.communityUpgrade"),
                        GetTranslation("quickShop.button.houseUpgrade.communityUpgrade.description"))
                    {
                        OnLeftClicked = () => { GetMethod(gameLocation, "communityUpgradeAccept").Invoke(); }
                    });
                }
            }
        }

        private string GetButtonTranslation(string key)
        {
            return ModEntry.GetInstance().Helper.Translation.Get("quickShop.button." + key);
        }

        private string GetTranslation(string key)
        {
            return ModEntry.GetInstance().Helper.Translation.Get(key);
        }

        private bool OnSandyShopPurchase(ISalable item, Farmer who, int amount)
        {
            Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Sandy, item,
                amount);
            return false;
        }

        private ItemGrabMenu ShippingBin()
        {
            ItemGrabMenu itemGrabMenu = new ItemGrabMenu(null, true, false,
                Utility.highlightShippableObjects,
                Game1.getFarm().shipItem, "", snapToBottom: true,
                canBeExitedWithKey: true, playRightClickSound: false, context: this);
            itemGrabMenu.initializeUpperRightCloseButton();
            itemGrabMenu.setBackgroundTransparency(false);
            itemGrabMenu.setDestroyItemOnClick(true);
            itemGrabMenu.initializeShippingBin();
            return itemGrabMenu;
        }

        private IReflectedMethod GetMethod(object obj, string name)
        {
            return ModEntry.GetInstance().Helper.Reflection.GetMethod(obj, name);
        }
    }
}