using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Framework.Objects.InformationFiles.Furniture
{
    public class ChairInformation:FurnitureInformation
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
