/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using StardewValley;
using StardewValley.Mods;
using System.Collections.Generic;

namespace Trading.Utilities
{
    internal class NetworkPlayer
    {
        public long SenderId { get; set; }

        public NetworkPlayer(long id) => SenderId = id;

        public static implicit operator Farmer(NetworkPlayer player) => Game1.getFarmer(player.SenderId);

        public static implicit operator NetworkPlayer(Farmer farmer) => new(farmer.UniqueMultiplayerID);
    }

    internal class NetworkObject
    {
        public string QualifiedId { get; set; }

        public int Quality { get; set; }

        public int Stack { get; set; }

        [JsonIgnore]
        public ModDataDictionary ModData 
        {
            get
            {
                ModDataDictionary dict = new();
                if (NetModData is not null)
                    foreach (var key in NetModData.Keys)
                        dict[key] = NetModData[key];
                return dict;
            }
        }

        public Dictionary<string, string> NetModData { get; set; }

        public NetworkObject(string id, int quality, int stack, ModDataDictionary? modData = null)
        {
            QualifiedId = id;
            Quality = quality;
            Stack = stack;
            NetModData = new Dictionary<string, string>();
            if (modData is not null)
                foreach (var key in modData.Keys)
                    NetModData[key] = modData[key];
        }

        public static explicit operator NetworkObject(Item obj) => new(obj.QualifiedItemId, obj.Quality, obj.Stack, obj.modData);

        public static explicit operator Item(NetworkObject obj)
        {
            Item i = ItemRegistry.Create(obj.QualifiedId, obj.Stack, obj.Quality);
            foreach (var item in obj.NetModData)
                i.modData[item.Key] = item.Value;
            return i;
        }
    }

    /// <summary>
    /// To be replaced with NetworkObject after the new 1.6 update
    /// </summary>
    /*[Obsolete("Replaced with NetworkObject")]
    internal class TemporaryNetworkObject
    {
        public string ItemName { get; set; }

        public int Quality { get; set; }

        public int Stack { get; set; }

        public int ItemType { get; set; }

        public Color? Color { get; set; }

        public int UpgradeLevel { get; set; }

        [JsonIgnore]
        public ModDataDictionary? ModData 
        {
            get
            {
                ModDataDictionary dict = new();
                if (NetModData is not null)
                    foreach (var key in NetModData.Keys)
                        dict[key] = NetModData[key];
                return dict;
            }
        }

        public Dictionary<string, string> NetModData { get; set; }

        public TemporaryNetworkObject(string name, int type, int quality = 0, int stack = 1, Color? color = null, int upgradeLevel = 0, ModDataDictionary? modData = null)
        {
            ItemName = name;
            ItemType = type;
            Quality = quality;
            Stack = stack;
            Color = color;
            UpgradeLevel = upgradeLevel;
            NetModData = new Dictionary<string, string>();
            if (modData is not null)
                foreach (var key in modData.Keys)
                    NetModData[key] = modData[key];
        }

        public static implicit operator TemporaryNetworkObject(Item obj)
        {
            int itemType = ItemFactory.getItemType(obj);
            string itemName = obj is Tool t ? t.BaseName : ItemFactory.GetItemNameFromIndex(ItemFactory.GetItemIndexInParentSheet(obj, itemType), itemType);
            Color? itemColor = obj is ColoredObject co ? co.color.Value : null;
            int quality = 0; 
            int stack = 1;
            if (obj is SObject o)
            {
                quality = o.Quality;
                stack = o.Stack;
            }
            return new(itemName, itemType, quality, stack, itemColor, obj is Tool tu ? tu.UpgradeLevel : 0, obj.modData);
        }

        public static implicit operator Item?(TemporaryNetworkObject obj) => ItemFactory.getItemFromName(obj.ItemName, obj.ItemType, obj.Stack, obj.Quality, obj.UpgradeLevel, obj.Color);
    }*/

    internal class NetworkInventory
    {
        public long SenderId { get; set; }

        public float Gold { get; set; }

        public List<NetworkObject> Inventory { get; set; }

        public NetworkInventory(long sender, float gold, List<NetworkObject> inventory)
        {
            SenderId = sender;
            Gold = gold;
            Inventory = inventory;
        }
    }
}
