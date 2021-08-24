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

namespace TheLion.Stardew.Common.Extensions
{
	public static class ArrayExtensions
	{
		/// <summary>Get a subarray from the calling array.</summary>
		/// <param name="index">The starting index.</param>
		/// <param name="length">The subarray length.</param>
		public static T[] SubArray<T>(this T[] data, int index, int length)
		{
			var result = new T[length];
			Array.Copy(data, index, result, 0, length);
			return result;
		}
	}
}