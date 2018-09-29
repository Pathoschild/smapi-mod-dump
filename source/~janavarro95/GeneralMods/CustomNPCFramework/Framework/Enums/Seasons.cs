using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CustomNPCFramework.Framework.Enums
{
    /// <summary>
    /// An enum signifying the different seasons that are supported when chosing npc graphics.
    /// </summary>
    public enum Seasons
    {
        /// <summary>
        /// The spring season. This ensures that a corresponding graphic with this enum in it's seasons list can be chosen in the spring time.
        /// Also used for functionality to check seasons.
        /// </summary>
        spring,
        /// <summary>
        /// The summer season. This ensures that a corresponding graphic with this enum in it's seasons list can be chosen in the summer time.
        /// Also used for functionality to check seasons.
        /// </summary>
        summer,
        /// <summary>
        /// The fall season. This ensures that a corresponding graphic with this enum in it's seasons list can be chosen in the fall time.
        /// Also used for functionality to check seasons.
        /// </summary>
        fall,
        /// <summary>
        /// The winter season. This ensures that a corresponding graphic with this enum in it's seasons list can be chosen in the winter time.
        /// Also used for functionality to check seasons.
        /// </summary>
        winter
    }
}
