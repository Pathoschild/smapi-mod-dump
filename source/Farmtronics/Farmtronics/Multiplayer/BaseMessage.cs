/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoeStrout/Farmtronics
**
*************************************************/

namespace Farmtronics.Multiplayer {
	abstract class BaseMessage<T> where T : BaseMessage<T> {
		public abstract void Apply();

		public void Send(long[] playerIDs = null) {
			MultiplayerManager.SendMessage(this as T, playerIDs);
		}
		
		public void SendToHost() {
			MultiplayerManager.SendMessageToHost(this as T);
		}
	}
}