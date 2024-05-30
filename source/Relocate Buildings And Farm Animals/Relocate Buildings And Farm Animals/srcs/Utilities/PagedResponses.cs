/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/RelocateFarmAnimals
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace RelocateBuildingsAndFarmAnimals.Utilities
{
	internal class PagedResponsesMenuUtility
	{
		private static readonly PerScreen<bool> isPagedResponsesMenu = new(() => false);
		private static readonly PerScreen<float> zoomLevel = new(() => -1f);
		private static readonly PerScreen<IClickableMenu> previousMenu = new();

		public static bool IsPagedResponsesMenu
		{
			get => isPagedResponsesMenu.Value;
			set => isPagedResponsesMenu.Value = value;
		}

		public static float ZoomLevel
		{
			get => zoomLevel.Value;
			set => zoomLevel.Value = value;
		}

		public static IClickableMenu PreviousMenu
		{
			get => previousMenu.Value;
			set => previousMenu.Value = value;
		}

		internal static void BeforeOpen(bool returnToPreviousMenuAfterClose)
		{
			IsPagedResponsesMenu = true;
			ZoomLevel = Game1.options.zoomLevel;
			if (returnToPreviousMenuAfterClose)
			{
				PreviousMenu = Game1.activeClickableMenu;
			}
		}

		public static void Open(string prompt, List<KeyValuePair<string, string>> responses, Action<string> on_response, bool auto_select_single_choice = false, bool addCancel = true, int itemsPerPage = 5, bool returnToPreviousMenuAfterClose = true)
		{
			void WrappedOnResponse(string response)
			{
				Close();
				on_response(response);
			}

			BeforeOpen(returnToPreviousMenuAfterClose);
			Game1.playSound("smallSelect");
			Game1.currentLocation.ShowPagedResponses(prompt, responses, WrappedOnResponse, auto_select_single_choice, addCancel, itemsPerPage);
		}

		public static void ReceiveMenuButtonKeyPress(SButton button)
		{
			if (Game1.activeClickableMenu is DialogueBox dialogueBox && IsPagedResponsesMenu)
			{
				dialogueBox.receiveKeyPress(Keys.N);
				ModEntry.Helper.Input.Suppress(button);
			}
		}

		public static void Close()
		{
			if (Game1.activeClickableMenu is DialogueBox dialogueBox && IsPagedResponsesMenu)
			{
				Game1.currentLocation.answerDialogueAction("pagedResponse_cancel", null);
				dialogueBox.closeDialogue();
			}
		}

		internal static void AfterClose()
		{
			if (IsPagedResponsesMenu)
			{
				if (PreviousMenu is not null)
				{
					Game1.activeClickableMenu = PreviousMenu;
				}
				PreviousMenu = null;
				ZoomLevel = -1f;
				IsPagedResponsesMenu = false;
			}
		}

		public static List<KeyValuePair<string, string>> GetRelocateBuildingsResponses()
		{
			List<KeyValuePair<string, string>> responses = new();

			foreach (GameLocation location in Game1.locations)
			{
				if (location.IsBuildableLocation())
				{
					responses.Add(new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName));
				}
			}
			if (!responses.Any())
			{
				Farm farm = Game1.getFarm();

				responses.Add(new KeyValuePair<string, string>(farm.NameOrUniqueName, farm.DisplayName));
			}
			return responses;
		}

		public static List<KeyValuePair<string, string>> GetRelocateFarmAnimalsResponses()
		{
			List<KeyValuePair<string, string>> responses = new();

			foreach (GameLocation location in Game1.locations)
			{
				if (location.buildings.Any((Building p) => p.GetIndoors() is AnimalHouse) && (!Game1.IsClient || location.CanBeRemotedlyViewed()))
				{
					responses.Add(new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName));
				}
			}
			if (!responses.Any())
			{
				Farm farm = Game1.getFarm();

				responses.Add(new KeyValuePair<string, string>(farm.NameOrUniqueName, farm.DisplayName));
			}
			return responses;
		}
	}
}
