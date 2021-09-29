/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TwinBuilderOne/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace DynamicScriptLoader
{
    public class Macro
    {
        public string Name { get; }
        public SButton Key { get; }
        public MacroType Type { get; }

        public Macro(string name, SButton key, MacroType type)
        {
            Name = name;
            Key = key;
            Type = type;
        }

        public override string ToString() => $"{{Name: {Name}, Key: {Key}, Type: {Type}}}";
    }

    public enum MacroType
    {
        Pressed,
        Released,
        Held,
        Toggled
    }
}
