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

namespace Revitalize.Resources.DataNodes
{
    class SeedDataNode
    {
       public int parentIndex;
       public int cropIndex;

        /// <summary>
        ///  //crop row number is actually counts row 0 on upper left and row right on upper right.
        /// </summary>
        /// <param name="parentInt"> parentsheetindex for seeds image,</param>
        /// <param name="cropInt">actualCropNumber from crops.xnb</param>
        public SeedDataNode(int parentInt, int cropInt)
        {
            parentIndex = parentInt;
            cropIndex = cropInt;

        }

    }
}
