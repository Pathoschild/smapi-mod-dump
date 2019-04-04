
using Harmony;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;
using CustomEmojis.Framework.Extensions;
using Microsoft.Xna.Framework;

namespace CustomEmojis.Framework.Patches {

	internal static class ChatBoxPatch {

		internal class ReceiveChatMessagePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(ChatBox), nameof(Game1.chatBox.receiveChatMessage), new Type[] { typeof(long), typeof(int), typeof(LocalizedContentManager.LanguageCode), typeof(string) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(ReceiveChatMessagePatch.ReceiveChatMessage_Postfix));

			private static void ReceiveChatMessage_Postfix(ChatBox __instance, ref long sourceFarmer, ref int chatKind, ref LocalizedContentManager.LanguageCode language, ref string message) {
				__instance.ChatBoxReceivedMessage(sourceFarmer, chatKind, language, message);
			}

		}

		internal class AddMessagePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(ChatBox), nameof(Game1.chatBox.addMessage), new Type[] { typeof(string), typeof(Color) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(AddMessagePatch.AddChatMessage_Postfix));

			private static void AddChatMessage_Postfix(ChatBox __instance, ref string message, ref Color color) {
				__instance.ChatBoxAddedMessage(message, color);
			}

		}

	}

}
