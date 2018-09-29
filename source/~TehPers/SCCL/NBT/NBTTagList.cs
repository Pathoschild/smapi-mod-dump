using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.NBT {
    public class NBTTagList : NBTBase {
        public NBTBase[] Value { get; set; }

        protected override void ReadData(BinaryReader stream) {
            int length = stream.ReadInt32();
            Value = new NBTBase[length];
            for (int i = 0; i < length; i++)
                Value[i] = ReadStream(stream);
        }

        protected override void WriteData(BinaryWriter stream) {
            if (Value == null)
                stream.Write((int) 0);
            else {
                stream.Write(Value.Length);
                for (int i = 0; i < Value.Length; i++)
                    WriteStream(stream, Value[i]);
            }
        }
    }
}
