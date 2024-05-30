/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/QuickShop
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using xTile.Dimensions;

namespace QuickShop.Framework.Gui;

public class QuickShopScreen : ScreenGui
{
    public QuickShopScreen() : base("Quick Shop")
    {
        var gameLocation = Game1.game1.instanceGameLocation;
        
        var shops = new List<Shop>
        {
            new("pierreShop", "SeedShop", "Pierre", Shop.ShopType.Normal),
            new("harveyShop", "Hospital", "Harvey", Shop.ShopType.Normal),
            new("gusShop", "Saloon", "Gus", Shop.ShopType.Normal),
            new("robinShop", "Carpenter", "Robin", Shop.ShopType.Normal),
            new("willyShop", "FishShop", "Willy", Shop.ShopType.Normal),
            new("krobusShop", "ShadowShop", "Krobus", Shop.ShopType.Normal),
            new("marnieShop", "AnimalShop", "Marnie", Shop.ShopType.Normal),
            new("travelingCart", "Traveler", "AnyOrNone", Shop.ShopType.Normal),
            new("magicShopBoat", "Festival_NightMarket_DecorationBoat", "AnyOrNone", Shop.ShopType.Normal),
            new("clintShop", "Blacksmith", "Clint", Shop.ShopType.Normal),
            new("jojaMarket", "Joja", "AnyOrNone", Shop.ShopType.Normal),
            new("dwarfShop", "Dwarf", "Dwarf", Shop.ShopType.Normal),
            new("danceOfTheMoonlightJellies", "Festival_DanceOfTheMoonlightJellies_Pierre", "AnyOrNone",
                Shop.ShopType.Festival),
            new("eggFestival", "Festival_EggFestival_Pierre", "AnyOrNone", Shop.ShopType.Festival),
            new("feastOfTheWinterStar", "Festival_FeastOfTheWinterStar_Pierre", "AnyOrNone", Shop.ShopType.Festival),
            new("festivalOfIceTravelingMerchant", "Festival_FestivalOfIce_TravelingMerchant", "AnyOrNone",
                Shop.ShopType.Festival),
            new("flowerDance", "Festival_FlowerDance_Pierre", "AnyOrNone", Shop.ShopType.Festival),
            new("luau", "Festival_Luau_Pierre", "AnyOrNone", Shop.ShopType.Festival),
            new("spiritsEve", "Festival_SpiritsEve_Pierre", "AnyOrNone", Shop.ShopType.Festival),
            new("stardewValleyFair", "Festival_StardewValleyFair_StarTokens", "AnyOrNone", Shop.ShopType.Festival),
            new("volcanoDungeonShop", "VolcanoShop", "VolcanoShop", Shop.ShopType.Normal),
            new("marlonShop", "AdventureShop", "Marlon", Shop.ShopType.Normal),
            new("adventureGuildRecovery", "AdventureGuildRecovery", "Marlon", Shop.ShopType.Normal),
            new("hatShop", "HatMouse", "AnyOrNone", Shop.ShopType.Normal),
            new("movieTheaterShop", "BoxOffice", "AnyOrNone", Shop.ShopType.Normal),
            new("casinoShop", "Casino", "AnyOrNone", Shop.ShopType.Normal),
            new("qiShop", "QiGemShop", "AnyOrNone", Shop.ShopType.Normal),
            new("sandyShop", "Sandy", "Sandy", Shop.ShopType.Normal),
            new("desertTrade", "DesertTrade", "AnyOrNone", Shop.ShopType.Normal),
            new("desertFestival", "DesertFestival_EggShop", "AnyOrNone", Shop.ShopType.Festival),
            new("islandTrade", "IslandTrade", "AnyOrNone", Shop.ShopType.Normal),
            new("resortBar", "ResortBar", "Gus", Shop.ShopType.Normal),
            new("iceCreamStand", "IceCreamStand", "AnyOrNone", Shop.ShopType.Normal),
            new("raccoonShop", "Raccoon", "AnyOrNone", Shop.ShopType.Normal),
            new("booksellerShop", "Bookseller", "AnyOrNone", Shop.ShopType.Normal),
            new("booksellerTrade", "BooksellerTrade", "AnyOrNone", Shop.ShopType.Normal),
            new("concessions", "Concessions", "AnyOrNone", Shop.ShopType.Normal),
            new("petAdoption", "PetAdoption", "AnyOrNone", Shop.ShopType.Normal)
        };

        AddElement(new Label(ModEntry.GetInstance().GetLabelTranslation("shop")));
        AddElement(new Button(ModEntry.GetInstance().GetButtonTranslation("normalShop"),
            ModEntry.GetInstance().GetButtonTranslation("normalShop.description"))
        {
            OnLeftClicked = () =>
            {
                OpenScreenGui(
                    new NormalShopScreen(shops.Where(shop => shop.Type == Shop.ShopType.Normal).ToList()));
            }
        });

        AddElement(new Button(ModEntry.GetInstance().GetButtonTranslation("festivalShop"),
            ModEntry.GetInstance().GetButtonTranslation("festivalShop.description"))
        {
            OnLeftClicked = () =>
            {
                OpenScreenGui(
                    new FestivalShopScreen(shops.Where(shop => shop.Type == Shop.ShopType.Festival).ToList()));
            }
        });
        AddElement(new Label(ModEntry.GetInstance().GetLabelTranslation("utility")));
        if (InventoryPage.ShouldShowJunimoNoteIcon())
        {
            var bundlesTitle = $"{ModEntry.GetInstance().GetButtonTranslation("bundles")}";
            AddElement(new Button(bundlesTitle, bundlesTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new Bundle(); }
            });
        }

        AddElement(new Button(Game1.content.LoadString("Strings\\Buildings:ShippingBin_Name"),
            Game1.content.LoadString("Strings\\Buildings:ShippingBin_Description"))
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = ShippingBin(); }
        });
        AddElement(new Button(Game1.content.LoadString("Strings\\Objects:PrizeTicket_Name"),
            Game1.content.LoadString("Strings\\Objects:PrizeTicket_Description"))
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new PrizeTicketMenu(); }
        });

        if (ModEntry.GetInstance().Config.AllowBuildingAgain || Game1.player.daysUntilHouseUpgrade.Value < 0 &&
            !Game1.IsThereABuildingUnderConstruction())
        {
            AddElement(new Button(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct"))
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new CarpenterMenu("Robin", Game1.getFarm()); }
            });
        }
        
        var animalShopLocation = "";
        var animalShopTileX = 0;
        var animalShopTileY = 0;
        var animalShop = false;
        AddElement(new Button(Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals"))
        {
            OnLeftClicked = () =>
            {
                animalShopTileX = (int)Game1.player.Tile.X;
                animalShopTileY = (int)Game1.player.Tile.Y;
                animalShopLocation = Game1.player.currentLocation.Name;
                animalShop = true;
                Game1.activeClickableMenu = new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock(Game1.getFarm()));
            }
        });
        
        ModEntry.GetInstance().Helper.Events.GameLoop.UpdateTicked += (sender, args) =>
        {
            if (!animalShop) return;
            if (Game1.activeClickableMenu is PurchaseAnimalsMenu) return;
            if (animalShopLocation != Game1.player.currentLocation.Name)
            {
                Game1.warpFarmer(animalShopLocation, animalShopTileX, animalShopTileY, Game1.player.FacingDirection);
            }

            animalShop = false;
        };
        AddElement(new Button(Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes"))
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new GeodeMenu(); }
        });
        AddElement(new Button(Game1.content.LoadString("Strings\\UI:Collections_Letters"))
        {
            OnLeftClicked = () => { gameLocation.performAction("Mailbox", Game1.player, new Location()); }
        });
        AddElement(new Button(Game1.content.LoadString("Strings\\Furniture:Calendar"))
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new Billboard(); }
        });

        var helpWantedTitle = $"{ModEntry.GetInstance().GetButtonTranslation("helpWanted")}";
        AddElement(new Button(helpWantedTitle, helpWantedTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new Billboard(true); }
        });

        var specialOrdersBoardTitle = $"{ModEntry.GetInstance().GetButtonTranslation("specialOrdersBoard")}";
        AddElement(new Button(specialOrdersBoardTitle, specialOrdersBoardTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new SpecialOrdersBoard(); }
        });

        var qiSpecialOrdersBoardTitle = $"{ModEntry.GetInstance().GetButtonTranslation("qiSpecialOrdersBoard")}";
        AddElement(new Button(qiSpecialOrdersBoardTitle, qiSpecialOrdersBoardTitle)
        {
            OnLeftClicked = () => { gameLocation.performAction("QiChallengeBoard", Game1.player, new Location()); }
        });

        if (Game1.player.mailReceived.Contains("JojaMember"))
        {
            var joJaCdTitle = $"{ModEntry.GetInstance().GetButtonTranslation("joJaCD")}";
            AddElement(new Button(joJaCdTitle, joJaCdTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu =
                        new JojaCDMenu(Game1.temporaryContent.Load<Texture2D>("LooseSprites\\JojaCDForm"));
                }
            });
        }

        var wizardBuildingTitle = $"{ModEntry.GetInstance().GetButtonTranslation("wizardBuilding")}";
        AddElement(new Button(wizardBuildingTitle, wizardBuildingTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new CarpenterMenu("Wizard", Game1.getFarm()); }
        });

        var changeAppearanceTitle = $"{ModEntry.GetInstance().GetButtonTranslation("changeAppearance")}";
        AddElement(new Button(changeAppearanceTitle, changeAppearanceTitle)
        {
            OnLeftClicked = () =>
            {
                gameLocation.createQuestionDialogue(
                    Game1.content.LoadString("Strings\\Locations:WizardTower_WizardShrine").Replace('\n', '^'),
                    gameLocation.createYesNoResponses(), "WizardShrine");
            }
        });
        var changeProfessionsTitle = $"{ModEntry.GetInstance().GetButtonTranslation("changeProfessions")}";
        AddElement(new Button(changeProfessionsTitle, changeProfessionsTitle)
        {
            OnLeftClicked = () =>
            {
                gameLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatue"),
                    gameLocation.createYesNoResponses(), "dogStatue");
            }
        });
        AddElement(new Button(Game1.content.LoadString("Strings\\BigCraftables:SewingMachine_Name"),
            Game1.content.LoadString("Strings\\BigCraftables:SewingMachine_Description"))
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new TailoringMenu(); }
        });

        var dyeTitle = $"{ModEntry.GetInstance().GetButtonTranslation("dye")}";
        AddElement(new Button(dyeTitle, dyeTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new DyeMenu(); }
        });

        AddElement(new Button(Game1.content.LoadString("Strings\\1_6_Strings:MiniForge_Name"),
            Game1.content.LoadString("Strings\\1_6_Strings:MiniForge_Description"))
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new ForgeMenu(); }
        });

        var minesTitle = $"{ModEntry.GetInstance().GetButtonTranslation("mines")}";
        AddElement(new Button(minesTitle, minesTitle)
        {
            OnLeftClicked = () => { gameLocation.performAction("MineElevator", Game1.player, new Location()); }
        });

        if (ModEntry.GetInstance().Config.AllowToolUpgradeAgain || (Game1.player.toolBeingUpgraded.Value == null &&
                                                                    Game1.player.daysLeftForToolUpgrade.Value <= 0))
        {
            AddElement(new Button(Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade"))
            {
                OnLeftClicked = () => { Utility.TryOpenShopMenu("ClintUpgrade", "Clint"); }
            });
        }

        if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value <= 0)
        {
            var getUpgradedToolTitle = ModEntry.GetInstance().GetButtonTranslation("getUpgradedTool");
            AddElement(new Button(getUpgradedToolTitle,
                getUpgradedToolTitle)
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
                        Game1.DrawDialogue(Game1.getCharacterFromName("Clint"),
                            Game1.content.LoadString("Data\\ExtraDialogue:Clint_NoInventorySpace",
                                Game1.player.toolBeingUpgraded.Value.DisplayName));
                    }
                }
            });
        }

        if (Game1.player.maxItems.Value < 36)
        {
            AddElement(new Button(Game1.player.maxItems.Value == 12
                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8708")
                    : Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8709"),
                Game1.player.maxItems.Value == 12
                    ? Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question24")
                    : Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question36"))
            {
                OnLeftClicked = () => { gameLocation.performAction("BuyBackpack", Game1.player, new Location()); }
            });
        }

        if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
        {
            if (Game1.player.HouseUpgradeLevel < 3)
            {
                AddElement(new Button(
                    Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse"))
                {
                    OnLeftClicked = () => { GetMethod(gameLocation, "houseUpgradeOffer").Invoke(); }
                });
            }
            else if (Game1.player.HouseUpgradeLevel >= 2)
            {
                AddElement(new Button(
                    Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse"))
                {
                    OnLeftClicked = HouseRenovation.ShowRenovationMenu
                });
            }
            else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") ||
                      Game1.MasterPlayer.mailReceived.Contains("JojaMember") ||
                      Game1.MasterPlayer.hasCompletedCommunityCenter()) &&
                     new Town().daysUntilCommunityUpgrade.Value <= 0 &&
                     !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
            {
                AddElement(new Button(
                    Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade"))
                {
                    OnLeftClicked = () => { GetMethod(gameLocation, "communityUpgradeOffer").Invoke(); }
                });
            }
        }

        if (Game1.player.isMarriedOrRoommates())
        {
            var divorceTranslation = ModEntry.GetInstance().GetButtonTranslation("divorce");
            AddElement(new Button(divorceTranslation, divorceTranslation)
            {
                OnLeftClicked = () =>
                {
                    gameLocation.createQuestionDialogue(
                        Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_Question"),
                        gameLocation.createYesNoResponses(), "divorce");
                }
            });
        }
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

    private class Bundle : JunimoNoteMenu
    {
        public Bundle(int area = 1) : base(false,
            Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible") &&
            !Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater")
                ? 6
                : area)
        {
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            areaNextButton.draw(b);
            areaBackButton.draw(b);
            drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (var bundle in bundles)
            {
                bundle.depositsAllowed = true;
            }

            base.receiveLeftClick(x, y, playSound);
            if (areaNextButton.containsPoint(x, y))
            {
                SwapPage(1);
            }
            else if (areaBackButton.containsPoint(x, y))
            {
                SwapPage(-1);
            }

            if (areaNextButton.containsPoint(x, y) || areaBackButton.containsPoint(x, y))
            {
                if (Game1.activeClickableMenu is JunimoNoteMenu junimoNoteMenu)
                {
                    Game1.activeClickableMenu = new Bundle(junimoNoteMenu.whichArea);
                }
            }
        }
    }
}