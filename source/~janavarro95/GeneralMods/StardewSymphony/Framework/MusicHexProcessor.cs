using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StardewValley;

namespace Omegasis.StardewSymphony.Framework
{
    internal class MusicHexProcessor
    {
        /*********
        ** Properties
        *********/
        /// <summary>All of the music/soundbanks and their locations.</summary>
        private readonly IList<MusicManager> MasterList;

        /// <summary>The registered soundbanks.</summary>
        private readonly List<string> SoundBanks = new List<string>();

        /// <summary>The callback to reset the game audio.</summary>
        private readonly Action Reset;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="masterList">All of the music/soundbanks and their locations.</param>
        /// <param name="reset">The callback to reset the game audio.</param>
        public MusicHexProcessor(IList<MusicManager> masterList, Action reset)
        {
            this.MasterList = masterList;
            this.Reset = reset;
        }

        /// <summary>Add a file path to the list of soundbanks.</summary>
        /// <param name="path">The soundbank file path.</param>
        public void AddSoundBank(string path)
        {
            this.SoundBanks.Add(path);
        }

        public void ProcessHex()
        {
            int counter = 0;
            foreach (string entry in this.SoundBanks)
            {
                List<string> cleanCueNames = new List<string>();
                byte[] array = File.ReadAllBytes(entry);
                string rawName = entry.Substring(0, entry.Length - 4);
                string cueName = rawName + "CueList.txt";
                if (File.Exists(cueName)) continue;

                string hexDumpContents = this.HexDump(array);

                string rawHexName = rawName + "HexDump.txt";
                File.WriteAllText(rawHexName, hexDumpContents);

                string[] readText = File.ReadAllLines(rawHexName);
                string largeString = "";
                foreach (var line in readText)
                {
                    try
                    {
                        string newString = "";
                        for (int i = 62; i <= 77; i++)
                            newString += line[i];
                        largeString += newString;
                    }
                    catch { }
                }
                string[] splits = largeString.Split('ÿ');
                string fix = "";
                foreach (string s in splits)
                {
                    if (s == "") continue;
                    fix += s;
                }
                splits = fix.Split('.');

                foreach (var split in splits)
                {
                    if (split == "") continue;

                    try
                    {
                        Game1.waveBank = this.MasterList[counter].Wavebank;
                        Game1.soundBank = this.MasterList[counter].Soundbank;

                        if (Game1.soundBank.GetCue(split) != null)
                            cleanCueNames.Add(split);

                        this.Reset();
                    }
                    catch { }
                }

                cueName = rawName + "CueList.txt";
                cleanCueNames.Sort();
                File.WriteAllLines(cueName, cleanCueNames);
                counter++;
            }
        }


        /*********
        ** Private methods
        *********/
        private string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null)
                return "<null>";

            int bytesLength = bytes.Length;

            char[] hexChars = "0123456789ABCDEF".ToCharArray();

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

            char[] line = (new String(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = hexChars[(i >> 28) & 0xF];
                line[1] = hexChars[(i >> 24) & 0xF];
                line[2] = hexChars[(i >> 20) & 0xF];
                line[3] = hexChars[(i >> 16) & 0xF];
                line[4] = hexChars[(i >> 12) & 0xF];
                line[5] = hexChars[(i >> 8) & 0xF];
                line[6] = hexChars[(i >> 4) & 0xF];
                line[7] = hexChars[(i >> 0) & 0xF];

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
                        line[hexColumn] = hexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = hexChars[b & 0xF];
                        line[charColumn] = this.GetAsciiSymbol(b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }

        private char GetAsciiSymbol(byte ch)
        {
            if (ch < 32) return '.';  // Non-printable ASCII
            if (ch < 127) return (char)ch;   // Normal ASCII
            // Handle the hole in Latin-1
            if (ch == 127) return '.';
            if (ch < 0x90) return "€.‚ƒ„…†‡ˆ‰Š‹Œ.Ž."[ch & 0xF];
            if (ch < 0xA0) return ".‘’“”•–—˜™š›œ.žŸ"[ch & 0xF];
            if (ch == 0xAD) return '.';   // Soft hyphen: this symbol is zero-width even in monospace fonts
            return (char)ch;   // Normal Latin-1
        }
    }
}
