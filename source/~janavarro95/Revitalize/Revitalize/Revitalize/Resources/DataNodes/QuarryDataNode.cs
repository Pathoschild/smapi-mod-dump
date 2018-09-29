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
