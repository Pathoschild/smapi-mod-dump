/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.Automate;

/// <inheritdoc />
internal sealed class AutomateIntegration : ModIntegration<IAutomateApi>
{
    private const string ModUniqueId = "Pathoschild.Automate";

    /// <summary>
    ///     Initializes a new instance of the <see cref="AutomateIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public AutomateIntegration(IModRegistry modRegistry)
        : base(modRegistry, AutomateIntegration.ModUniqueId)
    {
        // Nothing
    }
}