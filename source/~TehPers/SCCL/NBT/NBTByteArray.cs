using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.NBT {
    public class NBTByteArray : NBTBase {
        public byte[] Value { get; set; }

        protected override void ReadData(BinaryReader stream) {
            int length = stream.ReadInt32();
            Value = new byte[length];
            for (int i = 0; i < length; i++)
                Value[i] = stream.ReadByte();
        }

        protected override void WriteData(BinaryWriter stream) {
            stream.Write(Value.Length);
            for (int i = 0; i < Value.Length; i++)
                stream.Write(Value[i]);
        }
    }
}
