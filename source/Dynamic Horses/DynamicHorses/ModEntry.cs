using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Characters;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using StardewValley.Menus;

namespace DynamicHorses
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public Dictionary<string, string> Horses;
		private HorseHelper hh;
		public string RelativePath;

		/*********
		** Public methods
		*********/
		/// <summary>Initialise the mod.</summary>
		/// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
		public override void Entry(IModHelper helper)
		{
			ModConfig config = helper.ReadConfig<ModConfig>();
			
			Horses = config.Horses.ToDictionary(x => x.Name, x => x.XNBName, StringComparer.CurrentCultureIgnoreCase);
			hh = new HorseHelper(Horses, this);
			this.Monitor.Log("Horses loaded:");
			foreach (var x in config.Horses)
			{
				this.Monitor.Log(x.ToString());
			}

			SaveEvents.AfterLoad += FileLoaded;
			MenuEvents.MenuClosed += MenuClosed;
			string[] a = { "Stardew Valley" };
			string b = helper.DirectoryPath.Split(a, StringSplitOptions.RemoveEmptyEntries)[1];
			RelativePath = Path.Combine("SDV", b).Replace("SDV","..");
		}

		

		/// <summary>
		/// Listens for the closing of menus for when a horse is named, is disabled if a game is loaded and a horse is named and present.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void MenuClosed(object sender, EventArgsClickableMenuClosed e)
		{
			if(e.PriorMenu.GetType().Name.Equals("NamingMenu"))
			{
				hh.FindTheHorse();
			}

		}

		/// <summary>
		/// Listens for Save File load and will then 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void FileLoaded(object sender, EventArgs e)
		{
			hh.FindTheHorse();
		}

	}
}