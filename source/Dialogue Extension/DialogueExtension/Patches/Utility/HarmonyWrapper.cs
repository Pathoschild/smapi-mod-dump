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
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace DialogueExtension.Patches.Utility
{
  public interface IHarmonyWrapper
  {
    void Create(string id);
    void PatchAll();
    void PatchAll(Assembly assembly);
    void Patch(MethodBase original, HarmonyMethod prefix = null,
      HarmonyMethod postfix = null, HarmonyMethod transpiler = null);
    void UnpatchAll(string harmonyId = null);
    void Unpatch(MethodBase original, HarmonyPatchType type, string harmonyId = null);
    void Unpatch(MethodBase original, MethodInfo patch);
    bool HasAnyPatches(string harmonyId);
    HarmonyLib.Patches GetPatchInfo(MethodBase method);
    IEnumerable<MethodBase> GetPatchedMethods();
    Dictionary<string, Version> VersionInfo(out Version currentVersion);
  }
  public class HarmonyWrapper : IHarmonyWrapper
  {
    private Harmony _harmonyInstance;

    public void Create(string id) => _harmonyInstance = new Harmony(id);

    public void PatchAll() =>
    _harmonyInstance.PatchAll();

    public void PatchAll(Assembly assembly) =>
      _harmonyInstance.PatchAll(assembly);

    public void Patch(MethodBase original, HarmonyMethod prefix = null,
      HarmonyMethod postfix = null, HarmonyMethod transpiler = null) =>
      _harmonyInstance.Patch(original, prefix, postfix, transpiler);

    public void UnpatchAll(string harmonyId = null) =>
      _harmonyInstance.UnpatchAll(harmonyId);

    public void Unpatch(MethodBase original, HarmonyPatchType type, string harmonyId = null) =>
      _harmonyInstance.Unpatch(original, type, harmonyId);

    public void Unpatch(MethodBase original, MethodInfo patch) =>
      _harmonyInstance.Unpatch(original, patch);

    public bool HasAnyPatches(string harmonyId) =>
      Harmony.HasAnyPatches(harmonyId);

    public HarmonyLib.Patches GetPatchInfo(MethodBase method) =>
      Harmony.GetPatchInfo(method);

    public IEnumerable<MethodBase> GetPatchedMethods() =>
      _harmonyInstance.GetPatchedMethods();

    public Dictionary<string, Version> VersionInfo(out Version currentVersion) =>
      Harmony.VersionInfo(out currentVersion);
  }
}
