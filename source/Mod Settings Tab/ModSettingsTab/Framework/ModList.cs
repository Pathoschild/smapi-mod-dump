/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GilarF/SVM
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace ModSettingsTab.Framework
{
    /// <summary>
    /// main collection of modifications
    /// </summary>
    /// <remarks>
    /// Dictionary&lt;uniqueId, Mod&gt;
    /// </remarks>
    public class ModList : Dictionary<string, Mod>
    {
        public ModList()
        {
            var path = Path.Combine(Constants.ExecutionPath, "Mods");
            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                try
                {
                    var configPath = Path.Combine(directory, "config.json");
                    var manifestPath = Path.Combine(directory, "manifest.json");
                    
                    if (!File.Exists(manifestPath)) continue;
                    var uniqueId = JObject.Parse(File.ReadAllText(manifestPath))["UniqueID"].ToString();
                    StaticConfig config = null;
                    if (File.Exists(configPath))
                    {
                        var jObj = JObject.Parse(File.ReadAllText(configPath));
                        config = new StaticConfig(configPath, jObj, uniqueId);
                    }
                    Add(uniqueId, new Mod(uniqueId, directory, config));
                }
                catch (Exception e)
                {
                    Helper.Console.Warn(e.Message);
                }
            }
        }
    }
}