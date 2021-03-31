/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace Su226.StayUp {
  class TimePart : IComparable<TimePart> {
    public int begin;
    public int end;

    public TimePart(int begin, int end) {
      this.begin = begin;
      this.end = end;
    }

    public int CompareTo(TimePart other) {
      return this.begin - other.begin;
    }

    public bool Include(int time) {
      return this.begin <= time && time <= this.end;
    }

    public override string ToString() {
      return string.Format("{0} {1}", this.begin, this.end);
    }
  }

  class TimeParts {
    private List<TimePart> parts = new List<TimePart>();

    public void Parse(string raw) {
      string[] values = raw.Split(' ');
      this.parts.Clear();
      for (int i = 0; i < values.Length; i += 2) {
        this.parts.Add(new TimePart(int.Parse(values[i]), int.Parse(values[i + 1])));
      }
    }

    public bool Include(int time) {
      foreach (TimePart i in parts) {
        if (i.Include(time)) {
          return true;
        }
      }
      return false;
    }

    public void Add(int begin, int end) {
      this.parts.Add(new TimePart(begin, end));
    }

    public void Optimize() {
      this.parts.Sort();
      List<TimePart> newParts = new List<TimePart>{ parts[0] };
      int i = 0;
      for (int j = 1; j < parts.Count; j++) {
        if (newParts[i].Include(parts[j].begin)) {
          newParts[i].end = Math.Max(newParts[i].end, parts[j].end);
        } else {
          newParts.Add(parts[j]);
          i++;
        }
      }
      this.parts = newParts;
    }

    public override string ToString() {
      return string.Join(" ", this.parts);
    }
  }

  class FishEditor : IAssetEditor {
    public bool CanEdit<T>(IAssetInfo asset) {
      return asset.AssetNameEquals("Data/Fish");
    }

    public void Edit<T>(IAssetData asset) {
      IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
      TimeParts time = new TimeParts();
      foreach (int i in new List<int>(data.Keys)) {
        string[] values = data[i].Split('/');
        if (values[1] == "trap") {
          continue;
        }
        bool modified = false;
        time.Parse(values[5]);
        if (time.Include(2600)) {
          modified = true;
          time.Add(150, 400);
        }
        if (time.Include(600)) {
          modified = true;
          time.Add(400, 600);
        }
        if (modified) {
          time.Optimize();
          string old = values[5];
          values[5] = time.ToString();
          data[i] = string.Join("/", values);
          M.Monitor.Log(string.Format("{0}: {1} -> {2}", values[0], old, values[5]));
        } else {
          M.Monitor.Log(string.Format("{0}: {1} (Not modified)", values[0], values[5]));
        }
      }
    }
  }
}
