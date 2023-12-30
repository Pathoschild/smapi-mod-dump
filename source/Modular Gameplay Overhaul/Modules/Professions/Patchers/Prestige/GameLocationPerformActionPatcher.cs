/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformActionPatcher"/> class.</summary>
    internal GameLocationPerformActionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformActionPrefix(GameLocation __instance, string? action, Farmer who)
    {
        if (!ProfessionsModule.EnableSkillReset || action?.Contains("DogStatue") != true ||
            !who.IsLocalPlayer)
        {
            return true; // run original logic
        }

        try
        {
            string message;
            if (!ProfessionsModule.Config.Prestige.AllowMultipleResets && ProfessionsModule.State.SkillsToReset.Count > 0)
            {
                message = I18n.Prestige_DogStatue_Dismiss();
                Game1.drawObjectDialogue(message);
                return false; // don't run original logic
            }

            if (TryOfferSkillReset(__instance) || TryOfferRespecOptions(__instance))
            {
                return false; // don't run original logic
            }

            message = I18n.Prestige_DogStatue_First();
            Game1.drawObjectDialogue(message);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    #region dialog handlers

    private static bool TryOfferSkillReset(GameLocation location)
    {
        if (!ISkill.CanResetAny())
        {
            return false;
        }

        var message = I18n.Prestige_DogStatue_First();
        if (ProfessionsModule.Config.Prestige.ForgetRecipesOnSkillReset)
        {
            message += I18n.Prestige_DogStatue_Forget();
        }

        message += I18n.Prestige_DogStatue_Offer();
        location.createQuestionDialogue(message, location.createYesNoResponses(), "dogStatue");
        return true;
    }

    private static bool TryOfferRespecOptions(GameLocation location)
    {
        var message = I18n.Prestige_DogStatue_What();
        var options = Array.Empty<Response>();

        if (ProfessionsModule.Config.Limit.EnableLimitBreaks &&
            Game1.player.Get_Ultimate() is not null)
        {
            options = options.Concat(new Response[]
            {
                new(
                    "changeUlt",
                    I18n.Prestige_DogStatue_Changeult() +
                    (ProfessionsModule.Config.Limit.LimitRespecCost > 0
                        ? ' ' + I18n.Prestige_DogStatue_Cost(ProfessionsModule.Config.Limit.LimitRespecCost)
                        : string.Empty)),
            }).ToArray();
        }

        if (Skill.List.Any(s => GameLocation.canRespec(s)))
        {
            options = options.Concat(new Response[]
            {
                new(
                    "prestigeRespec",
                    I18n.Prestige_DogStatue_Respec() +
                    (ProfessionsModule.Config.Prestige.PrestigeRespecCost > 0
                        ? ' ' + I18n.Prestige_DogStatue_Cost(ProfessionsModule.Config.Prestige.PrestigeRespecCost)
                        : string.Empty)),
            }).ToArray();
        }

        if (options.Length <= 0)
        {
            return false;
        }

        location.createQuestionDialogue(message, options, "dogStatue");
        return true;
    }

    #endregion dialog handlers
}
