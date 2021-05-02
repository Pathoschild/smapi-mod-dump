/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods/-/tree/master/WalkOfLife
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;

namespace TheLion.Common.Integrations
{
	/// <summary>Handles the logic for integrating with the Generic Mod Configuration Menu mod.</summary>
	/// <typeparam name="TConfig">The mod configuration type.</typeparam>
	internal class GenericModConfigMenuIntegration<TConfig> : BaseIntegration
		where TConfig : new()
	{
		/// <summary>The mod's public API.</summary>
		private readonly IGenericModConfigMenuAPI ModApi;

		/// <summary>The manifest for the mod consuming the API.</summary>
		private readonly IManifest ConsumerManifest;

		/// <summary>Get the current config model.</summary>
		private readonly Func<TConfig> GetConfig;

		/// <summary>Reset the config model to the default values.</summary>
		private readonly Action Reset;

		/// <summary>Save and apply the current config model.</summary>
		private readonly Action SaveAndApply;

		/// <summary>Construct an instance.</summary>
		/// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
		/// <param name="monitor">Encapsulates monitoring and logging.</param>
		/// <param name="consumerManifest">The manifest for the mod consuming the API.</param>
		/// <param name="getConfig">Get the current config model.</param>
		/// <param name="reset">Reset the config model to the default values.</param>
		/// <param name="saveAndApply">Save and apply the current config model.</param>
		public GenericModConfigMenuIntegration(IModRegistry modRegistry, IMonitor monitor, IManifest consumerManifest, Func<TConfig> getConfig, Action reset, Action saveAndApply)
			: base("Generic Mod Config Menu", "spacechase0.GenericModConfigMenu", "1.1.0", modRegistry, monitor)
		{
			// init
			ConsumerManifest = consumerManifest;
			GetConfig = getConfig;
			Reset = reset;
			SaveAndApply = saveAndApply;

			// get mod API
			if (IsLoaded)
			{
				ModApi = GetValidatedApi<IGenericModConfigMenuAPI>();
				IsLoaded = ModApi != null;
			}
		}

		/// <summary>Register the mod config.</summary>
		public GenericModConfigMenuIntegration<TConfig> RegisterConfig()
		{
			AssertLoaded();
			ModApi.RegisterModConfig(ConsumerManifest, Reset, SaveAndApply);
			return this;
		}

		/// <summary>Add a label to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		public GenericModConfigMenuIntegration<TConfig> AddLabel(string label, string description = null)
		{
			AssertLoaded();
			ModApi.RegisterLabel(ConsumerManifest, label, description);
			return this;
		}

		/// <summary>Start a new form page.</summary>
		/// <param name="pageName">The name of the page.</param>
		public GenericModConfigMenuIntegration<TConfig> AddNewPage(string pageName)
		{
			AssertLoaded();
			ModApi.StartNewPage(ConsumerManifest, pageName);
			return this;
		}

		/// <summary>Add a label to a different form page.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="page">The target page name.</param>
		public GenericModConfigMenuIntegration<TConfig> AddPageLabel(string label, string description = null, string page = "")
		{
			AssertLoaded();
			ModApi.RegisterPageLabel(ConsumerManifest, label, description, page);
			return this;
		}

		/// <summary>Add a checkbox to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddCheckbox(string label, string description, Func<TConfig, bool> get, Action<TConfig, bool> set, bool enable = true)
		{
			AssertLoaded();

			if (enable)
			{
				ModApi.RegisterSimpleOption(
					mod: ConsumerManifest,
					optionName: label,
					optionDesc: description,
					optionGet: () => get(GetConfig()),
					optionSet: val => set(GetConfig(), val)
				);
			}

			return this;
		}

		/// <summary>Add a dropdown to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="choices">The choices to choose from.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddDropdown(string label, string description, Func<TConfig, string> get, Action<TConfig, string> set, string[] choices, bool enable = true)
		{
			AssertLoaded();

			if (enable)
			{
				ModApi.RegisterChoiceOption(
					mod: ConsumerManifest,
					optionName: label,
					optionDesc: description,
					optionGet: () => get(GetConfig()),
					optionSet: val => set(GetConfig(), val),
					choices: choices
				);
			}

			return this;
		}

		/// <summary>Add a checkbox to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddTextbox(string label, string description, Func<TConfig, string> get, Action<TConfig, string> set, bool enable = true)
		{
			AssertLoaded();

			if (enable)
			{
				ModApi.RegisterSimpleOption(
					mod: ConsumerManifest,
					optionName: label,
					optionDesc: description,
					optionGet: () => get(GetConfig()),
					optionSet: val => set(GetConfig(), val)
				);
			}

			return this;
		}

		/// <summary>Add a numeric field to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddNumberField(string label, string description, Func<TConfig, int> get, Action<TConfig, int> set, int min, int max, bool enable = true)
		{
			AssertLoaded();

			if (enable)
			{
				ModApi.RegisterClampedOption(
					mod: ConsumerManifest,
					optionName: label,
					optionDesc: description,
					optionGet: () => get(GetConfig()),
					optionSet: val => set(GetConfig(), val),
					min: min,
					max: max
				);
			}

			return this;
		}

		/// <summary>Add a numeric field to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddNumberField(string label, string description, Func<TConfig, float> get, Action<TConfig, float> set, float min, float max, bool enable = true)
		{
			AssertLoaded();

			if (enable)
			{
				ModApi.RegisterClampedOption(
					mod: ConsumerManifest,
					optionName: label,
					optionDesc: description,
					optionGet: () => get(GetConfig()),
					optionSet: val => set(GetConfig(), val),
					min: min,
					max: max
				);
			}

			return this;
		}

		/// <summary>Add a key binding field to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddKeyBinding(string label, string description, Func<TConfig, KeybindList> get, Action<TConfig, KeybindList> set, bool enable = true)
		{
			AssertLoaded();

			if (enable)
			{
				ModApi.RegisterSimpleOption(
					mod: ConsumerManifest,
					optionName: label,
					optionDesc: description,
					optionGet: () => get(GetConfig()),
					optionSet: val => set(GetConfig(), val)
				);
			}

			return this;
		}
	}
}