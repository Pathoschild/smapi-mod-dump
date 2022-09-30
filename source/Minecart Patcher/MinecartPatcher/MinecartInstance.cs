/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bwdy/SDVModding
**
*************************************************/

namespace MinecartPatcher
{
	public class MinecartInstance
	{
		/// <summary>
		/// The internal map Name, used to locate warp target.
		/// </summary>
		public string LocationName { get; set; } = "???";

		/// <summary>
		/// The display name that will appear in the destination menu.
		/// </summary>
		public string DisplayName { get; set; } = null;

		/// <summary>
		/// The X location the player will land at. Needs to be within 5 tiles of the cart (so that it can be identified as an origin).
		/// </summary>
		public int LandingPointX { get; set; } = 0;

		/// <summary>
		/// The Y location the player will land at. Needs to be within 5 tiles of the cart (so that it can be identified as an origin).
		/// </summary>
		public int LandingPointY { get; set; } = 0;

		/// <summary>
		/// The direction the player will face when they arrive here.
		/// </summary>
		public int LandingPointDirection { get; set; } = 2;

		/// <summary>
		/// This option mimics the vanilla caves behavior, which silences music if it's playing springtown.
		/// </summary>
		public bool IsUnderground { get; set; } = false;

		/// <summary>
		/// This is optional - if not null, it should contain a string - the option to choose this minecart will only fire if the
		/// named mail flag has been set.
		/// </summary>
		public string MailCondition { get; set; } = null;

		/// <summary>
		/// This is really intended for internal use - it contains a vanilla dialogue response that triggers a vanilla warp handler.
		/// Might be useful if the game has updated or someone has extended the vanilla minecart stuff.
		/// </summary>
		public string VanillaPassthrough { get; set; } = null;

		/// <summary>
		/// If ommitted, a minecart will connect to the vanilla minecart network. You can specify a network id here, which will
		/// cause the minecart to only connect to other minecarts with this network id.
		/// </summary>
		public string NetworkId { get; set; } = null;
	}
}
