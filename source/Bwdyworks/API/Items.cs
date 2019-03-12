using bwdyworks.Registry;
using bwdyworks.Structures;
using System.Collections.Generic;

namespace bwdyworks.API
{
    public class Items
    {
        internal static List<TrashLootEntry> TrashLoot = new List<TrashLootEntry>();

        public void AddItem(string module, BasicItemEntry entry)
        {
            ItemRegistry.RegisterItem(module, entry);
        }

        public int? GetModItemId(string module, string internalName)
        {
            string globalid = module + ":" + internalName;
            if (ItemRegistry.LocalRegistry.RegisteredItemIds.ContainsKey(globalid))
            {
                return ItemRegistry.LocalRegistry.RegisteredItemIds[globalid];
            }
            else
            {
                Modworks.Log.Error("Item id lookup failed for: " + globalid);
                return null;
            }
        }

        public void AddMonsterLoot(string module, MonsterLootEntry loot)
        {
            Modworks.Assets.MonsterLoot.Add(loot);
            Modworks.Log.Trace("Module " + module + " added a loot drop to " + loot.MonsterID + "s.");
        }

        public void AddTrashLoot(string module, TrashLootEntry loot)
        {
            TrashLoot.Add(loot);
        }

        public StardewValley.Object CreateItemstack(int id, int count)
        {
            StardewValley.Object i = (StardewValley.Object)StardewValley.Objects.ObjectFactory.getItemFromDescription(0, id, count);
            i.IsSpawnedObject = true;
            i.ParentSheetIndex = id;
            return i;
        }
    }
}
