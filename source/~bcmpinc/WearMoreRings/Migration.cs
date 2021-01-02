/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;

namespace StardewHack.WearMoreRings
{
    using OldSaveRingsDict   = Dictionary<long, OldSaveRings>;

    /// <summary>
    /// Old structure for save data.
    /// Used to migrate to new chest based storage.
    /// </summary>
    public class OldSaveRings {
        public int which1;
        public int which2;
        public int which3;
        public int which4;
    }
    
    static class Migration {
        static private Ring MakeRing(int which) {
            if (which < 0) return null;
            try {
                return new Ring(which);
            } catch {
                // Ring no longer exists, so delete it.
                ModEntry.getInstance().Monitor.Log($"Failed to create ring with id {which}.", LogLevel.Warn);
                return null;
            }
        }
        
        static public void Import(IMonitor monitor, IModHelper helper) {
            if (!Game1.IsMasterGame) return;
            
            // Load data from mod's save file, if available.
            var savedata = helper.Data.ReadSaveData<OldSaveRingsDict>("extra-rings");
            if (savedata == null) {
                monitor.Log("Old save data not available.");
                return;
            }
            
            monitor.Log("Migrating old save data");
            // Iterate through each farmer to load the extra equipped rings.
            foreach(Farmer f in Game1.getAllFarmers()) {
                Chest c = ModEntry.GetRingInventory(f);
                if (savedata.ContainsKey(f.UniqueMultiplayerID)) {
                    var data = savedata[f.UniqueMultiplayerID];
                    c.items[0] = MakeRing(data.which1);
                    c.items[1] = MakeRing(data.which2);
                    c.items[2] = MakeRing(data.which3);
                    c.items[3] = MakeRing(data.which4);
                }
            }
            helper.Data.WriteSaveData<OldSaveRingsDict>("extra-rings", null);
        }
    }
}
