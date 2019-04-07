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
    public class Bench:ChairMultiTiledObject
    {
        public Bench() : base()
        {

        }

        public List<long> playersSittingHere = new List<long>();

        public Bench(BasicItemInformation info, Vector2 TilePosition) : base(info, TilePosition)
        {

        }


        public Bench(BasicItemInformation info,Vector2 TilePosition, Dictionary<Vector2,MultiTiledComponent> Objects) : base(info, TilePosition, Objects)
        {

        }

        /// <summary>
        /// Rotate all chair components associated with this chair object.
        /// </summary>
        public override void rotate()
        {

            if (Revitalize.ModCore.playerInfo.sittingInfo.SittingObject == this) return;
            if (this.playersSittingHere.Count > 0)
            {
                Game1.showRedMessage("Can't rotate furniture when people are siting on it.");
                return;
            }

            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.objects)
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

            return new Bench(this.info, this.TileLocation, objs);
        }


        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Bench obj = (Bench)Revitalize.ModCore.Serializer.DeserializeGUID<Bench>(additionalSaveData["GUID"]);
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


    }
}
