/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Integrations.ProducerFrameworkMod;

using StardewModdingAPI;

/// <inheritdoc />
internal class ProducerFrameworkModIntegration : ModIntegration<IProducerFrameworkModApi>
{
    private const string ModUniqueId = "Digus.ProducerFrameworkMod";

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProducerFrameworkModIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public ProducerFrameworkModIntegration(IModRegistry modRegistry)
        : base(modRegistry, ProducerFrameworkModIntegration.ModUniqueId)
    {
    }
}