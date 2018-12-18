using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardustCore.Animations;

namespace StardustCore.NetCode.Graphics
{
    public class NetAnimationManager : Netcode.NetField<Animations.AnimationManager, NetAnimationManager>
    {

        public NetAnimationManager()
        {

        }

        public NetAnimationManager(Animations.AnimationManager manager) : base(manager)
        {
            this.Set(manager);
        }

        public NetString currentAnimationName;
        public NetInt currentAnimationListIndex;
        public NetTexture2DExtended objectTexture;
        public NetAnimation defaultDrawFrame;
        public NetBool enabled;
        public NetString animationDataString;

        protected override void ReadDelta(BinaryReader reader, NetVersion version)
        {
            //Checks to see if the current animation is nothing, aka null.
            NetBool isNull = new NetBool();
            isNull.Read(reader, version);
            bool valueIsNull = isNull.Value;
            if (isNull)
            {
                NetTexture2DExtended nullTexture = new NetTexture2DExtended();
                nullTexture.Read(reader, version);

                NetAnimation nullAnimation = new NetAnimation();
                nullAnimation.Read(reader, version);

                Value.setExtendedTexture(nullTexture.Value);
                Value.defaultDrawFrame = nullAnimation.Value;
                return;
            }


            NetString currentAnimationName = new NetString();
            currentAnimationName.Read(reader, version);

            NetInt currentIndex = new NetInt();
            currentIndex.Read(reader, version);

            NetTexture2DExtended text = new NetTexture2DExtended();
            text.Read(reader, version);

            NetAnimation defaultAnimation = new NetAnimation();
            defaultAnimation.Read(reader, version);

            NetBool enabled = new NetBool();
            enabled.Read(reader, version);

            NetString data = new NetString();
            data.Read(reader, version);

            Value.setExtendedTexture(text.Value);
            Value.defaultDrawFrame = defaultAnimation.Value;
            Value.enabled = enabled.Value;
            //Try and prevent unnecessary parsing.
            if (Value.animations == null && !String.IsNullOrEmpty(Value.animationDataString))
            {
                Value.animations = Animations.AnimationManager.parseAnimationsFromXNB(data.Value);
            }
            if (!String.IsNullOrEmpty(data.Value))
            {
                Value.setAnimation(currentAnimationName.Value, currentIndex.Value);
            }
            else
            {
                Value.currentAnimation = defaultDrawFrame.Value;
            }
        }

        protected override void WriteDelta(BinaryWriter writer)
        {


            if (String.IsNullOrEmpty(Value.currentAnimationName))
            {
                NetBool isNull = new NetBool(true);
                writer.Write(isNull);



                NetTexture2DExtended defaultTexture = new NetTexture2DExtended(Value.getExtendedTexture());
                defaultTexture.Write(writer);

                //do read/write null values here!!!
                if (Value == null) throw new Exception("DONT WRITE A NULL VALUE!!!!");
                NetAnimation drawFrame = new NetAnimation(Value.defaultDrawFrame);
                drawFrame.Write(writer);
                return;
                //throw new Exception("Null string value for currentAnimationName!");
            }
            else
            {
                NetBool isNull = new NetBool(false);
                writer.Write(isNull);
            }
            NetString curentAnimationName = new NetString(!String.IsNullOrEmpty(Value.currentAnimationName) ? Value.currentAnimationName : "");
            currentAnimationName.Write(writer);


            NetInt currentAnimationListIndex = new NetInt(Value.currentAnimationListIndex);
            currentAnimationListIndex.Write(writer);

            NetTexture2DExtended texture = new NetTexture2DExtended(Value.getExtendedTexture());
            texture.Write(writer);

            //do read/write null values here!!!
            NetAnimation defaultDrawFrame = new NetAnimation(Value.defaultDrawFrame);
            defaultDrawFrame.Write(writer);

            NetBool enabled = new NetBool(Value.enabled);
            enabled.Write(writer);

            NetString animationData = new NetString(Value.animationDataString);
            animationData.Write(writer);

        }

        public override void Set(AnimationManager newValue)
        {
            this.value = newValue;
        }
    }
}
