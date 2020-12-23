/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using PyTK.CustomElementHandler;
using StardewValley.Characters;
using StardewValley.Locations;
using AnimalHusbandryMod.animals;
using Netcode;
using StardewModdingAPI;
using StardewValley.Objects;

namespace AnimalHusbandryMod.tools
{
    public class FeedingBasket : MilkPail, ISaveElement
    {
        private FarmAnimal _animal;
        private Pet _pet;

        public static int InitialParentTileIndex = 519;
        public static int IndexOfMenuItemView = 519;
        public static int AttachmentMenuTile = 69;

        public FeedingBasket() : base()
        {
            this.Name = "Feeding Basket";
            this.initialParentTileIndex.Value = InitialParentTileIndex;
            this.indexOfMenuItemView.Value = IndexOfMenuItemView;
            this.Stackable = false;
            this.CurrentParentTileIndex = initialParentTileIndex;
            this.numAttachmentSlots.Value = 1;
            this.attachments.SetCount(numAttachmentSlots);
            this.Category = -99;
        }

        public override Item getOne()
        {
            return (Item)new FeedingBasket();
        }

        protected override string loadDisplayName()
        {
            return DataLoader.i18n.Get("Tool.FeedingBasket.Name");
        }

        protected override string loadDescription()
        {
            return DataLoader.i18n.Get("Tool.FeedingBasket.Description");
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
            // Added this because for some wierd reason the current value appears subtracted by 5 the first time the tool is used.
            this.CurrentParentTileIndex = InitialParentTileIndex;

            if (Context.IsMainPlayer && !DataLoader.ModConfig.DisableTreats)
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

            if (this._animal != null)
            {
                string dialogue = "";
                if (this.attachments[0] == null)
                {
                    Game1.showRedMessage(DataLoader.i18n.Get("Tool.FeedingBasket.Empty"));
                    this._animal = null;
                }
                else if (!TreatsController.CanReceiveTreat(this._animal))
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.NotLikeTreat", new { itemName = this.attachments[0].DisplayName });
                }
                else if (!TreatsController.IsLikedTreat(this._animal, this.attachments[0].ParentSheetIndex) && !TreatsController.IsLikedTreat(this._animal, this.attachments[0].Category))
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.NotLikeTreat", new  { itemName = this.attachments[0].DisplayName});
                }
                else if (this.attachments[0].Category == StardewValley.Object.MilkCategory && !this._animal.isBaby())
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.OnlyBabiesCanEatMilk");
                }
                else if (!TreatsController.IsReadyForTreat(this._animal))
                {
                    if (TreatsController.GetTreatItem(this._animal)?.MinimumDaysBetweenTreats == 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.AlreadyAteTreatToday", new { animalName = this._animal.displayName });
                    }
                    else
                    {
                        int daysUntilNextTreat = TreatsController.DaysUntilNextTreat(this._animal);
                        if (daysUntilNextTreat > 1)
                        {
                            dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatInDays", new { animalName = this._animal.displayName ,  numberOfDays = daysUntilNextTreat });
                        }
                        else if (daysUntilNextTreat == 1)
                        {
                            dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatTomorrow", new { animalName = this._animal.displayName });
                        }
                    }
                }
                else
                {
                    this._animal.pauseTimer = 1000;               
                }


                if (dialogue.Length > 0)
                {
                    DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    this._animal = null;
                }
            }
            if (this._pet != null)
            {
                string dialogue = "";
                if (this.attachments[0] == null)
                {
                    Game1.showRedMessage(DataLoader.i18n.Get("Tool.FeedingBasket.Empty"));
                    this._pet = null;
                }
                else if (!TreatsController.IsLikedTreatPet(this.attachments[0].ParentSheetIndex) && !TreatsController.IsLikedTreatPet(this.attachments[0].Category))
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.NotLikeTreat", new { itemName = this.attachments[0].DisplayName });
                }
                else if (!TreatsController.IsReadyForTreatPet())
                {
                    int daysUntilNextTreat = TreatsController.DaysUntilNextTreatPet();

                    if (DataLoader.AnimalData.Pet.MinimumDaysBetweenTreats == 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.AlreadyAteTreatToday", new { animalName = this._pet.displayName });
                    }
                    else if (daysUntilNextTreat > 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatInDays", new { animalName = this._pet.displayName, numberOfDays = daysUntilNextTreat });
                    }
                    else if (daysUntilNextTreat == 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatTomorrow", new { animalName = this._pet.displayName });
                    }
                }
                else
                {
                    _pet.Halt();
                    _pet.CurrentBehavior = 0;
                    _pet.OnNewBehavior();
                    _pet.Halt();
                    _pet.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>(){ new FarmerSprite.AnimationFrame(18, 200) });

                }


                if (dialogue.Length > 0)
                {
                    DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    this._pet = null;
                }
            }

            
            who.Halt();
            int currentFrame = who.FarmerSprite.currentFrame;
            if (this._animal != null || this._pet != null)
            {
                switch (who.FacingDirection)
                {
                    case 0:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(62, 900, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 1:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(58, 900, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 2:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(54, 900, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 3:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(58, 900, false, true, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                }
            } else
            {                
                who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(currentFrame, 0, false, who.FacingDirection == 3, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
            }
            who.FarmerSprite.oldFrame = currentFrame;
            who.UsingTool = true;
            who.CanMove = false;

            if (this._animal != null || this._pet != null)
            {
                Rectangle boundingBox;
                if (this._animal != null)
                {
                    boundingBox = this._animal.GetBoundingBox();
                }
                else
                {
                    boundingBox = this._pet.GetBoundingBox();
                }

                double numX = boundingBox.Center.X;
                double numY = boundingBox.Center.Y;

                Vector2 vectorBasket = new Vector2((float) numX - 32, (float) numY);
                Vector2 vectorFood = new Vector2((float) numX - 24, (float) numY - 10);
                var foodScale = Game1.pixelZoom * 0.75f;

                Multiplayer multiplayer = DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

                TemporaryAnimatedSprite basketSprite = new TemporaryAnimatedSprite(Game1.toolSpriteSheetName,
                    Game1.getSourceRectForStandardTileSheet(Game1.toolSpriteSheet, this.CurrentParentTileIndex, 16, 16),
                    750.0f, 1, 1, vectorBasket, false, false, ((float) boundingBox.Bottom + 0.1f) / 10000f, 0.0f,
                    Color.White, Game1.pixelZoom, 0.0f, 0.0f, 0.0f) {delayBeforeAnimationStart = 100};
                multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1]{basketSprite});
                TemporaryAnimatedSprite foodSprite = new TemporaryAnimatedSprite(Game1.objectSpriteSheetName,
                    Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,this.attachments[0].ParentSheetIndex,
                    16, 16), 500.0f, 1, 1, vectorFood, false, false,((float) boundingBox.Bottom + 0.2f) / 10000f, 0.0f,
                    Color.White, foodScale, 0.0f, 0.0f, 0.0f) {delayBeforeAnimationStart = 100};
                multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1] { foodSprite });

                for (int index = 0; index < 8; ++index)
                {
                    Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                        this.attachments[0].ParentSheetIndex, 16, 16);
                    standardTileSheet.X += 8;
                    standardTileSheet.Y += 8;

                    standardTileSheet.Width = Game1.pixelZoom;
                    standardTileSheet.Height = Game1.pixelZoom;
                    TemporaryAnimatedSprite temporaryAnimatedSprite2 =
                        new TemporaryAnimatedSprite(Game1.objectSpriteSheetName, standardTileSheet, 400f, 1, 0,
                            vectorFood + new Vector2(12, 12), false, false,
                            ((float) boundingBox.Bottom + 0.2f) / 10000f, 0.0f, Color.White, (float) foodScale, 0.0f,
                            0.0f, 0.0f, false)
                        {
                            motion = new Vector2((float) Game1.random.Next(-30, 31) / 10f,(float) Game1.random.Next(-6, -3)),
                            acceleration = new Vector2(0.0f, 0.5f),
                            delayBeforeAnimationStart = 600
                        };
                    multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1] { temporaryAnimatedSprite2 });
                }

                if (this._animal != null)
                {
                    FarmAnimal tempAnimal = this._animal;
                    Game1.delayedActions.Add(new DelayedAction(300, new DelayedAction.delayedBehavior(() => {
                        AnimalHusbandryModEntry.ModHelper.Reflection.GetField<NetBool>(tempAnimal, "isEating").GetValue().Value = true;
                        tempAnimal.Sprite.loop = false;
                    })));
                }
                else if (this._pet != null)
                {
                    
                    _pet.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(18, 300),
                        new FarmerSprite.AnimationFrame(17, 100),
                        new FarmerSprite.AnimationFrame(16, 100),
                        new FarmerSprite.AnimationFrame(0, 100),
                        new FarmerSprite.AnimationFrame(16, 100),
                        new FarmerSprite.AnimationFrame(17, 100),
                        new FarmerSprite.AnimationFrame(18, 300)
                    });
                    _pet.Sprite.loop = false;
                }
                DelayedAction.playSoundAfterDelay("eat", 600);
            }

            return true;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            this.lastUser = who;
            Game1.recentMultiplayerRandom = new Random((int)(short)Game1.random.Next((int)short.MinValue, 32768));
            this.CurrentParentTileIndex = InitialParentTileIndex;
            this.indexOfMenuItemView.Value = IndexOfMenuItemView;

            if (this._animal != null)
            {
                this._animal.doEmote(20, true);

                if (!DataLoader.ModConfig.DisableFriendshipInscreseWithTreats)
                {
                    double professionAjust = 1.0;
                    if (!_animal.isCoopDweller() && who.professions.Contains(3) || _animal.isCoopDweller() && who.professions.Contains(2))
                    {
                        professionAjust += DataLoader.ModConfig.PercentualAjustOnFriendshipInscreaseFromProfessions;
                    }
                    this._animal.friendshipTowardFarmer.Value = Math.Min(1000, this._animal.friendshipTowardFarmer.Value + (int)Math.Ceiling(professionAjust * this.attachments[0].Price * (1.0 + this.attachments[0].Quality * 0.25) / (this._animal.price.Value / 1000.0)));
                }
                if (!DataLoader.ModConfig.DisableMoodInscreseWithTreats)
                {
                    this._animal.happiness.Value = byte.MaxValue;
                }

                if (DataLoader.ModConfig.EnableTreatsCountAsAnimalFeed)
                {
                    this._animal.fullness.Value = byte.MaxValue;
                }

                TreatsController.FeedAnimalTreat(this._animal, this.attachments[0]);

                if (this.attachments[0].Category == StardewValley.Object.MilkCategory)
                {
                    this._animal.age.Value = Math.Min(this._animal.ageWhenMature.Value - 1, this._animal.age.Value + 1);
                }

                --this.attachments[0].Stack;
                if (this.attachments[0].Stack <= 0)
                {
                    Game1.showGlobalMessage(DataLoader.i18n.Get("Tool.FeedingBasket.ItemConsumed", new { itemName = this.attachments[0].DisplayName }));
                    this.attachments[0] = (StardewValley.Object)null;
                }
            }
            else if (this._pet != null)
            {
                this._pet.doEmote(20, true);

                if (!DataLoader.ModConfig.DisableFriendshipInscreseWithTreats)
                {
                    this._pet.friendshipTowardFarmer.Value = Math.Min(Pet.maxFriendship, this._pet.friendshipTowardFarmer.Value + 6);
                }
                TreatsController.FeedPetTreat(this.attachments[0]);

                --this.attachments[0].Stack;
                if (this.attachments[0].Stack <= 0)
                {
                    Game1.showGlobalMessage(DataLoader.i18n.Get("Tool.FeedingBasket.ItemConsumed", new { itemName = this.attachments[0].DisplayName }));
                    this.attachments[0] = (StardewValley.Object)null;
                }
            }
            this._animal = null;
            this._pet = null;

            if (Game1.activeClickableMenu == null)
            {
                who.CanMove = true;
                who.completelyStopAnimatingOrDoingAction();
            }
            else
            {
                who.Halt();
            }
            who.UsingTool = false;
            who.canReleaseTool = true;

            DataLoader.Helper.Reflection.GetMethod(this, "finish").Invoke();
        }

        

        public override bool canThisBeAttached(StardewValley.Object o)
        {
            if (o == null 
                || o.Category == StardewValley.Object.VegetableCategory 
                || o.Category == StardewValley.Object.FruitsCategory 
                || TreatsController.IsLikedTreat(o.ParentSheetIndex)
                || TreatsController.IsLikedTreat(o.Category)
                )
            {
                return true;
            }
            return false;
        }

        public override StardewValley.Object attach(StardewValley.Object o)
        {
            if (o != null)
            {
                StardewValley.Object @object = this.attachments[0];
                if (@object != null && @object.canStackWith((Item)o))
                {
                    @object.Stack = o.addToStack(@object);
                    if (@object.Stack <= 0)
                        @object = (StardewValley.Object)null;
                }
                this.attachments[0] = o;
                Game1.playSound("button1");
                return @object;
            }
            else
            {
                if (this.attachments[0] != null)
                {
                    StardewValley.Object attachment = this.attachments[0];
                    this.attachments[0] = (StardewValley.Object)null;
                    Game1.playSound("dwop");
                    return attachment;
                }
            }
            return (StardewValley.Object)null;
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {

            if (attachments[0] != null)
            {
                b.Draw(Game1.menuTexture, new Vector2((float)x, (float)y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
                attachments[0].drawInMenu(b, new Vector2((float)x, (float)y), 1f);
            }
            else
            {
                b.Draw(Game1.menuTexture, new Vector2((float)x, (float)y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, AttachmentMenuTile, -1, -1)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
            }
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("name", this.Name);
            return savedata;
       }

        public object getReplacement()
        {
            var replacement = new Chest(true);
            if (attachments.Any() && attachments[0] != null)
            {
                replacement.addItem(attachments[0]);
            }
            return replacement;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            if (replacement is Chest c && c.items.Any())
            {
                this.attachments.Add((StardewValley.Object)c.items[0]);
            }
            else if (replacement is Tool tool)
            {
                this.attachments.Set(tool.attachments);
            }
            this.Name = additionalSaveData["name"];
        }
    }
}
