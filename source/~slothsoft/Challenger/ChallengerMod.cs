/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Menus;
using Slothsoft.Challenger.Objects;
using Slothsoft.Challenger.ThirdParty;
using StardewModdingAPI.Events;

namespace Slothsoft.Challenger;

// ReSharper disable once ClassNeverInstantiated.Global
public class ChallengerMod : Mod {
    public static ChallengerMod Instance = null!;

    private IChallengerApi? _api;
    internal ChallengerConfig Config = null!;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="modHelper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper modHelper) {
        Instance = this;
        Config = modHelper.ReadConfig<ChallengerConfig>();

        modHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        modHelper.Events.Input.ButtonPressed += OnButtonPressed;
        modHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
        modHelper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;

        modHelper.Events.GameLoop.DayStarted += OnDayStarted;
        modHelper.Events.GameLoop.DayEnding += OnDayEnding;
        
        // Patches
        MagicalObject.PatchObject(ModManifest.UniqueID);
        ChallengerMail.InitAndSend();
    }

    /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e) {
        // ignore if player hasn't loaded a save yet
        if (!Context.IsWorldReady)
            return;

        if (e.Button == Config.ButtonOpenMenu) {
            if (Game1.activeClickableMenu is ChallengeMenu) {
                Game1.activeClickableMenu.exitThisMenu();
            } else {
                Game1.activeClickableMenu = new ChallengeMenu();
            }
        }
    }

    public override IChallengerApi? GetApi() => _api;

    public bool IsInitialized() => _api != null;

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
        // hook other Mods if they exist
        HookToGenericModConfigMenu.Apply(this);
        HookToInformant.Apply(this);
    }
    
    /// <summary>
    /// This method sets up everything necessary for the current save file.
    /// <see cref="OnReturnToTitle"/>
    /// </summary>
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {
        _api = new ChallengerApi(Helper);
        Monitor.Log($"Challenge \"{_api.ActiveChallenge.DisplayName} ({_api.ActiveDifficulty})\" was initialized.", LogLevel.Debug);
    }
    
    /// <summary>
    /// This methods cleans up everything that was necessary in the current save file.
    /// <see cref="OnSaveLoaded"/>
    /// </summary>
    private void OnReturnToTitle(object? sender, ReturnedToTitleEventArgs e) {
        Monitor.Log($"Challenge \"{_api?.ActiveChallenge.DisplayName}\" was cleaned up.", LogLevel.Debug);
        _api?.Dispose();
        _api = null;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e) {
        var count = 0;
        foreach (var obj in FetchAllObjects().Where(IsVanillaSaveObject)) {
            RestoreVanillaObjectFromSave(obj);
            count++;
        }
        if (count == 0 && Game1.player.mailReceived.Contains(ChallengerMail.MagicalObjectMail) && !Game1.player.mailbox.Contains(ChallengerMail.MagicalObjectMail)) {
            // the player received the mail, but no longer has the magical object - resend it
            Game1.player.mailbox.Add(ChallengerMail.MagicalObjectLostMail);
            Game1.player.mailReceived.Add(ChallengerMail.MagicalObjectLostMail);
        }
    }

    private IEnumerable<SObject> FetchAllObjects() {
        foreach (var playerItem in Game1.player.Items) {
            if (playerItem is SObject item) {
                yield return item;
            }
        }

        foreach (var location in Game1.locations) {
            foreach (var obj in location.netObjects.Values) {
                yield return obj;
            }
        }
    }

    private static bool IsVanillaSaveObject(SObject instance) {
        return instance is { bigCraftable.Value: true} bigCraftable 
               && bigCraftable.ParentSheetIndex == MagicalReplacement.Default.ParentSheetIndex
               && bigCraftable.preservedParentSheetIndex.Value == MagicalObject.ObjectId;
    }
    
    private static void RestoreVanillaObjectFromSave(SObject obj) {
        obj.ParentSheetIndex = MagicalObject.ObjectId;
    }
    

    private void OnDayEnding(object? sender, DayEndingEventArgs e) {
        foreach (var obj in FetchAllObjects().Where(MagicalObject.IsMagicalObject)) {
            MakeVanillaObjectForSave(obj);
        }
    }

    private void MakeVanillaObjectForSave(SObject obj) {
        obj.preservedParentSheetIndex.Value = MagicalObject.ObjectId;
        obj.ParentSheetIndex = MagicalReplacement.Default.ParentSheetIndex;
    }
}