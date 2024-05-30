/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Torsang/FarmingToolsPatch
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace FarmingToolsPatch
{
    public enum Pwr
    {
        Copper,
        Steel,
        Gold,
        Iridium
    }

    public class ModConfig
    {
        public KeybindList incLengthBtn, incRadiusBtn, decLengthBtn, decRadiusBtn, cyclePwrLvl;
        public SButtonState resetHold;
        public int pwrIndex, resetTime;
        public int cLength, sLength, gLength, iLength;
        public int cRadius, sRadius, gRadius, iRadius;
        public bool cBool, sBool, gBool, iBool, hKeyBool, resetBool;

        public ModConfig()
        {
            //Hotkey fields
            incLengthBtn = new KeybindList(SButton.OemOpenBrackets);
            incRadiusBtn = new KeybindList(SButton.OemCloseBrackets);
            decLengthBtn = new KeybindList(SButton.OemSemicolon);
            decRadiusBtn = new KeybindList(SButton.OemQuotes);
            cyclePwrLvl = new KeybindList(SButton.OemPipe);
            hKeyBool = true;
            resetBool = false;
            pwrIndex = (int)Pwr.Copper;
            resetTime = 2;

            //Iridium fields
            iLength = 5;
            iRadius = 2;
            iBool = true;

            //Gold fields
            gLength = 6;
            gRadius = 1;
            gBool = true;

            //Steel fields
            sLength = 3;
            sRadius = 1;
            sBool = true;

            //Copper fields
            cLength = 3;
            cRadius = 0;
            cBool = true;
        }
    }
}
