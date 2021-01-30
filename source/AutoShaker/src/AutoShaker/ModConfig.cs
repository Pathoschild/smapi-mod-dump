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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
			set => _fruitsReadyToShake = 
				value <= MinFruitsReady
					? MinFruitsReady
					: value >= MaxFruitsReady
						? MaxFruitsReady
						: value;
		}
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

			gmcmApi.RegisterModConfig(manifest, ResetToDefault, () => helper.WriteConfig(this));

			// IsShakerActive
			gmcmApi.RegisterSimpleOption(
				manifest,
				"Shaker Is Active",
				"Whether or not the AutoShaker mod is active.",
				() => IsShakerActive,
				val => IsShakerActive = val);

			// ToggleShaker
			{
				//gmcmApi.RegisterSimpleOption(
				//	manifest,
				//	"Toggle Shaker Keybind",
				//	"Keybinding to toggle the AutoShaker on and off.",
				//	() => ToggleShaker,
				//	val => ToggleShaker = val);

				gmcmApi.RegisterLabel(
					manifest,
					"ToggleShaker Keybind Currently Unavailable",
					"Changing the ToggleShaker keybind is currently only possible via the config.json file directly due to recent changes. This should be temporary.");
			}

			// ShakeRegularTrees
			gmcmApi.RegisterSimpleOption(
				manifest,
				"Shake Regular Trees?",
				"Whether or not the AutoShaker will shake regular trees that you walk by for seeds.",
				() => ShakeRegularTrees,
				val => ShakeRegularTrees = val);

			// ShakeFruitTrees
			gmcmApi.RegisterSimpleOption(
				manifest,
				"Shake Fruit Trees?",
				"Whether or not the AutoShaker will shake fruit trees that you walk by for fruit.",
				() => ShakeFruitTrees,
				val => ShakeFruitTrees = val);

			// FruitsReadyToShake
			gmcmApi.RegisterClampedOption(
				manifest,
				"Minimum Fruits Ready to Shake",
				"Minimum amount of fruits a Fruit Tree should have ready before the AutoShaker shakes the tree.",
				() => FruitsReadyToShake,
				val => FruitsReadyToShake = val,
				MinFruitsReady,
				MaxFruitsReady);

			// ShakeBushes
			gmcmApi.RegisterSimpleOption(
				manifest,
				"Shake Bushes?",
				"Whether or not the AutoShaker will shake bushes that you walk by.",
				() => ShakeBushes,
				val => ShakeBushes = val);

			// UsePlayerMagnetism
			gmcmApi.RegisterSimpleOption(
				manifest,
				"Use Player Magnetism Distance?",
				"Whether or not the AutoShaker will shake bushes at the same distance players can pick up items. Note: Overrides 'Shake Distance'",
				() => UsePlayerMagnetism,
				val => UsePlayerMagnetism = val);

			// ShakeDistance
			gmcmApi.RegisterSimpleOption(
				manifest,
				"Shake Distance",
				"Distance to shake bushes when not using 'Player Magnetism.'",
				() => ShakeDistance,
				val => ShakeDistance = val);
		}
	}

	public interface IGenericModConfigMenu
	{
		void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

		void RegisterLabel(IManifest mod, string labelName, string labelDesc);
		void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
		void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);
		void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);
		void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);
		void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<SButton> optionGet, Action<SButton> optionSet);

		void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);
		void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet, float min, float max);

		void RegisterChoiceOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet, string[] choices);

		void RegisterComplexOption(IManifest mod, string optionName, string optionDesc,
			Func<Vector2, object, object> widgetUpdate,
			Func<SpriteBatch, Vector2, object, object> widgetDraw,
			Action<object> onSave);
	}
}
