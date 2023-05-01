/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace BNWCore
{
    public class Magic_Net_Events
    {
        public void onDayEnding(DayEndingEventArgs e)
        {
            if (!Context.IsMainPlayer) return;
            foreach (var l in Game1.locations)
            {
                if (l.Objects is null || l.Objects.Count() <= 0)
                {
                    if (l.modData.ContainsKey(ModEntry.ModDataKey))
                        l.modData.Remove(ModEntry.ModDataKey);
                    continue;
                }
                var magicNets = l.Objects.Values.Where(x => x is BNWCoreMagicNet);
                var serializable = new List<BNWCoreMagicNet.BNWCoreMagicNetSerializable>();
                foreach (var f in magicNets)
                {
                    f.DayUpdate(l);
                    serializable.Add(new((BNWCoreMagicNet)f));
                }
                if (serializable is not null && serializable.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(serializable);
                    l.modData[ModEntry.ModDataKey] = json;
                }
                else l.modData.Remove(ModEntry.ModDataKey);

                foreach (var f in magicNets)
                    l.Objects.Remove(f.TileLocation);
            }
        }
        public void onDayStarted(DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer) return;
            foreach (var l in Game1.locations)
            {
                if (!l.modData.ContainsKey(ModEntry.ModDataKey)) continue;
                string json = l.modData[ModEntry.ModDataKey];
                var deserialized = JsonConvert.DeserializeObject<List<BNWCoreMagicNet.BNWCoreMagicNetSerializable>>(json);
                foreach (var f in deserialized)
                {
                    var magicNet = new BNWCoreMagicNet(f.Tile);
                    magicNet.owner.Value = f.Owner;
                    if (f.Bait >= 0)
                        magicNet.bait.Value = new Object(f.Bait, 1);
                    if (f.ObjectId >= 0)
                        magicNet.heldObject.Value = new Object(f.ObjectId, 1);
                    if (!l.Objects.ContainsKey(f.Tile))
                        l.Objects.Add(f.Tile, magicNet);
                    magicNet.DayUpdate(l);
                }
            }
        }
    }
}
