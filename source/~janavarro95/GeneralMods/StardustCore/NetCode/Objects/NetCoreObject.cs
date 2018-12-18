using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using StardewValley;
using StardustCore.NetCode.Graphics;
using StardustCore.UIUtilities;

namespace StardustCore.NetCode
{
    /*
     * Need to send default source rect and width
     * 
     */


    public class NetCoreObject : Netcode.NetField<CoreObject, NetCoreObject>
    {


        public NetInt which;
        public NetVector2 tilePos;


        public NetRectangle boundingBox;



        public NetVector2 position;
        public NetInt Decoration_type;
        public NetInt rotations;
        public NetInt currentRotation;
        public NetInt sourceIndexOffset;
        public NetVector2 drawPosition;
        public NetRectangle sourceRect;
        public NetRectangle defaultSourceRect;
        public NetRectangle defaultBoundingBox;
        public NetString description;
        public NetTexture2DExtended texture;
        public NetBool flipped;
        public NetBool flaggedForPickup;
        public NetBool lightGlowAdded;
        public NetObjectList<Item> inventory;
        public NetInt InventoryMaxSize;
        public NetBool itemReadyForHarvest;
        public NetBool lightsOn;
        public NetString locationName;
        public NetColor lightColor;
        public NetBool removable;
        public NetColor drawColor;
        public NetBool useXML;
        public NetString serializationName;


        //Animation Manager.....
        public NetAnimationManager animationManager;




        public NetCoreObject()
        {

        }

        public NetCoreObject(CoreObject value) : base(value)
        {
            Set(value);
        }

        public override void Set(CoreObject newValue)
        {
            this.value = newValue;
        }

        protected override void ReadDelta(BinaryReader reader, NetVersion version)
        {
            texture = new NetTexture2DExtended();
            texture.Read(reader, version);
            
            Value.setExtendedTexture(texture.Value);

            which = new NetInt();
            which.Read(reader, version);
            Value.ParentSheetIndex = which.Value;

            tilePos = new NetVector2();
            tilePos.Read(reader, version);
            Value.TileLocation = tilePos.Value;

            InventoryMaxSize = new NetInt();
            InventoryMaxSize.Read(reader, version);
            Value.inventoryMaxSize = InventoryMaxSize.Value;

            sourceRect = new NetRectangle();
            sourceRect.Read(reader, version);
            Value.sourceRect = sourceRect.Value;

            boundingBox = new NetRectangle();
            boundingBox.Read(reader, version);
            Value.boundingBox.Value = boundingBox.Value;

            drawPosition = new NetVector2();
            drawPosition.Read(reader, version);
            Value.drawPosition = drawPosition.Value;

            locationName = new NetString();
            locationName.Read(reader, version);
            Value.locationsName = locationName.Value;
            Value.thisLocation = Game1.getLocationFromName(locationName.Value);

            /*
            animationManager = new NetAnimationManager();
            animationManager.Read(reader, version);
            Value.animationManager = animationManager.Value;
            */
            
        }

        protected override void WriteDelta(BinaryWriter writer)
        {
            texture = new NetTexture2DExtended(Value.getExtendedTexture());
            texture.Write(writer);

            which = new NetInt(Value.ParentSheetIndex);
            which.Write(writer);

            tilePos = new NetVector2(Value.TileLocation);
            tilePos.Write(writer);

            InventoryMaxSize = new NetInt(Value.inventoryMaxSize);
            InventoryMaxSize.Write(writer);

            sourceRect = new NetRectangle(Value.sourceRect);
            sourceRect.Write(writer);

            boundingBox = new NetRectangle(Value.boundingBox.Value);
            sourceRect.Write(writer);

            drawPosition = new NetVector2(Value.drawPosition);
            drawPosition.Write(writer);

            locationName = new NetString(Value.locationsName);
            locationName.Write(writer);

            /*
            if (Value.animationManager == null)
            {
                throw new Exception("WTF, why is the animation manager null???????????");
            }
            animationManager = new NetAnimationManager(Value.animationManager);
            animationManager.Write(writer);
           */
        }
    }
}