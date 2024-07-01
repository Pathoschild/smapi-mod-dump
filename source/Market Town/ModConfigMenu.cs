/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

using ContentPatcher;
using HarmonyLib;
using MarketTown.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.LocationContexts;
using StardewValley.GameData.Shops;
using StardewValley.GameData.SpecialOrders;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace MarketTown
{

    public partial class ModEntry
    {

        public static void ConfigMenu (IContentPatcherAPI api, IManifest ModManifest, IModHelper Helper)
        {
            api.RegisterToken(ModManifest, "IslandSign", () =>
            {
                if (Context.IsWorldReady && SHelper.Data.ReadSaveData<MailData>("MT.MailLog") != null && SHelper.Data.ReadSaveData<MailData>("MT.MailLog").TotalVisitorVisited != 0)
                {
                    var totalVisitor = SHelper.Data.ReadSaveData<MailData>("MT.MailLog").TotalVisitorVisited;

                    GameLocation locat = Game1.getLocationFromName("Custom_MT_Island");
                    string returnValue = SHelper.Translation.Get("foodstore.islandsign", new { season = locat.GetSeason().ToString(), visitor = totalVisitor.ToString() });
                    return new[] { returnValue };
                }
                else if (Context.IsWorldReady && SHelper.Data.ReadSaveData<MailData>("MT.MailLog") == null)
                {
                    GameLocation locat = Game1.getLocationFromName("Custom_MT_Island");
                    string returnValue = SHelper.Translation.Get("foodstore.islandsign", new { season = locat.GetSeason().ToString(), visitor = "0" });
                    return new[] { returnValue };
                }
                return null;
            });

            api.RegisterToken(ModManifest, "IslandFestivalDay", () =>
            {
                if (Context.IsWorldReady)
                {
                    int dayOfWeek = Game1.dayOfMonth % 7;
                    bool festivalDay = false;

                    switch (dayOfWeek)
                    {
                        case 0:
                            festivalDay = Config.FestivalSun;
                            break;
                        case 1:
                            festivalDay = Config.FestivalMon;
                            break;
                        case 2:
                            festivalDay = Config.FestivalTue;
                            break;
                        case 3:
                            festivalDay = Config.FestivalWed;
                            break;
                        case 4:
                            festivalDay = Config.FestivalThu;
                            break;
                        case 5:
                            festivalDay = Config.FestivalFri;
                            break;
                        case 6:
                            festivalDay = Config.FestivalSat;
                            break;
                        default:
                            // Handle invalid dayOfWeek value
                            break;
                    }

                    return new[] { festivalDay.ToString().ToLower() };
                }
                return null;
            });

            api.RegisterToken(ModManifest, "IslandProgressLevel", () =>
            {
                if (Context.IsWorldReady && Game1.IsMasterGame && Helper.Data.ReadSaveData<MailData>("MT.MailLog") != null)
                {
                    MailData model = Helper.Data.ReadSaveData<MailData>("MT.MailLog");
                    int level = model.FestivalEarning;
                    string islandProgressLevel = "0";

                    if (30000 < level && level <= 100000) islandProgressLevel = "1";
                    else if (100000 < level && level <= 250000) islandProgressLevel = "2";
                    else if (level > 250000) islandProgressLevel = "3";

                    GameLocation locat = Game1.getLocationFromName("Custom_MT_Island");
                    var islandBrazier = locat.getObjectAtTile(22, 36);
                    if (locat == null || islandBrazier == null || islandBrazier.ItemId == null || islandBrazier.ItemId != "MT.Objects.ParadiseIslandBrazier" || !islandBrazier.IsOn)
                        return new[] { "-1" };

                    if (!Config.IslandProgress) islandProgressLevel = "3";

                    return new[] { islandProgressLevel };
                }
                return null;
            });

            Texture2D originalTexture = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("Assets/markettown.png");

            int newWidth = (int)(originalTexture.Width / 1.35);
            int newHeight = (int)(originalTexture.Height / 1.35);
            Texture2D resizedTexture = new Texture2D(originalTexture.GraphicsDevice, newWidth, newHeight);

            Color[] originalData = new Color[originalTexture.Width * originalTexture.Height];
            originalTexture.GetData(originalData);
            Color[] resizedData = new Color[newWidth * newHeight];

            // Resize
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    int originalX = (int)(x * 1.35);
                    int originalY = (int)(y * 1.35);
                    resizedData[x + y * newWidth] = originalData[originalX + originalY * originalTexture.Width];
                }
            }
            resizedTexture.SetData(resizedData);
            Microsoft.Xna.Framework.Rectangle displayArea = new Microsoft.Xna.Framework.Rectangle(0, 0, newWidth, newHeight);

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<MarketTown.Data.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddImage(mod: ModManifest, texture: () => resizedTexture, texturePixelArea: null, scale: 1);

            configMenu.AddBoolOption(
            mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.disablenonfoodonfarm"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.disablenonfoodonfarmText"),
                getValue: () => Config.AllowRemoveNonFood,
                setValue: value => Config.AllowRemoveNonFood = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.minutetohungry"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.minutetohungryText"),
                getValue: () => Config.MinutesToHungry,
                setValue: value => Config.MinutesToHungry = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.movetofoodchange"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.movetofoodchangeText"),
                getValue: () => Config.MoveToFoodChance,
                setValue: value => Config.MoveToFoodChance = value,
                min: 0.0f,
                max: 0.33f,
                interval: 0.0025f
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.maxdistancetofindfood"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.maxdistancetofindfoodText"),
                getValue: () => "" + Config.MaxDistanceToFind,
                setValue: delegate (string value) { try { Config.MaxDistanceToFind = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.maxdistancetoeat"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.maxdistancetoeatText"),
                getValue: () => "" + Config.MaxDistanceToEat,
                setValue: delegate (string value) { try { Config.MaxDistanceToEat = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.randompurchase"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.randompurchaseText"),
                getValue: () => Config.RandomPurchase,
                setValue: value => Config.RandomPurchase = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.restockchance"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.restockchanceText"),
                getValue: () => Config.RestockChance,
                setValue: value => Config.RestockChance = value,
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.rushhour"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.rushhourText"),
                getValue: () => Config.RushHour,
                setValue: value => Config.RushHour = value
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.enabledecor"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.enabledecorText"),
                getValue: () => Config.EnableDecor,
                setValue: value => Config.EnableDecor = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.enabletipclose"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.enabletipcloseText"),
                getValue: () => Config.TipWhenNeaBy,
                setValue: value => Config.TipWhenNeaBy = value
            );

            configMenu.AddPageLink(mod: ModManifest, "island", () => SHelper.Translation.Get("foodstore.config.island"));
            configMenu.AddPageLink(mod: ModManifest, "shed", () => SHelper.Translation.Get("foodstore.config.shed"));
            configMenu.AddPageLink(mod: ModManifest, "dialogue", () => SHelper.Translation.Get("foodstore.config.dialogue"));
            configMenu.AddPageLink(mod: ModManifest, "inviteTime", () => SHelper.Translation.Get("foodstore.config.invitetime"));
            configMenu.AddPageLink(mod: ModManifest, "salePrice", () => SHelper.Translation.Get("foodstore.config.saleprice"));
            configMenu.AddPageLink(mod: ModManifest, "advance", () => SHelper.Translation.Get("foodstore.config.advance"));

            // Island setting
            configMenu.AddPage(mod: ModManifest, "island", () => SHelper.Translation.Get("foodstore.config.island"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.islandprogress"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.islandprogressText"),
                getValue: () => Config.IslandProgress,
                setValue: value => Config.IslandProgress = value
            );

            configMenu.AddNumberOption(
               mod: ModManifest,
               name: () => SHelper.Translation.Get("foodstore.config.maxislandNPC"),
               tooltip: () => SHelper.Translation.Get("foodstore.config.maxislandNPCText"),
               getValue: () => Config.ParadiseIslandNPC,
               setValue: value => Config.ParadiseIslandNPC = value,
               min: 0,
               max: 100,
               interval: 1
           );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.islandwalkaround"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.islandwalkaroundText"),
                getValue: () => Config.IslandWalkAround,
                setValue: value => Config.IslandWalkAround = value,
                min: 0.0f,
                max: 0.75f,
                interval: 0.01f
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.islandvisithouse"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.islandvisithouseText"),
                getValue: () => Config.VisitChanceIslandHouse,
                setValue: value => Config.VisitChanceIslandHouse = value,
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.islandvisitbuilding"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.islandvisitbuildingText"),
                getValue: () => Config.VisitChanceIslandBuilding,
                setValue: value => Config.VisitChanceIslandBuilding = value,
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => SHelper.Translation.Get("foodstore.config.islandplant")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.islandplantboost"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.islandplantboostText"),
                getValue: () => Config.IslandPlantBoost,
                setValue: value => Config.IslandPlantBoost = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.islandplantboostchance"),
                getValue: () => Config.IslandPlantBoostChance,
                setValue: value => Config.IslandPlantBoostChance = value,
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => SHelper.Translation.Get("foodstore.config.festivalschedule")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.islandfestivalsellchance"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.islandfestivalsellchanceText"),
                getValue: () => Config.FestivalMaxSellChance,
                setValue: value => Config.FestivalMaxSellChance = value,
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f
            );
            configMenu.AddNumberOption(
               mod: ModManifest,
               name: () => SHelper.Translation.Get("foodstore.config.festivaltimestart"),
               getValue: () => Config.FestivalTimeStart,
               setValue: value => Config.FestivalTimeStart = value,
               min: 630,
               max: 2400,
               interval: 10
           );
            configMenu.AddNumberOption(
               mod: ModManifest,
               name: () => SHelper.Translation.Get("foodstore.config.festivaltimeend"),
               getValue: () => Config.FestivalTimeEnd,
               setValue: value => Config.FestivalTimeEnd = value,
               min: 700,
               max: 2400,
               interval: 10
           );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.festivalmon"),
                getValue: () => Config.FestivalMon,
                setValue: value => Config.FestivalMon = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.festivaltue"),
                getValue: () => Config.FestivalTue,
                setValue: value => Config.FestivalTue = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.festivalwed"),
                getValue: () => Config.FestivalWed,
                setValue: value => Config.FestivalWed = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.festivalthu"),
                getValue: () => Config.FestivalThu,
                setValue: value => Config.FestivalThu = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.festivalfri"),
                getValue: () => Config.FestivalFri,
                setValue: value => Config.FestivalFri = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.festivalsat"),
                getValue: () => Config.FestivalSat,
                setValue: value => Config.FestivalSat = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.festivalsun"),
                getValue: () => Config.FestivalSun,
                setValue: value => Config.FestivalSun = value
            );

            // Shed setting
            configMenu.AddPage(mod: ModManifest, "shed", () => SHelper.Translation.Get("foodstore.config.shed"));

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.easylicense"),
                getValue: () => Config.EasyLicense,
                setValue: value => Config.EasyLicense = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.shedvisitchance"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.shedvisitchanceText"),
                getValue: () => Config.ShedVisitChance,
                setValue: value => Config.ShedVisitChance = value,
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.museumpricemarkup"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.museumpricemarkupText"),
                getValue: () => Config.MuseumPriceMarkup,
                setValue: value => Config.MuseumPriceMarkup = value,
                min: 0.0f,
                max: 4.0f,
                interval: 0.025f
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.maxshedcapacity"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.maxshedcapacityText"),
                getValue: () => "" + Config.MaxShedCapacity,
                setValue: delegate (string value) { try { Config.MaxShedCapacity = Int32.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.timestay"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.timestayText"),
                getValue: () => "" + Config.TimeStay,
                setValue: delegate (string value) { try { Config.TimeStay = Int32.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.doorentry"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.doorentryText"),
                getValue: () => Config.DoorEntry,
                setValue: value => Config.DoorEntry = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.buswalk"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.buswalkText"),
                getValue: () => Config.BusWalk,
                setValue: value => Config.BusWalk = value
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.shedminutetohungry"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.shedminutetohungryText"),
                getValue: () => "" + Config.ShedMinuteToHungry,
                setValue: delegate (string value) { try { Config.ShedMinuteToHungry = Int32.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.shedbuychance"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.shedbuychanceText"),
                getValue: () => Config.ShedMoveToFoodChance,
                setValue: value => Config.ShedMoveToFoodChance = value,
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.openhour"),
                getValue: () => Config.OpenHour,
                setValue: value => Config.OpenHour = (int)value,
                min: 610,
                max: 2400f,
                interval: 10f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.closehour"),
                getValue: () => Config.CloseHour,
                setValue: value => Config.CloseHour = (int)value,
                min: 610,
                max: 2400f,
                interval: 10f
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => SHelper.Translation.Get("foodstore.config.shedvisitor")
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.modkey"),
                getValue: () => Config.ModKey,
                setValue: value => Config.ModKey = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.tablesit"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.tablesitText"),
                getValue: () => Config.TableSit,
                setValue: value => Config.TableSit = value,
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.orderchance"),
                getValue: () => Config.OrderChance,
                setValue: value => Config.OrderChance = value,
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.lovedishchance"),
                getValue: () => Config.LovedDishChance,
                setValue: value => Config.LovedDishChance = value,
                min: 0.0f,
                max: 1.0f,
                interval: 0.01f
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.pricemultiplier"),
                getValue: () => "" + Config.PriceMarkup,
                setValue: delegate (string value) { try { Config.PriceMarkup = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.maxordernight"),
                getValue: () => "" + Config.MaxNPCOrdersPerNight,
                setValue: delegate (string value) { try { Config.MaxNPCOrdersPerNight = Int32.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );

            //Dialogue setting
            configMenu.AddPage(mod: ModManifest, "dialogue", () => SHelper.Translation.Get("foodstore.config.dialogue"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.textchat"),
                getValue: () => Config.DisableTextChat,
                setValue: value => Config.DisableTextChat = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.chat"),
                getValue: () => Config.DisableChat,
                setValue: value => Config.DisableChat = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.kidaskchance"),
                getValue: () => "" + Config.KidAskChance,
                setValue: delegate (string value) { try { Config.KidAskChance = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.disablekidask"),
                getValue: () => Config.DisableKidAsk,
                setValue: value => Config.DisableKidAsk = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.disableallmessage"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.disableallmessageText"),
                getValue: () => Config.DisableChatAll,
                setValue: value => Config.DisableChatAll = value
            );


            //Villager invite
            configMenu.AddPage(mod: ModManifest, "inviteTime", () => SHelper.Translation.Get("foodstore.config.invitetime"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.enablevisitinside"),
                getValue: () => Config.EnableVisitInside,
                setValue: value => Config.EnableVisitInside = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.invitecometime"),
                getValue: () => Config.InviteComeTime,
                setValue: value => Config.InviteComeTime = (int)value,
                min: 600,
                max: 2400f,
                interval: 10f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.inviteleavetime"),
                getValue: () => Config.InviteLeaveTime,
                setValue: value => Config.InviteLeaveTime = (int)value,
                min: 600,
                max: 2400f,
                interval: 10f
            );
            //sell multiplier

            configMenu.AddPage(mod: ModManifest, "salePrice", () => SHelper.Translation.Get("foodstore.config.saleprice"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.multiplayermode"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.multiplayermodeText"),
                getValue: () => Config.MultiplayerMode,
                setValue: value => Config.MultiplayerMode = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.enableprice"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.enablepriceText"),
                getValue: () => Config.EnablePrice,
                setValue: value => Config.EnablePrice = value
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.pricelovemulti"),
                getValue: () => "" + Config.LoveMultiplier,
                setValue: delegate (string value) { try { Config.LoveMultiplier = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.pricelikemulti"),
                getValue: () => "" + Config.LikeMultiplier,
                setValue: delegate (string value) { try { Config.LikeMultiplier = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.priceneutralmulti"),
                getValue: () => "" + Config.NeutralMultiplier,
                setValue: delegate (string value) { try { Config.NeutralMultiplier = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.pricedislikemulti"),
                getValue: () => "" + Config.DislikeMultiplier,
                setValue: delegate (string value) { try { Config.DislikeMultiplier = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.pricehatemulti"),
                getValue: () => "" + Config.HateMultiplier,
                setValue: delegate (string value) { try { Config.HateMultiplier = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.enabletip"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.enabletipText"),
                getValue: () => Config.EnableTip,
                setValue: value => Config.EnableTip = value
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.enabletipcloselove"),
                getValue: () => "" + Config.TipLove,
                setValue: delegate (string value) { try { Config.TipLove = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.enabletipcloselike"),
                getValue: () => "" + Config.TipLike,
                setValue: delegate (string value) { try { Config.TipLike = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.enabletipcloseneutral"),
                getValue: () => "" + Config.TipNeutral,
                setValue: delegate (string value) { try { Config.TipNeutral = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.enabletipclosedislike"),
                getValue: () => "" + Config.TipDislike,
                setValue: delegate (string value) { try { Config.TipDislike = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.enabletipclosehate"),
                getValue: () => "" + Config.TipHate,
                setValue: delegate (string value) { try { Config.TipHate = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );

            // Advance page
            configMenu.AddPage(mod: ModManifest, "advance", () => SHelper.Translation.Get("foodstore.config.advance"));
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.signrange"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.signrangeText"),
                getValue: () => "" + Config.SignRange,
                setValue: delegate (string value) { try { Config.SignRange = int.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            //configMenu.AddNumberOption(
            //    mod: ModManifest,
            //    name: () => SHelper.Translation.Get("foodstore.config.pathcheck"),
            //    tooltip: () => SHelper.Translation.Get("foodstore.config.pathcheckText"),
            //    getValue: () => Config.NPCCheckTimer,
            //    setValue: value => Config.NPCCheckTimer = (int)value,
            //    min: 0,
            //    max: 7,
            //    interval: 1
            //);
            //configMenu.AddBoolOption(
            //    mod: ModManifest,
            //    name: () => SHelper.Translation.Get("foodstore.config.advancenpcfix"),
            //    tooltip: () => SHelper.Translation.Get("foodstore.config.advancenpcfixText"),
            //    getValue: () => Config.AdvanceNpcFix,
            //    setValue: value => Config.AdvanceNpcFix = value
            //);
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("foodstore.config.advanceoutputitemid"),
                tooltip: () => SHelper.Translation.Get("foodstore.config.advanceoutputitemidText"),
                getValue: () => Config.AdvanceOutputItemId,
                setValue: value => Config.AdvanceOutputItemId = value
            );
        }

    }

}