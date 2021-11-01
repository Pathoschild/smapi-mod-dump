/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace GarbageDay
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Helpers;
    using Common.Helpers.ItemRepository;
    using Common.Integrations.EvenBetterRNG;
    using Common.Integrations.XSLite;
    using Microsoft.Xna.Framework;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.Characters;
    using StardewValley.Locations;
    using StardewValley.Menus;
    using StardewValley.Objects;
    using xTile;
    using xTile.Dimensions;

    public class GarbageDay : Mod, IAssetEditor, IAssetLoader
    {
        private static readonly ItemRepository ItemRepository = new();
        private readonly PerScreen<Chest> _chest = new();
        private readonly HashSet<string> _excludedAssets = new();
        private readonly IDictionary<string, GarbageCan> _garbageCans = new Dictionary<string, GarbageCan>();
        private readonly PerScreen<NPC> _npc = new();
        private ModConfig _config;
        private Multiplayer _multiplayer;
        private XSLiteIntegration _xsLite;

        internal static IDictionary<string, IDictionary<string, float>> Loot { get; private set; }

        internal static IEnumerable<SearchableItem> Items
        {
            get => GarbageDay.ItemRepository.GetAll();
        }

        internal static EvenBetterRngIntegration BetterRng { get; private set; }

        /// <inheritdoc />
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.DataType == typeof(Map) && !this._excludedAssets.Contains(asset.AssetName);
        }

        /// <inheritdoc />
        public void Edit<T>(IAssetData asset)
        {
            var map = asset.AsMap().Data;
            if (!map.Properties.TryGetValue("GarbageDay", out var mapLoot))
            {
                if (!asset.AssetNameEquals(@"Maps\Town"))
                {
                    this._excludedAssets.Add(asset.AssetName);
                    return;
                }

                mapLoot = "Town";
            }

            for (var x = 0; x < map.Layers[0].LayerWidth; x++)
            {
                for (var y = 0; y < map.Layers[0].LayerHeight; y++)
                {
                    var layer = map.GetLayer("Buildings");
                    var tile = layer.PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                    if (tile is null)
                    {
                        continue;
                    }

                    // Look for Action: Garbage [WhichCan]
                    tile.Properties.TryGetValue("Action", out var property);
                    if (property is null)
                    {
                        continue;
                    }

                    var parts = property.ToString().Split(' ');
                    if (parts.Length != 2 || parts[0] != "Garbage")
                    {
                        continue;
                    }

                    var whichCan = parts[1];
                    if (string.IsNullOrWhiteSpace(whichCan))
                    {
                        continue;
                    }

                    if (!this._garbageCans.TryGetValue(whichCan, out var garbageCan))
                    {
                        garbageCan = new(PathUtilities.NormalizeAssetName(asset.AssetName), mapLoot, whichCan, new(x, y));
                        this._garbageCans.Add(whichCan, garbageCan);
                    }

                    // Remove base tile
                    if (layer.Tiles[x, y] is not null && layer.Tiles[x, y].TileSheet.Id == "Town" && layer.Tiles[x, y].TileIndex == 78)
                    {
                        layer.Tiles[x, y] = null;
                    }

                    // Remove Lid tile
                    layer = map.GetLayer("Front");
                    if (layer.Tiles[x, y] is not null && layer.Tiles[x, y].TileSheet.Id == "Town" && layer.Tiles[x, y].TileIndex == 46)
                    {
                        layer.Tiles[x, y] = null;
                    }

                    // Add NoPath to tile
                    map.GetLayer("Back").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size)?.Properties.Add("NoPath", string.Empty);
                }
            }
        }

        /// <inheritdoc />
        public bool CanLoad<T>(IAssetInfo asset)
        {
            var segments = PathUtilities.GetSegments(asset.AssetName);
            return segments.Length == 2
                   && segments.ElementAt(0).Equals("GarbageDay", StringComparison.OrdinalIgnoreCase)
                   && segments.ElementAt(1).Equals("Loot", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public T Load<T>(IAssetInfo asset)
        {
            return (T)this.Helper.Content.Load<IDictionary<string, IDictionary<string, float>>>("assets/loot.json");
        }

        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            Log.Init(this.Monitor);

            this._xsLite = new(this.Helper.ModRegistry);
            GarbageDay.BetterRng = new(this.Helper.ModRegistry);
            this._config = this.Helper.ReadConfig<ModConfig>();

            // Console Commands
            this.Helper.ConsoleCommands.Add("garbage_fill", "Adds loot to all Garbage Cans.", this.GarbageFill);
            this.Helper.ConsoleCommands.Add("garbage_kill", "Removes all Garbage Cans.", this.GarbageKill);

            // Events
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            if (Context.IsMainPlayer)
            {
                helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
                helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            }

            helper.Events.Display.MenuChanged += this.OnMenuChanged;
        }

        private void GarbageFill(string command, string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out var amount))
            {
                amount = 1;
            }

            foreach (var garbageCan in this._garbageCans)
            {
                if (garbageCan.Value.Chest is null)
                {
                    continue;
                }

                for (var i = 0; i < amount; i++)
                {
                    garbageCan.Value.AddLoot();
                }
            }
        }

        private void GarbageKill(string command, string[] args)
        {
            foreach (var garbageCan in this._garbageCans)
            {
                if (garbageCan.Value.Location.Objects.TryGetValue(garbageCan.Value.Tile, out var obj) && obj is Chest)
                {
                    garbageCan.Value.Location.Objects.Remove(garbageCan.Value.Tile);
                }
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this._multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            // Load GarbageCan using XSLite
            this._xsLite.API.LoadContentPack(this.ModManifest, this.Helper.DirectoryPath);
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            var locations = Game1.locations.Concat(Game1.locations.OfType<BuildableGameLocation>().SelectMany(location => location.buildings.Where(building => building.indoors.Value is not null).Select(building => building.indoors.Value)));

            foreach (var location in locations)
            {
                var mapPath = PathUtilities.NormalizeAssetName(location.mapPath.Value);
                foreach (var garbageCan in this._garbageCans.Where(gc => gc.Value.MapName.Equals(mapPath)))
                {
                    garbageCan.Value.Location = location;
                }

                var objects = location.Objects.Pairs.Where(obj => obj.Value is Chest chest && chest.playerChest.Value && chest.modData.TryGetValue("furyx639.ExpandedStorage/Storage", out var storage) && storage == "Garbage Can");
                foreach (var obj in objects)
                {
                    if (obj.Value.modData.TryGetValue("furyx639.GarbageDay/WhichCan", out var whichCan) && this._garbageCans.ContainsKey(whichCan))
                    {
                        continue;
                    }

                    // Remove invalid cans
                    Log.Trace($"Removing invalid Garbage Can {whichCan} at {location.Name}");
                    location.Objects.Remove(obj.Key);
                }
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            GarbageDay.Loot = this.Helper.Content.Load<IDictionary<string, IDictionary<string, float>>>("GarbageDay/Loot", ContentSource.GameContent);
            foreach (var garbageCan in this._garbageCans)
            {
                if (garbageCan.Value.Chest is null)
                {
                    continue;
                }

                // Empty chest on garbage day
                if (Game1.dayOfMonth % 7 == this._config.GarbageDay % 7)
                {
                    garbageCan.Value.Chest.items.Clear();
                }

                garbageCan.Value.AddLoot();
            }
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // Open Can
            if (e.NewMenu is ItemGrabMenu {context: Chest chest} && chest.modData.TryGetValue("furyx639.GarbageDay/WhichCan", out var whichCan) && this._garbageCans.TryGetValue(whichCan, out var garbageCan))
            {
                var character = Utility.isThereAFarmerOrCharacterWithinDistance(garbageCan.Tile, 7, garbageCan.Location);
                if (character is not (NPC npc and not Horse))
                {
                    return;
                }

                this._npc.Value = npc;
                this._chest.Value = chest;
                this._multiplayer.globalChatInfoMessage("TrashCan", Game1.player.Name, npc.Name);
                if (npc.Name.Equals("Linus"))
                {
                    npc.doEmote(32);
                    npc.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Linus"), true, true);
                    Game1.player.changeFriendship(5, npc);
                    this._multiplayer.globalChatInfoMessage("LinusTrashCan");
                }
                else
                {
                    switch (npc.Age)
                    {
                        case 2:
                            npc.doEmote(28);
                            npc.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Child"), true, true);
                            Game1.player.changeFriendship(-25, npc);
                            break;
                        case 1:
                            npc.doEmote(8);
                            npc.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Teen"), true, true);
                            Game1.player.changeFriendship(-25, npc);
                            break;
                        default:
                            npc.doEmote(12);
                            npc.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Adult"), true, true);
                            Game1.player.changeFriendship(-25, npc);
                            break;
                    }
                }

                garbageCan.CheckAction();
            }

            // Close Can
            else if (e.OldMenu is ItemGrabMenu && this._npc.Value is not null)
            {
                Game1.drawDialogue(this._npc.Value);
                if (!this._chest.Value.items.Any() && !this._chest.Value.playerChoiceColor.Value.Equals(Color.Black))
                {
                    this._chest.Value.playerChoiceColor.Value = Color.DarkGray;
                }

                this._npc.Value = null;
                this._chest.Value = null;
            }
        }
    }
}