/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Su226.AnySave {
  public class AnySaveApi {
    public delegate bool LocationHandler(string map, float x, float y, int facing);
    private Regex EscapingRegex = new Regex(@"[^A-Za-z0-9]");
    private IList<LocationHandler> LocationHandlers = new List<LocationHandler>();

    ////////////////////////////////
    // Player
    ////////////////////////////////
    public void SavePlayer(Farmer f) {
      M.Monitor.Log(string.Format("Saving player {0}.", f.Name));
      M.Helper.Data.WriteSaveData(string.Format("player.{0}", f.UniqueMultiplayerID), new FarmerData {
        map = f.currentLocation.NameOrUniqueName,
        x = f.position.X,
        y = f.position.Y,
        facing = f.facingDirection,
        swimSuit = f.bathingClothes,
        swimming = f.swimming,
        horse = f.mount?.Name
      });
    }

    public FarmerData GetAndClearPlayerData(long id) {
      M.Monitor.Log(string.Format("Getting and clearing player data {0}", id));
      string key = string.Format("player.{0}", id);
      FarmerData data = M.Helper.Data.ReadSaveData<FarmerData>(key);
      M.Helper.Data.WriteSaveData<FarmerData>(key, null);
      return data;
    }

    public void RegisterLocationHandler(LocationHandler handler) {
      LocationHandlers.Add(handler);
    }

    public void RestoreCurrentPlayer(FarmerData data) {
      M.Monitor.Log(string.Format("Restoring current player."));
      // Restore actual position
      foreach (LocationHandler handler in LocationHandlers.Reverse()) {
        if (handler(data.map, data.x, data.y, data.facing)) {
          break;
        }
      }
      Game1.fadeToBlackAlpha = 1.2f;
      // Restore swimming
      if (data.swimSuit) {
        Game1.player.changeIntoSwimsuit();
      }
      if (data.swimming) {
        Game1.player.swimming.Value = true;
      }
    }

    ////////////////////////////////
    // NPC
    ////////////////////////////////
    public string Escape(string s) {
      return this.EscapingRegex.Replace(s, match => "-" + ((int)match.Value[0]).ToString() + "-");
    }

    public string GetNpcKey(NPC c) {
      // Mister Qi and your pet's name may have space or non-English char.
      return string.Format("npc.{0}.{1}", c.DefaultMap, this.Escape(c.name));
    }

    public void SaveNpc(NPC c) {
      string key = this.GetNpcKey(c);
      M.Monitor.Log(string.Format("Saving NPC {0}::{1} ({2})", c.DefaultMap, c.Name, key));
      PathData target = null;
      if (c.controller != null) {
        Point[] points = c.controller.pathToEndPoint.ToArray();
        if (points.Length != 0) {
          Point endPoint = points[points.Length - 1];
          target = new PathData {
            map = c.controller.location.NameOrUniqueName,
            x = endPoint.X,
            y = endPoint.Y,
            facing = c.controller.finalFacingDirection
          };
        }
      }
      M.Helper.Data.WriteSaveData(key, new CharacterData {
        map = c.currentLocation.NameOrUniqueName,
        x = c.position.X,
        y = c.position.Y,
        facing = c.facingDirection,
        target = target,
        queued = c.queuedSchedulePaths.ConvertAll(i => i.Key).ToArray()
      });
    }

    public bool RestoreNpc(NPC c) {
      string key = this.GetNpcKey(c);
      M.Monitor.Log(string.Format("Restoring NPC {0}::{1} ({2})", c.DefaultMap, c.Name, key));
      CharacterData data = M.Helper.Data.ReadSaveData<CharacterData>(key);
      if (data == null) {
        M.Monitor.Log("Cannot find data.");
        return false;
      }
      Game1.warpCharacter(c, data.map, new Vector2(data.x / 64, data.y / 64));
      c.faceDirection(data.facing);
      // Restore pathding target
      if (data.target != null) {
        c.controller = new PathFindController(
          c,
          Game1.getLocationFromName(data.target.map),
          new Point(data.target.x, data.target.y),
          data.target.facing
        );
      }
      // Restore schedule data
      c.queuedSchedulePaths.Clear();
      foreach (int i in data.queued) {
        c.queuedSchedulePaths.Add(new KeyValuePair<int, SchedulePathDescription>(i, c.Schedule[i]));
      }
      M.Helper.Data.WriteSaveData<CharacterData>(key, null);
      return true;
    }

    ////////////////////////////////
    // Shipping Bin
    ////////////////////////////////
    public void SaveShippingBin(Farmer f) {
      M.Monitor.Log(string.Format("Saving {0}'s shipping bin.", f.Name));
      ICollection<Item> shippingBin = Game1.getFarm().getShippingBin(f);
      StringWriter sw = new StringWriter();
      XmlSerializer xs = new XmlSerializer(typeof(Item));
      string[] result = new string[shippingBin.Count];
      int pos = 0;
      foreach (Item i in shippingBin) {
        xs.Serialize(sw, i);
        result[pos++] = sw.ToString();
        sw.GetStringBuilder().Clear();
      }
      M.Helper.Data.WriteSaveData(string.Format("shippingBin.{0}", f.UniqueMultiplayerID), result);
    }

    public bool RestoreShippingBin(Farmer f) {
      M.Monitor.Log(string.Format("Restoring {0}'s shipping bin.", f.Name));
      string key = string.Format("shippingBin.{0}", f.UniqueMultiplayerID);
      string[] data = M.Helper.Data.ReadSaveData<string[]>(key);
      if (data == null) {
        M.Monitor.Log("Cannot find data.");
        return false;
      }
      ICollection<Item> shippingBin = Game1.getFarm().getShippingBin(f);
      XmlSerializer xs = new XmlSerializer(typeof(Item));
      foreach (string i in data) {
        shippingBin.Add((Item)xs.Deserialize(new StringReader(i)));
      }
      M.Helper.Data.WriteSaveData<string[]>(key, null);
      return true;
    }

    ////////////////////////////////
    // Time
    ////////////////////////////////
    public void SaveTime() {
      M.Monitor.Log("Saving time.");
      M.Helper.Data.WriteSaveData<TimeData>("time", new TimeData { time = Game1.timeOfDay });
    }

    public bool RestoreTime() {
      M.Monitor.Log("Restoring time.");
      TimeData data = M.Helper.Data.ReadSaveData<TimeData>("time");
      if (data == null) {
        M.Monitor.Log("Cannot find data.");
        return false;
      }
      Game1.timeOfDay = data.time;
      M.Helper.Data.WriteSaveData<TimeData>("time", null);
      return true;
    }
  }
}