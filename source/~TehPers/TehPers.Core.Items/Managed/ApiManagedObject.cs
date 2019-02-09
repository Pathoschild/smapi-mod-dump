using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.Core.Multiplayer.Synchronized;
using TehPers.Core.Multiplayer.Synchronized.Collections;

namespace TehPers.Core.Items.Managed {
    public abstract class ApiManagedObject : IStackableObject {
        private readonly SynchronizedList<ISynchronized> _synchronizedProperties = new SynchronizedList<ISynchronized>(s => new SynchronizedTransparentWrapper(s));

        public int StackSize { get; set; }
        public int MaxStackSize { get; protected set; }

        public void AddToStack(int amount) {
            this.StackSize = Math.Min(this.StackSize + amount, this.MaxStackSize);
        }

        public bool CanStackWith(IStackableObject other) {
            return this.GetType() == other.GetType();
        }

        protected void KeepSynchronized(ISynchronized synchronized) {
            this._synchronizedProperties.Add(synchronized);
        }

        public abstract string GetDisplayName();
        public abstract string GetDescription();
        public abstract Texture2D GetTexture();
        public abstract Rectangle? GetSourceRectangle();

        #region ISynchronized
        public bool Dirty => this._synchronizedProperties.Any(s => s.Dirty);

        public virtual void WriteFull(BinaryWriter writer) {
            this._synchronizedProperties.WriteFull(writer);
        }

        public virtual void ReadFull(BinaryReader reader) {
            this._synchronizedProperties.ReadFull(reader);
        }

        public virtual void MarkClean() {
            this._synchronizedProperties.MarkClean();
        }

        public virtual void WriteDelta(BinaryWriter writer) {
            this._synchronizedProperties.WriteDelta(writer);
        }

        public virtual void ReadDelta(BinaryReader reader) {
            this._synchronizedProperties.ReadDelta(reader);
        }
        #endregion
    }
}