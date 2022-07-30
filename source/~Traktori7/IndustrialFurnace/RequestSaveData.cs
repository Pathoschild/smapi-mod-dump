/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

namespace IndustrialFurnace
{
	public class RequestSaveData
	{
		public long PlayerID { get; set; }


		public RequestSaveData(long playerID)
		{
			PlayerID = playerID;
		}
	}
}
