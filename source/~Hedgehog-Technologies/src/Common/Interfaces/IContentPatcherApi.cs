/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

namespace HedgeTech.Common.Interfaces
{
	public interface IContentPatcherApi
	{
		/*********
		** Accessors
		*********/
		/// <summary>Whether the conditions API is initialized and ready for use.</summary>
		/// <remarks>Due to the Content Patcher lifecycle, the conditions API becomes available roughly two ticks after the <see cref="IGameLoopEvents.GameLaunched"/> event.</remarks>
		bool IsConditionsApiReady { get; }
	}
}
