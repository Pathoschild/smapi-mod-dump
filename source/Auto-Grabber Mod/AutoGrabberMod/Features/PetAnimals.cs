using System.Linq;
using AutoGrabberMod.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;

namespace AutoGrabberMod.Features
{
    class PetAnimals : Feature
    {
        public override string FeatureName => "Auto Pet Animals";

        public override string FeatureConfig => "pet";

        public override int Order => 1;

        public override bool IsAllowed => Utilities.Config.AllowAutoPet;

        public PetAnimals()
        {
            Value = false;
        }

        public override void Action()
        {
            if (!IsAllowed || !(bool)Value) return;

            //Utilities.Monitor.Log($"  {Grabber.InstanceName} Attempting to pet animals", StardewModdingAPI.LogLevel.Trace);

            var boundingBox = new Rectangle(Grabber.Grabber.getBoundingBox(Grabber.Tile).Left - Grabber.Range, Grabber.Grabber.getBoundingBox(Grabber.Tile).Top - Grabber.Range, 2 * Grabber.Range, 2 * Grabber.Range);
            var pets = Grabber.RangeEntireMap
                ? Grabber.Location.characters.OfType<Pet>()
                : Grabber.Location.characters.OfType<Pet>().Where(pet => pet.GetBoundingBox().Intersects(boundingBox));
            foreach (var pet in pets.ToArray())
            {
                //Utilities.Monitor.Log($"    - Attempting to pet {pet.Name} {pet.friendshipTowardFarmer}");
                if (!Utilities.Helper.Reflection.GetField<bool>(pet, "wasPetToday").GetValue()) pet.checkAction(Game1.player, Grabber.Location);
            }

            var animals = Grabber.RangeEntireMap
                ? Utilities.GetFarmAnimals(Grabber.Location)
                : Utilities.GetFarmAnimals(Grabber.Location).Where(animal => boundingBox.Intersects(animal.GetBoundingBox()));
            foreach (var animal in animals.Where(animal => !animal.wasPet.Value))
            {
                Utilities.Monitor.Log($"    - Attempting to pet {animal.Name} {animal.friendshipTowardFarmer.Value}");
                animal.pet(Game1.player);
            }
        }        
    }
}
