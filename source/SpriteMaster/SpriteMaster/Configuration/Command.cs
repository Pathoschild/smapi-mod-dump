/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpriteMaster.Configuration;

internal static class Command {
	private static Serialize.Category Root => Serialize.Root;

	[Command("config", "Config Commands")]
	public static void OnConsoleCommand(string command, Queue<string> arguments) {
		if (arguments.Count == 0) {
			EmitHelp(new());
			return;
		}

		var subCommand = arguments.Dequeue();

		switch (subCommand.ToLowerInvariant()) {
			case "help":
			case "h":
				EmitHelp(arguments);
				break;
			case "query":
			case "q":
				Query(arguments);
				break;
			case "set":
			case "s":
				Set(arguments);
				break;
			case "load":
				Load(arguments);
				break;
			case "save":
			case "commit":
				Save(arguments);
				break;
		}
	}

	private static void EmitHelp(Queue<string> arguments) {
		if (arguments.Count == 0) {

		}
	}

	private static void DumpCategory(Serialize.Category category) {
		var output = new StringBuilder();
		if (category.Children.Count != 0) {
			output.AppendLine("Categories:");
			int maxCategoryLength = int.MinValue;
			foreach (var subCategory in category.Children) {
				maxCategoryLength = Math.Max(maxCategoryLength, subCategory.Value.Name.Length);
			}
			foreach (var subCategory in category.Children) {
				var comment = subCategory.Value.Type.GetCustomAttribute<Attributes.CommentAttribute>();
				output.AppendLine(
					comment is null
					? $"\t{subCategory.Value.Name}"
					: $"\t{subCategory.Value.Name.PadRight(maxCategoryLength)} : {comment.Message}"
				);
			}
			if (category.Fields.Count != 0) {
				output.AppendLine();
			}
		}
		if (category.Fields.Count != 0) {
			output.AppendLine("Fields:");
			int maxFieldLength = int.MinValue;
			foreach (var field in category.Fields) {
				maxFieldLength = Math.Max(maxFieldLength, field.Value.Name.Length);
			}

			foreach (var field in category.Fields) {
				var comment = field.Value.GetCustomAttribute<Attributes.CommentAttribute>();
				output.AppendLine(
					comment is null
					? $"\t{field.Value.Name}"
					: $"\t{field.Value.Name.PadRight(maxFieldLength)} : {comment.Message}"
				);
			}
		}
		Debug.Info(output.ToString());
	}

	private static List<Serialize.Category> GetHierarchy(Serialize.Category category) {
		if (category == Root) {
			return new List<Serialize.Category>();
		}

		var result = new List<Serialize.Category>() { category };

		var parent = category.Parent;
		while (parent is not null) {
			if (parent == Root) {
				break;
			}
			result.Add(parent);
			parent = parent.Parent;
		}

		result.Reverse();
		return result;
	}

	private static void Query(Queue<string> arguments) {
		if (arguments.Count == 0) {
			DumpCategory(Root);
			return;
		}

		var chainString = arguments.Dequeue();
		var chain = chainString.Split('.');

		Serialize.Category category = Root;

		foreach (var arg in chain) {
			var key = arg.ToLowerInvariant();

			if (category.Children.TryGetValue(key, out var subCategory)) {
				category = subCategory;
			}
			else if (category.Fields.TryGetValue(key, out var field)) {
				if (arguments.Count != 0) {
					Debug.Warning($"Unknown Category: {string.Join('.', GetHierarchy(category).Select(cat => cat.Name))}.{key} (found matching field)");
					return;
				}

				var fieldValue = field.GetValue(null);
				if (field.FieldType.IsEnum) {
					var validNames = string.Join(", ", Enum.GetNames(field.FieldType));
					Debug.Info($"{string.Join('.', GetHierarchy(category).Select(cat => cat.Name))}.{field.Name} = '{fieldValue}' ({field.FieldType.Name}: {validNames})");
				}
				else {
					Debug.Info($"{string.Join('.', GetHierarchy(category).Select(cat => cat.Name))}.{field.Name} = '{fieldValue}' ({field.FieldType.Name})");
				}
				return;
			}
			else {
				Debug.Warning($"Unknown Category: {string.Join('.', GetHierarchy(category).Select(cat => cat.Name))}.{key}");
				return;
			}
		}

		DumpCategory(category);
	}

	private static void Set(Queue<string> arguments) {
		if (arguments.Count == 0) {
			Debug.Warning("Nothing passed to set");
			return;
		}

		var chainString = arguments.Dequeue();
		if (arguments.Count == 0) {
			Debug.Warning("No value passed to set");
			return;
		}
		var value = arguments.Dequeue();
		var chain = chainString.Split('.');

		Serialize.Category category = Root;
		FieldInfo? field = null;

		foreach (var arg in chain) {
			var key = arg.ToLowerInvariant();

			if (category.Children.TryGetValue(key, out var subCategory)) {
				category = subCategory;
			}
			else if (category.Fields.TryGetValue(key, out field)) {
				if (arguments.Count != 0) {
					Debug.Warning($"Unknown Category: {string.Join('.', GetHierarchy(category).Select(cat => cat.Name))}.{arg} (found matching field)");
					return;
				}
			}
			else {
				Debug.Warning($"Unknown Field or Category: {string.Join('.', GetHierarchy(category).Select(cat => cat.Name))}.{arg}");
				return;
			}
		}
		if (field is null) {
			Debug.Warning($"Field not found: {chainString}");
			return;
		}

		SetValue(field, value);
	}

	internal static void OnSetValue(FieldInfo field) {
		var options = field.GetCustomAttribute<Attributes.OptionsAttribute>();
		if (options is null) {
			return;
		}

		if (options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.FlushTextureCache)) {
		}
		if (options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.FlushSuspendedSpriteCache)) {
			Caching.SuspendedSpriteCache.Purge();
		}
		if (options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.FlushFileCache)) {
			Caching.FileCache.Purge(reset: true);
		}
		if (options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.FlushResidentCache)) {
			Caching.ResidentCache.Purge();
		}
		if (options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.FlushTextureFileCache)) {
			Caching.TextureFileCache.Purge();
		}
		if (options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.ResetDisplay)) {
			StardewValley.Game1.graphics.ApplyChanges();
		}
		if (options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.GarbageCollect)) {
			Extensions.Garbage.Collect(compact: true, blocking: true, background: false);
		}
		if (options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.FlushMetaData) || options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.FlushMetaDataRecache)) {
			Metadata.Metadata.Purge(recache: options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.FlushMetaDataRecache));
		}
		if (options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.FlushSpriteMap)) {
			SpriteMap.Purge();
		}
		if (options.Flags.HasFlag(Attributes.OptionsAttribute.Flag.GarbageCollect)) {
			Extensions.Garbage.Collect(compact: true, blocking: true, background: false);
		}
	}

	internal static void SetValue(FieldInfo field, string value) {
		if (field.FieldType == typeof(string)) {
			field.SetValue(null, value);
		}
		else if (field.FieldType.IsEnum) {
			var enumValue = Enum.Parse(field.FieldType, value, true);
			field.SetValue(null, enumValue);
		}
		else if (
			field.FieldType == typeof(byte) ||
			field.FieldType == typeof(ushort) ||
			field.FieldType == typeof(uint) ||
			field.FieldType == typeof(ulong)
		) {
			if (!ulong.TryParse(value, out var intValue)) {
				Debug.Warning($"Could not parse '{value}' as an unsigned integer");
			}
			field.SetValue(null, Convert.ChangeType(intValue, field.FieldType));
		}
		else if (
			field.FieldType == typeof(sbyte) ||
			field.FieldType == typeof(short) ||
			field.FieldType == typeof(int) ||
			field.FieldType == typeof(long)
		) {
			if (!long.TryParse(value, out var intValue)) {
				Debug.Warning($"Could not parse '{value}' as a signed integer");
			}
			field.SetValue(null, Convert.ChangeType(intValue, field.FieldType));
		}
		else if (field.FieldType == typeof(float) || field.FieldType == typeof(double)) {
			if (!double.TryParse(value, out var realValue)) {
				Debug.Warning($"Could not parse '{value}' as a floating-point value");
			}
			field.SetValue(null, Convert.ChangeType(realValue, field.FieldType));
		}
		else {
			throw new NotImplementedException($"Type not yet implemented: {field.FieldType}");
		}

		OnSetValue(field);
	}

	internal static void SetValue<T>(FieldInfo field, T value) {
		if (field.FieldType.IsEnum) {
			field.SetValue(null, (int)(object)value!);
		}
		else {
			field.SetValue(null, Convert.ChangeType(value, field.FieldType));
		}
		OnSetValue(field);
	}

	internal static T GetValue<T>(FieldInfo field) {
		if (field.FieldType.IsEnum) {
			return (T)(object)field.GetValue(null)!;
		}
		else {
			return (T)Convert.ChangeType(field.GetValue(null), typeof(T))!;
		}
	}

	private static void Load(Queue<string> arguments) {
		string path = Config.Path;
		if (arguments.Count > 1) {
			throw new ArgumentException($"Too many arguments for load: expected 0 or 1, found {arguments.Count}");
		}
		if (arguments.Count == 1) {
			path = arguments.Dequeue();
		}

		LoadConfig(path, retain: true);
	}

	internal static bool LoadConfig(string? path = null, bool retain = true) {
		path ??= Config.Path;

		return Serialize.Load(path, retain);
	}

	private static void Save(Queue<string> arguments) {
		string path = Config.Path;
		if (arguments.Count > 1) {
			throw new ArgumentException($"Too many arguments for save: expected 0 or 1, found {arguments.Count}");
		}
		if (arguments.Count == 1) {
			path = arguments.Dequeue();
		}

		SaveConfig(path);
	}

	internal static bool SaveConfig(string? path = null) {
		path ??= Config.Path;

		return Serialize.Save(path);
	}
}
