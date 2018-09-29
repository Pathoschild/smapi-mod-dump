
using Newtonsoft.Json;

public class ModConfig {

	/// <summary>
	/// Animate icon within the emote button 
	/// </summary>
	public bool AnimateEmoteButtonIcon { get; set; } = true;

	/// <summary>
	/// Show or hide tooltip on hover
	/// </summary>
	public bool ShowTooltipOnHover { get; set; } = true;

	///// <summary>
	///// This option allows to other players that do not have this mod to see the played emote.
	///// Beware: They will see the emote in the last place it was made. The emote will display above the head but wont follow the player.
	///// </summary>
	//public bool NonInstalledModPlayersWorkarround { get; set; } = false;

}
