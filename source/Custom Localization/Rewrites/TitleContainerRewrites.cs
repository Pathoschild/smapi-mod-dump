using System;
using System.IO;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class TitleContainerRewrites
    {
        [HarmonyPatch(typeof(TitleContainer))]
        [HarmonyPatch("OpenStream")]
        public class OpenStreamRewrite
        {
            [HarmonyPrefix]
            public static bool Prefix(string name, ref Stream __result)
            {
                Stream stream = null;
                if (string.IsNullOrEmpty(name))
                {
                    return true;
                }
                string newPath = Path.Combine(ModEntry.ModPath, name);
                string safeName = (string)typeof(TitleContainer).GetMethod("NormalizeRelativePath", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { name });
                try
                {
                    stream = new FileStream(newPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    __result = stream;
                    return false;
                }
                catch
                {
                }
                if (stream == null)
                {
                    try
                    {
                        MethodInfo PlatformOpenStream = typeof(TitleContainer).GetMethod("PlatformOpenStream", BindingFlags.Static | BindingFlags.NonPublic);
                        stream = (Stream)PlatformOpenStream.Invoke(null, new object[] { safeName });
                        __result = stream;
                    }
                    catch
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
