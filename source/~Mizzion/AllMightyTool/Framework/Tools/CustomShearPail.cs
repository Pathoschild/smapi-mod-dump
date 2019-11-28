using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace AllMightyTool.Framework.Tools
{
    internal class CustomShearPail : MilkPail
    {
        public FarmAnimal Animal;
        private readonly bool _usePailOnOtherAnimals;

        public CustomShearPail(bool usePailOnOtherAnimals)
        {
            _usePailOnOtherAnimals = usePailOnOtherAnimals;
        }

        public FarmAnimal TargetAnimal(GameLocation loc, int x, int y, Farmer player)
        {
            Rectangle rect = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize);
            if (loc is Farm farm)
            {
                foreach (var a in farm.animals.Pairs)
                {
                    var anim = a.Value;
                    if (anim.GetBoundingBox().Intersects(rect))
                    {
                        Animal = anim;
                        return Animal;
                    }
                }
            }
            else
            {
                if (loc is AnimalHouse aHouse)
                {
                    foreach (var ah in aHouse.animals.Pairs)
                    {
                        var anim = ah.Value;
                        if (anim.GetBoundingBox().Intersects(rect))
                        {
                            Animal = anim;
                            return Animal;
                        }
                    }
                }
            }
            return Animal;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            string type = Animal.type.Value;

            if (Animal != null && !_usePailOnOtherAnimals && (!type.Contains("Sheep") && !type.Contains("Cow") && !type.Contains("Goat")))
                Game1.showRedMessage($"{Animal.Name} is not a Cow, Goat, or Sheep.");
            else if (Animal != null && Animal.currentProduce.Value > 0 &&
                     Animal.age.Value >= Animal.ageWhenMature.Value &&
                     who.couldInventoryAcceptThisObject(Animal.currentProduce.Value, 1))
            {
                Animal.doEmote(20);
                Animal.friendshipTowardFarmer.Value += Math.Min(1000, Animal.friendshipTowardFarmer.Value + 5);
                Animal.pauseTimer = 1500;
                base.DoFunction(location, x, y, power, who);
                CurrentParentTileIndex = 6;
                IndexOfMenuItemView = 6;

                if (Animal != null && Animal.currentProduce.Value > 0 && Animal.age.Value > Animal.ageWhenMature.Value)
                {
                    SObject obj = new SObject(Vector2.Zero, Animal.currentProduce.Value, null, false, true, false,
                        false)
                    { Quality = Animal.produceQuality.Value };

                    if (who.addItemToInventoryBool(obj))
                    {
                        Game1.playSound("coin");
                        Animal.currentProduce.Value = -1;

                        if (Animal.showDifferentTextureWhenReadyForHarvest.Value)
                            Animal.Sprite.LoadTexture("Animal\\Sheared" + Animal.type.Value);
                        who.gainExperience(0, 5);

                    }
                }
            }
            else if (Animal != null && Animal.currentProduce.Value > 0 && Animal.age.Value > Animal.ageWhenMature.Value)
            {
                if (!who.couldInventoryAcceptThisObject(Animal.currentProduce.Value, 1))
                    Game1.showRedMessage("Inventory Full.");
            }
            else
            {
                string source = "";

                if (Animal != null && Animal.isBaby())
                    source = $"{Animal.Name} is too young to produce animal products.";
                if (Animal != null && Animal.age.Value > Animal.ageWhenMature.Value)
                    source = $"{Animal.Name} has no animal product right now.";
                if (source.Any())
                    Game1.showRedMessage(source);
            }

            Animal = null;
        }
    }
}