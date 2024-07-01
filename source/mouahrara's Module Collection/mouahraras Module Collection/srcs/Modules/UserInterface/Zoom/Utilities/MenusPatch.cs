/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using mouahrarasModuleCollection.UserInterface.Zoom.Handlers;

namespace mouahrarasModuleCollection.UserInterface.Zoom.Utilities
{
	internal class MenusPatchUtility
	{
		private static readonly PerScreen<int>	afterResetTicks = new(() => -1);

		internal static int AfterResetTicks
		{
			get => afterResetTicks.Value;
			set => afterResetTicks.Value = value;
		}

		internal static void EnterFarmViewPostfix()
		{
			if (!ModEntry.Config.UserInterfaceZoom)
				return;

			ModEntry.Helper.Events.GameLoop.UpdateTicking -= UpdateTickingHandler.Apply;
			ModEntry.Helper.Events.GameLoop.UpdateTicking += UpdateTickingHandler.Apply;
		}

		internal static void LeaveFarmViewPostfix()
		{
			if (!ModEntry.Config.UserInterfaceZoom)
				return;

			ModEntry.Helper.Events.GameLoop.UpdateTicking -= UpdateTickingHandler.Apply;
			if (Game1.isWarping && Game1.locationRequest is not null)
			{
				Game1.locationRequest.OnLoad += ZoomUtility.Reset;
			}
			else
			{
				ZoomUtility.Reset();
			}
		}

		internal static bool ShouldProcess(IClickableMenu __instance)
		{
			if (!ModEntry.Config.UserInterfaceZoom)
				return false;

			CarpenterMenu carpenterMenu = __instance as CarpenterMenu;
			PurchaseAnimalsMenu purchaseAnimalsMenu = __instance as PurchaseAnimalsMenu;
			AnimalQueryMenu animalQueryMenu = __instance as AnimalQueryMenu;

			if (carpenterMenu is null && purchaseAnimalsMenu is null && animalQueryMenu is null)
				return false;
			if (carpenterMenu is not null && carpenterMenu.freeze)
				return false;
			if (purchaseAnimalsMenu is not null && purchaseAnimalsMenu.freeze)
				return false;
			if (Game1.IsFading())
				return false;
			if (!__instance.shouldClampGamePadCursor() || !__instance.overrideSnappyMenuCursorMovementBan())
				return false;
			return true;
		}
	}
}
