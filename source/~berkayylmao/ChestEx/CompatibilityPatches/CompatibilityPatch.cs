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

// 
//    ChestEx (StardewValleyMods)
//    Copyright (c) 2022 Berkay Yigit <berkaytgy@gmail.com>
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY, without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;

using StardewModdingAPI;

namespace ChestEx.CompatibilityPatches {
  internal class CompatibilityPatch {
    // Private:
    #region Private

    private String name;
    private readonly String uid;
    private readonly ISemanticVersion version;

    private void printInfo() {
      if (GlobalVars.gSMAPIHelper.ModRegistry.Get(this.uid) is not IModInfo info) return;
      this.name = info.Manifest.Name;
      this.beginLog();

      if (info.Manifest.Version.Equals(this.version))
        GlobalVars.gSMAPIMonitor.Log("ChestEx is fully compatible with this version.", LogLevel.Info);
      else if (info.Manifest.Version.IsNewerThan(this.version))
        GlobalVars.gSMAPIMonitor.Log($"You seem to be running a newer version of '{info.Manifest.Name}'! "
                                     + "This warning can safely be ignored if you don't experience any issues. "
                                     + "However, if you do experience any issues, please report it to me on the Stardew Valley Discord or on Nexus (https://www.nexusmods.com/stardewvalley/mods/4180).",
                                     LogLevel.Warn);
      else if (info.Manifest.Version.IsOlderThan(this.version))
        GlobalVars.gSMAPIMonitor.Log($"You seem to be running an older version of '{info.Manifest.Name}'! "
                                     + $"There is a high chance that you will experience issues, please update your copy of '{info.Manifest.Name}'.",
                                     LogLevel.Warn);

      this.OnLoaded();
    }

    private void beginLog() {
      GlobalVars.gSMAPIMonitor.Log($"--------------------- Compatibility info for '{this.name}':", LogLevel.Info);
      GlobalVars.gSMAPIMonitor.Log($"The target version of '{this.name}' for the compatibility patches is '{this.version}'.", LogLevel.Info);
    }

    private void endLog() { GlobalVars.gSMAPIMonitor.Log("Finished installing compatibility patches.", LogLevel.Info); }

    #endregion

    // Protected:
    #region Protected

    // Virtuals:
    #region Virtuals

    protected virtual void InstallPatches() { }

    protected virtual void OnLoaded() {
      this.InstallPatches();
      this.endLog();
    }

    #endregion

    #endregion

    // Public:
    #region Public

    public void Install() { this.printInfo(); }

    #endregion

    // Constructors:
    #region Constructors

    internal CompatibilityPatch(String uid, ISemanticVersion version) {
      this.uid = uid;
      this.version = version;
    }

    #endregion
  }
}
