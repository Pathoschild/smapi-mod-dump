namespace Teleport
{
    public class TPData
    {
        public string name { get; }

        public string locationName { get; }

        public int tileX { get; }
        
        public int tileY { get; }

        public TPData(string name, string locationName,int tileX,int tileY)
        {
            this.name = name;
            this.locationName = locationName;
            this.tileX = tileX;
            this.tileY = tileY;
        }
        
    }
}