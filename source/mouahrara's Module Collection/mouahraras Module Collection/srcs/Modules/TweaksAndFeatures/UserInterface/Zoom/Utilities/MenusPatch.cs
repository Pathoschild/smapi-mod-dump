/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using mouahrarasModuleCollection.TweaksAndFeatures.UserInterface.Zoom.Handlers;

namespace mouahrarasModuleCollection.TweaksAndFeatures.UserInterface.Zoom.Utilities
{
	internal class MenusPatchUtility
	{
		internal static void EnterFarmViewPostfix()
		{
			if (!ModEntry.Config.UserInterfaceZoom)
				return;
			ModEntry.Helper.Events.GameLoop.UpdateTicking += UpdateTickingHandler.Apply;
		}

		internal static void LeaveFarmViewPostfix()
		{
			if (!ModEntry.Config.UserInterfaceZoom)
				return;
			ModEntry.Helper.Events.GameLoop.UpdateTicking -= UpdateTickingHandler.Apply;
			ZoomUtility.Reset();
		}
	}
}
