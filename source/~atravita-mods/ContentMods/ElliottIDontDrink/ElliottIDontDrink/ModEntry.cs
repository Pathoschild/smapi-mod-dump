/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace ElliottIDontDrink;

internal class ModEntry : Mod
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Gets the monitor for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private int countdown = 5;
    
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        => this.Helper.Events.GameLoop.UpdateTicked += this.FiveTicksPostGameLaunched;

    private void FiveTicksPostGameLaunched(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
    {
        if (--this.countdown < 0)
        {
            this.Helper.Content.AssetEditors.Add(AssetEditor.Instance);
            this.Helper.Events.GameLoop.UpdateTicked -= this.FiveTicksPostGameLaunched;
        }
    }
}

internal class AssetEditor : IAssetEditor
{

    private AssetEditor()
    {
    }

    internal static AssetEditor Instance { get; } = new();

    public bool CanEdit<T>(IAssetInfo asset) => asset.AssetNameEquals("Data/Events/Saloon");
    public void Edit<T>(IAssetData asset)
    {
        IAssetDataForDictionary<string, string>? editor = asset.AsDictionary<string, string>();
        foreach ((string key, string value) in editor.Data)
        {
            if(key.StartsWith("40", StringComparison.OrdinalIgnoreCase))
            {
                int index = value.IndexOf("speak Elliott");
                int second = value.IndexOf("speak Elliott", index+1);
                int nextslash = value.IndexOf('/', second);
                if (nextslash > -1)
                {
                    string initial = value[..nextslash] + $"/question fork1 \"#{I18n.Drink()}#{I18n.Nondrink()}/fork atravita_elliott_nodrink/";
                    string remainder = value[(nextslash + 1)..];

                    editor.Data["atravita_elliott_nodrink"] = remainder.Replace("346", "253");
                    editor.Data[key] = initial + remainder;
                }
                return;
            }
        }
    }
}