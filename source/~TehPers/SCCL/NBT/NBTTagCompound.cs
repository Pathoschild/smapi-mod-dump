using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.NBT {
    public class NBTTagCompound : NBTBase {
        public Dictionary<string, NBTBase> Value { get; set; } = new Dictionary<string, NBTBase>();
        public NBTBase this[string key] {
            get {
                return Value[key];
            }
            set {
                Value[key] = value;
            }
        }

        #region Setters
        public void Set(string name, byte value) {
            this[name] = new NBTByte() { Value = value };
        }

        public void Set(string name, short value) {
            this[name] = new NBTShort() { Value = value };
        }

        public void Set(string name, int value) {
            this[name] = new NBTInt() { Value = value };
        }

        public void Set(string name, long value) {
            this[name] = new NBTLong() { Value = value };
        }

        public void Set(string name, float value) {
            this[name] = new NBTFloat() { Value = value };
        }

        public void Set(string name, double value) {
            this[name] = new NBTDouble() { Value = value };
        }

        public void Set(string name, byte[] value) {
            this[name] = new NBTByteArray() { Value = value };
        }

        public void Set(string name, string value) {
            this[name] = new NBTString() { Value = value };
        }

        public void Set(string name, NBTBase[] value) {
            this[name] = new NBTTagList() { Value = value };
        }

        public void Set(string name, NBTTagCompound value) {
            this[name] = value;
        }

        public void Set(string name, int[] value) {
            this[name] = new NBTIntArray() { Value = value };
        }
        #endregion

        #region Getters
        public byte? GetByte(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTByte)
                return ((NBTByte) Value[name]).Value;
            return null;
        }

        public short? GetShort(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTShort)
                return ((NBTShort) Value[name]).Value;
            return null;
        }

        public int? GetInt(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTInt)
                return ((NBTInt) Value[name]).Value;
            return null;
        }

        public long? GetLong(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTLong)
                return ((NBTLong) Value[name]).Value;
            return null;
        }

        public float? GetFloat(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTFloat)
                return ((NBTFloat) Value[name]).Value;
            return null;
        }

        public double? GetDouble(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTDouble)
                return ((NBTDouble) Value[name]).Value;
            return null;
        }

        public byte[] GetByteArray(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTByteArray)
                return ((NBTByteArray) Value[name]).Value;
            return null;
        }

        public string GetString(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTString)
                return ((NBTString) Value[name]).Value;
            return null;
        }

        public NBTBase[] GetTagList(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTTagList)
                return ((NBTTagList) Value[name]).Value;
            return null;
        }

        public NBTTagCompound GetTagCompound(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTTagCompound)
                return (NBTTagCompound) Value[name];
            return null;
        }

        public int[] GetIntArray(string name) {
            if (Value.ContainsKey(name) && Value[name] is NBTIntArray)
                return ((NBTIntArray) Value[name]).Value;
            return null;
        }

        public bool? GetBool(string name) {
            if (Value.ContainsKey(name)) {
                if (Value[name] is NBTTrue)
                    return true;
                else if (Value[name] is NBTFalse)
                    return false;
            }
            return null;
        }
        #endregion

        protected override void ReadData(BinaryReader stream) {
            NBTBase tag = null;
            while (!(tag is NBTEnd)) {
                string name = stream.ReadString();
                tag = ReadStream(stream);
            }
        }

        protected override void WriteData(BinaryWriter stream) {
            foreach (KeyValuePair<string, NBTBase> tag in this.Value) {
                stream.Write(tag.Key);
                WriteStream(stream, tag.Value);
            }
            stream.Write("");
            WriteStream(stream, new NBTEnd());
        }
    }
}
