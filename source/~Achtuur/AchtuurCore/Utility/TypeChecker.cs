/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

namespace AchtuurCore.Utility;

public static class TypeChecker
{
    public static bool isType<T>(object obj)
    {
        return obj is not null && obj.GetType() == typeof(T);
    }
}
