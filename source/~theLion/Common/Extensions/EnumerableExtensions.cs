/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace TheLion.Stardew.Common.Extensions
{
	public static class EnumerableExtensions
	{
		/// <summary>Apply an action to each item in <see cref="IEnumerable{T}" />.</summary>
		/// <param name="action">An action to apply.</param>
		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items) action(item);
		}
	}
}