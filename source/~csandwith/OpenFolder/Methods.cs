/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System;
using System.Text;
using System.Xml;
using StardewValley.Buildings;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace OpenFolder
{
    public partial class ModEntry
    {
        private static void TryOpenFolder(string folder)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = folder,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch(Exception ex)
            {
                SMonitor.Log($"Error opening folder {folder}: \n\n{ex}");
            }
        }

    }
}