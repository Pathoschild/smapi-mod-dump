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
using Unlockable_Bundles.Lib.Enums;
using Unlockable_Bundles.API;

namespace Unlockable_Bundles.Lib
{
    //Makes sure bundle shops are placed every DayStart and LocationListChanged
    //DayEnding shop removal happens in SaveDataEvents
    public class ShopPlacement
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static List<GameLocation> ModifiedLocations = new List<GameLocation>();

        //This flag is supposed to prevent locationListChanged from possibly running before dayStarted
        public static bool HasDayStarted = false;
        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            Helper.Events.GameLoop.DayStarted += dayStarted;
            Helper.Events.World.LocationListChanged += locationListChanged;

            Helper.Events.GameLoop.DayEnding += delegate { cleanupDay(); };
            Helper.Events.GameLoop.ReturnedToTitle += delegate { cleanupDay(); };
        }

        private static void cleanupDay()
        {
            ContentPatcherHandling.DaysSincePurchaseToken.Ready = false;
            HasDayStarted = false;
            ModData.Instance = null;

            UpdateHandler.clearCache();
            UnlockableBundlesAPI.clearCache();
        }

        private static void locationListChanged(object sender, LocationListChangedEventArgs e)
        {
            //This is relevant for buildings
            if (!Context.IsWorldReady || !HasDayStarted || !Context.IsMainPlayer)
                return;

            var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");

            foreach (var loc in e.Added.Where(e => !ModifiedLocations.Contains(e)))
                foreach (var unlockable in unlockables.Where(el => locationMatchesName(el.Value.Location, loc))) {
                    unlockable.Value.ID = unlockable.Key;
                    unlockable.Value.LocationUnique = loc.NameOrUniqueName;

                    if (ModData.isUnlockablePurchased(unlockable.Key, loc.NameOrUniqueName))
                        continue;

                    unlockable.Value.applyDefaultValues();
                    ModData.applySaveData(unlockable.Value);
                    placeShop(new Unlockable(unlockable.Value), loc);
                }
        }

        //resetDay is also called when clients receive the UnlockablesReady message
        public static void resetDay()
        {
            HasDayStarted = true;
            Unlockable.CachedJsonAssetIDs.Clear();
            Helper.GameContent.InvalidateCache(asset => asset.NameWithoutLocale.IsEquivalentTo("UnlockableBundles/Bundles"));

            UnlockableBundlesAPI.clearCache();
            UpdateHandler.clearCache();

            ModData.Instance = new ModData();
            ContentPatcherHandling.DaysSincePurchaseToken.Ready = false;
        }


        [EventPriority(EventPriority.Low)] //EventPriority needs to be low, or CP will be very inconsistent with token evaluations before UB dayStarted
        public static void dayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            resetDay();
            SaveDataEvents.LoadModData();
            var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");
            ModifiedLocations = new List<GameLocation>();
            List<UnlockableModel> applyList = new List<UnlockableModel>();
            AssetRequested.MailData = new Dictionary<string, string>();

            foreach (KeyValuePair<string, UnlockableModel> entry in unlockables) {
                entry.Value.ID = entry.Key;
                entry.Value.applyDefaultValues();
                validateUnlockable(entry.Value);

                var locations = getLocationsFromName(entry.Value.Location);
                foreach (var location in locations) {
                    entry.Value.LocationUnique = location.NameOrUniqueName;
                    ModData.applySaveData(entry.Value);
                    var unlockable = new Unlockable(entry.Value);
                    var loc = unlockable.getGameLocation();

                    if(loc is null) {
                        Monitor.Log($"skipped applying bundle logic to location {location.NameOrUniqueName} as it does not exist");
                        continue;
                    }

                    if (ModData.isUnlockablePurchased(entry.Key, location.NameOrUniqueName)) {
                        applyList.Add((UnlockableModel)unlockable);
                        UpdateHandler.applyUnlockable(unlockable);
                    } else
                        placeShop(unlockable, loc);

                    if (entry.Value.BundleCompletedMail != "")
                        AssetRequested.MailData.Add(Unlockable.getMailKey(entry.Key), entry.Value.BundleCompletedMail);

                }
            }

            ModEntry._Helper.Multiplayer.SendMessage(new KeyValuePair<List<UnlockableModel>, ModData>(applyList, ModData.Instance), "UnlockablesReady", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID });
            ModEntry._Helper.Multiplayer.SendMessage(AssetRequested.MailData, "UpdateMailData", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID });
            Helper.GameContent.InvalidateCache("Data/Mail"); //I kind of wish I could just append my mail data instead of reloading the entire asset
            ModEntry._API.raiseIsReady(new IsReadyEventArgs(Game1.player));
        }

        private static void validateUnlockable(UnlockableModel u)
        {
            if (u.ShopTexture.ToLower() != "none")
                try {
                    Helper.GameContent.Load<Texture2D>(u.ShopTexture);
                } catch (Exception e) { Monitor.LogOnce($"Invalid ShopTexture for '{u.ID}':\n {e.Message}", LogLevel.Error); }

            if (u.ShopAnimation != "" && !new Regex("^\\s*(?:(?:\\d+|\\d+\\s*-\\s*\\d+)(?:\\s*@\\s*\\d+){0,1})\\s*(?:,\\s*(?:(?:\\d+|\\d+\\s*-\\s*\\d+)(?:\\s*@\\s*\\d+){0,1})\\s*)*$").IsMatch(u.ShopAnimation))
                Monitor.LogOnce($"Invalid ShopAnimation format for '{u.ID}'. Expected comma seperated \"<From>-<To>@<Interval>\". eg. \"0-2@300, 3, 5-4@600\"", LogLevel.Error);

            validatePrice(u, u.Price);
            validatePrice(u, u.BundleReward);

            if (u.EditMap.ToLower() != "none")
                try {
                    Helper.GameContent.Load<xTile.Map>(u.EditMap);
                } catch (Exception e) { Monitor.LogOnce($"Invalid EditMap for '{u.ID}':\n {e.Message}", LogLevel.Error); };
        }

        private static void validatePrice(UnlockableModel u, Dictionary<string, int> price)
        {
            foreach (var el in price) {
                if (el.Key.ToLower().Trim() == "money")
                    continue;

                foreach (var entry in el.Key.Split(",")) {
                    var id = Unlockable.getIDFromReqSplit(entry);
                    var quality = Unlockable.getQualityFromReqSplit(entry);

                    if (quality == -1)
                        Monitor.LogOnce($"Invalid quality '{entry.Split(":").Last()}' for item '{u.ID}': '{id}'", LogLevel.Error);

                    if (Unlockable.parseItem(id, el.Value).Name == "Error Item")
                        Monitor.LogOnce($"Invalid item ID for '{u.ID}': '{id}'", LogLevel.Error);
                }

            }
        }

        public static void placeShop(Unlockable unlockable, GameLocation location)
        {
            ModifiedLocations.Add(location);

            var shopObject = new ShopObject(unlockable.ShopPosition, unlockable);

            if (!location.IsTileOccupiedBy(unlockable.ShopPosition, collisionMask: CollisionMask.Objects)) {
                location.setObject(unlockable.ShopPosition, shopObject);
                Monitor.Log($"Placed Shop Object for '{unlockable.ID}' at '{location.NameOrUniqueName}':'{unlockable.ShopPosition}'");
            } else
                Monitor.Log($"Failed to place Shop Object for '{unlockable.ID}' at '{location.NameOrUniqueName}':'{unlockable.ShopPosition}' as it is occupied", LogLevel.Warn);
        }

        public static bool shopExists(Unlockable unlockable)
        {
            var location = unlockable.getGameLocation();
            var obj = location.getObjectAtTile((int)unlockable.ShopPosition.X, (int)unlockable.ShopPosition.Y);
            return obj != null
                && obj is ShopObject
                && (obj as ShopObject).Unlockable.ID == unlockable.ID;
        }

        public static void removeShop(Unlockable unlockable)
        {
            if (shopExists(unlockable)) {
                var location = unlockable.getGameLocation();
                location.removeObject(unlockable.ShopPosition, false);
                Monitor.Log($"Removed Shop Object for '{unlockable.ID}' at '{location.NameOrUniqueName}':'{unlockable.ShopPosition}'");
            }

        }

        //Returns a list of all GameLocations with the same Name.
        //In the case of buildings they'll share the Name but have a different NameOrUniqueName
        public static List<GameLocation> getLocationsFromName(string name)
        {
            var res = new List<GameLocation>();

            foreach (var loc in Game1.locations) {
                if (locationMatchesName(name, loc)) {
                    res.Add(loc);
                    continue;
                }

                foreach (var building in loc.buildings.Where(el => el.indoors?.Value?.Name == name))
                    res.Add(building.indoors.Value);
            }

            return res;
        }

        public static bool locationMatchesName(string name, GameLocation loc)
        {
            return (loc.Name == name)
                || (loc is Cellar && name == "Cellar") //Cellar location names are enumerated
                || (loc is FarmHouse && name == "FarmHouse"); //Farmhand FarmHouses are called "Cabin"
        }
    }
}
