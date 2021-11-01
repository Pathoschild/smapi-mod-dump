/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Integrations.XSPlus
{
    using StardewModdingAPI;

    /// <inheritdoc />
    internal class XSPlusIntegration : ModIntegration<IXSPlusAPI>
    {
        /// <summary>Initializes a new instance of the <see cref="XSPlusIntegration" /> class.</summary>
        /// <param name="modRegistry">SMAPI's mod registry.</param>
        public XSPlusIntegration(IModRegistry modRegistry)
            : base(modRegistry, "furyx639.XSPlus")
        {
        }
    }
}