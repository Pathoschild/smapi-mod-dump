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
