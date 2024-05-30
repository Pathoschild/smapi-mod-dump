/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tocseoj/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using GenericModConfigMenu;

namespace Tocseoj.Stardew.StardewShuffle;

public sealed class ModConfig
{
	public bool EnableMod { get; set; } = true;
	public bool SendArcadeInMail { get; set; } = true;
}
internal class ConfigMenu(IMonitor Monitor, IManifest ModManifest, IModHelper Helper, ModConfig Config)
	: ModComponent(Monitor, ModManifest, Helper, Config)
{
	public void Setup()
	{
		// get Generic Mod Config Menu's API (if it's installed)
		var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
		if (configMenu is null)
			return;

		// register mod
		configMenu.Register(
			mod: ModManifest,
			reset: () => Config = new ModConfig(),
			save: () => Helper.WriteConfig(Config)
		);

		// add some config options
		configMenu.AddBoolOption(
			mod: ModManifest,
			name: () => "Send arcade game in mail",
			tooltip: () => "If enabled, the arcade game will be sent to you in the mail.",
			getValue: () => Config.SendArcadeInMail,
			setValue: value => Config.SendArcadeInMail = value
		);
	}
}