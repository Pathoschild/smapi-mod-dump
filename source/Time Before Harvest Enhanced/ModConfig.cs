/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/enom/time-before-harvest-enhanced
**
*************************************************/

using StardewModdingAPI;

internal class ModConfig
{
	public SButton TriggerButton { get; set; }

	public ModConfig()
	{
		TriggerButton = SButton.F2;
	}
}
