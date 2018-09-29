using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes.Framework.Constants {

	internal static class ModNetwork {

		internal static byte MessageTypeID = 50;

		internal enum MessageAction {
			None,
			EmoteBroadcast,
			CharacterEmoteBroadcast
		};
	}

}
