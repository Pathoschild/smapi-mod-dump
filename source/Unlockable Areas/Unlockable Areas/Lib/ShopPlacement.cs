/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace Unlockable_Areas.Lib
{
    public class ShopPlacement
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static List<GameLocation> modifiedLocations = new List<GameLocation>();
        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            Helper.Events.GameLoop.DayStarted += dayStarted;
            Helper.Events.World.LocationListChanged += locationListChanged;
        }

        private static void locationListChanged(object sender, LocationListChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
                return;

            var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableAreas/Unlockables");

            foreach (var loc in e.Added)
                foreach (var unlockable in unlockables.Where(el => el.Value.Location == loc.Name)) {
                    unlockable.Value.ID = unlockable.Key;
                    unlockable.Value.LocationUnique = loc.NameOrUniqueName;
                    placeShop(new Unlockable(unlockable.Value));
                }
        }

        public static void dayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer) {
                if (ModData.Instance == null)
                    ModData.Instance = new ModData();
                return;
            }

            ModData.Instance = Helper.Data.ReadSaveData<ModData>("main") ?? new ModData();
            Helper.GameContent.InvalidateCache(asset => asset.NameWithoutLocale.IsEquivalentTo("UnlockableAreas/Unlockables"));
            API.UnlockableAreasAPI.clearCache();
            var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableAreas/Unlockables");
            modifiedLocations = new List<GameLocation>();

            foreach (KeyValuePair<string, UnlockableModel> entry in unlockables) {
                entry.Value.ID = entry.Key;
                validateUnlockable(entry.Value);
                var locations = getLocationsFromName(entry.Value.Location);
                foreach (var location in locations) {
                    entry.Value.LocationUnique = location.NameOrUniqueName;

                    if (ModData.isUnlockablePurchased(entry.Key, location.NameOrUniqueName)) {
                        ModEntry._Helper.Multiplayer.SendMessage(entry.Value, "ApplyUnlockable", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID });
                        UpdateHandler.applyUnlockable(new Unlockable(entry.Value));
                    } else
                        placeShop(new Unlockable(entry.Value));

                }
            }

            ModEntry._Helper.Multiplayer.SendMessage(true, "UnlockablesReady", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID });
            ModEntry._API.raiseIsReady(new API.IsReadyEventArgs(Game1.player));
        }

        private static void validateUnlockable(UnlockableModel u)
        {
            if (!new Regex("\\s*[0-9]+\\s*,\\s*[0-9]+\\s*").IsMatch(u.ShopPosition))
                Monitor.LogOnce($"Invalid ShopPosition format for '{u.ID}'. Expected: \"x, y\"", LogLevel.Error);

            try {
                Helper.GameContent.Load<Texture2D>(u.ShopTexture);
            } catch (Exception e) { Monitor.LogOnce($"Invalid ShopTexture for '{u.ID}':\n {e.Message}", LogLevel.Error); }

            if (u.ShopAnimation != "" && !new Regex("\\s*[0-9]+\\s*@\\s*[0-9]+\\s*").IsMatch(u.ShopAnimation))
                Monitor.LogOnce($"Invalid ShopAnimation format for '{u.ID}'. Expected: \"Frames@Milliseconds\". eg. \"5@100\"", LogLevel.Error);

            foreach (var el in u.Price) {
                if (el.Key.ToLower() == "money")
                    continue;

                foreach (var id in el.Key.Split(",")) {
                    if (int.TryParse(id, out int result)) {
                        if (new StardewValley.Object(result, el.Value).Name == "Error Item")
                            Monitor.LogOnce($"Invalid price item ID for '{u.ID}': '{id}'", LogLevel.Error);
                    } else
                        Monitor.LogOnce($"Price item ID not numeric for '{u.ID}': '{id}'", LogLevel.Error);
                }

            }
            if (u.UpdateMap.ToLower() != "none")
                try {
                    Helper.GameContent.Load<xTile.Map>(u.UpdateMap);
                } catch (Exception e) { Monitor.LogOnce($"Invalid UpdateMap for '{u.ID}':\n {e.Message}", LogLevel.Error); };

            if (u.UpdateType.ToLower() != "overlay" && u.UpdateType.ToLower() != "replace")
                Monitor.LogOnce($"UpdateType for '{u.ID}' is neither 'Overlay', nor 'Replace'. Will default to 'Overlay' ", LogLevel.Warn);

            if (u.UpdatePosition != "" && !new Regex("\\s*[0-9]+\\s*,\\s*[0-9]+\\s*").IsMatch(u.UpdatePosition))
                Monitor.LogOnce($"Invalid UpdatePosition format for '{u.ID}'. Expected: \"x, y\"", LogLevel.Error);
        }

        public static void placeShop(Unlockable unlockable)
        {
            var location = unlockable.getGameLocation();
            modifiedLocations.Add(location);

            var shopObject = new ShopObject(unlockable.vShopPosition, unlockable);

            if (!location.isTileOccupiedIgnoreFloors(unlockable.vShopPosition))
                location.setObject(unlockable.vShopPosition, shopObject);
            else
                Monitor.Log($"Failed to place Shop Object for '{unlockable.ID}' at '{location.NameOrUniqueName}':'{unlockable.ShopPosition}' as it is occupied", LogLevel.Warn);
        }

        public static bool shopExists(Unlockable unlockable)
        {
            var location = unlockable.getGameLocation();
            var obj = location.getObjectAtTile((int)unlockable.vShopPosition.X, (int)unlockable.vShopPosition.Y);
            return obj != null
                && obj is ShopObject
                && (obj as ShopObject).Unlockable.ID == unlockable.ID;
        }

        public static void removeShop(Unlockable unlockable)
        {
            if (shopExists(unlockable))
                unlockable.getGameLocation().removeObject(unlockable.vShopPosition, false);
        }

        //Returns a list of all GameLocations with the same Name.
        //In the case of buildings they'll share the Name but have a different NameOrUniqueName
        public static List<GameLocation> getLocationsFromName(string name)
        {
            var res = new List<GameLocation>();

            foreach (var loc in Game1.locations) {
                if (loc.Name == name)
                    return new List<GameLocation> { loc };

                if (!(loc is BuildableGameLocation))
                    continue;

                foreach (var building in (loc as BuildableGameLocation).buildings.Where(el => el.indoors?.Value?.Name == name))
                    res.Add(building.indoors.Value);
            }

            return res;
        }
    }
}
