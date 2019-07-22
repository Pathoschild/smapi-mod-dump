using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *  Testing.
 *  Trying to make it so clients of the save game can warp by using chat commands.
 */

namespace WarpToFriends
{

	public class NewChatMessageEvent : EventArgs
	{
		public string farmer;
		public string message;

		public NewChatMessageEvent(string msg)
		{

			List<string> msgA = msg.Split(' ').ToList();
			farmer = msgA[0].Substring(0, msgA[0].Length - 1);
			msgA.RemoveAt(0);
			message = string.Join(" ", msgA.ToArray());

		}
	}

	public class ModChatBox : ChatBox
	{
		public string lastCommand = "";

		protected override void runCommand(string command)
		{
			base.runCommand(command);
			lastCommand = command;
		}
	}

	public class ChatReader
	{

		private IModHelper Helper;
		private ChatMessage _lastMessage;
		private List<ChatMessage> _chatMessages;
		public static EventHandler<NewChatMessageEvent> NewChatMessageEvent;

		public ChatReader(IModHelper helper)
		{
			Helper = helper;

		}

		public void OnSaveLoaded()
		{
			Helper.Events.GameLoop.UpdateTicked += InitializeChatList;
			Helper.Events.GameLoop.UpdateTicked += CheckChat;
		}

		public void OnReturnToTitle()
		{
			Helper.Events.GameLoop.UpdateTicked -= InitializeChatList;
			Helper.Events.GameLoop.UpdateTicked -= CheckChat;
		}

		private void InitializeChatList(object sender, EventArgs e)
		{

			_chatMessages = Helper.Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();

			if (_chatMessages.Count > 0)
			{
				_lastMessage = _chatMessages.Last();
			}
			else
			{
				_lastMessage = new ChatMessage();
			}
		}

		private void CheckChat(object sender, EventArgs e)
		{
			if (!Context.IsWorldReady) return;

			if (_chatMessages.Count <= 0) return;
			if (_lastMessage != _chatMessages.Last())
			{
				_lastMessage = _chatMessages.Last();
				string msg = ChatMessage.makeMessagePlaintext(_lastMessage.message);

				NewChatMessageEvent(null, new NewChatMessageEvent(msg));

			}
		}
	}
}
