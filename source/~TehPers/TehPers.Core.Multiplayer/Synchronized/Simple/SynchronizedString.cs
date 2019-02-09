using System.IO;

namespace TehPers.Core.Multiplayer.Synchronized.Simple {
    internal class SynchronizedString : SynchronizedSimple<string> {
        public SynchronizedString(string initialValue) : base(initialValue) { }

        /// <inheritdoc />
        public override void WriteFull(BinaryWriter writer) {
            writer.Write(this.Value);
        }

        /// <inheritdoc />
        public override void ReadFull(BinaryReader reader) {
            this.SetValueWithoutDirty(reader.ReadString());
        }
    }
}