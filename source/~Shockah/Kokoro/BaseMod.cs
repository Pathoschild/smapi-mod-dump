/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shockah.Kokoro.SMAPI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.Kokoro;

public abstract class BaseMod : Mod
{
	private Dictionary<string, Action<ModMessageReceivedEventArgs>> ModMessageHandlers { get; init; } = new();

	public override object? GetApi()
		=> this;

	public override void Entry(IModHelper helper)
	{
		helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
	}

	private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
	{
		if (e.FromModID != ModManifest.UniqueID)
			return;
		if (!ModMessageHandlers.TryGetValue(e.Type, out var handler))
		{
			Monitor.Log($"Received a `{e.Type}` mod message, but there is no handler registered for it.", LogLevel.Error);
			return;
		}
		handler(e);
	}

	public void RegisterModMessageHandler<T>(Action<Farmer, T> handler) where T : notnull
	{
		var key = typeof(T).GetBestName();
		ModMessageHandlers[key] = e => handler(e.GetPlayer(), e.ReadAs<T>());
	}

	public void RegisterModMessageHandler<T>(Action<T> handler) where T : notnull
		=> RegisterModMessageHandler<T>((_, message) => handler(message));

	public void SendModMessageToEveryone<T>(T message) where T : notnull
		=> SendModMessage(message, Game1.getAllFarmers().Where(p => p != Game1.player));

	public void SendModMessage<T>(T message, Farmer recipient) where T : notnull
		=> SendModMessage(message, new Farmer[] { recipient });

	public void SendModMessage<T>(T message, IEnumerable<Farmer> recipients) where T : notnull
	{
		var key = typeof(T).GetBestName();
		if (!ModMessageHandlers.ContainsKey(key))
			throw new InvalidOperationException($"Tried to send a `{key}` mod message, but there is no handler registered for it.");
		Helper.Multiplayer.SendMessage(message, key, new[] { ModManifest.UniqueID }, recipients.Select(p => p.UniqueMultiplayerID).ToArray());
	}

	public void SendModMessage<T>(T message, IMultiplayerPeer recipient) where T : notnull
		=> SendModMessage(message, new IMultiplayerPeer[] { recipient });

	public void SendModMessage<T>(T message, IEnumerable<IMultiplayerPeer> recipients) where T : notnull
	{
		var key = typeof(T).GetBestName();
		if (!ModMessageHandlers.ContainsKey(key))
			throw new InvalidOperationException($"Tried to send a `{key}` mod message, but there is no handler registered for it.");
		Helper.Multiplayer.SendMessage(message, key, new[] { ModManifest.UniqueID }, recipients.Select(p => p.PlayerID).ToArray());
	}
}

public interface IVersioned
{
	ISemanticVersion? Version { get; }

	public interface Modifiable : IVersioned
	{
		new ISemanticVersion? Version { get; set; }

		ISemanticVersion? IVersioned.Version
			=> Version;
	}
}

public enum BaseModConfigMigrationMode
{
	BeforeOnEntry,
	AfterOnEntry,
	OnGameLaunched
}

public abstract class BaseMod<TConfig> : BaseMod
	where TConfig : class, new()
{
	public TConfig Config
	{
		get => ConfigStorage!;
		set
		{
			ConfigStorage = value;
			LogConfig();
		}
	}

	private TConfig? ConfigStorage;
	private JsonSerializerSettings? JsonSerializerSettings;

	protected internal virtual BaseModConfigMigrationMode ConfigMigrationMode
		=> BaseModConfigMigrationMode.BeforeOnEntry;

	public sealed override void Entry(IModHelper helper)
	{
		bool configExisted = helper.Data.ReadJsonFile<JObject>("config.json") is not null;
		Config = ReadConfig(helper);
		var versionedConfig = Config as IVersioned.Modifiable; // we don't need it to be modifiable here, but we don't want to migrate if it's not
		bool isMigrationRequired = configExisted && versionedConfig is not null && (versionedConfig.Version is null || versionedConfig.Version.IsOlderThan(ModManifest.Version));

		if (!configExisted && versionedConfig is not null)
			WriteConfig(); // updating new config version on disk

		if (isMigrationRequired)
		{
			if (ConfigMigrationMode == BaseModConfigMigrationMode.BeforeOnEntry)
				InternalMigrateConfig(configVersion: versionedConfig!.Version, modVersion: ModManifest.Version, helper: helper);
			else if (ConfigMigrationMode == BaseModConfigMigrationMode.OnGameLaunched)
				helper.Events.GameLoop.GameLaunched += MigrateConfigOnGameLaunched;
		}

		base.Entry(helper);
		OnEntry(helper);

		if (isMigrationRequired && ConfigMigrationMode == BaseModConfigMigrationMode.AfterOnEntry)
			InternalMigrateConfig(configVersion: versionedConfig!.Version, modVersion: ModManifest.Version, helper: helper);
	}

	public abstract void OnEntry(IModHelper helper);

	private void MigrateConfigOnGameLaunched(object? sender, GameLaunchedEventArgs e)
	{
		if (Config is not IVersioned.Modifiable versionedConfig)
			return;
		InternalMigrateConfig(configVersion: versionedConfig.Version, modVersion: ModManifest.Version);
	}

	private void InternalMigrateConfig(ISemanticVersion? configVersion, ISemanticVersion modVersion, IModHelper? helper = null)
	{
		Monitor.Log($"Migrating config from {(configVersion is null ? "unknown version" : $"{configVersion}")} to {modVersion}...", LogLevel.Debug);
		MigrateConfig(configVersion: configVersion, modVersion: modVersion);
		WriteConfig(helper: helper);
	}

	public virtual void MigrateConfig(ISemanticVersion? configVersion, ISemanticVersion modVersion)
	{
#if DEBUG
		throw new InvalidOperationException($"Unhandled config migration from {(configVersion is null ? "unknown version" : $"{configVersion}")} to {modVersion}.");
#else
		Monitor.Log($"Unhandled config migration from {(configVersion is null ? "unknown version" : $"{configVersion}")} to {modVersion}. Please report this to the developer.", LogLevel.Error);
#endif
	}

	private TConfig ReadConfig(IModHelper? helper = null)
	{
		helper ??= Helper;
		var config = helper.ReadConfig<TConfig>();
		return config;
	}

	protected internal void WriteConfig(TConfig? @override = null, IModHelper? helper = null)
	{
		helper ??= Helper;
		var config = @override ?? Config;
		if (config is IVersioned.Modifiable versionedConfig)
			versionedConfig.Version = ModManifest.Version;
		helper.WriteConfig(@override ?? Config);
	}

	protected internal void LogConfig(TConfig? @override = null)
	{
		JsonSerializerSettings ??= JsonSerializerExt.GetSMAPISerializerSettings(Helper.Data);

		var json = JsonConvert.SerializeObject(@override ?? Config, JsonSerializerSettings);
		Monitor.Log($"Current config:\n{json}", LogLevel.Trace);
	}
}