/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GilarF/SVM
**
*************************************************/

using System.Collections.Generic;

namespace ModSettingsTab.Framework.Integration
{
    public class Param
    {
        public string Name { get; set; } = null;
        public I18N Description { get; set; } = new I18N();
        public ParamType Type { get; set; } = ParamType.None;
        public I18N Label { get; set; } = new I18N();
        public bool Ignore { get; set; } = false;

        // DropDown
        public List<string> DropDownOptions { get; set; } = new List<string>();

        // PlusMinus
        public List<string> PlusMinusOptions { get; set; } = new List<string>();

        // Slider
        public int SliderMaxValue { get; set; } = 100;
        public int SliderMinValue { get; set; } = 0;
        public int SliderStep { get; set; } = 1;

        // TextBox
        public bool? TextBoxNumbersOnly { get; set; } = null;
        public bool? TextBoxFloatOnly { get; set; }  = null;
    }
}