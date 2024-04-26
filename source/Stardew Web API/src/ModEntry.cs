/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewWebApi.Game;
using StardewWebApi.Game.Events;
using StardewWebApi.Server;

namespace StardewWebApi;

public class ModEntry : Mod
{
    private EventManager? _eventManager;

    public override void Entry(IModHelper helper)
    {
        SMAPIWrapper.Instance.Initialize(Monitor, helper);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        SMAPIWrapper.LogDebug("Starting web server");
        WebServer.Instance.StartWebServer();

        _eventManager = new EventManager();
    }
}