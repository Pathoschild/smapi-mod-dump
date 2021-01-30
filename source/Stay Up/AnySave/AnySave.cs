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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Su226.AnySave {
  class M {
    public static IModHelper Helper;
    public static IMonitor Monitor;
  }
  class AnySave : Mod {
    public Config config;

    private bool isModdedSave;
    private SaveData saveData;

    public override void Entry(IModHelper helper) {
      this.config = helper.ReadConfig<Config>();
      M.Helper = Helper;
      M.Monitor = Monitor;

      helper.Events.Input.ButtonPressed += this.OnButtonPressed;
      helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
      helper.Events.GameLoop.Saving += this.OnSaving;
      helper.Events.GameLoop.Saved += this.OnSaved;
      helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
    }

    private void OnButtonPressed(object o, ButtonPressedEventArgs e) {
      if (e.Button != this.config.saveButton) {
        return;
      }
      if (!Game1.player.CanMove) {
        Monitor.Log("Can't save: Can't move");
        return;
      }
      if (Game1.eventUp) {
        Monitor.Log("Can't save: Event up");
        return;
      }
      if (Game1.activeClickableMenu != null) {
        Monitor.Log("Can't save: Menu open");
        return;
      }
      if (Game1.currentMinigame != null) {
        Monitor.Log("Can't save: Minigame open");
        return;
      }
      this.isModdedSave = true;
      if (Game1.IsMultiplayer) {
        Game1.activeClickableMenu = new ReadyCheckDialog("anysave", true, delegate {
          this.isModdedSave = true;
          Game1.newDaySync = new NewDaySynchronizer();
          Game1.activeClickableMenu = new SaveGameMenu();
          Game1.player.team.SetLocalReady("anysave", false);
        });
      } else {
        this.isModdedSave = true;
        Game1.newDaySync = new NewDaySynchronizer();
        Game1.activeClickableMenu = new SaveGameMenu();
      }
    }

    private void OnModMessageReceived(object o, ModMessageReceivedEventArgs e) {
      switch (e.Type) {
      case "Su226.AnySave.GetFarmerData":
        long id = e.ReadAs<long>();
        if (this.saveData != null && this.saveData.farmer.ContainsKey(id)) {
          Monitor.Log(string.Format("Send player {0} to {1}.", id, e.FromPlayerID));
          Helper.Multiplayer.SendMessage(
            this.saveData.farmer[id],
            "Su226.AnySave.ReceiveFarmerData",
            new string[] { e.FromModID },
            new long[] { e.FromPlayerID }
          );
        } else {
          Monitor.Log(string.Format("Can't find player {0}.", id));
        }
        break;
      case "Su226.AnySave.ReceiveFarmerData":
        Monitor.Log(string.Format("Received player from host."));
        this.RestotePlayer(e.ReadAs<FarmerData>());
        break;
      }
    }

    private void OnSaving(object o, SavingEventArgs e) {
      if (!Game1.IsMasterGame) {
        return;
      }
      // Erase data if saved without mod.
      if (!this.isModdedSave) {
        Monitor.Log("Erasing.");
        Helper.Data.WriteSaveData<SaveData>("AnySave", null);
      }
      // Save all NPCs.
      Dictionary<string, CharacterData> characters = new Dictionary<string, CharacterData>();
      foreach (GameLocation l in Game1.locations) {
        foreach (NPC c in l.characters) {
          string key = c.DefaultMap + c.Name;
          Monitor.Log(string.Format("Save NPC {0}", key));
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
          characters[key] = new CharacterData {
            map = c.currentLocation.NameOrUniqueName,
            x = c.position.X,
            y = c.position.Y,
            facing = c.facingDirection,
            target = target,
            queued = c.queuedSchedulePaths.ConvertAll(i => i.Key).ToArray()
          };
        }
      }
      // Save all players.
      Dictionary<long, FarmerData> farmers = new Dictionary<long, FarmerData>();
      foreach (Farmer f in Game1.getAllFarmers()) {
        if (f.mount != null) {
          Monitor.Log(string.Format("Save mounted horse {0}", f.mount.Name));
          characters[f.mount.Name] = new CharacterData {
            map = f.currentLocation.NameOrUniqueName,
            x = f.position.X,
            y = f.position.Y,
            facing = f.facingDirection,
            queued = new int[0]
          };
        }
        Monitor.Log(string.Format("Save player {0}", f.uniqueMultiplayerID));
        farmers[f.uniqueMultiplayerID] = new FarmerData {
          map = f.currentLocation.NameOrUniqueName,
          x = f.position.X,
          y = f.position.Y,
          facing = f.facingDirection,
          swimSuit = f.bathingClothes,
          swimming = f.swimming,
          horse = f.mount?.Name
        };
      }
      // Save shipping bin
      Dictionary<long, string[]> shippingBins = new Dictionary<long, string[]>();
      if (Game1.player.team.useSeparateWallets) {
        foreach (Farmer f in Game1.getAllFarmers()) {
          shippingBins[f.UniqueMultiplayerID] = this.SaveShippingBin(f.personalShippingBin);
        }
      } else {
        shippingBins[-1] = this.SaveShippingBin(Game1.getFarm().getShippingBin(Game1.player));
      }
      // Write data.
      Helper.Data.WriteSaveData("AnySave", new SaveData {
        character = characters,
        farmer = farmers,
        ship = shippingBins,
        time = Game1.timeOfDay
      });
      this.isModdedSave = false;
    }

    private void OnSaved(object o, SavedEventArgs e) {
      // Reset ready or it will stuck
      Game1.player.team.SetLocalReady("ready_for_save", false);
      Game1.player.team.SetLocalReady("wakeup", false);
    }

    private void OnSaveLoaded(object o, SaveLoadedEventArgs e) {
      if (!Game1.IsMasterGame) {
        // Restore other player
        Monitor.Log("Getting player data from host.");
        Helper.Multiplayer.SendMessage(
          Game1.player.UniqueMultiplayerID,
          "Su226.AnySave.GetFarmerData",
          new string[] { "Su226.AnySave" },
          new long[] { Game1.MasterPlayer.UniqueMultiplayerID }
        );
        return;
      }
      // Read data from save
      this.saveData = Helper.Data.ReadSaveData<SaveData>("AnySave");
      if (this.saveData == null) {
        return;
      }
      // Restore time
      Game1.timeOfDay = this.saveData.time;
      // Restore master player
      if (this.saveData.farmer.ContainsKey(Game1.player.uniqueMultiplayerID)) {
        this.RestotePlayer(this.saveData.farmer[Game1.player.uniqueMultiplayerID]);
      } else {
        Monitor.Log(string.Format("Can't find player {1}", Game1.player.uniqueMultiplayerID));
      }
      // Restore shipping bin
      if (Game1.player.team.useSeparateWallets) {
        foreach (Farmer i in Game1.getAllFarmers()) {
          Monitor.Log(string.Format("Restore {0}'s shipping bin", i.uniqueMultiplayerID));
          this.RestoreShippingBin(i.personalShippingBin, this.saveData.ship[i.UniqueMultiplayerID]);
        }
      } else {
        Monitor.Log("Restore shared shipping bin");
        this.RestoreShippingBin(Game1.getFarm().getShippingBin(Game1.player), this.saveData.ship[-1]);
      }
      // Restore all NPCs
      List<NPC> npcs = new List<NPC>();
      foreach (GameLocation l in Game1.locations) {
        npcs.AddRange(l.characters);
      }
      foreach (NPC c in npcs) {
        string key = c.DefaultMap + c.Name;
        if (this.saveData.character.ContainsKey(key)) {
          Monitor.Log(string.Format("Restore NPC {0}", key));
          CharacterData data2 = this.saveData.character[key];
          Game1.warpCharacter(c, data2.map, new Vector2(data2.x / 64, data2.y / 64));
          c.faceDirection(data2.facing);
          if (data2.target != null) {
            Monitor.Log("Restore pathfind data.");
            c.controller = new PathFindController(
              c,
              Game1.getLocationFromName(data2.target.map),
              new Point(data2.target.x, data2.target.y),
              data2.target.facing
            );
          }
          Monitor.Log("Restore queue data.");
          c.queuedSchedulePaths.Clear();
          foreach (int i in data2.queued) {
            c.queuedSchedulePaths.Add(new KeyValuePair<int, SchedulePathDescription>(i, c.Schedule[i]));
          }
        } else {
          Monitor.Log(string.Format("Can't find NPC {0}.", key));
        }
      }
    }

    private string[] SaveShippingBin(ICollection<Item> shippingBin) {
      StringWriter sw = new StringWriter();
      XmlSerializer xs = new XmlSerializer(typeof(Item));
      string[] result = new string[shippingBin.Count];
      int pos = 0;
      foreach (Item i in shippingBin) {
        xs.Serialize(sw, i);
        result[pos++] = sw.ToString();
        sw.GetStringBuilder().Clear();
      }
      return result;
    }

    private void RestoreShippingBin(ICollection<Item> shippingBin, string[] data) {
      XmlSerializer xs = new XmlSerializer(typeof(Item));
      foreach (string i in data) {
        shippingBin.Add((Item)xs.Deserialize(new StringReader(i)));
      }
    }

    private void RestotePlayer(FarmerData data) {
      Monitor.Log(string.Format("Restore player {0}", Game1.player.uniqueMultiplayerID));
      // Restore actual position
      LocationRequest request = Game1.getLocationRequest(data.map);
      request.OnWarp += delegate {
        Game1.player.Position = new Vector2(data.x, data.y);
      };
      Game1.warpFarmer(request, 0, 0, data.facing);
      Game1.fadeToBlackAlpha = 1.2f;
      // Restore swimming
      if (data.swimSuit) {
        Game1.player.changeIntoSwimsuit();
      }
      if (data.swimming) {
        Game1.player.swimming.Value = true;
      }
    }
  }
}