/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UIInfoSuite2
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace UIInfoSuite2.Compatibility;

/// <summary>Entrypoint for all things DGA</summary>
public class DynamicGameAssetsEntry : IDisposable
{
  private const string MOD_ID = "spacechase0.DynamicGameAssets";
  private DynamicGameAssetsHelper? _dgaHelper;

  public DynamicGameAssetsEntry(IModHelper helper, IMonitor monitor)
  {
    Helper = helper;
    Monitor = monitor;

    Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
  }

  private IModHelper Helper { get; }
  private IMonitor Monitor { get; }

  public IDynamicGameAssetsApi? Api { get; private set; }

  public bool IsLoaded { get; private set; }

  public void Dispose()
  {
    Helper.Events.GameLoop.GameLaunched -= OnGameLaunched;
  }

  private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
  {
    // Check if DGA is loaded
    if (Helper.ModRegistry.IsLoaded(MOD_ID))
    {
      IsLoaded = true;

      // Get DGA's API
      var api = Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>(MOD_ID);
      if (api != null)
      {
        Api = api;
        _dgaHelper = new DynamicGameAssetsHelper(Api, Helper, Monitor);
      }
    }
  }

  /// <summary>
  ///   Check if <paramref name="obj" /> is a DGA CustomCrop and provide a <see cref="DynamicGameAssetsHelper" />
  /// </summary>
  public bool IsCustomCrop(object obj, [NotNullWhen(true)] out DynamicGameAssetsHelper? dgaHelper)
  {
    dgaHelper = null;
    if (IsLoaded && obj.GetType().FullName == "DynamicGameAssets.Game.CustomCrop")
    {
      dgaHelper = _dgaHelper?.InjectDga(obj);
    }

    return dgaHelper != null;
  }


  /// <summary>
  ///   Check if <paramref name="obj" /> is a DGA CustomObject and provide a <see cref="DynamicGameAssetsHelper" />
  /// </summary>
  public bool IsCustomObject(object obj, [NotNullWhen(true)] out DynamicGameAssetsHelper? dgaHelper)
  {
    dgaHelper = null;
    if (IsLoaded && obj.GetType().FullName == "DynamicGameAssets.Game.CustomObject")
    {
      dgaHelper = _dgaHelper?.InjectDga(obj);
    }

    return dgaHelper != null;
  }
}
