using Netcode;
using StardustCore.NetCode.Graphics;
using StardustCore.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.NetCode.Objects
{
    public class NetMultiTileComponent : Netcode.NetField<MultiTileComponent, NetMultiTileComponent>
    {
        private NetTexture2DExtended texture;
        private NetInt which;
        private NetVector2 tilePos;
        private NetRectangle sourceRect;
        private NetRectangle boundingBox;
        private NetVector2 drawPosition;
        private NetAnimationManager animationManager;

        public NetMultiTileComponent()
        {

        }

        public NetMultiTileComponent(MultiTileComponent obj): base(obj)
        {
            Set(obj);
        }

        public NetInt InventoryMaxSize { get; private set; }

        public override void Set(MultiTileComponent newValue)
        {
            this.value = newValue;
        }

        protected override void ReadDelta(BinaryReader reader, NetVersion version)
        {

            texture = new NetTexture2DExtended();
            texture.Read(reader, version);

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

            animationManager = new NetAnimationManager();
            animationManager.Read(reader, version);
            Value.animationManager = animationManager.Value;
            //NetCoreObject obj = new NetCoreObject();
            //obj.ReadData(reader, version);

            /*
            NetMultiTileObject hmm = new NetMultiTileObject();
            hmm.Read(reader,version);
            Value.containerObject = hmm.Value;
            */
        }

        protected override void WriteDelta(BinaryWriter writer)
        {
            //NetCoreObject obj = new NetCoreObject(Value);
            //obj.WriteData(writer);

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

            animationManager = new NetAnimationManager(Value.animationManager);
            animationManager.Write(writer);

            //NetMultiTileObject hmm = new NetMultiTileObject(Value.containerObject);
            //hmm.Write(writer);
        }
    }
}
