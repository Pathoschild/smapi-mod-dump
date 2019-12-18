using System.Collections.Generic;

namespace ModSettingsTab.Framework.Interfaces
{
    public interface IOptionPlusMinus : IModOption
    {
        List<string> PlusMinusOptions { get; set; }

        int PlusMinusSelectedOption { get; set; }
    }
}