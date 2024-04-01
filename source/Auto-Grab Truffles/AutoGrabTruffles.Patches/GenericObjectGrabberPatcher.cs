/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/qixing-jk/QiXingAutoGrabTruffles
**
*************************************************/

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace AutoGrabTruffles.Patches;

internal class GenericObjectGrabberPatcher
{
	private static IMonitor Monitor;

	private static ModConfig Config;

	public GenericObjectGrabberPatcher(IMonitor monitor, ModConfig config)
	{
		Monitor = monitor;
		Config = config;
	}

	public void Apply(Harmony harmony)
	{
		Type GenericObjectGrabber = AppDomain.CurrentDomain.GetAssemblies().First((Assembly a) => a.GetName().Name == "DeluxeGrabberRedux").GetTypes()
			.First((Type t) => t.Name == "GenericObjectGrabber");
		harmony.Patch(AccessTools.Method(GenericObjectGrabber, "IsGrabbable") ?? throw new InvalidOperationException("Can't find GenericObjectGrabber.IsGrabbable method."), null, new HarmonyMethod(typeof(GenericObjectGrabberPatcher), "IsGrabbable_Postfix"));
	}

	private static void IsGrabbable_Postfix(StardewValley.Object obj, ref bool __result)
	{
		if (Config.EnableAutoGrabTruffles && obj.ParentSheetIndex == 430)
		{
			__result = false;
		}
	}
}
