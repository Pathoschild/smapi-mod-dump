/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ItemBags.Helpers
{
    public static class XMLSerializer
    {
        public static bool TrySerializeToString<T>(T Data, out string Result, out Exception Error)
        {
            try
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(T));
                using (var StringOutput = new StringWriter())
                {
                    using (var TextWriter = new XmlTextWriter(StringOutput) { Formatting = Formatting.Indented })
                    {
                        Serializer.Serialize(TextWriter, Data);
                        Result = StringOutput.ToString();
                        Error = null;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Result = null;
                Error = ex;
                return false;
            }
        }

        public static bool TryDeserializeFromString<T>(string Data, out T Result, out Exception Error)
        {
            try
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(T));
                using (TextReader Reader = new StringReader(Data))
                {
                    Result = (T)Serializer.Deserialize(Reader);
                    Error = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Result = default(T);
                Error = ex;
                return false;
            }
        }

        ///<summary>Serializes this object to the given file.  Warning: this will overwrite existing files.</summary>
        public static void Serialize<T>(T Data, string FullFilePath, out bool Successful, out Exception Error)
        {
            string Directory = Path.GetDirectoryName(FullFilePath);
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);

            try
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(T));

                //  Failsafe - ensure the object is properly serializable by trying to serialize to a string first.
                //  If this fails, assume the object is bad data and that we shouldn't overwrite an existing file with it.
                using (var StringOutput = new StringWriter())
                {
                    using (var TextWriter = new XmlTextWriter(StringOutput) { Formatting = Formatting.Indented })
                    {
                        Serializer.Serialize(TextWriter, Data);
                    }
                }

                using (TextWriter Writer = new StreamWriter(FullFilePath))
                {
                    Serializer.Serialize(Writer, Data);
                }
            }
            catch (Exception ex)
            {
                Successful = false;
                Error = ex;
                return;
            }

            Successful = true;
            Error = null;
        }

        public static T Deserialize<T>(string FullFilePath, out bool Successful, out Exception DeserializationError, bool ThrowExceptionIfFileNotFound = false)
        {
            if (ThrowExceptionIfFileNotFound && !File.Exists(FullFilePath))
            {
                Successful = false;
                DeserializationError = new FileNotFoundException("No file found to deserialize at: " + FullFilePath);
                return default(T);
            }

            try
            {
                if (File.Exists(FullFilePath))
                {
                    using (FileStream FileStream = File.Open(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        XmlDictionaryReader Reader = XmlDictionaryReader.CreateTextReader(FileStream, new XmlDictionaryReaderQuotas());
                        XmlSerializer Serializer = new XmlSerializer(typeof(T));
                        T Settings = (T)Serializer.Deserialize(Reader);
                        Successful = true;
                        DeserializationError = null;
                        return Settings;
                    }
                }
                else
                {
                    Successful = true;
                    DeserializationError = null;
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Unhandled error while attempting to deserialize the file at {0}:\n\n{1}", FullFilePath, ex.Message), LogLevel.Error);

                Successful = false;
                DeserializationError = ex;
                return default(T);
            }
        }
    }
}
