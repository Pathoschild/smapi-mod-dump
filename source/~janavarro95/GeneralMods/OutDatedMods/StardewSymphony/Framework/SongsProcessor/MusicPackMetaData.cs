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

namespace Omegasis.StardewSymphony.Framework.SongsProcessor
{
    public class MusicPackMetaData
    {
        public string name;
        public string fileLocation;
        public bool xwbWavePack;


        /// <summary>
        /// Constructor for non-xwb music packs
        /// </summary>
        /// <param name="name"></param>
        public MusicPackMetaData(string name)
        {

            this.name = name;
            this.xwbWavePack = false;
        }

        /// <summary>
        /// Constructor for xnb music packs
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fileLocation"></param>
        public MusicPackMetaData(string name,string fileLocation)
        {
            this.name = name;
            this.fileLocation = fileLocation;
            this.xwbWavePack = true;
        }

    }
}
