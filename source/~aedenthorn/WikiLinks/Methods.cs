/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Diagnostics;
using System.Globalization;

namespace WikiLinks
{
    public partial class ModEntry
    {
        private static void OpenPage(string page)
        {
            SMonitor.Log($"Opening wiki page for {page}");
            var ps = new ProcessStartInfo($"https://stardewvalleywiki.com/{page}")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

    }
}