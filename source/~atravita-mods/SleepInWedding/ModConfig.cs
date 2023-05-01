/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Integrations.GMCMAttributes;

namespace SleepInWedding;

/// <summary>
/// The config class for this mod.
/// </summary>
internal sealed class ModConfig
{
    private int weddingTime = 800;

    /// <summary>
    /// Gets or sets when the wedding should begin.
    /// </summary>
    [GMCMInterval(10)]
    [GMCMRange(600, 2600)]
    public int WeddingTime
    {
        get => this.weddingTime;
        set
        {
            int hour = Math.DivRem(value, 100, out int minute);
            this.weddingTime = (Math.Clamp(hour, 0, 26) * 100) + Math.Clamp(minute, 0, 60);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not we should try to
    /// set the wedding on save load.
    /// </summary>
    public bool TryRecoverWedding { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the wedding should play when the player
    /// enters town, even if it's not "time yet".
    /// </summary>
    public bool WedWhenEnteringTown { get; set; } = true;
}
