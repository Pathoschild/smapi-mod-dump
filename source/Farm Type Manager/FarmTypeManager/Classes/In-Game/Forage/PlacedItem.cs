using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Netcode;

using Microsoft.Xna.Framework.Graphics;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A terrain feature representing an <see cref="StardewValley.Item"/> placed on the ground, similar to <see cref="StardewValley.Object"/>.</summary>
        /// <remarks>This class is not currently designed to be saved by the game's native processes. All instances should be removed from the game before saving (i.e. end of day).</remarks>
        public class PlacedItem : TerrainFeature
        {
            [XmlElement("Item")]
            public readonly NetRef<Item> item = new NetRef<Item>(null);
            /// <summary>The item represented by this class.</summary>
            public Item Item
            {
                get
                {
                    return item.Value;
                }

                set
                {
                    item.Value = value;
                }
            }

            public PlacedItem()
                : base(false) //needsTick = false, because this class doesn't use tick updates
            {
                initNetFields();
            }

            /// <summary>Create a new placed item.</summary>
            /// <param name="tileLocation">The tile location of the placed item.</param>
            /// <param name="item">The item contained by this object.</param>
            public PlacedItem(Vector2 tileLocation, Item item)
                : this() //call this class's default constructor
            {
                currentTileLocation = tileLocation;
                this.item.Value = item;
            }

            protected virtual void initNetFields()
            {
                NetFields.AddFields(item); //include this class's custom field
            }

            public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
            {
                if (Item != null)
                {
                    Vector2 screenPosition = Game1.GlobalToLocal(tileLocation * 64);
                    Item.drawInMenu(spriteBatch, screenPosition, 1f, 1f, 0.01f, StackDrawType.Hide);
                }
                else if (currentLocation != null) //if this 
                {
                    currentLocation.terrainFeatures.Remove(tileLocation); //remove it from the game
                }
            }

            /// <summary>Determines whether a character is obstructed by this placed item when moving through it.</summary>
            public override bool isPassable(Character c = null)
            {
                if (Constants.TargetPlatform == GamePlatform.Android) //if this is SDV Android
                    return true; //always treat placed items as passable
                else
                    return base.isPassable(c);
            }

            /// <summary>Performs behavior when a character collides with this placed item.</summary>
            public override void doCollisionAction(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who, GameLocation location)
            {
                if (Constants.TargetPlatform == GamePlatform.Android) //if this is running on Android
                {
                    if (who is Farmer farmer && farmer.IsLocalPlayer) //if the colliding character is the local player
                    {
                        performUseAction(tileLocation, location); //attempt to pick up the item
                    }
                }
            }

            /// <summary>Performs behavior when a player "grabs" this placed item.</summary>
            /// <remarks>This method does not seem to be properly invoked by Android.
            /// <see cref="isPassable(Character)"/>and <see cref="doCollisionAction(Rectangle, int, Vector2, Character, GameLocation)"/> are used as a conditional workaround.</remarks>
            public override bool performUseAction(Vector2 tileLocation, GameLocation location)
            {
                SetForageQuality(location); //if this is forage, set its quality
                if (!Game1.player.canMove || this.isTemporarilyInvisible || !Game1.player.couldInventoryAcceptThisItem(Item)) //if this isn't the local player OR they can't currently pick this up
                    return false; //this placed item was not used

                //add the contained item to the player's inventory and remove this placed item
                if (Game1.player.addItemToInventoryBool(Item, true)) //add this item to the player's inventory; if successful,
                {
                    OnForagePickup(Item, location); //if this is forage, perform related tasks

                    location.localSound("pickUpItem");
                    DelayedAction.playSoundAfterDelay("coin", 300, location, -1);
                    Game1.player.animateOnce(279 + Game1.player.FacingDirection); //do the player's "pick up object" animation

                    if (Item.GetType() != typeof(StardewValley.Object) || (Item as StardewValley.Object).bigCraftable) //if this item is anything other than a basic StardewValley.Object
                    {
                        //prevent displaying the item during the "pick up object" animation
                        for (int x = 0; x < Game1.player.FarmerSprite.CurrentAnimation.Count; x++) //modify each frame of the player's "pick up object" animation
                        {
                            Game1.player.FarmerSprite.CurrentAnimation[x] = new FarmerSprite.AnimationFrame(
                                Game1.player.FarmerSprite.CurrentAnimation[x].frame,
                                Game1.player.FarmerSprite.CurrentAnimation[x].milliseconds,
                                Game1.player.FarmerSprite.CurrentAnimation[x].secondaryArm,
                                Game1.player.FarmerSprite.CurrentAnimation[x].flip,
                                null, //null the delegate that normally calls Farmer.showItemIntake (which always uses "springobjects" images)
                                false);
                        }
                    }

                    Item = null; //clear this placed item's reference to the item
                    currentLocation.terrainFeatures.Remove(tileLocation); //remove this placed item from the game
                }  

                return true; //this placed item was used
            }

            /// <summary>Assigns quality to this item if it is a forage object based on the local player's Foraging skill and professions.</summary>
            /// <param name="location">The current location of this item.</param>
            private void SetForageQuality(GameLocation location)
            {
                if (Item is StardewValley.Object obj && obj.isForage(location)) //if this item is forage
                {
                    //determine forage quality
                    if (Game1.player.professions.Contains(Farmer.botanist)) //if the player has the Botanist profession
                        obj.Quality = StardewValley.Object.bestQuality; //maximize the object's quality
                    else
                    {
                        //imitate Stardew's random seed, which produces the same result when give the same object position and in-game day
                        Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)currentTileLocation.X + (int)currentTileLocation.Y * 777);
                        
                        //set random qualities based on the player's Foraging skill
                        if (random.NextDouble() < Game1.player.ForagingLevel / 30.0)
                            obj.Quality = StardewValley.Object.highQuality; //gold quality
                        else if (random.NextDouble() < Game1.player.ForagingLevel / 15.0)
                            obj.Quality = StardewValley.Object.medQuality; //silver quality
                        else
                            obj.Quality = StardewValley.Object.lowQuality; //bronze quality, a.k.a. normal/no quality
                    }
                }
            }

            /// <summary>Performs post-pickup forage object behaviors. These include Gatherer profession effects, experience gain, and statistics updates.</summary>
            /// <param name="item">The possible forage item.</param>
            /// <param name="location">The item's current location.</param>
            private void OnForagePickup(Item item, GameLocation location)
            {
                if (Item is StardewValley.Object obj && obj.isForage(location)) //if this is a forage object
                {
                    int totalStackSize = obj.Stack;

                    //check the "double forage" profession chance
                    if (Game1.player.professions.Contains(Farmer.gatherer) && Utility.RNG.NextDouble() < 0.2) //20% chance if the player has the Gatherer profession
                    {
                        totalStackSize *= 2; //double the recorded total stack size

                        Item secondStack = Item.getOne(); //copy the item
                        secondStack.Stack = Item.Stack; //copy the original stack size

                        if (!Game1.player.addItemToInventoryBool(secondStack, true)) //add the second stack to the player's inventory; if there is not enough space for all of it,
                        {
                            //drop the remaining stack on the ground
                            Point centerOfPlayer = Game1.player.GetBoundingBox().Center; //get the player's center
                            Vector2 dropPosition = new Vector2(centerOfPlayer.X, centerOfPlayer.Y); //create a Vector2 of the player's center
                            Game1.createItemDebris(secondStack, dropPosition, Game1.player.FacingDirection, location); //drop the item at the player's position
                        }
                    }

                    if (location.isFarmBuildingInterior()) //if this is was collected inside a farm building
                        Game1.player.gainExperience(Farmer.farmingSkill, 5 * totalStackSize); //gain farming experience multiplied by stack size
                    else
                        Game1.player.gainExperience(Farmer.foragingSkill, 7 * totalStackSize); //gain foraging experience multiplied by stack size

                    Game1.stats.ItemsForaged += (uint)totalStackSize; //increase the "items foraged" stat by the object's stack size
                }
            }
        }
    }
}
