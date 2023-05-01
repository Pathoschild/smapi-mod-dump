/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Weapons.Events;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

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

    /// <summary>Add Dark Sword + transformation.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformActionPrefix(GameLocation __instance, string? action, Farmer who)
    {
        if (!WeaponsModule.Config.InfinityPlusOne || action is null || !who.IsLocalPlayer)
        {
            return true; // run original logic
        }

        try
        {
            if (action.StartsWith("Yoba"))
            {
                HandleYobaAltar(__instance, who);
            }
            else if (action.StartsWith("GoldenScythe"))
            {
                HandleReaperStatue(__instance, who);
            }
            else
            {
                return true; // run original logic
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

    #region handlers

    private static void HandleYobaAltar(GameLocation location, Farmer who)
    {
        if (!who.mailReceived.Contains("gotHolyBlade") && who.hasQuest((int)Quest.HeroReward) &&
            who.CurrentTool is MeleeWeapon { InitialParentTileIndex: ItemIDs.DarkSword })
        {
            who.Halt();
            who.CanMove = false;
            who.faceDirection(2);
            who.showCarrying();
            who.jitterStrength = 1f;
            Game1.pauseThenDoFunction(3000, GetHolyBlade);
            Game1.changeMusicTrack("none", false, Game1.MusicContext.Event);
            location.playSound("crit");
            Game1.screenGlowOnce(Color.Transparent, true, 0.01f, 0.999f);
            DelayedAction.playSoundAfterDelay("stardrop", 1500);
            Game1.screenOverlayTempSprites.AddRange(
                Utility.sparkleWithinArea(
                    new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
                    500,
                    Color.Gold,
                    10,
                    2000));
            Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(
                Game1.afterDialogues,
                (Game1.afterFadeFunction)(() => Game1.stopMusicTrack(Game1.MusicContext.Event)));
            who.completeQuest((int)Quest.HeroReward);
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Get("locations.SeedShop.Yoba"));
            if (!who.hasQuest((int)Quest.CurseIntro))
            {
                return;
            }

            Game1.afterDialogues = () =>
            {
                string question = I18n.Get("yoba.question");
                var responses = Virtue.List
                    .Select(v => new Response(v.Name, v.DisplayName))
                    .ToArray();
                location.createQuestionDialogue(question, responses, "Yoba");
            };
        }
    }

    private static void HandleReaperStatue(GameLocation location, Farmer who)
    {
        if (!who.mailReceived.Contains("gotGoldenScythe"))
        {
            if (!who.isInventoryFull())
            {
                Game1.playSound("parry");
                who.mailReceived.Add("gotGoldenScythe");
                location.setMapTileIndex(29, 4, 245, "Front");
                location.setMapTileIndex(30, 4, 246, "Front");
                location.setMapTileIndex(29, 5, 261, "Front");
                location.setMapTileIndex(30, 5, 262, "Front");
                location.setMapTileIndex(29, 6, 277, "Buildings");
                location.setMapTileIndex(30, 56, 278, "Buildings");
                who.addItemByMenuIfNecessaryElseHoldUp(new MeleeWeapon(ItemIDs.GoldenScythe));
            }
            else
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }
        }
        else if (!who.mailReceived.Contains("gotDarkSword") && !who.isInventoryFull())
        {
            ProposeGrabDarkSword(location);
        }
        else
        {
            Game1.changeMusicTrack("none");
            location.performTouchAction("MagicWarp Mine 67 10", who.getStandingPosition());
        }
    }

    private static void ProposeGrabDarkSword(GameLocation location)
    {
        Game1.multipleDialogues(new string[]
        {
            I18n.Get("weapons.darksword.found"),
            I18n.Get("weapons.darksword.chill"),
        });

        Game1.afterDialogues = () => location.createQuestionDialogue(
            I18n.Get("weapons.darksword.question"),
            new Response[]
            {
                new("GrabIt", I18n.Get("weapons.darksword.grabit")),
                new("LeaveIt", I18n.Get("weapons.darksword.leaveit")),
            },
            "DarkSword");
    }

    private static void GetHolyBlade()
    {
        var player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: ItemIDs.DarkSword } darkSword)
        {
            Log.W($"[WPNZ]: {player.Name} cannot receive the Holy Blade because they are not holding the Dark Sword!");
            return;
        }

        Game1.flashAlpha = 1f;
        player.holdUpItemThenMessage(new MeleeWeapon(ItemIDs.HolyBlade));
        darkSword.transform(ItemIDs.HolyBlade);
        darkSword.Write(DataKeys.CursePoints, null);
        darkSword.RefreshStats();
        player.jitterStrength = 0f;
        Game1.screenGlowHold = false;
        player.mailReceived.Add("gotHolyBlade");
        EventManager.Disable<CurseUpdateTickedEvent>();
    }

    #endregion handlers
}
