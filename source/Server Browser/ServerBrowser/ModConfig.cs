/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/Server-Browser
**
*************************************************/

using System.Collections.Generic;

namespace ServerBrowser
{
	class ModConfig
	{
		public List<string> RequiredMods { get; set; } = new List<string>() { };//Use UniqueIDs, e.g. Ilyaki.ServerBrowser
		public string ServerMessage { get; set; } = "This is my server message.\nHello world!";
		public string Password { get; set; } = "";
		public bool PublicCheckedByDefault = false;
	}
}
