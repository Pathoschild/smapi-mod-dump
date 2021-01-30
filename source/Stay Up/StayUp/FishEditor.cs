/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace Su226.StayUp {
  class FishEditor : IAssetEditor {
    public bool CanEdit<T>(IAssetInfo asset) {
      return asset.AssetNameEquals("Data/Fish");
    }

    public void Edit<T>(IAssetData asset) {
      IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
      List<int> keys = new List<int>(data.Keys);
      foreach (int i in keys) {
        string[] values = data[i].Split('/');
        if (values[1] == "trap") {
          M.Monitor.Log(string.Format("Ignore crab pot fish {0}", values[0]));
          continue;
        }
        string[] times = values[5].Split(' ');
        bool canCatch = false;
        for (int j = 0; j < times.Length; j += 2) {
          if (times[j | 1] == "2600") {
            canCatch = true;
            break;
          }
        }
        if (!canCatch) {
          M.Monitor.Log(string.Format("{0} can't be caught: {1}", values[0], values[5]));
          continue;
        }
        bool edited = false;
        for (int j = 0; j < times.Length; j += 2) {
          if (times[j] == "600") {
            times[j] = "150";
            edited = true;
            break;
          }
        }
        string original = values[5];
        if (!edited) {
          values[5] += " 150 600";
          M.Monitor.Log(string.Format("Add time to {0}: {1} -> {2}", values[0], original, values[5]));
        } else {
          values[5] = string.Join(" ", times);
          M.Monitor.Log(string.Format("Modify time of {0}: {1} -> {2}", values[0], original, values[5]));
        }
        data[i] = string.Join("/", values);
      }
    }
  }
}