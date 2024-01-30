/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/evfredericksen/StardewSpeak
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using static StardewValley.Menus.CoopMenu;

namespace StardewSpeak
{
    public static class Utils
    {
        public static string TrackingIdKey = $"{ModEntry.helper.ModRegistry.ModID}/trackingId";

        public static bool IsTileHoeable(GameLocation location, int x, int y)
        {
            var tile = new Vector2(x, y);
            if (location.terrainFeatures.ContainsKey(tile) || location.objects.ContainsKey(tile)) return false;
            return location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") != null;
        }

        public static bool isOnScreen(Vector2 positionTile, int acceptableDistanceFromScreenNonTile, GameLocation location = null)
        {
            if (location != null && !location.Equals(Game1.currentLocation))
            {
                return false;
            }
            if (positionTile.X * 64 > Game1.viewport.X - acceptableDistanceFromScreenNonTile && positionTile.X * 64 < Game1.viewport.X + Game1.viewport.Width + acceptableDistanceFromScreenNonTile && positionTile.Y * 64 > Game1.viewport.Y - acceptableDistanceFromScreenNonTile)
            {
                return positionTile.Y * 64 < Game1.viewport.Y + Game1.viewport.Height + acceptableDistanceFromScreenNonTile;
            }
            return false;
        }

        public static dynamic RectangleToClickableComponent(Rectangle rect, Point mousePosition) 
        {
            bool containsMouse = rect.Contains(mousePosition.X, mousePosition.Y);
            return new
            {
                type = "clickableComponent",
                bounds = new { x = rect.X, y = rect.Y, width = rect.Width, height = rect.Height },
                center = new List<int> { rect.Center.X, rect.Center.Y },
                name = "",
                containsMouse,
                visible = true,
                rect,
            };
        }

        public static dynamic TileToClickableComponent(int x, int y, Point mousePosition)
        {
            int posX = x * 64 - Game1.viewport.X;
            int posY = y * 64 - Game1.viewport.Y;
            Rectangle rect = new Rectangle(posX, posY, 64, 64);
            return RectangleToClickableComponent(rect, mousePosition);
        }
        public static bool GetClosestAnimal(Vector2 positionTile, int acceptableDistanceFromScreenNonTile, GameLocation location = null)
        {
            if (location != null && !location.Equals(Game1.currentLocation))
            {
                return false;
            }
            if (positionTile.X * 64 > Game1.viewport.X - acceptableDistanceFromScreenNonTile && positionTile.X * 64 < Game1.viewport.X + Game1.viewport.Width + acceptableDistanceFromScreenNonTile && positionTile.Y * 64 > Game1.viewport.Y - acceptableDistanceFromScreenNonTile)
            {
                return positionTile.Y * 64 < Game1.viewport.Y + Game1.viewport.Height + acceptableDistanceFromScreenNonTile;
            }
            return false;
        }
        public static void WriteJson(string fname, object obj)
        {
        }
        public static dynamic Merge(object item1, object item2)
        {
            if (item1 == null || item2 == null)
                return item1 ?? item2 ?? new ExpandoObject();

            dynamic expando = new ExpandoObject();
            var result = expando as IDictionary<string, object>;
            foreach (var kvp in IterObject(item1)) 
            {
                result[kvp.key] = kvp.value;
            }
            foreach (var kvp in IterObject(item2))
            {
                result[kvp.key] = kvp.value;
            }
            return result;
        }

        public static List<dynamic> IterObject(dynamic obj) {
            var items = new List<dynamic>();
            if (obj is ExpandoObject)
            {
                var exp = obj as ExpandoObject;
                foreach (var kvp in exp)
                {
                    items.Add(new { key = kvp.Key, value = kvp.Value });
                }
            }
            else 
            {
                foreach (System.Reflection.PropertyInfo fi in obj.GetType().GetProperties())
                {
                    items.Add(new { 
                        key = fi.Name, 
                        value = fi.GetValue(obj, null)
                    });
                }
            }
            return items;
        }

        public static object SerializeMenu(IClickableMenu menu)
        {
            Point mousePosition = Game1.getMousePosition();
            return SerializeMenu(menu, mousePosition);
        }
        public static object SerializeMenu(IClickableMenu menu, Point mousePosition)
        {
            dynamic serialized = Menus.SerializeMenu(menu, mousePosition);
            if (serialized != null) return serialized;
            if (menu == null) return null;
            Rectangle menuRect = new Rectangle(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height);
            bool containsMouse = menu.isWithinBounds(mousePosition.X, mousePosition.Y);
            var menuBarObj = new
            {
                menu.xPositionOnScreen,
                menu.yPositionOnScreen, 
                upperRightCloseButton = Utils.SerializeClickableCmp(menu.upperRightCloseButton, mousePosition),
                containsMouse,
                menuType = "unknown",
                classType = menu.GetType().ToString(),
            };
            dynamic menuTypeObj = new { };
            if (menu is ShopMenu)
            {
                var sm = menu as ShopMenu;
                var forSale = sm.forSale.Select(x => Utils.SerializeItem((Item)x));
                var forSaleButtons = Utils.SerializeComponentList(sm.forSaleButtons, mousePosition);
                menuTypeObj = new
                {
                    menuType = "shopMenu",
                    forSale,
                    forSaleButtons,
                    sm.currentItemIndex,
                    inventory = SerializeMenu(sm.inventory),
                    upArrow = SerializeClickableCmp(sm.upArrow, mousePosition),
                    downArrow = SerializeClickableCmp(sm.downArrow, mousePosition),
                    scrollBar = SerializeClickableCmp(sm.scrollBar, mousePosition),
                };
            }
            else if (menu is ProfileMenu)
            {
                var pm = menu as ProfileMenu;
                var clickableProfileItems = Utils.SerializeComponentList(pm.clickableProfileItems, mousePosition);
                menuTypeObj = new
                {
                    menuType = "profileMenu",
                    backButton = Utils.SerializeClickableCmp(pm.backButton, mousePosition),
                    forwardButton = Utils.SerializeClickableCmp(pm.forwardButton, mousePosition),
                    previousCharacterButton = Utils.SerializeClickableCmp(pm.previousCharacterButton, mousePosition),
                    nextCharacterButton = Utils.SerializeClickableCmp(pm.nextCharacterButton, mousePosition),
                    clickableProfileItems,
                    upArrow = SerializeClickableCmp(pm.upArrow, mousePosition),
                    downArrow = SerializeClickableCmp(pm.downArrow, mousePosition),
                };
            }
            else if (menu is DialogueBox)
            {
                var db = menu as DialogueBox;
                var responseCC = SerializeComponentList(db.responseCC, mousePosition);
                var responses = db.responses.Select(x => new { x.responseKey, x.responseText, x.hotkey }).ToList();
                menuTypeObj = new
                {
                    menuType = "dialogueBox",
                    responseCC,
                    responses,
                };
            }
            else if (menu is CoopMenu)
            {
                var cb = menu as CoopMenu;
                var slotButtons = SerializeComponentList(cb.slotButtons, mousePosition);
                String currentTab = Utils.GetPrivateField(cb, "currentTab").ToString();
                menuTypeObj = new
                {
                    menuType = "coopMenu",
                    downArrow = SerializeClickableCmp(cb.downArrow, mousePosition),
                    hostTab = SerializeClickableCmp(cb.hostTab, mousePosition),
                    joinTab = SerializeClickableCmp(cb.joinTab, mousePosition),
                    refreshButton = SerializeClickableCmp(cb.refreshButton, mousePosition),
                    upArrow = SerializeClickableCmp(cb.upArrow, mousePosition),
                    slotButtons,
                    currentTab,
                };
                if (currentTab == "JOIN_TAB") 
                {
                
                }
                else if (currentTab == "HOST_TAB")
                {

                }
            }
            else if (menu is TitleTextInputMenu)
            {
                var ttim = menu as TitleTextInputMenu;
                menuTypeObj = new
                {
                    menuType = "titleTextInputMenu",
                    doneNamingButton = SerializeClickableCmp(ttim.doneNamingButton, mousePosition),
                    pasteButton = SerializeClickableCmp(ttim.pasteButton, mousePosition),
                    textBoxCC = SerializeClickableCmp(ttim.textBoxCC, mousePosition),

                };
            }
            else if (menu is AnimalQueryMenu)
            {
                var aqm = menu as AnimalQueryMenu;
                bool movingAnimal = (bool)Utils.GetPrivateField(aqm, "movingAnimal");
                menuTypeObj = new
                {
                    menuType = "animalQueryMenu",
                    movingAnimal,
                };
                if (movingAnimal)
                {
                    var vt = VisibleTiles(Game1.getFarm());
                    var tileComponents = vt.Select(t => TileToClickableComponent(t[0], t[1], mousePosition)).ToList();
                    var okButton = SerializeClickableCmp(aqm.okButton, mousePosition);
                    menuTypeObj = Merge(menuTypeObj, new { tileComponents, okButton });
                }
                else
                {
                    bool confirmingSell = (bool)Utils.GetPrivateField(aqm, "confirmingSell");
                    if (confirmingSell)
                    {
                        var noButton = SerializeClickableCmp(aqm.noButton, mousePosition);
                        var yesButton = SerializeClickableCmp(aqm.yesButton, mousePosition);
                        menuTypeObj = Merge(menuTypeObj, new { yesButton, noButton });
                    }
                    else
                    {
                        menuTypeObj = Merge(menuTypeObj, new
                        {
                            allowReproductionButton = SerializeClickableCmp(aqm.allowReproductionButton, mousePosition),
                            sellButton = SerializeClickableCmp(aqm.sellButton, mousePosition),
                            textBoxCC = SerializeClickableCmp(aqm.textBoxCC, mousePosition),
                            moveHomeButton = SerializeClickableCmp(aqm.moveHomeButton, mousePosition),
                            okButton = SerializeClickableCmp(aqm.okButton, mousePosition),

                        });
                    }
                }
            }
            else if (menu is MineElevatorMenu)
            {
                var mem = menu as MineElevatorMenu;
                var elevators = SerializeComponentList(mem.elevators, mousePosition);
                menuTypeObj = new
                {
                    menuType = "mineElevatorMenu",
                    elevators,
                };
            }
            else if (menu is LetterViewerMenu)
            {
                var lvm = menu as LetterViewerMenu;
                var itemsToGrab = SerializeComponentList(lvm.itemsToGrab, mousePosition);
                var acceptQuestButton = SerializeClickableCmp(lvm.acceptQuestButton, mousePosition);
                var backButton = SerializeClickableCmp(lvm.backButton, mousePosition);
                var forwardButton = SerializeClickableCmp(lvm.forwardButton, mousePosition);
                menuTypeObj = new
                {
                    menuType = "letterViewerMenu",
                    acceptQuestButton,
                    backButton,
                    forwardButton,
                    itemsToGrab,
                };
            }
            else if (menu is InventoryMenu)
            {
                var im = menu as InventoryMenu;
                menuTypeObj = new
                {
                    menuType = "inventoryMenu",
                    inventory = SerializeComponentList(im.inventory, mousePosition),
                    im.rows,
                    im.capacity,
                };
            }
            else if (menu is CarpenterMenu)
            {
                var cm = menu as CarpenterMenu;
                bool onFarm = (bool)Utils.GetPrivateField(cm, "onFarm");
                menuTypeObj = new
                {
                    menuType = "carpenterMenu",
                    cancelButton = SerializeClickableCmp(cm.cancelButton, mousePosition),
                    onFarm,
                };
                if (onFarm)
                {
                    var vt = VisibleTiles(Game1.getFarm());
                    var tileComponents = vt.Select(t => TileToClickableComponent(t[0], t[1], mousePosition)).ToList();
                    menuTypeObj = Merge(menuTypeObj, new { tileComponents });
                }
                else 
                {
                    menuTypeObj = Merge(menuTypeObj, new
                    {
                        backButton = SerializeClickableCmp(cm.backButton, mousePosition),
                        demolishButton = SerializeClickableCmp(cm.demolishButton, mousePosition),
                        forwardButton = SerializeClickableCmp(cm.forwardButton, mousePosition),
                        moveButton = SerializeClickableCmp(cm.moveButton, mousePosition),
                        okButton = SerializeClickableCmp(cm.okButton, mousePosition),
                        paintButton = SerializeClickableCmp(cm.paintButton, mousePosition),
                        upgradeIcon = SerializeClickableCmp(cm.upgradeIcon, mousePosition),
                    });
                }
            }
            else if (menu is TitleMenu)
            {
                var tm = menu as TitleMenu;

                menuTypeObj = new
                {
                    menuType = "titleMenu",
                    windowedButton = SerializeClickableCmp(tm.windowedButton, mousePosition),
                };
                if (TitleMenu.subMenu != null)
                {
                    bool addBackButton = tm.backButton != null && !(TitleMenu.subMenu is CharacterCustomization && !TitleMenu.subMenu.readyToClose());
                    dynamic backButton = addBackButton ? SerializeClickableCmp(tm.backButton, mousePosition) : null;
                    var subMenu = Merge(SerializeMenu(TitleMenu.subMenu), new { backButton });
                    menuTypeObj = Merge(menuTypeObj, new { subMenu });
                }
                else 
                {
                    dynamic subMenu = null;
                    menuTypeObj = Merge(menuTypeObj, new
                    {
                        subMenu,
                        buttons = SerializeComponentList(tm.buttons, mousePosition),
                        languageButton = SerializeClickableCmp(tm.languageButton, mousePosition),
                        aboutButton = SerializeClickableCmp(tm.aboutButton, mousePosition),
                        muteMusicButton = SerializeClickableCmp(tm.muteMusicButton, mousePosition),
                    });
                }
            }
            else if (menu is CharacterCustomization)
            {
                var ccm = menu as CharacterCustomization;
                menuTypeObj = new
                {
                    menuType = "characterCustomizationMenu",
                };
                if (ccm.showingCoopHelp)
                {
                    menuTypeObj = Merge(menuTypeObj, new
                    {
                        okButton = SerializeClickableCmp(ccm.coopHelpOkButton, mousePosition),
                        coopHelpLeftButton = SerializeClickableCmp(ccm.coopHelpLeftButton, mousePosition),
                        coopHelpRightButton = SerializeClickableCmp(ccm.coopHelpRightButton, mousePosition),
                    });
                }
                else 
                {
                    menuTypeObj = Merge(menuTypeObj, new
                    {
                        advancedOptionsButton = SerializeClickableCmp(ccm.advancedOptionsButton, mousePosition),
                        backButton = SerializeClickableCmp(ccm.backButton, mousePosition),
                        cabinLayoutButtons = SerializeComponentList(ccm.cabinLayoutButtons, mousePosition),
                        coopHelpButton = SerializeClickableCmp(ccm.coopHelpButton, mousePosition),
                        farmTypeButtons = SerializeComponentList(ccm.farmTypeButtons, mousePosition),
                        favThingBoxCC = SerializeClickableCmp(ccm.favThingBoxCC, mousePosition),
                        farmnameBoxCC = SerializeClickableCmp(ccm.farmnameBoxCC, mousePosition),
                        genderButtons = SerializeComponentList(ccm.genderButtons, mousePosition),
                        leftSelectionButtons = SerializeComponentList(ccm.leftSelectionButtons, mousePosition),
                        nameBoxCC = SerializeClickableCmp(ccm.nameBoxCC, mousePosition),
                        okButton = SerializeClickableCmp(ccm.okButton, mousePosition),
                        petButtons = SerializeComponentList(ccm.petButtons, mousePosition),
                        randomButton = SerializeClickableCmp(ccm.randomButton, mousePosition),
                        rightSelectionButtons = SerializeComponentList(ccm.rightSelectionButtons, mousePosition),
                        skipIntroButton = SerializeClickableCmp(ccm.skipIntroButton, mousePosition),
                    });
                }
            }
            else if (menu is QuestLog)
            {
                var qlm = menu as QuestLog;
                StardewValley.Quests.IQuest shownQuest = GetPrivateField(qlm, "_shownQuest");
                int currentPage = GetPrivateField(qlm, "currentPage");
                int questPage = GetPrivateField(qlm, "questPage");
                float contentHeight = GetPrivateField(qlm, "_contentHeight");
                float scissorRectHeight = GetPrivateField(qlm, "_scissorRectHeight");
                float scrollAmount = GetPrivateField(qlm, "scrollAmount");

                List<List<StardewValley.Quests.IQuest>> pages = GetPrivateField(qlm, "pages");
                menuTypeObj = new { menuType = "questLogMenu" };
                if (questPage == -1)
                {
                    if (pages.Count > 0 && pages[currentPage].Count > 0)
                        menuTypeObj = Merge(menuTypeObj, new { questLogButtons = SerializeComponentList(qlm.questLogButtons, mousePosition) });
                    if (currentPage < pages.Count - 1)
                        menuTypeObj = Merge(menuTypeObj, new { forwardButton = SerializeClickableCmp(qlm.forwardButton, mousePosition) });
                    if (currentPage > 0)
                        menuTypeObj = Merge(menuTypeObj, new { backButton = SerializeClickableCmp(qlm.backButton, mousePosition) });
                }
                else
                {
                    var quest = shownQuest as StardewValley.Quests.Quest;
                    bool needsScroll = qlm.NeedsScroll();
                    if (questPage != -1 && shownQuest.ShouldDisplayAsComplete() && shownQuest.HasMoneyReward())
                        menuTypeObj = Merge(menuTypeObj, new { rewardBox = SerializeClickableCmp(qlm.rewardBox, mousePosition) });
                    if (questPage != -1 && quest != null && !quest.completed && (bool)quest.canBeCancelled)
                        menuTypeObj = Merge(menuTypeObj, new { cancelQuestButton = SerializeClickableCmp(qlm.cancelQuestButton, mousePosition) });
                    if (needsScroll) 
                    {
                        if (scrollAmount < contentHeight - scissorRectHeight)
                            menuTypeObj = Merge(menuTypeObj, new { downArrow = SerializeClickableCmp(qlm.downArrow, mousePosition) });
                        else if (scrollAmount > 0f)
                            menuTypeObj = Merge(menuTypeObj, new { upArrow = SerializeClickableCmp(qlm.upArrow, mousePosition) });
                    }
                    else 
                    {
                        menuTypeObj = Merge(menuTypeObj, new { backButton = SerializeClickableCmp(qlm.backButton, mousePosition) });
                    }
                }
                //backButton = SerializeClickableCmp(qlm.backButton, mousePosition),
                //downArrow = SerializeClickableCmp(qlm.downArrow, mousePosition),
                //forwardButton = SerializeClickableCmp(qlm.forwardButton, mousePosition),
                //questLogButtons = SerializeComponentList(qlm.questLogButtons, mousePosition),
                //upArrow = SerializeClickableCmp(qlm.upArrow, mousePosition),
                //rewardBox = SerializeClickableCmp(qlm.rewardBox, mousePosition),

            }
            else if (menu is LanguageSelectionMenu)
            {
                var lsm = menu as LanguageSelectionMenu;
                menuTypeObj = new
                {
                    menuType = "languageSelectionMenu",
                    languages = SerializeComponentList(lsm.languages, mousePosition),
                };
            }
            else if (menu is LevelUpMenu)
            {
                var lum = menu as LevelUpMenu;
                menuTypeObj = new
                {
                    menuType = "levelUpMenu",
                };
                if (lum.isProfessionChooser) 
                {
                    menuTypeObj = Merge(menuTypeObj, new {
                        leftProfession = SerializeClickableCmp(lum.leftProfession, mousePosition),
                        rightProfession = SerializeClickableCmp(lum.rightProfession, mousePosition),
                    });
                }
                else
                {
                    menuTypeObj = Merge(menuTypeObj, new
                    {
                        okButton = SerializeClickableCmp(lum.okButton, mousePosition)
                    });
                }
            }
            else if (menu is LoadGameMenu)
            {
                var lgm = menu as LoadGameMenu;
                int currentItemIndex = (int)GetPrivateField(lgm, "currentItemIndex");
                menuTypeObj = new
                {
                    menuType = "loadGameMenu",
                    currentItemIndex,
                    lgm.deleteConfirmationScreen,
                };
                if (lgm.deleteConfirmationScreen)
                {
                    menuTypeObj = Utils.Merge(menuTypeObj, new
                    {
                        cancelDeleteButton = SerializeClickableCmp(lgm.cancelDeleteButton, mousePosition),
                        okDeleteButton = SerializeClickableCmp(lgm.okDeleteButton, mousePosition),
                    });
                }
                else 
                {
                    menuTypeObj = Utils.Merge(menuTypeObj, new
                    {
                        deleteButtons = SerializeComponentList(lgm.deleteButtons, mousePosition),
                        slotButtons = SerializeComponentList(lgm.slotButtons, mousePosition),
                        upArrow = SerializeClickableCmp(lgm.upArrow, mousePosition),
                        downArrow = SerializeClickableCmp(lgm.downArrow, mousePosition),
                    });
                }
            }
            else if (menu is Billboard)
            {
                var bb = menu as Billboard;
                bool dailyQuestBoard = Utils.GetPrivateField(bb, "dailyQuestBoard");
                menuTypeObj = new { menuType = "billboard" };
                if (dailyQuestBoard)
                {
                    menuTypeObj = Merge(menuTypeObj, new { 
                        acceptQuestButton = SerializeClickableCmp(bb.acceptQuestButton, mousePosition) 
                    });
                }
                else
                {
                    menuTypeObj = Merge(menuTypeObj, new
                    {
                        calendarDays = SerializeComponentList(bb.calendarDays, mousePosition)
                    });
                }
            }
            else if (menu is LevelUpMenu)
            {
                var lum = menu as LevelUpMenu;
                
                menuTypeObj = new
                {
                    menuType = "levelUpMenu",
                    okButton = SerializeClickableCmp(lum.okButton, mousePosition),
                    rightProfession = SerializeClickableCmp(lum.rightProfession, mousePosition),
                    leftProfession = SerializeClickableCmp(lum.leftProfession, mousePosition),
                    lum.isProfessionChooser
                };
            }
            else if (menu is MuseumMenu)
            {
                var mm = menu as MuseumMenu;
                var location = Game1.currentLocation as StardewValley.Locations.LibraryMuseum;
                var museumPieceTileComponents =
                    from t in VisibleTiles(location)
                    where
                        location.museumPieces.ContainsKey(new Vector2(t[0], t[1])) || 
                        location.isTileSuitableForMuseumPiece(t[0], t[1])
                    select TileToClickableComponent(t[0], t[1], mousePosition);
                var inventoryRect = new Rectangle(mm.inventory.xPositionOnScreen, mm.inventory.yPositionOnScreen, mm.inventory.width, mm.inventory.height);
                var clickableTileComponents = museumPieceTileComponents.Where(x => !x.rect.Intersects(inventoryRect)).ToList();
                menuTypeObj = new
                {
                    menuType = "museumMenu",
                    okButton = SerializeClickableCmp(mm.okButton, mousePosition),
                    clickableTileComponents,
                    inventory = SerializeMenu(mm.inventory),
                    trashCan = SerializeClickableCmp(mm.trashCan, mousePosition),
                };
            }
            else if (menu is GeodeMenu)
            {
                var gm = menu as GeodeMenu;
                menuTypeObj = new
                {
                    menuType = "geodeMenu",
                    okButton = SerializeClickableCmp(gm.okButton, mousePosition),
                    geodeSpot = SerializeClickableCmp(gm.geodeSpot, mousePosition),
                    inventory = SerializeMenu(gm.inventory),
                    trashCan = SerializeClickableCmp(gm.trashCan, mousePosition),
                };
            }
            return Utils.Merge(menuBarObj, menuTypeObj);
        }

        public static List<List<int>> VisibleTiles(GameLocation location)
        {
            xTile.Map map = location.Map;
            int mapWidth = map.Layers[0].LayerWidth;
            int mapHeight = map.Layers[0].LayerWidth;
            int startX = (Math.Max(Game1.viewport.X, 0) + 63) / 64;
            int startY = (Math.Max(Game1.viewport.Y, 0) + 63) / 64;
            int endX = Math.Min(Game1.viewport.MaxCorner.X / 64, mapWidth);
            int endY = Math.Min(Game1.viewport.MaxCorner.X / 64, mapHeight);
            var tiles = new List<List<int>>();
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    var tile = new List<int> { x, y };
                    tiles.Add(tile);
                }
            }
            return tiles;
        }

        public static List<object> SerializeComponentList(List<ClickableComponent> components, Point mousePosition)
        {
            return components?.Select(x => SerializeClickableCmp(x, mousePosition)).ToList();
        }

        public static List<object> SerializeComponentList(List<ClickableTextureComponent> components, Point mousePosition)
        {
            return components?.Select(x => SerializeClickableCmp(x, mousePosition)).ToList();
        }

        public static FarmAnimal FindAnimalByName(string name) 
        {
            var location = Game1.player.currentLocation;
            if (location is IAnimalLocation)
            {
                foreach (FarmAnimal animal in (location as IAnimalLocation).Animals.Values)
                {
                    if (name == animal.Name) return animal;
                }
            }
            return null;
        }
        public static object SerializeClickableCmp(ClickableComponent cmp, Point mousePosition, bool adjustUiMode = true)
        {
            if (cmp == null) return null;
            Rectangle bounds = cmp.bounds;
            bool containsMouse = cmp.containsPoint(mousePosition.X, mousePosition.Y);
            var serializedCmp = new
            {
                type = "clickableComponent",
                bounds = new { x = bounds.X, y = bounds.Y, width = bounds.Width, height = bounds.Height },
                center = new List<dynamic> { bounds.Center.X, bounds.Center.Y },
                cmp.name,
                containsMouse,
                cmp.visible,
                hoverText = cmp is ClickableTextureComponent ? (cmp as ClickableTextureComponent).hoverText : "",
            };
            if (adjustUiMode)
            {
                float x = (float)Convert.ToInt32(serializedCmp.center[0]);
                float y = (float)Convert.ToInt32(serializedCmp.center[1]);
                serializedCmp.center[0] = x * Game1.options.uiScale / Game1.options.zoomLevel;
                serializedCmp.center[1] = y * Game1.options.uiScale / Game1.options.zoomLevel;
            }
            return serializedCmp;
        }
        public static dynamic GetPrivateField(object obj, string fieldName)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic |
                         BindingFlags.Static | BindingFlags.Instance;
            var type = obj.GetType();
            var fi = type.GetField(fieldName, flags);
            if (fi != null) return fi.GetValue(obj);
            var methodFi = type.GetMethod(fieldName, flags);
            if (methodFi != null) return methodFi;
            return type.GetProperty(fieldName, flags)?.GetValue(obj);
        }

        public static bool DoesPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
            {
                return ((IDictionary<string, object>)settings).ContainsKey(name);
            }
            return settings.GetType().GetProperty(name) != null;
        }

        public static MethodInfo GetStaticMethod(Type type, string methodName) 
        {
            MethodInfo info = type.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return info;
        }

        public static void SetPrivateField(object obj, string fieldName, dynamic value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) field.SetValue(obj, value);
        }

        public static double DistanceBeteenPoints(float x1, float y1, float x2, float y2) 
        {
            float xdiff = Math.Abs(x1 - x2);
            float ydiff = Math.Abs(y1 - y2);
            return Math.Sqrt((Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)));
        }

        public static bool CanPlantOnHoeDirt(HoeDirt hd)
        {
            Item currentItem = Game1.player.ActiveObject;
            if (currentItem == null) return false;
            bool equippedFertilizer = currentItem.Category == -19;
            // canPlantThisSeedHere fertilizer test doesn't account for existing crops
            if (equippedFertilizer)
            {
                int fertilizer = hd.fertilizer.Value;
                bool emptyOrUngrownCrop = hd.crop == null || hd.crop.currentPhase.Value == 0;
                return emptyOrUngrownCrop && fertilizer == 0;
            }
            int objIndex = currentItem.ParentSheetIndex;
            Vector2 tileLocation = hd.currentTileLocation;
            int tileX = (int)tileLocation.X;
            int tileY = (int)tileLocation.Y;
            return hd.canPlantThisSeedHere(objIndex, tileX, tileY, false);
        }

        public static int DistanceBetweenTiles(int x1, int y1, int x2, int y2) 
        {
            int xdiff = Math.Abs(x1 - x2);
            int ydiff = Math.Abs(y1 - y2);
            return xdiff + ydiff;
        }

        public static GameLocation getLocationFromName(string locationName) 
        {
            return locationName == null ? Game1.player.currentLocation : Game1.getLocationFromName(locationName);
        }

        public static object SerializeItem(Item i) 
        {
            if (i == null) return null;
            var player = Game1.player;
            dynamic obj = new
            {
                netName = i.netName.Value,
                stack = i.Stack,
                displayName = i.DisplayName,
                name = i.Name,
                type = "",
                isTool = false,
            };
            if (i is Tool) 
            {
                var tool = i as Tool;
                var toolLocation = player.GetToolLocation();
                int tileX = (int)toolLocation.X / 64;
                int tileY = (int)toolLocation.Y / 64;

                obj = Utils.Merge(obj, new 
                { 
                    type = "tool",
                    isTool = true,
                    upgradeLevel = tool.UpgradeLevel,
                    power = player.toolPower,
                    baseName = tool.BaseName,
                    inUse = player.UsingTool,
                    tileX,
                    tileY,

                });
            }
            if (i is MeleeWeapon)
            {
                var mw = i as MeleeWeapon;
                string type = mw.isScythe() ? "scythe" : "meleeWeapon";
                obj = Utils.Merge(obj, new { type });
            }
            else if (i is FishingRod)
            {
                var fr = i as FishingRod;
                obj = Utils.Merge(obj, new
                {
                    fr.castingPower,
                    fr.isNibbling,
                    fr.isFishing,
                    fr.isLostItem,
                    fr.isReeling,
                    fr.isTimingCast,
                    type = "fishingRod",
                });
            }
            else if (i is Axe)
            {
                obj = Utils.Merge(obj, new { type = "axe" });
            }
            else if (i is Pickaxe)
            {
                obj = Utils.Merge(obj, new { type = "pickaxe" });
            }
            else if (i is WateringCan)
            {
                obj = Utils.Merge(obj, new { type = "wateringCan" });
            }
            else if (i is Hoe) obj = Utils.Merge(obj, new { type = "hoe" });
            else if (i is MilkPail) obj = Utils.Merge(obj, new { type = "Milk Pail" });
            else if (i is Pan) obj = Utils.Merge(obj, new { type = "pan" });
            else if (i is Shears) obj = Utils.Merge(obj, new { type = "shears" });

            return obj;
        }
    }
}
