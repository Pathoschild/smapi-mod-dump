/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/


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
