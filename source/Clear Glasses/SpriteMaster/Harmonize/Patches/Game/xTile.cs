/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Threading;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class xTile {

	[Harmonize(
		"xTile.Tiles.TileIndexPropertyAccessor",
		"get_Item",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.First,
		critical: false
	)]
	public static void DeprecationManagerWarnPre(
		object __instance,
		object ___m_cache,
		int tileIndex
	) {
		Monitor.Enter(___m_cache);
	}

	[Harmonize(
		"xTile.Tiles.TileIndexPropertyAccessor",
		"get_Item",
		Harmonize.Fixation.Finalizer,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void DeprecationManagerWarnPost(
		object __instance,
		object ___m_cache,
		int tileIndex
	) {
		Monitor.Exit(___m_cache);
	}

	[Harmonize(
		"xTile.Tiles.TileIndexPropertyCollection",
		"IndexKey",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.First,
		critical: false
	)]
	public static void DeprecationManagerWarnPre(
		object __instance,
		object ___m_indexKeys,
		string key
	) {
		Monitor.Enter(___m_indexKeys);
	}

	[Harmonize(
		"xTile.Tiles.TileIndexPropertyCollection",
		"IndexKey",
		Harmonize.Fixation.Finalizer,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void DeprecationManagerWarnPost(
		object __instance,
		object ___m_indexKeys,
		string key
	) {
		Monitor.Exit(___m_indexKeys);
	}
}
