using System;
using System.IO;
using System.Linq;

namespace TehPers.Stardew.SCCL.NBT {
    /**<summary>Custom implementation of NBT which is (hopefully) backwards compatible and won't take too much extra space</summary>**/
    public abstract class NBTBase {
#pragma warning disable CRRSP01 // A misspelled word has been found.
        const string FORMAT_NAME = "NBTesque";
#pragma warning restore CRRSP01 // A misspelled word has been found.

        public NBTBase() {

        }

        protected abstract void ReadData(BinaryReader stream);

        protected abstract void WriteData(BinaryWriter stream);

        public static NBTBase CreateTag(byte id) {
            if (id >= TAG_IDS.Length) return null;
            return Activator.CreateInstance(TAG_IDS[id]) as NBTBase;
        }

        public static NBTBase ReadStream(Stream stream) {
            try {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    if (reader.ReadString() != FORMAT_NAME || reader.ReadByte() != 0)
                        throw new Exception("Data file is not readable"); // Whatever exception type this should be, I'll figure it out later
                    return ReadStream(reader);
                }
            } catch (Exception) {
                throw new Exception("Data is corrupt");
            }
        }

        protected static NBTBase ReadStream(BinaryReader stream) {
            NBTBase tag = CreateTag(stream.ReadByte());
            tag.ReadData(stream);
            return tag;
        }

        public static void WriteStream(Stream stream, NBTBase tag) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                // Guess I'll include a header
                writer.Write(FORMAT_NAME); // Type
                writer.Write((byte) 0); // Version number
                WriteStream(writer, tag);
            }
        }

        protected static void WriteStream(BinaryWriter stream, NBTBase tag) {
            int id = Array.IndexOf(TAG_IDS, tag.GetType());
            if (id == -1)
                throw new ArgumentException("Unknown type " + tag.GetType().FullName, "tag");
            stream.Write((byte) id);
            tag.WriteData(stream);
        }

        private static Type[] TAG_IDS = new Type[] {
            typeof(NBTEnd),
            typeof(NBTByte),
            typeof(NBTShort),
            typeof(NBTInt),
            typeof(NBTLong),
            typeof(NBTFloat),
            typeof(NBTDouble),
            typeof(NBTByteArray),
            typeof(NBTString),
            typeof(NBTTagList),
            typeof(NBTTagCompound),
            typeof(NBTIntArray),
            typeof(NBTFalse),
            typeof(NBTTrue)
        };
    }
}
