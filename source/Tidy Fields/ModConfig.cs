/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/tophatsquid/sdv-tidy-fields
**
*************************************************/

using System;

namespace TidyFields
{
	public class ModConfig
	{
		public bool place_torches_in_scarecrows { get; set; } = true;
		public bool place_torches_in_sprinklers { get; set; } = true;
		public bool place_sprinklers_on_fences { get; set; } = true;
	}
}