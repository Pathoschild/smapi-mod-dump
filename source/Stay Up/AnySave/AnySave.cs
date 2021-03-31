/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Su226.AnySave {
  class M {
    public static IModHelper Helper;
    public static IMonitor Monitor;
    public static Config Config;
    public static AnySaveApi Api;
  }
  class AnySave : Mod {
    public override void Entry(IModHelper helper) {
      M.Helper = Helper;
      M.Monitor = Monitor;
      M.Config = helper.ReadConfig<Config>();
      M.Api = new AnySaveApi();

      helper.Events.Input.ButtonPressed += this.OnButtonPressed;
      helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
      helper.Events.GameLoop.Saved += this.OnSaved;
      helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
      helper.Events.GameLoop.DayEnding += this.OnDayEnding;
    }

    public override object GetApi() {
      return M.Api;
    }

    private void OnButtonPressed(object o, ButtonPressedEventArgs e) {
      if (e.Button != M.Config.saveButton) {
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
      if (Game1.IsMultiplayer) {
        Game1.activeClickableMenu = new ReadyCheckDialog("anysave", true, delegate {
          if (Game1.IsMasterGame) {
            this.Save();
          }
          Game1.newDaySync = new NewDaySynchronizer();
          Game1.activeClickableMenu = new SaveGameMenu();
          Game1.player.team.SetLocalReady("anysave", false);
        });
      } else {
        this.Save();
        Game1.newDaySync = new NewDaySynchronizer();
        Game1.activeClickableMenu = new SaveGameMenu();
      }
    }

    private void Save() {
      M.Api.SaveTime();
      foreach (GameLocation l in Game1.locations) {
        foreach (NPC c in l.characters) {
          M.Api.SaveNpc(c);
        }
      }
      Farm farm = Game1.getFarm();
      foreach (Farmer f in Game1.getAllFarmers()) {
        if (f.mount != null) {
          M.Api.SaveNpc(f.mount);
        }
        if (f.team.useSeparateWallets) {
          M.Api.SaveShippingBin(f);
        }
        if (f.isActive()) {
          M.Api.SavePlayer(f);
        } else {
          M.Api.GetAndClearPlayerData(f.UniqueMultiplayerID);
        }
      }
      if (!Game1.player.team.useSeparateWallets) {
        M.Api.SaveShippingBin(Game1.player);
      }
    }

    private void OnModMessageReceived(object o, ModMessageReceivedEventArgs e) {
      switch (e.Type) {
      case "Su226.AnySave.RequestRestoreFarmer":
        FarmerData data = M.Api.GetAndClearPlayerData(e.FromPlayerID);
        if (data != null) {
          Monitor.Log(string.Format("Send player data {0}", e.FromPlayerID));
          Helper.Multiplayer.SendMessage(
            data,
            "Su226.AnySave.RestoreFarmer",
            new string[] { e.FromModID },
            new long[] { e.FromPlayerID }
          );
        } else {
          Monitor.Log(string.Format("Can't find player {0}", e.FromPlayerID));
        }
        break;
      case "Su226.AnySave.RestoreFarmer":
        Monitor.Log(string.Format("Received player data from host."));
        M.Api.RestoreCurrentPlayer(e.ReadAs<FarmerData>());
        break;
      }
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
          "Su226.AnySave.RequestRestoreFarmer",
          new string[] { "Su226.AnySave" }
        );
        return;
      }
      M.Api.RestoreTime();
      foreach (GameLocation l in Game1.locations) {
        foreach (NPC c in l.characters) {
          M.Api.RestoreNpc(c);
        }
      }
      FarmerData data = M.Api.GetAndClearPlayerData(Game1.player.UniqueMultiplayerID);
      if (data != null) {
        M.Api.RestoreCurrentPlayer(data);
      }
      if (Game1.player.team.useSeparateWallets) {
        foreach (Farmer f in Game1.getAllFarmers()) {
          M.Api.RestoreShippingBin(f);
        }
      } else {
        M.Api.RestoreShippingBin(Game1.player);
      }
    }

    private void OnDayEnding(object o, DayEndingEventArgs e) {
      // Clear unused player data when sleeping.
      foreach (Farmer f in Game1.getAllFarmers()) {
        M.Api.GetAndClearPlayerData(f.UniqueMultiplayerID);
      }
    }
  }
}