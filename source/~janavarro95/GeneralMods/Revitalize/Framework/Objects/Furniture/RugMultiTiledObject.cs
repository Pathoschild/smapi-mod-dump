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

        public RugMultiTiledObject(BasicItemInformation Info) : base(Info)
        {

        }

        public RugMultiTiledObject(BasicItemInformation Info, Vector2 TilePosition) : base(Info, TilePosition)
        {

        }

        public RugMultiTiledObject(BasicItemInformation Info, Vector2 TilePosition, Dictionary<Vector2, MultiTiledComponent> Objects) : base(Info, TilePosition, Objects)
        {


        }

        /// <summary>
        /// Rotate all chair components associated with this chair object.
        /// </summary>
        public override void rotate()
        {
            Revitalize.ModCore.log("Rotate!");
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                (pair.Value as RugTileComponent).rotate();
            }
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                (pair.Value as RugTileComponent).checkForSpecialUpSittingAnimation();
            }

            base.rotate();
        }

        public override Item getOne()
        {
            Dictionary<Vector2, MultiTiledComponent> objs = new Dictionary<Vector2, MultiTiledComponent>();
            foreach (var pair in this.objects)
            {
                objs.Add(pair.Key, (MultiTiledComponent)pair.Value);
            }


            return new RugMultiTiledObject(this.info, this.TileLocation, objs);
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


        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            return base.canBePlacedHere(l, tile);
        }
    }
}
