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
using Netcode;
using StardewValley.Network;
using Unlockable_Areas.NetLib;

namespace Unlockable_Areas.Lib
{
    public sealed class ModData
    {
        public static ModData Instance = new ModData();

        public Dictionary<string, Dictionary<string, bool>> UnlockablePurchased { get; set; } = new Dictionary<string, Dictionary<string, bool>>();

        public static bool isUnlockablePurchased(string key, string location)
        {
            if (ModData.Instance == null)
                ModData.Instance = new ModData();

            return Instance.UnlockablePurchased.ContainsKey(key)
               && Instance.UnlockablePurchased[key].ContainsKey(location)
               && Instance.UnlockablePurchased[key][location];

        }

        public static void setUnlockablePurchased(string key, string location, bool value = true)
        {
            if (ModData.Instance == null) //Can happen when a player connects during the day
                ModData.Instance = new ModData();

            if (!Instance.UnlockablePurchased.ContainsKey(key))
                Instance.UnlockablePurchased[key] = new Dictionary<string, bool>();

            Instance.UnlockablePurchased[key][location] = value;
            API.UnlockableAreasAPI.clearCache();
        }
    }
}
