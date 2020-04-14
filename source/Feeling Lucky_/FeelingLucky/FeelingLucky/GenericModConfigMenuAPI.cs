using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FeelingLucky
{
    public interface IGenericModConfigMenuAPI
        {
        //Also TODO: comment the hell out of this
            void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
            void RegisterLabel(IManifest mod, string labelName, string labelDesc);
            void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        }
}
