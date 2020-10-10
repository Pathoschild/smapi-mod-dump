/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using System;
namespace StardewNotification
{
	public class Pair<T1, T2>
	{
		public T1 First { get; set; }
		public T2 Second { get; set; }
		public Pair(T1 first, T2 second)
		{
			this.First = first;
			this.Second = second;
		}
	}
}
