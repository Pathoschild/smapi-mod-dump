<Query Kind="Program">
  <Reference Relative="..\..\SMAPI\bin\Debug\SMAPI.Toolkit\netstandard2.0\SMAPI.Toolkit.CoreInterfaces.dll">C:\source\_Stardew\SMAPI\bin\Debug\SMAPI.Toolkit\netstandard2.0\SMAPI.Toolkit.CoreInterfaces.dll</Reference>
  <Reference Relative="..\..\SMAPI\bin\Debug\SMAPI.Toolkit\netstandard2.0\SMAPI.Toolkit.dll">C:\source\_Stardew\SMAPI\bin\Debug\SMAPI.Toolkit\netstandard2.0\SMAPI.Toolkit.dll</Reference>
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference Prerelease="true">Pathoschild.FluentNexus</NuGetReference>
  <NuGetReference>Squid-Box.SevenZipSharp</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Pathoschild.FluentNexus</Namespace>
  <Namespace>Pathoschild.FluentNexus.Models</Namespace>
  <Namespace>Pathoschild.Http.Client</Namespace>
  <Namespace>SevenZip</Namespace>
  <Namespace>StardewModdingAPI.Toolkit</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.Wiki</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModScanning</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Serialisation</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Serialisation.Models</Namespace>
  <Namespace>StardewModdingAPI</Namespace>
</Query>

/*

  This script...
     1. fetches metadata and files for every Stardew Valley mod on Nexus;
	 2. unpacks the downloaded files;
	 3. parses the unpacked downloads;
	 4. optionally runs custom queries over the metadata & downloads.

*/

/*********
** Configuration
*********/
/// <summary>The Nexus API key.</summary>
readonly string ApiKey = "";

/// <summary>The path in which to store cached data.</summary>
readonly string RootPath = @"C:\dev\nexus";

/// <summary>Which mods to refetch from Nexus (or <c>null</c> to not refetch any).</summary>
readonly ISelectStrategy FetchMods =
	null;
	//new FetchAllFromStrategy(startFrom: 3792);
	//new FetchUpdatedStrategy(TimeSpan.FromDays(3));
	//new FetchUpdatedStrategy("1d"); // "1d", "1w", "1m", or a custom timespan/date up to 28 days ago

/// <summary>Whether to delete the entire unpacked folder and unpack all files from the export path. If this is false, only updated mods will be re-unpacked.</summary>
readonly bool ResetUnpacked = false;


/*********
** Script
*********/
async Task Main()
{
	Directory.CreateDirectory(this.RootPath);
	NexusClient nexus = new NexusClient(this.ApiKey, "Pathoschild", "1.0.0");

	// fetch mods from Nexus API
	HashSet<int> unpackMods = new HashSet<int>();
	if (this.FetchMods != null)
		unpackMods = new HashSet<int>(await this.ImportMods(nexus, gameKey: "stardewvalley", fetchStrategy: this.FetchMods, rootPath: this.RootPath));

	// unpack fetched files
	this.UnpackMods(rootPath: this.RootPath, filter: id => this.ResetUnpacked || unpackMods.Contains(id));

	// run analysis
	ParsedModData[] mods = this.ReadMods(this.RootPath).ToArray();
	await this.GetModsNotOnWiki(mods).Dump("SMAPI mods not on the wiki");
	await this.GetInvalidMods(mods).Dump("Mods marked invalid by SMAPI toolkit (except blacklist)");
	//this.GetModsDependentOn(mods, "Entoarox.EntoaroxFramework").Dump("Dependent mods");
	//await this.CustomContentPatcherQuery(mods).Dump("Custom CP query");
}

/*********
** Common queries
*********/
/// <summary>Get all mods which depend on the given mod.</summary>
/// <param name="parsedMods">The mods to check.</param>
/// <param name="modID">The dependency mod ID.</param>
IEnumerable<ModFolder> GetModsDependentOn(IEnumerable<ParsedModData> parsedMods, string modID)
{
	foreach (ParsedModData mod in parsedMods)
	{
		foreach (ModFolder folder in mod.ModFolders.Select(p => p.RawFolder.Value))
		{
			bool dependent =
				folder.Manifest?.Dependencies?.Any(p => p.UniqueID?.Equals("Entoarox.EntoaroxFramework", StringComparison.InvariantCultureIgnoreCase) == true) == true
				|| folder.Manifest?.ContentPackFor?.UniqueID?.Equals("Entoarox.EntoaroxFramework", StringComparison.InvariantCultureIgnoreCase) == true;
			if (dependent)
				yield return folder;
		}
	}
}

/// <summary>Placeholder for custom Content Patcher pack queries.</summary>
/// <param name="parsedMods">The mods to check.</param>
async Task<dynamic> CustomContentPatcherQuery(IEnumerable<ParsedModData> parsedMods)
{
	IDictionary<string, int> formatVersions = new Dictionary<string, int>();
	
	JToken GetProperty(JToken parent, string name)
	{
		if (parent is JObject obj)
			return obj.GetValue(name, StringComparison.InvariantCultureIgnoreCase);
		throw new InvalidOperationException($"Can't get property '{name}' for non-object token (found {parent.Type} at {parent.Path}).");
	}
	
	List<string> results = new List<string>();
	JsonHelper jsonHelper = new JsonHelper();
	foreach (ParsedModData modEntry in parsedMods)
	{
		foreach (ParsedFileData download in modEntry.ModFolders)
		{
			// get Content Patcher mod
			var mod = download.RawFolder.Value;
			if (mod.Type != ModType.ContentPack || !"Pathoschild.ContentPatcher".Equals(mod.Manifest.ContentPackFor.UniqueID, StringComparison.InvariantCultureIgnoreCase))
				continue;

			// find content.json
			FileInfo contentFile = new FileInfo(Path.Combine(mod.Directory.FullName, "content.json"));
			if (!contentFile.Exists)
			{
				Helper.Print($"Ignored CP pack {modEntry.ID} > {download.FileID}: no content.json found.", Severity.Error);
				continue;
			}

			// parse content.json
			CPContentModel content;
			try
			{
				content = jsonHelper.Deserialise<CPContentModel>(File.ReadAllText(contentFile.FullName));
			}
			catch (Exception ex)
			{
				Helper.Print($"Ignored CP pack {modEntry.ID} > {download.FileID}: {ex.Message}.", Severity.Error);
				new Lazy<Exception>(() => ex).Dump("`--> exception");
				continue;
			}

			// get format version
			results.Add(content.Format);

			// check if any When conditions have values containing a colon
//			List<CPPatchModel> patches = new List<CPPatchModel>();
//			foreach (CPPatchModel patch in content.Changes ?? new CPPatchModel[0])
//			{
//				foreach (var entry in patch.When ?? new Dictionary<string, string>())
//				{
//					if (entry.Value.Contains(":"))
//						patches.Add(patch);
//				}
//			}
//			if (patches.Any())
//				results.Add(new { mod, content = new Lazy<CPContentModel>(() => content), patches = new Lazy<CPPatchModel[]>(() => patches.ToArray()) });
		}
	}
	
	return results.GroupBy(p => p).ToDictionary(p => p.Key, p => p.Count()).OrderByDescending(p => p.Value);
}

class CPContentModel
{
	public string Format { get; set; }
	
	public CPPatchModel[] Changes { get; set; }

	[JsonExtensionData]
	public IDictionary<string, JToken> OtherFields { get; set; }
}

class CPPatchModel
{
	public IDictionary<string, string> When { get; set; }

	[JsonExtensionData]
	public IDictionary<string, JToken> OtherFields { get; set; }
}

/// <summary>Get SMAPI mods which aren't listed on the wiki compatibility list.</summary>
/// <param name="mods">The mods to check.</param>
async Task<dynamic[]> GetModsNotOnWiki(IEnumerable<ParsedModData> mods)
{
	// set mods to ignore
	HashSet<string> ignoreModIDs = new HashSet<string>(new[]
	{
		// utility mods that are part of a larger mod
		"Demiacle.ExtraHair",
		"Entoarox.BushReset",
		"funnysnek.serverconnectionreset",
		"TNT.Village.ConsoleCommand",
		
		// legacy/broken content packs
		"katekatpixel.portraits",
		"magimatica.HudsonValley",
		"oomps62.HudsonValleySeasonalObelisks",
		
		// outdated fork reposted without permission
		"shuaiz.SaveAnywhereV3"
	}, StringComparer.InvariantCultureIgnoreCase);

	// fetch mods on the wiki
	ModToolkit toolkit = new ModToolkit();
	WikiModList compatList = await toolkit.GetWikiCompatibilityListAsync();
	HashSet<string> knownModIDs = new HashSet<string>(compatList.Mods.SelectMany(p => p.ID), StringComparer.InvariantCultureIgnoreCase);

	// fetch report
	return (
		from mod in mods
		from folder in mod.ModFolders
		where
			folder.ModType == ModType.Smapi
			&& !string.IsNullOrWhiteSpace(folder.ModID)
			&& !knownModIDs.Contains(folder.ModID)
			&& !ignoreModIDs.Contains(folder.ModID)
		let manifest = folder.RawFolder.Value.Manifest
		select new
		{
			NexusID = new Hyperlinq($"https://www.nexusmods.com/stardewvalley/mods/{mod.ID}", mod.ID.ToString()),
			NexusName = mod.Name,
			NexusAuthor = mod.Author,
			NexusVersion = SemanticVersion.TryParse(mod.Version, out ISemanticVersion nexusVersion) ? nexusVersion.ToString() : mod.Version,
			NexusStatus = mod.Status,
			folder.FileID,
			folder.FileCategory,
			folder.FileName,
			folder.ModType,
			folder.ModID,
			folder.ModVersion,
			UpdateKeys = new Lazy<string[]>(() => manifest.UpdateKeys),
			Manifest = new Lazy<Manifest>(() => manifest),
			NexusMod = new Lazy<ParsedModData>(() => mod),
			Folder = new Lazy<ParsedFileData>(() => folder),
			WikiEntry = new Lazy<string>(() =>
				"{{/entry\n"
				+ $"  | name     = {folder.ModDisplayName}\n"
				+ $"  | author   = {manifest?.Author}{(manifest?.Author != null && !manifest.Author.Trim().Equals(mod.Author?.Trim(), StringComparison.InvariantCultureIgnoreCase) ? $", {mod.Author}" : "")}\n"
				+ $"  | id       = {manifest?.UniqueID}\n"
				+ $"  | nexus id = {mod.ID}\n"
				+ $"  | github   = {manifest?.UpdateKeys?.Where(p => p.Trim().StartsWith("GitHub")).Select(p => p.Trim().Substring(6)).FirstOrDefault()}\n"
				+ $"  | 3.0 ready= yes\n"
				+ "}}"
			)
		}
	)
	.OrderBy(p => p.NexusName)
	.ToArray();
}

/// <summary>Get mods which the SMAPI toolkit marked as invalid or unparseable.</summary>
/// <param name="mods">The mods to check.</param>
async Task<dynamic[]> GetInvalidMods(IEnumerable<ParsedModData> mods)
{
	// set mod files to ignore
	// This query ensures that mods which *should* be correctly parsed by SMAPI are, so we don't want to list downloads that were already manually checked and confirmed invalid.
	HashSet<int> ignoreMods = new HashSet<int>
	{
		// non-mod tools
		3431, // BFAV JSON Update [tool]
		1080, // Easy XNB for Xnb Node
		1213, // Natural Color - Reshade
		21,   // SDVMM/Stardew Valley Mod Manager
		1022, // SDV MultiTweak
		2400, // SMAPI
		2367, // SMAPI Templates [for Visual Studio]
		782,  // Sound Modding Tools
		1298, // Stardew Editor
		3814, // Stardew Valley Hack Player for Name_Yusuf (???)
		3916, // Stardew Valley Money Hack
		3787, // Stardew Valley Planner
		2451, // StardewZem - Very Easy XNB Merger
		337,  // SVPM/Stardew Valley Package Manager
		1832, // Twelfth Night - American Gothic - ReShade
		1770, // Twelfth Night - Depixelate - ReShade
		1798, // Twelfth Night - Gameboy Pocket - ReShade
		2152  // Updated XACT file for audio modding [.xap file]
	};
	HashSet<int> ignoreFiles = new HashSet<int>
	{
		// pre-manifest SMAPI mods
		239,   // Rise and Shine (#3)
		294,   // Sprint (#2)
		456,   // Taxes Mod (#38)

		// SMAPI mods with outdated manifest formats (e.g. old version format)
		929,   // No Soil Decay (#283)
		2949,  // Siv's Marriage Mod (#366)
		3757,  // SmartMod (#1048)

		// replacement files (e.g. tbin to drop into downloaded mod)
		12282, // Ace's Expanded Farms MTN (#2711) > MelodicLullaby Less Saturated Valley Compatibility
		2051,  // Add a Room and Attic (#379)
		16992, // Bears in the Barn for BFAV (#4000) > BFAV JSON Update data file
		17296, // BFAV Dragons (#3991) > BFAV JSON Update
		16975, // BFAV Velociraptors (#4015) > Animals file to BFAV Json Update
		16979, // BFAV Cutter Animals (#4016) > Animals file to BFAV Json Update
		15896, // Capitalist Dream Farm (#3679) > SVE Compatibility
		9873,  // Even More Secret Woods (#2364), replacement file for Immersive Farm 2
		13120, // Immersive Farm 2 (#1531)
		13647, // Immersive Farm 2 (#1531)
		17338, // Oasis Greenhouse (#3969) > IF2R Version
		12634, // Phoenix Farm (#3026) > Pethouse Phoenix Farm
		12863, // Secret Gardens Greenhouse (#3067) > "" for Immersive Farm 2
		16524, // Stardew Valley Expanded (#3753)

		// legacy zipped Seasonal Immersion content packs
		5438,  // Seasonal Custom Farm Buildings (#1451)
		5439,  // Seasonal Custom Farm Buildings (#1451)
		3164,  // Seasonal Victorian Buildings and Flowers (#891)
		5688,  // Witchy Decorations (#1515)

		// legacy CustomNPC pack (files to drop into Mods/CustomNPC/Npcs)
		8179,  // Costum Npc Base (#1964)
		8203,  // Costum Npc Base (#1964)
		7569,  // CustomNPCs Nagito Komaeda (#1964)
		6423,  // NPC Alec (#1692)
		8870,  // Steins Gate Kurisu Maho and Leskinen mod (#2249)
		8871,  // Steins Gate Kurisu Maho and Leskinen mod (#2249)

		// legacy Stardew Symphony pack (files to drop into Mods/StardewSymphonyRemastered/Content/Music/Wav)
		12421, // Chill of Winter Music Pack (#3015)

		// Better Farm Animal Variety pack (files to merged into BFAV's config file)
		14395, // Gray Chicken (#3416)
		14394, // Harvest Moon Cows (#3419)
		14367, // Monster Girls (#3408)
		14365, // Yoshis (#3420)
		14366, // Zelda LTTP Lifestock Animals (#3421)

		// collections of zipped content packs
		13533, // A Less Yellow Stardew (#2415) > All Lanuage Version In One File
		17433, // A Less Yellow Stardew (#2415) > ALYSD Map update (invalid manifest)
		9295,  // Clint Narrative Overhaul (#1067)
		9297,  // Demetrius Narrative Overhaul (#1120)
		9303,  // Dwarf Narrative Overhaul (#1250)
		9299,  // Gus Narrative Overhaul (#1144)
		9307,  // Linus Narrative Overhaul (#1488)
		9301,  // Marnie Narrative Overhaul (#1192)
		9309,  // Pam Narrative Overhaul (#1978)
		9293,  // Willy Narrative Overhaul (#1047)
		9305,  // Wizard Narrative Overhaul (#1309)

		// XNB mods with non-standard files
		9634,  // Ali's Foraging Map With a Few Changes (#2381), includes redundant .zip files
		445,   // Better Pigs and Recolours (#10), collection of zipped XNB mods
		2008,  // Chickens to Cardinal or Toucan (#578), XNB mod with misnamed `White Chickenxnb`
		10040, // Hero Academia Shota Mod (#2490), includes .zip file
		4462,  // Hope's Secret Cave (#1155), includes unpacked files
		535,   // New Rabbit Sprites and Recolours (#535), collection of zipped XNB mods
		2118,  // Semi-Realistic Animal Replacer (#597), collection of zipped XNB mods
		1680,  // Simple Building Cleaner (#493), has a `ModInfo.ini` file for some reason
		15332, // Tieba Chinese Revision (#2936), has junk files to show instructions in filenames
		2224,  // Toddlers Take After Parents (#626), files misnamed with `.zip_`

		// other
		10976, // Always On Server (#2677) > AutoHotKey Paste Every 2 Minutes
		12257, // Always On Server (#2677) > Auto Restart SDV
		13516, // Battle Royalley (#3199) > World File for Hosting
		14839, // Battle Royalley (#3199), custom .bat/.command/.sh launch script
		15901, // Better Crab Pots (#3159) > Config Updater
		10352, // Birthstone Plants (#1632), JA pack with broken manifest JSON
		5721,  // Chao Replacement for Cat (#1524), .wav files
		15399, // Hidden Forest Farm (#3583) > XNB version, includes .tbin file
		14664, // Husky New NPC (#14664), has .xslx file in root with multiple content pack folders
		9967,  // Sam to Samantha (#2472), CP pack with invalid update keys
		16623, // Stardew In-Game Daily Planner > Example Plan
		16660, // Stardew In-Game Daily Planner > Example Checklist
		17288, // Stardew Valley Expanded (#3753) > Wallpapers, Event Guide and Script
		16198, // Stardew Valley Expanded (#3753), includes replacement files
		11658, // Visual Crossing Sprite Overhaul (#1942), CP pack with invalid version format
		11717, // Pencilstab's Portraits (#2351), content pack with separate previews folder including .zip
		9495,  // Quieter Cat Dog and Keg (#2371), .wav files
	};

	// fetch report
	return (
		from mod in mods
		where !ignoreMods.Contains(mod.ID)
		let invalid = mod.ModFolders
			.Where(folder => 
				(folder.ModType == ModType.Invalid || folder.ModType == ModType.Ignored)
				&& folder.ModError != ModParseError.EmptyFolder // contains only non-mod files (e.g. replacement PNG assets)
				&& !ignoreFiles.Contains(folder.RawDownload.Value.FileID)
			)
			.ToArray()
		where invalid.Any()
		select new { mod, invalid }
	)
	.ToArray();
}


/*********
** Implementation
*********/
/// <summary>Import data for matching mods.</summary>
/// <param name="apiKey">The Nexus API client.</param>
/// <param name="gameKey">The unique game key.</param>
/// <param name="fetchStrategy">The strategy which decides which mods to fetch.</param>
/// <param name="rootPath">The path in which to store cached data.</param>
/// <returns>Returns the imported mod IDs.</returns>
async Task<int[]> ImportMods(NexusClient nexus, string gameKey, ISelectStrategy fetchStrategy, string rootPath)
{
	// get mod IDs
	int[] modIDs = await fetchStrategy.GetModIds(nexus, gameKey);
	if (!modIDs.Any())
		return modIDs;
	
	// fetch mods
	var progress = new IncrementalProgressBar(modIDs.Length).Dump();
	foreach (int id in modIDs)
	{
		// update progress
		progress.Increment();
		var rateLimits = await nexus.GetRateLimits();
		progress.Caption = $"Fetching mod {id} ({progress.Percent}%)";

		// fetch
		await this.ImportMod(nexus, gameKey, id, fetchStrategy, rootPath);
	}
	
	progress.Caption = $"Fetched {modIDs.Length} updated mods ({progress.Percent}%)";
	return modIDs;
}

/// <summary>Import data for a given mod.</summary>
/// <param name="nexus">The Nexus API client.</param>
/// <param name="gameKey">The unique game key.</param>
/// <param name="id">The unique mod ID.</param>
/// <param name="selectStrategy">The strategy which decides which mods to fetch.</param>
/// <param name="rootPath">The path in which to store cached data.</param>
async Task ImportMod(NexusClient nexus, string gameKey, int id, ISelectStrategy selectStrategy, string rootPath)
{
	while (true)
	{
		try
		{
			// fetch mod data
			Mod mod;
			try
			{
				mod = await nexus.Mods.GetMod(gameKey, id);
			}
			catch (ApiException ex) when (ex.Status == HttpStatusCode.NotFound)
			{
				Helper.Print($"Skipped mod {id} (HTTP 404).", Severity.Warning);
				return;
			}
			if (!(await selectStrategy.ShouldUpdate(nexus, mod)))
			{
				Helper.Print($"Skipped mod {id} (select strategy didn't match).", Severity.Warning);
				return;
			}

			// fetch file data
			ModFile[] files = mod.Status == ModStatus.Published
				? (await nexus.ModFiles.GetModFiles(gameKey, id, FileCategory.Main, FileCategory.Optional)).Files
				: new ModFile[0];

			// reset cache folder
			DirectoryInfo folder = new DirectoryInfo(Path.Combine(rootPath, id.ToString(CultureInfo.InvariantCulture)));
			if (folder.Exists)
			{
				FileHelper.ForceDelete(folder);
				folder.Refresh();
			}
			folder.Create();

			// write mod metadata
			{
				var metadata = new ModMetadata
				{
					Updated = mod.Updated,
					Status = mod.Status,
					Mod = mod,
					Files = files
				};
				File.WriteAllText(Path.Combine(folder.FullName, "mod.json"), JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
			}
			
			// save files
			using (WebClient downloader = new WebClient())
			{
				foreach (ModFile file in files)
				{
					// create folder
					FileInfo localFile = new FileInfo(Path.Combine(folder.FullName, "files", $"{file.FileID}{Path.GetExtension(file.FileName)}"));
					localFile.Directory.Create();

					// download file from first working CDN
					Queue<ModFileDownloadLink> sources = new Queue<ModFileDownloadLink>((await nexus.ModFiles.GetDownloadLinks(gameKey, id, file.FileID)));
					while (true)
					{
						if (!sources.Any())
						{
							Helper.Print($"Skipped file {id} > {file.FileID}: no download sources available for this file.", Severity.Error);
							break;
						}

						ModFileDownloadLink source = sources.Dequeue();
						try
						{
							downloader.DownloadFile(source.Uri, localFile.FullName);
							break;
						}
						catch (Exception ex)
						{
							Helper.Print($"Failed downloading mod {id} > file {file.FileID} from {source.CdnName}.{(sources.Any() ? " Trying next CDN..." : "")}\n{ex}", Severity.Error);
						}
					}
				}
			}

			// break retry loop
			break;
		}
		catch (ApiException ex) when (ex.Status == (HttpStatusCode)429)
		{
			var rateLimits = await nexus.GetRateLimits();
			TimeSpan unblockTime = rateLimits.GetTimeUntilRenewal();
			Helper.Print($"Rate limit exhausted: {this.GetRateLimitSummary(rateLimits)}; resuming in {this.GetFormattedTime(unblockTime)} ({DateTime.Now + unblockTime} local time).");
			Thread.Sleep(unblockTime);
		}
		catch (ApiException ex)
		{
			new { error = ex, response = await ex.Response.AsString() }.Dump("error occurred");
			string choice = Helper.GetChoice("What do you want to do?", "r", "s", "a");
			if (choice == "r")
				continue; // retry
			else if (choice == "s")
				return; // skip
			else if (choice == "a")
				throw; // abort
			else
				throw new NotSupportedException($"Invalid choice: '{choice}'", ex);
		}
	}
}

/// <summary>Unpack all mods in the given folder.</summary>
/// <param name="rootPath">The path in which to store cached data.</param>
/// <param name="filter">A filter which indicates whether a mod ID should be unpacked.</param>
void UnpackMods(string rootPath, Func<int, bool> filter)
{
	SevenZipBase.SetLibraryPath(@"C:\Program Files\7-Zip\7z.dll");

	// get folders to unpack
	DirectoryInfo[] modDirs = this
		.GetSortedSubfolders(new DirectoryInfo(rootPath))
		.Where(p => filter(int.Parse(p.Name)))
		.ToArray();
	if (!modDirs.Any())
		return;

	// unpack files
	var progress = new IncrementalProgressBar(modDirs.Count()).Dump();
	foreach (DirectoryInfo modDir in modDirs)
	{
		progress.Increment();
		progress.Caption = $"Unpacking {modDir.Name} ({progress.Percent}%)...";

		// get packed folder
		DirectoryInfo packedDir = new DirectoryInfo(Path.Combine(modDir.FullName, "files"));
		if (!packedDir.Exists)
			continue;

		// create/reset unpacked folder
		DirectoryInfo unpackedDir = new DirectoryInfo(Path.Combine(modDir.FullName, "unpacked"));
		if (unpackedDir.Exists)
		{
			FileHelper.ForceDelete(unpackedDir);
			unpackedDir.Refresh();
		}
		unpackedDir.Create();

		// unzip each download
		foreach (FileInfo archiveFile in packedDir.GetFiles())
		{
			progress.Caption = $"Unpacking {modDir.Name} > {archiveFile.Name} ({progress.Percent}%)...";

			// validate
			if (archiveFile.Extension == ".exe")
			{
				Helper.Print($"  Skipped {archiveFile.FullName} (not an archive).", Severity.Error);
				continue;
			}

			// unzip into temporary folder
			string id = Path.GetFileNameWithoutExtension(archiveFile.Name);
			DirectoryInfo tempDir = new DirectoryInfo(Path.Combine(unpackedDir.FullName, "_tmp", $"{archiveFile.Name}"));
			tempDir.Create();
			try
			{
				this.ExtractFile(archiveFile, tempDir);
			}
			catch (Exception ex)
			{
				Helper.Print($"  Could not unpack {archiveFile.FullName}:\n{(ex is SevenZipArchiveException ? ex.Message : ex.ToString())}", Severity.Error);
				Console.WriteLine();
				FileHelper.ForceDelete(tempDir);
				continue;
			}
			
			// move into final location
			if (tempDir.EnumerateFiles().Any() || tempDir.EnumerateDirectories().Count() > 1) // no root folder in zip
				tempDir.Parent.MoveTo(Path.Combine(unpackedDir.FullName, id));
			else
			{
				tempDir.MoveTo(Path.Combine(unpackedDir.FullName, id));
				FileHelper.ForceDelete(new DirectoryInfo(Path.Combine(unpackedDir.FullName, "_tmp")));
			}
		}
	}
	
	progress.Caption = $"Unpacked {progress.Total} mods (100%)";
}

/// <summary>Parse unpacked mod data in the given folder.</summary>
/// <param name="rootPath">The full path to the folder containing unpacked mod files.</param>
IEnumerable<ParsedModData> ReadMods(string rootPath)
{
	ModToolkit toolkit = new ModToolkit();
	
	var modFolders = this.GetSortedSubfolders(new DirectoryInfo(rootPath)).ToArray();
	var progress = new IncrementalProgressBar(modFolders.Length).Dump();
	foreach (DirectoryInfo modFolder in modFolders)
	{
		progress.Increment();
		progress.Caption = $"Reading {modFolder.Name}...";
		
		// read metadata files
		ModMetadata metadata = JsonConvert.DeserializeObject<ModMetadata>(File.ReadAllText(Path.Combine(modFolder.FullName, "mod.json")));
		IDictionary<int, ModFile> fileMap = metadata.Files.ToDictionary(p => p.FileID);

		// load mod folders
		IDictionary<ModFile, ModFolder[]> unpackedFileFolders = new Dictionary<ModFile, ModFolder[]>();
		DirectoryInfo unpackedFolder = new DirectoryInfo(Path.Combine(modFolder.FullName, "unpacked"));
		if (unpackedFolder.Exists)
		{
			foreach (DirectoryInfo fileDir in this.GetSortedSubfolders(unpackedFolder))
			{
				progress.Caption = $"Reading {modFolder.Name} > {fileDir.Name}...";
				
				// get Nexus file data
				ModFile fileData = fileMap[int.Parse(fileDir.Name)];

				// get mod folders from toolkit
				ModFolder[] mods = toolkit.GetModFolders(rootPath: unpackedFolder.FullName, modPath: fileDir.FullName).ToArray();
				if (mods.Length == 0)
				{
					Helper.Print($"   Ignored {fileDir.FullName}, folder is empty?");
					continue;
				}

				// store metadata
				unpackedFileFolders[fileData] = mods;
			}
		}
		
		// yield mod
		yield return new ParsedModData(metadata, unpackedFileFolders);
	}
	
	progress.Caption = $"Read {progress.Total} mods (100%)";
}

/// <summary>Get the subfolders of a given folder sorted by numerical or alphabetical order.</summary>
/// <param name="root">The folder whose subfolders to get.</param>
private IEnumerable<DirectoryInfo> GetSortedSubfolders(DirectoryInfo root)
{
	return
		(
			from subfolder in root.GetDirectories()
			let isNumeric = int.TryParse(subfolder.Name, out int _)
			let numericName = isNumeric ? int.Parse(subfolder.Name) : int.MaxValue
			orderby numericName, subfolder.Name
			select subfolder
		);
}

/// <summary>Extract an archive file to the given folder.</summary>
/// <param name="file">The archive file to extract.</param>
/// <param name="extractTo">The directory to extract into.</param>
void ExtractFile(FileInfo file, DirectoryInfo extractTo)
{
	try
	{
		Task
			.Run(() =>
			{
				using (SevenZipExtractor unpacker = new SevenZipExtractor(file.FullName))
					unpacker.ExtractArchive(extractTo.FullName);
			})
			.Wait(TimeSpan.FromSeconds(60));
	}
	catch (AggregateException outerEx)
	{
		throw outerEx.InnerException;
	}
}

/// <summary>Get a human-readable formatted time span.</summary>
/// <param name="span">The time span to format.</param>
private string GetFormattedTime(TimeSpan span)
{
	int hours = (int)span.TotalHours;
	int minutes = (int)span.TotalMinutes - (hours * 60);
	return $"{hours:00}:{minutes:00}";
}

/// <summary>Get a human-readable summary for the current rate limits.</summary>
/// <param name="meta">The current rate limits.</param>
private string GetRateLimitSummary(IRateLimitManager meta)
{
	return $"{meta.DailyRemaining}/{meta.DailyLimit} daily resetting in {this.GetFormattedTime(meta.DailyReset - DateTimeOffset.UtcNow)}, {meta.HourlyRemaining}/{meta.HourlyLimit} hourly resetting in {this.GetFormattedTime(meta.HourlyReset - DateTimeOffset.UtcNow)}";
}

/// <summary>Contains parsed data about a mod page.</summary>
class ParsedModData
{
	/*********
	** Accessors
	*********/
	/// <summary>The Nexus mod name.</summary>
	public string Name { get; }

	/// <summary>The Nexus author names.</summary>
	public string Author { get; }

	/// <summary>The Nexus mod ID.</summary>
	public int ID { get; }

	/// <summary>The mod publication status.</summary>
	public ModStatus Status { get; }

	/// <summary>The mod version number.</summary>
	public string Version { get; }

	/// <summary>The parsed mod folders.</summary>
	public ParsedFileData[] ModFolders { get; }

	/// <summary>The raw mod metadata.</summary>
	public Lazy<ModMetadata> RawMod { get; }

	/// <summary>The raw mod download data.</summary>
	public Lazy<IDictionary<ModFile, ModFolder[]>> RawDownloads { get; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="mod">The raw mod metadata.</param>
	/// <param name="downloads">The raw mod download data.</param>
	public ParsedModData(ModMetadata mod, IDictionary<ModFile, ModFolder[]> downloads)
	{
		try
		{
			// set raw data
			this.RawMod = new Lazy<ModMetadata>(() => mod);
			this.RawDownloads = new Lazy<IDictionary<ModFile, ModFolder[]>>(() => downloads);

			// set mod fields
			this.Name = mod.Mod.Name;
			this.Author = mod.Mod?.User?.Name ?? mod.Mod.Author;
			if (!this.Author.Equals(mod.Mod.Author, StringComparison.InvariantCultureIgnoreCase))
				this.Author += $", {mod.Mod.Author}";
			this.ID = mod.Mod.ModID;
			this.Status = mod.Mod.Status;
			this.Version = mod.Mod.Version;

			// set mod folders
			this.ModFolders =
				(
					from entry in downloads
					from folder in entry.Value
					select new ParsedFileData(entry.Key, folder)
				)
				.ToArray();
		}
		catch (Exception)
		{
			new { mod, downloads }.Dump("failed parsing mod data");
			throw;
		}
	}
}

/// <summary>Contains parsed data about a mod download.</summary>
class ParsedFileData
{
	/*********
	** Accessors
	*********/
	/// <summary>The file ID.</summary>
	public int FileID { get; }
	
	/// <summary>The file category.</summary.
	public FileCategory FileCategory { get; }

	/// <summary>The file name on Nexus.</summary>
	public string FileName { get; }

	/// <summary>The file version on Nexus.</summary>
	public string FileVersion { get; }

	/// <summary>The mod display name based on the manifest.</summary>
	public string ModDisplayName { get; }

	/// <summary>The mod type.</summary>
	public ModType ModType { get; }

	/// <summary>The mod parse error, if it could not be parsed.</summary>
	public ModParseError? ModError { get; }

	/// <summary>The mod ID from the manifest.</summary>
	public string ModID { get; }

	/// <summary>The mod version from the manifest.</summary>
	public string ModVersion { get; }

	/// <summary>The raw mod file.</summary>
	public Lazy<ModFile> RawDownload { get; }

	/// <summary>The raw parsed mod folder.</summary>
	public Lazy<ModFolder> RawFolder { get; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="download">The raw mod file.</param>
	/// <param name="folder">The raw parsed mod folder.</param>
	public ParsedFileData(ModFile download, ModFolder folder)
	{
		// set raw data
		this.RawDownload = new Lazy<ModFile>(() => download);
		this.RawFolder = new Lazy<ModFolder>(() => folder);

		// set file fields
		this.FileID = download.FileID;
		this.FileCategory = download.Category;
		this.FileName = download.FileName;
		this.FileVersion = download.FileVersion;

		// set folder fields
		this.ModDisplayName = folder.DisplayName;
		this.ModType = folder.Type;
		this.ModError = folder.ManifestParseError == ModParseError.None ? (ModParseError?)null : folder.ManifestParseError;
		this.ModID = folder.Manifest?.UniqueID;
		this.ModVersion = folder.Manifest?.Version?.ToString();
	}
}

/// <summary>The mod metadata written for each mod.</summary>
class ModMetadata
{
	/// <summary>When the mod metadata or files were last updated.</summary>
	public DateTimeOffset Updated { get; set; }

	/// <summary>The mod publication status.</summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public ModStatus Status { get; set; }

	/// <summary>The mod data from the Nexus API.</summary>
	public Mod Mod { get; set; }

	/// <summary>The mod file metadata from the Nexus API.</summary>
	public ModFile[] Files { get; set; }
}

/// <summary>Handles the logic for deciding which mods to fetch.</summary>
interface ISelectStrategy
{
	/// <summary>Get the mod IDs to try fetching.</summary>
	/// <param name="nexus">The Nexus API client.</param>
	/// <param name="gameKey">The unique game key.</param>
	Task<int[]> GetModIds(NexusClient nexus, string gameKey);
	
	/// <summary>Get whether the given mod should be refetched, including all files.</summary>
	/// <param name="nexus">The Nexus API client.</summary>
	/// <param name="gameKey">The mod metadata.</summary>
	Task<bool> ShouldUpdate(NexusClient nexus, Mod mod);
}

/// <summary>Fetch all mods starting from a given mod ID.</summary>
public class FetchAllFromStrategy : ISelectStrategy
{
	/*********
	** Fields
	*********/
	/// <summary>The minimum mod ID to fetch.</summary>
	private int StartFrom;


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="startFrom">The minimum mod ID to fetch.</param>
	public FetchAllFromStrategy(int startFrom)
	{
		this.StartFrom = Math.Max(1, startFrom);
	}

	/// <summary>Get the mod IDs to try fetching.</summary>
	/// <param name="nexus">The Nexus API client.</param>
	/// <param name="gameKey">The unique game key.</param>
	public virtual async Task<int[]> GetModIds(NexusClient nexus, string gameKey)
	{
		int lastID = (await nexus.Mods.GetLatestAdded(gameKey)).Max(p => p.ModID);
		return Enumerable.Range(this.StartFrom, lastID - this.StartFrom + 1).ToArray();
	}

	/// <summary>Get whether the given mod should be refetched, including all files.</summary>
	/// <param name="nexus">The Nexus API client.</summary>
	/// <param name="gameKey">The mod metadata.</summary>
	public async Task<bool> ShouldUpdate(NexusClient nexus, Mod mod)
	{
		return true;
	}
}

/// <summary>Fetch mods which were updated since the given date.</summary>
public class FetchUpdatedStrategy : FetchAllFromStrategy
{
	/*********
	** Fields
	*********/
	/// <summary>The date from which to fetch mod data, or <c>null</c> for no date filter. Mods last updated before this date will be ignored.</summary>
	private DateTimeOffset StartFrom;
	
	/// <summary>The update period understood by the Nexus Mods API.</summary>
	private string UpdatePeriod;


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="startFrom">The minimum date from which to start fetching.</param>
	public FetchUpdatedStrategy(DateTimeOffset startFrom)
		: base(startFrom: 1)
	{
		// save start date
		this.StartFrom = startFrom;

		// calculate update period
		TimeSpan duration = DateTimeOffset.UtcNow - this.StartFrom;
		if (duration.TotalDays <= 1)
			this.UpdatePeriod = "1d";
		else if (duration.TotalDays <= 7)
			this.UpdatePeriod = "1w";
		else if (duration.TotalDays <= 28)
			this.UpdatePeriod = "1m";
		else
			throw new InvalidOperationException($"The given date ({this.StartFrom}) can't be used with {this.GetType().Name} because it exceeds the maximum update period for the Nexus API.");
	}

	/// <summary>Construct an instance.</summary>
	/// <param name="startFrom">The amount of time to fetch, working back from today.</param>
	public FetchUpdatedStrategy(TimeSpan startFrom)
		: this(DateTimeOffset.UtcNow.Subtract(startFrom)) { }

	/// <summary>Construct an instance.</summary>
	/// <param name="startFrom">The period to fetch. The supported values are <c>1d</c>, <c>1w</c>, or <c>1m</c>.</param>
	public FetchUpdatedStrategy(string period)
		: base(startFrom: 1)
	{
		if (period != "1d" && period != "1w" && period != "1m")
			throw new InvalidOperationException($"The given period ({period}) is not a valid value; must be '1d', '1w', or '1m'.");
		
		this.UpdatePeriod = period;
	}

	/// <summary>Get the mod IDs to try fetching.</summary>
	/// <param name="nexus">The Nexus API client.</param>
	/// <param name="gameKey">The unique game key.</param>
	public override async Task<int[]> GetModIds(NexusClient nexus, string gameKey)
	{
		List<int> modIDs = new List<int>();
		foreach (ModUpdate mod in await nexus.Mods.GetUpdated(gameKey, this.UpdatePeriod))
		{
			if (mod.LatestFileUpdate >= this.StartFrom)
				modIDs.Add(mod.ModID);
		}
		return modIDs.ToArray();
	}

	/// <summary>Get whether the given mod should be refetched, including all files.</summary>
	/// <param name="nexus">The Nexus API client.</summary>
	/// <param name="gameKey">The mod metadata.</summary>
	public async Task<bool> ShouldUpdate(NexusClient nexus, Mod mod)
	{
		return true;
	}
}
