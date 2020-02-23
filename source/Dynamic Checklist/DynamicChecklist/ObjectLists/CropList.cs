namespace DynamicChecklist.ObjectLists
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewValley;
    using StardewValley.TerrainFeatures;

    public class CropList : ObjectList
    {
        private Action action;

        public CropList(ModConfig config, Action action)
            : base(config)
        {
            this.action = action;
            switch (action)
            {
                case Action.Water:
                    this.ImageTexture = OverlayTextures.WateringCan;
                    this.OptionMenuLabel = "Water Crops";
                    this.TaskDoneMessage = "All crops have been watered";
                    this.Name = TaskName.Water;
                    break;
                case Action.Harvest:
                    this.ImageTexture = OverlayTextures.Plus;
                    this.OptionMenuLabel = "Harvest Crops";
                    this.TaskDoneMessage = "All crops have been harvested";
                    this.Name = TaskName.Harvest;
                    break;
                default:
                    throw new NotImplementedException();
            }

            this.ObjectInfoList = new List<StardewObjectInfo>();
        }

        public enum Action
        {
            Water, Harvest
        }

        public override string OptionMenuLabel { get; protected set; }

        public override string TaskDoneMessage { get; protected set; }

        protected override Texture2D ImageTexture { get; set; }

        public override void OnMenuOpen()
        {
        }

        public override void BeforeDraw()
        {
            if (Game1.currentLocation == Game1.getFarm() || Game1.currentLocation == Game1.getLocationFromName("Greenhouse"))
            {
                this.UpdateObjectInfoList();
            }
        }

        protected override void UpdateObjectInfoList()
        {
            this.ObjectInfoList.Clear();
            Farm farm = Game1.getFarm();
            GameLocation greenhouse = Game1.getLocationFromName("Greenhouse");

            this.UpdateObjectInfoList(farm);
            this.UpdateObjectInfoList(greenhouse);

            this.TaskDone = this.CountNeedAction == 0;
        }

        private void UpdateObjectInfoList(GameLocation loc)
        {
            foreach (KeyValuePair<Vector2, TerrainFeature> entry in loc.terrainFeatures.Pairs)
            {
                var terrainFeature = entry.Value;
                if (terrainFeature is HoeDirt)
                {
                    var coordinate = entry.Key;
                    var hoeDirt = (HoeDirt)terrainFeature;
                    StardewObjectInfo soi = this.CreateSOI(hoeDirt, coordinate, loc, this.action);
                    this.ObjectInfoList.Add(soi);
                }
            }
        }

        private StardewObjectInfo CreateSOI(HoeDirt hoeDirt, Vector2 coordinate, GameLocation loc, Action action)
        {
            var soi = new StardewObjectInfo();
            soi.Coordinate = coordinate * Game1.tileSize + new Vector2(Game1.tileSize / 2, Game1.tileSize / 2);
            soi.Location = loc;
            switch (action)
            {
                case Action.Water:
                    var isWatered = hoeDirt.state.Value == HoeDirt.watered;
                    soi.NeedAction = hoeDirt.needsWatering() && !isWatered && !hoeDirt.crop.dead.Value;
                    break;
                case Action.Harvest:
                    soi.NeedAction = hoeDirt.readyForHarvest();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return soi;
        }
    }
}
