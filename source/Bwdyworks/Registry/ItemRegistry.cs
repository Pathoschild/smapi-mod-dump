using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.Registry
{
    public static class ItemRegistry
    {
        public static Dictionary<int, BasicItemEntry> RegisteredItems { get; set; } = new Dictionary<int, BasicItemEntry>();
        public static Dictionary<string, BasicItemEntry> GlobalRegistry { get; set; } = new Dictionary<string, BasicItemEntry>();
        internal static ItemRegistrySaveDataModel LocalRegistry = new ItemRegistrySaveDataModel();
        internal static bool Loaded = false;
        private static int BaseItemId;

        internal static void RegisterItem(string module, BasicItemEntry itemdata)
        {
            string globalid = module + ":" + itemdata.InternalName;
            GlobalRegistry[globalid] = itemdata;
        }

        internal static void Load()
        {
            Loaded = true;
            BaseItemId = 1600; //reset the base item id
            LocalRegistry = Modworks.Helper.Data.ReadJsonFile<ItemRegistrySaveDataModel>("ModworksRegistry.json");
            if (LocalRegistry == null) {
                LocalRegistry = new ItemRegistrySaveDataModel();
                Modworks.Log.Trace("Item registry created.");
            } else Modworks.Log.Trace("Item registry loaded.");
            foreach (KeyValuePair<string, BasicItemEntry> entry in GlobalRegistry)
            {
                int integerId;
                //does data exist already? use existing integerId to maintain savegame integrity.
                if (LocalRegistry.RegisteredItemIds.ContainsKey(entry.Key)) integerId = LocalRegistry.RegisteredItemIds[entry.Key];
                else integerId = BaseItemId++;
                while (LocalRegistry.RegisteredItemIds.ContainsValue(BaseItemId)) BaseItemId++; //skip existing ids
                entry.Value.IntegerId = integerId;
                entry.Value.GlobalId = entry.Key;
                LocalRegistry.RegisteredItemIds[entry.Key] = integerId;
                RegisteredItems[integerId] = entry.Value;
                Modworks.Log.Trace("Registered item configured: " + entry.Value.GlobalId);
            }
        }

        internal static void Save()
        {
            Modworks.Log.Trace("Item registry saved.");
            Modworks.Helper.Data.WriteJsonFile("bwdyItemIds.json", LocalRegistry);
        }
    }

    public class ItemRegistrySaveDataModel
    {
        public Dictionary<string, int> RegisteredItemIds { get; set; } = new Dictionary<string, int>();
    }
}
