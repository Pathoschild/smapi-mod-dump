
using Harmony;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;
using CustomEmojis.Framework.Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace CustomEmojis.Framework.Patches {

	internal static class ModSupportPatch {

		internal class ChatCommands_ReceiveChatMessage : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(Type.GetType("ChatCommands.ClassReplacements.CommandChatBox, ChatCommands"), nameof(Game1.chatBox.receiveChatMessage), new Type[] { typeof(long), typeof(int), typeof(LocalizedContentManager.LanguageCode), typeof(string) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(ChatCommands_ReceiveChatMessage.ReceiveChatMessage_Postfix));

			private static void ReceiveChatMessage_Postfix(ChatBox __instance, ref long sourceFarmer, ref int chatKind, ref LocalizedContentManager.LanguageCode language, ref string message) {
				__instance.ChatBoxReceivedMessage(sourceFarmer, chatKind, language, message);
			}

		}

		internal class ChatCommands_AddMessagePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(Type.GetType("ChatCommands.ClassReplacements.CommandChatBox, ChatCommands"), nameof(Game1.chatBox.addMessage), new Type[] { typeof(string), typeof(Color) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(ChatCommands_AddMessagePatch.AddChatMessage_Postfix));

			private static void AddChatMessage_Postfix(ChatBox __instance, ref string message, ref Color color) {
				__instance.ChatBoxAddedMessage(message, color);
			}

		}

		internal class ChatCommands_AddConsoleMessagePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(Type.GetType("ChatCommands.ClassReplacements.CommandChatBox, ChatCommands"), "AddConsoleMessage", new Type[] { typeof(string), typeof(Color) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(ChatCommands_AddConsoleMessagePatch.AddConsoleMessage_Postfix));

			private static IReflectionHelper Reflection;

			public ChatCommands_AddConsoleMessagePatch(IReflectionHelper reflection) {
				Reflection = reflection;
			}

			private static void AddConsoleMessage_Postfix(ChatBox __instance, ref string message, ref Color color) {
				__instance.ChatBoxAddedMessage(message, color);
			}

		}

	}

}
