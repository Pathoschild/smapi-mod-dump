/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeonBlade/TreeTransplant
**
*************************************************/

using StardewValley;

namespace TreeTransplant
{
	public class Notifier
	{
		public static void Message(string message, int time = 3500)
		{
			Game1.hudMessages.Add(new HUDMessage(message, 2) { timeLeft = time });
		}
	}
}
