using StardustCore.Interfaces;
using StardustCore.Objects.Tools.SerializationInformation;
using StardustCore.UIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.Objects.Tools.SerializationInformation
{
    public class Serialization_ExtendedHoe : SerializedObjectBase
    {
        public string Name;
        public string Description;
        public int UpgradeLevel;
        public Texture2DExtended TextureInformation;

        public Serialization_ExtendedHoe() : base()
        {
            this.SerializationName = GetSerializationName();
        }

        public Serialization_ExtendedHoe(ExtendedHoe axe) : base()
        {
            this.UpgradeLevel = axe.UpgradeLevel;
            this.Name = axe.Name;
            this.Description = axe.description;
            this.TextureInformation = axe.texture;
            this.SerializationName = GetSerializationName();
        }

        public override Type getCustomType()
        {
            return typeof(ExtendedHoe);
        }

        public override string GetSerializationName()
        {
            return typeof(ExtendedHoe).ToString();
        }
    }
}
