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

internal class StaminaToolAutomationConfig : ToolAutomationConfig
{
    public float StopStamina;
    
    public StaminaToolAutomationConfig(int range, float stopStamina, bool findToolFromInventory) 
        : base(range, findToolFromInventory)
    {
        this.StopStamina = stopStamina;
    }
}