
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using CustomEmojis.Framework.Events;
using CustomEmojis.Framework.Extensions;
using CustomEmojis.Framework.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace CustomEmojis.Framework.Menu {

	public class CachedMessageEmojis : IClickableMenu {

		private readonly IReflectionHelper Reflection;
		private readonly IModEvents Events;

		private readonly IReflectedField<List<ChatMessage>> messagesField;
		private readonly IReflectedField<int> cheatHistoryPositionField;
		private readonly IReflectedField<bool> choosingEmojiField;
		private readonly IReflectedField<ClickableTextureComponent> emojiMenuIconField;

		private readonly IReflectedMethod formatMessageMethod;
		private readonly IReflectedMethod messageColorMethod;

		private List<PlayerMessage> PlayerMessageList;

		public int NumberVanillaEmoji { get; private set; }

		// ChatCommands support
		private readonly IReflectedMethod getEndDisplayIndexMethod;
		private readonly IReflectedField<int> displayLineIndexField;
		private readonly bool chatCommandsIsLoaded;

		public CachedMessageEmojis(IModHelper helper, int numberVanillaEmojis) {

			Reflection = helper.Reflection;
			Events = helper.Events;

			messagesField = Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages");
			cheatHistoryPositionField = Reflection.GetField<int>(Game1.chatBox, "cheatHistoryPosition");
			choosingEmojiField = Reflection.GetField<bool>(Game1.chatBox, "choosingEmoji");
			emojiMenuIconField = Reflection.GetField<ClickableTextureComponent>(Game1.chatBox, "emojiMenuIcon");

			formatMessageMethod = Reflection.GetMethod(Game1.chatBox, "formatMessage");
			messageColorMethod = Reflection.GetMethod(Game1.chatBox, "messageColor");

			PlayerMessageList = new List<PlayerMessage>();

			NumberVanillaEmoji = numberVanillaEmojis;

			chatCommandsIsLoaded = helper.ModRegistry.IsLoaded("cat.chatcommands");
			if(chatCommandsIsLoaded) {
				displayLineIndexField = Reflection.GetField<int>(Game1.chatBox, "displayLineIndex");
				getEndDisplayIndexMethod = Reflection.GetMethod(Game1.chatBox, "GetEndDisplayIndex");
			}

			SubscribeEvents();

		}

		private void SubscribeEvents() {
			Events.Display.MenuChanged += OnMenuChanged;
			ChatBoxExtension.OnChatBoxAddedMessage += AddPlayerChatMessage;
			ChatBoxExtension.OnChatBoxReceivedMessage += AddPlayerChatMessage;
		}

		private void UnsubscribeEvents() {
			Events.Display.MenuChanged -= OnMenuChanged;
			ChatBoxExtension.OnChatBoxAddedMessage -= AddPlayerChatMessage;
			ChatBoxExtension.OnChatBoxReceivedMessage -= AddPlayerChatMessage;
		}

		/// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
			if(e.NewMenu is TitleMenu) {
				UnsubscribeEvents();
				MemoryCache.Default.Dispose();
			}
		}

		private void AddPlayerChatMessage(object sender, ChatMessageEventArgs e) {

			int messageHash = messagesField.GetValue().Last().GetHashCode();

			RemoveExcedentMessages(messageHash);

			string message = formatMessageMethod.Invoke<string>(e.SourcePlayer.UniqueMultiplayerID, e.ChatKind, e.Message);

			int witdh = Game1.chatBox.chatBox.Width;
			if(e.ChatKind == Constants.ChatMessageKind.Normal) {
				witdh -= 8;
			}

			message = Game1.parseText(message, Game1.chatBox.chatBox.Font, witdh - 8);

			PlayerMessageList.Add(new PlayerMessage(e.SourcePlayer, e.Language, message, messageHash));

			foreach(ChatSnippet item in messagesField.GetValue().Last().message) {
				if(item.emojiIndex >= NumberVanillaEmoji) {
					item.emojiIndex = -1;
				}
			}

		}

		private void RemoveExcedentMessages(int messageHash) {

			if(PlayerMessageList.Count >= messagesField.GetValue().Count) {

				if(PlayerMessageList[0].MessageHash != messageHash) {

					ObjectCache cache = MemoryCache.Default;
#if DEBUG
					ModEntry.ModLogger.LogToMonitor = false;
					ModEntry.ModLogger.Log("Removed from cache:", $"{PlayerMessageList[0].MessageHash}_playerMessage");
					ModEntry.ModLogger.LogToMonitor = true;
#endif
					cache.Remove($"{PlayerMessageList[0].MessageHash}_playerMessage");

#if DEBUG
					ModEntry.ModLogger.LogToMonitor = false;
					ModEntry.ModLogger.Log("Removed from message list:", $"{PlayerMessageList[0].Message}");
					ModEntry.ModLogger.LogToMonitor = true;
#endif
					PlayerMessageList.RemoveAt(0);

					// Because of commands like 'clear', could be more
					RemoveExcedentMessages(messagesField.GetValue().Last().GetHashCode());

				}

			}

		}

		public void DrawMessages(SpriteBatch b) {

			if(Game1.chatBox != null) {

				ObjectCache cache = MemoryCache.Default;
				long uniqueMultiplayerID = Game1.player.UniqueMultiplayerID;

				int startIndex = chatCommandsIsLoaded ? displayLineIndexField.GetValue() : messagesField.GetValue().Count - 1;
				int endIndex = chatCommandsIsLoaded ? getEndDisplayIndexMethod.Invoke<int>() : 0;

				int verticalPosAcum = 0;
				for(int i = startIndex; i >= endIndex; --i) {

					ChatMessage message = messagesField.GetValue()[i];
					verticalPosAcum += message.verticalSize;

					int messageHash = message.GetHashCode();
					if(!(cache[$"{messageHash}_message"] is PlayerMessage cachedPlayerMessage)) {

#if DEBUG
						ModEntry.ModLogger.LogToMonitor = false;
						ModEntry.ModLogger.Log($"Message hash: {messageHash}");
						ModEntry.ModLogger.LogToMonitor = true;
#endif

						cachedPlayerMessage = PlayerMessageList.Find(msg => msg.MessageHash == messageHash);
						if(cachedPlayerMessage == null) {
							string msg = string.Concat(message.message.Select(chatSnippet => chatSnippet.message));
							cachedPlayerMessage = new PlayerMessage(Game1.player, LocalizedContentManager.CurrentLanguageCode, msg, messageHash);
							PlayerMessageList.Add(cachedPlayerMessage);
						}

						// Cache message for later
						cache[$"{messageHash}_message"] = cachedPlayerMessage;

#if DEBUG
						ModEntry.ModLogger.LogToMonitor = false;
						ModEntry.ModLogger.Log($"vvvvvv Cached {cache.GetCount()} elements vvvvvv");
						foreach(var item in cache) {
							ModEntry.ModLogger.Log($"Message hash: {item}");
						}
						ModEntry.ModLogger.Log($"^^^^^^ Cached {cache.GetCount()} elements ^^^^^^");
						ModEntry.ModLogger.LogToMonitor = true;
#endif

					}

					int x = 12;
					int y = Game1.chatBox.yPositionOnScreen - verticalPosAcum - 8 + (Game1.chatBox.chatBox.Selected ? 0 : Game1.chatBox.chatBox.Height);

					foreach(MessageEmoji msgEmoji in cachedPlayerMessage.MessageEmojis) {


						Vector2 position = new Vector2(msgEmoji.HorizontalPosition + x + 1.0f, msgEmoji.VerticalPosition + y - 4.0f);
						bool playerLeft = Game1.getAllFarmers().Any(farmer => farmer.UniqueMultiplayerID == cachedPlayerMessage.Player.UniqueMultiplayerID);

						if(playerLeft && Game1.chatBox.isWithinBounds((int)position.X, (int)position.Y) && msgEmoji.Index >= NumberVanillaEmoji) {
							msgEmoji.DrawEmoji(b, position, message.alpha);
						}

					}

				}

				// Draw the emoji menu
				if(choosingEmojiField.GetValue()) {
					Game1.chatBox.emojiMenu.draw(b);
				}

				// Draw the cursor on top
				if((Game1.chatBox.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()) || Game1.chatBox.isActive()) && !Game1.options.hardwareCursor) {
					b.Draw(Game1.mouseCursors, new Vector2((float)Game1.getOldMouseX(), (float)Game1.getOldMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float)(4.0 + Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
				}

			}

		}

	}

}
