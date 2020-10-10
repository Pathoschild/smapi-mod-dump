/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/Server-Browser
**
*************************************************/

namespace ServerBrowser
{
	class SearchOptions
	{
		public bool ShowFullServers { get; set; } = true;
		public bool ShowFullCabinServers { get; set; } = true;
		public bool ShowPasswordProtectedSerers { get; set; } = false;
		public string SearchQuery { get; set; } = "";
	}
}
