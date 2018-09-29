using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.NBT {
    public class NBTFalse : NBTBase {
        protected override void ReadData(BinaryReader stream) {
            // Nothing to read
        }

        protected override void WriteData(BinaryWriter stream) {
            // Nothing to write
        }
    }
}
