/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardustCore.Utilities.Serialization;

namespace StardustCore.Utilities
{
    public class HexDumper
    {

        /// <summary>
        /// Dumps the contents of a file to a hex file.
        /// </summary>
        /// <param name="InFile"></param>
        /// <param name="OutFile"></param>
        public static void HexDumpFile(string InFile, string OutFile)
        {
            if (File.Exists(InFile))
            {
                byte[] buffer = File.ReadAllBytes(InFile);
                string hexInfo = HexDump(buffer);
                Serializer s = new Serializer();
                s.Serialize(OutFile, buffer);
            }
        }

        public static string HexDumpString(string InFile)
        {
            if (File.Exists(InFile))
            {
                byte[] buffer = File.ReadAllBytes(InFile);
                string hexInfo = HexDump(buffer);
                return hexInfo;
            }
            return "";
        }

        public static void StripSoundCuesToFile(string FileName,List<string> SoundCues)
        {
            Serializer s = new Serializer();
            s.Serialize(FileName, SoundCues);
        }

        public static List<string> StripSoundCuesFromHex(string HexString)
        {
            string[] lines = HexString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            StringBuilder b = new StringBuilder();
            foreach (string s in lines)
            {
                try
                {
                    string sub = s.Substring(62, s.Length - 62); //Throw away the first 62 trash characters;
                    sub = sub.Replace("ÿ", "");
                    b.Append(sub);
                    //ModCore.ModMonitor.Log(sub, StardewModdingAPI.LogLevel.Info);
                }
                catch(Exception err)
                {
                    //ModCore.ModMonitor.Log("Failed on: "+s, StardewModdingAPI.LogLevel.Info);
                }
            }

            string ok = b.ToString();
            string[] split1=ok.Split(new string[] { "doorClose" }, StringSplitOptions.None);
            string s2 = split1[1];
            string[] split2 = s2.Split('·');
            List<string> hexLess = new List<string>();
            hexLess.Add("doorClose");
            foreach (string cue in split2)
            {
                if (string.IsNullOrEmpty(cue)) continue;
                //ModCore.ModMonitor.Log("Cue is:"+cue, StardewModdingAPI.LogLevel.Info);
                hexLess.Add(cue);
            }

            return hexLess;
        }

        public static string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new string(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }
    }
}
