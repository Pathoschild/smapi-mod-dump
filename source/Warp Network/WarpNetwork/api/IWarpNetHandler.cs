/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/



namespace WarpNetwork.api
{
	public interface IWarpNetHandler
	{
		public void Warp();
		public bool GetEnabled();
		public string GetLabel();
		public string GetIconName();
	}
}
