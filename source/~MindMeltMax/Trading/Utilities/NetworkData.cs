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
using StardewValley.Tools;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Trading.Utilities
{
    internal class NetworkPlayer
    {
        public long SenderId { get; set; }

        public NetworkPlayer(long id)
        {
            SenderId = id;
        }

        public static implicit operator Farmer(NetworkPlayer player) => Game1.getFarmer(player.SenderId);

        public static implicit operator NetworkPlayer(Farmer farmer) => new NetworkPlayer(farmer.UniqueMultiplayerID);
    }

    /*internal class NetworkObject
    {
        public string QualifiedId { get; set; }

        public int Quality { get; set; }

        public int Stack { get; set; }

        [JsonIgnore]
        public ModDataDictionary? ModData 
        {
            get
            {
                ModDataDictionary dict = new ModDataDictionary();
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

        public static implicit operator NetworkObject(SObject obj) => new NetworkObject(obj.QualifiedId, obj.Quality, obj.Stack, obj.modData);

        public static implicit operator SObject(NetworkObject obj) => new SObject(obj.QualifiedId, obj.Stack, quality: obj.Quality) { modData = obj.ModData };
    }*/

    /// <summary>
    /// To be replaced with NetworkObject after the new 1.6 update
    /// </summary>
    internal class TemporaryNetworkObject
    {
        public int ParentSheetIndex { get; set; }

        public int Quality { get; set; }

        public int Stack { get; set; }

        [JsonIgnore]
        public ModDataDictionary? ModData 
        {
            get
            {
                ModDataDictionary dict = new ModDataDictionary();
                if (NetModData is not null)
                    foreach (var key in NetModData.Keys)
                        dict[key] = NetModData[key];
                return dict;
            }
        }

        public Dictionary<string, string> NetModData { get; set; }

        public TemporaryNetworkObject(int parentSheetIndex, int quality, int stack, ModDataDictionary? modData = null)
        {
            ParentSheetIndex = parentSheetIndex;
            Quality = quality;
            Stack = stack;
            NetModData = new Dictionary<string, string>();
            if (modData is not null)
                foreach (var key in modData.Keys)
                    NetModData[key] = modData[key];
        }

        public static implicit operator TemporaryNetworkObject(SObject obj) => new TemporaryNetworkObject(obj.ParentSheetIndex, obj.Quality, obj.Stack, obj.modData);

        public static implicit operator SObject(TemporaryNetworkObject obj) => new SObject(obj.ParentSheetIndex, obj.Stack, quality: obj.Quality) { modData = obj.ModData };
    }

    internal class NetworkInventory
    {
        public long SenderId { get; set; }

        public float Gold { get; set; }

        public List<TemporaryNetworkObject> Inventory { get; set; }

        public NetworkInventory(long sender, float gold, List<TemporaryNetworkObject> inventory)
        {
            SenderId = sender;
            Gold = gold;
            Inventory = inventory;
        }
    }
}
