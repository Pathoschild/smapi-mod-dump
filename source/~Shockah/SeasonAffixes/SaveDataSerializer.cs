/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.Kokoro;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Shockah.SeasonAffixes;

internal interface ISaveDataSerializer
{
	SerializedSaveData Serialize(SaveData data);
	SaveData Deserialize(SerializedSaveData data);
}

internal sealed class SaveDataSerializer : ISaveDataSerializer
{
	public SerializedSaveData Serialize(SaveData data)
	{
		SerializedSaveData result = new(SeasonAffixes.Instance.ModManifest.Version);
		result.ActiveAffixes.AddRange(data.ActiveAffixes.Select(a => a.UniqueID));
		result.AffixChoiceHistory.AddRange(data.AffixChoiceHistory.Select(step => step.Select(a => a.UniqueID).ToList()));
		result.AffixSetChoiceHistory.AddRange(data.AffixSetChoiceHistory.Select(step => step.Select(set => set.Select(a => a.UniqueID).ToList()).ToList()));
		return result;
	}

	public SaveData Deserialize(SerializedSaveData data)
	{
		ISeasonAffix? GetOrLog(string id, string context, LogLevel level)
		{
			var affix = SeasonAffixes.Instance.GetAffix(id);
			if (affix is null)
				SeasonAffixes.Instance.Monitor.Log($"Tried to deserialize affix `{id}` for {context}, but no such affix is registered. Did you remove a mod?", level);
			return affix;
		}

		bool TryGetOrLog(string id, string context, LogLevel level, [NotNullWhen(true)] out ISeasonAffix? affix)
		{
			var value = GetOrLog(id, context, level);
			affix = value;
			return value is not null;
		}

		SaveData result = new();
		foreach (var id in data.ActiveAffixes)
			if (TryGetOrLog(id, "active affixes", LogLevel.Warn, out var affix))
				result.ActiveAffixes.Add(affix);
		foreach (var step in data.AffixChoiceHistory)
			result.AffixChoiceHistory.Add(step.Select(id => GetOrLog(id, "affix choice history", LogLevel.Info)).WhereNotNull().ToHashSet());
		foreach (var step in data.AffixSetChoiceHistory)
			result.AffixSetChoiceHistory.Add(step.Select(set => (ISet<ISeasonAffix>)set.Select(id => GetOrLog(id, "affix set choice history", LogLevel.Info)).WhereNotNull().ToHashSet()).ToHashSet());
		return result;
	}
}