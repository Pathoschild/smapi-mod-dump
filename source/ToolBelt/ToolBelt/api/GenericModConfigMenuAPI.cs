/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/yuri0r/toolbelt
**
*************************************************/

using StardewModdingAPI;
using System;

namespace ToolBelt.api
{
    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void SetDefaultIngameOptinValue(IManifest mod, bool optedIn);
        void RegisterLabel(IManifest mod, string labelName, string labelDesc);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<SButton> optionGet, Action<SButton> optionSet);
        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);
    }
}
