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

namespace Tocseoj.Stardew.BigCropBonus;

public sealed class ModConfig
{
	public bool EnableMod { get; set; } = true;
	public float SellModifier { get; set; } = 0.10f;
	public float GiftModifier { get; set; } = 0.10f;
	public float EatModifier { get; set; } = 0.10f;
	public float GrowChance { get; set; } = 0.01f;
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
		configMenu.AddNumberOption(
			mod: ModManifest,
			name: () => "Selling: Percent (%) Increase",
			tooltip: () => "Increase in sell value of items related to your giant crops.",
			getValue: () => (float)Math.Truncate(Config.SellModifier * 100),
			setValue: value => Config.SellModifier = (float)Math.Round(value / 100, 2)
		);
		configMenu.AddNumberOption(
			mod: ModManifest,
			name: () => "Gifting: Percent (%) Increase",
			tooltip: () => "Increase in friendship gained when gifting items related to your giant crops.",
			getValue: () => (float)Math.Truncate(Config.GiftModifier * 100),
			setValue: value => Config.GiftModifier = (float)Math.Round(value / 100, 2)
		);
		configMenu.AddNumberOption(
			mod: ModManifest,
			name: () => "Eating: Percent (%) Increase",
			tooltip: () => "Increase in health and stamina gained when eating items related to your giant crops.",
			getValue: () => (float)Math.Truncate(Config.EatModifier * 100),
			setValue: value => Config.EatModifier = (float)Math.Round(value / 100, 2)
		);
		configMenu.AddNumberOption(
			mod: ModManifest,
			name: () => "Cheat: Giant Crop grow percent (%) chance.",
			tooltip: () => "Default is 1% per night when you have a 3x3 of the crop.",
			getValue: () => (float)Math.Truncate(Config.GrowChance * 100),
			setValue: value => Config.GrowChance = (float)Math.Round(value / 100, 2)
		);
	}
}