/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;
namespace Revitalize.Resources.DataNodes
{
    class QuarryDataNode
    {
       public string Name;
       public StardewValley.Object Output;
       public int TimeToProcess;

        

        public QuarryDataNode(string name, Object output, int timeToProcess)
        {
            Name = name;
            Output = output;
            TimeToProcess = timeToProcess;
        }


    }
}
