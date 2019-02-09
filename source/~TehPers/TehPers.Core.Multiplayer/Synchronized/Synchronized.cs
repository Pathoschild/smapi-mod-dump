using System.Collections.Generic;
using System.IO;
using TehPers.Core.Multiplayer.Synchronized.Collections;
using TehPers.Core.Multiplayer.Synchronized.Simple;

namespace TehPers.Core.Multiplayer.Synchronized {
    public static class Synchronized {

        public static ISynchronizedWrapper<byte> MakeSynchronized(this byte initialValue) => new SynchronizedByte(initialValue);
        public static ISynchronizedWrapper<short> MakeSynchronized(this short initialValue) => new SynchronizedInt16(initialValue);
        public static ISynchronizedWrapper<int> MakeSynchronized(this int initialValue) => new SynchronizedInt32(initialValue);
        public static ISynchronizedWrapper<long> MakeSynchronized(this long initialValue) => new SynchronizedInt64(initialValue);
        
        public static ISynchronizedWrapper<sbyte> MakeSynchronized(this sbyte initialValue) => new SynchronizedSByte(initialValue);
        public static ISynchronizedWrapper<ushort> MakeSynchronized(this ushort initialValue) => new SynchronizedUInt16(initialValue);
        public static ISynchronizedWrapper<uint> MakeSynchronized(this uint initialValue) => new SynchronizedUInt32(initialValue);
        public static ISynchronizedWrapper<ulong> MakeSynchronized(this ulong initialValue) => new SynchronizedUInt64(initialValue);

        public static ISynchronizedWrapper<float> MakeSynchronized(this float initialValue) => new SynchronizedSingle(initialValue);
        public static ISynchronizedWrapper<double> MakeSynchronized(this double initialValue) => new SynchronizedDouble(initialValue);
        public static ISynchronizedWrapper<decimal> MakeSynchronized(this decimal initialValue) => new SynchronizedDecimal(initialValue);

        public static ISynchronizedWrapper<bool> MakeSynchronized(this bool initialValue) => new SynchronizedBoolean(initialValue);
        public static ISynchronizedWrapper<char> MakeSynchronized(this char initialValue) => new SynchronizedChar(initialValue);
        public static ISynchronizedWrapper<string> MakeSynchronized(this string initialValue) => new SynchronizedString(initialValue);

        public static ISynchronizedWrapper<T?> MakeNullable<T>(this ISynchronizedWrapper<T> valueSynchronizer) where T : struct => valueSynchronizer.MakeNullable(valueSynchronizer.Value);
        public static ISynchronizedWrapper<T?> MakeNullable<T>(this ISynchronizedWrapper<T> valueSynchronizer, T? initialValue) where T : struct {
            return new SynchronizedNullable<T>(valueSynchronizer, initialValue);
        }

        internal static void WriteDeltaWithLength(this ISynchronized synchronized, BinaryWriter writer) {
            // Write synchronized object to a buffer
            byte[] data;
            using (MemoryStream dataStream = new MemoryStream())
            using (BinaryWriter dataWriter = new BinaryWriter(dataStream)) {
                synchronized.WriteDelta(dataWriter);
                data = dataStream.ToArray();
            }

            // Write the buffer to the stream
            writer.Write(data.Length);
            writer.Write(data);
        }

        internal static void ReadDeltaWithLength(this ISynchronized synchronized, BinaryReader reader) {
            int length = reader.ReadInt32();
            byte[] data = reader.ReadBytes(length);

            using (MemoryStream dataStream = new MemoryStream(data))
            using (BinaryReader dataReader = new BinaryReader(dataStream)) {
                synchronized.ReadDelta(dataReader);
            }
        }

        internal static void WriteFullWithLength(this ISynchronized synchronized, BinaryWriter writer) {
            // Write synchronized object to a buffer
            byte[] data;
            using (MemoryStream dataStream = new MemoryStream())
            using (BinaryWriter dataWriter = new BinaryWriter(dataStream)) {
                synchronized.WriteFull(dataWriter);
                data = dataStream.ToArray();
            }

            // Write the buffer to the stream
            writer.Write(data.Length);
            writer.Write(data);
        }

        internal static void ReadFullWithLength(this ISynchronized synchronized, BinaryReader reader) {
            int length = reader.ReadInt32();
            byte[] data = reader.ReadBytes(length);

            using (MemoryStream dataStream = new MemoryStream(data))
            using (BinaryReader dataReader = new BinaryReader(dataStream)) {
                synchronized.ReadFull(dataReader);
            }
        }
    }
}