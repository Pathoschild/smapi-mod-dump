using System.IO;

namespace TehPers.Core.Multiplayer.Synchronized.Simple {
    public abstract class SynchronizedSimple<T> : ISynchronizedWrapper<T> {
        private T _value;

        public bool Dirty { get; private set; }
        public T Value {
            get => this._value;
            set {
                if (object.Equals(this._value, value)) {
                    return;
                }

                // Update internal value and mark as dirty
                this._value = value;
                this.Dirty = true;
            }
        }

        protected SynchronizedSimple() : this(default) { }
        protected SynchronizedSimple(T initialValue) {
            this._value = initialValue;
        }

        protected void SetValueWithoutDirty(T value) {
            this._value = value;
        }

        /// <inheritdoc />
        public abstract void WriteFull(BinaryWriter writer);

        /// <inheritdoc />
        public abstract void ReadFull(BinaryReader reader);

        /// <inheritdoc />
        public void MarkClean() {
            this.Dirty = false;
        }

        /// <inheritdoc />
        public virtual void WriteDelta(BinaryWriter writer) {
            this.WriteFull(writer);
        }

        /// <inheritdoc />
        public virtual void ReadDelta(BinaryReader reader) {
            this.ReadFull(reader);
        }
    }
}