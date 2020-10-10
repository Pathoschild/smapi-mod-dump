/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drynwynn/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace DsStardewLib.Config
{
  /// <summary>
  /// Specify this config can be used as part of the HarmonyWrapper loading system.  The config must
  /// tell the mod whether it should load Harmony and if it should turn on debug.
  /// </summary>
  interface HarmonyConfig
  {
    /// <summary>
    /// If false, even if the conditional 'INCLUDEHARMONY' is defined, Harmony will not load or run this hack.
    /// </summary>
    bool HarmonyLoad { get; set; }

    /// <summary>
    /// Turn on the global Harmony debug information flag.
    /// </summary>
    bool HarmonyDebug { get; set; }
  }
}
