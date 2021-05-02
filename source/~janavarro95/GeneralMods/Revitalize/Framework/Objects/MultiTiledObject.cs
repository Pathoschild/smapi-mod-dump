/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using PyTK.CustomElementHandler;
using Revitalize.Framework.Illuminate;
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Revitalize.Framework.Objects
{
    public class MultiTiledObject : CustomObject
    {
        [JsonIgnore]
        public override string ItemInfo
        {
            get
            {
                string infoStr = Revitalize.ModCore.Serializer.ToJSONString(this.info);
                string guidStr = this.guid.ToString();
                string pytkData = ModCore.Serializer.ToJSONString(this.data);
                string dictStr = ModCore.Serializer.ToJSONString(this.childrenGuids);
                if (this.objects != null)
                {
                    foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
                    {
                        if (pair.Value == null) continue;
                        if ((pair.Value as CustomObject) == null) continue;
                        if ((pair.Value as CustomObject).guid == null) continue;
                        if (ModCore.CustomObjects.ContainsKey((pair.Value as CustomObject).guid))
                        {
                            ModCore.CustomObjects[(pair.Value as CustomObject).guid] = (CustomObject)pair.Value;
                        }
                        else
                        {
                            ModCore.CustomObjects.Add((pair.Value as CustomObject).guid, (pair.Value as CustomObject));
                        }
                    }
                }

                //ModCore.log("Serializing dict:" + dictStr);
                return infoStr + "<" + guidStr + "<" + pytkData + "<" + dictStr;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                string[] data = value.Split('<');
                string infoString = data[0];
                string guidString = data[1];
                string pytkData = data[2];
                string dictStr = data[3];
                //ModCore.log("Getting dictStr:" + dictStr);

                this.info = (BasicItemInformation)Revitalize.ModCore.Serializer.DeserializeFromJSONString(infoString, typeof(BasicItemInformation));
                this.data = Revitalize.ModCore.Serializer.DeserializeFromJSONString<CustomObjectData>(pytkData);
                Guid oldGuid = this.guid;
                this.guid = Guid.Parse(guidString);

                //this.childrenGuids = ModCore.Serializer.DeserializeFromJSONString<Dictionary<Vector2, Guid>>(dictStr);
                if (ModCore.CustomObjects.ContainsKey(this.guid))
                {
                    ModCore.CustomObjects[this.guid] = this;
                }
                else
                {
                    ModCore.CustomObjects.Add(this.guid, this);
                }

                if (ModCore.CustomObjects.ContainsKey(oldGuid) && ModCore.CustomObjects.ContainsKey(this.guid))
                {
                    if (ModCore.CustomObjects[oldGuid] == ModCore.CustomObjects[this.guid] && oldGuid != this.guid)
                    {
                        //ModCore.CustomObjects.Remove(oldGuid);
                    }
                }

                Dictionary<Vector2, Guid> dict = ModCore.Serializer.DeserializeFromJSONString<Dictionary<Vector2, Guid>>(dictStr);
                if (dict == null) return;
                if (dict.Count == 0) return;
                else
                {
                    //ModCore.log("Children dicts are updated!");
                    this.childrenGuids = dict;
                    foreach (KeyValuePair<Vector2, Guid> pair in dict)
                    {
                        if (ModCore.CustomObjects.ContainsKey(pair.Value))
                        {
                            this.removeComponent(pair.Key);
                            this.addComponent(pair.Key, (MultiTiledComponent)ModCore.CustomObjects[pair.Value]);
                        }
                        else
                        {
                            MultiplayerUtilities.RequestGuidObject(pair.Value);
                        }
                    }
                }
            }
        }

        [JsonIgnore]
        public Dictionary<Vector2, StardewValley.Object> objects;

        public Dictionary<Vector2, Guid> childrenGuids;

        private int width;
        private int height;
        public int Width
        {
            get
            {
                return this.width + 1;
            }
        }
        public int Height
        {
            get
            {
                return this.height + 1;
            }
        }

        public MultiTiledObject()
        {
            this.objects = new Dictionary<Vector2, StardewValley.Object>();
            this.childrenGuids = new Dictionary<Vector2, Guid>();

        }

        public MultiTiledObject(CustomObjectData PyTKData, BasicItemInformation info)
            : base(PyTKData, info)
        {
            this.objects = new Dictionary<Vector2, StardewValley.Object>();
            this.childrenGuids = new Dictionary<Vector2, Guid>();

        }

        public MultiTiledObject(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation)
            : base(PyTKData, info, TileLocation)
        {
            this.objects = new Dictionary<Vector2, StardewValley.Object>();
            this.childrenGuids = new Dictionary<Vector2, Guid>();

        }

        public MultiTiledObject(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation, Dictionary<Vector2, MultiTiledComponent> ObjectsList)
            : base(PyTKData, info, TileLocation)
        {
            this.objects = new Dictionary<Vector2, StardewValley.Object>();
            this.childrenGuids = new Dictionary<Vector2, Guid>();
            foreach (var v in ObjectsList)
            {
                MultiTiledComponent component = (MultiTiledComponent)v.Value.getOne();
                this.addComponent(v.Key, component);
            }


        }

        public bool addComponent(Vector2 key, MultiTiledComponent obj)
        {
            if (this.objects.ContainsKey(key))
            {
                ModCore.log("Bad DATA");
                return false;
            }

            this.objects.Add(key, obj);
            if (this.childrenGuids.ContainsKey(key) == false)
            {
                this.childrenGuids.Add(key, obj.guid);
            }


            if (key.X > this.width) this.width = (int)key.X;
            if (key.Y > this.height) this.height = (int)key.Y;
            (obj as MultiTiledComponent).containerObject = this;
            (obj as MultiTiledComponent).offsetKey = key;
            this.info.forceUpdate();
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
            this.updateInfo();
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                (pair.Value as MultiTiledComponent).draw(spriteBatch, x + ((int)pair.Key.X), y + ((int)pair.Key.Y), alpha);
            }
        }

        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
        {
            this.updateInfo();
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                pair.Value.draw(spriteBatch, xNonTile + (int)pair.Key.X * Game1.tileSize, yNonTile + (int)pair.Key.Y * Game1.tileSize, layerDepth, alpha);
            }

            //base.draw(spriteBatch, xNonTile, yNonTile, layerDepth, alpha);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            this.updateInfo();
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                //ModCore.log(location + (pair.Key * 16) + new Vector2(32, 32));
                pair.Value.drawInMenu(spriteBatch, location + (pair.Key * 16) + new Vector2(32, 32), 1.0f, transparency, layerDepth, drawStackNumber, color, drawShadow);
            }
            //base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, c, drawShadow);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            this.updateInfo();
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
                pair.Value.drawWhenHeld(spriteBatch, objectPosition + (pair.Key * Game1.tileSize), f);
            //base.drawWhenHeld(spriteBatch, objectPosition, f);
        }


        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {
            this.updateInfo();
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                if (!this.isPlaceable())
                    return;
                int x = Game1.getOldMouseX() + Game1.viewport.X + (int)((pair.Value as MultiTiledComponent).offsetKey.X * Game1.tileSize);
                int y = Game1.getOldMouseY() + Game1.viewport.Y + (int)((pair.Value as MultiTiledComponent).offsetKey.Y * Game1.tileSize);
                if ((double)Game1.mouseCursorTransparency == 0.0)
                {
                    x = ((int)Game1.player.GetGrabTile().X + (int)((pair.Value as MultiTiledComponent).offsetKey.X)) * 64;
                    y = ((int)Game1.player.GetGrabTile().Y + (int)((pair.Value as MultiTiledComponent).offsetKey.Y)) * 64;
                }
                if (Game1.player.GetGrabTile().Equals(Game1.player.getTileLocation()) && (double)Game1.mouseCursorTransparency == 0.0)
                {
                    Vector2 translatedVector2 = Utility.getTranslatedVector2(Game1.player.GetGrabTile(), Game1.player.FacingDirection, 1f);
                    translatedVector2 += (pair.Value as MultiTiledComponent).offsetKey;
                    x = (int)translatedVector2.X * 64;
                    y = (int)translatedVector2.Y * 64;
                }
                bool flag = (pair.Value as MultiTiledComponent).canBePlacedHere(location, new Vector2(x / Game1.tileSize, y / Game1.tileSize));
                spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)(x / 64 * 64 - Game1.viewport.X), (float)(y / 64 * 64 - Game1.viewport.Y)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(flag ? 194 : 210, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);

                //Revitalize.ModCore.log(new Vector2(x + ((int)pair.Key.X), y + ((int)pair.Key.Y)));
                if ((pair.Value as MultiTiledComponent).info.ignoreBoundingBox)
                {
                    //x *= -1;
                    //y *= -1;
                }
                (pair.Value as MultiTiledComponent).draw(spriteBatch, x / Game1.tileSize, y / Game1.tileSize, 0.5f);
                //break;
                //this.draw(spriteBatch, x / 64, y / 64, 0.5f);
            }
        }

        public override void drawFullyInMenu(SpriteBatch spriteBatch, Vector2 objectPosition, float Depth)
        {
            this.updateInfo();

            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
                (pair.Value as CustomObject).drawFullyInMenu(spriteBatch, objectPosition + (pair.Key * Game1.tileSize), Depth);
        }

        public virtual void pickUp(Farmer who)
        {
            bool canPickUp = this.removeAndAddToPlayersInventory();
            if (canPickUp)
            {
                foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
                {
                    if ((pair.Value as MultiTiledComponent).info.lightManager != null)
                    {
                        //ModCore.log("Let there be light.");
                        if ((pair.Value as MultiTiledComponent).info.lightManager.lightsOn == true)
                        {
                            //ModCore.log("Got a light???");
                        }
                    }
                    (pair.Value as MultiTiledComponent).removeFromLocation(who.currentLocation, pair.Key);

                }
                this.location = null;
            }
            else
                Game1.showRedMessage("Can't pickup item for some reason.");
        }

        public override bool removeAndAddToPlayersInventory()
        {
            bool f = Game1.player.addItemToInventoryBool(this, false);
            return f;
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
            this.updateInfo();
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                /*
                if ((pair.Value as CustomObject).info.ignoreBoundingBox)
                {
                    pair.Value.placementAction(location, -1 * (x + (int)pair.Key.X * Game1.tileSize), -1 * (y + (int)pair.Key.Y * Game1.tileSize), who);
                }
                else
                {
                    pair.Value.placementAction(location, x + (int)pair.Key.X * Game1.tileSize, y + (int)pair.Key.Y * Game1.tileSize, who);
                }*/
                (pair.Value as MultiTiledComponent).placementAction(location, x + (int)pair.Key.X * Game1.tileSize, y + (int)pair.Key.Y * Game1.tileSize, who);
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
                this.pickUp(who);
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
                objs.Add(pair.Key, (MultiTiledComponent)pair.Value.getOne());
            }
            return new MultiTiledObject(this.data, this.info.Copy(), this.TileLocation, objs);
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            MultiTiledObject obj = (MultiTiledObject)Revitalize.ModCore.Serializer.DeserializeGUID<MultiTiledObject>(additionalSaveData["GUID"]);
            if (obj == null)
            {
                return null;
            }

            Dictionary<Vector2, Guid> guids = new Dictionary<Vector2, Guid>();

            foreach (KeyValuePair<Vector2, Guid> pair in obj.childrenGuids)
            {
                guids.Add(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<Vector2, Guid> pair in guids)
            {
                obj.childrenGuids.Remove(pair.Key);
                MultiTiledComponent component = Revitalize.ModCore.Serializer.DeserializeGUID<MultiTiledComponent>(pair.Value.ToString());
                component.InitNetFields();
                obj.removeComponent(pair.Key);
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

        /// <summary>
        /// Recreate the data from data already stored on the object.
        /// </summary>
        public virtual void recreate()
        {
            Dictionary<Vector2, Guid> guids = new Dictionary<Vector2, Guid>();

            foreach (KeyValuePair<Vector2, Guid> pair in this.childrenGuids)
            {
                guids.Add(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<Vector2, Guid> pair in guids)
            {
                this.childrenGuids.Remove(pair.Key);
                MultiTiledComponent component = Revitalize.ModCore.Serializer.DeserializeGUID<MultiTiledComponent>(pair.Value.ToString());
                component.InitNetFields();
                this.removeComponent(pair.Key);
                this.addComponent(pair.Key, component);


            }
            this.InitNetFields();

            if (!Revitalize.ModCore.ObjectGroups.ContainsKey(this.guid.ToString()))
            {
                Revitalize.ModCore.ObjectGroups.Add(this.guid.ToString(), this);
            }
        }

        public override Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> saveData = base.getAdditionalSaveData();
            saveData.Add("GUID", this.guid.ToString());
            Revitalize.ModCore.Serializer.SerializeGUID(this.guid.ToString(), this);
            return saveData;
        }

        public void setAllAnimationsToDefault()
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                string animationKey = (pair.Value as MultiTiledComponent).generateDefaultRotationalAnimationKey();
                if ((pair.Value as MultiTiledComponent).animationManager.animations.ContainsKey(animationKey))
                {
                    (pair.Value as MultiTiledComponent).animationManager.setAnimation(animationKey);
                }
            }
        }

        public override bool canStackWith(ISalable other)
        {
            return false;
        }

        public override int maximumStackSize()
        {
            return 1;
        }

        public override void updateInfo()
        {
            //this.recreateComponents();
            if (this.info == null)
            {
                this.ItemInfo = this.text;
                //ModCore.log("Updated item info for container!");
                return;
            }
            if (this.objects != null)
            {
                if (this.childrenGuids != null)
                {
                    if (this.objects.Count == 0 || this.objects.Count != this.childrenGuids.Count)
                    {
                        this.ItemInfo = this.text;
                        //ModCore.log("Updated item info for container!");
                        return;
                    }
                }
                else
                {
                    if (this.objects.Count == 0)
                    {
                        this.ItemInfo = this.text;
                        //ModCore.log("Updated item info for container!");
                        return;
                    }

                }
            }

            if (this.requiresUpdate())
            {
                //this.ItemInfo = this.text;
                this.text = this.ItemInfo;
                this.info.cleanAfterUpdate();
                MultiplayerUtilities.RequestUpdateSync(this.guid);
            }
        }

        public virtual void recreateComponents()
        {

            if (ModCore.CustomObjects.ContainsKey(this.guid))
            {
                if ((ModCore.CustomObjects[this.guid] as MultiTiledObject).objects.Count > 0)
                {
                    this.objects = (ModCore.CustomObjects[this.guid] as MultiTiledObject).objects;
                }
                else
                {

                }
                if ((ModCore.CustomObjects[this.guid] as MultiTiledObject).childrenGuids.Count > 0)
                {
                    this.childrenGuids = (ModCore.CustomObjects[this.guid] as MultiTiledObject).childrenGuids;
                }
            }
            else
            {
                MultiplayerUtilities.RequestGuidObject(this.guid);
                MultiplayerUtilities.RequestGuidObject_Tile(this.guid);
            }
            if (this.objects == null || this.childrenGuids == null)
            {
                //ModCore.log("Either objects or children guids are null");
                return;
            }


            //ModCore.log("Recreate children components");
            if (this.objects.Count < this.childrenGuids.Count)
            {
                foreach (KeyValuePair<Vector2, Guid> pair in this.childrenGuids)
                {
                    if (ModCore.CustomObjects.ContainsKey(pair.Value))
                    {
                        this.removeComponent(pair.Key);
                        this.addComponent(pair.Key, (MultiTiledComponent)ModCore.CustomObjects[pair.Value]);
                    }
                    else
                    {
                        MultiplayerUtilities.RequestGuidObject(pair.Value);
                        MultiplayerUtilities.RequestGuidObject_Tile(pair.Value);
                    }
                }
            }
            else
            {
                //ModCore.log("Count is exactly the same!");
                //ModCore.log("Count is: " + this.objects.Count+" : and " + this.childrenGuids.Count);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="DyeColor"></param>
        public override void dyeColor(NamedColor DyeColor)
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                (pair.Value as CustomObject).dyeColor(DyeColor);
            }
            this.info.DyedColor = DyeColor;
        }

        public override void eraseDye()
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                (pair.Value as CustomObject).eraseDye();
            }
            this.info.DyedColor = new NamedColor("", new Color(0, 0, 0, 0));
        }
    }
}
