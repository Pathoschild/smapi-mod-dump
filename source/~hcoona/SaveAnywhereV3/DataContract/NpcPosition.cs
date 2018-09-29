namespace SaveAnywhereV3.DataContract
{
    public class NpcPosition
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Location { get; set; }
        public int Direction { get; set; }
        public int? SchedulePathDescriptionKey { get; set; }
        public int? PathToEndPointCount { get; set; }
    }
}
