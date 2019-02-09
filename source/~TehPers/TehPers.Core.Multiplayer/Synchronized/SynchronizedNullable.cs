using System.IO;

namespace TehPers.Core.Multiplayer.Synchronized {
    internal class SynchronizedNullable<T> : ISynchronizedWrapper<T?> where T : struct {
        private readonly ISynchronizedWrapper<T> _valueSynchronizer;
        private bool _isNull;

        public bool Dirty { get; private set; }
        public T? Value {
            get => (this._isNull ? (T?) null : this._valueSynchronizer.Value);
            set {
                if (object.Equals(value, this.Value)) {
                    return;
                }

                // Update internal value and mark as dirty
                this._isNull = value.HasValue;
                this._valueSynchronizer.Value = value ?? default;
                this.Dirty = true;
            }
        }

        public SynchronizedNullable(ISynchronizedWrapper<T> valueSynchronizer) : this(valueSynchronizer, default) { }
        public SynchronizedNullable(ISynchronizedWrapper<T> valueSynchronizer, T? initialValue) {
            this._valueSynchronizer = valueSynchronizer;
            this._isNull = initialValue.HasValue;
            this._valueSynchronizer.Value = initialValue ?? default;
        }

        /// <inheritdoc />
        public void WriteFull(BinaryWriter writer) {
            writer.Write(this._isNull);
            if (!this._isNull) {
                this._valueSynchronizer.WriteFull(writer);
            }
        }

        /// <inheritdoc />
        public void ReadFull(BinaryReader reader) {
            this._isNull = reader.ReadBoolean();
            if (!this._isNull) {
                this._valueSynchronizer.ReadFull(reader);
            }
        }

        /// <inheritdoc />
        public void MarkClean() {
            throw new System.NotImplementedException();
        }

        public void WriteDelta(BinaryWriter writer) {
            this.WriteFull(writer);
        }

        public void ReadDelta(BinaryReader reader) {
            this.ReadFull(reader);
        }
    }
}