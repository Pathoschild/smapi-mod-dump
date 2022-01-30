/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using DialogueExtension.Api;
using DialogueExtension.Patches;
using DialogueExtension.Patches.Utility;
using DialogueExtension.Utilities;
using LightInject;
using SDV.Shared.Abstractions.Utility;
using StardewModdingAPI;
using LogLevel = StardewModdingAPI.LogLevel;

namespace DialogueExtension
{
  public class Initialization
  {
    public Initialization(IMod mod)
    {
      var container = new ServiceContainer();
      container.RegisterAssembly(GetType().Assembly);
      container.RegisterInstance(mod.Helper);
      container.RegisterInstance(mod.Monitor);
      container.Register<IHarmonyWrapper, HarmonyWrapper>();
      container.Register<IWrapperFactory, WrapperFactory>();
      container.Decorate<IMonitor, Logger>();
      container.RegisterInstance<IServiceFactory>(container.BeginScope());

      foreach (var service in container.AvailableServices) 
        mod.Monitor.Log(service.ServiceType.FullName + " | " + container.CanGetInstance(service.ServiceType, String.Empty), LogLevel.Debug);
      
      container.GetInstance<IHarmonyPatch>();
      container.GetInstance<IDialogueApi>();
     
    }
  }
}
