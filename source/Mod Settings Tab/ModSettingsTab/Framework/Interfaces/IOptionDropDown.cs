using System.Collections.Generic;

namespace ModSettingsTab.Framework.Interfaces
{
    public interface IOptionDropDown : IModOption
    {
        List<string> DropDownOptions { get; set; }

        int DropDownSelectedOption { get; set; }
    }
}