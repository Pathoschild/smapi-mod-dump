using StardustCore.Interfaces;
using StardustCore.UIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.Objects.Tools.SerializationInformation
{
    public class Serialization_ExtendedWateringCan : SerializedObjectBase
    {
        public string Name;
        public string Description;
        public int UpgradeLevel;
        public Texture2DExtended TextureInformation;
        public int MaxCapacity;
        public int WaterLeft;

        public Serialization_ExtendedWateringCan() : base()
        {
            this.SerializationName = GetSerializationName();
        }

        public Serialization_ExtendedWateringCan(ExtendedWateringCan tool) : base()
        {
            this.UpgradeLevel = tool.UpgradeLevel;
            this.Name = tool.Name;
            this.Description = tool.description;
            this.TextureInformation = tool.texture;
            this.SerializationName = GetSerializationName();
            this.MaxCapacity = tool.waterCanMax;
            this.WaterLeft = tool.WaterLeft;
        }

        public override Type getCustomType()
        {
            return typeof(ExtendedWateringCan);
        }

        public override string GetSerializationName()
        {
            return typeof(ExtendedWateringCan).ToString();
        }
    }
}
