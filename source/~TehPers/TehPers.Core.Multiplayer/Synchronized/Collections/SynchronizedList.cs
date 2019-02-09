using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TehPers.Core.Multiplayer.Synchronized.Collections {
    public class SynchronizedList<T> : ISynchronized, IList<T> {
        private readonly Func<T, ISynchronizedWrapper<T>> _synchronizerFactory;
        private IList<ISynchronizedWrapper<T>> _synchronizers = new List<ISynchronizedWrapper<T>>();
        private readonly List<IDelta> _deltas = new List<IDelta>();
        private bool _fullUpdateRequired = false;

        public bool Dirty => this._fullUpdateRequired || this._deltas.Any();

        public SynchronizedList(Func<T, ISynchronizedWrapper<T>> synchronizerFactory) {
            this._synchronizerFactory = synchronizerFactory;
        }

        public void WriteFull(BinaryWriter writer) {
            // Write length
            writer.Write(this.Count);

            // Write each element
            foreach (ISynchronizedWrapper<T> element in this._synchronizers) {
                element.WriteFullWithLength(writer);
            }
        }

        public void ReadFull(BinaryReader reader) {
            // Read length
            int remaining = reader.ReadInt32();

            // Read each element, recreating the entire list
            this._synchronizers.Clear();
            while (remaining-- > 0) {
                ISynchronizedWrapper<T> synchronizer = this._synchronizerFactory(default);
                synchronizer.ReadFullWithLength(reader);
                this._synchronizers.Add(synchronizer);
            }
        }

        public void MarkClean() {
            this._deltas.Clear();
            this._fullUpdateRequired = false;
            foreach (ISynchronizedWrapper<T> synchronizer in this._synchronizers) {
                synchronizer.MarkClean();
            }
        }

        public void CopyFrom(IEnumerable<T> source) {
            this._fullUpdateRequired = true;
            this._synchronizers = source.Select(v => this._synchronizerFactory(v)).ToList();
        }

        public void WriteDelta(BinaryWriter writer) {
            // If a full update is needed anyway, don't bother processing the rest of the deltas
            if (this._fullUpdateRequired) {
                this._deltas.Clear();
                this._deltas.Add(new UpdateAllDelta());
            }

            // Write how many deltas there are
            writer.Write(this._deltas.Count);

            // Write each delta
            foreach (IDelta delta in this._deltas) {
                switch (delta) {
                    case UpdateAllDelta _:
                        writer.Write((byte) 0);
                        this.WriteFull(writer);
                        break;
                    case ClearDelta _:
                        writer.Write((byte) 1);
                        break;
                    case AddDelta addDelta:
                        writer.Write((byte) 2);
                        addDelta.Synchronizer.WriteFullWithLength(writer);
                        break;
                    case InsertDelta insertDelta:
                        writer.Write((byte) 3);
                        writer.Write(insertDelta.Index);
                        insertDelta.Synchronizer.WriteFullWithLength(writer);
                        break;
                    case UpdateDelta updateDelta:
                        writer.Write((byte) 4);
                        writer.Write(updateDelta.Index);
                        updateDelta.Synchronizer.WriteDeltaWithLength(writer);
                        break;
                    case RemoveDelta removeDelta:
                        writer.Write((byte) 5);
                        writer.Write(removeDelta.Index);
                        break;
                }
            }
        }

        public void ReadDelta(BinaryReader reader) {
            // Read how many deltas there are
            int remaining = reader.ReadInt32();

            // Read each delta
            while (remaining-- > 0) {
                byte deltaType = reader.ReadByte();

                switch (deltaType) {
                    case 0: // Update all
                        this.ReadFull(reader);
                        break;
                    case 1: // Clear
                        this._synchronizers.Clear();
                        break;
                    case 2: // Add
                        ISynchronizedWrapper<T> added = this._synchronizerFactory(default);
                        added.ReadFullWithLength(reader);
                        this._synchronizers.Add(added);
                        break;
                    case 3: // Insert
                        int insertIndex = reader.ReadInt32();
                        ISynchronizedWrapper<T> inserted = this._synchronizerFactory(default);
                        this._synchronizers.Insert(insertIndex, inserted);
                        inserted.ReadFullWithLength(reader);
                        break;
                    case 4: // Update
                        int updateIndex = reader.ReadInt32();
                        this._synchronizers[updateIndex].ReadDeltaWithLength(reader);
                        break;
                    case 5: // Remove
                        int removeIndex = reader.ReadInt32();
                        this._synchronizers.RemoveAt(removeIndex);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown delta type {deltaType}");
                }
            }
        }

        #region IList<T>
        public IEnumerator<T> GetEnumerator() {
            return this._synchronizers.Select(e => e.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) this._synchronizers).GetEnumerator();
        }

        public void Add(T item) {
            ISynchronizedWrapper<T> synchronizer = this._synchronizerFactory(item);
            this._synchronizers.Add(synchronizer);
            this._deltas.Add(new AddDelta(synchronizer));
        }

        public void Clear() {
            // Previous deltas don't matter anymore if the whole list is being cleared
            this._synchronizers.Clear();
            this._deltas.Clear();
            this._deltas.Add(new ClearDelta());
        }

        public bool Contains(T item) {
            return this._synchronizers.Select(e => e.Value).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            this._synchronizers.Select(e => e.Value).ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            // Use RemoveAt so it can be tracked
            int index = this.IndexOf(item);
            if (index == -1) {
                return false;
            }
            this.RemoveAt(index);
            return true;
        }

        public int Count => this._synchronizers.Count;

        public bool IsReadOnly => this._synchronizers.IsReadOnly;

        public int IndexOf(T item) {
            for (int i = 0; i < this._synchronizers.Count; i++) {
                if (object.Equals(this._synchronizers[i].Value, item)) {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item) {
            ISynchronizedWrapper<T> synchronizer = this._synchronizerFactory(item);
            this._synchronizers.Insert(index, synchronizer);
            this._deltas.Add(new InsertDelta(index, synchronizer));
        }

        public void RemoveAt(int index) {
            this._synchronizers.RemoveAt(index);
            this._deltas.Add(new RemoveDelta(index));
        }

        public T this[int index] {
            get => this._synchronizers[index].Value;
            set {
                ISynchronizedWrapper<T> synchronizer = this._synchronizers[index];
                synchronizer.Value = value;

                // Only add an update delta if there isn't an insert delta for this synchronizer because the insert delta will send the updated value if it exists
                if (synchronizer.Dirty && !this._deltas.Any(d => d is InsertDelta insertDelta && insertDelta.Synchronizer == synchronizer)) {
                    this._deltas.Add(new UpdateDelta(index, synchronizer));
                }
            }
        }
        #endregion

        #region Delta Operations
        private interface IDelta { }

        private class AddDelta : IDelta {
            public ISynchronizedWrapper<T> Synchronizer { get; }

            // Copy data from the synchronizer
            public AddDelta(ISynchronizedWrapper<T> synchronizer) {
                using (MemoryStream dataStream = new MemoryStream())
                using (BinaryWriter dataWriter = new BinaryWriter(dataStream)) {
                    synchronizer.WriteFullWithLength(dataWriter);
                    this.Synchronizer = synchronizer;
                }
            }
        }

        private class InsertDelta : IDelta {
            public int Index { get; }
            public ISynchronizedWrapper<T> Synchronizer { get; }

            public InsertDelta(int index, ISynchronizedWrapper<T> synchronizer) {
                this.Index = index;
                this.Synchronizer = synchronizer;
            }
        }

        private class UpdateDelta : IDelta {
            public int Index { get; }
            public ISynchronizedWrapper<T> Synchronizer { get; }

            public UpdateDelta(int index, ISynchronizedWrapper<T> synchronizer) {
                this.Index = index;
                this.Synchronizer = synchronizer;
            }
        }

        private class RemoveDelta : IDelta {
            public int Index { get; }

            public RemoveDelta(int index) {
                this.Index = index;
            }
        }

        private class ClearDelta : IDelta { }

        private class UpdateAllDelta : IDelta { }
        #endregion
    }
}
