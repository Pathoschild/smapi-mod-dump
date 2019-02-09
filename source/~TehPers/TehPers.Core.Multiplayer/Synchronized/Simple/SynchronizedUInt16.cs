using System.IO;

namespace TehPers.Core.Multiplayer.Synchronized.Simple {
    internal class SynchronizedUInt16 : SynchronizedSimple<ushort> {
        public SynchronizedUInt16(ushort initialValue) : base(initialValue) { }

        /// <inheritdoc />
        public override void WriteFull(BinaryWriter writer) {
            writer.Write(this.Value);
        }

        /// <inheritdoc />
        public override void ReadFull(BinaryReader reader) {
            this.SetValueWithoutDirty(reader.ReadUInt16());
        }
    }
}