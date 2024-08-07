/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Newtonsoft.Json;
using StardewModdingAPI;

namespace Igorious.StardewValley.DynamicAPI
{
    public abstract class DynamicConfiguration
    {
        public virtual void CreateDefaultConfiguration() {}

        public void Load(string rootPath)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;            
            CreateDefaultConfiguration();

            var jsonSettings = new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.Ignore};
            jsonSettings.Converters.AddDefaults();

            var basePath = Path.Combine(rootPath, "Configuration");
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

            var properties = GetType().GetProperties();
            foreach (var property in properties.Where(p => p.CanWrite))
            {               
                var filePath = Path.Combine(basePath, $"{property.Name}.json");
                if (File.Exists(filePath))
                {
                    try
                    {
                        var value = JsonConvert.DeserializeObject(File.ReadAllText(filePath), property.PropertyType, jsonSettings);
                        property.SetValue(this, value);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Exception during reading {property.Name}. Cause: {e}");
                    }
                }
                else
                {
                    var value = property.GetValue(this);
                    File.WriteAllText(filePath, value.ToJson());
                    Log.Info($"Created default configuration for {property.Name}");
                }              
            }
        }
    }
}
