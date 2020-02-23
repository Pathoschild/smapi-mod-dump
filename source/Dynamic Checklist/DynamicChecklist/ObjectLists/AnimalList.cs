namespace DynamicChecklist.ObjectLists
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Menus;

    public class AnimalList : ObjectList
    {
        private Action action;

        public AnimalList(ModConfig config, Action action)
            : base(config)
        {
            this.action = action;
            switch (action)
            {
                case Action.Pet:
                    this.ImageTexture = OverlayTextures.Heart;
                    this.OptionMenuLabel = "Pet Animals";
                    this.TaskDoneMessage = "All animals have been petted";
                    this.Name = TaskName.Pet;
                    break;
                case Action.Milk:
                    this.ImageTexture = OverlayTextures.MilkPail;
                    this.OptionMenuLabel = "Milk Cows/Goats";
                    this.TaskDoneMessage = "All Cows and Goats have been milked";
                    this.Name = TaskName.Milk;
                    break;
                case Action.Shear:
                    this.ImageTexture = OverlayTextures.Shears;
                    this.OptionMenuLabel = "Shear Sheep";
                    this.TaskDoneMessage = "All sheep have been sheared";
                    this.Name = TaskName.Shear;
                    break;
                default:
                    throw new NotImplementedException();
            }

            this.ObjectInfoList = new List<StardewObjectInfo>();
        }

        public enum Action
        {
            Pet, Milk, Shear
        }

        public override string OptionMenuLabel { get; protected set; }

        public override string TaskDoneMessage { get; protected set; }

        protected override Texture2D ImageTexture { get; set; }

        public override void OnMenuOpen()
        {
        }

        public override void BeforeDraw()
        {
            if (!this.TaskDone && Game1.currentLocation.IsFarm)
            {
                this.UpdateObjectInfoList();
            }
        }

        protected override void UpdateObjectInfoList()
        {
            this.ObjectInfoList.Clear();

            // Outside animals
            var outsideAnimals = Game1.getFarm().animals.Values.ToList<FarmAnimal>();
            foreach (FarmAnimal animal in outsideAnimals)
            {
                StardewObjectInfo soi = this.CreateSOI(animal, Game1.getFarm(), this.action);
                this.ObjectInfoList.Add(soi);
            }

            // Inside animals
            var farmBuildings = Game1.getFarm().buildings;

            foreach (Building building in farmBuildings)
            {
                var indoors = building.indoors.Value;
                if (indoors != null && indoors is AnimalHouse)
                {
                    var animalHouse = (AnimalHouse)indoors;
                    foreach (FarmAnimal animal in animalHouse.animals.Values.ToList())
                    {
                        StardewObjectInfo soi = this.CreateSOI(animal, animalHouse, this.action);
                        this.ObjectInfoList.Add(soi);
                    }
                }
            }

            this.TaskDone = this.CountNeedAction == 0;
        }

        private StardewObjectInfo CreateSOI(FarmAnimal animal, GameLocation loc, Action action)
        {
            var soi = new StardewObjectInfo();
            soi.Coordinate = animal.getStandingPosition();
            soi.Location = loc;
            switch (action)
            {
                case Action.Pet:
                    soi.NeedAction = !animal.wasPet.Value;
                    break;
                case Action.Milk:
                    soi.NeedAction = animal.currentProduce.Value > 0 && animal.toolUsedForHarvest.Value == "Milk Pail";
                    break;
                case Action.Shear:
                    soi.NeedAction = animal.currentProduce.Value > 0 && animal.toolUsedForHarvest.Value == "Shears";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return soi;
        }
    }
}
