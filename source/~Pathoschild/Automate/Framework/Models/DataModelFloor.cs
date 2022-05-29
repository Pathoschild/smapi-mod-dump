/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.Automate.Framework.Models
{
    /// <summary>A floor to support by name in configuration.</summary>
    internal class DataModelFloor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The English name for the floor item.</summary>
        public string Name { get; }

        /// <summary>The item's unique ID.</summary>
        public int ItemId { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The English name for the floor item.</param>
        /// <param name="itemId">The item's unique ID.</param>
        public DataModelFloor(string name, int itemId)
        {
            this.Name = name;
            this.ItemId = itemId;
        }
    }
}
