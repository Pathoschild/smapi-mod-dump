/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Fishnets.Patches;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;
using Microsoft.Xna.Framework;
using System;

namespace Fishnets
{
    public class ModEntry : Mod
    {
        public static int FishNetId { get; private set; } = -1; //931; - For TexturePatch
        public static string ModDataKey => $"{IHelper.ModRegistry.ModID}.FishNets";
        internal static bool HasQualityBait => IHelper.ModRegistry.IsLoaded("MindMeltMax.QualityBait");
        internal static bool HasAlternativeTextures => IHelper.ModRegistry.IsLoaded("PeacefulEnd.AlternativeTextures");
        internal static bool HasJsonAssets => IHelper.ModRegistry.IsLoaded("spacechase0.JsonAssets");
        internal static bool HasDynamicGameAssets => IHelper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets");

        internal static IModHelper IHelper;
        internal static IMonitor IMonitor;
        internal static ITranslationHelper i18n;
        internal static IApi IApi;
        internal static IQualityBaitApi IQualityBaitApi;
        internal static IAlternativeTexturesApi IAlternativeTexturesApi;
        internal static IJsonAssetsApi IJsonAssetsApi;
        internal static IDynamicGameAssetsApi IDynamicGameAssetsApi;

        private int lastObjectId;
        private Rectangle? fishNetTextureLocation;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;
            i18n = Helper.Translation;

            Helper.Events.GameLoop.GameLaunched += onGameLaunched;
            Helper.Events.Content.AssetRequested += onAssetRequested;
            Helper.Events.GameLoop.DayStarted += onDayStarted;
            Helper.Events.GameLoop.DayEnding += onDayEnding;

            _ = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
            _ = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
            _ = Game1.content.Load<Texture2D>("Maps\\springobjects");
        }

        public override object GetApi() => IApi ??= new Api();

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Patcher.Patch(Helper);
            if (HasQualityBait)
                IQualityBaitApi = Helper.ModRegistry.GetApi<IQualityBaitApi>("MindMeltMax.QualityBait");
            if (HasAlternativeTextures)
                IAlternativeTexturesApi = Helper.ModRegistry.GetApi<IAlternativeTexturesApi>("PeacefulEnd.AlternativeTextures");
            if (HasJsonAssets)
                IJsonAssetsApi = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (HasDynamicGameAssets)
                IDynamicGameAssetsApi = Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");
        }

        private void onDayEnding(object sender, DayEndingEventArgs e)
        {
            if (!Context.IsMainPlayer) return;
            // Fishnets are removed from every location once the day ends and added to ModData
            // to avoid Crashes / Broken objects
            foreach (var l in Game1.locations)
            {
                if (l.Objects is null || l.Objects.Count() <= 0)
                {
                    if (l.modData.ContainsKey(ModDataKey))
                        l.modData.Remove(ModDataKey);
                    continue;
                }

                var fishNets = l.Objects.Values.Where(x => x is FishNet);
                var serializable = new List<FishNet.FishNetSerializable>();
                foreach (var f in fishNets) 
                {
                    f.DayUpdate(l);
                    serializable.Add(new((FishNet)f));
                }

                if (serializable is not null && serializable.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(serializable);
                    l.modData[ModDataKey] = json;
                }
                else l.modData.Remove(ModDataKey);

                foreach (var f in fishNets)
                    l.Objects.Remove(f.TileLocation);
            }
        }

        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.player.FishingLevel >= 6 && !Game1.player.knowsRecipe("Fish Net"))
                Game1.player.craftingRecipes.Add("Fish Net", 0);

            if (!Context.IsMainPlayer) return;

            foreach (var l in Game1.locations)
            {
                if (!l.modData.ContainsKey(ModDataKey)) continue;

                string json = l.modData[ModDataKey];
                var deserialized = JsonConvert.DeserializeObject<List<FishNet.FishNetSerializable>>(json);
                
                foreach (var f in deserialized)
                {
                    var fishNet = new FishNet(f.Tile);
                    fishNet.owner.Value = f.Owner;
                    if (f.Bait >= 0)
                        fishNet.bait.Value = new Object(f.Bait, 1) { Quality = f.BaitQuality };
                    fishNet.heldObject.Value = Statics.GetObjectFromSerializable(f);
                    if (!l.Objects.ContainsKey(f.Tile))
                        l.Objects.Add(f.Tile, fishNet);

                    //If fishnet failed to update previously, try again
                    fishNet.DayUpdate(l);
                }

                l.modData.Remove(ModDataKey);
            }
        }

        private void onAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    Monitor.VerboseLog("Entered patch for Data/ObjectInformation");
                    var data = asset.AsDictionary<int, string>().Data;
                    if (FishNetId == -1)
                    {
                        lastObjectId = data.Keys.Last();
                        FishNetId = data.Keys.Last() + 1;
                        Monitor.Log($"Loaded Fishing Nets with id : {FishNetId}");
                    }
                    Monitor.VerboseLog($"FishNetId : {FishNetId}");
                    data[FishNetId] = $"Fish Net/50/-300/Crafting/{i18n.Get("Name")}/{i18n.Get("Description")}";
                    Monitor.VerboseLog("Exiting patch for Data/ObjectInformation");
                }, AssetEditPriority.Early);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    try
                    {
                        if (FishNetId == -1)
                        {
                            Monitor.VerboseLog("Tried to apply texture to fishnet before it was added to object information, Exiting");
                            return;
                        }
                        Monitor.VerboseLog("Entered patch for Data/CraftingRecipes");
                        var data = asset.AsDictionary<string, string>().Data;
                        data["Fish Net"] = $"335 3 771 30/Field/{FishNetId}/false/Fishing 6";
                        Monitor.VerboseLog("Exiting patch for Data/CraftingRecipes");
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log("Error thrown in early patch for Data/CraftingRecipes", LogLevel.Error);
                        Monitor.Log($"{ex.GetType().Name} : {ex.Message} - {ex.StackTrace}");
                    }
                }, AssetEditPriority.Early);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Maps/springobjects"))
            {
                e.Edit(asset =>
                {
                    if (FishNetId <= -1)
                    {
                        Monitor.VerboseLog("Tried to apply texture to fishnet before it was added to object information, Exiting");
                        return;
                    }
                    Monitor.VerboseLog("Entered early patch for Maps/springobjects");
                    try
                    {
                        var img = asset.AsImage();
                        var fishNetTexture = Helper.ModContent.Load<Texture2D>("assets/FishNet.png");
                        int columnPosition = lastObjectId % 24;
                        if (columnPosition == 0)
                            img.ExtendImage(img.Data.Width, img.Data.Height + 16);
                        fishNetTextureLocation ??= new(img.Data.Bounds.X + (16 * columnPosition) + 16, img.Data.Bounds.Y + img.Data.Height - 16, 16, 16);
                        //fishNetTextureLocation ??= new(304, 608, 16, 16);
                        Monitor.VerboseLog($"Texture Area : (X:{fishNetTextureLocation.Value.X}-Y:{fishNetTextureLocation.Value.Y}-Width:{fishNetTextureLocation.Value.Width}-Height:{fishNetTextureLocation.Value.Height})\nSource Area : (X:{img.Data.Bounds.X}-Y:{img.Data.Bounds.Y}-Width:{img.Data.Bounds.Width}-Height:{img.Data.Bounds.Height})");
                        img.PatchImage(fishNetTexture, targetArea: fishNetTextureLocation.Value);
                    }
                    catch (Exception ex) 
                    { 
                        Monitor.Log("Error thrown in early patch for Maps/springobjects", LogLevel.Error); 
                        Monitor.Log($"{ex.GetType().Name} : {ex.Message} - {ex.StackTrace}"); 
                    }
                    Monitor.VerboseLog("Exiting early patch for Maps/springobjects");
                }, AssetEditPriority.Early);

                e.Edit(asset =>
                {
                    if (FishNetId <= -1)
                    {
                        Monitor.VerboseLog("Tried to apply texture to fishnet before it was added to object information, Exiting");
                        return;
                    }
                    Monitor.VerboseLog("Entered late patch for Maps/springobjects");
                    try
                    {
                        var img = asset.AsImage();
                        var fishNetTexture = Helper.ModContent.Load<Texture2D>("assets/FishNet.png");
                        int columnPosition = lastObjectId % 24;
                        if (columnPosition == 0)
                            img.ExtendImage(img.Data.Width, img.Data.Height + 16);
                        fishNetTextureLocation ??= new(img.Data.Bounds.X + (16 * columnPosition) + 16, img.Data.Bounds.Y + img.Data.Height - 16, 16, 16);
                        //fishNetTextureLocation ??= new(304, 608, 16, 16);
                        Monitor.VerboseLog($"Texture Area : (X:{fishNetTextureLocation.Value.X}-Y:{fishNetTextureLocation.Value.Y}-Width:{fishNetTextureLocation.Value.Width}-Height:{fishNetTextureLocation.Value.Height})\nSource Area : (X:{img.Data.Bounds.X}-Y:{img.Data.Bounds.Y}-Width:{img.Data.Bounds.Width}-Height:{img.Data.Bounds.Height})");
                        img.PatchImage(fishNetTexture, targetArea: fishNetTextureLocation.Value);
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log("Error thrown in late patch for Maps/springobjects", LogLevel.Error);
                        Monitor.Log($"{ex.GetType().Name} : {ex.Message} - {ex.StackTrace}");
                    }
                    Monitor.VerboseLog("Exiting late patch for Maps/springobjects");
                }, AssetEditPriority.Late);
            }
        }
    }

    public interface IQualityBaitApi
    {
        int GetQuality(int currentQuality, int baitQuality);
    }

    public interface IAlternativeTexturesApi
    {
        Texture2D GetTextureForObject(Object obj, out Rectangle sourceRect);
    }

    public interface IJsonAssetsApi
    {
        int GetObjectId(string name);
    }

    public interface IDynamicGameAssetsApi
    {
        string GetDGAItemId(object item);
        object SpawnDGAItem(string fullId);
    }
}
