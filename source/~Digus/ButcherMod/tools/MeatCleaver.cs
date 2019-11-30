using System;
using System.Collections.Generic;
using AnimalHusbandryMod.animals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Tools;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using PyTK.CustomElementHandler;

namespace AnimalHusbandryMod.tools
{
    public class MeatCleaver : Tool, ISaveElement
    {

        private FarmAnimal _animal;
        private FarmAnimal _tempAnimal;

        public new static int initialParentTileIndex = 504;
        public new static int indexOfMenuItemView = 530;

        private string _sufix = "";

        public MeatCleaver() : base("Meat Cleaver", 0, initialParentTileIndex, indexOfMenuItemView, false, 0)
        {
            if (DataLoader.ModConfig.Softmode)
            {
                _sufix = ".Soft";
            }
        }

        public override Item getOne()
        {
            return (Item)new MeatCleaver();
        }

        protected override string loadDisplayName()
        {
            return DataLoader.i18n.Get("Tool.MeatCleaver.Name"+ _sufix);
        }

        protected override string loadDescription()
        {
            return DataLoader.i18n.Get("Tool.MeatCleaver.Description"+ _sufix);
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            x = (int) who.GetToolLocation(false).X;
            y = (int) who.GetToolLocation(false).Y;
            Rectangle rectangle = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize,
                Game1.tileSize);

            if (!DataLoader.ModConfig.DisableMeat)
            {
                if (location is Farm)
                {
                    foreach (FarmAnimal farmAnimal in (location as Farm).animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            if (farmAnimal == this._tempAnimal)
                            {
                                this._animal = farmAnimal;
                                break;
                            }
                            else
                            {
                                this._tempAnimal = farmAnimal;
                                ICue hurtSound;
                                if (!DataLoader.ModConfig.Softmode)
                                {
                                    if (farmAnimal.sound.Value != null)
                                    {
                                        hurtSound = Game1.soundBank.GetCue(farmAnimal.sound.Value);
                                        hurtSound.SetVariable("Pitch", 1800);
                                        hurtSound.Play();
                                    }
                                }
                                else
                                {
                                    hurtSound = Game1.soundBank.GetCue("toolCharge");
                                    hurtSound.SetVariable("Pitch", 5000f);
                                    hurtSound.Play();
                                }
                                
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
                            if (farmAnimal == this._tempAnimal)
                            {
                                this._animal = farmAnimal;
                                break;
                            }
                            else
                            {
                                this._tempAnimal = farmAnimal;

                                ICue hurtSound;
                                if (!DataLoader.ModConfig.Softmode)
                                {
                                    if (farmAnimal.sound.Value != null)
                                    {
                                        hurtSound = Game1.soundBank.GetCue(farmAnimal.sound.Value);
                                        hurtSound.SetVariable("Pitch", 1800);
                                        hurtSound.Play();
                                    }
                                }
                                else
                                {
                                    hurtSound = Game1.soundBank.GetCue("toolCharge");
                                    hurtSound.SetVariable("Pitch", 5000f);
                                    hurtSound.Play();
                                }

                                
                                break;
                            }
                        }
                    }
                }                
            }

            this.Update(who.facingDirection, 0, who);
            if (this._tempAnimal != null && this._tempAnimal.age.Value < (int)this._tempAnimal.ageWhenMature.Value)
            {
                string dialogue = DataLoader.i18n.Get("Tool.MeatCleaver.TooYoung"+_sufix, new { animalName = this._tempAnimal.displayName });
                DelayedAction.showDialogueAfterDelay(dialogue, 150);
                this._tempAnimal = null;
            }
            who.EndUsingTool();
            return true;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            base.DoFunction(location, x, y, power, who);
            who.Stamina -= ((float)4f - (float)who.FarmingLevel * 0.2f);
            if (this._animal != null && !MeatController.CanGetMeatFrom(this._animal))
            {
                return;
            }

            if (this._animal != null && this._animal.age.Value >= (int) this._animal.ageWhenMature.Value)
            {
                (this._animal.home.indoors.Value as AnimalHouse)?.animalsThatLiveHere.Remove(this._animal.myID.Value);
                this._animal.health.Value = -1;
                int numClouds = this._animal.frontBackSourceRect.Width / 2;
                int cloudSprite = !DataLoader.ModConfig.Softmode ? 5 : 10;
                Multiplayer multiplayer = DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                for (int i = 0; i < numClouds; i++)
                {
                    int nonRedness = Game1.random.Next(0, 80);
                    Color cloudColor = new Color(255, 255 - nonRedness, 255 - nonRedness); ;

                    

                    multiplayer.broadcastSprites(
                        Game1.player.currentLocation
                        , new TemporaryAnimatedSprite[1]{
                            new TemporaryAnimatedSprite
                            (
                                cloudSprite
                                ,this._animal.position +new Vector2(Game1.random.Next(-Game1.tileSize / 2, this._animal.frontBackSourceRect.Width * 3)
                                ,Game1.random.Next(-Game1.tileSize / 2, this._animal.frontBackSourceRect.Height * 3))
                                ,cloudColor
                                , 8
                                , false,
                                Game1.random.NextDouble() < .5 ? 50 : Game1.random.Next(30, 200), 0, Game1.tileSize
                                , -1
                                ,Game1.tileSize, Game1.random.NextDouble() < .5 ? 0 : Game1.random.Next(0, 600)
                            )
                            {
                                scale = Game1.random.Next(2, 5) * .25f,
                                alpha = Game1.random.Next(2, 5) * .25f,
                                motion = new Vector2(0, (float) -Game1.random.NextDouble())
                            }
                        }
                    );
                }
                Color animalColor;
                float alfaFade;
                if (!DataLoader.ModConfig.Softmode)
                {
                    animalColor = Color.LightPink;
                    alfaFade = .025f;
                }
                else
                {
                    animalColor = Color.White;
                    alfaFade = .050f;
                }
                multiplayer.broadcastSprites(
                    Game1.player.currentLocation
                    , new TemporaryAnimatedSprite[1]{
                        new TemporaryAnimatedSprite
                        (
                            this._animal.Sprite.textureName.Value
                            ,this._animal.Sprite.SourceRect
                            , this._animal.position
                            , this._animal.FacingDirection == Game1.left
                            , alfaFade
                            , animalColor
                        )
                        {
                            scale = 4f
                        }
                    }
                );
                if (!DataLoader.ModConfig.Softmode)
                {
                    Game1.playSound("killAnimal");
                } else
                {
                    ICue warptSound = Game1.soundBank.GetCue("wand");
                    warptSound.SetVariable("Pitch", 1800);
                    warptSound.Play();
                }

                MeatController.ThrowItem(MeatController.CreateMeat(this._animal), this._animal);
                who.gainExperience(0, 5);
                this._animal = (FarmAnimal)null;
                this._tempAnimal = (FarmAnimal)null;
            }
        }

        public object getReplacement()
        {
            Object replacement = new Object(169,1);
            return replacement;
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
