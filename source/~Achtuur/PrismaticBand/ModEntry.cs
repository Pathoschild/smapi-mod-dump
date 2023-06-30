/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace PrismaticBand;

public class ModEntry : Mod
{
    internal static ModEntry Instance;
    internal ModConfig Config;

    public override void Entry(IModHelper helper)
    {

        I18n.Init(helper.Translation);
        ModEntry.Instance = this;

        // HarmonyPatcher.ApplyPatches(this,

        // );

        this.Config = this.Helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
    }

    private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
    {
        this.Config.createMenu();
    }
}
