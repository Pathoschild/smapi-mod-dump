/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using System;

namespace AchtuurCore.Utility;

public static class Debug
{
    /// <summary>
    /// Execute a method only if in debug mode, so you never have to deal with <c>#if DEBUG</c> automatic indents in visual studio :)
    /// </summary>
    /// <param name="func"></param>
    public static void DebugOnlyExecute(Action func)
    {
#if DEBUG
        func.Invoke();
#endif
    }
}
