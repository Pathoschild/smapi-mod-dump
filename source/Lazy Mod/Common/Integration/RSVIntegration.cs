/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Reflection;

namespace Common.Integration;

public class RSVIntegration
{
    private const string DllPath = @"C:\Projects\StardewValleyMods\Common\ModDll\RidgesideVillage.dll";

    public static Type? GetType(string name)
    {
        var assembly = Assembly.LoadFrom(DllPath);
        return assembly.GetType(name);
    }
}