/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;
using Object = StardewValley.Object;

namespace Omegasis.HappyBirthday.Framework
{
    /// <summary>Provides utility methods for managing in-game objects.</summary>
    internal class ObjectUtility
    {
        /*********
        ** Fields
        *********/
        /// <summary>The cached object data.</summary>
        private static readonly Object[] ObjectList = ObjectUtility.GetAllObjects().ToArray();


        /*********
        ** Public methods
        *********/
        /// <summary>Get objects with the given category.</summary>
        /// <param name="category">The category for which to find objects.</param>
        public static IEnumerable<Object> GetObjectsInCategory(int category)
        {
            if (category > 0)
                yield break;

            foreach (Object obj in ObjectUtility.ObjectList)
            {
                if (obj.Category == category)
                    yield return obj;
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get all objects defined by the game.</summary>
        private static IEnumerable<Object> GetAllObjects()
        {
            foreach (int key in Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation").Keys)
                yield return new Object(key, 1);
        }
    }
}
