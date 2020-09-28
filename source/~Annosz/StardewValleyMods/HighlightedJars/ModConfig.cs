using System.Collections.Generic;

namespace HighlightedJars
{
    public class ModConfig
    {
        public string HighlightType { get; set; } = "Highlight";

        public bool HighlightJars { get; set; } = true;
        public bool HighlightKegs { get; set; } = true;
        public bool HighlightCasks { get; set; } = true;
    }
}
