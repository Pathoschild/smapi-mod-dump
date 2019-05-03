<Query Kind="Program">
  <Reference Relative="..\..\SMAPI\bin\Debug\SMAPI.Toolkit\netstandard2.0\SMAPI.Toolkit.CoreInterfaces.dll">C:\source\_Stardew\SMAPI\bin\Debug\SMAPI.Toolkit\netstandard2.0\SMAPI.Toolkit.CoreInterfaces.dll</Reference>
  <Reference Relative="..\..\SMAPI\bin\Debug\SMAPI.Toolkit\netstandard2.0\SMAPI.Toolkit.dll">C:\source\_Stardew\SMAPI\bin\Debug\SMAPI.Toolkit\netstandard2.0\SMAPI.Toolkit.dll</Reference>
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference Prerelease="true">Pathoschild.FluentNexus</NuGetReference>
  <NuGetReference>Squid-Box.SevenZipSharp</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Pathoschild.FluentNexus</Namespace>
  <Namespace>Pathoschild.FluentNexus.Models</Namespace>
  <Namespace>Pathoschild.Http.Client</Namespace>
  <Namespace>SevenZip</Namespace>
  <Namespace>StardewModdingAPI.Toolkit</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.Wiki</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModScanning</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

/*

  This script...
     1. fetches metadata and files for every Stardew Valley mod on Nexus;
	 2. unpacks the downloaded files;
	 3. parses the unpacked downloads;
	 4. optionally runs custom queries over the metadata & downloads.

*/

/*********
** Fields
*********/
/// <summary>The Nexus API key.</summary>
readonly string ApiKey = "";

/// <summary>The path in which to store cached data.</summary>
readonly string RootPath = @"C:\dev\nexus";

/// <summary>Which mods to refetch from Nexus (or <c>null</c> to not refetch any).</summary>
readonly ISelectStrategy FetchMods =
	null;
	//new FetchAllFromStrategy(startFrom: 3855);
	//new FetchUpdatedStrategy("1d"); // 1d, 1w, 1m, or a custom date up to 28 days ago

/// <summary>Whether to delete the unpacked folder and unpack files from the export path.</summary>
readonly bool ResetUnpacked = false;


/*********
** Script
*********/
async Task Main()
{
	Directory.CreateDirectory(this.RootPath);
	NexusClient nexus = new NexusClient(this.ApiKey);

	// fetch mods from Nexus API
	HashSet<int> unpackMods = new HashSet<int>();
	if (this.FetchMods != null)
		unpackMods = new HashSet<int>(await this.ImportMods(apiKey: this.ApiKey, gameKey: "stardewvalley", fetchStrategy: this.FetchMods));

	// unpack fetched files
	this.UnpackMods(rootPath: this.RootPath, filter: id => this.ResetUnpacked || unpackMods.Contains(id));

	// run analysis
	ModToolkit toolkit = new ModToolkit();
	WikiModList compatList = await toolkit.GetWikiCompatibilityListAsync();
	HashSet<string> knownModIDs = new HashSet<string>(compatList.Mods.SelectMany(p => p.ID), StringComparer.InvariantCultureIgnoreCase);

	// mods not listed on the wiki
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
		
		// not worth tracking
		"shuaiz.SaveAnywhereV3"    // fork reposted without permission
	}, StringComparer.InvariantCultureIgnoreCase);
	(
		from mod in this.ReadMods(this.RootPath)
		let notOnWiki = mod.ModFolders
			.Where(folder => 
				folder.ModType == ModType.Smapi
				&& !string.IsNullOrWhiteSpace(folder.ModID)
				&& !knownModIDs.Contains(folder.ModID)
				&& !ignoreModIDs.Contains(folder.ModID)
			)
			.ToArray()
		where notOnWiki.Any()
		select new { mod, notOnWiki }
	)
	.Dump("SMAPI mod files not listed on the wiki");
}

/// <summary>Import data for matching mods.</summary>
/// <param name="apiKey">The Nexus API key.</param>
/// <returns>Returns the imported mod IDs.</returns>
async Task<int[]> ImportMods(string apiKey, string gameKey, ISelectStrategy fetchStrategy)
{
	NexusClient nexus = new NexusClient(apiKey);
	
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
		await this.ImportMod(nexus, gameKey, id, fetchStrategy, this.RootPath);
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
					Uri downloadUri = (await nexus.ModFiles.GetDownloadLinks(gameKey, id, file.FileID)).First().Uri;

					FileInfo localFile = new FileInfo(Path.Combine(folder.FullName, "files", $"{file.FileID}{Path.GetExtension(file.FileName)}"));
					localFile.Directory.Create();
					downloader.DownloadFile(downloadUri, localFile.FullName);
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