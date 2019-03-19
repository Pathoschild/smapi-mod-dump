namespace BusLocations.Framework
{
    internal class BusLoc
    {
        public string MapName { get; set; }
        public string DisplayName { get; set; }
        public int DestinationX { get; set; }
        public int DestinationY { get; set; }
        public int ArrivalFacing { get; set; }
        public int TicketPrice { get; set; }
    }
}
