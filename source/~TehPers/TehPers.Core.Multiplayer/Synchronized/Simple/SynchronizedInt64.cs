using System.IO;

namespace TehPers.Core.Multiplayer.Synchronized.Simple {
    internal class SynchronizedInt64 : SynchronizedSimple<long> {
        public SynchronizedInt64(long initialValue) : base(initialValue) { }

        /// <inheritdoc />
        public override void WriteFull(BinaryWriter writer) {
            writer.Write(this.Value);
        }

        /// <inheritdoc />
        public override void ReadFull(BinaryReader reader) {
            this.SetValueWithoutDirty(reader.ReadInt64());
        }
    }
}
