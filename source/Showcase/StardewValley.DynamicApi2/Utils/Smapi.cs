using System;
using System.Reflection;
using StardewModdingAPI;

namespace Igorious.StardewValley.DynamicApi2.Utils
{
    internal class Smapi
    {
        public static IMonitor GetMonitor(string source)
        {
            var getSecondaryMonitorMethod = typeof(Program).GetMethod("GetSecondaryMonitor", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (getSecondaryMonitorMethod == null) throw new InvalidOperationException("[DAPI2] Can't create Monitor object.");
            return (IMonitor)getSecondaryMonitorMethod.Invoke(null, new object[] {source});
        }

        public static IModRegistry GetModRegistry()
        {
            var modRegistryField = typeof(Program).GetField("ModRegistry", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (modRegistryField == null) throw new InvalidOperationException("[DAPI2] Can't get ModRegistry object.");
            return (IModRegistry)modRegistryField.GetValue(null);
        }
    }
}