namespace GetDressed.Framework
{
    /// <summary>The JSON model for global settings read from the mod's config file.</summary>
    public class GlobalConfig
    {
        /// <summary>The keyboard button which opens the menu.</summary>
        public string MenuAccessKey { get; set; } = "C";

        /// <summary>Whether to show the 'new' button next to the customisation menu tabs. This is automatically disabled after the player sees the menu once.</summary>
        public bool ShowIntroBanner { get; set; } = true;

        /// <summary>Whether to show patch the dresser into the farmhouse.</summary>
        public bool ShowDresser { get; set; } = true;

        /// <summary>The number of male faces available.</summary>
        public int MaleFaceTypes { get; set; } = 2;

        /// <summary>The number of male noses available.</summary>
        public int MaleNoseTypes { get; set; } = 3;

        /// <summary>The number of male bottoms available.</summary>
        public int MaleBottomsTypes { get; set; } = 6;

        /// <summary>The number of male shoes available.</summary>
        public int MaleShoeTypes { get; set; } = 2;

        /// <summary>The number of female faces available.</summary>
        public int FemaleFaceTypes { get; set; } = 2;

        /// <summary>The number of female noses available.</summary>
        public int FemaleNoseTypes { get; set; } = 3;

        /// <summary>The number of female bottoms available.</summary>
        public int FemaleBottomsTypes { get; set; } = 12;

        /// <summary>The number of female shoes available.</summary>
        public int FemaleShoeTypes { get; set; } = 4;

        /// <summary>Whether to move the dresser for compatibility with another mod that adds a stove in the same spot.</summary>
        public bool StoveInCorner { get; set; }

        /// <summary>Whether to hide the skirt options for male characters.</summary>
        public bool HideMaleSkirts { get; set; }

        /// <summary>Whether to temporarily zoom out when showing the menu.</summary>
        public bool MenuZoomOut { get; set; } = true;
    }
}
