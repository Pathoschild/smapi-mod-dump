/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System;

namespace SonoCore.Attributes
{
    /// <summary>Indicates the property is an api token.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TokenAttribute : Attribute
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the property to populate with the token value.</summary>
        public string OutputPropertyName { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructsa an instance.</summary>
        /// <param name="outputPropertyName">The name of the property to populate with the token value.</param>
        public TokenAttribute(string outputPropertyName)
        {
            OutputPropertyName = outputPropertyName;
        }
    }
}
