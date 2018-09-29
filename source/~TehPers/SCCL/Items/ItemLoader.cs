using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using TehPers.Stardew.Framework;
using SObject = StardewValley.Object;

namespace TehPers.Stardew.SCCL.Items {
    internal class ItemLoader {

        public ItemLoader() {
            SaveEvents.BeforeSave += (sender, e) => Save();
            SaveEvents.AfterLoad += (sender, e) => Load();
            SaveEvents.AfterSave += (sender, e) => Load();
        }

        public void Save() {
            //string path = $"{Constants.SaveFolderName}/modItems.json";
            ModEntry.INSTANCE.Monitor.Log("Saving mod items...");
            string path = "modItems.json";
            HashSet<ItemData> data = new HashSet<ItemData>();

            for (int slot = 0; slot < Game1.player.items.Count; slot++) {
                Item item = Game1.player.items[slot];
                if (item is ModObject modObject) {
                    data.Add(new ItemData(modObject.Template.ID, slot, modObject.Data));
                }
            }

            HashSet<GameLocation> checkLocs = Game1.locations.Concat(
                from loc in Game1.locations
                where loc is BuildableGameLocation

                from building in (loc as BuildableGameLocation).buildings
                where building.indoors != null
                select building.indoors
                ).ToHashSet();

            if (Game1.getLocationFromName("FarmHouse") != null)
                checkLocs.Add(Game1.getLocationFromName("FarmHouse"));

            foreach (GameLocation location in checkLocs) {
                if (location.objects == null) continue;
                foreach (Vector2 position in location.objects.Keys) {
                    if (location.objects[position] is Chest chest) {
                        for (int slot = 0; slot < chest.items.Count; slot++) {
                            if (chest.items[slot] is ModObject item) {
                                data.Add(new ItemData(item.Template.ID, slot, item.Data, location.name, position));
                            }
                        }
                    }
                }
            }

            ModEntry.INSTANCE.Helper.WriteJsonFile(path, data);
        }

        public void Load() {
            ModEntry.INSTANCE.Monitor.Log("Loading mod items...");
            //string path = $"{Constants.SaveFolderName}/modItems.json";
            string path = "modItems.json";
            HashSet<ItemData> data = ModEntry.INSTANCE.Helper.ReadJsonFile<HashSet<ItemData>>(path);

            foreach (ItemData item in data) {
                if (ItemTemplate.Templates.TryGetValue(item.ItemID, out ItemTemplate template)) {
                    if (item.Location == null) {
                        if (item.Slot >= Game1.player.items.Count) {
                            ModEntry.INSTANCE.Monitor.Log($"Skipped item in player slot {item.Slot}.", LogLevel.Warn);
                        } else {
                            Game1.player.items[item.Slot] = Game1.player.items[item.Slot] != null ? new ModObject(template, item.Data, Game1.player.items[item.Slot].Stack) : new ModObject(template, item.Data);
                        }
                    } else {
                        GameLocation location = Game1.getLocationFromName(item.Location);
                        if (location != null) {
                            if (location.objects.TryGetValue(item.Position, out SObject obj)) {
                                if (obj is Chest chest) {
                                    ModObject modObject = new ModObject(template, item.Data);
                                    if (chest.items.Count >= item.Slot) {
                                        chest.items.Add(modObject);
                                    } else {
                                        modObject.stack = chest.items[item.Slot].Stack;
                                        chest.items[item.Slot] = modObject;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
