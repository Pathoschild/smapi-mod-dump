/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ZaneYork/SDV_CustomLocalization
**
*************************************************/

using System;
using System.IO;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class TitleContainerRewrites
    {
        public class OpenStreamRewrite
        {
            public static bool Prefix(string name, ref Stream __result)
            {
                Stream stream = null;
                if (string.IsNullOrEmpty(name))
                {
                    return true;
                }
                string newPath = Path.Combine(ModEntry.ModPath, name);
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
                        string safeName = ModEntry.Reflection.GetMethod(typeof(TitleContainer), "NormalizeRelativePath", false)?.Invoke<string>(name);
                        if(safeName != null)
                            __result = ModEntry.Reflection.GetMethod(typeof(TitleContainer), "PlatformOpenStream", false)?.Invoke<Stream>(safeName);
                        else
                            __result = ModEntry.Reflection.GetMethod(typeof(TitleContainer), "PlatformOpenStream", false)?.Invoke<Stream>(name);
                        if(__result == null)
                        {
                            return true;
                        }
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
