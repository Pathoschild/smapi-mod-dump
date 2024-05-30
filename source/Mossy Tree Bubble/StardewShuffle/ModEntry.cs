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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Minigames;

namespace Tocseoj.Stardew.StardewShuffle;
internal class ModEntry : Mod
{
	private ModConfig Config = null!;
	private ShuffleMinigame ShuffleMinigame = null!;
	public override void Entry(IModHelper helper)
	{
		Config = helper.ReadConfig<ModConfig>();
		if (Config.EnableMod == false) {
			// TODO: this is a bad way to handle this, as it will turn an existing arcade into an error item
			Monitor.Log("Mod is disabled. Arcade machine will be an error item and despawn.", LogLevel.Debug);
			return;
		}
    ConfigMenu menu = new(Monitor, ModManifest, helper, Config);
		helper.Events.GameLoop.GameLaunched += (sender, e) => menu.Setup();
		helper.Events.Input.ButtonPressed += OnButtonPressed;

		ShuffleMinigame = new(Monitor, ModManifest, helper, Config);
		ShuffleMinigame.AddConsoleCommand();

		helper.Events.Content.AssetRequested += OnAssetRequested;
	}
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
	{
		Monitor.Log($"Asset requested: {e.NameWithoutLocale}");
		// TODO: Add custom assets here
	}

	private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
	{
		if (!Context.IsWorldReady)
			return;

		if (e.Button == SButton.G)
			ShuffleMinigame.Start();
		else if (e.Button == SButton.J)
			Game1.currentMinigame = new CalicoJack();
	}
}
