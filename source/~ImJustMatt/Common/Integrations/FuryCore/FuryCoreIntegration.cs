/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Integrations.FuryCore;

using StardewModdingAPI;

/// <inheritdoc />
internal class FuryCoreIntegration : ModIntegration<IFuryCoreApi>
{
    private const string ModUniqueId = "furyx639.FuryCore";

    /// <summary>
    ///     Initializes a new instance of the <see cref="FuryCoreIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public FuryCoreIntegration(IModRegistry modRegistry)
        : base(modRegistry, FuryCoreIntegration.ModUniqueId)
    {
    }
}