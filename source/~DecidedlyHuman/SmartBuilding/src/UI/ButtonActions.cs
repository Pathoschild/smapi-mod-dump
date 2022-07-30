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
        private ModEntry mod;
        private ModState modState;
        
        // Simply passing in our main mod class is terrible, but... it's either this, or spend
        // forever rearchitecting when I could be working on the usability of the mod.
        public ButtonActions(ModEntry mod, ModState modState)
        {
            this.mod = mod;
            this.modState = modState;
        }
        
        public void DrawClicked()
        {
            modState.ActiveTool = ButtonId.Draw;
            modState.ResetVolatileTiles();
        }

        public void EraseClicked()
        {
            modState.ActiveTool = ButtonId.Erase;
            modState.ResetVolatileTiles();
        }

        public void FilledRectangleClicked()
        {
            modState.ActiveTool = ButtonId.FilledRectangle;
            modState.ResetVolatileTiles();
        }

        public void DrawnLayerClicked()
        {
            modState.SelectedLayer = TileFeature.Drawn;
            modState.ResetVolatileTiles();
        }

        public void ObjectLayerClicked()
        {
            modState.SelectedLayer = TileFeature.Object;
            modState.ResetVolatileTiles();
        }

        public void TerrainFeatureLayerClicked()
        {
            modState.SelectedLayer = TileFeature.TerrainFeature;
            modState.ResetVolatileTiles();
        }

        public void FurnitureLayerClicked()
        {
            modState.SelectedLayer = TileFeature.Furniture;
            modState.ResetVolatileTiles();
        }

        public void InsertClicked()
        {
            modState.ActiveTool = ButtonId.Insert;
            modState.ResetVolatileTiles();
        }

        public void ConfirmBuildClicked()
        {
            mod.ConfirmBuild();
        }

        public void ClearBuildClicked()
        {
            mod.ClearBuild();
        }
    }
}