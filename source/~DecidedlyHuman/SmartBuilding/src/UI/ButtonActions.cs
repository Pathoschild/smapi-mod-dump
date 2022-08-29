/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

namespace SmartBuilding.UI
{
    public class ButtonActions
    {
        private readonly ModEntry mod;
        private readonly ModState modState;

        // Simply passing in our main mod class is terrible, but... it's either this, or spend
        // forever rearchitecting when I could be working on the usability of the mod.
        public ButtonActions(ModEntry mod, ModState modState)
        {
            this.mod = mod;
            this.modState = modState;
        }

        public void DrawClicked()
        {
            this.modState.ActiveTool = ButtonId.Draw;
            this.modState.ResetVolatileTiles();
        }

        public void EraseClicked()
        {
            this.modState.ActiveTool = ButtonId.Erase;
            this.modState.ResetVolatileTiles();
        }

        public void FilledRectangleClicked()
        {
            this.modState.ActiveTool = ButtonId.FilledRectangle;
            this.modState.ResetVolatileTiles();
        }

        public void DrawnLayerClicked()
        {
            this.modState.SelectedLayer = TileFeature.Drawn;
            this.modState.ResetVolatileTiles();
        }

        public void ObjectLayerClicked()
        {
            this.modState.SelectedLayer = TileFeature.Object;
            this.modState.ResetVolatileTiles();
        }

        public void TerrainFeatureLayerClicked()
        {
            this.modState.SelectedLayer = TileFeature.TerrainFeature;
            this.modState.ResetVolatileTiles();
        }

        public void FurnitureLayerClicked()
        {
            this.modState.SelectedLayer = TileFeature.Furniture;
            this.modState.ResetVolatileTiles();
        }

        public void InsertClicked()
        {
            this.modState.ActiveTool = ButtonId.Insert;
            this.modState.ResetVolatileTiles();
        }

        public void ConfirmBuildClicked()
        {
            this.mod.ConfirmBuild();
        }

        public void ClearBuildClicked()
        {
            this.mod.ClearBuild();
        }
    }
}
