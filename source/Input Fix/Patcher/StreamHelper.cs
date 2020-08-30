using System;
using System.IO;
using System.Text;

namespace Patcher
{
    public class StreamHelper
    {
        public static void WriteString(Stream stream, string str)
        {
            WriteByte(stream, Encoding.Unicode.GetBytes(str));
        }

        public static void WriteByte(Stream stream, byte[] data)
        {
            WriteInt32(stream, data.Length);
            foreach (var item in data)
            {
                stream.WriteByte(item);
            }
        }

        public static void WriteInt32(Stream stream, int val)
        {
            byte[] val_ = BitConverter.GetBytes(val);
            foreach (var item in val_)
            {
                stream.WriteByte(item);
            }
        }

        public static string ReadString(Stream stream)
        {
            return Encoding.Unicode.GetString(ReadByte(stream));
        }

        public static byte[] ReadByte(Stream stream)
        {
            int val_len = ReadInt32(stream);
            byte[] data = new byte[val_len];
            for (int i = 0; i < val_len; i++)
            {
                data[i] = (byte)stream.ReadByte();
            }
            return data;
        }

        public static int ReadInt32(Stream stream)
        {
            byte[] data = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                data[i] = (byte)stream.ReadByte();
            }
            return BitConverter.ToInt32(data, 0);
        }

        public static void WriteFile(Stream stream, string path)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            FileStream fileStream = new FileStream(path,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Flush();
            fileStream.Close();
        }
    }
}