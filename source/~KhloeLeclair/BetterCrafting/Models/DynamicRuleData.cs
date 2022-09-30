/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Leclair.Stardew.BetterCrafting.Models;

/// <summary>
/// IDynamicRuleData instances represent all the configuration data associated
/// with dynamic rules that have been added to categories.
/// </summary>
public interface IDynamicRuleData {

	/// <summary>
	/// The ID of the dynamic rule this data is for.
	/// </summary>
	public string Id { get; }

	/// <summary>
	/// A dictionary of configuration data for this dynamic rule, as parsed
	/// by the game's JSON serializer.
	/// </summary>
	public IDictionary<string, JToken> Fields { get; }

}

public class DynamicRuleData : IDynamicRuleData {

	public string Id { get; set; } = string.Empty;

	[JsonExtensionData]
	public IDictionary<string, JToken> Fields { get; set; } = new Dictionary<string, JToken>();

	public static DynamicRuleData FromGeneric(IDynamicRuleData other) {
		var result = new DynamicRuleData {
			Id = other.Id
		};

		foreach(var entry in other.Fields)
			result.Fields.Add(entry.Key, entry.Value);

		return result;
	}

}
