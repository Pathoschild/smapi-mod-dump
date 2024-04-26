/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace FullFishingBar
{
	public class ModConfig
	{
		private const string _gmcmUniqueId = "spacechase0.GenericModConfigMenu";

		public bool IsEnabled { get; set; }
		public bool OnlyCorkBobber { get; set; }
		public double BarSizePercentage { get; set; }
		public bool ExceptBossFish { get; set; }

		public ModConfig()
		{
			IsEnabled = true;
			OnlyCorkBobber = false;
			BarSizePercentage = 1.0;
			ExceptBossFish = false;
		}

		public void ResetToDefault()
		{
			IsEnabled = true;
			OnlyCorkBobber = false;
			BarSizePercentage = 1.0;
			ExceptBossFish = false;
		}

		public void RegisterModConfigMenu(IModHelper helper, IManifest manifest)
		{
			if (!helper.ModRegistry.IsLoaded(_gmcmUniqueId)) return;

			var gmcmApi = helper.ModRegistry.GetApi<IGenericModConfigMenu>(_gmcmUniqueId);
			if (gmcmApi is null) return;

			try
			{
				gmcmApi.Unregister(manifest);
			}
			catch { }

			gmcmApi.Register(
				mod: manifest,
				reset: ResetToDefault,
				save: () => helper.WriteConfig(this));

			gmcmApi.AddSectionTitle(
				mod: manifest,
				text: I18n.Section_General);

			gmcmApi.AddBoolOption(
				mod: manifest,
				name: I18n.Option_Enabled_Name,
				tooltip: I18n.Option_Enabled_Tooltip,
				getValue: () => IsEnabled,
				setValue: (val) => IsEnabled = val);

			gmcmApi.AddSectionTitle(
				mod: manifest,
				text: I18n.Section_Customizations);

			gmcmApi.AddBoolOption(
				mod: manifest,
				name: I18n.Option_OnlyCorkBobber_Name,
				tooltip: I18n.Option_OnlyCorkBobber_Tooltip,
				getValue: () => OnlyCorkBobber,
				setValue: (val) => OnlyCorkBobber = val);

			gmcmApi.AddNumberOption(
				mod: manifest,
				name: I18n.Option_BarSizePercentage_Name,
				tooltip: I18n.Option_BarSizePercentage_Tooltip,
				getValue: () => Math.Max(10, Math.Min((int)(BarSizePercentage * 100), 100)),
				setValue: (val) => BarSizePercentage = val / 100.0,
				min: 10,
				max: 100,
				interval: 1,
				formatValue: (val) => $"{val}%");

			gmcmApi.AddBoolOption(
				mod: manifest,
				name: I18n.Option_ExceptBossFish_Name,
				tooltip: I18n.Option_ExceptBossFish_Tooltip,
				getValue: () => ExceptBossFish,
				setValue: (val) => ExceptBossFish = val);
		}
	}

	public interface IGenericModConfigMenu
	{
		/*********
		** Methods
		*********/

		/// <summary>Register a mod whose config can be edited through the UI.</summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="reset">Reset the mod's config to its default values.</param>
		/// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
		/// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
		/// <remarks>Each mod can only be registered once, unless it's deleted via <see cref="Unregister"/> before calling this again.</remarks>
		void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

		/****
		** Basic options
		****/

		/// <summary>Add a section title at the current position in the form.</summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="text">The title text shown in the form.</param>
		/// <param name="tooltip">The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the tooltip.</param>
		void AddSectionTitle(IManifest mod, Func<string> text, Func<string>? tooltip = null);

		/// <summary>Add a paragraph of text at the current position in the form.</summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="text">The paragraph text to display.</param>
		void AddParagraph(IManifest mod, Func<string> text);

		/// <summary>Add a boolean option at the current position in the form.</summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="getValue">Get the current value from the mod config.</param>
		/// <param name="setValue">Set a new value in the mod config.</param>
		/// <param name="name">The label text to show in the form.</param>
		/// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
		/// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
		void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);

		/// <summary>Add an integer option at the current position in the form.</summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="getValue">Get the current value from the mod config.</param>
		/// <param name="setValue">Set a new value in the mod config.</param>
		/// <param name="name">The label text to show in the form.</param>
		/// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
		/// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
		/// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
		/// <param name="interval">The interval of values that can be selected.</param>
		/// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
		/// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
		void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string>? tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, string? fieldId = null);


		/// <summary>Add a key binding list at the current position in the form.</summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="getValue">Get the current value from the mod config.</param>
		/// <param name="setValue">Set a new value in the mod config.</param>
		/// <param name="name">The label text to show in the form.</param>
		/// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
		/// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
		void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);

		/****
		** Multi-page management
		****/

		/// <summary>Start a new page in the mod's config UI, or switch to that page if it already exists. All options registered after this will be part of that page.</summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="pageId">The unique page ID.</param>
		/// <param name="pageTitle">The page title shown in its UI, or <c>null</c> to show the <paramref name="pageId"/> value.</param>
		/// <remarks>You must also call <see cref="AddPageLink"/> to make the page accessible. This is only needed to set up a multi-page config UI. If you don't call this method, all options will be part of the mod's main config UI instead.</remarks>
		void AddPage(IManifest mod, string pageId, Func<string>? pageTitle = null);

		/// <summary>Add a link to a page added via <see cref="AddPage"/> at the current position in the form.</summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
		/// <param name="text">The link text shown in the form.</param>
		/// <param name="tooltip">The tooltip text shown when the cursor hovers on the link, or <c>null</c> to disable the tooltip.</param>
		void AddPageLink(IManifest mod, string pageId, Func<string> text, Func<string>? tooltip = null);

		/****
		** Advanced
		****/

		/// <summary>Register a method to notify when any option registered by this mod is edited through the config UI.</summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="onChange">The method to call with the option's unique field ID and new value.</param>
		/// <remarks>Options use a randomized ID by default; you'll likely want to specify the <c>fieldId</c> argument when adding options if you use this.</remarks>
		void OnFieldChanged(IManifest mod, Action<string, object> onChange);

		/// <summary>Remove a mod from the config UI and delete all its options and pages.</summary>
		/// <param name="mod">The mod's manifest.</param>
		void Unregister(IManifest mod);
	}
}
