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
using PyTK.CustomElementHandler;
using StardewValley;

namespace Revitalize.Framework.Objects.Machines
{
    public class WireMultiTiledObject : MultiTiledObject
    {


        public WireMultiTiledObject() : base()
        {

        }

        public WireMultiTiledObject(CustomObjectData PyTKData, BasicItemInformation info)
            : base(PyTKData, info)
        {

        }

        public WireMultiTiledObject(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation)
            : base(PyTKData, info, TileLocation)
        {

        }

        public WireMultiTiledObject(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation, Dictionary<Vector2, MultiTiledComponent> ObjectsList)
            : base(PyTKData, info, TileLocation, ObjectsList)
        {


        }

        public override Item getOne()
        {
            Dictionary<Vector2, MultiTiledComponent> objs = new Dictionary<Vector2, MultiTiledComponent>();
            foreach (var pair in this.objects)
            {
                objs.Add(pair.Key, (MultiTiledComponent)pair.Value.getOne());
            }
            return new WireMultiTiledObject(this.data, this.info.Copy(), this.TileLocation, objs);
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            WireMultiTiledObject obj = (WireMultiTiledObject)Revitalize.ModCore.Serializer.DeserializeGUID<WireMultiTiledObject>(additionalSaveData["GUID"]);
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

        public override Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> saveData = base.getAdditionalSaveData();
            //saveData.Add("GUID", this.guid.ToString());
            //Revitalize.ModCore.Serializer.SerializeGUID(this.guid.ToString(), this);
            return saveData;
        }

        public override void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            base.rebuild(additionalSaveData, replacement);
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
            if (drawStackNumber.ShouldDrawFor(this) && this.maximumStackSize() > 1 && ((double)scaleSize > 0.3 && this.Stack != int.MaxValue))
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2((float)(Game1.tileSize - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize)) + 3f * scaleSize, (float)((double)Game1.tileSize - 18.0 * (double)scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
            //base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, c, drawShadow);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            this.updateInfo();
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
                pair.Value.drawWhenHeld(spriteBatch, objectPosition + (pair.Key * Game1.tileSize), f);
            //base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public override bool canStackWith(ISalable other)
        {
            if (other is WireMultiTiledObject)
            {
                return (other as WireMultiTiledObject).info.id == this.info.id && (other as WireMultiTiledObject).info.DyedColor == this.info.DyedColor;
            }
            else return false;
        }

        public override int maximumStackSize()
        {
            return 999;
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            this.updateInfo();
            WireMultiTiledObject m = (WireMultiTiledObject)this.getOne();

            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in m.objects)
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
            m.location = location;
            return true;
            //return base.placementAction(location, x, y, who);
        }
    }
}
