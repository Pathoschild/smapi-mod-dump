/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CommunityKitchen
{
    public class ModEntry : Mod
	{
		internal static ModEntry Instance;
		internal static ITranslationHelper i18n => Instance.Helper.Translation;
		internal static CustomCommunityCentre.API.ICustomCommunityCentreAPI ICCCAPI;

		private const string CommandPrefix = "bb.cck.";


		public override void Entry(IModHelper helper)
		{
			ModEntry.Instance = this;

			this.RegisterEvents();
			Kitchen.AddConsoleCommands(cmd: ModEntry.CommandPrefix);
			GusDeliveryService.AddConsoleCommands(cmd: ModEntry.CommandPrefix);

			AssetManager assetManager = new();
			helper.Content.AssetLoaders.Add(assetManager);
			helper.Content.AssetEditors.Add(assetManager);
		}

		private void RegisterEvents()
		{
            this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
			this.Helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
			this.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;

			Kitchen.RegisterEvents();
			GusDeliveryService.RegisterEvents();
		}

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
			ModEntry.ICCCAPI = Helper.ModRegistry.GetApi
				<CustomCommunityCentre.API.ICustomCommunityCentreAPI>
				(CustomCommunityCentre.ModEntry.Instance.Helper.ModRegistry.ModID);
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			this.SaveLoadedBehaviours();
		}

		private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
		{
			if (CustomCommunityCentre.ModEntry.IsNewGame && !CustomCommunityCentre.ModEntry.Instance.IsSaveLoaded)
			{
				// Perform OnSaveLoaded behaviours when starting a new game
				this.SaveLoadedBehaviours();
			}

			this.DayStartedBehaviours();
		}

		private void SaveLoadedBehaviours()
		{
			Kitchen.SaveLoadedBehaviours();
			GusDeliveryService.SaveLoadedBehaviours();
		}

		private void DayStartedBehaviours()
		{
			Kitchen.DayStartedBehaviours();
			GusDeliveryService.DayStartedBehaviours();
		}
	}
}
