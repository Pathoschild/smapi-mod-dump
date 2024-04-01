/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.GarbageDay.Framework.Services;

using StardewMods.Common.Interfaces;
using StardewMods.Common.Services;
using StardewMods.GarbageDay.Framework.Interfaces;
using StardewMods.GarbageDay.Framework.Models;

/// <inheritdoc cref="StardewMods.GarbageDay.Framework.Interfaces.IModConfig" />
internal sealed class ConfigManager : ConfigManager<DefaultConfig>, IModConfig
{
    /// <summary>Initializes a new instance of the <see cref="ConfigManager" /> class.</summary>
    /// <param name="eventPublisher">Dependency used for publishing events.</param>
    /// <param name="modHelper">Dependency for events, input, and content.</param>
    public ConfigManager(IEventPublisher eventPublisher, IModHelper modHelper)
        : base(eventPublisher, modHelper) { }

    /// <inheritdoc />
    public DayOfWeek GarbageDay => this.Config.GarbageDay;

    /// <inheritdoc />
    public bool OnByDefault => this.Config.OnByDefault;
}