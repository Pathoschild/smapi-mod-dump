/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

// Decompiled with JetBrains decompiler
// Type: BetterFarmAnimalVariety.ModEntry
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System;
using System.Collections.Generic;
using BetterFarmAnimalVariety.Framework.Events;
using BetterFarmAnimalVariety.Framework.Exceptions;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BetterFarmAnimalVariety
{
  public class ModEntry : Mod
  {
    public static ModEntry Instance;

    public override void Entry(IModHelper helper)
    {
      Instance = this;
      try
      {
        RefreshCache.SeedCacheWithVanillaFarmAnimals();
        LoadMod.OnEntry(this);
      }
      catch (Exception ex)
      {
        Monitor.Log(ex.Message, LogLevel.Error);
        return;
      }

      Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
      Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
      Helper.Events.GameLoop.Saving += OnSaving;
      Helper.Events.GameLoop.Saved += OnSaved;
    }

    public override object GetApi()
    {
      return new Framework.Api.BetterFarmAnimalVariety(Framework.Helpers.Mod
        .ReadConfig<ModConfig>());
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
      LoadContentPacks.SetUpContentPacks(Helper.ContentPacks.GetOwned(), Monitor);
      RefreshCache.ValidateCachedFarmAnimals(Helper, Monitor);
      try
      {
        IntegrateWithMoreAnimals.RegisterAnimals(Helper, Monitor);
      }
      catch (ApiNotFoundException ex)
      {
        Monitor.Log("Cannot register animals with More Animals: " + ex.Message);
      }
    }

    private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
      try
      {
        IntegrateWithJsonAssets.RefreshFarmAnimalData(Helper);
      }
      catch (ApiNotFoundException ex)
      {
        Monitor.Log("Cannot refresh farm animal data: " + ex.Message);
      }
      catch (SaveNotLoadedException ex)
      {
        Monitor.Log("Cannot refresh farm animal data: " + ex.Message);
      }

      try
      {
        ConvertDirtyFarmAnimals.OnSaveLoaded(e);
      }
      catch (KeyNotFoundException ex)
      {
        HandleKeyNotFoundException(ex);
      }
    }

    private void OnSaving(object sender, SavingEventArgs e)
    {
      try
      {
        ConvertDirtyFarmAnimals.OnSaving(e);
      }
      catch (KeyNotFoundException ex)
      {
        HandleKeyNotFoundException(ex);
      }
    }

    private void OnSaved(object sender, SavedEventArgs e)
    {
      try
      {
        ConvertDirtyFarmAnimals.OnSaved(e);
      }
      catch (KeyNotFoundException ex)
      {
        HandleKeyNotFoundException(ex);
      }
    }

    private void HandleKeyNotFoundException(KeyNotFoundException exception)
    {
      Monitor.Log(exception.Message, LogLevel.Error);
      throw exception;
    }
  }
}