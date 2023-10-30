/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Searches all loaded assemblies for subclasses of the provided class type and returns them in a list.</summary>
            /// <param name="baseClass">The returned type must be derived from this class.</param>
            /// <returns>A list of types derived from baseClass.</returns>
            public static List<Type> GetAllSubclassTypes(Type baseClass)
            {
                List<Type> subclassTypes = new List<Type>();

                if (baseClass == null)
                {
                    return subclassTypes;
                }

                bool filterSubclass(Type type) => type.IsSubclassOf(baseClass); //true when a type is derived from the provided base class

                subclassTypes.AddRange
                (
                    AppDomain.CurrentDomain.GetAssemblies() //get all assemblies
                    .Where
                    (
                        //ignore any assemblies that can't contain monster and/or may cause errors
                        assembly => assembly.IsDynamic == false
                        && assembly.ManifestModule.Name != "<In Memory Module>"
                        && !assembly.FullName.StartsWith("System")
                        && !assembly.FullName.StartsWith("Microsoft")
                    )
                    .SelectMany(assembly => Utility.TryGetTypes(assembly)) //get all types from each assembly as a single sequence
                    .Where(filterSubclass) //ignore any types that are not subclasses of baseClass
                );

                return subclassTypes;
            }
        }
    }
}