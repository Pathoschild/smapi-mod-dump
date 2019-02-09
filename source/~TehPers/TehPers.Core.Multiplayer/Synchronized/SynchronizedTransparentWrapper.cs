using System.IO;

namespace TehPers.Core.Multiplayer.Synchronized {
    public class SynchronizedTransparentWrapper : ISynchronizedWrapper<ISynchronized> {
        public ISynchronized Value { get; set; }
        public bool Dirty => this.Value.Dirty;

        public SynchronizedTransparentWrapper(ISynchronized value) {
            this.Value = value;
        }

        public void WriteFull(BinaryWriter writer) {
            this.Value.WriteFull(writer);
        }

        public void ReadFull(BinaryReader reader) {
            this.Value.ReadFull(reader);
        }

        public void MarkClean() {
            this.Value.MarkClean();
        }

        public void WriteDelta(BinaryWriter writer) {
            this.Value.WriteDelta(writer);
        }

        public void ReadDelta(BinaryReader reader) {
            this.Value.ReadDelta(reader);
        }
    }
}
