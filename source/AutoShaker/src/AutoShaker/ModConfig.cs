/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jag3dagster/AutoShaker
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace AutoShaker
{
	internal class ModConfig
	{
		private const int MinFruitsReady = 1;
		private const int MaxFruitsReady = 3;

		private int _fruitsReadyToShake;

		public bool IsShakerActive { get; set; }
		public KeybindList ToggleShaker { get; set; }
		public bool ShakeRegularTrees { get; set; }
		public bool ShakeFruitTrees { get; set; }
		public int FruitsReadyToShake
		{
			get => _fruitsReadyToShake;
			set => _fruitsReadyToShake = Math.Clamp(value, MinFruitsReady, MaxFruitsReady);
		}
		public bool ShakeTeaBushes { get; set; }
		public bool ShakeBushes { get; set; }
		public bool UsePlayerMagnetism { get; set; }
		public int ShakeDistance { get; set; }

		public void ResetToDefault()
		{
			IsShakerActive = true;
			ToggleShaker = new KeybindList(
				new Keybind(SButton.LeftAlt, SButton.H),
				new Keybind(SButton.RightAlt, SButton.H));

			ShakeRegularTrees = true;
			ShakeFruitTrees = true;
			ShakeTeaBushes = true;
			FruitsReadyToShake = MinFruitsReady;
			ShakeBushes = true;

			UsePlayerMagnetism = false;
			ShakeDistance = 2;
		}

		public ModConfig()
		{
			ResetToDefault();
		}

		public void RegisterModConfigMenu(IModHelper helper, IManifest manifest)
		{
			if (!helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu")) return;

			var gmcmApi = helper.ModRegistry.GetApi<IGenericModConfigMenu>("spacechase0.GenericModConfigMenu");

			gmcmApi.Register(manifest, ResetToDefault, () => helper.WriteConfig(this));

			// IsShakerActive
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: I18n.IsShakerActive_Name,
				tooltip: I18n.IsShakerActive_Description,
				getValue: () => IsShakerActive,
				setValue: val => IsShakerActive = val);

			// ToggleShaker
			gmcmApi.AddKeybindList(
				mod: manifest,
				name: I18n.ToggleShaker_Name,
				tooltip: I18n.ToggleShaker_Description ,
				getValue: () => ToggleShaker,
				setValue: val => ToggleShaker = val);

			// ShakeRegularTrees
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: I18n.ShakeRegularTrees_Name,
				tooltip: I18n.ShakeRegularTrees_Description,
				getValue: () => ShakeRegularTrees,
				setValue: val => ShakeRegularTrees = val);

			// ShakeFruitTrees
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: I18n.ShakeFruitTrees_Name,
				tooltip: I18n.ShakeFruitTrees_Description,
				getValue: () => ShakeFruitTrees,
				setValue: val => ShakeFruitTrees = val);

			// FruitsReadyToShake
			gmcmApi.AddNumberOption(
				mod: manifest,
				name: I18n.FruitsReadyToShake_Name,
				tooltip: I18n.FruitsReadyToShake_Description,
				getValue: () => FruitsReadyToShake,
				setValue: val => FruitsReadyToShake = val,
				min: MinFruitsReady,
				max: MaxFruitsReady);

			// ShakeTeaBushes
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: I18n.ShakeTeaBushes_Name,
				tooltip: I18n.ShakeTeaBushes_Description,
				getValue: () => ShakeTeaBushes,
				setValue: val => ShakeTeaBushes = val);

			// ShakeBushes
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: I18n.ShakeBushes_Name,
				tooltip: I18n.ShakeBushes_Description,
				getValue: () => ShakeBushes,
				setValue: val => ShakeBushes = val);

			// UsePlayerMagnetism
			gmcmApi.AddBoolOption(
				mod: manifest,
				name: I18n.UsePlayerMagnetism_Name,
				tooltip: I18n.UsePlayerMagnetism_Description,
				getValue: () => UsePlayerMagnetism,
				setValue: val => UsePlayerMagnetism = val);

			// ShakeDistance
			gmcmApi.AddNumberOption(
				mod: manifest,
				name: I18n.ShakeDistance_Name,
				tooltip: I18n.ShakeDistance_Description,
				getValue: () => ShakeDistance,
				setValue: val => ShakeDistance = val);
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
	}
}
