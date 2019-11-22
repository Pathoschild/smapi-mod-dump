using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpouseStuff.Spouses
{
    interface ISpouseRoom
    {
        void InteractWithSpot(int tileX, int tileY, int faceDirection);
    }
}
