using System.IO;

namespace TehPers.Core.Multiplayer.Synchronized.Simple {
    internal class SynchronizedUInt32 : SynchronizedSimple<uint> {
        public SynchronizedUInt32(uint initialValue) : base(initialValue) { }

        /// <inheritdoc />
        public override void WriteFull(BinaryWriter writer) {
            writer.Write(this.Value);
        }

        /// <inheritdoc />
        public override void ReadFull(BinaryReader reader) {
            this.SetValueWithoutDirty(reader.ReadUInt32());
        }
    }
}