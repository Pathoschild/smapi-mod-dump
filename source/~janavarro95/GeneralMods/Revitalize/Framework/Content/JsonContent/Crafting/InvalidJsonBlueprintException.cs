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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.Revitalize.Framework.Content.JsonContent.Crafting
{
    /// <summary>
    /// Exception used to notify users that an error has occured when loading a JsonBlueprint from disk.
    /// </summary>
    public class InvalidJsonBlueprintException:Exception
    {

        public InvalidJsonBlueprintException() {


        }

        public InvalidJsonBlueprintException(string message) : base(message) { }
    }
}
