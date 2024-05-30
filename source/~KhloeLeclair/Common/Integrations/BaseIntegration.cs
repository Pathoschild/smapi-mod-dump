/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;

using StardewModdingAPI;

namespace Leclair.Stardew.Common.Integrations;

public abstract class BaseIntegration<M> where M : Mod {

	protected M Self { get; }

	public string ModID { get; }
	public string? MinVersion { get; }
	public string? MaxVersion { get; }

	[MemberNotNullWhen(true, nameof(Manifest))]
	public virtual bool IsLoaded { get; protected set; }

	public IModInfo? Other { get; }
	public IManifest? Manifest { get; }

	public BaseIntegration(M self, string modID, string? minVersion, string? maxVersion = null) {
		Self = self;
		ModID = modID;
		MinVersion = minVersion;
		MaxVersion = maxVersion;

		// Look up the mod.
		Other = Self.Helper.ModRegistry.Get(ModID);
		Manifest = Other?.Manifest;
		if (Manifest == null)
			return;

		if (!IsVersionOkay) {
			Log($"Unsupported version {Manifest.Version} is present. Disabling integration.", LogLevel.Warn);
			return;
		}

		IsLoaded = true;
	}


	protected virtual void Log(string message, LogLevel level = LogLevel.Debug, Exception? ex = null, LogLevel? exLevel = null) {
		string name = Manifest?.Name ?? ModID;
		Self.Monitor.Log($"[IG:{name}] {message}", level);
		if (ex != null)
			Self.Monitor.Log($"[IG:{name}] Details:\n{ex}", level: exLevel ?? level);
	}

	[MemberNotNull(nameof(Manifest))]
	protected virtual void AssertLoaded() {
		if (!IsLoaded || Manifest is null)
			throw new InvalidOperationException($"{ModID} integration is disabled.");
	}

	protected virtual bool IsVersionOkay {
		get {
			if (Manifest == null)
				return false;

			if (!string.IsNullOrEmpty(MinVersion) && Manifest.Version.IsOlderThan(MinVersion))
				return false;

			if (!string.IsNullOrEmpty(MaxVersion) && Manifest.Version.IsNewerThan(MaxVersion))
				return false;

			return true;
		}
	}
}
