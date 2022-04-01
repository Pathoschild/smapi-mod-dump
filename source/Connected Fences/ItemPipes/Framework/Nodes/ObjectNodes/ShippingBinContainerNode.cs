/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using ItemPipes.Framework.Model;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using Netcode;
using System.Collections.Generic;
using System.Linq;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Items;
using System;
using SObject = StardewValley.Object;


namespace ItemPipes.Framework.Nodes.ObjectNodes
{
    public class ShippingBinContainerNode : ContainerNode
    {
        public ShippingBin ShippingBin { get; set; }
        public Farm Farm { get; set; }
        public ShippingBinContainerNode() { }

        public ShippingBinContainerNode(Vector2 position, GameLocation location, StardewValley.Object obj, Building building) : base(position, location, obj)
        {

            Name = building.buildingType.ToString();
            if (building is ShippingBin)
            {
                ShippingBin = (ShippingBin)building;
            }
            Farm = Game1.getFarm();
            Filter = new NetObjectList<Item>();
            Type = "ShippingBin";

        }



        public bool ShipItem(Item item)
        {
            bool shipped = false;
            if (item != null && item is StardewValley.Object && Farm != null)
            {
                Farm.getShippingBin(Game1.MasterPlayer).Add(item);
                ShippingBin.showShipment(item as StardewValley.Object, playThrowSound: false);
                Farm.lastItemShipped = item;
                shipped = true;
            }
            return shipped;
        }

        public void ShowShipment(SObject o, bool playThrowSound = true)
        {
            if (Farm != null)
            {
                if (playThrowSound)
                {
                    Farm.localSound("backpackIN");
                }
                DelayedAction.playSoundAfterDelay("Ship", playThrowSound ? 250 : 0);

                if (o is CustomObjectItem)
                {
                    //Need texture item to be required, not path
                    int temp = Game1.random.Next();
                    Farm.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(524, 218, 34, 22), new Vector2((int)ShippingBin.tileX, (int)ShippingBin.tileY - 1) * 64f + new Vector2(-1f, 5f) * 4f, flipped: false, 0f, Color.White)
                    {
                        interval = 100f,
                        totalNumberOfLoops = 1,
                        animationLength = 3,
                        pingPong = true,
                        alpha = 1f,
                        scale = 4f,
                        layerDepth = (float)(((int)ShippingBin.tileY + 1) * 64) / 10000f + 0.0002f,
                        id = temp,
                        extraInfoForEndBehavior = temp,
                        endFunction = Farm.removeTemporarySpritesWithID
                    });
                }
                else
                {
                    int temp = Game1.random.Next();
                    Farm.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(524, 218, 34, 22), new Vector2((int)ShippingBin.tileX, (int)ShippingBin.tileY - 1) * 64f + new Vector2(-1f, 5f) * 4f, flipped: false, 0f, Color.White)
                    {
                        interval = 100f,
                        totalNumberOfLoops = 1,
                        animationLength = 3,
                        pingPong = true,
                        alpha = 1f,
                        scale = 4f,
                        layerDepth = (float)(((int)ShippingBin.tileY + 1) * 64) / 10000f + 0.0002f,
                        id = temp,
                        extraInfoForEndBehavior = temp,
                        endFunction = Farm.removeTemporarySpritesWithID
                    });
                    Farm.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(524, 230, 34, 10), new Vector2((int)ShippingBin.tileX, (int)ShippingBin.tileY - 1) * 64f + new Vector2(-1f, 17f) * 4f, flipped: false, 0f, Color.White)
                    {
                        interval = 100f,
                        totalNumberOfLoops = 1,
                        animationLength = 3,
                        pingPong = true,
                        alpha = 1f,
                        scale = 4f,
                        layerDepth = (float)(((int)ShippingBin.tileY + 1) * 64) / 10000f + 0.0003f,
                        id = temp,
                        extraInfoForEndBehavior = temp
                    });
                    Farm.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, o.parentSheetIndex, 16, 16), new Vector2((int)ShippingBin.tileX, (int)ShippingBin.tileY - 1) * 64f + new Vector2(7 + Game1.random.Next(6), 2f) * 4f, flipped: false, 0f, Color.White)
                    {
                        interval = 9999f,
                        scale = 4f,
                        alphaFade = 0.045f,
                        layerDepth = (float)(((int)ShippingBin.tileY + 1) * 64) / 10000f + 0.000225f,
                        motion = new Vector2(0f, 0.3f),
                        acceleration = new Vector2(0f, 0.2f),
                        scaleChange = -0.05f
                    });
                }
            }
        }

        public override NetObjectList<Item> UpdateFilter(NetObjectList<Item> filteredItems)
        {
            Filter = new NetObjectList<Item>();
            if (filteredItems == null)
            {
                Filter.Add(Farm.lastItemShipped);
            }
            else
            {
                foreach (Item item in filteredItems.ToList())
                {
                    Filter.Add(item);
                }
            }
            return Filter;

        }

        public override bool CanSendItems()
        {
            return !IsEmpty();
        }
        public override bool CanRecieveItems()
        {
            return true;
        }
        public override bool CanRecieveItem(Item item)
        {
            if (item is Tool)
            {
                return false;
            }
            return true;
        }
        public override bool CanStackItem(Item item)
        {
            return false;
        }
        public override bool InsertItem(Item item)
        {
            return ShipItem(item);
        }

        public override Item GetItemForInput(InputPipeNode input, int flux)
        {
            Item item = null;
            if (input != null)
            {
                NetCollection<Item> itemList = Farm.getShippingBin(Game1.MasterPlayer);
                int index = itemList.Count - 1;
                if (CanSendItems() && Farm.lastItemShipped != null)
                {
                    if (input.HasFilter())
                    {
                        if (input.Filter.Any(i => i.Name.Equals(itemList[index].Name)))
                        {
                            item = TryExtractItem(input.ConnectedContainer, itemList);
                        }
                    }
                    else
                    {

                        item = TryExtractItem(input.ConnectedContainer, itemList);
                    }
                }
            }
            return item;
        }
        public Item TryExtractItem(ContainerNode input, NetCollection<Item> itemList)
        {
            Printer.Info("ShippingBin");
            //Exception for multiple thread collisions
            Item source = itemList.Last();
            Item tosend = null;
            if (source is SObject)
            {
                SObject obj = (SObject)source;
                SObject tosendObject = (SObject)tosend;
                if (input.CanRecieveItem(source))
                {
                    tosendObject = obj;
                    itemList.Remove(itemList.Last());
                    Farm.lastItemShipped = itemList.Last();
                    if (itemList.Count == 1)
                    {
                        Farm.lastItemShipped = null;
                    }
                    else
                    {
                        Farm.lastItemShipped = itemList.Last();
                    }
                    return tosendObject;
                }
            }
            else if (source is Tool)
            {
                /*
                Tool tool = (Tool)source;
                Tool tosendTool = (Tool)tosend;
                if (input.CanRecieveItems())
                {
                    tosendTool = tool;
                    itemList.RemoveAt(index);
                }
                Chest.clearNulls();
                */
                return null;
                
            }
            return null;
        }

        public override bool IsEmpty()
        {
            if (Farm.getShippingBin(Game1.MasterPlayer).Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool CanStackItems()
        {
            return true;
        }
    }
}
