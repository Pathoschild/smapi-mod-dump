using StardewModdingAPI;

namespace AdvancedKeyBindings.Config
{
    public class ModConfigKeys
    {
        public SButton[] AddToExistingStacks { get; }
        public SButton[] PanScreenScrollLeft { get; }
        public SButton[] PanScreenScrollRight { get; }
        public SButton[] PanScreenScrollUp { get; }
        public SButton[] PanScreenScrollDown { get; }
        public SButton[] PanScreenPreviousBuilding { get; }
        public SButton[] PanScreenNextBuilding { get; }

        public ModConfigKeys(SButton[] addToExistingStacks, SButton[] panScreenScrollLeft, SButton[] panScreenScrollRight, SButton[] panScreenScrollUp, SButton[] panScreenScrollDown, SButton[] panScreenPreviousBuilding, SButton[] panScreenNextBuilding)
        {
            AddToExistingStacks = addToExistingStacks;
            PanScreenScrollLeft = panScreenScrollLeft;
            PanScreenScrollRight = panScreenScrollRight;
            PanScreenScrollUp = panScreenScrollUp;
            PanScreenScrollDown = panScreenScrollDown;
            PanScreenPreviousBuilding = panScreenPreviousBuilding;
            PanScreenNextBuilding = panScreenNextBuilding;
        }
    }
}