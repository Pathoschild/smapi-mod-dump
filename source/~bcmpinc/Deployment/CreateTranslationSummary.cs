/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/


using System;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Internationalization.Deployment
{
	/* See documentation at https://github.com/Pathoschild/StardewScripts. */

    internal class CreateTranslationSummary {
		/*********
		** Configure
		*********/
		/****
		** Repo settings
		****/
		/// <summary>The solution folder to scan for translatable mods.</summary>
		readonly string SolutionFolder = @"C:\Users\bauke\source\repos\StardewHack";

		/// <summary>The relative paths within <see cref="SolutionFolder"/> to ignore when scanning for mod folders, using the system default separators (e.g. <c>\</c> on Windows).</summary>
		readonly string[] IgnoreRelativePaths = new[] { "_archived" };

		/// <summary>Path substrings within <see cref="SolutionFolder"/> to ignore when scanning for mod folders, using the system default separators (e.g. <c>\</c> on Windows).</summary>
		readonly string[] IgnorePathSubstrings = new[]
		{
			Path.Combine("bin", "Debug"),
			Path.Combine("bin", "Release")
		};


		/****
		** Format settings
		****/
		/// <summary>The translation table style (one of <c>Auto</c>, <c>RowPerLocale</c>, or <c>RowPerMod</c>).</summary>
		readonly TableStyle Style = TableStyle.Auto;

		/// <summary>Whether to link each status in the translation table to the i18n file, if it exists.</summary>
		/// <remarks>This also changes how mods with multiple <c>i18n</c> folders are handled. If true, each linked file will have its own status in the summary cell. If false, only one overall status will be shown.</remarks>
		readonly bool LinkToFiles = true;

		/// <summary>Whether to link the status for a missing translation file to the <c>i18n</c> folder so it's easier to find.</summary>
		readonly bool LinkMissingToFolder = true;

		/// <summary>The display info for known languages.</summary>
		readonly Dictionary<string, ModLanguage> Languages = new(StringComparer.OrdinalIgnoreCase)
		{
			// required vanilla languages (shown as missing if there's no translation file)
			["de"] = new ModLanguage("German"),
			["es"] = new ModLanguage("Spanish"),
			["fr"] = new ModLanguage("French"),
			["hu"] = new ModLanguage("Hungarian"),
			["it"] = new ModLanguage("Italian"),
			["ja"] = new ModLanguage("Japanese"),
			["ko"] = new ModLanguage("Korean"),
			["pt"] = new ModLanguage("Portuguese"),
			["ru"] = new ModLanguage("Russian"),
			["tr"] = new ModLanguage("Turkish"),
			["zh"] = new ModLanguage("Chinese"),

			// optional custom languages (only listed if repo has translations for them)
			["pl"] = new ModLanguage("Polish", Required: false, Url: "https://www.nexusmods.com/stardewvalley/mods/3616"),
			["th"] = new ModLanguage("Thai", Required: false, Url: "https://www.nexusmods.com/stardewvalley/mods/7052"),
			["uk"] = new ModLanguage("Ukrainian", Required: false, Url: "https://www.nexusmods.com/stardewvalley/mods/8427")
		};

		/// <summary>A hidden comment to add just under the section header. You can set it to null to disable it.</summary>
		readonly string AddSectionComment = @"<!--

			This section is auto-generated using a script, there's no need to edit it manually.
			https://github.com/Pathoschild/StardewScripts/tree/main/create-translation-summary

		-->";

		readonly JsonSerializerOptions options = new JsonSerializerOptions() {
			AllowTrailingCommas = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			
		};

		/*********
		** Script
		*********/
		public static void Main() => new CreateTranslationSummary();

		/// <summary>Run the script.</summary>
		public CreateTranslationSummary()
		{
			// get mod folders
			ModFolder[] modFolders = this
				.GetModFolders(new DirectoryInfo(this.SolutionFolder))
				.Where(p => !this.ShouldIgnore(p.RelativePath))
				.OrderBy(p => p.ModName.ToLower())
				.ToArray();
	
			// get translation metadata
			this.PopulateTranslationStatuses(modFolders);

			// show warnings
			{
				var warnings = modFolders
					.Where(p => p.Warnings.Any())
					.Select(p => new { p.RelativePath, p.ModName, p.Warnings })
					.ToArray();
				if (warnings.Any())
					Array.ForEach(warnings, (x) => Console.WriteLine(x));
			}

			// format
			var markdown = this.FormatTranslationSummary(modFolders);
			Console.WriteLine(markdown);
		}

		/// <summary>Whether a mod folder should be ignored, based on <see cref="IgnoreFolders"/>.</summary>
		/// <param name="relativePath">The relative path to check.</param>
		private bool ShouldIgnore(string relativePath)
		{
			if (this.IgnoreRelativePaths.Any(ignorePath => relativePath == ignorePath || relativePath.StartsWith(ignorePath + Path.DirectorySeparatorChar)))
				return true;

			if (this.IgnorePathSubstrings.Any(ignoreSubstr => relativePath.Contains(ignoreSubstr)))
				return true;

			return false;
		}

		/// <summary>Render a Markdown translation summary section for the given mods.</summary>
		/// <param name="modFolders">The mods to summarize.</param>
		private string FormatTranslationSummary(ModFolder[] modFolders)
		{
			// get known locales
			modFolders = modFolders.ToArray();
			var locales =
				(
					from locale in this.GetKnownLocales(modFolders)
					let metadata = this.Languages.TryGetValue(locale, out ModLanguage language)
						? language
						: null
					orderby (metadata?.Name ?? locale).ToLower()
					select (Locale: locale, Metadata: metadata)
				)
				.ToArray();

			// generic header
			StringBuilder str = new StringBuilder();
			str.AppendLine("## Translating the mods");

			if (!string.IsNullOrWhiteSpace(this.AddSectionComment))
				str.AppendLine(this.AddSectionComment);

			str
				.AppendLine("The mods can be translated into any language supported by the game, and SMAPI will automatically")
				.AppendLine("use the right translations.")
				.AppendLine()
				.AppendLine("Contributions are welcome! See [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations)")
				.AppendLine("on the wiki for help contributing translations.")
				.AppendLine()
				.AppendLine("(❑ = untranslated, ↻ = partly translated, ✓ = fully translated)")
				.AppendLine();

			// get table style
			TableStyle style = this.Style;
			if (style == TableStyle.Auto)
			{
				style = locales.Length > modFolders.Length
					? TableStyle.RowPerLocale
					: TableStyle.RowPerMod;
			}

			// build data table
			{
				DataTable table = new();

				table.Columns.Add("&nbsp;");

				// row per locale
				if (style == TableStyle.RowPerLocale)
				{
					// header
					foreach (var mod in modFolders)
						table.Columns.Add(mod.ModName);

					// rows
					foreach (var locale in locales)
					{
						string[] row = new string[modFolders.Length + 1];

						// locale name
						row[0] = locale.Metadata?.Url != null
							? $"[{locale.Metadata.Name}]"
							: (locale.Metadata?.Name ?? locale.Locale);

						// statuses
						int i = 1;
						foreach (var mod in modFolders)
							row[i++] = this.RenderStatusCell(mod, locale.Locale);

						table.Rows.Add(row);
					}
				}

				// row per mod
				else
				{
					// header
					foreach (var locale in locales)
						table.Columns.Add(this.RenderLocaleCell(locale.Locale, locale.Metadata));

					// rows
					foreach (var mod in modFolders)
					{
						string[] row = new string[locales.Length + 1];

						int i = 0;
						row[i++] = mod.ModName;
						foreach (var locale in locales)
							row[i++] = this.RenderStatusCell(mod, locale.Locale);

						table.Rows.Add(row);
					}
				}

				str.AppendLine(this.ToMarkdownTable(table));
			}

			// add custom language links
			{
				var links = locales.Select(p => p.Metadata).Where(p => p?.Url != null).ToArray();
				if (links.Any())
				{
					foreach (var link in links)
						str.AppendLine($"[{link.Name}]: {link.Url}");
				}
			}

			return str.ToString();
		}

		/// <summary>Render the content of a locale name cell in a translation summary table.</summary>
		/// <param name="locale">The locale code.</param>
		/// <param name="data">The language data, if any.</param>
		private string RenderLocaleCell(string locale, ModLanguage data)
		{
			return data?.Url != null
				? $"[{data.Name}]"
				: (data?.Name ?? locale);
		}

		/// <summary>Render the content of a status cell in a translation summary table.</summary>
		/// <param name="modFolder">The mod folder.</param>
		/// <param name="locale">The locale code.</param>
		private string RenderStatusCell(ModFolder modFolder, string locale)
		{
			// single status without link
			if (!this.LinkToFiles)
			{
				TranslationStatus[] statuses = modFolder.GetStatusForLocale(locale).Values.Distinct().ToArray();
				if (statuses.All(p => p == TranslationStatus.Complete))
					return "✓";
				if (statuses.All(p => p == TranslationStatus.Missing))
					return "❑";
				return "↻";
			}

			// individual links + statuses
			return string.Join(
				"<br />",
				modFolder
					.GetStatusForLocale(locale)
					.Select(p =>
					{
						string relativePath = p.Key;
						TranslationStatus status = p.Value;

						// get status symbol
						string symbol = status switch
						{
							TranslationStatus.Missing => "❑",
							TranslationStatus.Complete => "✓",
							_ => "↻"
						};

						// render plain symbol if link disabled
						if (status == TranslationStatus.Missing && !this.LinkMissingToFolder)
							return symbol;

						// get link URL
						string url = Path.Combine(modFolder.RelativePath, relativePath);
						if (status != TranslationStatus.Missing)
							url = Path.Combine(url, $"{locale}.json");
						url = url.Replace("\\", "/");

						// fix URL encoding
						// Markdown links break if a relative URL contains [] or spaces
						url = url
							.Replace(" ", "%20")
							.Replace("[", "%5B")
							.Replace("]", "%5D");

						// render
						return $"[{symbol}]({url})";
					})
			);
		}

		/// <summary>Get every locale for which a translation status is defined.</summary>
		/// <param name="modFolders">The mod folders whose translation statuses to scan.</param>
		private HashSet<string> GetKnownLocales(ModFolder[] folders)
		{
			HashSet<string> knownLocales = new(this.Languages.Where(p => p.Value.Required).Select(p => p.Key));

			foreach (var folder in folders)
			{
				foreach (var entry in folder.TranslationStatus.Values)
				{
					foreach (string locale in entry.Keys)
						knownLocales.Add(locale);
				}
			}

			return knownLocales;
		}

		/// <summary>Get all the mod folders which contain <c>i18n</c> subfolders, grouped by parent folder in cases where a mod has submods.</summary>
		/// <param name="solutionDir">The solution directory to search for mod folders.</param>
		private IEnumerable<ModFolder> GetModFolders(DirectoryInfo solutionDir)
		{
			DirectoryInfo[] i18nDirs =
			(
					from dir in solutionDir.GetFiles("default.json", SearchOption.AllDirectories).Select(p => p.Directory)
					where dir.Name == "i18n"
					orderby dir.FullName descending // list parent folders first
					select dir
				)
				.ToArray();

			ModFolder folder = null;
			foreach (DirectoryInfo dir in i18nDirs)
			{
				// collect info
				DirectoryInfo modDir = dir.Parent;
				string relativePath = Path.GetRelativePath(solutionDir.FullName, modDir.FullName);

				// start new folder
				if (folder == null || !relativePath.StartsWith(folder.RelativePath + Path.DirectorySeparatorChar))
				{
					// yield previous folder
					if (folder != null)
					{
						yield return folder;
						folder = null;
					}

					// find manifest.json
					FileInfo manifest = modDir.GetFiles("manifest.json").FirstOrDefault();
					if (manifest == null)
					{
						Console.WriteLine($"Ignored {relativePath}: contains an i18n subfolder, but no manifest.json.");
						continue;
					}

					// build mod info
					string modName = JsonSerializer.Deserialize<Manifest>(File.ReadAllText(manifest.FullName), options).Name;
					folder = new ModFolder(modDir, relativePath, modName);
				}

				// add translation dir
				folder.TranslationDirs.Add(dir);
			}

			if (folder != null)
				yield return folder;
		}

		/// <summary>Populate the translation statuses for the given mod folders.</summary>
		/// <param name="modFolders">The mod folders whose translation statuses to populate.</param>
		private void PopulateTranslationStatuses(ModFolder[] modFolders)
		{
			// populate statuses for existing files
			foreach (var folder in modFolders)
				this.PopulateTranslationStatus(folder);

			// add missing files
			var knownLocales = this.GetKnownLocales(modFolders);
			foreach (var folder in modFolders)
			{
				this.PopulateTranslationStatus(folder);
				foreach (var entry in folder.TranslationStatus.Values)
				{
					foreach (var locale in knownLocales)
						entry.TryAdd(locale, TranslationStatus.Missing);
				}
			}
		}

		/// <summary>Populate the translation statuses for a mod folder.</summary>
		/// <param name="modFolder">The mod folder whose translation statuses to populate.</param>
		private void PopulateTranslationStatus(ModFolder modFolder)
		{
			var regex = new Regex("(^\\s+|,\\s+)([_A-Za-z][_A-Za-z0-9]*)(\\s*:\\s*\")", RegexOptions.Multiline);
			foreach (DirectoryInfo dir in modFolder.TranslationDirs)
			{
				// build status lookup
				string dirRelativePath = Path.GetRelativePath(modFolder.ModDir.FullName, dir.FullName);
				var statuses = modFolder.TranslationStatus[dirRelativePath] = new();

				// get default translation keys
				HashSet<string> defaultKeys = new(StringComparer.OrdinalIgnoreCase);
				{
					var defaultEntry = dir.GetFiles("default.json").First();
					string content = File.ReadAllText(defaultEntry.FullName, Encoding.UTF8);
					content = regex.Replace(content, "$1\"$2\"$3");
					Console.WriteLine(content);
					foreach (var key in JsonSerializer.Deserialize<Dictionary<string, string>>(content, options).Keys)
						defaultKeys.Add(key);
				}

				// get translation files
				foreach (FileInfo file in dir.GetFiles())
				{
					string warnPrefix = $"[{Path.GetRelativePath(modFolder.ModDir.FullName, file.FullName)}]";

					// skip default file
					if (file.Name == "default.json")
						continue;

					// skip invalid
					if (file.Extension != ".json")
					{
						modFolder.Warnings.Add($"{warnPrefix} unknown file extension '{file.Extension}'.");
						continue;
					}

					// parse raw data
					string locale = Path.GetFileNameWithoutExtension(file.Name);
					string content = File.ReadAllText(file.FullName, Encoding.UTF8);
					content = regex.Replace(content, "$1\"$2\"$3");
					Console.WriteLine(content);
					HashSet<string> keys = new HashSet<string>(JsonSerializer.Deserialize<Dictionary<string, string>>(content, options).Keys, StringComparer.OrdinalIgnoreCase);

					// get translation status
					TranslationStatus status = TranslationStatus.Complete;
					if (Regex.IsMatch(content, @"//[ \t]*TODO\b", RegexOptions.IgnoreCase) || defaultKeys.Any(p => !keys.Contains(p)))
						status = TranslationStatus.Incomplete;

					// warn for key mismatch
					{
						var missingDefaultKeys = defaultKeys.Where(key => !keys.Contains(key)).ToArray();
						var unknownKeys = keys.Where(key => !defaultKeys.Contains(key)).ToArray();

						if (missingDefaultKeys.Any())
							modFolder.Warnings.Add($"{warnPrefix} missing default keys: {string.Join(", ", missingDefaultKeys.OrderBy(p => p))}");
						if (unknownKeys.Any())
							modFolder.Warnings.Add($"{warnPrefix} unknown keys: {string.Join(", ", unknownKeys.OrderBy(p => p))}");
					}

					// track
					statuses[locale] = status;
				}
			}
		}

		/// <summary>Get a Markdown representation of the given data table.</summary>
		/// <param name="source">The data table to render.</param>
		private string ToMarkdownTable(DataTable source)
		{
			// get info
			var columns = source.Columns.Cast<DataColumn>().ToArray();
			var rows = source.Rows.Cast<DataRow>().ToArray();

			// get column widths
			int[] colWidths = new int[columns.Length];
			for (int i = 0; i < columns.Length; i++)
				colWidths[i] = columns[i].ColumnName?.Length ?? 0;
			foreach (DataRow row in rows)
			{
				for (int i = 0; i < columns.Length; i++)
					colWidths[i] = Math.Max(colWidths[i], row.Field<object>(i)?.ToString().Length ?? 0);
			}

			// header
			StringBuilder str = new StringBuilder();
			str.AppendLine(
				string.Join(" | ", columns.Select((col, i) => $"{col.ColumnName}".PadRight(colWidths[i]))).TrimEnd()
			);
			str.AppendLine(
				string.Join(" | ", columns.Select((col, i) => ":".PadRight(colWidths[i], '-')))
			);

			// row
			foreach (var row in rows)
			{
				str.AppendLine(
					string.Join(" | ", row.ItemArray.Select((field, i) => $"{field}".PadRight(colWidths[i]))).TrimEnd()
				);
			}

			return str.ToString();
		}

		/// <summary>The raw info for a mod folder containing translations.</summary>
		/// <param name="ModDir">The directory containing the mod files.</param>
		/// <param name="RelativePath">The mod's folder path relative to the solution directory.</param>
		/// <param name="ModName">The display name for the mod.</param>
		private record ModFolder(DirectoryInfo ModDir, string RelativePath, string ModName)
		{
			/// <summary>The directories containing translation files for this mod.</summary>
			public List<DirectoryInfo> TranslationDirs { get; } = new();

			/// <summary>The translation status for each language indexed by subpath and then locale.</summary>
			public Dictionary<string, Dictionary<string, TranslationStatus>> TranslationStatus { get; } = new();

			/// <summary>The validation warnings for issues like missing or unknown keys.</summary>
			public HashSet<string> Warnings { get; } = new();

			/// <summary>Get the statuses by relative folder path for the given locale.</summary>
			/// <param name="locale">The locale code to match.</param>
			public IDictionary<string, TranslationStatus> GetStatusForLocale(string locale)
			{
				Dictionary<string, TranslationStatus> result = new();

				foreach ((string relativePath, IDictionary<string, TranslationStatus> statuses) in this.TranslationStatus)
				{
					result[relativePath] = 
						statuses.TryGetValue(locale, out TranslationStatus status)
						? status
						: CreateTranslationSummary.TranslationStatus.Missing;
				}

				return result;
			}
		};

		/// <summary>Display info about a mod language.</summary>
		/// <param name="Name">The display name for the language.</param>
		/// <param name="Required">Whether to mark this language as missing if none of the mods have a translation for it.</param>
		/// <param name="Url">The URL to link for the language name in translation tables.</param>
		private record ModLanguage(string Name, bool Required = true, string Url = null);

		/// <summary>The minimal model for a <c>manifest.json</c> file.</summary>
		/// <param name="Name">The mod's display name.</param>
		private record Manifest(string Name);

		/// <summary>The layout to use for the translation summary table.</summary>
		private enum TableStyle
		{
			/// <summary>Automatically choose <see cref="RowPerLocale"/> or <see cref="RowPerMod"/> so there are more rows than columns.</summary>
			Auto,

			/// <summary>One row per locale, with the mod names in the column header.</summary>
			RowPerLocale,

			/// <summary>One row per mod, with the locale names in the column header.</summary>
			RowPerMod
		}

		/// <summary>The status of translations for a locale.</summary>
		private enum TranslationStatus
		{
			/// <summary>The translation file is missing entirely.</summary>
			Missing,

			/// <summary>The translation file is present, but has some untranslated keys.</summary>
			Incomplete,

			/// <summary>The translation file is fully translated.</summary>
			Complete
		};
	}
}
