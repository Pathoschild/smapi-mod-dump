/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Types;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Special
{
    class PetAnimalOnTouch : SingleEffect<PetAnimalOnTouchParameters>
    {
        public PetAnimalOnTouch(PetAnimalOnTouchParameters parameters)
            : base(parameters)
        {
            // --
        }

        public PetAnimalOnTouch(AnimalType animalType)
            : base(PetAnimalOnTouchParameters.With(animalType))
        {
            // --
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree) return;

            if (Game1.currentLocation is IAnimalLocation loc)
            {
                CheckPetAnimal(loc, Game1.player);
            }
        }

        private bool canPet(FarmAnimal animal)
        {
            // petting after 19:00 will display a dialogue box, soft-locking the game
            return Game1.timeOfDay < 1900 && !animal.wasPet;
        }

        private void CheckPetAnimal(IAnimalLocation location, Farmer who)
        {
            foreach (KeyValuePair<long, FarmAnimal> kvp in location.Animals.Pairs)
            {
                FarmAnimal animal = kvp.Value;
                if (Parameters.AnimalType == AnimalType.Any || animal.GetAnimalType() == Parameters.AnimalType)
                {
                    if (canPet(animal) && animal.GetCursorPetBoundingBox().Contains((int)who.position.X, (int)who.position.Y))
                    {                        
                        animal.pet(who, false);
                    }
                }
            }
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
        }

        protected override EffectDescriptionLine GenerateEffectDescription()
        {                     
           return new EffectDescriptionLine(Parameters.AnimalType.GetPetEffectIcon(), Parameters.AnimalType.GetPetEffectDescription());            
        }
    }

    public class PetAnimalOnTouchParameters : IEffectParameters
    {
        public AnimalType AnimalType { get; set; } = AnimalType.Any;

        public static PetAnimalOnTouchParameters With(AnimalType animalType)
        {
            return new PetAnimalOnTouchParameters() { AnimalType = animalType };
        }
    }
}
