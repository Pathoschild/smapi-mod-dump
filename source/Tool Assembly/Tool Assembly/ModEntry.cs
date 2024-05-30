/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/ToolAssembly
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Tools;
using StardewValley.Inventories;
using StardewValley.Network;
using Netcode;
using StardewValley.GameData.BigCraftables;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.GameData.Locations;
using xTile;
using StardewValley.Objects;
using GenericModConfigMenu;
using StardewValley.Tools;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;

namespace Tool_Assembly
{
    public class ModEntry : Mod
    {
        public ModConfig Config = new ModConfig();
        public static readonly NetLongDictionary<Inventory, NetRef<Inventory>> metaData = new();
        public static readonly NetLongDictionary<int, NetInt> indices = new();
        public static readonly NetInt topIndex = new();
        public static readonly NetStringHashSet items = new() { "(O)ofts.wandCris" };
        public static readonly PerScreen<bool> inBound = new(() => false);
        public static IModHelper? _Helper = null;
        public static IMonitor? _Monitor = null;
        public static ClickableTextureComponent destroyButton = new(new(0, 0, 64, 64), Game1.mouseCursors, new(268, 471, 16, 16), 4f);

        public override void Entry(IModHelper helper)
        {
            Helper.Events.Content.AssetRequested += loadTool;
            Helper.Events.Input.ButtonPressed += ButtonPressed;
            Helper.Events.GameLoop.SaveCreated += onSaveCreated;
            Helper.Events.GameLoop.SaveLoaded += load;
            Helper.Events.GameLoop.DayEnding += save;
            Helper.Events.Display.RenderedActiveMenu += injectDestroyButton;
            Helper.Events.GameLoop.ReturnedToTitle += (a, b) => { metaData.Clear(); indices.Clear(); };
            Helper.Events.Display.WindowResized += adjustWindowSize;
            //Helper.Events.GameLoop.DayStarted += debug;
            Helper.Events.GameLoop.GameLaunched += initAPI;
            Helper.Events.GameLoop.Saving += (a, b) => { Helper.Data.WriteSaveData("ofts.toolInd", topIndex.Value.ToString()); };
            Helper.Events.Specialized.LoadStageChanged += launched;
            Helper.ConsoleCommands.Add("tool", "", command);
            Config = Helper.ReadConfig<ModConfig>();
            _Helper = helper;
            _Monitor = Monitor;
        }

        public void adjustWindowSize(object? sender, WindowResizedEventArgs args)
        {
            if (Game1.activeClickableMenu is not ItemGrabMenu menu || menu.context is not string strcontext || !strcontext.Contains("ofts.toolConfigTable")) return;
            destroyButton.bounds.X = menu.xPositionOnScreen + menu.width + 4;
            destroyButton.bounds.Y = menu.yPositionOnScreen + 144;
        }

        public void injectDestroyButton(object? sender, RenderedActiveMenuEventArgs args)
        {
            if (Game1.activeClickableMenu is not ItemGrabMenu menu || menu.context is not string strcontext || !strcontext.Contains("ofts.toolConfigTable")) return;
            SpriteBatch b = args.SpriteBatch;
            
            if(destroyButton.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
            {
                destroyButton.scale = 4.2f;
                destroyButton.draw(b);
                destroyButton.scale = 4f;
                IClickableMenu.drawHoverText(b, Helper.Translation.Get("destroy"), Game1.smallFont);
                inBound.Value = true;
            }
            else
            {
                destroyButton.draw(b);
                inBound.Value = false;
            }
            menu.drawMouse(b);
        }

        public bool isTool(Item item)
        {
            if(item is Tool) return true;
            if(item is MeleeWeapon) return true;
            if(items.Contains(item.QualifiedItemId)) return true;
            return false;
        }

        public void initAPI(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                getValue: () => Config.Prev,
                setValue: value => Config.Prev = value,
                name: () => Helper.Translation.Get("left")
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                getValue: () => Config.Next,
                setValue: value => Config.Next = value,
                name: () => Helper.Translation.Get("right")
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                getValue: () => Config.EnableToolSwichKey,
                setValue: value => Config.EnableToolSwichKey = value,
                name: () => Helper.Translation.Get("toolswichkey")
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.GiveToolWhenSaveCreated,
                setValue: value => Config.GiveToolWhenSaveCreated = value,
                name: () => Helper.Translation.Get("giveTool")
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableToolSwich,
                setValue: value => Config.EnableToolSwich = value,
                name: () => Helper.Translation.Get("toolswich")
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.StoryMode,
                setValue: value => Config.StoryMode = value,
                name: () => Helper.Translation.Get("storyMode"),
                tooltip: () => Helper.Translation.Get("storyModeDesc")
            );
        }

        public override object? GetApi()
        {
            return new ToolAPIBase();
        }

        public void launched(object? sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage != StardewModdingAPI.Enums.LoadStage.SaveAddedLocations
                && e.NewStage != StardewModdingAPI.Enums.LoadStage.CreatedLocations) return;
            string? assetName = Helper.ModContent.GetInternalAssetName("assets/map.tmx").ToString();

            GameLocation location = new GameLocation(assetName, "aVeryVeryStrangeFourDimentionalSpaceThatStoresPlayersToolsStoredInToolAssemblyBecauseIDontKnowHowToSerizeTheDatasSoIDescideToLetTheGameItselfStoreTheDataForMeHaHaHaIAmSoSmart") { IsOutdoors = false, IsFarm = false };
            Game1.locations.Add(location);
        }

        public void save(object? sender, DayEndingEventArgs args)
        {
            if(!Context.IsMainPlayer) return;

            GameLocation location = Game1.getLocationFromName("aVeryVeryStrangeFourDimentionalSpaceThatStoresPlayersToolsStoredInToolAssemblyBecauseIDontKnowHowToSerizeTheDatasSoIDescideToLetTheGameItselfStoreTheDataForMeHaHaHaIAmSoSmart");

            for (int i = 0; i < 128; i++)
            {
                for (int j = 0; j < 128; j++)
                {
                    if (metaData.ContainsKey(i * 128 + j))
                    {
                        Chest chest2 = new(true, new Vector2(i, j));
                        chest2.Items.AddRange(metaData[i * 128 + j]);
                        location.Objects.Remove(chest2.TileLocation);
                        location.Objects[chest2.TileLocation] = chest2;
                    }
                }
            }
        }

        public void load(object? sender, SaveLoadedEventArgs args)
        {
            destroyButton = new(new(0, 0, 64, 64), Game1.mouseCursors, new(268, 471, 16, 16), 4f);
            inBound.Value = false;
            if (!Context.IsMainPlayer) return;
            metaData.Clear();
            indices.Clear();

            GameLocation location = Game1.getLocationFromName("aVeryVeryStrangeFourDimentionalSpaceThatStoresPlayersToolsStoredInToolAssemblyBecauseIDontKnowHowToSerizeTheDatasSoIDescideToLetTheGameItselfStoreTheDataForMeHaHaHaIAmSoSmart");

            for (int i = 0; i < 128; i++)
                for (int j = 0; j < 128; j++)
                    if (location.Objects.TryGetValue(new Vector2(i, j), out var tmp) && tmp is Chest chest)
                    {
                        metaData.Add(i * 128 + j, chest.Items);
                        indices.Add(i * 128 + j, 0);
                    }

            if(int.TryParse(Helper.Data.ReadSaveData<string>("ofts.toolInd"), out int ind))
                topIndex.Value = ind;
            else topIndex.Value = 0;
        }

        public void command(string c, string[] n)
        {
            foreach (var a in Game1.player.ActiveItem.modData)
            {
                foreach(var b in a)
                {
                    Monitor.Log(b.ToString(), LogLevel.Info);
                }
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            value = "";
            foreach (var a in Game1.player.ActiveItem.modData)
            {
                foreach (var b in a)
                {
                    if(b.Key == key)
                    {
                        value = b.Value;
                        return true;
                    }
                }
            }
            return false;
        }

        public void onSaveCreated(object? sender, SaveCreatedEventArgs e)
        {
            if (!Config.GiveToolWhenSaveCreated) return;
            Game1.player.addItemToInventory(ItemRegistry.Create("(T)ofts.toolAss"));
            Game1.player.addItemToInventory(ItemRegistry.Create("(BC)ofts.toolConfig"));
            metaData.Clear();
            indices.Clear();
        }

        public void loadTool(object? sender, AssetRequestedEventArgs args)
        {
            if (args.NameWithoutLocale.IsEquivalentTo("Data/Tools"))
            {
                args.Edit(asset => {
                    IDictionary<string, ToolData> datas = asset.AsDictionary<string, ToolData>().Data;
                    ToolData data = new()
                    {
                        ClassName = "GenericTool",
                        Name = "toolAssembly",
                        AttachmentSlots = 0,
                        SalePrice = 1000,
                        DisplayName = "[LocalizedText Strings\\ofts_toolass:display_tool]",
                        Description = "[LocalizedText Strings\\ofts_toolass:descrip_tool]",
                        Texture = "toolAss/asset/texture",
                        SpriteIndex = 0,
                        MenuSpriteIndex = 5,
                        UpgradeLevel = -1,
                        ApplyUpgradeLevelToDisplayName = false,
                        CanBeLostOnDeath = false
                    };
                    datas.Add("ofts.toolAss", data);
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                args.Edit(asset =>
                {
                    IDictionary<string, ObjectData> datas = asset.AsDictionary<string, ObjectData>().Data;
                    ObjectData data = new()
                    {
                        Name = "Wand Cristal",
                        DisplayName = "[LocalizedText Strings\\ofts_toolass:display_cris]",
                        Description = "[LocalizedText Strings\\ofts_toolass:descrip_cris]",
                        Type = "Basic",
                        Category = -999,
                        Price = 10000,
                        Texture = "toolAss/asset/texture",
                        SpriteIndex = 26,
                        CanBeGivenAsGift = false,
                        ExcludeFromShippingCollection = true,
                        ContextTags = new() { "ofts.toolass.toolRelated" }
                    };
                    datas.Add("ofts.wandCris", data); ;
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                args.Edit(asset =>
                {
                    IDictionary<string, BigCraftableData> datas = asset.AsDictionary<string, BigCraftableData>().Data;
                    BigCraftableData data = new()
                    {
                        Name = "ToolConfigTable",
                        DisplayName = "[LocalizedText Strings\\ofts_toolass:display_table]",
                        Description = "[LocalizedText Strings\\ofts_toolass:descrip_table]",
                        Price = 100,
                        Fragility = 0,
                        IsLamp = false,
                        Texture = "toolAss/asset/texture",
                        SpriteIndex = 6,
                        ContextTags = new() { "ofts.toolass.toolRelated" }
                    };
                    datas.Add("ofts.toolConfig", data);
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                args.Edit(asset => {
                    IDictionary<string, ShopData> datas = asset.AsDictionary<string, ShopData>().Data;

                    if (Config.StoryMode)
                    {
                        ShopData wizard = new()
                        {
                            Currency = 0,
                            StackSizeVisibility = StackSizeVisibility.ShowIfMultiple,
                            OpenSound = "dwop",
                            PurchaseSound = "purchaseClick",
                            PurchaseRepeatSound = "purchaseRepeat",
                            PriceModifiers = new(),
                            Owners = new()
                            {
                                new()
                                {
                                    Portrait = "Wizard",
                                    Name = "Wizard",
                                    Id = "Wizard",
                                    Dialogues = new()
                                    {
                                        new()
                                        {
                                            Id = "Wizard_Dialogue",
                                            Dialogue = "[LocalizedText Strings\\ofts_toolass:dialogue]",
                                            Condition = "PLAYER_HAS_MAIL Current ofts.toolass.seenWizardShop received",
                                        },
                                        new()
                                        {
                                            Id = "Wizard_Dialogue_First",
                                            Dialogue = "[LocalizedText Strings\\ofts_toolass:dialogue1]",
                                            Condition = "!PLAYER_HAS_MAIL Current ofts.toolass.seenWizardShop received",
                                        }
                                    }
                                }
                            },
                            VisualTheme = new()
                            {
                                new()
                                {
                                    WindowBorderTexture =  "LooseSprites\\Cursors2",
                                    WindowBorderSourceRect = new(0, 256, 18, 18),
                                    ItemRowBackgroundTexture = "LooseSprites\\Cursors2",
                                    ItemRowBackgroundSourceRect = new (18, 256, 15, 15),
                                    ItemRowBackgroundHoverColor = "Blue",
                                    ItemRowTextColor = "White",
                                    ItemIconBackgroundTexture = "LooseSprites\\Cursors2",
                                    ItemIconBackgroundSourceRect = new(33, 256, 18, 18)
                                }
                            },
                            SalableItemTags = new() { "ofts.toolass.toolRelated" },
                            Items = new()
                            {
                                new()
                                {
                                    ItemId = "(T)ofts.toolAss",
                                    Id = "ofts.toolass.shopitems.tool",
                                    AvailableStock = 1,
                                    AvailableStockLimit = LimitedStockMode.Global,
                                    UseObjectDataPrice = true,
                                    ActionsOnPurchase = new() { "AddMail Current ofts.toolass.seenWizardShop received" }
                                },
                                new()
                                {
                                    ItemId = "(BC)ofts.toolConfig",
                                    Id = "ofts.toolass.shopitems.bc",
                                    AvailableStock = 1,
                                    AvailableStockLimit = LimitedStockMode.Global,
                                    UseObjectDataPrice = true,
                                    ActionsOnPurchase = new() { "AddMail Current ofts.toolass.seenWizardShop received" }
                                },
                                new()
                                {
                                    ItemId = "(O)ofts.wandCris",
                                    Id = "ofts.toolass.shopitems.o",
                                    AvailableStock = 1,
                                    AvailableStockLimit = LimitedStockMode.Global,
                                    UseObjectDataPrice = true,
                                    ActionsOnPurchase = new() { "AddMail Current ofts.toolass.seenWizardShop received" }
                                }
                            }
                        };
                        datas.Add("ofts.toolass.wizardshop", wizard);
                    }
                    else
                    {
                        ShopData data = datas["AdventureShop"];
                        ShopItemData wand = new()
                        {
                            Price = 2000,
                            Id = "ofts.shop.wand",
                            ItemId = "(T)ofts.toolAss",
                        };
                        data.Items.Add(wand);
                        ShopItemData cris = new()
                        {
                            Price = 20000,
                            Id = "ofts.shop.cris",
                            ItemId = "(O)ofts.wandCris",
                        };
                        data.Items.Add(cris);
                        data.SalableItemTags.Add("id_(T)ofts.toolAss");
                    }
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                args.Edit(asset =>
                {
                    IDictionary<string, string> datas = asset.AsDictionary<string, string>().Data;
                    datas.Add("Tool Configuation Table", "(BC)130 1 335 5 388 30/Home/ofts.toolConfig 1/true/default/");
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("toolAss/asset/texture"))
            {
                args.LoadFromModFile<Texture2D>("assets/texture", AssetLoadPriority.Low);
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("toolAss/asset/map"))
            {
                args.LoadFromModFile<Map>("assets/map", AssetLoadPriority.Low);
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Strings\\ofts_toolass"))
            {
                args.LoadFrom(() => {
                    return new Dictionary<string, string>()
                    {
                        { "display_table", Helper.Translation.Get("display_table") },
                        { "descrip_table", Helper.Translation.Get("descrip_table") },
                        { "display_tool", Helper.Translation.Get("display_tool") },
                        { "descrip_tool", Helper.Translation.Get("descrip_tool") },
                        { "display_cris", Helper.Translation.Get("display_cris") },
                        { "descrip_cris", Helper.Translation.Get("descrip_cris") },
                        { "dialogue", Helper.Translation.Get("dialogue") },
                        { "dialogue1", Helper.Translation.Get("dialogue1") }
                    };
                }, AssetLoadPriority.Low);
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
            {
                args.Edit(asset =>
                {
                    IDictionary<string, LocationData> datas = asset.AsDictionary<string, LocationData>().Data;
                    LocationData data = new()
                    {
                        DisplayName = "aVeryVeryStrangeFourDimentionalSpaceThatStoresPlayersToolsStoredInToolAssemblyBecauseIDontKnowHowToSerizeTheDatasSoIDescideToLetTheGameItselfStoreTheDataForMeHaHaHaIAmSoSmart",
                        ExcludeFromNpcPathfinding = true,
                        CreateOnLoad = new()
                        {
                            MapPath = "toolAss/asset/map",
                            AlwaysActive = true
                        },
                        CanPlantHere = false,
                        CanHaveGreenRainSpawns = false,
                        MinDailyWeeds = 0,
                        MaxDailyWeeds = 0,
                        FirstDayWeedMultiplier = 1,
                        MinDailyForageSpawn = 0,
                        MaxDailyForageSpawn = 0,
                        ChanceForClay = 0,
                    };
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/Events/WizardHouse"))
            {
                if(Config.StoryMode)
                args.Edit(asset =>
                {
                    IDictionary<string, string> datas = asset.AsDictionary<string, string>().Data;
                    datas.Add(
                        "ofts.toolass.wizard.giveWandCris/FreeInventorySlots 1/Friendship Wizard 2040/HasItem (T)ofts.toolAss",

                        "WizardSong/2 14/farmer 3 14 3 Wizard 1 14 1/skippable/pause 1000/" +
                        $"speak Wizard \"{Helper.Translation.Get("wizard.1")}\"/" +
                        "move Wizard 1 0 1/pause 1000/" +
                        $"speak Wizard \"{Helper.Translation.Get("wizard.2")}\"/" +
                        $"speak Wizard \"{Helper.Translation.Get("wizard.3")}\"/" +
                        "pause 1000/itemAboveHead (O)ofts.wandCris/pause 3300/awardFestivalPrize (O)ofts.wandCris/pause 1000/" +
                        $"speak Wizard \"{Helper.Translation.Get("wizard.4")}\"/" +
                        $"speak Wizard \"{Helper.Translation.Get("wizard.5")}\"/end"
                    );
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Maps/WizardHouse"))
            {
                if (Config.StoryMode)
                {
                    args.Edit(asset =>
                    {
                        asset.AsMap().PatchMap(
                            Helper.ModContent.Load<Map>("assets\\map.tmx"),
                            sourceArea: new(122, 125, 6, 3),
                            targetArea: new(1, 16, 6, 3),
                            patchMode: PatchMapMode.Overlay
                        );
                        var a = asset.AsMap().Data;
                        a.GetLayer("Buildings").Tiles.Array[3, 18].Properties.Add("Action", "OpenShop ofts.toolass.wizardshop");
                        a.GetLayer("Buildings").Tiles.Array[4, 18].Properties.Add("Action", "OpenShop ofts.toolass.wizardshop");
                        a.GetLayer("Buildings").Tiles.Array[2, 18].Properties.Add("Action", "None");
                    });
                }
            }
        }

        public void ButtonPressed(object? sender, ButtonPressedEventArgs args)
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu == null && Config.EnableToolSwichKey.JustPressed())
            {
                Config.EnableToolSwich = !Config.EnableToolSwich;
                Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("autoswichswiched") + Config.EnableToolSwich.ToString()));
                return;
            }

            if (Game1.activeClickableMenu is ItemGrabMenu menu && menu.context is string strcontext && strcontext.Contains("ofts.toolConfigTable"))
            {
                if (args.Button == SButton.MouseLeft && inBound.Value && long.TryParse(strcontext.AsSpan(20), out long idinv))
                {
                    Inventory playerinv = Game1.player.Items;
                    playerinv.Remove(Game1.player.ActiveItem);

                    if (!metaData.ContainsKey(idinv))
                    {
                        Monitor.Log("Error: The given id does not present. This tool may be damaged. Resetting tool id. You can find your lost items in the lost and found office if any", LogLevel.Error);
                        Inventory inv = new();
                        inv.AddRange(new List<Item>(36));
                        metaData.TryAdd(idinv, inv);
                        indices.TryAdd(idinv, 0);
                    }
                    else foreach (Item item in metaData[idinv])
                        {
                            if (item == null) continue;
                            Game1.currentLocation.debris.Add(Game1.createItemDebris(item, Game1.player.Position, 0));
                            item.modData.Remove("ofts.toolAss.id");
                        }
                    if (menu.heldItem != null)
                    {
                        Game1.currentLocation.debris.Add(Game1.createItemDebris(menu.heldItem, Game1.player.Tile, 0));
                        menu.heldItem = null;
                    }
                    menu.exitThisMenu(true);
                    Game1.activeClickableMenu = null;
                    return;
                }
            }

            if (Context.IsWorldReady && Game1.activeClickableMenu == null && Game1.player.ActiveItem != null &&
                TryGetValue("ofts.toolAss.id", out string id))
            {
                if (long.TryParse(id, out long longid))
                {
                    if (metaData.TryGetValue(longid, out var inventory))
                    {
                        if (inventory.Count > 0)
                        {
                            int indexPlayer = Game1.player.Items.IndexOf(Game1.player.ActiveItem);

                            if (Config.Next.JustPressed())
                            {
                                Game1.player.Items[indexPlayer] = inventory[(indices[longid] + 1) % inventory.Count];
                                indices[longid] = (indices[longid] + 1) % inventory.Count;
                            }

                            else if (Config.Prev.JustPressed())
                            {
                                Game1.player.Items[indexPlayer] = inventory[(indices[longid] + inventory.Count - 1) % inventory.Count];
                                indices[longid] = (indices[longid] + inventory.Count - 1) % inventory.Count;
                            }

                            else
                            {
                                bool keydown = false;
                                foreach (var key in Game1.options.useToolButton)
                                {
                                    if (args.IsDown(key.ToSButton()))
                                    {
                                        keydown = true;
                                        break;
                                    }
                                }

                                if (keydown && Config.EnableToolSwich)
                                {
                                    Game1.player.Items[indexPlayer] = ToolSwitchHandler.GetBestIndex(indices[longid], inventory);
                                }
                            }
                        }
                    }
                    else
                    {
                        Monitor.Log("Error: The given id does not present. This tool may be damaged. Resetting tool id. You can find your lost items in the lost and found office if any", LogLevel.Error);
                        Inventory inv = new();
                        inv.AddRange(new List<Item>(36));
                        metaData.TryAdd(longid, inv);
                        indices.TryAdd(longid, 0);
                    }
                }
            }

            bool keyPressed = false;
            foreach (var key in Game1.options.actionButton)
            {
                if (args.IsDown(key.ToSButton()))
                {
                    keyPressed = true;
                    break;
                }
            }

            if (keyPressed)
            {
                if (Game1.player.currentLocation.Objects.TryGetValue(Game1.player.GetGrabTile(), out var obj)
                    && obj.QualifiedItemId == "(BC)ofts.toolConfig")
                {
                    clickConfigTable();
                }
                else if (Game1.player.ActiveItem != null && TryGetValue("ofts.toolAss.id", out string invId)
                    && long.TryParse(invId, out long invNum)
                    && metaData.TryGetValue(invNum, out var invt) && invt.GetById("(O)ofts.wandCris").Any())
                {
                    clickConfigTable();
                }
                else if (Game1.currentLocation.Name == "WizardHouse" && Game1.lastCursorTile == new Vector2(2, 18) && Config.StoryMode)
                {
                    clickConfigTable();
                }
            }
        }

        public int assignNewInventory(Item tool)
        {
            Inventory inv = new();
            inv.AddRange(new List<Item>(36));
            while (metaData.ContainsKey(topIndex.Value)) topIndex.Value++;
            tool.modData.Add("ofts.toolAss.id", $"{topIndex.Value}");
            metaData.Add(topIndex.Value, inv);
            indices.Add(topIndex.Value++, 0);
            return topIndex.Value - 1;
        }

        public void clickConfigTable()
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                Item? tool = Game1.player.ActiveItem;
                if (tool != null && isTool(tool) && (
                    TryGetValue("ofts.toolAss.id", out string id) || tool.Name == "toolAssembly"))
                {
                    if (id == ""){
                        assignNewInventory(tool);
                        TryGetValue("ofts.toolAss.id", out id);
                    }

                    if (!metaData.TryGetValue(long.Parse(id), out var i))
                    {
                        Monitor.Log("Error: The given id does not present. This tool may be damaged. Resetting tool id. You can find your lost items in the lost and found office if any", LogLevel.Error);
                        Inventory inv = new();
                        inv.AddRange(new List<Item>(36));
                        metaData.TryAdd(long.Parse(id), inv);
                        indices.TryAdd(long.Parse(id), 0);
                    }

                    Item it = ItemRegistry.Create("ofts.toolAss");
                    it.modData.Add("ofts.toolAss.id", id);
                    Game1.player.Items[Game1.player.CurrentToolIndex] = it;
                    Game1.delayedActions.Add(new DelayedAction(10, () => 
                    { 
                        ItemGrabMenu menu = new ItemGrabMenu(
                            inventory: i, 
                            reverseGrab: false, showReceivingMenu: true, (item) => {
                                if (item == null) return true;
                                if (i.Contains(item)) return true;
                                return isTool(item) && !item.modData.ContainsKey("ofts.toolAss.id");
                            }, behaviorOnItemSelectFunction: (a, b) => {
                                if (Game1.activeClickableMenu is not ItemGrabMenu menu)
                                    throw new InvalidOperationException("WTF Why current menu is not IGM?!!");
                                else menu.heldItem = null;
                                if (i.Count < 36) {
                                    i.Add(a);
                                    a.modData.Add("ofts.toolAss.id", Game1.player.ActiveItem.modData["ofts.toolAss.id"]);
                                    Game1.player.Items.Remove(a);
                                } 
                            }, "",
                            behaviorOnItemGrab: (a, b) => {
                                if (Game1.activeClickableMenu is not ItemGrabMenu menu) 
                                    throw new InvalidOperationException("WTF Why current menu is not IGM?!!");
                                else menu.heldItem = null;

                                if (Utility.canItemBeAddedToThisInventoryList(a, Game1.player.Items))
                                { 
                                    for(int i = 0; i < Game1.player.Items.Count; i++)
                                    {
                                        if (Game1.player.Items[i] == null)
                                        {
                                            Game1.player.Items[i] = a;
                                            break;
                                        }
                                    }
                                    i.Remove(a);
                                    i.RemoveEmptySlots();
                                    a.modData.Remove("ofts.toolAss.id");
                                } 
                            }, context: $"ofts.toolConfigTable{id}"
                        );
                        Game1.activeClickableMenu = menu;
                        destroyButton.bounds.X = menu.xPositionOnScreen + menu.width + 4;
                        destroyButton.bounds.Y = menu.yPositionOnScreen + 144;
                        //xPositionOnScreen + width + 4, yPositionOnScreen + height - 192 - IClickableMenu.borderWidth
                    }));
                }
                else
                {
                    Game1.multipleDialogues(Helper.Translation.Get("requireTool", 
                        new Dictionary<string, string>() 
                        {
                            {
                                "shopLocation", 
                                Config.StoryMode ? 
                                NPC.GetDisplayName("Wizard") : 
                                GameLocation.GetData("WizardHouse").DisplayName 
                            } 
                        }).ToString().Split('*')
                    );
                }
            }
        }
    }

    public class ModConfig
    {
        public KeybindList Next { get; set; } = KeybindList.Parse("Right");
        public KeybindList Prev { get; set; } = KeybindList.Parse("Left");
        public KeybindList EnableToolSwichKey { get; set; } = KeybindList.Parse("Up");
        public bool GiveToolWhenSaveCreated { get; set; } = true;
        public bool EnableToolSwich { get; set; } = true;
        public bool StoryMode { get; set; } = true;
    }
}