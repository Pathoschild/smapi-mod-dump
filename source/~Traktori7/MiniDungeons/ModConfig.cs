/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;


namespace MiniDungeons
{
	internal class ModConfig
	{
		public bool enableHUDNotification = true;
		public int maxNumberOfDungeonsPerDay = -1;
		public bool enableFightingchallenges = true;
		public bool enableDeathProtection = true;
		public Dictionary<string, bool> enabledDungeons = new Dictionary<string, bool>();
		public Dictionary<string, float> dungeonSpawnChances = new Dictionary<string, float>();
	}
}
