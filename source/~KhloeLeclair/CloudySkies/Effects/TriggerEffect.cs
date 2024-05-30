/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Leclair.Stardew.CloudySkies.Models;
using Leclair.Stardew.Common.Serialization;
using Leclair.Stardew.Common.Serialization.Converters;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;

using StardewValley.Triggers;

namespace Leclair.Stardew.CloudySkies.Effects;

[DiscriminatedType("Trigger")]
public record TriggerEffectData : BaseEffectData, ITriggerEffectData {

	[JsonConverter(typeof(AbstractConverter<ValueEqualityList<string>, List<string>>))]
	public List<string> ApplyActions { get; set; } = new ValueEqualityList<string>();

	[JsonConverter(typeof(AbstractConverter<ValueEqualityList<string>, List<string>>))]
	public List<string> Actions { get; set; } = new ValueEqualityList<string>();

	[JsonConverter(typeof(AbstractConverter<ValueEqualityList<string>, List<string>>))]
	public List<string> RemoveActions { get; set; } = new ValueEqualityList<string>();

}

public class TriggerEffect : IWeatherEffect {

	private readonly ModEntry Mod;

	public ulong Id { get; }

	public uint Rate { get; private set; }

	private readonly string[]? ApplyActions;
	private readonly string[]? Actions;
	private readonly string[]? RemoveActions;
	private readonly uint _Rate;

	private bool isApplied = false;
	private bool isRemoved = true;

	private static string[]? ToArrayOrNull(List<string>? input) {
		if (input is null || input.Count == 0)
			return null;
		return input.ToArray();
	}

	public TriggerEffect(ModEntry mod, ulong id, ITriggerEffectData data) {
		Mod = mod;
		Id = id;
		_Rate = data.Rate;
		Rate = data.Rate;

		ApplyActions = ToArrayOrNull(data.ApplyActions);
		Actions = ToArrayOrNull(data.Actions);
		RemoveActions = ToArrayOrNull(data.RemoveActions);

		if (Actions is not null)
			foreach (string action in Actions) {
				if (action.Contains("If ") && action.Contains("##")) {
					Mod.Log($"Weather effect '{data.Id}' action contains 'If'. You should not use game state queries in Actions for performance reasons.", StardewModdingAPI.LogLevel.Warn);
					break;
				}
			}
	}

	public void Update(GameTime time) {

		isRemoved = false;

		if (!isApplied) {
			isApplied = true;
			if (ApplyActions != null)
				foreach (string action in ApplyActions)
					if (!TriggerActionManager.TryRunAction(action, out string? error, out Exception? ex)) {
						Mod.Log($"Error running trigger for weather effect.\nAction: {action}\nError: {error}", StardewModdingAPI.LogLevel.Error, ex);
						break;
					}
		}

		// If we don't have actions, and we've applied our
		// initial triggers, then we can just stop now.
		if (Actions is null) {
			Rate = uint.MaxValue;
			return;
		}

		foreach (string action in Actions)
			if (!TriggerActionManager.TryRunAction(action, out string? error, out Exception? ex)) {
				Mod.Log($"Error running trigger for weather effect.\nAction: {action}\nError: {error}", StardewModdingAPI.LogLevel.Error, ex);
				break;
			}
	}

	public void Remove() {

		if (isRemoved || RemoveActions is null)
			return;

		// Reset our state.
		isApplied = false;
		isRemoved = true;
		Rate = _Rate;

		foreach (string action in RemoveActions)
			if (!TriggerActionManager.TryRunAction(action, out string? error, out Exception? ex)) {
				Mod.Log($"Error running trigger for weather effect.\nAction: {action}\nError: {error}", StardewModdingAPI.LogLevel.Error, ex);
				break;
			}
	}

}
