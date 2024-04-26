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
using Fishnets.Integrations;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;

namespace Fishnets
{
    internal class ModEntry : Mod
    {
        public static string ModDataKey => $"{IHelper.ModRegistry.ModID}.FishNets";

        public static string ModDataTileIndexKey => $"{IHelper.ModRegistry.ModID}.TileIndex";

        internal static bool HasQualityBait => IHelper.ModRegistry.IsLoaded("MindMeltMax.QualityBait");
        internal static bool HasAlternativeTextures => IHelper.ModRegistry.IsLoaded("PeacefulEnd.AlternativeTextures");
        internal static bool HasJsonAssets => IHelper.ModRegistry.IsLoaded("spacechase0.JsonAssets");
        internal static bool HasBetterCrafting => IHelper.ModRegistry.IsLoaded("leclair.bettercrafting");
        internal static bool HasGMCM => IHelper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu");

        internal static IModHelper IHelper;
        internal static IMonitor IMonitor;
        internal static ITranslationHelper I18n;
        internal static IApi IApi;
        internal static Config IConfig;

        internal static IQualityBaitApi IQualityBaitApi;
        internal static IAlternativeTexturesApi IAlternativeTexturesApi;
        internal static IJsonAssetsApi IJsonAssetsApi;
        internal static IBetterCraftingApi IBetterCraftingApi;
        internal static IGMCMApi IGMCMApi;

        internal static ObjectInformation ObjectInfo;
        internal static bool NoSound = false; //Avoid sound bomb on backwards compat load (MY EARS!)

        private bool validateInventory = true;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;
            I18n = Helper.Translation;
            IConfig = Helper.ReadConfig<Config>();

            ObjectInfo = Helper.Data.ReadJsonFile<ObjectInformation>($"assets/data.json")!;
            Statics.ExcludedFish = Helper.Data.ReadJsonFile<List<string>>($"assets/excludedfish.json")!;
            Helper.Events.GameLoop.GameLaunched += onGameLaunched;
            Helper.Events.GameLoop.DayStarted += onDayStarted;
            Helper.Events.Content.AssetRequested += onAssetRequested;
            Helper.Events.GameLoop.ReturnedToTitle += (_, _) => validateInventory = true;
        }

        public override object GetApi() => IApi ??= new Api();

        private void onGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            Patches.Patch(ModManifest.UniqueID);
            if (HasQualityBait)
                IQualityBaitApi = Helper.ModRegistry.GetApi<IQualityBaitApi>("MindMeltMax.QualityBait");
            if (HasAlternativeTextures)
                IAlternativeTexturesApi = Helper.ModRegistry.GetApi<IAlternativeTexturesApi>("PeacefulEnd.AlternativeTextures");
            if (HasJsonAssets)
                IJsonAssetsApi = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (HasBetterCrafting)
                IBetterCraftingApi = Helper.ModRegistry.GetApi<IBetterCraftingApi>("leclair.bettercrafting");
            if (HasGMCM)
                IGMCMApi = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");

            IBetterCraftingApi?.AddRecipesToDefaultCategory(false, "fishing", ["Fish Net"]);
            if (IGMCMApi is not null)
                registerForGMCM();
        }

        private void onDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (Game1.player.FishingLevel >= 6 && !Game1.player.knowsRecipe("Fish Net"))
                Game1.player.craftingRecipes.Add("Fish Net", 0);

            if (validateInventory) //Check if the items id has changed, so the player doesn't end up with bugged objects
            {
                var items = new List<Item>(Game1.player.Items.Where(x => x is not null));
                foreach (var item in items)
                    if (item.Name == "Fish Net" && item.ItemId != ObjectInfo.Id)
                        Game1.player.Items[Game1.player.Items.IndexOf(item)] = new Object(ObjectInfo.Id, item.Stack, quality: item.Quality);
            }

            if (!Context.IsMainPlayer)
            {
                if (validateInventory)
                    validateInventory = false;
                return;
            }

            NoSound = true;
            foreach (var l in Game1.locations) //Apply save load / day update fixes
            {
                if (validateInventory)
                {
                    var objects = l.Objects.Keys;
                    foreach (var obj in objects)
                    {
                        if (l.Objects[obj].Name == "Fish Net" && l.Objects[obj].ItemId != ObjectInfo.Id)
                        {
                            Object o = new(ObjectInfo.Id, 1, quality: l.Objects[obj].Quality);
                            var modData = Statics.GetModDataAt(l, obj);
                            Statics.OnRemove(l, obj);
                            l.Objects.Remove(obj);
                            o.placementAction(l, (int)obj.X, (int)obj.Y, Game1.getFarmerMaybeOffline(l.Objects[obj].owner.Value));
                            Statics.SetModDataAt(l, obj, modData! with { BaitId = modData.BaitId, BaitQuality = modData.BaitQuality });
                            l.Objects[obj].DayUpdate();
                        }
                    }
                }

                //Backwards compatibility
                if (!l.modData.ContainsKey(ModDataKey))
                    continue;

                string json = l.modData[ModDataKey];
                var deserialized = JsonConvert.DeserializeObject<List<FishNetSerializable>>(json) ?? [];

                foreach (var f in deserialized)
                {
                    Object o = ItemRegistry.Create<Object>(ObjectInfo.Id);
                    o.heldObject.Value = Statics.GetObjectFromSerializable(f);
                    o.placementAction(l, (int)f.Tile.X * 64, (int)f.Tile.Y * 64, Game1.getFarmerMaybeOffline(f.Owner));
                    Statics.SetModDataAt(l, f.Tile, Statics.GetModDataAt(l, f.Tile)! with { BaitId = f.Bait, BaitQuality = f.BaitQuality });
                    l.Objects[f.Tile].DayUpdate();
                }

                l.modData.Remove(ModDataKey);
            }
            NoSound = false;
            validateInventory = false;
        }

        private void onAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    Monitor.LogOnce($"Loaded Fish Nets with id : {ObjectInfo.Id}");
                    ObjectInfo.Object.DisplayName = I18n.Get("Name");
                    ObjectInfo.Object.Description = I18n.Get("Description");
                    ObjectInfo.Object.SpriteIndex = Statics.GetSpriteIndexFromConfig();
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

        private void registerForGMCM()
        {
            IGMCMApi.Register(ModManifest, () => IConfig = new(), () => Helper.WriteConfig(IConfig));

            IGMCMApi.AddNumberOption(ModManifest, () => IConfig.TextureVariant, updateTextureVariant, () => I18n.Get("Config.TextureVariant.Name"), () => I18n.Get("Config.TextureVariant.Description"));

            IGMCMApi.AddBoolOption(ModManifest, () => IConfig.LessTrash, (v) => IConfig.LessTrash = v, () => I18n.Get("Config.LessTrash.Name"), () => I18n.Get("Config.LessTrash.Description"));

            IGMCMApi.AddBoolOption(ModManifest, () => IConfig.LessWeeds, (v) => IConfig.LessWeeds = v, () => I18n.Get("Config.LessWeeds.Name"), () => I18n.Get("Config.LessWeeds.Description"));

            IGMCMApi.AddBoolOption(ModManifest, () => IConfig.LessJelly, (v) => IConfig.LessJelly = v, () => I18n.Get("Config.LessJelly.Name"), () => I18n.Get("Config.LessJelly.Description"));
        }

        private void updateTextureVariant(int variant)
        {
            int height = 16 * variant;
            var texture = Helper.ModContent.Load<Texture2D>("assets/FishNet.png");
            if (height >= texture.Height)
                variant = (texture.Height - 16) / 16;
            IConfig.TextureVariant = variant; 
            Helper.GameContent.InvalidateCache("Data/Objects");
        }
    }
}
