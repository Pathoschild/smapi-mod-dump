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
using System.Linq;

namespace TheLion.Stardew.Common.Extensions
{
	public static class GeneralExtensions
	{
		/// <summary>Determine if the calling object is equivalent to any of the objects in a sequence.</summary>
		/// <param name="collection">A sequence of <typeparamref name="T" /> objects.</param>
		public static bool AnyOf<T>(this T obj, params T[] collection)
		{
			return collection.Contains(obj);
		}

		/// <summary>Determine if the calling object is equivalent to any of the objects in a sequence.</summary>
		/// <param name="collection">A sequence of <typeparamref name="T" /> objects.</param>
		public static bool AnyOf<T>(this T obj, IEnumerable<T> collection)
		{
			return collection.Contains(obj);
		}

		/// <summary>Determine if the calling object's type is equivalent to any of the types in a sequence.</summary>
		/// <param name="types">A sequence of <see cref="Type" />'s.</param>
		public static bool AnyOfType<T>(this T t, params Type[] types)
		{
			return t.GetType().AnyOf(types);
		}
	}
}