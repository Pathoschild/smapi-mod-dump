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

namespace Fishnets
{
    public class ModEntry : Mod
    {
        public static int FishNetId { get; private set; } = -1;
        public static string ModDataKey => $"{IHelper.ModRegistry.ModID}.FishNets";

        internal static IModHelper IHelper;
        internal static IMonitor IMonitor;
        internal static ITranslationHelper i18n;
        internal static IApi IApi;

        private int lastObjectId;
        private Rectangle? fishNetTextureLocation;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;
            i18n = Helper.Translation;

            Helper.Events.GameLoop.GameLaunched += (s, e) => Patcher.Patch(helper);
            Helper.Events.Content.AssetRequested += onAssetRequested;
            Helper.Events.GameLoop.DayStarted += onDayStarted;
            Helper.Events.GameLoop.DayEnding += onDayEnding;

            Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
            Game1.content.Load<Texture2D>("Maps\\springobjects");
            Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
        }

        public override object GetApi() => IApi ??= new Api();

        private void onDayEnding(object sender, DayEndingEventArgs e)
        {
            if (!Context.IsMainPlayer) return;
            // Fishnets are removed from every location once the day ends and added to ModData
            // to avoid Crashes / Broken objects
            foreach (var l in Game1.locations)
            {
                if (l.Objects is null || l.Objects.Count() <= 0) continue;

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
                        fishNet.bait.Value = new Object(f.Bait, 1);
                    if (f.ObjectId >= 0)
                        fishNet.heldObject.Value = new Object(f.ObjectId, 1);
                    if (!l.Objects.ContainsKey(f.Tile))
                        l.Objects.Add(f.Tile, fishNet);
                }
            }
        }

        private void onAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<int, string>().Data;
                    if (FishNetId == -1) 
                    { 
                        lastObjectId = data.Keys.Last();
                        FishNetId = data.Keys.Last() + 1;
                    }
                    data[FishNetId] = $"Fish Net/50/-300/Crafting/{i18n.Get("Name")}/{i18n.Get("Description")}";
                }, AssetEditPriority.Late);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["Fish Net"] = $"335 3 771 30/Field/{FishNetId}/false/Fishing 6";
                }, AssetEditPriority.Late);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Maps/springobjects"))
            {
                e.Edit(asset =>
                {
                    var img = asset.AsImage();
                    var fishNetTexture = Helper.ModContent.Load<Texture2D>("assets/FishNet.png");
                    int columnPosition = lastObjectId % 24;
                    bool shouldExtend = columnPosition == 0;
                    if (shouldExtend)
                        img.ExtendImage(img.Data.Width, img.Data.Height + 16);
                    fishNetTextureLocation ??= new(img.Data.Bounds.X + (16 * columnPosition) + 16, img.Data.Bounds.Y + img.Data.Height - 16, 16, 16);
                    img.PatchImage(fishNetTexture, targetArea: fishNetTextureLocation);
                }, AssetEditPriority.Late);
            }
        }
    }
}
