/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.StardustCore.Compatibility.SpaceCore;
using StardewValley;
using StardewValley.Objects;

namespace Omegasis.Revitalize.Framework.Utilities
{
    /// <summary>
    /// Handles serialization of all objects in existence.
    /// </summary>
    public static class Serializer
    {

        /// <summary>
        /// Automatically serialize all mod classes that have the XMLAttribute tag on them.
        /// </summary>
        public static void SerializeTypesForXMLUsingSpaceCore()
        {
            SpaceCoreAPIUtil.RegisterTypesForMod(RevitalizeModCore.Instance);
        }


    }
}
