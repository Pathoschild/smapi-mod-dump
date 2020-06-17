using System.Collections.Generic;

namespace AnimalsNeedWater
{
    /// <summary> Contains global variables for the mod. </summary>
    public static class ModData
    {
        public static List<string> CoopsWithWateredTrough { get; set; } = new List<string>();
        public static List<string> BarnsWithWateredTrough { get; set; } = new List<string>();
        public static List<string> FullAnimals { get; set; } = new List<string>();
    }
}
