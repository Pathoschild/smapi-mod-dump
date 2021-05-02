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

namespace Revitalize.Framework.Objects.InformationFiles.Furniture
{
    public class ChairInformation
    {
        public bool canSitHere;

        public ChairInformation():base()
        {

        }

        public ChairInformation(bool CanSitHere) : base()
        {
            this.canSitHere = CanSitHere;
        }
    }
}
