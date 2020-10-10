/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using Newtonsoft.Json;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Creates a new, separate instance of the provided object with identical contents. Uses Newtonsoft JSON conversion.</summary>
            /// <param name="oldObject">The object to be cloned.</param>
            /// <returns>A new, separate object with identical contents.</returns>
            public static T Clone<T>(T oldObject)
            {
                string oldString = Newtonsoft.Json.JsonConvert.SerializeObject(oldObject); //convert the old object into a JSON string
                T newObject = JsonConvert.DeserializeObject<T>(oldString); //convert the string back into a new object
                return newObject;
            }
        }
    }
}