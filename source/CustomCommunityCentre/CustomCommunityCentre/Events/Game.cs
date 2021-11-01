/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using StardewValley.Locations;
using System;

namespace CustomCommunityCentre.Events
{
    public class Game
	{
		public class ResetSharedStateEventArgs : EventArgs
		{
			public CommunityCenter CommunityCentre { get; }


			internal ResetSharedStateEventArgs(CommunityCenter communityCentre)
			{
				this.CommunityCentre = communityCentre;
			}
		}

		public class AreaLoadedEventArgs : EventArgs
		{
			public CommunityCenter CommunityCentre { get; }
			public string AreaName { get; }
			public int AreaNumber { get; }


			internal AreaLoadedEventArgs(CommunityCenter communityCentre, string areaName, int areaNumber)
			{
				this.CommunityCentre = communityCentre;
				this.AreaName = areaName;
				this.AreaNumber = areaNumber;
			}
		}

		public class AreaCompleteCutsceneStartedEventArgs : EventArgs
		{
			public string AreaName { get; }
			public int AreaNumber { get; }


			internal AreaCompleteCutsceneStartedEventArgs(string areaName, int areaNumber)
			{
				this.AreaName = areaName;
				this.AreaNumber = areaNumber;
			}
		}

		public static event EventHandler<ResetSharedStateEventArgs> ResetSharedState;
		public static event EventHandler<AreaLoadedEventArgs> AreaLoaded;
		public static event EventHandler<AreaCompleteCutsceneStartedEventArgs> AreaCompleteCutsceneStarted;


		internal static void InvokeOnResetSharedState(CommunityCenter communityCentre)
		{
			ResetSharedState?.Invoke(
				sender: null,
				e: new ResetSharedStateEventArgs(
					communityCentre: communityCentre));
		}

		internal static void InvokeOnAreaLoaded(CommunityCenter communityCentre, string areaName, int areaNumber)
		{
			AreaLoaded?.Invoke(
				sender: null,
				e: new AreaLoadedEventArgs(
					communityCentre: communityCentre,
					areaName: areaName,
					areaNumber: areaNumber));
		}

		internal static void InvokeOnAreaCompleteCutsceneStarted(string areaName, int areaNumber)
		{
			AreaCompleteCutsceneStarted?.Invoke(
				sender: null,
				e: new AreaCompleteCutsceneStartedEventArgs(
					areaName: areaName,
					areaNumber: areaNumber));
		}
	}
}
