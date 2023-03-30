/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace PrismaticSlime.Framework;

/// <summary>
/// The data model, used to save IDs so we can migrate later.
/// </summary>
public sealed class DataModel
{
    /// <summary>
    /// Gets or sets the int ID for the prismatic toast.
    /// </summary>
    public int ToastId { get; set; } = -1;

    /// <summary>
    /// Gets or sets the int ID for the prismatic slime ring.
    /// </summary>
    public int RingId { get; set; } = -1;

    /// <summary>
    /// Gets or sets the int id for the prismatic slime egg.
    /// </summary>
    public int EggId { get; set; } = -1;
}
