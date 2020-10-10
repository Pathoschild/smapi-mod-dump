/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

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
