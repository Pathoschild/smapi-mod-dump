/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

global using Object = StardewValley.Object;
using Fishnets.Data;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using System.Collections.Generic;
using System.Linq;


namespace Fishnets
{
    internal class ModEntry : Mod //Will soon revisit to do away with the Fishnet.cs object patch and just patch the functions I need in
    {
        public static string ModDataKey => $"{IHelper.ModRegistry.ModID}.FishNets";

        internal static bool HasQualityBait => IHelper.ModRegistry.IsLoaded("MindMeltMax.QualityBait");
        internal static bool HasAlternativeTextures => IHelper.ModRegistry.IsLoaded("PeacefulEnd.AlternativeTextures");
        internal static bool HasJsonAssets => IHelper.ModRegistry.IsLoaded("spacechase0.JsonAssets");
        internal static bool HasDynamicGameAssets => IHelper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets");
        internal static bool HasSaveAnywhere => IHelper.ModRegistry.IsLoaded("Omegasis.SaveAnywhere");

        internal static IModHelper IHelper;
        internal static IMonitor IMonitor;
        internal static ITranslationHelper I18n;
        internal static IApi IApi;

        internal static IQualityBaitApi IQualityBaitApi;
        internal static IAlternativeTexturesApi IAlternativeTexturesApi;
        internal static IJsonAssetsApi IJsonAssetsApi;
        internal static IDynamicGameAssetsApi IDynamicGameAssetsApi;
        internal static ISaveAnywhereApi ISaveAnywhereApi;

        internal static ObjectInformation ObjectInfo;
        private bool fromMidDay = false;
        private bool validateInventory = true;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;
            I18n = Helper.Translation;

            ObjectInfo = Helper.Data.ReadJsonFile<ObjectInformation>($"assets/data.json");
            Statics.ExcludedFish = Helper.Data.ReadJsonFile<List<string>>($"assets/excludedfish.json");

            Helper.Events.GameLoop.GameLaunched += onGameLaunched;
            Helper.Events.GameLoop.DayStarted += onDayStarted;
            Helper.Events.GameLoop.DayEnding += onDayEnding;
            Helper.Events.Content.AssetRequested += onAssetRequested;
            Helper.Events.GameLoop.ReturnedToTitle += (_, _) => validateInventory = true;
        }

        public override object GetApi() => IApi ??= new Api();

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Patches.Patch(IHelper);
            if (HasQualityBait)
                IQualityBaitApi = Helper.ModRegistry.GetApi<IQualityBaitApi>("MindMeltMax.QualityBait");
            if (HasAlternativeTextures)
                IAlternativeTexturesApi = Helper.ModRegistry.GetApi<IAlternativeTexturesApi>("PeacefulEnd.AlternativeTextures");
            if (HasJsonAssets)
                IJsonAssetsApi = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (HasDynamicGameAssets)
                IDynamicGameAssetsApi = Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");
            if (HasSaveAnywhere)
                ISaveAnywhereApi = Helper.ModRegistry.GetApi<ISaveAnywhereApi>("Omegasis.SaveAnywhere");
            if (ISaveAnywhereApi is not null)
            {
                ISaveAnywhereApi.addBeforeSaveEvent(Helper.ModRegistry.ModID, () =>
                {
                    fromMidDay = true;
                    onDayEnding(null, null);
                    fromMidDay = false;
                });
                ISaveAnywhereApi.addAfterLoadEvent(Helper.ModRegistry.ModID, () =>
                {
                    fromMidDay = true;
                    onDayStarted(null, null);
                    fromMidDay = false;
                });
                ISaveAnywhereApi.addAfterSaveEvent(Helper.ModRegistry.ModID, () =>
                {
                    fromMidDay = true;
                    onDayStarted(null, null);
                    fromMidDay = false;
                });
            }
        }

        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.player.FishingLevel >= 6 && !Game1.player.knowsRecipe("Fish Net"))
                Game1.player.craftingRecipes.Add("Fish Net", 0);

            if (validateInventory) //Check if the item's id has changed, so the player doesn't end up with bugged objects
            {
                var items = new List<Item>(Game1.player.Items.Where(x => x is not null));
                foreach (var item in items)
                    if (item.Name == "Fish Net")
                        Game1.player.Items[Game1.player.Items.IndexOf(item)] = new Object(ObjectInfo.Id, item.Stack, quality: item.Quality);
                validateInventory = false;
            }

            if (!Context.IsMainPlayer)
                return;

            foreach (var l in Game1.locations)
            {
                if (!l.modData.ContainsKey(ModDataKey))
                    continue;

                string json = l.modData[ModDataKey];
                var deserialized = JsonConvert.DeserializeObject<List<FishNetSerializable>>(json);

                foreach (var f in deserialized)
                {
                    var fishNet = new Fishnet(f.Tile);
                    fishNet.owner.Value = f.Owner;
                    if (!string.IsNullOrWhiteSpace(f.Bait))
                        fishNet.bait.Value = (Object?)ItemRegistry.Create(f.Bait, 1, f.BaitQuality, true);//new(f.Bait, 1) { Quality = f.BaitQuality };
                    fishNet.heldObject.Value = Statics.GetObjectFromSerializable(f);
                    if (!l.Objects.ContainsKey(f.Tile))
                        l.Objects.Add(f.Tile, fishNet);

                    //If fishnet failed to update previously, try again
                    if (!fromMidDay)
                        fishNet.DayUpdate();
                }

                l.modData.Remove(ModDataKey);
            }
        }

        private void onDayEnding(object sender, DayEndingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;
            // Fishnets are (still) removed from every location once the day ends and added to ModData
            // to avoid Crashes / Broken objects
            foreach (var l in Game1.locations)
            {
                if (l.Objects is null || l.Objects.Count() <= 0)
                {
                    if (l.modData.ContainsKey(ModDataKey))
                        l.modData.Remove(ModDataKey);
                    continue;
                }

                var fishNets = l.Objects.Values.Where(x => x is Fishnet).Cast<Fishnet>();
                var serializable = new List<FishNetSerializable>();
                foreach (var f in fishNets)
                {
                    if (!fromMidDay)
                        f.DayUpdate();
                    serializable.Add(new(f));
                }

                if (serializable.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(serializable);
                    l.modData[ModDataKey] = json;
                }
                else
                    l.modData.Remove(ModDataKey);

                foreach (var f in fishNets)
                    l.Objects.Remove(f.TileLocation);
            }
        }

        private void onAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    Monitor.LogOnce($"Loaded Fish Nets with id : {ObjectInfo.Id}");
                    ObjectInfo.Object.DisplayName = I18n.Get("Name");
                    ObjectInfo.Object.Description = I18n.Get("Description");
                    data[ObjectInfo.Id] = ObjectInfo.Object;
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["Fish Net"] = string.Format(ObjectInfo.Recipe, ObjectInfo.Id, I18n.Get("Name"));
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Fishnets/Fishnet"))
                e.LoadFromModFile<Texture2D>("assets/FishNet.png", AssetLoadPriority.Exclusive);
        }
    }
}
