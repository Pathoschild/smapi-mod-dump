/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/myuusubi/SteamDew
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Reflection;

namespace SteamDew {

internal sealed class SteamDew : Mod {

public static string PROTOCOL_VERSION;

private static SteamDew Instance;

public static Type SMultiplayerType;
public static Type SGalaxyNetClientType;
public static Type SGalaxyNetServerType;

public static SDKs.ClientType LastClientType;

private static object TryGetConst(Type declaringType, string name) {
	FieldInfo f = declaringType.GetField(
		name,
		BindingFlags.Public | BindingFlags.Static
	);
	if (f == null) {
		return null;
	}
	return f.GetValue(null);
}

private static bool DoStartupChecks() {
	SteamDew.PROTOCOL_VERSION = "";

	object oBuildType = SteamDew.TryGetConst(typeof(StardewValley.Program), "buildType");
	object oBuildSteam = SteamDew.TryGetConst(typeof(StardewValley.Program), "build_steam");
	object oProtocol = SteamDew.TryGetConst(typeof(StardewValley.Multiplayer), "protocolVersion");

	if (oBuildType == null || oBuildSteam == null || oProtocol == null) {
		return false;
	}

	if (!(oBuildType is int) || !(oBuildSteam is int) || !(oProtocol is string)) {
		return false;
	}

	int buildType = (int) oBuildType;
	int buildSteam = (int) oBuildSteam;
	string protocol = (string) oProtocol;

	if (buildType != buildSteam) {
		SteamDew.Log("SteamDew does not work on non-Steam builds. Disabling.", LogLevel.Error);
		return false;
	}

	int checkLZ4 = 0;
	try {
		checkLZ4 = LZ4.compressBound(10419);
	} catch (Exception) {

	}
	if (checkLZ4 == 0) {
		SteamDew.Log("LZ4 could not be loaded. Disabling SteamDew.", LogLevel.Error);
		return false;
	}

	SteamDew.PROTOCOL_VERSION = protocol;

	return true;
}

public override void Entry(IModHelper helper)
{
	SteamDew.Instance = this;

	if (!SteamDew.DoStartupChecks()) {
		return;
	}

	SteamDew.Log($"Passed startup checks. SteamDew initializing with protocol: {SteamDew.PROTOCOL_VERSION}", LogLevel.Info);

	SteamDew.LastClientType = SDKs.ClientType.Unknown;

	foreach (Type t in Assembly.GetAssembly(helper.GetType()).GetTypes()) {
		string s = t.ToString();
		switch (s) {
		case "StardewModdingAPI.Framework.SMultiplayer":
			SteamDew.SMultiplayerType = t;
			break;
		case "StardewModdingAPI.Framework.Networking.SGalaxyNetClient":
			SteamDew.SGalaxyNetClientType = t;
			break;
		case "StardewModdingAPI.Framework.Networking.SGalaxyNetServer":
			SteamDew.SGalaxyNetServerType = t;
			break;
		default:
			continue;
		}
		SteamDew.Log($"Found Type: {s}");
	}

	var harmony = new Harmony(this.ModManifest.UniqueID);

	Patches.Patcher[] patchers = new Patches.Patcher[] {
		new Patches.PFarmhandMenu.Update(),
		new Patches.PGalaxySocket.UpdateLobbyPrivacy(),
		new Patches.PGameServer.Ctor(),
		new Patches.PSMultiplayer.InitClient(),
		new Patches.PSMultiplayer.InitServer(),
		new Patches.PSteamHelper.OnGalaxyStateChange()
	};

	foreach (Patches.Patcher patcher in patchers) {
		patcher.Apply(harmony);
	}

	helper.Events.GameLoop.SaveLoaded += HandleSaveLoaded;
	helper.Events.GameLoop.ReturnedToTitle += HandleReturnedToTitle;
}

private void HandleSaveLoaded(object sender, SaveLoadedEventArgs evt)
{
	string steamDewMsg = null;

	switch (SteamDew.LastClientType) {
	case SDKs.ClientType.Galaxy:
		steamDewMsg = "The server is not using SteamDew. The connection may be less stable.";
		break;
	case SDKs.ClientType.SteamDew:
		steamDewMsg = "Connected to a SteamDew server!";
		break;
	}

	if (steamDewMsg != null) {
		StardewValley.Game1.chatBox.addInfoMessage(steamDewMsg);
		SteamDew.Log(steamDewMsg, LogLevel.Info);
	}
}

private void HandleReturnedToTitle(object sender, ReturnedToTitleEventArgs evt)
{
	SteamDew.LastClientType = SDKs.ClientType.Unknown;
}

public static void Log(string s, LogLevel level = LogLevel.Trace)
{
	if (SteamDew.Instance == null) {
		return;
	}
	SteamDew.Instance.Monitor.Log(s, level);
}

public static StardewValley.Multiplayer GetGameMultiplayer() {
	FieldInfo f = typeof(StardewValley.Game1).GetField(
		"multiplayer",
		BindingFlags.NonPublic | BindingFlags.Static
	);
	if (f == null) {
		SteamDew.Log($"Failed to access StardewValley.Game1.multiplayer");
		return null;
	}

	return (StardewValley.Multiplayer) (f.GetValue(null));
}

} /* class SteamDew */

} /* namespace SteamDew */