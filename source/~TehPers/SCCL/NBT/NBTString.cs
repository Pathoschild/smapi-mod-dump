using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.NBT {
    public class NBTString : NBTBase {
        public string Value { get; set; }

        protected override void ReadData(BinaryReader stream) {
            this.Value = stream.ReadString();
        }

        protected override void WriteData(BinaryWriter stream) {
            stream.Write(this.Value);
        }
    }
}
