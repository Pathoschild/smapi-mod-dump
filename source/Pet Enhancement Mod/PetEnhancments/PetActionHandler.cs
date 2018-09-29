using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Characters;

using xTile.Tiles;
using xTile.Dimensions;
using xTile.Display;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SFarmer = StardewValley.Farmer;

namespace PetEnhancements
{
    class PetActionHandler
    {
        private Pet pet;
        private SFarmer farmer;
        private PetActionProvider provider;
        private List<Vector2> currentPath;
        private Vector2 savedFarmerPosition;

        public PetActionHandler(Pet pet)
        {
            this.pet = pet;
            farmer = Game1.player;
            savedFarmerPosition = farmer.getTileLocation();
        }

        public void intialize()
        {
            provider = new PetActionProvider(pet);
            currentPath = null;
        }

        public void performAction()
        {
            if (provider != null)
            {
                provider.prepareNextAction();
                var currentAction = provider.getCurrentAction();
                if (currentAction == Action.FOLLOWING)
                {
                    followPlayer();
                }
                else
                {
                    performSit();
                }
            }
        }

        private void performSit()
        {
            if (pet.CurrentBehavior != Pet.behavior_Sit_Down)
            {
                pet.CurrentBehavior = Pet.behavior_Sit_Down;
                pet.SetMovingDown(false);
                pet.SetMovingLeft(false);
                pet.SetMovingRight(false);
                pet.SetMovingUp(false);
            }
        }

        private void followPlayer()
        {
            if (pet.CurrentBehavior != Pet.behavior_walking)
            {
                pet.CurrentBehavior = Pet.behavior_walking;
            }

            if (currentPath == null || savedFarmerPosition != farmer.getTileLocation())
            {
                generatePath();
            }

            if (currentPath != null && currentPath.Count > 0)
            {
                var target = PathFindingUtil.getTargetPosition(currentPath.First());
                var currentPos = pet.position;

                if (Vector2.Distance(currentPos, target) < 4)
                {
                    currentPath.Remove(currentPath.First());
                    pet.position = target;
                }
                else
                {
                    var velocity = Utility.getVelocityTowardPoint(currentPos, target, 4);
                    pet.xVelocity = velocity.X;
                    pet.yVelocity = velocity.Y * -1;


                    if (Math.Abs(velocity.X) > Math.Abs(velocity.Y))
                    {
                        pet.facingDirection = velocity.X >= 0 ? 1 : 3;
                    }
                    else
                    {
                        pet.facingDirection = velocity.Y >= 0 ? 2 : 0;
                    }

                    pet.setMovingInFacingDirection();
                    pet.animateInFacingDirection(Game1.currentGameTime);
                }
            }
            else
            {
                currentPath = null;
                provider.forceUpdate();
            }
        }

        private void generatePath()
        {
            try
            {
                currentPath = PathFindingUtil.FindPathToFarmer(pet, farmer);

                // save farmer position, don't include in final pathing
                savedFarmerPosition = currentPath.Last();
                currentPath.Remove(savedFarmerPosition);
                currentPath.Remove(currentPath.Last());
            }
            catch (Exception)
            {
                
                currentPath = null;
                provider.setCurrentAction(Action.SITTING);
            }
        }
    }
}
