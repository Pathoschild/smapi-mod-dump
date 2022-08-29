/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.EasyAccess;

/// <inheritdoc />
internal class EasyAccessIntegration : ModIntegration<IEasyAccessApi>
{
    private const string ModUniqueId = "furyx639.EasyAccess";

    /// <summary>
    ///     Initializes a new instance of the <see cref="EasyAccessIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public EasyAccessIntegration(IModRegistry modRegistry)
        : base(modRegistry, EasyAccessIntegration.ModUniqueId)
    {
        // Nothing
    }
}