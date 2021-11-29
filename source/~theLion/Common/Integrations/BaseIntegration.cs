/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace TheLion.Stardew.Common.Integrations
{
	/// <summary>The base implementation for a mod integration.</summary>
	/// <remarks>Credit to <c>Pathoschild</c>.</remarks>
	internal abstract class BaseIntegration : IModIntegration
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="label">A human-readable name for the mod.</param>
		/// <param name="modID">The mod's unique ID.</param>
		/// <param name="minVersion">The minimum version of the mod that's supported.</param>
		/// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
		/// <param name="log">Encapsulates monitoring and logging.</param>
		protected BaseIntegration(string label, string modID, string minVersion, IModRegistry modRegistry,
			Action<string, LogLevel> log)
		{
			// init
			Label = label;
			ModID = modID;
			ModRegistry = modRegistry;
			Log = log;

			// validate mod
			var manifest = modRegistry.Get(ModID)?.Manifest;
			if (manifest is null) return;

			if (manifest.Version.IsOlderThan(minVersion))
			{
				Log(
					$"Detected {label} {manifest.Version}, but need {minVersion} or later. Disabled integration with this mod.",
					LogLevel.Warn);
				return;
			}

			IsLoaded = true;
		}

		/// <summary>The mod's unique ID.</summary>
		protected string ModID { get; }

		/// <summary>API for fetching metadata about loaded mods.</summary>
		protected IModRegistry ModRegistry { get; }

		/// <summary>Encapsulates monitoring and logging.</summary>
		protected Action<string, LogLevel> Log { get; }

		/// <summary>A human-readable name for the mod.</summary>
		public string Label { get; }

		/// <summary>Whether the mod is available.</summary>
		public bool IsLoaded { get; protected set; }

		/// <summary>Get an API for the mod, and show a message if it can't be loaded.</summary>
		/// <typeparam name="TInterface">The API type.</typeparam>
		protected TInterface GetValidatedApi<TInterface>() where TInterface : class
		{
			var api = ModRegistry.GetApi<TInterface>(ModID);
			if (api is not null) return api;

			Log($"Detected {Label}, but couldn't fetch its API. Disabled integration with this mod.",
				LogLevel.Warn);
			return null;
		}

		/// <summary>Assert that the integration is loaded.</summary>
		/// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
		protected void AssertLoaded()
		{
			if (!IsLoaded) throw new InvalidOperationException($"The {Label} integration isn't loaded.");
		}
	}
}