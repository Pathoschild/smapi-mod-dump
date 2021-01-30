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

namespace Su226.AnySave {
  class PathData {
    public string map;
    public int x;
    public int y;
    public int facing;
  }
  class CharacterData {
    public string map;
    public float x;
    public float y;
    public int facing;
    public PathData target;
    public int[] queued;
  }
  class FarmerData {
    public string map;
    public float x;
    public float y;
    public int facing;
    public bool swimming;
    public bool swimSuit;
    public string horse;
  }
  class SaveData {
    public IDictionary<string, CharacterData> character;
    public IDictionary<long, FarmerData> farmer;
    public IDictionary<long, string[]> ship;
    public int time;
  }
}