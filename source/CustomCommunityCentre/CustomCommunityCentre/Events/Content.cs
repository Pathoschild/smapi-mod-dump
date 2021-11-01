/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using System;
using System.Collections.Generic;

namespace CustomCommunityCentre.Events
{
    public class Content
	{
		public class LoadingContentPacksEventArgs : EventArgs
		{
			internal readonly List<string> ContentPackPaths = new();


			internal LoadingContentPacksEventArgs()
			{
			}

			public void LoadContentPack(string absoluteDirectoryPath)
            {
				this.ContentPackPaths.Add(absoluteDirectoryPath);
			}
		}

		public static event EventHandler<LoadingContentPacksEventArgs> LoadingContentPacks;

		internal static IEnumerable<string> InvokeOnLoadingContentPacks()
		{
			var e = new LoadingContentPacksEventArgs();
			LoadingContentPacks?.Invoke(
				sender: null,
				e: e);
			return e.ContentPackPaths;
		}
	}
}
