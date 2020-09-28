namespace RemoteFridgeStorage
{
    public class Config
    {
        public bool FlipImage { get; set; } = false;

        public double ImageScale { get; set; } = 1;

        // If this is true use xOffset and yOffset.
        public bool OverrideOffset { get; set; } = false;
        public int XOffset { get; set; } = 1145;
        public int YOffset { get; set; } = 450;

        public bool Editable { get; set; } = false;
    }
}