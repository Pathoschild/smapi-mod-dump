using StardustCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.Objects.Tools.SerializationInformation
{
    public class SerializedObjectBase : IItemSerializeable
    {
        public string SerializationName;

        public SerializedObjectBase()
        {
            this.SerializationName = this.GetSerializationName();
        }

        public virtual Type getCustomType()
        {
            return this.GetType();
        }

        public virtual string GetSerializationName()
        {
            return this.GetType().ToString();
        }
    }
}
