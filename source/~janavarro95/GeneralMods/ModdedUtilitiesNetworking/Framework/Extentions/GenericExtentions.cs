using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;


namespace ModdedUtilitiesNetworking.Framework.Extentions
{
    public static class GenericExtentions
    {       
        
       public static string ReadString(this BinaryReader reader)
        {
            String s= reader.ReadString();
            return new string(s.ToCharArray());
        }

        public static void WriteString(this BinaryWriter writer, object str)
        {
            writer.Write((string)str);
            
        }

        /// <summary>
        /// Writes a string list to a binary stream.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="strList">The list to write.</param>
        public static void WriteStringList(this BinaryWriter writer, object strList)
        {
            List<string> list =(List<string>)strList;
            writer.Write(list.Count);
            for(int i=0; i<list.Count; i++)
            {
                writer.WriteString(list.ElementAt(i));
            }
        }

        /// <summary>
        /// Reads a string list from the binary data.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<String> ReadStringList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            List<string> strList = new List<string>();
            for(int i = 0; i < count; i++)
            {
                string s=reader.ReadString();
                strList.Add(s);
            }
            return strList;
        }

        /// <summary>
        /// Read the custom info packet sent from a modded client or server.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static object[] ReadModdedInfoPacket(this BinaryReader reader)
        {
            object[] o = new object[2]
            {
                reader.ReadString(),
                reader.ReadString()
            };
            return o;
        }

        /// <summary>
        /// Read the remaining byte data in an array.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            using (var memoryStream = new MemoryStream())
            {
                reader.BaseStream.CopyTo(memoryStream);
                
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Read a data info file from a binary stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DataInfo ReadDataInfo(this BinaryReader reader)
        {
            String key=reader.ReadString();
            object data = ModCore.processTypesToRead(reader, key);
            string ID = reader.ReadString();
            DataInfo info = new DataInfo(key,data,ID);
            return info;
        }
        
        /// <summary>
        /// Write a dataInfo file to binary.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="obj"></param>
        public static void WriteDataInfo(this BinaryWriter writer, object obj)
        {
            DataInfo dataInfo = (DataInfo)obj;
            writer.WriteString(dataInfo.type);
            //ModCore.monitor.Log("WRITE DATA INFO FUNCTION3: " + dataInfo.type);
            ModCore.processTypesToWrite(writer, dataInfo.type, dataInfo.data);
            writer.WriteString(dataInfo.recipientID);
        }

        //Can do custom classes here for reading and writing.
        //That way it will be better to save/load data
    }
}
