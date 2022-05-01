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

public abstract class BaseAPIIntegration<T, M> : BaseIntegration<M> where M : Mod where T : class {

	protected T? API { get; }

	protected BaseAPIIntegration(M self, string modID, string? minVersion, string? maxVersion = null) : base(self, modID, minVersion, maxVersion) {
		if (!IsLoaded) {
			API = null;
			return;
		}

		API = GetAPI();

		if (API == null) {
			Log("Unable to obtain API instance. Disabling integration.", LogLevel.Warn);

			IsLoaded = false;
			return;
		}
	}

	[MemberNotNullWhen(true, nameof(API))]
	public override bool IsLoaded { get; protected set; }

	[MemberNotNull(nameof(API))]
	protected override void AssertLoaded() {
		base.AssertLoaded();
		if (API is null)
			throw new InvalidOperationException($"{ModID} integration is disabled.");
	}

	private T? GetAPI() {
		try {
			return Self.Helper.ModRegistry.GetApi<T>(ModID);
		} catch (Exception ex) {
			Log("An error occurred calling GetApi.", LogLevel.Debug, ex);

			return null;
		}
	}
}
