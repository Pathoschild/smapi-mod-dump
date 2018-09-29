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
