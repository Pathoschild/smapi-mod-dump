/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Integrations.XSLite
{
    using StardewModdingAPI;

    /// <inheritdoc />
    internal class XSLiteIntegration : ModIntegration<IXSLiteAPI>
    {
        /// <summary>Initializes a new instance of the <see cref="XSLiteIntegration" /> class.</summary>
        /// <param name="modRegistry">SMAPI's mod registry.</param>
        public XSLiteIntegration(IModRegistry modRegistry)
            : base(modRegistry, "furyx639.ExpandedStorage")
        {
        }
    }
}