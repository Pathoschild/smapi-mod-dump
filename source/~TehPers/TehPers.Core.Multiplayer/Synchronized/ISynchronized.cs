using System.IO;

namespace TehPers.Core.Multiplayer.Synchronized {
    public interface ISynchronized {
        /// <summary>True if this object has been modified and needs to send an update.</summary>
        bool Dirty { get; }

        /// <summary>Writes the full object.</summary>
        /// <param name="writer">The writer to write the full value to.</param>
        void WriteFull(BinaryWriter writer);

        /// <summary>Reads the full object.</summary>
        /// <param name="reader">The reader to read from.</param>
        void ReadFull(BinaryReader reader);

        /// <summary>Marks this object as clean.</summary>
        void MarkClean();

        /// <summary>Writes any changes to this value.</summary>
        /// <param name="writer">The writer to write changes to.</param>
        void WriteDelta(BinaryWriter writer);

        /// <summary>Reads any changes to this value.</summary>
        /// <param name="reader">The reader to read changes from.</param>
        void ReadDelta(BinaryReader reader);
    }
}