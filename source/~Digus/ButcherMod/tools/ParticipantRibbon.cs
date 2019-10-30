using System.Collections.Generic;
using System.Linq;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Tools;

namespace AnimalHusbandryMod.tools
{
    public class ParticipantRibbon : MilkPail, ISaveElement
    {
        private FarmAnimal _animal;
        private Pet _pet;

        public static int InitialParentTileIndex = 520;
        public static int IndexOfMenuItemView = 520;

        public ParticipantRibbon() : base()
        {
            this.Name = "Participant Ribbon";
            this.initialParentTileIndex.Value = InitialParentTileIndex;
            this.indexOfMenuItemView.Value = IndexOfMenuItemView;
            this.Stackable = false;
            this.CurrentParentTileIndex = InitialParentTileIndex;
        }

        public override Item getOne()
        {
            return (Item)new ParticipantRibbon();
        }

        protected override string loadDisplayName()
        {
            return DataLoader.i18n.Get("Tool.ParticipantRibbon.Name");
        }

        protected override string loadDescription()
        {
            return DataLoader.i18n.Get("Tool.ParticipantRibbon.Description");
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            x = (int)who.GetToolLocation(false).X;
            y = (int)who.GetToolLocation(false).Y;
            Rectangle rectangle = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize);

            if (Context.IsMainPlayer && !DataLoader.ModConfig.DisableAnimalContest)
            {
                if (location is Farm)
                {
                    foreach (FarmAnimal farmAnimal in (location as Farm).animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            this._animal = farmAnimal;
                            break;
                        }
                    }
                    if (this._animal == null)
                    {
                        foreach (Pet pet in location.characters.Where(i => i is Pet))
                        {
                            if (pet.GetBoundingBox().Intersects(rectangle))
                            {
                                this._pet = pet;
                                break;
                            }
                        }
                    }
                }
                else if (location is AnimalHouse)
                {
                    foreach (FarmAnimal farmAnimal in (location as AnimalHouse).animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            this._animal = farmAnimal;
                            break;
                        }
                    }
                }
                else if (location is FarmHouse)
                {
                    foreach (Pet pet in location.characters.Where(i => i is Pet))
                    {
                        if (pet.GetBoundingBox().Intersects(rectangle))
                        {
                            this._pet = pet;
                            break;
                        }
                    }
                }
            }

            string dialogue = "";
            if (this._animal != null)
            {
                if (this._animal.isBaby())
                {
                    dialogue = DataLoader.i18n.Get("Tool.ParticipantRibbon.CantBeBaby");
                }
                else if (AnimalContestController.HasParticipated(this._animal))
                {
                    dialogue = DataLoader.i18n.Get("Tool.ParticipantRibbon.HasAlreadyParticipatedContest", new { animalName = this._animal.displayName });
                }
                else if (AnimalContestController.IsParticipant(this._animal))
                {
                    dialogue = DataLoader.i18n.Get("Tool.ParticipantRibbon.IsAlreadyParticipant", new { animalName = this._animal.displayName});
                }
                else if (AnimalContestController.GetNextContestParticipantId() is long nextContestParticipantId)
                {
                    string participantName = nextContestParticipantId != AnimalData.PetId
                        ? AnimalContestController.GetAnimal(nextContestParticipantId).displayName
                        : Game1.player.getPetName();
                    dialogue = DataLoader.i18n.Get("Tool.ParticipantRibbon.AnotherParticipantAlready", new { participantName });
                }
                else
                {
                    this._animal.doEmote(8, true);
                    this._animal.makeSound();
                    this._animal.pauseTimer = 200;
                }
            }
            if (this._pet != null)
            {
                if (AnimalContestController.IsParticipantPet())
                {
                    dialogue = DataLoader.i18n.Get("Tool.ParticipantRibbon.IsAlreadyParticipant",
                        new {animalName = this._pet.displayName});
                }
                else if (AnimalContestController.GetNextContestParticipantId() is long nextContestParticipantId)
                {
                    string participantName = nextContestParticipantId != AnimalData.PetId 
                        ? AnimalContestController.GetAnimal(nextContestParticipantId).displayName 
                        : Game1.player.getPetName();
                    dialogue = DataLoader.i18n.Get("Tool.ParticipantRibbon.AnotherParticipantAlready", new { participantName });
                }
                else
                {
                    this._pet.doEmote(8, true);
                    this._pet.playContentSound();
                    _pet.Halt();
                    _pet.CurrentBehavior = 0;
                    _pet.Halt();
                    _pet.Sprite.setCurrentAnimation(
                        new List<FarmerSprite.AnimationFrame>() {new FarmerSprite.AnimationFrame(18, 200)});

                }
            }
            if (dialogue.Length > 0)
            {
                DelayedAction.showDialogueAfterDelay(dialogue, 150);
                this._pet = null;
                this._animal = null;
            }

            who.Halt();
            int currentFrame = who.FarmerSprite.currentFrame;
            if (this._animal != null || this._pet != null)
            {
                switch (who.FacingDirection)
                {
                    case 0:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(62, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 1:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(58, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 2:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(54, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 3:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(58, 200, false, true, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                }
            }
            else
            {
                who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(currentFrame, 0, false, who.FacingDirection == 3, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
            }
            who.FarmerSprite.oldFrame = currentFrame;
            who.UsingTool = true;
            who.CanMove = false;

            return true;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            this.CurrentParentTileIndex = InitialParentTileIndex;
            this.indexOfMenuItemView.Value = IndexOfMenuItemView;

            if (this._animal != null)
            {
                AnimalContestController.MakeAnimalParticipant(this._animal);
                Game1.player.removeItemFromInventory(this);

            }
            else if (this._pet != null)
            {
                AnimalContestController.MakePetParticipant();
                Game1.player.removeItemFromInventory(this);
            }

            this._animal = null;
            this._pet = null;

            if (Game1.activeClickableMenu == null)
            {
                who.CanMove = true;
            }
            else
            {
                who.Halt();
            }
            who.UsingTool = false;
            who.canReleaseTool = true;
        }

        public object getReplacement()
        {
            return new Object(168, 1);
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("name", Name);
            return savedata;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            this.Name = additionalSaveData["name"];
        }
    }
}
