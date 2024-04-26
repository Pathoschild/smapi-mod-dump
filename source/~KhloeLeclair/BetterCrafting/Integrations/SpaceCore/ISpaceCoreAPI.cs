/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable disable

using System;
using System.Reflection;

using StardewValley;

namespace SpaceCore {
	public interface IApi {
		string[] GetCustomSkills();
		int GetLevelForCustomSkill(Farmer farmer, string skill);
		void AddExperienceForCustomSkill(Farmer farmer, string skill, int amt);
		int GetProfessionId(string skill, string profession);

	}
}
