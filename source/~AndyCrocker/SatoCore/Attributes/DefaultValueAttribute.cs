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

namespace SatoCore.Attributes
{
    /// <summary>The default value of the member.</summary>
    /// <remarks>This should be used when the model has nullable members in order to support editing previous versions while still having a default value.</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DefaultValueAttribute : Attribute
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The default value of the member.</summary>
        public object DefaultValue { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="defaultValue">The default value of the member.</param>
        public DefaultValueAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>Constructs an instance.</summary>
        /// <param name="defaultValueType">The type of the default value to create.</param>
        /// <remarks>This is used to set the default value to the initial state of an object (an empty rectangle, for example).</remarks>
        public DefaultValueAttribute(Type defaultValueType)
        {
            if (defaultValueType != null)
                DefaultValue = Activator.CreateInstance(defaultValueType);
        }
    }
}
