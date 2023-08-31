/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.API;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using AeroCore.Utils;

namespace AeroCore.Framework
{
	internal class ConfigFactory
	{
		private static IGMCMAPI gmcm;
		private static readonly Dictionary<IManifest, Dictionary<PropertyInfo, object>> defaultConfigValues = new();

		internal static void ResetConfig(IManifest which, object config)
		{
			foreach ((var prop, var val) in defaultConfigValues[which])
				prop.SetValue(config, val);
		}
		internal static string TranslateEnum(IModHelper helper, string enumName, string value)
			=> helper.Translation.Get($"config.{enumName}.{value}");

		public static void BuildConfig<T>(IManifest who, IModHelper helper, T config, Action ConfigChanged = null, bool TitleScreenOnly = false)
			where T : class, new()
		{
			if (!ModEntry.helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
				return;
			gmcm ??= ModEntry.helper.ModRegistry.GetApi<IGMCMAPI>("spacechase0.GenericModConfigMenu");

			var type = typeof(T);
			var defaults = new Dictionary<PropertyInfo, object>();
			var i18n = helper.Translation;
			foreach (var prop in type.GetProperties())
				defaults.Add(prop, prop.GetValue(config));
			defaultConfigValues.Add(who, defaults);
			gmcm.Register(who,
				() => ResetConfig(who, config),
				() => { helper.WriteConfig(config); ConfigChanged?.Invoke(); },
				TitleScreenOnly
			);
			var pages = new Dictionary<string, Dictionary<string, List<PropertyInfo>>>();
			foreach (var prop in type.GetProperties())
			{
				var pid = prop.GetCustomAttribute<GMCMPageAttribute>()?.ID ?? string.Empty;
				var sid = prop.GetCustomAttribute<GMCMSectionAttribute>()?.ID ?? string.Empty;
				if (!pages.TryGetValue(pid, out var cpage))
					cpage = pages[pid] = new();
				if (!cpage.TryGetValue(sid, out var csec))
					csec = cpage[sid] = new();
				csec.Add(prop);
			}
			foreach ((var name, var page) in pages)
			{
				if (name != string.Empty)
				{
					gmcm.AddPage(who, name, () => i18n.Get($"config.pages.{name}.label"));
				}
				else
				{
					var img = type.GetCustomAttribute<GMCMImageAttribute>();
					if (img is not null)
						gmcm.AddImage(who, () => helper.ModContent.Load<Texture2D>(img.Path), scale: img.Scale);
				}

				foreach ((var sname, var section) in page)
				{
					if (sname != string.Empty)
						gmcm.AddSectionTitle(who, () => i18n.Get($"config.{sname}.header"));
					foreach (var option in section)
					{
						var img = option.GetCustomAttribute<GMCMImageAttribute>();
						if (img is not null)
							gmcm.AddImage(who, () => helper.ModContent.Load<Texture2D>(img.Path), scale: img.Scale);

						var t = option.PropertyType;
						if (t.IsEnum)
						{
							gmcm.AddTextOption(who,
								() => option.GetValue(config).ToString(), (s) => option.SetValue(config, Enum.Parse(t, s)),
								() => i18n.Get($"config.{option.Name}.label"), () => i18n.Get($"config.{option.Name}.desc"),
								Enum.GetNames(t), (s) => TranslateEnum(helper, t.Name, s)
							);
						}
						else if (t == typeof(string))
						{
							gmcm.AddTextOption(who,
								option.GetMethod.CreateDelegateOld<Func<string>>(config), option.SetMethod.CreateDelegateOld<Action<string>>(config),
								() => i18n.Get($"config.{option.Name}.label"), () => i18n.Get($"config.{option.Name}.desc")
							);
						}
						else if (t == typeof(bool))
						{
							gmcm.AddBoolOption(who,
								option.GetMethod.CreateDelegateOld<Func<bool>>(config), option.SetMethod.CreateDelegateOld<Action<bool>>(config),
								() => i18n.Get($"config.{option.Name}.label"), () => i18n.Get($"config.{option.Name}.desc")
							);
						}
						else if (t == typeof(float))
						{
							var range = option.GetCustomAttribute<GMCMRangeAttribute>();
							gmcm.AddNumberOption(who,
								option.GetMethod.CreateDelegateOld<Func<float>>(config), option.SetMethod.CreateDelegateOld<Action<float>>(config),
								() => i18n.Get($"config.{option.Name}.label"), () => i18n.Get($"config.{option.Name}.desc"),
								range?.Min, range?.Max, option.GetCustomAttribute<GMCMIntervalAttribute>()?.Interval ?? .01f
							);
						}
						else if (t == typeof(int))
						{
							var range = option.GetCustomAttribute<GMCMRangeAttribute>();
							gmcm.AddNumberOption(who,
								option.GetMethod.CreateDelegateOld<Func<int>>(config), option.SetMethod.CreateDelegateOld<Action<int>>(config),
								() => i18n.Get($"config.{option.Name}.label"), () => i18n.Get($"config.{option.Name}.desc"),
								(int?)(range?.Min), (int?)(range?.Max), (int?)(option.GetCustomAttribute<GMCMIntervalAttribute>()?.Interval) ?? 1
							);
						}
						else if (t == typeof(KeybindList))
						{
							gmcm.AddKeybindList(who,
								option.GetMethod.CreateDelegateOld<Func<KeybindList>>(config), option.SetMethod.CreateDelegateOld<Action<KeybindList>>(config),
								() => i18n.Get($"config.{option.Name}.label"), () => i18n.Get($"config.{option.Name}.desc")
							);
						}
						var text = option.GetCustomAttribute<GMCMParagraphAttribute>();
						if (text is not null)
							gmcm.AddParagraph(who, () => i18n.Get($"config.{option.Name}.text"));
					}
				}

				if (name.Length == 0)
					foreach (var link in pages.Keys)
						if (link.Length > 0)
							gmcm.AddPageLink(who, link,
								() => i18n.Get($"config.pages.{link}.label"),
								() => i18n.Get($"config.pages.{link}.desc")
							);
			}
		}
	}
}
