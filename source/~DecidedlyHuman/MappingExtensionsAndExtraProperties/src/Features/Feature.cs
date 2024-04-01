/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Reflection;
using DecidedlyShared.Constants;
using DecidedlyShared.Logging;
using DecidedlyShared.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MappingExtensionsAndExtraProperties.Features;

public abstract class Feature
{
    /// <summary>
    /// The ID of this feature for pack loading purposes.
    /// </summary>
    public abstract string FeatureId { get; init; }

    /// <summary>
    /// The <see cref="Harmony">Harmony</see> reference used to apply this feature's patches.
    /// </summary>
    public abstract Harmony HarmonyPatcher { get; init; }

    /// <summary>
    /// Whether this feature has a position in the world, or will otherwise respond to cursor position.
    /// </summary>
    public abstract bool AffectsCursorIcon { get; init; }

    /// <summary>
    /// If <see cref="AffectsCursorIcon"/> is true, the feature will expose the cursor ID to be used here.
    /// </summary>
    public abstract int CursorId { get; init; }

    /// <summary>
    /// Whether or not this feature has been enabled.
    /// </summary>
    public abstract bool Enabled { get; internal set; }

    /// <summary>
    /// Performs any actions this feature requires to be enabled. This often involves applying some Harmony patches.
    /// <returns>True if the feature was initialised successfully, and false if something failed.</returns>
    /// </summary>
    public abstract void Enable();

    /// <summary>
    /// Disable this feature and all of its functionality.
    /// </summary>
    public abstract void Disable();

    /// <summary>
    /// This is where the feature should register any callbacks it requires with the feature manager.
    ///
    /// Called once when the feature is added to the manager.
    /// </summary>
    public abstract void RegisterCallbacks();

    /// <summary>
    /// Whether the game's cursor should be changed when hovering over a given tile.
    /// </summary>
    /// <param name="tile">The in-world tile to check against.</param>
    /// <param name="cursorId">The cursor ID to change the cursor to.</param>
    /// <returns></returns>
    public abstract bool ShouldChangeCursor(GameLocation location, int tileX, int tileY, out int cursorId);

    public override int GetHashCode()
    {
        return this.FeatureId.GetHashCode();
    }
}
