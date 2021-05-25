/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

#region License

// clang-format off
// 
//    ChestEx (StardewValleyMods)
//    Copyright (c) 2021 Berkay Yigit <berkaytgy@gmail.com>
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.
// 
// clang-format on

#endregion

using System;

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ChestEx {
  public class Config {
  #region Private

    private struct ConfigMP {
      public Int32 mRows    { get; set; }
      public Int32 mColumns { get; set; }
    }

    private ConfigMP mpInstance;
    private Int32    rows;
    private Int32    columns;

  #region Statics

    private static Config sInstance;

  #endregion

  #endregion

  #region Public

    public Int32 mRows {
      get => Context.IsMainPlayer ? this.rows : this.mpInstance.mRows;
      set {
        if (Context.IsMainPlayer) this.rows = value;
        this.mpInstance.mRows = value;
      }
    }

    public Int32 mColumns {
      get => Context.IsMainPlayer ? this.columns : this.mpInstance.mColumns;
      set {
        if (Context.IsMainPlayer) this.columns = value;
        this.mpInstance.mColumns = value;
      }
    }

    public Boolean mShowChestHoverTooltip        { get; set; }
    public Boolean mShowColourPaletteHelpTooltip { get; set; }

    public Boolean CanEdit() { return Context.IsMainPlayer; }

    public Int32 GetCapacity() { return Context.IsMainPlayer ? this.mRows * this.mColumns : this.mpInstance.mRows * this.mpInstance.mColumns; }

  #region Statics

    public static Config Get() { return sInstance; }

    public static void Load() { sInstance = GlobalVars.gSMAPIHelper.ReadConfig<Config>(); }

    public static void Save() {
      if (!Context.IsMainPlayer) return;

      GlobalVars.gSMAPIHelper.WriteConfig(sInstance);

      try { GlobalVars.gSMAPIHelper.Multiplayer.SendMessage(Get().mpInstance, "ConfigMP", new[] { GlobalVars.CONST_MOD_UID }); }
      catch (Exception) {
        GlobalVars.gSMAPIMonitor.Log("Config could NOT be sent to the multiplayer clients!" + $"\r\nDetails:\r\nWas class initialized? {(Get() is not null ? "Yes" : "No")}.",
                                     LogLevel.Error);
      }
    }

  #region SMAPI Events

    public static void PlayerConnecting(Object sender, PeerContextReceivedEventArgs e) {
      if (e.Peer.IsHost) return;

      try { GlobalVars.gSMAPIHelper.Multiplayer.SendMessage(Get().mpInstance, "ConfigMP", new[] { GlobalVars.CONST_MOD_UID }, new[] { e.Peer.PlayerID }); }
      catch (Exception) {
        GlobalVars.gSMAPIMonitor.Log($"Config could NOT be sent to the connecting multiplayer player (ID:{e.Peer.PlayerID})!"
                                     + $"\r\nDetails:\r\nWas class initialized? {(Get() is not null ? "Yes" : "No")}.",
                                     LogLevel.Error);
      }
    }

    public static void MPDataReceived(Object sender, ModMessageReceivedEventArgs e) {
      if (e.FromModID != GlobalVars.CONST_MOD_UID) return;
      if (e.Type != "ConfigMP") return;

      var config = e.ReadAs<ConfigMP>();
      Get().mRows    = config.mRows;
      Get().mColumns = config.mColumns;
    }

  #endregion

  #endregion

  #endregion

  #region Constructors

    public Config() {
      this.mpInstance                    = new ConfigMP();
      this.mRows                         = 6;
      this.mColumns                      = 14;
      this.mShowChestHoverTooltip        = true;
      this.mShowColourPaletteHelpTooltip = true;
    }

  #endregion
  }
}
