using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace StackToNearbyChests
{
	/// <summary>The mod entry class loaded by SMAPI.</summary>
	class ModEntry : Mod
	{
		internal static ModConfig Config { get; private set; }

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Config = helper.ReadConfig<ModConfig>();
			ButtonHolder.ButtonIcon = helper.Content.Load<Texture2D>(@"icon.png");

			helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
		}

		/// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			Patch.PatchAll(HarmonyInstance.Create("me.ilyaki.StackToNearbyChests"));
		}
	}
}
