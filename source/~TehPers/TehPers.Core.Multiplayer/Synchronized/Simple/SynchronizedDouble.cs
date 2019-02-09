using System.IO;

namespace TehPers.Core.Multiplayer.Synchronized.Simple {
    internal class SynchronizedDouble : SynchronizedSimple<double> {
        public SynchronizedDouble(double initialValue) : base(initialValue) { }

        /// <inheritdoc />
        public override void WriteFull(BinaryWriter writer) {
            writer.Write(this.Value);
        }

        /// <inheritdoc />
        public override void ReadFull(BinaryReader reader) {
            this.SetValueWithoutDirty(reader.ReadDouble());
        }
    }
}