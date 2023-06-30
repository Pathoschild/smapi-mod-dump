/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.MachineStatus;

public static class MachineRenderingOptions
{
	public enum Grouping
	{
		None,
		ByMachine,
		ByMachineAndItem
	}

	public enum Sorting
	{
		None,
		ByMachineAZ, ByMachineZA,
		ReadyFirst, WaitingFirst, BusyFirst,
		ByDistanceAscending, ByDistanceDescending,
		ByItemAZ, ByItemZA
	}

	public enum BubbleSway
	{
		Static,
		Together,
		Wave
	}

	public enum Visibility
	{
		Hidden,
		Normal,
		Focused
	}
}