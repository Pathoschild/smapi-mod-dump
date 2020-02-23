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
