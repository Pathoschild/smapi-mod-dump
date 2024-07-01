/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Shockah.XPDisplay.WalkOfLife
{
	public interface IMasteriesConfig
	{

		/// <summary>Gets a value indicating whether the player can gain levels up to 20 and choose Prestiged professions.</summary>
		[JsonProperty]
		public bool EnablePrestigeLevels { get; }

		/// <summary>Gets how much skill experience is required for each level up beyond 10.</summary>
		[JsonProperty]
		public uint ExpPerPrestigeLevel { get; }

		/// <summary>Gets the monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
		[JsonProperty]
		public uint PrestigeRespecCost { get; }
	}
}
