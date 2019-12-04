using StardewModdingAPI;

namespace PondPainter
{
    class PondPainterConfig
    {
        public bool Enable_Custom_Pond_Coloring { get; set; } = true;
        public bool Enable_Animations { get; set; } = false;
        public bool Auto_Color_Other_Ponds_by_Dye_Color_of_Inhabitants { get; set; } = true;
        public int Minimum_Population_For_Auto_Coloring { get; set; } = 1;

    }
}
