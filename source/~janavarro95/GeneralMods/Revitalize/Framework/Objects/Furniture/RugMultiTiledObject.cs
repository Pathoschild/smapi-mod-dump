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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewValley;

namespace Revitalize.Framework.Objects.Furniture
{
    public class RugMultiTiledObject:MultiTiledObject
    {
        public RugMultiTiledObject() : base()
        {

        }

        public RugMultiTiledObject(CustomObjectData PyTKData, BasicItemInformation Info) : base(PyTKData,Info)
        {
            this.Price = Info.price;
        }

        public RugMultiTiledObject(CustomObjectData PyTKData, BasicItemInformation Info, Vector2 TilePosition) : base(PyTKData,Info, TilePosition)
        {
            this.Price = Info.price;
        }

        public RugMultiTiledObject(CustomObjectData PyTKData, BasicItemInformation Info, Vector2 TilePosition, Dictionary<Vector2, MultiTiledComponent> Objects) : base(PyTKData,Info, TilePosition, Objects)
        {
            this.Price = Info.price;

        }

        /// <summary>
        /// Rotate all chair components associated with this chair object.
        /// </summary>
        public override void rotate()
        {
            
        }

        public override Item getOne()
        {
            Dictionary<Vector2, MultiTiledComponent> objs = new Dictionary<Vector2, MultiTiledComponent>();
            foreach (var pair in this.objects)
            {
                objs.Add(pair.Key, (MultiTiledComponent)pair.Value);
            }


            return new RugMultiTiledObject(this.data,this.info.Copy(), this.TileLocation, objs);
        }


        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            RugMultiTiledObject obj = (RugMultiTiledObject)Revitalize.ModCore.Serializer.DeserializeGUID<RugMultiTiledObject>(additionalSaveData["GUID"]);
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
                //Revitalize.ModCore.log("DESERIALIZE: " + pair.Value.ToString());
                RugTileComponent component = Revitalize.ModCore.Serializer.DeserializeGUID<RugTileComponent>(pair.Value.ToString());
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

        public override void recreate()
        {
            Dictionary<Vector2, Guid> guids = new Dictionary<Vector2, Guid>();

            foreach (KeyValuePair<Vector2, Guid> pair in this.childrenGuids)
            {
                guids.Add(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<Vector2, Guid> pair in guids)
            {
                this.childrenGuids.Remove(pair.Key);
                RugTileComponent component = Revitalize.ModCore.Serializer.DeserializeGUID<RugTileComponent>(pair.Value.ToString());
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


        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            return base.canBePlacedHere(l, tile);
        }
    }
}
