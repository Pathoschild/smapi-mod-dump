/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace SpecialOrdersExtended.DataModels;

/// <summary>
/// Base data model class.
/// </summary>
public abstract class AbstractDataModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractDataModel"/> class.
    /// </summary>
    /// <param name="savefile">String that represents the savefile name.</param>
    /// <remarks>Savefile name is farmname + unique ID in 1.5+.</remarks>
    public AbstractDataModel(string savefile) => this.Savefile = savefile;

    /// <summary>
    /// Gets or sets string that represents the savefile name.
    /// </summary>
    /// <remarks>Savefile name is farmname + unique ID in 1.5+.</remarks>
    public virtual string Savefile { get; set; }

    /// <summary>
    /// Handles saving.
    /// </summary>
    /// <param name="identifier">An identifier token to add to the filename.</param>
    internal virtual void Save(string identifier)
    {
        Task.Run(() => ModEntry.DataHelper.WriteGlobalData(this.Savefile + identifier, this))
            .ContinueWith((t) => ModEntry.ModMonitor.Log(t.Status == TaskStatus.RanToCompletion ? $"Saved {identifier}" : $"{identifier} failed to save with {t.Status} - {t.Exception}"));
    }

    /// <summary>
    /// A way to save a temporary file.
    /// </summary>
    /// <param name="identifier">An identifier token to add to the filename.</param>
    /// <remarks>NOT IMPLEMENTED YET.</remarks>
    internal virtual void SaveTemp(string identifier) => this.Save($"{identifier}_temp_{SDate.Now().DaysSinceStart}");
}
