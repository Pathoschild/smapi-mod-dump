using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using Revitalize.Framework.Objects.InformationFiles.Furniture;
using StardewValley;
using StardewValley.Objects;

namespace Revitalize.Framework.Objects.Furniture
{
    /// <summary>
    /// Chair "piece" which represents one of the objects in the game that takes up roughly one tile.
    /// </summary>
    public class ChairTileComponent:FurnitureTileComponent
    {
        public ChairInformation furnitureInfo;

        /// <summary>
        /// Checks if the player can sit "on" this component.
        /// </summary>
        public bool CanSitHere
        {
            get
            {
                return (this.furnitureInfo as InformationFiles.Furniture.ChairInformation).canSitHere;
            }
        }

        public ChairTileComponent():base()
        {

        }

        public ChairTileComponent(BasicItemInformation Info,ChairInformation FurnitureInfo) : base(Info)
        {
            this.furnitureInfo = FurnitureInfo;
        }

        public ChairTileComponent(BasicItemInformation Info,Vector2 TileLocation, ChairInformation FurnitureInfo) : base(Info, TileLocation)
        {
            this.furnitureInfo = FurnitureInfo;
        }

        

        /// <summary>
        /// When the chair is right clicked ensure that all pieces associated with it are also rotated.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool rightClicked(Farmer who)
        {
            this.containerObject.rotate(); //Ensure that all of the chair pieces rotate at the same time.

            checkForSpecialUpSittingAnimation();
            return true;
            //return base.rightClicked(who);
        }

        /// <summary>
        /// Used for more object interactions.
        /// When the chair is shift right clicked sit on that specific chair tile if you can sit there.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool shiftRightClicked(Farmer who)
        {
            if (this.CanSitHere)
            {
                Revitalize.ModCore.playerInfo.sittingInfo.sit(this.containerObject, this.TileLocation*Game1.tileSize);
                if(this.containerObject is Bench)
                {
                    (this.containerObject as Bench).playersSittingHere.Add(Game1.player.uniqueMultiplayerID);
                }
                foreach(KeyValuePair<Vector2, StardewValley.Object> pair in this.containerObject.objects)
                {
                    (pair.Value as ChairTileComponent).checkForSpecialUpSittingAnimation();
                }
                
            }
            return base.shiftRightClicked(who);
        }


        public override Item getOne()
        {
            ChairTileComponent component = new ChairTileComponent(this.info, (ChairInformation)this.furnitureInfo);
            component.containerObject = this.containerObject;
            component.offsetKey = this.offsetKey;
            return component;
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            //instead of using this.offsetkey.x use get additional save data function and store offset key there

            Vector2 offsetKey = new Vector2(Convert.ToInt32(additionalSaveData["offsetKeyX"]), Convert.ToInt32(additionalSaveData["offsetKeyY"]));
            ChairTileComponent self = Revitalize.ModCore.Serializer.DeserializeGUID<ChairTileComponent>(additionalSaveData["GUID"]);
            if (self == null)
            {
                return null;
            }

            if (!Revitalize.ModCore.ObjectGroups.ContainsKey(additionalSaveData["ParentGUID"]))
            {
                //Get new container
                ChairMultiTiledObject obj = (ChairMultiTiledObject)Revitalize.ModCore.Serializer.DeserializeGUID<ChairMultiTiledObject>(additionalSaveData["ParentGUID"]);
                self.containerObject = obj;
                obj.addComponent(offsetKey, self);
                //Revitalize.ModCore.log("ADD IN AN OBJECT!!!!");
                Revitalize.ModCore.ObjectGroups.Add(additionalSaveData["ParentGUID"], obj);
            }
            else
            {
                self.containerObject = Revitalize.ModCore.ObjectGroups[additionalSaveData["ParentGUID"]];
                Revitalize.ModCore.ObjectGroups[additionalSaveData["GUID"]].addComponent(offsetKey, self);
                //Revitalize.ModCore.log("READD AN OBJECT!!!!");
            }

            return (ICustomObject)self;
        }

        public override Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> saveData = base.getAdditionalSaveData();
            Revitalize.ModCore.Serializer.SerializeGUID(this.containerObject.childrenGuids[this.offsetKey].ToString(), this);

            return saveData;

        }


        /// <summary>
        ///Used to manage graphics for chairs that need to deal with special "layering" for transparent chair backs. Otherwise the player would be hidden.
        /// </summary>
        public void checkForSpecialUpSittingAnimation()
        {
            if (this.info.facingDirection == Enums.Direction.Up && Revitalize.ModCore.playerInfo.sittingInfo.SittingObject == this.containerObject)
            {
                string animationKey = "Sitting_" + (int)Enums.Direction.Up;
                if (this.animationManager.animations.ContainsKey(animationKey))
                {
                    this.animationManager.setAnimation(animationKey);
                }
            }
        }
    }
}
