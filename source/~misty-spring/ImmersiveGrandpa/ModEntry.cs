/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;

namespace ImmersiveGrandpa;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.GameLaunched += OnGameLaunch;
        helper.Events.Specialized.LoadStageChanged += LoadStageChange;
    }

    private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
    {
        var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            
        api.RegisterToken(this.ModManifest, "SkinTone", () =>
        {
            /*created save
            if (CreatedSave == true)
                return new[] { $"{Game1.MasterPlayer.skinColor}" };
            if (Game1.MasterPlayer.skin.Value is not 0)
                return new[] { $"{Game1.MasterPlayer.skin.Value}" };*/

            if (Game1.player.skin.Value is not 0)
                return new[] { $"{Game1.player.skin.Value}" };

            // save is loaded
            if (Context.IsWorldReady)
                return new[] { $"{Game1.player.skin.Value}" };

            // no save loaded (e.g. on the title screen)
            return new[] { "vanilla" };

        });
    }
    private void LoadStageChange(object sender, LoadStageChangedEventArgs e)
    {
        if (e.NewStage.Equals(LoadStage.CreatedBasicInfo) || e.NewStage.HasFlag(LoadStage.CreatedBasicInfo))
        {
            this.Monitor.Log("Created basic info");
            Helper.GameContent.InvalidateCache("Minigames/jojacorps");

            this.Monitor.Log($"Skin value: {Game1.player.skin.Value}");
        }
    }
}