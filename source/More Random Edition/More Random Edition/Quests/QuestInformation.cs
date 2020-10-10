/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Contains the information required for editing quests
	/// </summary>
	public class QuestInformation
	{
		public Dictionary<int, string> QuestReplacements = new Dictionary<int, string>();
		public Dictionary<string, string> MailReplacements = new Dictionary<string, string>();

		public QuestInformation(
			Dictionary<int, string> questReplacements,
			Dictionary<string, string> mailReplacements)
		{
			QuestReplacements = questReplacements;
			MailReplacements = mailReplacements;
		}
	}
}
