/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotAttachPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotAttachPatcher"/> class.</summary>
    internal SlingshotAttachPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.attach));
        this.Prefix!.before = new[] { "atravita.StopRugRemoval" };
    }

    #region harmony patches

    /// <summary>Patch to attach Rascal's additional ammo.</summary>
    [HarmonyPrefix]
    [HarmonyBefore("atravita.StopRugRemoval")]
    private static bool SlingshotAttachPrefix(Slingshot __instance, ref SObject? __result, SObject? o)
    {
        if (__instance.numAttachmentSlots.Value != 2 || __instance.attachments.Length != 2)
        {
            return true; // run original logic
        }

        try
        {
            var top = __instance.attachments[0];
            var bottom = __instance.attachments[1];
            if (o is not null)
            {
                if (top is null && (bottom?.canStackWith(o) != true || bottom.Stack == 999))
                {
                    __instance.attachments[0] = o;
                    __result = null;
                    Game1.playSound("button1");
                }
                else if (top is not null)
                {
                    if (top.canStackWith(o) && top.Stack < 999)
                    {
                        top.Stack = o.addToStack(top);
                        if (top.Stack <= 0)
                        {
                            top = null;
                        }

                        __instance.attachments[0] = o;
                        __result = top;
                        Game1.playSound("button1");
                    }
                    else if (bottom?.canStackWith(o) == true && bottom.Stack < 999)
                    {
                        bottom.Stack = o.addToStack(bottom);
                        if (bottom.Stack <= 0)
                        {
                            bottom = null;
                        }

                        __instance.attachments[1] = o;
                        __result = bottom;
                        Game1.playSound("button1");
                    }
                    else
                    {
                        if (bottom is null)
                        {
                            __instance.attachments[1] = o;
                            __result = null;
                            Game1.playSound("button1");
                        }
                        else
                        {
                            if (bottom.canStackWith(o) && bottom.Stack < 999)
                            {
                                bottom.Stack = o.addToStack(bottom);
                                if (bottom.Stack <= 0)
                                {
                                    bottom = null;
                                }
                            }

                            __instance.attachments[1] = o;
                            __result = bottom;
                            Game1.playSound("button1");
                        }
                    }
                }
            }
            else
            {
                if (__instance.attachments[0] is not null)
                {
                    __result = __instance.attachments[0];
                    __instance.attachments[0] = null;
                    Game1.playSound("button1");
                }
                else if (__instance.attachments[1] is not null)
                {
                    __result = __instance.attachments[1];
                    __instance.attachments[1] = null;
                    Game1.playSound("button1");
                }
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
