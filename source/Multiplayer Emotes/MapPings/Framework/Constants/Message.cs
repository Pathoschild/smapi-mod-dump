/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/


namespace MapPings.Framework.Constants {

	internal static class Message {

		internal const byte TypeID = 50;

		internal enum Action {
			None,
			SendMapPing,
			BroadcastMapPing
		};

	}

}
