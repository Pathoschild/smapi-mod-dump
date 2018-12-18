using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardustCore.UIUtilities;

namespace StardustCore.NetCode
{
    public class NetTexture2DExtended : Netcode.NetField<UIUtilities.Texture2DExtended, NetTexture2DExtended>
    {



        public NetTexture2DExtended()
        {

        }

        public NetTexture2DExtended(Texture2DExtended value) : base(value)
        {
            this.Set(value);
        }

        public void ReadData(BinaryReader reader, NetVersion version)
        {
            ReadDelta(reader, version);
        }

        public override void Set(Texture2DExtended newValue)
        {
            this.value = newValue;
        }

        public void WriteData(BinaryWriter writer)
        {
            WriteDelta(writer);
        }

        protected override void ReadDelta(BinaryReader reader, NetVersion version)
        {
            NetString name = new NetString();
            name.Read(reader, version);

            NetString iD = new NetString();
            iD.Read(reader, version);


            StardustCore.ModCore.ModMonitor.Log("Name: "+name);
            StardustCore.ModCore.ModMonitor.Log("Mod ID:"+iD);


            //Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice,width,height);
            if (String.IsNullOrEmpty(iD.Value) || String.IsNullOrEmpty(name.Value))
            {
                ModCore.ModMonitor.Log("FOR SOME REASON THE TEXTURE INFO IS NULL????");
                return;
            }


            Texture2DExtended texture = ModCore.TextureManagers[iD.Value].getTexture(name.Value);
            if (texture == null) throw new Exception("FOR SOME REASON NULL IS BEING READ???");
            this.Value = texture;
            
        }

        protected override void WriteDelta(BinaryWriter writer)
        {
            NetString name = new NetString(Value.Name);
            name.Write(writer);

            NetString iD = new NetString(Value.modID);
            iD.Write(writer);
            
        }


    }
}
