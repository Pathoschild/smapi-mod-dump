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
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;
using StardewModdingAPI.Events;

namespace Unlockable_Areas.Lib
{
    public class ShopPlacement
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            Helper.Events.GameLoop.DayStarted += dayStarted;
        }

        public static void dayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            SaveDataEvents.Data = Helper.Data.ReadSaveData<ModData>("main") ?? new ModData();
            var saveData = SaveDataEvents.Data.UnlockablePurchased;
            var unlockables = Unlockable.convertModelDicToEntity(Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableAreas/Unlockables"));

            foreach (KeyValuePair<string, Unlockable> entry in unlockables) {
                if (!saveData.ContainsKey(entry.Key))
                    saveData[entry.Key] = false;

                validateUnlockable(entry.Value);

                if (saveData[entry.Key]) {
                    ModEntry._Helper.Multiplayer.SendMessage((UnlockableModel)entry.Value, "ApplyUnlockable", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID });
                    UpdateHandler.applyUnlockable(entry.Value);

                } else
                    placeShop(entry.Value);

            }
        }

        private static void validateUnlockable(Unlockable u)
        {
            if (Game1.getLocationFromName(u.Location) == null)
                Monitor.LogOnce($"Invalid Location name '{u.Location}' for '{u.ID}'\nIf this is a custom map, don't forget that you need to add both the map and location", LogLevel.Error);

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

                foreach(var id in el.Key.Split(",")) {
                    if (int.TryParse(id, out int result)) {
                        if (new StardewValley.Object(result, el.Value).Name == "Error Item")
                            Monitor.LogOnce($"Invalid price item ID for '{u.ID}': '{id}'", LogLevel.Error);
                    } else
                        Monitor.LogOnce($"Price item ID not numeric for '{u.ID}': '{id}'", LogLevel.Error);
                }

            }

            try {
                Helper.GameContent.Load<xTile.Map>(u.UpdateMap);
            } catch (Exception e) { Monitor.LogOnce($"Invalid UpdateMap for '{u.ID}':\n {e.Message}", LogLevel.Error); };

            if (u.UpdateType.ToLower() != "overlay" && u.UpdateType.ToLower() != "replace")
                Monitor.LogOnce($"UpdateType for '{u.ID}' is neither 'Overlay', nor 'Replace'. Will default to 'Overlay' ", LogLevel.Warn);

            if (!new Regex("\\s*[0-9]+\\s*,\\s*[0-9]+\\s*").IsMatch(u.UpdatePosition))
                Monitor.LogOnce($"Invalid UpdatePosition format for '{u.ID}'. Expected: \"x, y\"", LogLevel.Error);
        }

        public static void placeShop(Unlockable unlockable)
        {
            var location = Game1.getLocationFromName(unlockable.Location);

            var shopObject = new ShopObject(unlockable.vShopPosition, unlockable);
            location.setObject(unlockable.vShopPosition, shopObject);
        }

        public static bool shopExists(Unlockable unlockable)
        {
            var location = Game1.getLocationFromName(unlockable.Location);
            var obj = location.getObjectAtTile((int)unlockable.vShopPosition.X, (int)unlockable.vShopPosition.Y);
            return obj != null
                && obj.GetType() == typeof(ShopObject)
                && (obj as ShopObject).Unlockable.ID == unlockable.ID;
        }

        public static void removeShop(Unlockable unlockable)
        {
            var location = Game1.getLocationFromName(unlockable.Location);
            if (shopExists(unlockable))
                location.removeObject(unlockable.vShopPosition, false);
        }
    }
}
