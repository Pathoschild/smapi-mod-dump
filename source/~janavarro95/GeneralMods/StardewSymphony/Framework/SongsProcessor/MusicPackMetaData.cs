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
