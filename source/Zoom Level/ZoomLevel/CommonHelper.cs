/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thespbgamer/ZoomLevel
**
*************************************************/

using System;
using System.IO;
using StardewModdingAPI;

namespace ZoomLevel
{
    public partial class ModEntry
    {
        private class CommonHelper
        {
            internal static void RemoveObsoleteFiles(IMod mod, params string[] relativePaths)
            {
                string basePath = mod.Helper.DirectoryPath;

                foreach (string relativePath in relativePaths)
                {
                    string fullPath = Path.Combine(basePath, relativePath);
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            File.Delete(fullPath);
                            mod.Monitor.Log($"Removed obsolete file '{relativePath}'.");
                        }
                        catch (Exception ex)
                        {
                            mod.Monitor.Log($"Failed deleting obsolete file '{relativePath}':\n{ex}");
                        }
                    }
                }
            }
        }
    }
}