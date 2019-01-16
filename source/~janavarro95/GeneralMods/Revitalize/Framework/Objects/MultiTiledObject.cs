using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Objects;

namespace Revitalize.Framework.Objects
{
    public class MultiTiledObject : CustomObject
    {
        [JsonIgnore]
        public Dictionary<Vector2, StardewValley.Object> objects;

        public Dictionary<Vector2,Guid> childrenGuids;

        private int width;
        private int height;
        public int Width
        {
            get
            {
                return this.width+1;
            }
        }
        public int Height
        {
            get
            {
                return this.height+1;
            }
        }

        public MultiTiledObject()
        {
            this.objects = new Dictionary<Vector2, StardewValley.Object>();
            this.childrenGuids = new Dictionary<Vector2, Guid>();
            this.guid = Guid.NewGuid();
        }

        public MultiTiledObject(BasicItemInformation info)
            : base(info)
        {
            this.objects = new Dictionary<Vector2, StardewValley.Object>();
            this.childrenGuids = new Dictionary<Vector2, Guid>();
            this.guid = Guid.NewGuid();
        }

        public MultiTiledObject(BasicItemInformation info, Vector2 TileLocation)
            : base(info, TileLocation)
        {
            this.objects = new Dictionary<Vector2, StardewValley.Object>();
            this.childrenGuids = new Dictionary<Vector2, Guid>();
            this.guid = Guid.NewGuid();
        }

        public MultiTiledObject(BasicItemInformation info, Vector2 TileLocation, Dictionary<Vector2, MultiTiledComponent> ObjectsList)
            : base(info, TileLocation)
        {
            this.objects = new Dictionary<Vector2, StardewValley.Object>();
            this.childrenGuids = new Dictionary<Vector2, Guid>();
            foreach (var v in ObjectsList)
            {
                MultiTiledComponent component =(MultiTiledComponent) v.Value.getOne();
                this.addComponent(v.Key, (component as MultiTiledComponent));
            }
            this.guid = Guid.NewGuid();

        }

        public bool addComponent(Vector2 key, MultiTiledComponent obj)
        {
            if (this.objects.ContainsKey(key))
                return false;

            this.objects.Add(key, obj);
            this.childrenGuids.Add(key, Guid.NewGuid());

            if (key.X > this.width) this.width = (int)key.X;
            if (key.Y > this.height) this.height = (int)key.Y;
            (obj as MultiTiledComponent).containerObject = this;
            (obj as MultiTiledComponent).offsetKey = key;
            return true;
        }

        public bool removeComponent(Vector2 key)
        {
             

            if (!this.objects.ContainsKey(key))
                return false;

            this.objects.Remove(key);
            return true;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
                pair.Value.draw(spriteBatch, x + (int)pair.Key.X * Game1.tileSize, y + (int)pair.Key.Y * Game1.tileSize, alpha);
        }

        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
                pair.Value.draw(spriteBatch, xNonTile + (int)pair.Key.X * Game1.tileSize, yNonTile + (int)pair.Key.Y * Game1.tileSize, layerDepth, alpha);

            //base.draw(spriteBatch, xNonTile, yNonTile, layerDepth, alpha);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color c, bool drawShadow)
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
                pair.Value.drawInMenu(spriteBatch, location + (pair.Key * 16), 1.0f, transparency, layerDepth, drawStackNumber, c, drawShadow);
            //base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, c, drawShadow);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
                pair.Value.drawWhenHeld(spriteBatch, objectPosition + (pair.Key * Game1.tileSize), f);
            //base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        
        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                if (!this.isPlaceable())
                    return;
                int x = Game1.getOldMouseX() + Game1.viewport.X+ (int)((pair.Value as MultiTiledComponent).offsetKey.X*Game1.tileSize);
                int y = Game1.getOldMouseY() + Game1.viewport.Y+ (int)((pair.Value as MultiTiledComponent).offsetKey.Y * Game1.tileSize);
                if ((double)Game1.mouseCursorTransparency == 0.0)
                {
                    x = ((int)Game1.player.GetGrabTile().X+ (int)((pair.Value as MultiTiledComponent).offsetKey.X))  * 64;
                    y = ((int)Game1.player.GetGrabTile().Y + (int)((pair.Value as MultiTiledComponent).offsetKey.Y)) * 64;
                }
                if (Game1.player.GetGrabTile().Equals(Game1.player.getTileLocation()) && (double)Game1.mouseCursorTransparency == 0.0)
                {
                    Vector2 translatedVector2 = Utility.getTranslatedVector2(Game1.player.GetGrabTile(), Game1.player.FacingDirection, 1f);
                    translatedVector2 += (pair.Value as MultiTiledComponent).offsetKey;
                    x = (int)translatedVector2.X * 64;
                    y = (int)translatedVector2.Y * 64;
                }
                bool flag = Utility.playerCanPlaceItemHere(location, (Item)pair.Value, x, y, Game1.player);
                spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)(x / 64 * 64 - Game1.viewport.X), (float)(y / 64 * 64 - Game1.viewport.Y)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(flag ? 194 : 210, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
                this.draw(spriteBatch, x / 64, y / 64, 0.5f);
            }
        }
        


        public virtual void pickUp()
        {
            bool canPickUp = this.removeAndAddToPlayersInventory();
            if (canPickUp)
            {
                foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
                    (pair.Value as MultiTiledComponent).removeFromLocation((pair.Value as MultiTiledComponent).location, pair.Key);
                this.location = null;
            }
            else
                Game1.showRedMessage("NOOOOOOOO");
        }

        public override bool removeAndAddToPlayersInventory()
        {
            if (Game1.player.isInventoryFull())
            {
                Game1.showRedMessage("Inventory full.");
                return false;
            }
            Game1.player.addItemToInventory(this);
            return true;
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                pair.Value.placementAction(location, x + (int)pair.Key.X * Game1.tileSize, y + (int)pair.Key.Y * Game1.tileSize, who);
                //ModCore.log(pair.Value.TileLocation);
            }
            this.location = location;
            return true;
            //return base.placementAction(location, x, y, who);
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                if (!pair.Value.canBePlacedHere(l, tile + pair.Key))
                    return false;
            }
            return true;

        }
        public override bool clicked(Farmer who)
        {
            bool cleanUp = this.clicked(who);
            if (cleanUp)
                this.pickUp();
            return cleanUp;
        }

        public override bool rightClicked(Farmer who)
        {
            return base.rightClicked(who);
        }

        public override bool shiftRightClicked(Farmer who)
        {
            return base.shiftRightClicked(who);
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {

            return base.checkForAction(who, justCheckingForActivity);
        }

        public override Item getOne()
        {
            Dictionary<Vector2, MultiTiledComponent> objs = new Dictionary<Vector2, MultiTiledComponent>();
            foreach (var pair in this.objects)
            {
                objs.Add(pair.Key, (MultiTiledComponent)pair.Value);
            }
            return new MultiTiledObject(this.info, this.TileLocation, objs);
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            MultiTiledObject obj = (MultiTiledObject)Revitalize.ModCore.Serializer.DeserializeGUID<MultiTiledObject>(additionalSaveData["GUID"]);
            if (obj == null)
            {
                return null;
            }

            Dictionary<Vector2, Guid> guids = new Dictionary<Vector2, Guid>();

            foreach(KeyValuePair<Vector2,Guid> pair in obj.childrenGuids)
            {
                guids.Add(pair.Key, pair.Value);
            }

            foreach(KeyValuePair<Vector2,Guid> pair  in guids)
            {
                    obj.childrenGuids.Remove(pair.Key);
                    //Revitalize.ModCore.log("DESERIALIZE: " + pair.Value.ToString());
                    MultiTiledComponent component= Revitalize.ModCore.Serializer.DeserializeGUID<MultiTiledComponent>(pair.Value.ToString());
                component.InitNetFields();

                obj.addComponent(pair.Key, component);
                    
                
            }
            obj.InitNetFields();

            if (!Revitalize.ModCore.ObjectGroups.ContainsKey(additionalSaveData["GUID"]))
            {
                Revitalize.ModCore.ObjectGroups.Add(additionalSaveData["GUID"], obj);
                return obj;
            }
            else
            {
                return Revitalize.ModCore.ObjectGroups[additionalSaveData["GUID"]];
            }

            
        }

        public override Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string,string> saveData= base.getAdditionalSaveData();

            Revitalize.ModCore.log("Serialize: " + this.guid.ToString());

            saveData.Add("GUID", this.guid.ToString());

            Revitalize.ModCore.Serializer.SerializeGUID(this.guid.ToString(), this);
            return saveData;
        }

        public void setAllAnimationsToDefault()
        {
            foreach(KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                string animationKey = (pair.Value as MultiTiledComponent) .generateDefaultRotationalAnimationKey();
                if ((pair.Value as MultiTiledComponent).animationManager.animations.ContainsKey(animationKey))
                {
                    (pair.Value as MultiTiledComponent).animationManager.setAnimation(animationKey);
                }
            }
        }

        public override bool canStackWith(Item other)
        {
            return false;
        }

        public override int maximumStackSize()
        {
            return 1;
        }

    }
}
