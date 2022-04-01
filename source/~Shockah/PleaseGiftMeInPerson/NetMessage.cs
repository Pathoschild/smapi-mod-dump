/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.PleaseGiftMeInPerson
{
	internal static class NetMessage
	{
		public struct RecordGift
		{
			public long PlayerID { get; set; }
			public string NpcName { get; set; }
			public GiftEntry GiftEntry { get; set; }

			public RecordGift(long playerID, string npcName, GiftEntry giftEntry)
			{
				this.PlayerID = playerID;
				this.NpcName = npcName;
				this.GiftEntry = giftEntry;
			}
		}
	}
}
