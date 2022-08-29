/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Integrations.LuckSkill;

#region using directives

using Extensions.Reflection;
using System.Reflection;

#endregion using directives

/// <summary>Provides functionality missing from <see cref="ILuckSkillAPI"/>.</summary>
public static class ExtendedLuckSkillAPI
{
    public static MethodInfo GetProfessions = null!;

    /// <summary>Whether the reflected fields have been initialized.</summary>
    public static bool Initialized { get; private set; }

    public static void Init()
    {
        GetProfessions = "LuckSkill.Mod".ToType().RequireMethod("GetProfessions");
        Initialized = true;
    }
}