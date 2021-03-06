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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ShowBirthdays
{
	class ModEntry : Mod
	{
		private CycleType cycleType;
		// Flag for if the calendar is open
		private bool calendarOpen = false;

		// Interval in ticks for changing the birthday sprite
		private int spriteCycleTicks = 120;
		private int currentCycle = 0;

		private int clickedDay = -1;
		private Vector2 cursorPos = new Vector2();

		private BirthdayHelper bdHelper;
		private ModConfig config;

		private Texture2D iconTexture;
		private readonly string iconPath = "assets/Icon.png";


		public override void Entry(IModHelper helper)
		{
			helper.Events.Display.MenuChanged += OnMenuChanged;
			helper.Events.Display.RenderingActiveMenu += OnRenderingActiveMenu;
			helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
			helper.Events.GameLoop.GameLaunched += OnLaunched;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
			helper.Events.Input.ButtonPressed += OnButtonPressed;
			helper.Events.Input.CursorMoved += OnCursorMoved;
		}


		private void OnLaunched(object sender, GameLaunchedEventArgs e)
		{
			// Load the config for generic mod config menu
			config = Helper.ReadConfig<ModConfig>();
			var api = Helper.ModRegistry.GetApi<GenericModConfigAPI>("spacechase0.GenericModConfigMenu");

			// Register options for GMCM
			if (api != null)
			{
				api.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));

				api.RegisterClampedOption(ModManifest, Helper.Translation.Get("duration-label"), Helper.Translation.Get("duration-desc"), () => config.cycleDuration, (int val) => config.cycleDuration = val, 1, 600);
				api.RegisterChoiceOption(ModManifest, Helper.Translation.Get("type-label"), Helper.Translation.Get("type-desc"), () => config.cycleType, (string val) => config.cycleType = val, ModConfig.cycleTypes);
				api.RegisterSimpleOption(ModManifest, Helper.Translation.Get("icon-label"), Helper.Translation.Get("icon-desc"), () => config.showIcon, (bool val) => config.showIcon = val);
			}

			// Load the icon from the mod folder
			iconTexture = Helper.Content.Load<Texture2D>(iconPath, ContentSource.ModFolder);

			// Check if the loading succeeded
			if (iconTexture == null)
			{
				Monitor.Log("Failed loading " + iconPath, LogLevel.Error);
			}
		}


		private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			// Unsubscribe from the events to reduce unnecessary event checks
			Helper.Events.Input.ButtonPressed -= OnButtonPressed;
			Helper.Events.Input.CursorMoved -= OnCursorMoved;
		}


		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// Initialize the helper
			bdHelper = new BirthdayHelper(Monitor);
			// Refresh the config
			config = Helper.ReadConfig<ModConfig>();

			// Subscribe to the correct events to prevent unnecessary event checks
			switch (cycleType)
			{
				case CycleType.Always:
					break;
				case CycleType.Hover:
					Helper.Events.Input.CursorMoved += OnCursorMoved;
					break;
				case CycleType.Click:
					Helper.Events.Input.ButtonPressed += OnButtonPressed;
					break;
				default:
					break;
			}

			// Read the config options and fix the values if needed
			switch (config.cycleType)
			{
				case "Always":
					cycleType = CycleType.Always;
					break;
				case "Hover":
					cycleType = CycleType.Hover;
					break;
				case "Click":
					cycleType = CycleType.Click;
					break;
				default:
					Monitor.Log("The only accepted cycle types are Always, Hover and Click. Defaulting to Always.", LogLevel.Error);
					cycleType = CycleType.Always;
					break;
			}

			spriteCycleTicks = config.cycleDuration;
			if (spriteCycleTicks < 1)
			{
				Monitor.Log("Cycle duration can't be less than 1", LogLevel.Error);
				spriteCycleTicks = 1;
			}

			foreach (NPC n in Utility.getAllCharacters())
			{
				// Checking for 0 should eliminate a lot of the non-friendable NPCs, needs verification
				if (n.isVillager() && n.Birthday_Day > 0)
				{
					bdHelper.AddBirthday(n.Birthday_Season, n.Birthday_Day, n);
				}
			}
		}


		/// <summary>
		/// Edits the calendar's hover texts to include extra birthdays.
		/// </summary>
		private void OnMenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.NewMenu == null || !(e.NewMenu is Billboard billboard))
			{
				calendarOpen = false;
				return;
			}

			// Check if we're looking the calendar or the quest part of the billboard
			bool dailyQuests = this.Helper.Reflection.GetField<bool>(billboard, "dailyQuestBoard").GetValue();

			// Do nothing if looking at the quest board
			if (dailyQuests)
				return;

			// We are looking at the calendar so update the flag for it
			calendarOpen = true;

			List<ClickableTextureComponent> days = billboard.calendarDays;

			// List of all the birthday days for the season
			List<int> list = bdHelper.GetDays(Game1.currentSeason);

			// NOTE: Remember that i goes from 1 to 28, so substract 1 from it to use as the index!
			for (int i = 1; i <= days.Count; i++)
			{
				// Build the hover text from festival, birthdays and wedding if applicable
				string newHoverText = "";

				// Add the festival text if needed
				if (Utility.isFestivalDay(i, Game1.currentSeason))
				{
					// Festival name hover text from base game
					newHoverText = Game1.temporaryContent.Load<Dictionary<string, string>>(string.Concat("Data\\Festivals\\" + Game1.currentSeason, i))["name"];
				}
				else if (Game1.currentSeason.Equals("winter") && i >= 15 && i <= 17)
				{
					// Night Market hover text from base game
					newHoverText = Game1.content.LoadString("Strings\\UI:Billboard_NightMarket");
				}

				// Get the list of all NPCs with the birthday and add them to the hover text
				List<NPC> listOfNPCs = bdHelper.GetNpcs(Game1.currentSeason, i);

				if (listOfNPCs != null)
				{
					for (int j = 0; j < listOfNPCs.Count; j++)
					{
						if (!newHoverText.Equals(""))
						{
							newHoverText += Environment.NewLine;
						}

						NPC n = listOfNPCs[j];
						// Build the hover text just like in the base game. I'm not touching that.
						newHoverText += ((n.displayName.Last() != 's' && (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.de || (n.displayName.Last() != 'x' && n.displayName.Last() != 'ß' && n.displayName.Last() != 'z'))) ? Game1.content.LoadString("Strings\\UI:Billboard_Birthday", n.displayName) : Game1.content.LoadString("Strings\\UI:Billboard_SBirthday", n.displayName));
					}
				}

				// Get a refrence to the list of weddings
				IReflectedField<Dictionary<ClickableTextureComponent, List<string>>> weddings = this.Helper.Reflection.GetField<Dictionary<ClickableTextureComponent, List<string>>>(billboard, "_upcomingWeddings");

				// Wedding text from base game
				if (weddings.GetValue().ContainsKey(days[i - 1]))
				{
					for (int j = 0; j < weddings.GetValue().Count / 2; j++)
					{
						if (!newHoverText.Equals(""))
						{
							newHoverText += Environment.NewLine;
						}

						newHoverText += Game1.content.LoadString("Strings\\UI:Calendar_Wedding", weddings.GetValue()[days[i - 1]][j * 2], weddings.GetValue()[days[i - 1]][j * 2 + 1]);
					}
				}

				days[i - 1].hoverText = newHoverText.Trim();


				// Add the NPC textures
				if (list.Contains(i))
				{
					days[i - 1].texture = bdHelper.GetSprite(Game1.currentSeason, i, false);
				}
			}
		}


		/// <summary>
		/// Changes the NPC sprites according to the selected cycle type
		/// </summary>
		private void OnRenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
		{
			if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is Billboard billboard))
				return;

			if (currentCycle < spriteCycleTicks)
			{
				currentCycle++;
			}

			// Get the calendarDays component, it will be null if we're looking at the questboard
			List<ClickableTextureComponent> days = billboard.calendarDays;

			if (days == null)
				return;

			List<int> listOfDays = bdHelper.GetDays(Game1.currentSeason, true);

			switch (cycleType)
			{
				case CycleType.Always:
					if (currentCycle == spriteCycleTicks)
					{
						for (int i = 0; i < listOfDays.Count; i++)
						{
							try
							{
								days[listOfDays[i] - 1].texture = bdHelper.GetSprite(Game1.currentSeason, listOfDays[i], true);
							}
							catch
							{
								// Generic error for now.
								Monitor.Log("There was a problem with parsing the birthday data", LogLevel.Error);
							}
						}

						// Reset the cycle
						currentCycle = 0;
					}
					break;
				case CycleType.Hover:
					if (currentCycle == spriteCycleTicks)
					{
						for (int i = 0; i < listOfDays.Count; i++)
						{
							if (days[listOfDays[i] - 1].containsPoint((int)cursorPos.X, (int)cursorPos.Y))
							{
								days[listOfDays[i] - 1].texture = bdHelper.GetSprite(Game1.currentSeason, listOfDays[i], true);
							}
						}

						// Reset the cycle
						currentCycle = 0;
					}
					break;
				case CycleType.Click:
					if (clickedDay != -1 && listOfDays.Contains(clickedDay))
					{
						days[clickedDay - 1].texture = bdHelper.GetSprite(Game1.currentSeason, clickedDay, true);
						clickedDay = -1;
					}
					break;
				default:
					Monitor.Log("Unknown cycle type encountered in OnRenderingActiveMenu", LogLevel.Error);
					break;
			}
		}


		/// <summary>
		/// Draws the icon for shared birthdays and redraws the cursor and hover text
		/// </summary>
		private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
		{
			if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is Billboard billboard) || !config.showIcon)
				return;

			// Get the calendarDays component, it will be null if we're looking at the questboard
			List<ClickableTextureComponent> days = billboard.calendarDays;

			if (days == null)
				return;

			// Get birthday days that are shared
			List<int> listOfDays = bdHelper.GetDays(Game1.currentSeason, true);

			int offsetX = iconTexture.Width * Game1.pixelZoom;
			int offsetY = iconTexture.Height * Game1.pixelZoom;

			for (int i = 0; i < listOfDays.Count; i++)
			{
				// Add the icon texture to the bottom right of the day
				Vector2 position = new Vector2(days[listOfDays[i] - 1].bounds.Right - offsetX, days[listOfDays[i] - 1].bounds.Bottom - offsetY);

				e.SpriteBatch.Draw(iconTexture, new Rectangle((int)position.X, (int)position.Y, offsetX, offsetY), Color.White);
			}

			// Redraw the cursor
			billboard.drawMouse(e.SpriteBatch);

			// The current hover text is stored in the billboard itself
			string text = Helper.Reflection.GetField<string>(billboard, "hoverText").GetValue();
			
			// Redraw the hover text
			if (text.Length > 0)
			{
				IClickableMenu.drawHoverText(e.SpriteBatch, text, Game1.dialogueFont);
			}
		}


		/// <summary>
		/// Handles changing the sprite if the day as clicked
		/// </summary>
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!calendarOpen || cycleType != CycleType.Click)
				return;

			if (e.Button == SButton.MouseLeft)
			{
				// Dangerous conversion, but calendarOpen should prevent problems for now
				List<ClickableTextureComponent> days = (Game1.activeClickableMenu as Billboard).calendarDays;

				for (int i = 0; i < days.Count; i++)
				{
					// Force the game in UIMode to get the correct scaling for pixel coordinates, since for calendar UIMode = false, for some reason...
					Game1.InUIMode(() =>
					{
						Vector2 point = e.Cursor.GetScaledScreenPixels();

						if (days[i].containsPoint((int)point.X, (int)point.Y))
						{
							clickedDay = i + 1;
							Monitor.Log("Player clicked on day " + clickedDay + " at " + point.ToString());
						}
					});
				}
			}
		}


		/// <summary>
		/// Tracks the cursor position to change the sprite if hovered over
		/// </summary>
		private void OnCursorMoved(object sender, CursorMovedEventArgs e)
		{
			if (cycleType != CycleType.Hover || !calendarOpen)
				return;
			
			// Force the game in UIMode to get the correct scaling for pixel coordinates, since for calendar UIMode = false, for some reason...
			Game1.InUIMode(() =>
			{
				cursorPos = e.NewPosition.GetScaledScreenPixels();
			});
		}

		
		class BirthdayHelper
		{
			// Reference to the monitor to allow error logging
			private readonly IMonitor monitor;
			public List<Birthday>[] birthdays = new List<Birthday>[4];


			public BirthdayHelper(IMonitor m)
			{
				monitor = m;

				// Initialize the array of lists
				for (int i = 0; i < birthdays.Length; i++)
				{
					birthdays[i] = new List<Birthday>();
				}
			}


			/// <summary>
			/// Adds birthday and the NPC to the correct list
			/// </summary>
			internal void AddBirthday(string season, int birthday, NPC n)
			{
				List<Birthday> list = GetListOfBirthdays(season);

				if (list == null)
				{
					monitor.Log("Failed to add birthday " + season + birthday.ToString() + " for " + n.Name, LogLevel.Error);
					return;
				}

				// null if birthday hasn't been added
				Birthday day = list.Find(x => x.day == birthday);

				if (day == null)
				{
					// Add the birthday
					Birthday newDay = new Birthday(monitor, birthday);
					newDay.AddNPC(n);
					list.Add(newDay);
				}
				else
				{
					// Add the npc to the existing day
					day.AddNPC(n);
				}
			}


			/// <summary>
			/// Returns the list of birthday days for the season
			/// </summary>
			/// <param name="season">Wanted season</param>
			/// <param name="onlyShared">Return only shared birthday days</param>
			/// <returns></returns>
			public List<int> GetDays(string season, bool onlyShared = false)
			{
				List<Birthday> list = GetListOfBirthdays(season);

				List<int> days = new List<int>();

				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!onlyShared || list[i].GetNPCs().Count > 1)
							days.Add(list[i].day);
					}
				}

				return days;
			}


			/// <summary>
			/// Returns the list of NPCs that have the given birthday. Returns null if no NPC matches the date.
			/// </summary>
			internal List<NPC> GetNpcs(string season, int day)
			{
				Birthday birthday = GetBirthday(season, day);

				if (birthday == null)
					return null;
				else
					return birthday.GetNPCs();
			}


			/// <summary>
			/// Returns the birthday object if it exists
			/// </summary>
			private Birthday GetBirthday(string season, int day)
			{
				List<Birthday> list = GetListOfBirthdays(season);

				if (list == null)
					return null;

				return list.Find(x => x.day == day);
			}


			/// <summary>
			/// Returns the list of Birthdays for the given season. Returns null if such list was not found.
			/// </summary>
			private List<Birthday> GetListOfBirthdays(string season)
			{
				try
				{
					int index = Utility.getSeasonNumber(season);
					return birthdays[index];
				}
				catch (Exception)
				{
					monitor.Log("Index problems", LogLevel.Error);
					return null;
				}

			}


			/// <summary>
			/// Returns the NPC for the day
			/// </summary>
			/// <param name="season">Season</param>
			/// <param name="day">Day</param>
			/// <param name="nextInCycle">Get next available sprite</param>
			internal Texture2D GetSprite(string season, int day, bool nextInCycle)
			{
				Birthday birthday = GetBirthday(season, day);
				return birthday.GetSprite(nextInCycle);
			}
		}


		class Birthday
		{
			// The day
			public int day;
			// List of NPCs who have a birthday that day
			private List<NPC> npcs = new List<NPC>();

			// Keep track of which npc is currently shown for the day
			private int currentSpriteIndex = 0;
			// Reference to the outer class to allow error logging
			private readonly IMonitor monitor;

			public Birthday(IMonitor m, int day)
			{
				monitor = m;
				this.day = day;
			}


			public List<NPC> GetNPCs(bool hideUnmet = true)
			{
				List<NPC> list = new List<NPC>();

				for (int i = 0; i < npcs.Count; i++)
				{
					// This should filter out NPCs you haven't met yet, maybe...
					if (npcs[i].CanSocialize || !hideUnmet)
					{
						list.Add(npcs[i]);
					}
				}

				return list;
			}


			public void AddNPC(NPC n)
			{
				npcs.Add(n);
			}


			/// <summary>
			/// Return the NPC sprite
			/// </summary>
			/// <param name="incrementSpriteIndex">Select the next available sprite</param>
			internal Texture2D GetSprite(bool incrementSpriteIndex)
			{
				NPC n;

				List<NPC> list = GetNPCs();

				// There is no texture if there is no NPCs
				if (list.Count == 0)
				{
					return null;
				}

				if (incrementSpriteIndex)
				{
					// Increment the index and loop it back to 0 if we reached the end
					currentSpriteIndex++;
					if (currentSpriteIndex == list.Count)
						currentSpriteIndex = 0;
				}

				try
				{
					n = list[currentSpriteIndex];
				}
				catch (Exception)
				{
					monitor.Log("Getting the NPC from the index failed", LogLevel.Error);
					return null;
				}

				Texture2D texture;

				// How the base game handles getting the sprite
				try
				{
					texture = Game1.content.Load<Texture2D>("Characters\\" + n.getTextureName());
				}
				catch (Exception)
				{
					texture = n.Sprite.Texture;
				}

				if (incrementSpriteIndex)
					monitor.Log("Sprite changed from " + (currentSpriteIndex == 0 ? list[list.Count - 1].Name : list[currentSpriteIndex - 1].Name) + " to " + list[currentSpriteIndex].Name, LogLevel.Trace);

				return texture;
			}
		}

		
		enum CycleType
		{
			Always,
			Hover,
			Click
		}
	}


	public interface GenericModConfigAPI
	{
		void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

		void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);
		void RegisterChoiceOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet, string[] choices);
		void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
	}
}
