
using CustomEmojis.Framework.Constants;
using CustomEmojis.Framework.Events;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;

namespace CustomEmojis.Framework.Extensions {

	public static class ChatBoxExtension {

		public static event EventHandler<ChatMessageEventArgs> OnChatBoxReceivedMessage = delegate { };
		public static event EventHandler<ChatMessageEventArgs> OnChatBoxAddedMessage = delegate { };

		public static void ChatBoxReceivedMessage(this ChatBox chatBox, long sourceFarmer, int chatKind, LocalizedContentManager.LanguageCode language, string message) {
			ChatMessageEventArgs args = new ChatMessageEventArgs {
				SourcePlayer = Game1.getFarmer(sourceFarmer),
				ChatKind = (ChatMessageKind)Enum.ToObject(typeof(ChatMessageKind), chatKind),
				Language = language,
				Message = message
			};
#if DEBUG
            ModEntry.ModLogger.LogToMonitor = false;
            ModEntry.ModLogger.LogTrace();
            ModEntry.ModLogger.LogToMonitor = true;
#endif
            OnChatBoxReceivedMessage(null, args);
		}

		public static void ChatBoxAddedMessage(this ChatBox chatBox, string message, Color color) {

			ChatMessageKind messageKind;
			if(color == Game1.chatBox.chatBox.TextColor) {
				messageKind = ChatMessageKind.Normal;
			} else if(color == Color.Yellow) {
				messageKind = ChatMessageKind.Notification;
			} else {
				messageKind = ChatMessageKind.Error;
			}

			ChatMessageEventArgs args = new ChatMessageEventArgs {
				SourcePlayer = Game1.player,
				ChatKind = messageKind,
				Language = LocalizedContentManager.CurrentLanguageCode,
				Message = message
			};
#if DEBUG
			ModEntry.ModLogger.LogToMonitor = false;
			ModEntry.ModLogger.LogTrace();
			ModEntry.ModLogger.LogToMonitor = true;
#endif
			OnChatBoxAddedMessage(null, args);
		}

	}

}
