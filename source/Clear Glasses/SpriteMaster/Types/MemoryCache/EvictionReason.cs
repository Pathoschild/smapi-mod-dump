/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Types.MemoryCache;

public enum EvictionReason {
	None,
	/// <summary>Manually</summary>
	Removed,
	/// <summary>Overwritten</summary>
	Replaced,
	/// <summary>Timed out</summary>
	Expired,
	/// <summary>Event</summary>
	TokenExpired,
	/// <summary>Overflow</summary>
	Capacity,
}
