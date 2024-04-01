/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System.IO;
using System.Linq;

namespace BushBloomMod.Patches.Integrations {
    internal static class Almanac {
        public static void Register(IManifest manifest, IModHelper helper, IMonitor monitor, Config config) {
            try {
                System.Reflection.Assembly.LoadFrom(Path.Combine(new FileInfo(typeof(ModEntry).Assembly.Location).DirectoryName, "AlmanacBBM.dll"))
                    .GetTypes().Where(t => t.Name == "Almanac")
                    .Select(t => t.GetMethod("Register"))
                    .FirstOrDefault().Invoke(null, new object[] { manifest, helper, monitor, config });
            } catch { }
        }
    }
}