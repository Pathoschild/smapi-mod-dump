/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;

namespace OrnithologistsGuild.Models
{
	public class SaveData
	{
		public Dictionary<long, PlayerSaveData> Players;

		public SaveData()
		{
			Players = new Dictionary<long, PlayerSaveData>();
		}

		public PlayerSaveData ForPlayer(long uniquePlayerId)
		{
			if (!Game1.IsMasterGame && Game1.player.UniqueMultiplayerID != uniquePlayerId)
			{
				throw new ArgumentOutOfRangeException(nameof(uniquePlayerId), "Farmhands only have access to their own PlayerSaveData");
			}

			if (!Players.ContainsKey(uniquePlayerId))
			{
				Players[uniquePlayerId] = new PlayerSaveData();
			}

			return Players[uniquePlayerId];
		}
	}

	public class PlayerSaveData
	{
        public LifeList LifeList;

        public PlayerSaveData()
        {
            this.LifeList = new LifeList();
        }
    }
}

