using StardewModdingAPI;

namespace AdvancedKeyBindings.Config
{
    public class ModConfigRawKeys
    {
        public string AddToExistingStacks { get; set; } = SButton.S.ToString();
        public string PanScreenScrollLeft { get; set; } = $"{SButton.Left.ToString()},{SButton.DPadLeft.ToString()}";
        public string PanScreenScrollRight { get; set; } = $"{SButton.Right.ToString()},{SButton.DPadRight.ToString()}";
        public string PanScreenScrollUp { get; set; } = $"{SButton.Up.ToString()},{SButton.DPadUp.ToString()}";
        public string PanScreenScrollDown { get; set; } = $"{SButton.Down.ToString()},{SButton.DPadDown.ToString()}";

        public string PanScreenPreviousBuilding { get; set; } =
            $"{SButton.LeftControl.ToString()},{SButton.LeftShoulder.ToString()}";

        public string PanScreenNextBuilding { get; set; } =
            $"{SButton.RightControl.ToString()},{SButton.RightShoulder.ToString()}";

        public ModConfigKeys ParseControls(IMonitor monitor)
        {
            return new ModConfigKeys(
                CommonHelper.ParseButtons(this.AddToExistingStacks, monitor, nameof(this.AddToExistingStacks)),
                CommonHelper.ParseButtons(this.PanScreenScrollLeft, monitor, nameof(this.PanScreenScrollLeft)),
                CommonHelper.ParseButtons(this.PanScreenScrollRight, monitor, nameof(this.PanScreenScrollRight)),
                CommonHelper.ParseButtons(this.PanScreenScrollUp, monitor, nameof(this.PanScreenScrollUp)),
                CommonHelper.ParseButtons(this.PanScreenScrollDown, monitor, nameof(this.PanScreenScrollDown)),
                CommonHelper.ParseButtons(this.PanScreenPreviousBuilding, monitor,
                    nameof(this.PanScreenPreviousBuilding)),
                CommonHelper.ParseButtons(this.PanScreenNextBuilding, monitor, nameof(this.PanScreenNextBuilding))
            );
        }
    }
}