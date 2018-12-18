using StardustCore.Interfaces;
using StardustCore.UIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.Objects.Tools.SerializationInformation
{
    public class Serialization_ExtendedPickaxe : SerializedObjectBase
    {
        public string Name;
        public string Description;
        public int UpgradeLevel;
        public Texture2DExtended TextureInformation;

        public Serialization_ExtendedPickaxe() : base()
        {
            this.SerializationName = GetSerializationName();
        }

        public Serialization_ExtendedPickaxe(ExtendedPickaxe axe) : base()
        {
            this.UpgradeLevel = axe.UpgradeLevel;
            this.Name = axe.Name;
            this.Description = axe.description;
            this.TextureInformation = axe.texture;
            this.SerializationName = GetSerializationName();
        }

        public override Type getCustomType()
        {
            return typeof(ExtendedPickaxe);
        }

        public override string GetSerializationName()
        {
            return typeof(ExtendedPickaxe).ToString();
        }
    }
}
