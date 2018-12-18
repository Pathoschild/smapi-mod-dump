using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.PathFindingCore
{
    public class TileExceptionMetaData
    {

        public TileNode tile;
        public string actionType;

        public TileExceptionMetaData(TileNode t, string ActionName)
        {
            tile = t;
            actionType = ActionName;
        }

    }
}
