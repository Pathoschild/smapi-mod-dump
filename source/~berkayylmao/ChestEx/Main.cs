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
//    Copyright (c) 2021 Berkay Yigit <berkaytgy@gmail.com>
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

using ChestEx.Types.CustomTypes.ExtendedSVObjects;

using Harmony;

using JetBrains.Annotations;

using StardewModdingAPI;

namespace ChestEx {
  [UsedImplicitly]
  public class Main : Mod {
    public override void Entry(IModHelper helper) {
      GlobalVars.gSMAPIHelper  = helper;
      GlobalVars.gSMAPIMonitor = this.Monitor;
      GlobalVars.gHarmony      = HarmonyInstance.Create("mod.berkayylmao.ChestEx");

      // Config
      {
        Config.Load();
        helper.Events.Multiplayer.PeerContextReceived += Config.PlayerConnecting;
        helper.Events.Multiplayer.ModMessageReceived  += Config.MPDataReceived;
      }

      // Main Patches
      {
        Patches.SVItemGrabMenu.Install();
        Patches.SVChest.Install();

        // Chest tooltip
        helper.Events.Display.RenderingHud += ExtendedChest.OnRenderingHud;
      }
      // Compatibility Patches
      {
        new CompatibilityPatches.Automate().Install();
        new CompatibilityPatches.ChestsAnywhere().Install();
        new CompatibilityPatches.ConvenientChests().Install();
        new CompatibilityPatches.RemoteFridgeStorage().Install();
      }
    }
  }
}
