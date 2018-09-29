using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.StardustMath
{
    /// <summary>
    /// A Class that helps represents 32 bit hex.
    /// </summary>
    class Hex32 : Hex
    {
        /// <summary>
        /// A default constructor.
        /// </summary>
        public Hex32()
        {
            this.hexValue = "0x00000000";
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="hexValue">A string in hex representation. Ex) 0x00000000</param>
        public Hex32(string hexValue)
        {
            this.hexValue = hexValue;
            if (verifyHexLength() == false) this.hexValue = "0x00000000";
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">An int to be converted into Hex.</param>
        public Hex32(int value)
        {
            this.hexValue= value.ToString("X");
            if (verifyHexLength() == false) this.hexValue = "0x00000000";
        }


        /// <summary>
        /// Makes sure the hex value is the appropriate length.
        /// </summary>
        /// <returns></returns>
        public override bool verifyHexLength()
        {
            if (this.hexValue.Length != 10) return false;
            else return true;
        }

        /// <summary>
        /// Trims the hex to get rid of the leading 0x;
        /// </summary>
        /// <returns></returns>
        public override string trimHex()
        {
            return base.trimHex();
        }

        /// <summary>
        /// Converts a hex value to a string.
        /// </summary>
        /// <returns></returns>
        public Color toColor()
        {
            var bytes = getBytes();
            int red = convertHexByteTo255Int(bytes[0]);
            int green = convertHexByteTo255Int(bytes[1]);
            int blue = convertHexByteTo255Int(bytes[2]);
            int alpha = convertHexByteTo255Int(bytes[3]);
            return new Color(red, green, blue, alpha);
        }

        /// <summary>
        /// Get the individual byte strings associated with this hex value.
        /// </summary>
        /// <returns></returns>
        public override List<string> getBytes()
        {
            List<string> bytes = new List<string>();
            bytes.Add(getByte(0));
            bytes.Add(getByte(1));
            bytes.Add(getByte(2));
            bytes.Add(getByte(3));
            return bytes;
        }
    }
}
