/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using GenericModConfigMenu;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace Leclair.Stardew.Common.Integrations.GenericModConfigMenu;

public class GMCMIntegration<T, M> : BaseAPIIntegration<IGenericModConfigMenuApi, M> where T : new() where M : Mod {

	private readonly Func<T> GetConfig;
	private readonly Action ResetConfig;
	private readonly Action SaveConfig;

	private bool IsRegistered;

	private IManifest Consumer { get => Self.ModManifest; }

	public GMCMIntegration(M self, Func<T> getConfig, Action resetConfig, Action saveConfig) : base(self, "spacechase0.GenericModConfigMenu", "1.8.0") {
		IsRegistered = false;

		GetConfig = getConfig;
		ResetConfig = resetConfig;
		SaveConfig = saveConfig;
	}

	#region Registration

	public GMCMIntegration<T, M> Register(bool? allowInGameChanges = null) {
		AssertLoaded();

		if (!IsRegistered)
			API.Register(Consumer, ResetConfig, SaveConfig, allowInGameChanges.HasValue && ! allowInGameChanges.Value);

		// Am I a joke to you?
		IsRegistered = true;

		return this;
	}

	public GMCMIntegration<T, M> Unregister() {
		AssertLoaded();

		API.Unregister(Consumer);
		IsRegistered = false;

		return this;
	}

	#endregion

	#region Editability

	public GMCMIntegration<T, M> SetTitleOnly(bool titleOnly) {
		AssertLoaded();
		API.SetTitleScreenOnlyForNextOptions(Consumer, titleOnly);
		return this;
	}

	#endregion

	#region Pages

	public GMCMIntegration<T, M> StartPage(string name, string displayName) {
		AssertLoaded();
		API.AddPage(
			Consumer,
			name,
			string.IsNullOrEmpty(displayName) ? null : () => displayName
		);
		return this;
	}

	public GMCMIntegration<T, M> StartPage(string name, Func<string> displayName) {
		AssertLoaded();
		API.AddPage(
			Consumer,
			name,
			displayName
		);
		return this;
	}

	#endregion

	#region Formatting

	public GMCMIntegration<T, M> AddLabel(string name, string? tooltip = null, string? shortcut = null) {
		return AddLabel(
			() => name,
			string.IsNullOrEmpty(tooltip) ? null : () => tooltip,
			shortcut
		);
	}

	public GMCMIntegration<T, M> AddLabel(Func<string> name, Func<string>? tooltip = null, string? shortcut = null) {
		AssertLoaded();
		if (string.IsNullOrEmpty(shortcut))
			API.AddSectionTitle(Consumer, name, tooltip);
		else
			API.AddPageLink(Consumer, shortcut, name, tooltip);
		return this;
	}

	public GMCMIntegration<T, M> AddParagraph(string text) {
		return AddParagraph(() => text);
	}

	public GMCMIntegration<T, M> AddParagraph(Func<string> text) {
		AssertLoaded();
		API.AddParagraph(Consumer, text);
		return this;
	}

	public GMCMIntegration<T, M> AddImage(string path, Rectangle? source = null, int scale = 4) {
		AssertLoaded();
		//API.AddImage(Consumer, () => Self.Helper.Content.Load<Texture2D>(path), source, scale);
		API.AddImage(Consumer, () => Self.Helper.ModContent.Load<Texture2D>(path), source, scale);
		return this;
	}

	public GMCMIntegration<T, M> AddImage(Func<Texture2D> texture, Rectangle? source = null, int scale = 4) {
		AssertLoaded();
		API.AddImage(Consumer, texture, source, scale);
		return this;
	}

	#endregion

	#region Fancy Controls

	public GMCMIntegration<T, M> AddChoice<TType>(Func<string> name, Func<string>? tooltip, Func<T, TType> get, Action<T, TType> set, IEnumerable<KeyValuePair<TType, Func<string>>> choices, string? fieldId = null) {
		List<TType> values = new();
		List<Func<string>> labels = new();

		foreach (KeyValuePair<TType, Func<string>> entry in choices) {
			values.Add(entry.Key);
			labels.Add(entry.Value);
		}

		return AddChoice(name, tooltip, get, set, labels, values, fieldId);
	}

	public GMCMIntegration<T, M> AddChoice<TType>(Func<string> name, Func<string>? tooltip, Func<T, TType> get, Action<T, TType> set, IEnumerable<KeyValuePair<Func<string>, TType>> choices, string? fieldId = null) {
		List<TType> values = new();
		List<Func<string>> labels = new();

		foreach (KeyValuePair<Func<string>, TType> entry in choices) {
			values.Add(entry.Value);
			labels.Add(entry.Key);
		}

		return AddChoice(name, tooltip, get, set, labels, values, fieldId);
	}

	public GMCMIntegration<T, M> AddChoice<TType>(Func<string> name, Func<string>? tooltip, Func<T, TType> get, Action<T, TType> set, IList<Func<string>> labels, IList<TType> values, string? fieldId = null) {
		return AddChoice(
			get,
			set,
			labels,
			values,
			name,
			tooltip,
			fieldId
		);
	}

	public GMCMIntegration<T, M> AddChoice<TType>(Func<T, TType> get, Action<T, TType> set, IList<Func<string>> labels, IList<TType> values, Func<string> name, Func<string>? tooltip = null, string? fieldId = null) {
		AssertLoaded();

		List<string> keys = new(labels.Count);
		for(int i = 0; i < labels.Count; i++)
			keys.Add(i.ToString());

		API.AddTextOption(
			mod: Consumer,
			getValue: () => {
				TType val = get(GetConfig());
				int idx = values.IndexOf(val);
				return idx == -1 ? "0" : idx.ToString();
			},
			setValue: val => {
				if (!int.TryParse(val, out int idx))
					idx = -1;

				set(GetConfig(), idx == -1 ? values[0] : values[idx]);
			},
			name: name,
			tooltip: tooltip,
			allowedValues: keys.ToArray(),
			formatAllowedValue: key => {
				if (!int.TryParse(key, out int idx))
					idx = -1;

				return labels[idx == -1 ? 0 : idx]();
			},
			fieldId: fieldId
		);

		return this;
	}

	public GMCMIntegration<T, M> AddChoice<TType>(string name, string? tooltip, Func<T, TType> get, Action<T, TType> set, IEnumerable<KeyValuePair<TType, string>> choices, string? fieldId = null) {
		List<TType> values = new();
		List<string> labels = new();

		foreach (KeyValuePair<TType, string> entry in choices) {
			values.Add(entry.Key);
			labels.Add(entry.Value);
		}

		return AddChoice(name, tooltip, get, set, labels, values, fieldId);
	}

	public GMCMIntegration<T, M> AddChoice<TType>(string name, string? tooltip, Func<T, TType> get, Action<T, TType> set, IEnumerable<KeyValuePair<string, TType>> choices, string? fieldId = null) {
		List<TType> values = new();
		List<string> labels = new();

		foreach (KeyValuePair<string, TType> entry in choices) {
			values.Add(entry.Value);
			labels.Add(entry.Key);
		}

		return AddChoice(name, tooltip, get, set, labels, values, fieldId);
	}

	public GMCMIntegration<T, M> AddChoice<TType>(string name, string? tooltip, Func<T, TType> get, Action<T, TType> set, IList<string> labels, IList<TType> values, string? fieldId = null) {
		return AddChoice(
			get,
			set,
			labels,
			values,
			() => name,
			string.IsNullOrEmpty(tooltip) ? null : () => tooltip,
			fieldId
		);
	}

	public GMCMIntegration<T, M> AddChoice<TType>(Func<T, TType> get, Action<T, TType> set, IList<string> labels, IList<TType> values, Func<string> name, Func<string>? tooltip = null, string? fieldId = null) {
		AssertLoaded();

		API.AddTextOption(
			mod: Consumer,
			getValue: () => {
				TType val = get(GetConfig());
				int idx = values.IndexOf(val);
				return idx == -1 ? labels[0] : labels[idx];
			},
			setValue: val => {
				int idx = labels.IndexOf(val);
				set(GetConfig(), idx == -1 ? values[0] : values[idx]);
			},
			name: name,
			tooltip: tooltip,
			allowedValues: labels.ToArray(),
			fieldId: fieldId
		);

		return this;
	}

	#endregion

	#region Basic Controls

	public GMCMIntegration<T, M> Add(string name, string? tooltip, Func<T, bool> get, Action<T, bool> set, string? fieldId = null) {
		return Add(
			() => name,
			string.IsNullOrEmpty(tooltip) ? null : () => tooltip,
			get,
			set,
			fieldId
		);
	}

	public GMCMIntegration<T, M> Add(Func<string> name, Func<string>? tooltip, Func<T, bool> get, Action<T, bool> set, string? fieldId = null) {
		AssertLoaded();
		API.AddBoolOption(
			mod: Consumer,
			getValue: () => get(GetConfig()),
			setValue: val => set(GetConfig(), val),
			name: name,
			tooltip: tooltip,
			fieldId: fieldId
		);
		return this;
	}

	public GMCMIntegration<T, M> Add(string name, string? tooltip, Func<T, string> get, Action<T, string> set, string? fieldId = null) {
		return Add(
			() => name,
			string.IsNullOrEmpty(tooltip) ? null : () => tooltip,
			get,
			set,
			fieldId
		);
	}

	public GMCMIntegration<T, M> Add(Func<string> name, Func<string>? tooltip, Func<T, string> get, Action<T, string> set, string? fieldId = null) {
		AssertLoaded();
		API.AddTextOption(
			mod: Consumer,
			getValue: () => get(GetConfig()),
			setValue: val => set(GetConfig(), val),
			name: name,
			tooltip: tooltip,
			fieldId: fieldId
		);
		return this;
	}

	public GMCMIntegration<T, M> Add(string name, string? tooltip, Func<T, SButton> get, Action<T, SButton> set, string? fieldId = null) {
		return Add(
			() => name,
			string.IsNullOrEmpty(tooltip) ? null : () => tooltip,
			get,
			set,
			fieldId
		);
	}

	public GMCMIntegration<T, M> Add(Func<string> name, Func<string>? tooltip, Func<T, SButton> get, Action<T, SButton> set, string? fieldId = null) {
		AssertLoaded();
		API.AddKeybind(
			mod: Consumer,
			getValue: () => get(GetConfig()),
			setValue: val => set(GetConfig(), val),
			name: name,
			tooltip: tooltip,
			fieldId: fieldId
		);
		return this;
	}

	public GMCMIntegration<T, M> Add(string name, string? tooltip, Func<T, KeybindList> get, Action<T, KeybindList> set, string? fieldId = null) {
		return Add(
			() => name,
			string.IsNullOrEmpty(tooltip) ? null : () => tooltip,
			get,
			set,
			fieldId
		);
	}

	public GMCMIntegration<T, M> Add(Func<string> name, Func<string>? tooltip, Func<T, KeybindList> get, Action<T, KeybindList> set, string? fieldId = null) {
		AssertLoaded();
		API.AddKeybindList(
			mod: Consumer,
			getValue: () => get(GetConfig()),
			setValue: val => set(GetConfig(), val),
			name: name,
			tooltip: tooltip,
			fieldId: fieldId
		);
		return this;
	}

	#endregion

	#region Numeric Controls

	public GMCMIntegration<T, M> Add(string name, string? tooltip, Func<T, int> get, Action<T, int> set, int? min = null, int? max = null, int? interval = null, Func<int, string>? format = null, string? fieldId = null) {
		return Add(
			() => name,
			string.IsNullOrEmpty(tooltip) ? null : () => tooltip,
			get,
			set,
			min,
			max,
			interval,
			format,
			fieldId
		);
	}

	public GMCMIntegration<T, M> Add(Func<string> name, Func<string>? tooltip, Func<T, int> get, Action<T, int> set, int? min = null, int? max = null, int? interval = null, Func<int, string>? format = null, string? fieldId = null) {
		AssertLoaded();

		API.AddNumberOption(
			Consumer,
			getValue: () => get(GetConfig()),
			setValue: val => set(GetConfig(), val),
			name: name,
			tooltip: tooltip,
			min: min,
			max: max,
			interval: interval,
			formatValue: format,
			fieldId: fieldId
		);

		return this;
	}

	public GMCMIntegration<T, M> Add(string name, string? tooltip, Func<T, float> get, Action<T, float> set, float? min = null, float? max = null, float? interval = null, Func<float, string>? format = null, string? fieldId = null) {
		return Add(
			() => name,
			string.IsNullOrEmpty(tooltip) ? null : () => tooltip,
			get,
			set,
			min,
			max,
			interval,
			format,
			fieldId
		);
	}

	public GMCMIntegration<T, M> Add(Func<string> name, Func<string>? tooltip, Func<T, float> get, Action<T, float> set, float? min = null, float? max = null, float? interval = null, Func<float, string>? format = null, string? fieldId = null) {
		AssertLoaded();

		API.AddNumberOption(
			Consumer,
			getValue: () => get(GetConfig()),
			setValue: val => set(GetConfig(), val),
			name: name,
			tooltip: tooltip,
			min: min,
			max: max,
			interval: interval,
			formatValue: format,
			fieldId: fieldId
		);

		return this;
	}

	#endregion

	#region Current Menu Stuff

	public GMCMIntegration<T, M> OpenMenu() {
		AssertLoaded();
		API.OpenModMenu(Consumer);
		return this;
	}

	public bool TryGetCurrentMenu(out IManifest mod, out string page) {
		AssertLoaded();
		return TryGetCurrentMenu(out mod, out page);
	}

	#endregion

}
