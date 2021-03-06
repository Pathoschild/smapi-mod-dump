/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MultipleMiniObelisks
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using MultipleMiniObelisks.Multiplayer;
using MultipleMiniObelisks.Objects;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultipleMiniObelisks
{
    public class ModEntry : Mod
    {
        // Monitor, Helper, Config
        internal static IMonitor monitor;
        internal static IModHelper helper;
        internal static ModConfig config;
        internal static IManifest manifest;

        // ModData related
        internal static string ObeliskLocationsKey;
        internal static string ObeliskNameDataKey;

        public override void Entry(IModHelper modHelper)
        {
            // Load the monitor, helper and config
            monitor = this.Monitor;
            helper = modHelper;
            config = helper.ReadConfig<ModConfig>();
            manifest = this.ModManifest;

            // Set the ModData keys we'll be using
            ObeliskLocationsKey = $"{this.ModManifest.UniqueID}/obelisk-locations";
            ObeliskNameDataKey = $"{this.ModManifest.UniqueID}/destination-name";

            // Load our Harmony patches
            try
            {
                var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patch: {e}", LogLevel.Error);
                return;
            }

            // Hook into ModMessageReceived
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;

            // Hook into ObjectListChanged to catch when Mini-Obelisks are placed / removed
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;

            // Hook into save related events
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != this.ModManifest.UniqueID)
            {
                return;
            }

            if (Context.IsMainPlayer)
            {
                if (e.Type == nameof(ObeliskUpdateMessage))
                {
                    ObeliskUpdateMessage message = e.ReadAs<ObeliskUpdateMessage>();

                    Monitor.Log($"Received rename message: {message.Obelisk.CustomName} at {message.Obelisk.LocationName} [{message.Obelisk.Tile.X}, {message.Obelisk.Tile.Y}]", LogLevel.Trace);

                    UpdateObeliskCustomName(message.Obelisk);
                }
                else if (e.Type == nameof(ObeliskTeleportRequestMessage))
                {
                    ObeliskTeleportRequestMessage message = e.ReadAs<ObeliskTeleportRequestMessage>();

                    Monitor.Log($"Received teleport request message from {message.FarmerId} with destination: {message.Obelisk.CustomName} at {message.Obelisk.LocationName} [{message.Obelisk.Tile.X}, {message.Obelisk.Tile.Y}]", LogLevel.Trace);

                    ValidateTeleportDestination(message.Obelisk, message.FarmerId);
                }
            }
            else
            {
                if (e.Type == nameof(ObeliskTeleportStatusMessage))
                {
                    ObeliskTeleportStatusMessage message = e.ReadAs<ObeliskTeleportStatusMessage>();
                    if (message.FarmerId != Game1.player.UniqueMultiplayerID)
                    {
                        return;
                    }

                    Monitor.Log($"Teleport request result: {message.DoTeleport}", LogLevel.Trace);

                    if (message.DoTeleport)
                    {
                        TeleportPlayerToDestination(message.DestinationName, message.DestinationTile);
                        return;
                    }

                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MiniObelisk_NeedsSpace"));
                }
            }
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            if (!Game1.player.modData.ContainsKey(ObeliskLocationsKey))
            {
                Game1.player.modData.Add(ObeliskLocationsKey, JsonConvert.SerializeObject(new List<MiniObelisk>()));
            }

            List<MiniObelisk> miniObelisks = JsonConvert.DeserializeObject<List<MiniObelisk>>(Game1.player.modData[ObeliskLocationsKey]);

            // Add any new obelisks
            foreach (var tileToObelisk in e.Added.Where(o => o.Value.ParentSheetIndex == 238 && o.Value.bigCraftable))
            {
                StardewValley.Object obelisk = tileToObelisk.Value;
                if (!obelisk.modData.ContainsKey(ObeliskNameDataKey))
                {
                    obelisk.modData.Add(ObeliskNameDataKey, String.Empty);
                }

                miniObelisks.Add(new MiniObelisk(e.Location.NameOrUniqueName, tileToObelisk.Key, obelisk.modData[ObeliskNameDataKey]));
            }

            // Remove any removed obelisks
            foreach (var tileToObelisk in e.Removed.Where(o => o.Value.ParentSheetIndex == 238 && o.Value.bigCraftable))
            {
                miniObelisks = miniObelisks.Where(o => !(o.LocationName == e.Location.NameOrUniqueName && o.Tile == tileToObelisk.Key)).ToList();
            }

            Game1.player.modData[ObeliskLocationsKey] = JsonConvert.SerializeObject(miniObelisks);
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            UpdateObeliskCache();
        }

        internal static void UpdateObeliskCustomName(MiniObelisk miniObelisk)
        {
            monitor.Log($"Received rename message for Mini-Obelisk: {miniObelisk.CustomName} at {miniObelisk.LocationName} [{miniObelisk.Tile.X}, {miniObelisk.Tile.Y}]", LogLevel.Debug);

            // Update the Obelisk's ModData[ObeliskNameDataKey]
            GameLocation location = Game1.getLocationFromName(miniObelisk.LocationName);
            StardewValley.Object obelisk = location.getObjectAtTile((int)miniObelisk.Tile.X, (int)miniObelisk.Tile.Y);

            if (!obelisk.modData.ContainsKey(ObeliskNameDataKey))
            {
                obelisk.modData[ObeliskNameDataKey] = String.Empty;
            }
            obelisk.modData[ObeliskNameDataKey] = miniObelisk.CustomName;

            // Update the MasterPlayer.ModData[ObeliskLocationsKey]
            UpdateObeliskCache();
        }

        internal static void UpdateObeliskCache()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            if (!Game1.player.modData.ContainsKey(ObeliskLocationsKey))
            {
                Game1.player.modData.Add(ObeliskLocationsKey, String.Empty);
            }

            List<MiniObelisk> miniObelisks = new List<MiniObelisk>();
            foreach (GameLocation location in Game1.locations)
            {
                if (location.numberOfObjectsOfType(238, true) > 0)
                {
                    foreach (var tileToObject in location.Objects.Pairs.Where(p => p.Value.ParentSheetIndex == 238 && p.Value.bigCraftable))
                    {
                        StardewValley.Object obelisk = tileToObject.Value;
                        if (!obelisk.modData.ContainsKey(ObeliskNameDataKey))
                        {
                            obelisk.modData.Add(ObeliskNameDataKey, String.Empty);
                        }

                        miniObelisks.Add(new MiniObelisk(location.NameOrUniqueName, tileToObject.Key, obelisk.modData[ObeliskNameDataKey]));
                    }
                }

                if (location is BuildableGameLocation)
                {
                    foreach (Building b2 in (location as BuildableGameLocation).buildings)
                    {
                        GameLocation indoorLocation = b2.indoors.Value;
                        if (indoorLocation is null)
                        {
                            continue;
                        }

                        if (indoorLocation.numberOfObjectsOfType(238, true) > 0)
                        {
                            foreach (var tileToObject in indoorLocation.Objects.Pairs.Where(p => p.Value.ParentSheetIndex == 238 && p.Value.bigCraftable))
                            {
                                StardewValley.Object obelisk = tileToObject.Value;
                                if (!obelisk.modData.ContainsKey(ObeliskNameDataKey))
                                {
                                    obelisk.modData.Add(ObeliskNameDataKey, String.Empty);
                                }

                                miniObelisks.Add(new MiniObelisk(indoorLocation.NameOrUniqueName, tileToObject.Key, obelisk.modData[ObeliskNameDataKey]));
                            }
                        }
                    }
                }
            }

            monitor.Log(JsonConvert.SerializeObject(miniObelisks), LogLevel.Trace);
            Game1.player.modData[ObeliskLocationsKey] = JsonConvert.SerializeObject(miniObelisks);
        }

        internal static void ValidateTeleportDestination(MiniObelisk obelisk, long farmerId)
        {
            Vector2 target = obelisk.Tile;
            GameLocation obeliskLocation = Game1.getLocationFromName(obelisk.LocationName);
            foreach (Vector2 v in new List<Vector2>
            {
                new Vector2(target.X, target.Y + 1f),
                new Vector2(target.X - 1f, target.Y),
                new Vector2(target.X + 1f, target.Y),
                new Vector2(target.X, target.Y - 1f)
            })
            {
                if (obeliskLocation.isTileLocationTotallyClearAndPlaceableIgnoreFloors(v))
                {
                    ModEntry.helper.Multiplayer.SendMessage(new ObeliskTeleportStatusMessage(obeliskLocation.NameOrUniqueName, v, farmerId, true), nameof(ObeliskTeleportStatusMessage), modIDs: new[] { ModEntry.manifest.UniqueID });
                    return;
                }
            }

            ModEntry.helper.Multiplayer.SendMessage(new ObeliskTeleportStatusMessage(obeliskLocation.NameOrUniqueName, obelisk.Tile, farmerId, false), nameof(ObeliskTeleportStatusMessage), modIDs: new[] { ModEntry.manifest.UniqueID });
        }

        internal static void TeleportPlayerToDestination(string destinationName, Vector2 destinationTile)
        {
            Farmer who = Game1.player;
            GameLocation obeliskLocation = Game1.getLocationFromName(destinationName);

            for (int i = 0; i < 12; i++)
            {
                who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)who.position.X - 256, (int)who.position.X + 192), Game1.random.Next((int)who.position.Y - 256, (int)who.position.Y + 192)), flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false));
            }
            who.currentLocation.playSound("wand");
            Game1.displayFarmer = false;
            who.temporarilyInvincible = true;
            who.temporaryInvincibilityTimer = -2000;
            who.Halt();
            who.faceDirection(2);
            who.CanMove = false;
            who.freezePause = 2000;
            Game1.flashAlpha = 1f;
            DelayedAction.fadeAfterDelay(delegate
            {
                Game1.warpFarmer(obeliskLocation.NameOrUniqueName, (int)destinationTile.X, (int)destinationTile.Y, flip: false);
                if (!Game1.isStartingToGetDarkOut() && !Game1.isRaining)
                {
                    Game1.playMorningSong();
                }
                else
                {
                    Game1.changeMusicTrack("none");
                }
                Game1.fadeToBlackAlpha = 0.99f;
                Game1.screenGlow = false;
                who.temporarilyInvincible = false;
                who.temporaryInvincibilityTimer = 0;
                Game1.displayFarmer = true;
                who.CanMove = true;
            }, 1000);
            new Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
            int j = 0;
            for (int xTile = who.getTileX() + 8; xTile >= who.getTileX() - 8; xTile--)
            {
                obeliskLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(xTile, who.getTileY()) * 64f, Color.White, 8, flipped: false, 50f)
                {
                    layerDepth = 1f,
                    delayBeforeAnimationStart = j * 25,
                    motion = new Vector2(-0.25f, 0f)
                });
                j++;
            }
        }
    }
}
