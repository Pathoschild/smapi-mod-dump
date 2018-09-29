using StardewValley;

namespace FarmExpansion.Framework
{
    /// <summary>An API provided by Farm Expansion for other mods to use.</summary>
    public class ModApi : IModApi
    {
        /*********
        ** Properties
        *********/
        /// <summary>The Farm Expansion core framework.</summary>
        private readonly FEFramework Framework;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="framework">The Farm Expansion core framework.</param>
        public ModApi(FEFramework framework)
        {
            this.Framework = framework;
        }

        /// <summary>Add a blueprint to all future carpenter menus for the farm area.</summary>
        /// <param name="blueprint">The blueprint to add.</param>
        public void AddFarmBluePrint(BluePrint blueprint)
        {
            this.Framework.FarmBlueprints.Add(blueprint);
        }

        /// <summary>Add a blueprint to all future carpenter menus for the expansion area.</summary>
        /// <param name="blueprint">The blueprint to add.</param>
        public void AddExpansionBluePrint(BluePrint blueprint)
        {
            this.Framework.ExpansionBlueprints.Add(blueprint);
        }
    }
}
