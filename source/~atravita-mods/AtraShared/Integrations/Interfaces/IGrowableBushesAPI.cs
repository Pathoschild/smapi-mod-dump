/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

using StardewValley.TerrainFeatures;

namespace AtraShared.Integrations.Interfaces;

/// <summary>
/// The API for Growable Bushes.
/// You will also need the enum BushSizes.cs.
/// </summary>
public interface IGrowableBushesAPI
{
    /// <summary>
    /// Checks whether or not an InventoryBush can be placed.
    /// </summary>
    /// <param name="obj">StardewValley.Object to place.</param>
    /// <param name="loc">GameLocation to place at.</param>
    /// <param name="tile">Tile to place at.</param>
    /// <param name="relaxed">Whether or to use relaxed placement rules.</param>
    /// <returns>True if the SObject is an InventoryBush and can be placed, false otherwise.</returns>
    public bool CanPlaceBush(SObject obj, GameLocation loc, Vector2 tile, bool relaxed);

    /// <summary>
    /// Called to place an InventoryBush at a specific location.
    /// </summary>
    /// <param name="obj">StardewValley.Object to place.</param>
    /// <param name="loc">GameLocation to place at.</param>
    /// <param name="tile">Which tile to place at.</param>
    /// <param name="relaxed">Whether or to use relaxed placement rules.</param>
    /// <returns>True if the SObject is an InventoryBush and was successfully placed, false otherwise.</returns>
    /// <remarks>Does not handle inventory management.</remarks>
    public bool TryPlaceBush(SObject obj, GameLocation loc, Vector2 tile, bool relaxed);

    /// <summary>
    /// Gets the InventoryBush associated with a specific size.
    /// </summary>
    /// <param name="size">The size to get.</param>
    /// <returns>An InventoryBush.</returns>
    public SObject GetBush(BushSizes size);

    /// <summary>
    /// Gets the size associated with this InventoryBush, or <see cref="BushSizes.Invalid"/> if it's not an InventoryBush.
    /// </summary>
    /// <param name="obj">Object to check.</param>
    /// <returns>Size if applicable, or <see cref="BushSizes.Invalid"/> if not.</returns>
    public BushSizes GetSizeOfBushIfApplicable(StardewValley.Object obj);

    /// <summary>
    /// Checks to see if a bush can be picked up from this location.
    /// </summary>
    /// <param name="loc">Game location.</param>
    /// <param name="tile">Tile to pick up from.</param>
    /// <param name="placedOnly">Whether or not we should be limited to just bushes we placed.</param>
    /// <returns>A BushSizes that corresponds size of the bush, or <see cref="BushSizes.Invalid"/> otherwise.</returns>
    /// <remarks>For safety reasons, a real (non-decorative) walnut bush still bearing a walnut will not be pick-up-able.</remarks>
    public BushSizes CanPickUpBush(GameLocation loc, Vector2 tile, bool placedOnly = false);

    /// <summary>
    /// Picks up a bush if possible.
    /// </summary>
    /// <param name="loc">Game location.</param>
    /// <param name="tile">Tile to pick up from.</param>
    /// <param name="placedOnly">Whether or not we should be limited to just bushes we placed.</param>
    /// <returns>InventoryBush if successfully picked up, null otherwise.</returns>
    /// <remarks>For safety reasons, a real (non-decorative) walnut bush still bearing a walnut will not be pick-up-able.
    /// This method also does NOT handle adding the bush to the user's inventory.</remarks>
    public SObject? TryPickUpBush(GameLocation loc, Vector2 tile, bool placedOnly = false);

    /// <summary>
    /// Draws in a little bush flinging animation corresponding to a specific inventoryBush.
    /// </summary>
    /// <param name="obj">Inventory bush to fling.</param>
    /// <param name="loc">Game location to fling from.</param>
    /// <param name="tile">The tile to fling from.</param>
    public void DrawPickUpGraphics(StardewValley.Object obj, GameLocation loc, Vector2 tile);
}

/// <summary>
/// Valid bush sizes.
/// </summary>
public enum BushSizes
{
    /// <summary>
    /// The marker for an invalid bush.
    /// </summary>
    Invalid = -1,

    // base game bush sizes

    /// <summary>
    /// A small 16x32 bush.
    /// </summary>
    Small = Bush.smallBush,

    /// <summary>
    /// The medium sized, berry bushes.
    /// </summary>
    Medium = Bush.mediumBush,

    /// <summary>
    /// A large bush.
    /// </summary>
    Large = Bush.largeBush,

    /// <summary>
    /// A harvested walnut bush.
    /// </summary>
    Harvested = Bush.walnutBush - 1,

    /// <summary>
    /// A decorative walnut bush.
    /// </summary>
    Walnut = Bush.walnutBush,

    // weird sizes

    /// <summary>
    /// The alternate form of the 16x32 small bush.
    /// </summary>
    SmallAlt = 7,

    /// <summary>
    /// A medium sized bush that usually grows in town.
    /// </summary>
    Town = 8,

    /// <summary>
    /// A large bush that usually grows in town.
    /// </summary>
    TownLarge = 9,
}