/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeModMessageReceivedEvent : ModMessageReceivedEvent
	{
		/// <inheritdoc />
		public override void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID != ModEntry.UniqueID) return;

			var key = e.ReadAs<int>();
			if (!ModEntry.ActivePeerSuperModes.ContainsKey(key))
				ModEntry.ActivePeerSuperModes[key] = new();

			switch (e.Type)
			{
				case "SuperModeActivated":
					ModEntry.ActivePeerSuperModes[key].Add(e.FromPlayerID);
					var glowingColor = Util.Professions.NameOf(key) switch
					{
						"Brute" => Color.OrangeRed,
						"Poacher" => Color.MediumPurple,
						"Desperado" => Color.DarkGoldenrod,
						"Piper" => Color.LimeGreen,
						_ => Color.White
					};
					Game1.getFarmer(e.FromPlayerID).startGlowing(glowingColor, false, 0.05f);
					break;

				case "SuperModeDeactivated":
					ModEntry.ActivePeerSuperModes[key].Remove(e.FromPlayerID);
					Game1.getFarmer(e.FromPlayerID).stopGlowing();
					break;
			}
		}
	}
}