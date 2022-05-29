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

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;

namespace ChestEx {
  public static class GlobalVars {
    #region Mod

    public const String CONST_MOD_UID = "berkayylmao.ChestEx";

    public static Harmony gHarmony;

    #endregion

    #region Stardew Valley

    public static Rectangle gUIViewport {
      get {
        xTile.Dimensions.Rectangle _ = Game1.uiViewport;

        return new Rectangle(0, 0, _.Width, _.Height);
      }
    }

    public static Rectangle gGameViewport => Utility.getSafeArea();

    #region Events

    // public delegate void             ObjectAction(GameLocation location, Vector2 tile);
    // public static event ObjectAction ObjectPlaced;
    // public static event ObjectAction ObjectRemoved;
    // 
    // public static void OnObjectPlaced(GameLocation  location, Vector2 tile) { ObjectPlaced?.Invoke(location, tile); }
    // public static void OnObjectRemoved(GameLocation location, Vector2 tile) { ObjectRemoved?.Invoke(location, tile); }

    #endregion

    #endregion

    #region SMAPI

    public static IModHelper gSMAPIHelper;
    public static IMonitor gSMAPIMonitor;

    #endregion

    #region Compatibilities

    public static Boolean gIgnoreInput = false;

    public static Boolean gIsAutomateLoaded = false;
    public static Boolean gIsChestsAnywhereLoaded = false;
    public static Boolean gIsConvenientChestsLoaded = false;
    public static Boolean gIsExpandedStorageLoaded = false;
    public static Boolean gIsRemoteFridgeStorageLoaded = false;

    #endregion
  }
}
