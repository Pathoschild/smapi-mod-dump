using Newtonsoft.Json.Linq;
using StarAI.PathFindingCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.PathFindingCore
{
    public class TileExceptionNode
    {
        public string imageSource;
        public int index;


        public TileExceptionNode()
        {
        }

        public TileExceptionNode(string ImageSource, int TileIndex)
        {
            imageSource = ImageSource;
            index = TileIndex;
        }


    }
}
