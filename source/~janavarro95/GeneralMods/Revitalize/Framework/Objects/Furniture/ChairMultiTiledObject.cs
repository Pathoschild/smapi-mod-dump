using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Objects;

namespace Revitalize.Framework.Objects.Furniture
{
    /// <summary>
    /// Object which encapsulates all of the pieces that make up a chair object in-game.
    /// </summary>
    public class ChairMultiTiledObject:MultiTiledObject
    {

        public ChairMultiTiledObject() : base()
        {

        }

        public ChairMultiTiledObject(BasicItemInformation Info) : base(Info)
        {

        }

        public ChairMultiTiledObject(BasicItemInformation Info, Vector2 TilePosition) : base(Info, TilePosition)
        {

        }

        public ChairMultiTiledObject(BasicItemInformation Info,Vector2 TilePosition,Dictionary<Vector2, MultiTiledComponent> Objects) : base(Info, TilePosition, Objects) {


        }

        /// <summary>
        /// Rotate all chair components associated with this chair object.
        /// </summary>
        public override void rotate()
        {
            Revitalize.ModCore.log("Rotate!");
            foreach(KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                (pair.Value as ChairTileComponent).rotate();
            }
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
            {
                (pair.Value as ChairTileComponent).checkForSpecialUpSittingAnimation();
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

            return new ChairMultiTiledObject(this.info, this.TileLocation, objs);
        }


        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            ChairMultiTiledObject obj = (ChairMultiTiledObject)Revitalize.ModCore.Serializer.DeserializeGUID<ChairMultiTiledObject>(additionalSaveData["GUID"]);
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
                ChairTileComponent component = Revitalize.ModCore.Serializer.DeserializeGUID<ChairTileComponent>(pair.Value.ToString());
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
