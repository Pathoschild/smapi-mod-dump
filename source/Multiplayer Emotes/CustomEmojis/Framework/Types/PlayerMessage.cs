using CustomEmojis.Framework.Constants;
using CustomEmojis.Framework.Utilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CustomEmojis.Framework.Types {

	public class PlayerMessage {

		public Farmer Player { get; set; }
		public ChatMessageKind ChatKind { get; set; }
		public LocalizedContentManager.LanguageCode Language { get; set; }
		public string Message { get; set; }
		public int MessageHash { get; }

		public List<MessageEmoji> MessageEmojis = new List<MessageEmoji>();

		//public PlayerMessage(Farmer sourcePlayer, LocalizedContentManager.LanguageCode language, string message) {

		//	Player = sourcePlayer;
		//	Language = language;
		//	Message = message;

		//	MatchCollection matchCollection = Regex.Matches(message, @"\[(-?[0-9]+)\]");

		//	int lastIndex = 0;
		//	int horizontalPosition = 0;
		//	foreach(Match match in matchCollection) {
		//		if(int.TryParse(match.Value.Trim('[', ']'), out int index)) {
		//			string messageSubstring = message.Substring(lastIndex, match.Index - lastIndex);
		//			ModEntry.ModLogger.Log($"Message: {message}", $"Substring Message: {messageSubstring}", $"Current Index: {lastIndex}", $"message.Length: {message.Length}", $"match.Index: {match.Index}", $"match.Length: {match.Length}");
		//			horizontalPosition += MeasureString(language, messageSubstring);
		//			MessageEmojis.Add(new MessageEmoji(index, horizontalPosition, true));
		//			horizontalPosition += 40; // We add the emoji width
		//			lastIndex = match.Index + match.Length;
		//		}
		//	}


		//}

		public PlayerMessage(Farmer sourcePlayer, LocalizedContentManager.LanguageCode language, string message, int messageHash) {

			Player = sourcePlayer;
			Language = language;
			Message = message;
			MessageHash = messageHash;

			MatchCollection matchCollection = Regex.Matches(Message, @"\[(-?[0-9]+)\]");

			int lastIndex = 0;
			int horizontalPosition = 0;
			int verticalPosition = 0;
			foreach(Match match in matchCollection) {

				if(int.TryParse(match.Value.Trim('[', ']'), out int index)) {

					string messageSubstring = Message.Substring(lastIndex, match.Index - lastIndex);
					string[] breakedLines = ModUtilities.BreakLines(messageSubstring);
#if DEBUG
                    ModEntry.ModLogger.LogToMonitor = false;
                    ModEntry.ModLogger.Log($"Message: \"{Message}\"", $"Substring Message: {messageSubstring}", $"Current Index: {lastIndex}", $"message.Length: {message.Length}", $"match.Index: {match.Index}", $"match.Length: {match.Length}");
                    ModEntry.ModLogger.LogToMonitor = true;
#endif
                    int stringMeasure = (int)MeasureString(Language, breakedLines.Last()).X;

					if(breakedLines.Count() > 1) {
						horizontalPosition = 0;
						verticalPosition += (int)MeasureString(Language, "(").Y;
					}

					horizontalPosition += stringMeasure;
					MessageEmojis.Add(new MessageEmoji(index, horizontalPosition, verticalPosition, MessageHash, true));

					horizontalPosition += 40; // We add the emoji width

					lastIndex = match.Index + match.Length;

				}

			}


		}

		private Vector2 MeasureString(LocalizedContentManager.LanguageCode language, string str) {
			return ChatBox.messageFont(language).MeasureString(str);
		}

	}

}
