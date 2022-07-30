/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Menuing;

/// <summary>
/// Extensions to help with menus.
/// </summary>
public static class MenuingExtensions
{
    // Thanks, RSV, for reminding me that there are other conditions for which I should probably not be handling shops....
    // From: https://github.com/Rafseazz/Ridgeside-Village-Mod/blob/816a66d0c9e667d3af662babc170deed4070c9ff/Ridgeside%20SMAPI%20Component%202.0/RidgesideVillage/TileActionHandler.cs#L37

    /// <summary>
    /// A couple of common checks.
    /// </summary>
    /// <returns>True if raising a menu is reasonble, false if that would be unwise.</returns>
    public static bool IsNormalGameplay()
        => Context.IsWorldReady && Context.CanPlayerMove && !Game1.player.isRidingHorse()
            && Game1.currentLocation is not null && !Game1.eventUp && !Game1.isFestival() && !Game1.IsFading()
            && Game1.activeClickableMenu is null;
}
