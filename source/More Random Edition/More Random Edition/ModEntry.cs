/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Randomizer
{
    /// <summary>The mod entry point</summary>
    public class ModEntry : Mod
	{
		private AssetLoader _modAssetLoader;
		private AssetEditor _modAssetEditor;
		static IGenericModConfigMenuApi api;

		/// <summary>The mod entry point, called after the mod is first loaded</summary>
		/// <param name="helper">Provides simplified APIs for writing mods</param>
		public override void Entry(IModHelper helper)
		{
			Globals.ModRef = this;
			Globals.Config = helper.ReadConfig<ModConfig>();

            _modAssetLoader = new AssetLoader(this);
            _modAssetEditor = new AssetEditor(this);

            helper.Events.Content.AssetRequested += OnAssetRequested;

			PreLoadReplacments();
			helper.Events.GameLoop.GameLaunched += (sender, args) => _modAssetLoader.ReplaceTitleScreenAssets();
			helper.Events.GameLoop.GameLaunched += (sender, args) => TryLoadModConfigMenu();
			helper.Events.GameLoop.GameLaunched += (sender, args) => MusicRandomizer.PatchChangeMusicTrack();
			helper.Events.Content.LocaleChanged += (sender, args) => _modAssetLoader.ReplaceTitleScreenAssets();
			helper.Events.GameLoop.ReturnedToTitle += (sender, args) => _modAssetEditor.ResetValuesAndInvalidateCache();
			helper.Events.GameLoop.ReturnedToTitle += (sender, args) => _modAssetLoader.ReplaceTitleScreenAssets();
            helper.Events.GameLoop.SaveLoaded += (sender, args) => CalculateAllReplacements();
			helper.Events.Display.MenuChanged += MenuAdjustments.AdjustMenus;
			helper.Events.GameLoop.DayStarted += (sender, args) => _modAssetEditor.CalculateAndInvalidateShopEdits();

            if (Globals.Config.RandomizeRain)
			{
				helper.Events.GameLoop.DayStarted += (sender, args) => _modAssetLoader.ReplaceRain();
			}

            if (Globals.Config.Animals.CritterHueShiftMax > 0)
            {
                helper.Events.GameLoop.DayStarted += (sender, args) => _modAssetLoader.ReplaceCritters();
            }

            if (Globals.Config.RandomizeForagables)
			{
				helper.Events.GameLoop.GameLaunched += (sender, args) => WildSeedAdjustments.ReplaceGetRandomWildCropForSeason();
			}

			if (Globals.Config.Bundles.Randomize && Globals.Config.Bundles.ShowDescriptionsInBundleTooltips)
			{
				helper.Events.Display.RenderedActiveMenu += (sender, args) => BundleMenuAdjustments.AddDescriptionsToBundleTooltips();
			}
		}

		/// <summary>
		/// When an asset is requested, attempt to replace it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            _modAssetLoader.OnAssetRequested(sender, e);
            _modAssetEditor.OnAssetRequested(sender, e);
        }

		/// <summary>
		/// Set up the mod config menu
		/// Exits early if it is not installed
		/// </summary>
        private void TryLoadModConfigMenu()
		{
			if (!Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
			{
				Globals.ConsoleTrace("GenericModConfigMenu not present");
				return;
			}

			api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			api.Register(ModManifest, () => Globals.Config = new ModConfig(), () => Helper.WriteConfig(Globals.Config));

			ModConfigMenuHelper menuHelper = new(api, ModManifest);
			menuHelper.RegisterModOptions();

		}

		/// <summary>
		/// Loads the replacements that can be loaded before a game is selected
		/// </summary>
		public void PreLoadReplacments()
		{
			_modAssetEditor.CalculateEditsBeforeLoad();
		}

		/// <summary>
		/// Does all the randomizer replacements that take place after a game is loaded
		/// </summary>
		public void CalculateAllReplacements()
		{
			// Seed is pulled from farm name
			byte[] seedvar = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value));
			int seed = BitConverter.ToInt32(seedvar, 0);

			Monitor.Log($"Seed Set: {seed}");

			Globals.SpoilerLog = new SpoilerLogger(Game1.player.farmName.Value);

            // Make replacements and edits
			_modAssetEditor.CalculateEdits();
			_modAssetLoader.RandomizeImages();

			// Invalidate all replaced and edited assets so they are reloaded
			_modAssetLoader.InvalidateCache();
			_modAssetEditor.InvalidateCache();

			// Ensure that the bundles get changed if they're meant to
			Game1.GenerateBundles(Game1.bundleType, true);

            WorldAdjustments.ChangeDayOneForagables();

            Globals.SpoilerLog.WriteFile();
        }

		/// <summary>
		/// A passthrough to calculate amd invalidate UI edits
		/// Used when the lanauage is changed
		/// </summary>
		public void CalculateAndInvalidateUIEdits()
		{
			_modAssetEditor.CalculateAndInvalidateUIEdits();
		}

		/// <summary>
		/// For testing purposes - not normally called
		/// </summary>
		public void CalculateAndInvalidateShopEdits()
		{
			_modAssetEditor.CalculateAndInvalidateShopEdits();

        }
	}
}