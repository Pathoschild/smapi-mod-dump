<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\StardewModdingAPI.Toolkit.CoreInterfaces.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\StardewModdingAPI.Toolkit.dll</Reference>
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <NuGetReference>Pathoschild.Http.FluentClient</NuGetReference>
  <Namespace>StardewModdingAPI</Namespace>
  <Namespace>StardewModdingAPI.Toolkit</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.WebApi</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.Wiki</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModData</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModScanning</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.UpdateData</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Serialisation</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Serialisation.Models</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Utilities</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

/*********
** Configuration
*********/
/// <summary>The absolute path to the folder which contains the Git repositories.</summary>
private readonly string RootPath = @"C:\source\_Stardew\_smapi-mod-dump\source";

/// <summary>Patterns matching valid file or folder names that are legitimately part of the Git repository, but should be removed from the cloned repositories.</summary>
private readonly Regex[] IgnoreLegitNames =
{
	// folders
	new Regex(@"^\.git$", RegexOptions.Compiled),
	
	// files
	new Regex(@"^\.gitattributes$", RegexOptions.Compiled),
	new Regex(@"^\.gitignore$", RegexOptions.Compiled)
};

/// <summary>Patterns matching valid file or folder names that shouldn't be in Git.</summary>
private readonly Regex[] IgnoreIncorrectNames =
{
	// folders
	new Regex(@"^_releases$", RegexOptions.Compiled),
	new Regex(@"^\.vs$", RegexOptions.Compiled),
	new Regex(@"^bin$", RegexOptions.Compiled),
	new Regex(@"^obj$", RegexOptions.Compiled),
	new Regex(@"^packages$", RegexOptions.Compiled),
	
	// files
	new Regex(@"\.csproj\.user$", RegexOptions.Compiled),
	new Regex(@"\.DotSettings\.user$", RegexOptions.Compiled),
	new Regex(@"\.userprefs$", RegexOptions.Compiled),
	new Regex(@"_(?:BACKUP|BASE|LOCAL)_\d+\.[a-z]+", RegexOptions.Compiled), // merge backups

	// mod release files
	new Regex(@"^AutoGate\.zip$", RegexOptions.Compiled), // AutoGate
	new Regex(@"^Demo\.gif$", RegexOptions.Compiled), // StackSplitX (10MB file)
	new Regex(@"^(?:OmegasisCore|StarDustCore|MusicNameSeeker)\.zip", RegexOptions.Compiled), // ~janavarro95
	new Regex(@"^Portraits_Einari_wFixes_ContentPatcher\.zip$", RegexOptions.Compiled), // ~Drynwynn
	new Regex(@"^Release$", RegexOptions.Compiled), // Birthday Mail, Faster Run
	new Regex(@"^SMAPIHealthBarMod.+\.zip$", RegexOptions.Compiled), // Enemy Health Bars
	new Regex(@"^SMAPISprinklerMod.+\.zip$", RegexOptions.Compiled), // Better Sprinklers
	new Regex(@"^(?:SMAPIChestLabelSystem.+\.zip|zip\.exe)$", RegexOptions.Compiled), // Chest Label System
};

/// <summary>The source URLs to skip when cloning repositories. This should match the GitHub repository name or custom URL specified on the wiki.</summary>
private readonly HashSet<string> IgnoreSourceUrls = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
{
	// SMAPI
	"Pathoschild/SMAPI",
	
	// bypass wiki cache
	"alexnoddings/StardewMods"
};


/*********
** Script
*********/
async Task Main()
{
	/****
	** Initialise
	****/
	var toolkit = new ModToolkit();
	var rootDir = new DirectoryInfo(this.RootPath);
	rootDir.Create();


	/****
	** Fetch Git URLs
	****/
	List<ModRepository> repos = new List<ModRepository>();
	Console.WriteLine("Fetching Git repository URLs...");
	{
		// fetch mods
		WikiCompatibilityEntry[] mods = await toolkit.GetWikiCompatibilityListAsync();
		int totalMods = mods.Length;
		mods = mods
			.Where(mod => !this.IgnoreSourceUrls.Contains(mod.GitHubRepo) && !this.IgnoreSourceUrls.Contains(mod.CustomSourceUrl))
			.ToArray();
		
		// fetch repositories
		repos.AddRange(
			from mod in mods
			let gitUrl = ModRepository.GetGitUrl(mod)
			where gitUrl != null
			group mod by gitUrl into modGroup
			select new ModRepository(modGroup.Key, modGroup)
		);

		// find invalid custom source URLs
		string[] invalidUrls = mods
			.Except(repos.SelectMany(p => p.Mods))
			.Where(mod => !string.IsNullOrWhiteSpace(mod.CustomSourceUrl))
			.Select(mod => mod.CustomSourceUrl)
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.ToArray();

		// print stats
		int uniqueRepos = repos.Count;
		int haveCode = repos.SelectMany(repo => repo.Mods).Count();
		int haveSharedRepo = haveCode - uniqueRepos;

		Console.WriteLine($"   Found {totalMods} mods with {uniqueRepos} Git repos. {haveCode} mods ({this.GetPercentage(haveCode, totalMods)}) have a Git repo; {haveSharedRepo} repos ({this.GetPercentage(haveSharedRepo, haveCode)}) contain multiple mods.");
		if (invalidUrls.Any())
		{
			Console.WriteLine($"   Found {invalidUrls.Length} unsupported source URLs on the wiki:");
			foreach (string url in invalidUrls.OrderBy(p => p))
				Console.WriteLine($"      {url}");
		}
	}
	Console.WriteLine();

	/****
	** Generate folder names
	****/
	IDictionary<string, ModRepository> repoFolders = new Dictionary<string, ModRepository>();
	foreach (ModRepository repo in repos)
	{
		string folderName = repo.GetRecommendedFolderName();
		if (repoFolders.ContainsKey(folderName))
			throw new InvalidOperationException($"Folder name conflict: can't add {folderName}, it matches both [{repo.GitUrl}] and [{repoFolders[folderName].GitUrl}].");
		repoFolders[folderName] = repo;
	}

	/****
	** Clear old repos
	****/
	if (rootDir.EnumerateFileSystemInfos().Any())
	{
		Console.WriteLine($"Deleting old Git repositories...");
		foreach (FileSystemInfo entry in rootDir.EnumerateFileSystemInfos())
			this.Delete(entry);
		Console.WriteLine();
	}

	/****
	** Clone repos
	****/
	Console.WriteLine("Fetching Git repositories...");
	foreach (var entry in repoFolders.OrderBy(p => p.Key))
	{
		// collect info
		DirectoryInfo dir = new DirectoryInfo(Path.Combine(this.RootPath, entry.Key));
		ModRepository repo = entry.Value;
		Console.WriteLine($"   {dir.Name} → {repo.GitUrl}...");

		// validate
		if (dir.Exists)
		{
			Console.WriteLine($"   ERROR: directory already exists.");
			continue;
		}

		// clone repo
		await this.ExecuteShellAsync("git", $"clone -q {repo.GitUrl} \"{dir.Name}\"", workingDir: rootDir.FullName);

		// write latest commit
		string lastCommit = await this.ExecuteShellAsync("git", "log -1", workingDir: dir.FullName);
		File.WriteAllText(
			Path.Combine(dir.FullName, "_metadata.txt"),
			$"url:\n   {repo.GitUrl}\n\n"
			+ $"mods:\n   {string.Join("\n   ", repo.Mods.Select(p => p.Name).OrderBy(p => p))}\n\n"
			+ $"latest commit:\n   {string.Join("\n   ", lastCommit.Replace("\r", "").Split('\n'))}"
		);
		
		// clean up
		var logDeletedEntries = this
			.RecursivelyDeleteMatches(dir, this.IgnoreLegitNames.Concat(this.IgnoreIncorrectNames).ToArray())
			.Where(deleted => !this.IgnoreLegitNames.Any(pattern => pattern.IsMatch(deleted.Name)))
			.ToArray();
		if (logDeletedEntries.Any())
			Console.WriteLine($"      deleted: {string.Join(", ", logDeletedEntries.Select(p => p.FullName.Substring(dir.FullName.Length + 1)))}.");
	}
	Console.WriteLine();

	Console.WriteLine("Done!");
}

/*********
** Private methods
*********/
/// <summary>Get a percentage string for display.</summary>
/// <param name="amount">The actual amount.</param>
/// <param name="total">The total possible amount.</param>
private string GetPercentage(int amount, int total)
{
	return $"{Math.Round(amount / (total * 1m) * 100)}%";
}

/// <summary>Execute an arbitrary shell command.</summary>
/// <param name="filename">The command filename to execute.</param>
/// <param name="arguments">The command arguments to execute.</summary>
/// <param name="workingDir">The working directory in which to execute the command.</param>
private async Task<string> ExecuteShellAsync(string filename, string arguments, string workingDir)
{
	string stdOut = null;
	string errorOut = null;
	try
	{
		var command = new ProcessStartInfo
		{
			FileName = filename,
			Arguments = arguments,
			WorkingDirectory = workingDir,
			CreateNoWindow = true,
			RedirectStandardError = true,
			RedirectStandardOutput = true,
			UseShellExecute = false
		};

		using (Process process = new Process { StartInfo = command })
		{
			process.Start();
			stdOut = await process.StandardOutput.ReadToEndAsync();
			errorOut = await process.StandardError.ReadToEndAsync();
			process.WaitForExit();

			if (!string.IsNullOrWhiteSpace(errorOut))
				throw new Exception($"The shell returned an error message.\nCommand: {filename} {arguments}\nError: {errorOut}");
			return stdOut;
		}
	}
	catch
	{
		new { command = $"{filename} {arguments}", workingDir, stdOut, errorOut }.Dump("shell error");
		throw;
	}
}

/// <summary>Recursively delete any files or folders which match one of the provided patterns.</summary>
/// <param name="entry">The file or root directory to check.</param>
/// <param name="namePatterns">The regex patterns matching file or directory names to delete.</param>
/// <returns>Returns the deleted files and directories.</returns>
private IEnumerable<FileSystemInfo> RecursivelyDeleteMatches(FileSystemInfo entry, Regex[] namePatterns)
{
	// delete if matched
	if (namePatterns.Any(p => p.IsMatch(entry.Name)))
	{
		this.Delete(entry);
		yield return entry;
	}

	// check subentries
	else if (entry is DirectoryInfo dir)
	{
		foreach (FileSystemInfo child in dir.GetFileSystemInfos())
		{
			foreach (FileSystemInfo deletedEntry in this.RecursivelyDeleteMatches(child, namePatterns))
				yield return deletedEntry;
		}
	}
}

/// <summary>Delete the given subfolder, handling permission issues along the way.</summary>
/// <param name="entry">The directory or file to delete.</param>
public void Delete(FileSystemInfo entry)
{
	if (!entry.Exists)
		return;

	// delete subentries
	if (entry is DirectoryInfo dir)
	{
		foreach (FileSystemInfo child in dir.GetFileSystemInfos())
			this.Delete(child);
	}

	// delete current
	entry.Attributes = FileAttributes.Normal; // clear readonly flag
	entry.Delete();
}

/// <summary>Metadata about a mod repository.</summary>
class ModRepository
{
	/*********
	** Accessors
	*********/
	/// <summary>The repository's Git URL.</summary>
	public string GitUrl { get; }

	/// <summary>The mod's wiki metadata.</summary>
	public WikiCompatibilityEntry[] Mods { get; }

	/// <summary>The mods' custom source URLs, if specified.</summary>
	public string[] CustomSourceUrls { get; }

	/// <summary>The repository owner name.</summary>
	public string RepositoryOwner { get; }

	/// <summary>The repository name.</summary>
	public string RepositoryName { get; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="gitUrl">The git URL.</param>
	/// <param name="mods">The mods in the repository.</param>
	public ModRepository(string gitUrl, IEnumerable<WikiCompatibilityEntry> mods)
	{
		this.GitUrl = gitUrl;
		this.Mods = mods.ToArray();
		this.CustomSourceUrls = (from mod in mods where !string.IsNullOrWhiteSpace(mod.CustomSourceUrl) select mod.CustomSourceUrl).ToArray();

		if (this.TryParseRepositoryUrl(gitUrl, out string owner, out string name))
		{
			this.RepositoryOwner = owner;
			this.RepositoryName = name;
		}
	}

	/// <summary>Get the recommended folder name for the repository.</summary>
	public string GetRecommendedFolderName()
	{
		string name = this.Mods.Length == 1
			? this.Mods.Single().Name
			: $"~{this.RepositoryOwner}";
		
		foreach (char invalidCh in Path.GetInvalidFileNameChars())
			name = name.Replace(invalidCh, '_');
		
		return name;
	}


	/*********
	** Private methods
	*********/
	/// <summary>Get the Git URL for a mod entry, if any.</summary>
	/// <param name="mod">The mod's wiki metadata.</param>
	public static string GetGitUrl(WikiCompatibilityEntry mod)
	{
		if (!string.IsNullOrWhiteSpace(mod.GitHubRepo))
			return $"https://github.com/{mod.GitHubRepo.Trim('/')}.git";

		if (mod.CustomSourceUrl != null)
		{
			if (mod.CustomSourceUrl.Contains("gitlab.com"))
				return $"{mod.CustomSourceUrl}.git";
		}

		return null;
	}

	/// <summary>Parse a Git URL.</param>
	/// <param name="url">The Git URL to parse.</summary>
	/// <param name="owner">The parsed repository owner.</param>
	/// <param name="name">The parsed repository name.</param>
	private bool TryParseRepositoryUrl(string url, out string owner, out string name)
	{
		if (!string.IsNullOrWhiteSpace(url))
		{
			var match = Regex.Match(url.Trim(), "^https://git(?:hub|lab).com/([^/]+)/([^/]+).git");
			if (match.Success)
			{
				owner = match.Groups[1].Value;
				name = match.Groups[2].Value;
				return true;
			}
		}

		// invalid
		owner = null;
		name = null;
		return false;
	}
}