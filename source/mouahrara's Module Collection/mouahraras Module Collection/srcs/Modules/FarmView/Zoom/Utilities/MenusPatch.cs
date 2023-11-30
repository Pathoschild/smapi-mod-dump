/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using mouahrarasModuleCollection.FarmView.Zoom.Hooks;

namespace mouahrarasModuleCollection.FarmView.Zoom.Utilities
{
	internal class MenusPatchUtility
	{
		internal static void EnterFarmViewPostfix()
		{
			if (!ModEntry.Config.FarmViewZoom)
				return;
			ModEntry.Helper.Events.GameLoop.UpdateTicking += UpdateTickingHook.Apply;
		}

		internal static void LeaveFarmViewPostfix()
		{
			if (!ModEntry.Config.FarmViewZoom)
				return;
			ModEntry.Helper.Events.GameLoop.UpdateTicking -= UpdateTickingHook.Apply;
			ZoomUtility.Reset();
		}
	}
}
