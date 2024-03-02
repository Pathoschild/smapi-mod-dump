/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.Common.Integrations.PelicanFiber
{
    /// <summary>Handles the logic for integrating with the Pelican Fiber mod.</summary>
    internal class PelicanFiberIntegration : BaseIntegration
    {
        /*********
        ** Fields
        *********/
        /// <summary>The full type name of the Pelican Fiber mod's build menu.</summary>
        private readonly string MenuTypeName = "PelicanFiber.Framework.ConstructionMenu";

        /// <summary>An API for accessing private code.</summary>
        private readonly IReflectionHelper Reflection;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
        /// <param name="reflection">An API for accessing private code.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        public PelicanFiberIntegration(IModRegistry modRegistry, IReflectionHelper reflection, IMonitor monitor)
            : base("Pelican Fiber", "jwdred.PelicanFiber", "3.1.1-unofficial.7.1-pathoschild", modRegistry, monitor)
        {
            this.Reflection = reflection;
        }

        /// <summary>Get whether the Pelican Fiber build menu is open.</summary>
        public bool IsBuildMenuOpen()
        {
            this.AssertLoaded();
            return Game1.activeClickableMenu?.GetType().FullName == this.MenuTypeName;
        }

        /// <summary>Get the selected blueprint from the Pelican Fiber build menu, if it's open.</summary>
        public CarpenterMenu.BlueprintEntry? GetBuildMenuBlueprint()
        {
            this.AssertLoaded();

            return this.IsBuildMenuOpen()
                ? this.Reflection.GetProperty<CarpenterMenu.BlueprintEntry>(Game1.activeClickableMenu, "Blueprint").GetValue()
                : null;
        }
    }
}
