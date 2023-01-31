/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.ConstantsAndEnums;

/// <summary>
/// Static class that contains common error messages.
/// </summary>
public static class ErrorMessageConsts
{
    /// <summary>
    /// The error message string used for a crash that happens while applying harmony patches.
    /// </summary>
    public const string HARMONYCRASH = "Mod crashed while applying harmony patches:\n\n{0}";

    /// <summary>
    /// The message string used to suppress StyleCop ordering warnings for records.
    /// </summary>
    public const string STYLECOPRECORDS = "Records break StyleCop :(.";
}