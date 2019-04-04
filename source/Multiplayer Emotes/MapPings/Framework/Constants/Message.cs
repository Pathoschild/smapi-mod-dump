
namespace MapPings.Framework.Constants {

	internal static class Message {

		internal const byte TypeID = 50;

		internal enum Action {
			None,
			SendMapPing,
			BroadcastMapPing
		};

	}

}
