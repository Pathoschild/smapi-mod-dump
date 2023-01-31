/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.MigrationManager;

/// <summary>
/// Class to faciliate handling migrations.
/// </summary>
public class MigrationManager
{
    private const string FILENAME = "migration_data";
    private string? oldversion;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationManager"/> class, which is used to handle migrations.
    /// </summary>
    /// <param name="manifest">The mod's manifest.</param>
    /// <param name="helper">SMAPI's helper.</param>
    /// <param name="monitor">The mod's logger.</param>
    public MigrationManager(IManifest manifest, IModHelper helper, IMonitor monitor)
    {
        this.Manifest = manifest;
        this.DataHelper = helper.Data;
        this.Helper = helper;
        this.Monitor = monitor;
    }

    /// <summary>
    /// Gets the mod's manifest.
    /// </summary>
    protected IManifest Manifest { get; init; }

    /// <summary>
    /// Gets the data helper.
    /// </summary>
    protected IDataHelper DataHelper { get; init; }

    /// <summary>
    /// Gets SMAPI's helper.
    /// </summary>
    protected IModHelper Helper { get; init; }

    /// <summary>
    /// Gets logger.
    /// </summary>
    protected IMonitor Monitor { get; init; }

    /// <summary>
    /// Reads the version info from global data.
    /// </summary>
    /// <returns>true if the version is the same as previous, false otherwise.</returns>
    public bool CheckVersionInfo()
    {
        if (this.DataHelper.ReadGlobalData<MigrationDataClass>(FILENAME) is MigrationDataClass migrationData
            && migrationData.VersionMap.TryGetValue(Constants.SaveFolderName!, out this.oldversion))
        {
            this.Monitor.Log($"Migrator found old version {this.oldversion} for {this.Manifest.UniqueID} - current is {this.Manifest.Version}", LogLevel.Trace);
            return this.Manifest.Version.ToString() == this.oldversion;
        }
        else
        {
            this.Monitor.Log($"{this.Manifest.UniqueID} not used previously on this save.", LogLevel.Trace);
            return false;
        }
    }

    /// <summary>
    /// Writes the version info into global data.
    /// </summary>
    public void SaveVersionInfo()
    {
        Task.Run(() =>
        {
            MigrationDataClass migrationData = this.DataHelper.ReadGlobalData<MigrationDataClass>(FILENAME) ?? new();
            this.Monitor.Log($"Writing version info {this.Manifest.Version} into global data for {Constants.SaveFolderName} for {this.Manifest.UniqueID}", LogLevel.Trace);
            migrationData.VersionMap[Constants.SaveFolderName!] = this.Manifest.Version.ToString();
            this.DataHelper.WriteGlobalData(FILENAME, migrationData);
        })
        .ContinueWith((t) => this.Monitor.Log(t.Status == TaskStatus.RanToCompletion ? "Migration data saved successfully!" : $"Migration data failed to save {t.Status}"));
    }

    /// <summary>
    /// Runs a specific migration.
    /// </summary>
    /// <param name="version">The <see cref="ISemanticVersion"/> for which this migration belongs.</param>
    /// <param name="migration">A function that runs the migration.</param>
    /// <returns>True if successfully run (or not relevant), false otherwise.</returns>
    public bool RunMigration(ISemanticVersion version, Func<IModHelper, IMonitor, bool> migration)
    {
        if (this.oldversion is null)
        {
            return false;
        }
        else if (!version.IsOlderThan(this.oldversion))
        {
            return migration.Invoke(this.Helper, this.Monitor);
        }
        else
        {
            return true;
        }
    }
}