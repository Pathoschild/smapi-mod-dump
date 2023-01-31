/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations.GMCMAttributes;
using Microsoft.Xna.Framework;

namespace HighlightEmptyMachines;

/// <summary>
/// The config class for this mod.
/// </summary>
public sealed class ModConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModConfig"/> class.
    /// </summary>
    public ModConfig()
    {
        this.VanillaMachines = new();
        foreach (VanillaMachinesEnum machine in Enum.GetValues<VanillaMachinesEnum>())
        {
            this.VanillaMachines.Add(machine, true);
        }
        this.VanillaMachines[VanillaMachinesEnum.SeedMaker] = false;
        this.VanillaMachines[VanillaMachinesEnum.RecyclingMachine] = false;
        this.VanillaMachines[VanillaMachinesEnum.CharcoalKiln] = false;
        this.VanillaMachines[VanillaMachinesEnum.Incubator] = false;
        this.VanillaMachines[VanillaMachinesEnum.SlimeIncubator] = false;
        this.VanillaMachines[VanillaMachinesEnum.OstrichIncubator] = false;

        // Set invalid to be just a little transparent.
        Color invalid = Color.Gray;
        invalid.A = 200;
        this.InvalidColor = invalid;
    }

    /// <summary>
    /// Gets or sets the color to color empty machines.
    /// </summary>
    [GMCMDefaultColor(255, 0, 0, 255)]
    public Color EmptyColor { get; set; } = Color.Red;

    /// <summary>
    /// Gets or sets the color to color invalid machines.
    /// </summary>
    [GMCMDefaultColor(128, 128, 128, 200)]
    public Color InvalidColor { get; set; } = Color.Gray;

    /// <summary>
    /// Gets or sets a mapping that sets whether coloration of vanilla machines should be enabled.
    /// </summary>
    [GMCMDefaultIgnore]
    public Dictionary<VanillaMachinesEnum, bool> VanillaMachines { get; set; }

    /// <summary>
    /// Gets or sets a mapping that sets whether coloration of PFM machines should be enabled.
    /// </summary>
    [GMCMDefaultIgnore]
    public Dictionary<string, bool> ProducerFrameworkModMachines { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether or not machine pulsing should be disabled.
    /// </summary>
    public bool DisablePulsing { get; set; } = false;
}