using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmHouseRedone
{
    public static class ObjectIDHelper
    {
        public static Dictionary<string, int> objectIDs;

        public static void init()
        {
            objectIDs = new Dictionary<string, int>();
            Dictionary<int, string> objectInfo = FarmHouseStates.loader.Load<Dictionary<int, string>>("Data/ObjectInformation", StardewModdingAPI.ContentSource.GameContent);

            foreach(int id in objectInfo.Keys)
            {
                string name = objectInfo[id].Split('/')[0];
                objectIDs[name] = id;
            }
        }

        public static int getID(string name)
        {
            if (objectIDs.ContainsKey(name))
                return objectIDs[name];
            return -1;
        }

        public static string getName(int id)
        {
            foreach(string name in objectIDs.Keys)
            {
                if (objectIDs[name] == id)
                    return name;
            }
            return "Unknown Object";
        }
    }
}
