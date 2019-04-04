
namespace MultiplayerEmotes.Framework.Network {

	public enum CharacterType {
		Unknown = -1,
		Farmer,
		NPC,
		FarmAnimal
	}

	public class EmoteMessage {

		public int EmoteIndex { get; set; }
		public string EmoteSourceId { get; set; }
		public CharacterType EmoteSourceType { get; set; }

	}

}
