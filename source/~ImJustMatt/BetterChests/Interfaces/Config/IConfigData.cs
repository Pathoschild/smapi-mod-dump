/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Interfaces.Config;

using StardewMods.BetterChests.Features;
using StardewMods.BetterChests.Models.Config;
using StardewMods.FuryCore.Enums;

/// <summary>
///     Mod config data.
/// </summary>
internal interface IConfigData
{
    /// <summary>
    ///     Gets or sets a value indicating how many chests containing items can be carried at once.
    /// </summary>
    public int CarryChestLimit { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether carrying a chest containing items will apply a slowness effect.
    /// </summary>
    public int CarryChestSlow { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether chests can be categorized.
    /// </summary>
    public bool CategorizeChest { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether Configurator will be enabled.
    /// </summary>
    public bool Configurator { get; set; }

    /// <summary>
    ///     Gets or sets the control scheme.
    /// </summary>
    ControlScheme ControlScheme { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="ComponentArea" /> that the <see cref="CustomColorPicker" /> will be aligned to.
    /// </summary>
    public ComponentArea CustomColorPickerArea { get; set; }

    /// <summary>
    ///     Gets or sets the default storage configuration.
    /// </summary>
    StorageData DefaultChest { get; set; }

    /// <summary>
    ///     Gets or sets the symbol used to denote context tags in searches.
    /// </summary>
    public char SearchTagSymbol { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the slot lock feature is enabled.
    /// </summary>
    public bool SlotLock { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the slot lock button needs to be held down.
    /// </summary>
    public bool SlotLockHold { get; set; }

    /// <summary>
    ///     Copies data from one <see cref="IConfigData" /> to another.
    /// </summary>
    /// <param name="other">The <see cref="IConfigData" /> to copy values to.</param>
    /// <typeparam name="TOther">The class/type of the other <see cref="IConfigData" />.</typeparam>
    public void CopyTo<TOther>(TOther other)
        where TOther : IConfigData
    {
        other.CarryChestLimit = this.CarryChestLimit;
        other.CarryChestSlow = this.CarryChestSlow;
        other.CategorizeChest = this.CategorizeChest;
        other.Configurator = this.Configurator;
        ((IControlScheme)other.ControlScheme).CopyTo(this.ControlScheme);
        other.CustomColorPickerArea = this.CustomColorPickerArea;
        ((IStorageData)other.DefaultChest).CopyTo(this.DefaultChest);
        other.SearchTagSymbol = this.SearchTagSymbol;
        other.SlotLock = this.SlotLock;
        other.SlotLockHold = this.SlotLockHold;
    }
}