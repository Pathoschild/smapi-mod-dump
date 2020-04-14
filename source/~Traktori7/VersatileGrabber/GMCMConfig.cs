using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VersatileGrabber
{
	class GMCMConfig
	{
		/*  Add to ModEntry: 
         *      internal static AutoGrabber.GMCMConfig config;
         *
         *  Add to Entry:
         *      config = helper.ReadConfig<AutoGrabber.GMCMConfig>();
         *      helper.Events.GameLoop.GameLaunched += (s, e) => config.RegisterModConfigMenu(helper, ModManifest);
         */

		public SButton Key { get; set; }
		public string Text { get; set; }
		public bool Active { get; set; }
		public string Choice { get; set; }
		public int Number { get; set; }
		public float Decimal { get; set; }

		public IEnumerable<string> GetChoices()
		{
			yield return "Option1";
			yield return "Option2";
			yield return "Option3";
			yield return "Option4";
			yield return "Option5";
			yield return "Option6";
		}

		public void ResetToDefault()
		{
			Key = SButton.J;
			Text = "Text";
			Number = 1;
			Active = true;
			Decimal = 0.5f;
			Choice = "Option1";
		}

		public void RegisterModConfigMenu(IModHelper helper, IManifest manifest)
		{
			if (!helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
				return;

			var api = helper.ModRegistry.GetApi<IGMCMAPI>("spacechase0.GenericModConfigMenu");

			api.RegisterModConfig(manifest, ResetToDefault, () => helper.WriteConfig(this));

			api.RegisterLabel(manifest, manifest.Name, manifest.Description);

			api.RegisterSimpleOption(manifest, helper.Translation.Get("Key.name"), helper.Translation.Get("Key.description"), () => Key, (SButton k) => Key = k);
			api.RegisterSimpleOption(manifest, helper.Translation.Get("Text.name"), helper.Translation.Get("Text.description"), () => Text, (string t) => Text = t);
			api.RegisterSimpleOption(manifest, helper.Translation.Get("Active.name"), helper.Translation.Get("Active.description"), () => Active, (bool a) => Active = a);

			api.RegisterClampedOption(manifest, helper.Translation.Get("Number.name"), helper.Translation.Get("Number.description"), () => Number, (int n) => Number = n, 0, 100);
			api.RegisterClampedOption(manifest, helper.Translation.Get("Decimal.name"), helper.Translation.Get("Decimal.description"), () => Number, (float f) => Decimal = f, 0, 1);

			api.RegisterChoiceOption(manifest, helper.Translation.Get("Choice.name"), helper.Translation.Get("Choice.description"), () => Choice, (string c) => Choice = c, GetChoices().ToArray());
		}

		public GMCMConfig()
		{
			ResetToDefault();
		}
	}

	public interface IGMCMAPI
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
