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

namespace QuickShop.Framework.Gui;

public class QuickShopScreen : ScreenGui
{
    public QuickShopScreen()
    {
        var gameLocation = Game1.game1.instanceGameLocation;

        var pierreShopTitle = $"{GetButtonTranslation("pierreShop")}";
        AddElement(new Button(pierreShopTitle, pierreShopTitle)
        {
            OnLeftClicked = () =>
            {
                if (Game1.getLocationFromName("SeedShop") is SeedShop)
                {
                    Utility.TryOpenShopMenu("SeedShop", "Pierre");
                }
            }
        });

        var harveyShopTitle = $"{GetButtonTranslation("harveyShop")}";

        AddElement(new Button(harveyShopTitle, harveyShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Hospital", "Harvey"); }
        });
        var gusShopTitle = $"{GetButtonTranslation("gusShop")}";
        AddElement(new Button(gusShopTitle, gusShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Saloon", "Gus"); }
        });

        var robinShopTitle = $"{GetButtonTranslation("robinShop")}";
        AddElement(new Button(robinShopTitle, robinShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Carpenter", "Robin"); }
        });
        //
        var carpenterBuildingTitle = $"{GetButtonTranslation("carpenterBuilding")}";
        AddElement(new Button(carpenterBuildingTitle, carpenterBuildingTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new CarpenterMenu("Robin", Game1.getFarm()); }
        });

        var willyShopTitle = $"{GetButtonTranslation("willyShop")}";
        AddElement(new Button(willyShopTitle, willyShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("FishShop", "Willy"); }
        });

        var krobusShopTitle = $"{GetButtonTranslation("krobusShop")}";
        AddElement(new Button(krobusShopTitle, krobusShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("ShadowShop", "Krobus"); }
        });

        var marnieShopTitle = $"{GetButtonTranslation("marnieShop")}";
        AddElement(new Button(marnieShopTitle, marnieShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("AnimalShop", "Marnie"); }
        });
        //
        var animalShopTitle = $"{GetButtonTranslation("animalShop")}";
        AddElement(new Button(animalShopTitle, animalShopTitle)
        {
            OnLeftClicked = () =>
            {
                Game1.activeClickableMenu =
                    new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock(Game1.getFarm()));
            }
        });

        var merchantShopTitle = $"{GetButtonTranslation("merchantShop")}";
        AddElement(new Button(merchantShopTitle, merchantShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Traveler", "TravelerNightMarket"); }
        });

        var magicShopBoatTitle = $"{GetButtonTranslation("magicShopBoat")}";
        AddElement(new Button(magicShopBoatTitle, magicShopBoatTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Festival_NightMarket_DecorationBoat", "magicBoatShop"); }
        });

        for (var i = 1; i <= 3; i++)
        {
            var decorationBoatShopTitle = $"{GetButtonTranslation("decorationBoatShop")} {i}";
            var finalI = i;
            AddElement(new Button(decorationBoatShopTitle, decorationBoatShopTitle)
            {
                OnLeftClicked = () =>
                {
                    Utility.TryOpenShopMenu($"Festival_NightMarket_MagicBoat_Day{finalI}", "BlueBoat");
                }
            });
        }

        var renovationTitle = $"{GetButtonTranslation("renovation")}";
        AddElement(new Button(renovationTitle, renovationTitle)
        {
            OnLeftClicked = HouseRenovation.ShowRenovationMenu
        });

        var clintShopTitle = $"{GetButtonTranslation("clintShop")}";
        AddElement(new Button(clintShopTitle, clintShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Blacksmith", "Clint"); }
        });

        if (Game1.player.toolBeingUpgraded.Value == null)
        {
            var upgradeTitle = $"{GetButtonTranslation("upgrade")}";
            AddElement(new Button(upgradeTitle, upgradeTitle)
            {
                OnLeftClicked = () => { Utility.TryOpenShopMenu("ClintUpgrade", "Clint"); }
            });
        }

        var geodeTitle = $"{GetButtonTranslation("geode")}";
        AddElement(new Button(geodeTitle, geodeTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new GeodeMenu(); }
        });

        var mailboxTitle = $"{GetButtonTranslation("mailbox")}";
        AddElement(new Button(mailboxTitle, mailboxTitle)
        {
            OnLeftClicked = () => { gameLocation.mailbox(); }
        });

        var calendarTitle = $"{GetButtonTranslation("calendar")}";
        AddElement(new Button(calendarTitle, calendarTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new Billboard(); }
        });

        var helpWantedTitle = $"{GetButtonTranslation("helpWanted")}";
        AddElement(new Button(helpWantedTitle, helpWantedTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new Billboard(true); }
        });

        var specialOrdersBoardTitle = $"{GetButtonTranslation("specialOrdersBoard")}";
        AddElement(new Button(specialOrdersBoardTitle, specialOrdersBoardTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new SpecialOrdersBoard(); }
        });

        var morrisShopTitle = $"{GetButtonTranslation("jojaMarket")}";
        AddElement(new Button(morrisShopTitle, morrisShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Joja", "Joja"); }
        });

        var dwarfShopTitle = $"{GetButtonTranslation("dwarfShop")}";
        AddElement(new Button(dwarfShopTitle, dwarfShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Dwarf", "Dwarf"); }
        });

        var volcanoDungeonShopTitle = $"{GetButtonTranslation("volcanoDungeonShop")}";
        AddElement(new Button(volcanoDungeonShopTitle, volcanoDungeonShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("VolcanoShop", "VolcanoShop"); }
        });

        var marlonShopTitle = $"{GetButtonTranslation("marlonShop")}";
        AddElement(new Button(marlonShopTitle, marlonShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("AdventureShop", "Marlon"); }
        });
        var adventureGuildRecoveryTitle = $"{GetButtonTranslation("adventureGuildRecovery")}";
        AddElement(new Button(adventureGuildRecoveryTitle, adventureGuildRecoveryTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("AdventureGuildRecovery", "AdventureGuildRecovery"); }
        });
        var hatShopTitle = $"{GetButtonTranslation("hatShop")}";
        AddElement(new Button(hatShopTitle, hatShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("HatMouse", "HatMouse"); }
        });

        var movieTheaterShopTitle = $"{GetButtonTranslation("movieTheaterShop")}";
        AddElement(new Button(movieTheaterShopTitle, movieTheaterShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("BoxOffice", "BoxOffice"); }
        });


        var casinoShopTitle = $"{GetButtonTranslation("casinoShop")}";
        AddElement(new Button(casinoShopTitle, casinoShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Casino", "MrQi"); }
        });

        var qiShopTitle = $"{GetButtonTranslation("qiShop")}";
        AddElement(new Button(qiShopTitle, qiShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("QiGemShop", "MrQi"); }
        });

        var qiSpecialOrdersBoardTitle = $"{GetButtonTranslation("qiSpecialOrdersBoard")}";
        AddElement(new Button(qiSpecialOrdersBoardTitle, qiSpecialOrdersBoardTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new SpecialOrdersBoard("Qi"); }
        });

        var sandyShopTitle = $"{GetButtonTranslation("sandyShop")}";
        AddElement(new Button(sandyShopTitle, sandyShopTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Sandy", "Sandy"); }
        });

        // var desertShopTitle = $"{GetButtonTranslation("desertShop")}";
        // AddElement(new Button(desertShopTitle, desertShopTitle)
        // {
        //     OnLeftClicked = () =>
        //     {
        //         // Game1.activeClickableMenu = new ShopMenu(Desert.getDesertMerchantTradeStock(Game1.player));
        //     }
        // });

        var islandTradeTitle = $"{GetButtonTranslation("islandTrade")}";
        AddElement(new Button(islandTradeTitle, islandTradeTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("IslandTrade", "IslandTrade"); }
        });

        var resortBarTitle = $"{GetButtonTranslation("resortBar")}";
        AddElement(new Button(resortBarTitle, resortBarTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("ResortBar", "Gus"); }
        });


        if (Game1.player.mailReceived.Contains("JojaMember"))
        {
            var joJaCdTitle = $"{GetButtonTranslation("joJaCD")}";
            AddElement(new Button(joJaCdTitle, joJaCdTitle)
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu =
                        new JojaCDMenu(Game1.temporaryContent.Load<Texture2D>("LooseSprites\\JojaCDForm"));
                }
            });
        }

        var iceCreamStandTitle = $"{GetButtonTranslation("iceCreamStand")}";
        AddElement(new Button(iceCreamStandTitle, iceCreamStandTitle)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("IceCreamStand", "IceCreamStand"); }
        });
        var wizardBuildingTitle = $"{GetButtonTranslation("wizardBuilding")}";
        AddElement(new Button(wizardBuildingTitle, wizardBuildingTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new CarpenterMenu("Wizard", Game1.getFarm()); }
        });

        var changeAppearanceTitle = $"{GetButtonTranslation("changeAppearance")}";
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
            var bundlesTitle = $"{GetButtonTranslation("bundles")}";
            AddElement(new Button(bundlesTitle, bundlesTitle)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new JunimoNoteMenu(true); }
            });
        }

        var sewingTitle = $"{GetButtonTranslation("sewing")}";
        AddElement(new Button(sewingTitle, sewingTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new TailoringMenu(); }
        });

        var dyeTitle = $"{GetButtonTranslation("dye")}";
        AddElement(new Button(dyeTitle, dyeTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new DyeMenu(); }
        });

        var forgeTitle = $"{GetButtonTranslation("forge")}";
        AddElement(new Button(forgeTitle, forgeTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new ForgeMenu(); }
        });

        var minesTitle = $"{GetButtonTranslation("mines")}";
        AddElement(new Button(minesTitle, minesTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = new MineElevatorMenu(); }
        });

        var shipTitle = $"{GetButtonTranslation("ship")}";
        AddElement(new Button(shipTitle, shipTitle)
        {
            OnLeftClicked = () => { Game1.activeClickableMenu = ShippingBin(); }
        });

        if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value <= 0)
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
                        Game1.DrawDialogue(Game1.getCharacterFromName("Clint"),
                            Game1.content.LoadString("Data\\ExtraDialogue:Clint_NoInventorySpace",
                                Game1.player.toolBeingUpgraded.Value.DisplayName));
                    }
                }
            });
        }

        if (Game1.player.maxItems.Value < 36)
        {
            AddElement(new Button(GetTranslation("quickShop.button.backpackUpgrade"),
                GetTranslation("quickShop.button.backpackUpgrade"))
            {
                OnLeftClicked = () => { gameLocation.answerDialogueAction("Backpack_Purchase", null); }
            });
        }

        if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
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
                     new Town().daysUntilCommunityUpgrade.Value <= 0 &&
                     !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
            {
                AddElement(new Button(GetTranslation("quickShop.button.houseUpgrade.communityUpgrade"),
                    GetTranslation("quickShop.button.houseUpgrade.communityUpgrade.description"))
                {
                    OnLeftClicked = () => { GetMethod(gameLocation, "communityUpgradeAccept").Invoke(); }
                });
            }
        }

        var raccoon = $"{GetButtonTranslation("raccoonShop")}";
        AddElement(new Button(raccoon, raccoon)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Raccoon", "Raccoon"); }
        });
        var booksellerTrade = $"{GetButtonTranslation("booksellerTrade")}";
        AddElement(new Button(booksellerTrade, booksellerTrade)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("BooksellerTrade", "BooksellerTrade"); }
        });
        var concessions = $"{GetButtonTranslation("concessions")}";
        AddElement(new Button(concessions, concessions)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("Concessions", "Concessions"); }
        });
        var petAdoption = $"{GetButtonTranslation("petAdoption")}";
        AddElement(new Button(petAdoption, petAdoption)
        {
            OnLeftClicked = () => { Utility.TryOpenShopMenu("PetAdoption", "PetAdoption"); }
        });
    }

    private string GetButtonTranslation(string key)
    {
        return ModEntry.GetInstance().Helper.Translation.Get("quickShop.button." + key);
    }

    private string GetTranslation(string key)
    {
        return ModEntry.GetInstance().Helper.Translation.Get(key);
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