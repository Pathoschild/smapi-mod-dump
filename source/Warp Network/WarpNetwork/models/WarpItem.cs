/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/



namespace WarpNetwork.models
{
	class WarpItem
	{
		public string Destination { set; get; }
		public bool IgnoreDisabled { set; get; } = false;
		public string Color { set; get; } = "#ffffff";
		public bool Consume { get; set; } = true;
	}
}
