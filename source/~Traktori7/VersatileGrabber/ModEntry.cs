/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace VersatileGrabber
{
	public class ModEntry : Mod
	{
		public const string versatileGrabberName = "Versatile Grabber";

		private const string jsonAssetsUniqueID = "spacechase0.JsonAssets";
		private bool jsonAssetsFound = false;

		private IJsonAssetsApi jsonAssetsApi;

		internal Config config;
		internal ITranslationHelper i18n => Helper.Translation;

		internal static int GrabberID { get; private set; } = -1;

		internal static IMonitor ModMonitor;
		internal static IModHelper ModHelper;


		/******************/
		/* Public methods */
		/******************/

		public override void Entry(IModHelper helper)
		{
			ModMonitor = Monitor;
			ModHelper = helper;

			//string startingMessage = i18n.Get("template.start", new { mod = helper.ModRegistry.ModID, folder = helper.DirectoryPath });
			//Monitor.Log(startingMessage, LogLevel.Trace);

			config = helper.ReadConfig<Config>();

			helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
			helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
			helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
			helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
			
		}

		





		/*******************/
		/* Private methods */
		/*******************/

		private void LoadApis()
		{
			jsonAssetsApi = Helper.ModRegistry.GetApi<IJsonAssetsApi>(jsonAssetsUniqueID);
			if (jsonAssetsApi != null)
			{
				jsonAssetsFound = true;
			}
		}


		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			LoadApis();

			if (!jsonAssetsFound)
			{
				ModMonitor.Log("JsonAssets not found.", LogLevel.Error);
				return;
			}

			jsonAssetsApi.LoadAssets(Path.Combine(ModHelper.DirectoryPath, "assets", "VersatileGrabber"));
			jsonAssetsApi.IdsAssigned += OnIdsAssigned;
		}


		private void OnIdsAssigned(object sender, EventArgs e)
		{
			GrabberID = jsonAssetsApi.GetBigCraftableId(versatileGrabberName);
			ModMonitor.Log($"Versatile Grabber loaded with ID {GrabberID}", LogLevel.Debug);
		}


		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			

			// Create one grabber
			/*VersatileGrabber grabber = new VersatileGrabber();
			grabber.ParentSheetIndex = GrabberID;

			Game1.getFarm().dropObject(grabber);*/
		}


		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			
		}


		private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
		{
			foreach (var keyValuePair in e.Added)
			{
				if (GrabberController.ItemShouldBeVersatileGrabber(keyValuePair.Value))
				{
					// Converts the placed object to a versatile grabber
					VersatileGrabber grabber = new VersatileGrabber(keyValuePair.Value);
					e.Location.objects[keyValuePair.Key] = grabber;
				}
			}
		}



		private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
		{
			foreach (var addedItem in e.Added)
			{
				// This check seems to be false for picked up grabbers...
				if (!(addedItem is VersatileGrabber grabber))
					continue;

				// At this point it should be of type Versatile Grabber
				ModMonitor.Log("Versatile grabber found in inventory, converting to SObject", LogLevel.Debug);

				int index = Game1.player.Items.IndexOf(addedItem);
				Game1.player.Items[index] = grabber.ToObject();
			}
			
		}
	}
}
