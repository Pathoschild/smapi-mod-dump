/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.MachineStatus
{
	public interface IDynamicGameAssetsApi
	{
		/// <summary>
		/// Get the DGA item ID of this item, if it has one.
		/// </summary>
		/// <param name="item">The item to get the DGA item ID of.</param>
		/// <returns>The DGA item ID if it has one, otherwise null.</returns>
		string? GetDGAItemId(object item);

		/// <summary>
		/// Spawn a DGA item, referenced with its full ID ("mod.id/ItemId").
		/// Some items, such as crafting recipes or crops, don't have an item representation.
		/// </summary>
		/// <param name="fullId">The full ID of the item to spawn.</param>
		/// <returns></returns>
		object? SpawnDGAItem(string fullId);
	}
}