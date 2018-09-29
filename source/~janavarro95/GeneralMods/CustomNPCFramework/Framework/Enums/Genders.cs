using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.Enums
{
    /// <summary>
    /// Gender enum to signify the different genders for npcs.
    /// Do what you want with this. For code simplicity anything that is non-binary is specified under other.
    /// </summary>
    public enum Genders
    {
        /// <summary>
        /// Used for npcs to signify that they are the male gender.
        /// </summary>
        male,
        /// <summary>
        /// Used for npcs to signify that they are the female gender.
        /// </summary>
        female,
        /// <summary>
        /// Used for npcs to signify that they are a non gender binary gender.
        /// </summary>
        other
    }
}
