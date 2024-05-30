/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/poohnhi/PoohCore
**
*************************************************/

using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.GameData.Movies;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley.Util;
using xTile;
using xTile.Dimensions;
using System.Collections.Generic;
using Netcode;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using System.Security.Cryptography.X509Certificates;
using ContentPatcher;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using StardewValley.Delegates;
using StardewValley.TokenizableStrings;
using static System.Net.Mime.MediaTypeNames;
using StardewValley.Extensions;
using StardewValley.GameData.Characters;
using StardewValley.GameData.Crops;
using StardewValley.GameData.GiantCrops;
using StardewValley.GameData.Objects;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.Shops;

namespace PoohCore
{
    public partial class ModEntry : Mod
    {
        public static HashSet<string> ListOfModifiedNPC = new HashSet<string> { };
        static List<string> ListOfMailFlag = new List<string> { };
        static string[] universalLoves = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Love"]);
        static string[] universalHates = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Hate"]);
        static string[] universalLikes = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Like"]);
        static string[] universalDislikes = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Dislike"]);
        static string[] universalNeutrals = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Neutral"]);
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.NPC), "behaviorOnFarmerLocationEntry"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.behaviorOnFarmerLocationEntry_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Event), "addActor"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.addActor_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.NPC), "resetForNewDay"),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.resetForNewDay_Prefix))
            );

        }
        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            api.RegisterToken(this.ModManifest, "GetMailFlagProgressNumber", new GetMailFlagProgressNumberToken());
            api.RegisterToken(this.ModManifest, "GetGiftTasteCPTokenFromSomeOne", new GetGiftTasteCPTokenFromSomeOneToken());
            GameStateQuery.Register("poohnhi.PoohCore_HARVEST_ITEM_PRICE", delegate (string[] query, GameStateQueryContext context)
            {
                try
                {
                    if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out var item, out var error) || !ArgUtility.TryGetInt(query, 2, out var minPrice, out error) || !ArgUtility.TryGetOptionalInt(query, 3, out var maxPrice, out error, int.MaxValue))
                    {
                        return GameStateQuery.Helpers.ErrorResult(query, error);
                    }
                    if (item.Category == -74)
                    {
                        // special case: fruit trees will be their fruit*15 for the final price
                        StardewValley.Object obj = new StardewValley.Object(item.ItemId, 1);
                        if (obj.IsFruitTreeSapling())
                        {
                            int? totalPrice = 0;
                            FruitTreeData fruitTreeData;
                            FruitTree.TryGetData(obj.ItemId, out fruitTreeData);
                            if (fruitTreeData?.Fruit != null)
                            {
                                foreach (FruitTreeFruitData entry in fruitTreeData.Fruit)
                                {
                                    Item cropItem = ItemRegistry.Create(entry.ItemId);
                                    if (cropItem != null)
                                    {
                                        totalPrice += cropItem?.sellToStorePrice(); ;
                                    }
                                }
                                int? price = totalPrice*15;
                                if (price >= minPrice)
                                {
                                    /*if (price <= maxPrice)
                                    {
                                        Monitor.Log($"Range is {minPrice} - {maxPrice}. Item {item.DisplayName} have crop price: {price}.", LogLevel.Error);
                                    }*/
                                    return price <= maxPrice;
                                }
                            }
                        }
                        else
                        {
                            // Get crop data from item (assume they're seeds)
                            CropData cropData;
                            Crop.TryGetData(item.ItemId, out cropData);

                            // Get price from that crop and compare it to max and min price
                            if (cropData != null)
                            {
                                Item cropItem = ItemRegistry.Create(cropData.HarvestItemId);
                                int? price = cropItem?.sellToStorePrice();
                                if (price >= minPrice)
                                {
                                    /*if (price <= maxPrice)
                                    {
                                        Monitor.Log($"Range is {minPrice} - {maxPrice}. Item {item.DisplayName} have crop price: {price}.", LogLevel.Error);
                                    }*/
                                    return price <= maxPrice;
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            });
        }
        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            universalLoves = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Love"]);
            universalHates = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Hate"]);
            universalLikes = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Like"]);
            universalDislikes = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Dislike"]);
            universalNeutrals = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Neutral"]);
            foreach (NPC thisChar in Utility.getAllVillagers())
            {
                try
                {
                    if (thisChar != null)
                    {
                        CharacterData data = thisChar.GetData();
                        if (data != null)
                        {
                            try
                            {
                                var test = data.CustomFields.TryGetValue("poohnhi.PoohCore/WideCharacter", out string WideCharacterValue);
                                if (WideCharacterValue != null && WideCharacterValue != "false")
                                ListOfModifiedNPC.Add(thisChar.Name);
                            }
                            catch { }
                            try
                            {
                                var test = data.CustomFields.TryGetValue("poohnhi.PoohCore/HighCharacter", out string HighCharacterValue);
                                if (HighCharacterValue != null && HighCharacterValue != "false")
                                ListOfModifiedNPC.Add(thisChar.Name);
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }
        }

        public void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            foreach (var todayMailFlag in ListOfMailFlag)
            {
                if (Game1.player.mailReceived.Contains(todayMailFlag))
                {
                    Game1.player.mailReceived.Remove(todayMailFlag);
                    string todayMailFlagNumber = GetMailFlagProgressNumberFromMailFlag(todayMailFlag);
                    string seenFlag = todayMailFlag + todayMailFlagNumber;
                    string nextFlag = todayMailFlag + GetNextNumberFromString(todayMailFlagNumber);
                    Game1.player.mailReceived.Remove(seenFlag);
                    Game1.player.mailReceived.Add(nextFlag);
                }
            }
            ListOfMailFlag = new List<string> { };
        }
    }
}
