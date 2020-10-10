/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drynwynn/StardewValleyMods
**
*************************************************/

#if INCLUDEHARMONY
using Harmony;
#endif
using DsStardewLib.Config;
using StardewModdingAPI;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

namespace DsStardewLib.Utils
{
  /// <summary>
  /// Wrap loading of Harmony into a single class.  Create an instance of this wrapper in the mod entry
  /// and then it will take care of loading all hacks that implement HarmonyHack when init is called.
  /// Also load Harmony as a reference and set a Build Conditional of 'INCLUDEHARMONY'.
  /// </summary>
  class HarmonyWrapper
  {
#if INCLUDEHARMONY
    private HarmonyInstance hInstance;
    public HarmonyInstance HarmonyInst { get => hInstance; private set => hInstance = value; }
#endif

    /// <summary>
    /// Call in the mod entry to load Harmony for the assembly.  The config and log parameters are assigned to the
    /// hack instance.
    /// </summary>
    /// <param name="helper">Helper provided by SMAPI</param>
    /// <param name="config">The HarmonyConfig used to configure Harmony loading</param>
    /// <param name="log">If provided, logging will be enabled for load and the hack.</param>
    /// <param name="msg">What piece is being patched.  Class#method is a good idea.</param>
    public void InitHarmony(IModHelper helper, HarmonyConfig config, Logger log)
    {
      if (config == null || log == null) {
        log.Error("Must provide a configuration and a logger to load Harmony");
        return;
      }

      log.Trace("Checking if Harmony is configured to be initialized");
#if INCLUDEHARMONY
      // Only run this function once.
      if (hInstance != null) {
        log?.Warn("InitHarmony called more than once.");
        return;
      }

      if (config.HarmonyLoad) {
        log.Silly("Enumerating through classes to find harmony hacks and set the log and config");
        foreach (var t in GetHarmonyHacks(Assembly.GetExecutingAssembly())) {
          // I don't know if Harmony enforces order, but just in case not, try to make logical sense
          string className = "", methodName = "", typesArray = "";
          foreach (var a in t.GetCustomAttributes(typeof(HarmonyPatch))) {
            HarmonyPatch hp = a as HarmonyPatch;
            if (hp.info?.originalType != null) className = hp.info.originalType.ToString();
            if (!string.IsNullOrWhiteSpace(hp.info?.methodName)) methodName = hp.info.methodName;
            if (hp.info?.parameter != null) { typesArray = string.Join<Type>(",", hp.info.parameter); }
          }
          log.Debug($"Using Harmony to patch IL for {className}#{methodName}({typesArray}).  Any issues, check this mod first");

          HarmonyHack hh = (HarmonyHack)Activator.CreateInstance(t);
          hh.Log = log;
          hh.Config = config;
        }
        
        // Only one patch file so have it patch everything instead of doing manual thing.
        HarmonyInstance.DEBUG = config.HarmonyDebug;
        HarmonyInst = HarmonyInstance.Create(helper.ModRegistry.ModID);
        HarmonyInst.PatchAll(Assembly.GetExecutingAssembly());
      }
#endif
    }

    /// <summary>
    /// Return any classes that implement HarmonyHack.
    /// Pulled directly from SO: https://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface
    /// </summary>
    /// <param name="asm"></param>
    /// <returns></returns>
    private IEnumerable<Type> GetHarmonyHacks(Assembly asm)
    {
      var it = typeof(HarmonyHack);
      return asm.GetLoadableTypes().Where(it.IsAssignableFrom).Where(t => !(t.Equals(it))).ToList();
    }
  }

  /// <summary>
  /// Extend the Assembly loader to get all types from an assembly.
  /// Pulled directoy from SO; https://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface
  /// </summary>
  public static class TypeLoaderExtensions
  {
    public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
    {
      if (assembly == null) throw new ArgumentNullException("assembly");
      try {
        return assembly.GetTypes();
      }
      catch (ReflectionTypeLoadException e) {
        return e.Types.Where(t => t != null);
      }
    }
  }
}
