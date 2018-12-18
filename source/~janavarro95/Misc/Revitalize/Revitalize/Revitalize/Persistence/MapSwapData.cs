using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Persistence
{
  public  class MapSwapData
    {
       public string mapPath;
        public string folderPath;


        public MapSwapData()
        {

        }
       public MapSwapData(string MAP_PATH,string FOLDER_PATH)
        {
            mapPath = MAP_PATH;
            folderPath = FOLDER_PATH;
        }
    }
}
