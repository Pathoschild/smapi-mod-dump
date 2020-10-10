/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/


using MapPings.Framework.Constants;
using StardewValley;
using System;

namespace MapPings.Framework.Events {
	public class ChatMessageEventArgs : EventArgs {
		public Farmer SourcePlayer { get; set; }
		public ChatMessageKind ChatKind { get; set; }
		public LocalizedContentManager.LanguageCode Language { get; set; }
		public string Message { get; set; }
	}
}
