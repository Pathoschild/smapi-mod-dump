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
using StardewModdingAPI.Events;

namespace ChallengerTest.Common; 

public class TestModHelper : IModHelper {

    private Dictionary<string, dynamic> _config = new();
    
    public TestModHelper(string directoryPath = ".") {
        DirectoryPath = directoryPath;
    }
    
    public string DirectoryPath { get; }
    public IModEvents Events => TestEvents;
    public TestModEvents TestEvents { get; } = new();
    public ICommandHelper ConsoleCommands { get; }
    public IGameContentHelper GameContent { get; }
    public IModContentHelper ModContent { get; }
    public IContentHelper Content { get; }
    public IContentPackHelper ContentPacks { get; }
    public IDataHelper Data => TestData;
    public TestDataHelper TestData { get; } = new();
    public IInputHelper Input => TestInput;
    public TestInputHelper TestInput { get; } = new();
    public IReflectionHelper Reflection { get; }
    public IModRegistry ModRegistry { get; }
    public IMultiplayerHelper Multiplayer { get; }
    public ITranslationHelper Translation { get; }

    public void ClearConfig<TConfig>() where TConfig : class, new() {
        _config.Remove(GetKey<TConfig>());
    }

    private static string GetKey<TConfig>() => typeof(TConfig).FullName ?? "unkown";

    public TConfig ReadConfig<TConfig>() where TConfig : class, new() {
        return (TConfig?) _config.GetValueOrDefault(GetKey<TConfig>()) ?? new TConfig();
    }

    public void WriteConfig<TConfig>(TConfig config) where TConfig : class, new() {
        _config[GetKey<TConfig>()] = config;
    }
}