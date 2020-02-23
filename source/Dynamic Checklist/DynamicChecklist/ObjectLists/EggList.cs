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
    using StardewValley.Buildings;

    public class EggList : ObjectList
    {
        public EggList(ModConfig config)
            : base(config)
        {
            this.ImageTexture = OverlayTextures.Hand;
            this.OptionMenuLabel = "Collect Animal Products";
            this.TaskDoneMessage = "All animal products have been collected";
            this.Name = TaskName.Egg;
            this.ObjectInfoList = new List<StardewObjectInfo>();
        }

        public override string OptionMenuLabel { get; protected set; }

        public override string TaskDoneMessage { get; protected set; }

        protected override Texture2D ImageTexture { get; set; }

        public override void BeforeDraw()
        {
            if (!this.TaskDone && Game1.currentLocation.IsFarm)
            {
                this.UpdateObjectInfoList();
            }
        }

        public override void OnMenuOpen()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateObjectInfoList()
        {
            this.ObjectInfoList.Clear();
            var farmBuildings = Game1.getFarm().buildings;
            foreach (Building building in farmBuildings)
            {
                var indoors = building.indoors.Value;
                if (indoors != null && indoors is AnimalHouse)
                {
                    var animalHouse = (AnimalHouse)indoors;
                    foreach (KeyValuePair<Vector2, StardewValley.Object> obj in animalHouse.Objects.Pairs)
                    {
                        if (obj.Value.IsSpawnedObject)
                        {
                            StardewObjectInfo soi = this.CreateSOI(obj, animalHouse);
                            this.ObjectInfoList.Add(soi);
                        }
                    }
                }
            }

            var taskDone = true;
            foreach (StardewObjectInfo soi in this.ObjectInfoList)
            {
                if (soi.NeedAction)
                {
                    taskDone = false;
                    break;
                }
            }

            this.TaskDone = taskDone;
        }

        private StardewObjectInfo CreateSOI(KeyValuePair<Vector2, StardewValley.Object> obj, GameLocation loc)
        {
            var soi = new StardewObjectInfo();
            soi.Coordinate = obj.Key * Game1.tileSize + new Vector2(Game1.tileSize / 2, Game1.tileSize / 2);
            soi.Location = loc;
            soi.NeedAction = true;
            return soi;
        }
    }
}
