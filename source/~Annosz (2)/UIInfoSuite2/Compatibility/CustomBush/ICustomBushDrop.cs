/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UIInfoSuite2
**
*************************************************/

using StardewValley;
using StardewValley.GameData;

namespace UIInfoSuite2.Compatibility.CustomBush;

/// <summary>Model used for drops from custom bushes.</summary>
public interface ICustomBushDrop : ISpawnItemData
{
  /// <summary>Gets the specific season when the item can be produced.</summary>
  public Season? Season { get; }

  /// <summary>Gets the probability that the item will be produced.</summary>
  public float Chance { get; }

  /// <summary>A game state query which indicates whether the item should be added. Defaults to always added.</summary>
  public string? Condition { get; }

  /// <summary>
  ///   An ID for this entry within the current list (not the item itself, which is
  ///   <see cref="P:StardewValley.GameData.GenericSpawnItemData.ItemId" />). This only needs to be unique within the current
  ///   list. For a custom entry, you should use a globally unique ID which includes your mod ID like
  ///   <c>ExampleMod.Id_ItemName</c>.
  /// </summary>
  public string? Id { get; }
}
