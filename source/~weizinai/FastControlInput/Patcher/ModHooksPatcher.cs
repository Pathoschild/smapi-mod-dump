/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Mods;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.FastControlInput.Framework;

namespace weizinai.StardewValleyMod.FastControlInput.Patcher;

internal class ModHooksPatcher : BasePatcher
{
    private static Func<ModConfig> getConfig = null!;
    private static ModConfig Config => getConfig();

    private static float actionButtonRemainder;
    private static float useToolButtonRemainder;

    public ModHooksPatcher(Func<ModConfig> getConfig)
    {
        ModHooksPatcher.getConfig = getConfig;
    }
    
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(this.RequireMethod<ModHooks>(nameof(ModHooks.OnGame1_UpdateControlInput)),
            postfix: this.GetHarmonyMethod(nameof(UpdateControlInputPostfix))
        );
    }

    private static void UpdateControlInputPostfix()
    {
        var gameTime = Game1.currentGameTime;
         
        for (var i = 0; i < GetSkipsThisTick(Config.ActionButton,ref actionButtonRemainder); i++)
            Game1.rightClickPolling -= gameTime.ElapsedGameTime.Milliseconds;
        
        for (var i = 0; i < GetSkipsThisTick(Config.UseToolButton,ref useToolButtonRemainder); i++) 
            Game1.mouseClickPolling += gameTime.ElapsedGameTime.Milliseconds;
    }

    private static int GetSkipsThisTick(float multiplier, ref float remainder)
    {
        if (multiplier <= 1) return 0;

        var skips = multiplier + remainder - 1;
        remainder = skips % 1;
        return (int)skips;
    }
}