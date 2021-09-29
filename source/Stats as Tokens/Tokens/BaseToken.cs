/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/StatsAsTokens
**
*************************************************/

// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace StatsAsTokens
{
	/// <summary>
	/// Base class which all Tokens extend - handles basic token functionality. Tokens must implement their own parsing of input and returning of values.
	/// </summary>
	internal abstract class BaseToken
	{
		internal static readonly string host = "hostPlayer", loc = "localPlayer";

		/*********
		** Public methods
		*********/

		/****
		** Metadata
		****/

		/// <summary>Get whether the token allows input arguments (e.g. an NPC name for a relationship token).</summary>
		public bool AllowsInput()
		{
			return true;
		}

		/// <summary>Whether the token requires input arguments to work, and does not provide values without it (see <see cref="AllowsInput"/>).</summary>
		/// <remarks>Default false.</remarks>
		public bool RequiresInput()
		{
			return true;
		}

		/// <summary>Whether the token may return multiple values for the given input.</summary>
		/// <param name="input">The input arguments, if applicable.</param>
		public bool CanHaveMultipleValues(string input = null)
		{
			return false;
		}

		/// <summary>Get whether the token always returns a value within a bounded numeric range for the given input. Mutually exclusive with <see cref="HasBoundedValues"/>.</summary>
		/// <param name="input">The input arguments, if any.</param>
		/// <param name="min">The minimum value this token may return.</param>
		/// <param name="max">The maximum value this token may return.</param>
		/// <remarks>Default false.</remarks>
		public bool HasBoundedRangeValues(string input, out int min, out int max)
		{
			min = 0;
			max = int.MaxValue;
			return true;
		}


		/// <summary>Validate that the provided input arguments are valid.</summary>
		/// <param name="input">The input arguments, if any.</param>
		/// <param name="error">The validation error, if any.</param>
		/// <returns>Returns whether validation succeeded.</returns>
		/// <remarks>Default true.</remarks>
		public abstract bool TryValidateInput(string input, out string error);

		/****
		** State
		****/

		/// <summary>Update the values when the context changes.</summary>
		/// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
		public bool UpdateContext()
		{
			bool hasChanged = false;

			if (SaveGame.loaded != null || Context.IsWorldReady)
			{
				hasChanged = DidStatsChange();
			}

			return hasChanged;
		}

		/// <summary>
		/// Checks to see if stats changed. Updates cached values if they are out of date.
		/// </summary>
		/// <returns><c>True</c> if stats changed, <c>False</c> otherwise.</returns>
		protected abstract bool DidStatsChange();

		/// <summary>Get whether the token is available for use.</summary>
		public bool IsReady()
		{
			return SaveGame.loaded != null || Context.IsWorldReady;
		}

		/// <summary>Get the current values.</summary>
		/// <param name="input">The input arguments, if applicable.</param>
		public abstract IEnumerable<string> GetValues(string input);

	}
}
