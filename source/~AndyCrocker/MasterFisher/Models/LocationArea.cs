/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using SatoCore;
using SatoCore.Attributes;
using System.Collections.Generic;

namespace MasterFisher.Models
{
    /// <summary>Represents an area in a location that can be configured.</summary>
    public class LocationArea
    {
        /*********
        ** Accessors
        *********/
        /// <summary>How the location data should be interpreted.</summary>
        public Action Action { get; set; } = Action.Add;

        /// <summary>The unique name of the location area being configured.</summary>
        [Identifier]
        public string UniqueName { get; set; }

        /// <summary>The name of the location being configured.</summary>
        [Required]
        [Editable]
        public string LocationName { get; set; }

        /// <summary>The area in the location being configured.</summary>
        [Editable]
        [DefaultValue(typeof(Rectangle))]
        public Rectangle? Area { get; set; }

        /// <summary>The fish that can be caught in spring in the area.</summary>
        [Editable]
        [DefaultValue(typeof(List<string>))]
        public List<string> SpringFish { get; set; }

        /// <summary>The fish that can be caught in summer in the area.</summary>
        [Editable]
        [DefaultValue(typeof(List<string>))]
        public List<string> SummerFish { get; set; }

        /// <summary>The fish that can be caught in fall in the area.</summary>
        [Editable]
        [DefaultValue(typeof(List<string>))]
        public List<string> FallFish { get; set; }

        /// <summary>The fish that can be caught in winter in the area.</summary>
        [Editable]
        [DefaultValue(typeof(List<string>))]
        public List<string> WinterFish { get; set; }

        // TODO: add fishable objects property

        /// <summary>The chance of finding treasure when fishing in the area.</summary>
        [Editable]
        [DefaultValue(.15f)]
        public float? TreasureChance { get; set; }
    }
}
