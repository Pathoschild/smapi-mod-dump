/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

namespace weizinai.StardewValleyMod.LazyMod.Framework.Config;

internal class BaseAutomationConfig
{
    public bool IsEnable { get; set; } 
    public int Range { get; set; }

    public BaseAutomationConfig(int range)
    {
        this.Range = range;
    }
}