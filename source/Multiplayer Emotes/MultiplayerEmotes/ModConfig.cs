
namespace MultiplayerEmotes {

	public class ModConfig {

		/// <summary>
		/// Animate icon within the emote button.
		/// </summary>
		public bool AnimateEmoteButtonIcon { get; set; } = true;

		/// <summary>
		/// Show or hide tooltip on hover.
		/// </summary>
		public bool ShowTooltipOnHover { get; set; } = true;

		/// <summary>
		/// Allow other players to use the command 'emote_npc'.
		/// This command allows to force to a NPC to play an emote.
		/// </summary>
		public bool AllowNonHostEmoteNpcCommand { get; set; } = false;

		/// <summary>
		/// Allow other players to use the command 'emote_animal'.
		/// This command allows to force to a FarmAnimal to play an emote.
		/// </summary>
		public bool AllowNonHostEmoteAnimalCommand { get; set; } = false;

		///// <summary>
		///// This option allows to other players that do not have this mod to see the played emote.
		///// Beware: They will see the emote in the last place it was made. The emote will display above the head but wont follow the player.
		///// </summary>
		//public bool NonInstalledModPlayersWorkarround { get; set; } = false;

	}

}
