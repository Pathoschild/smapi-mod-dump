/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if PINTAIL

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;

using HarmonyLib;

using Leclair.Stardew.Common.Extensions;

using Nanoray.Pintail;

using StardewModdingAPI;

namespace Leclair.Stardew.Common.Events;

public abstract class PintailModSubscriber : ModSubscriber {

	#region Pin the Tail on Pintail

	private IProxyManager<string>? SMAPI_ProxyManager;

	/// <summary>
	/// Grab SMAPI's ProxyManager so we use the same Pintail instances as
	/// SMAPI's API proxying.
	/// </summary>
	public IProxyManager<string>? GetProxyManager() {
		if (SMAPI_ProxyManager is not null)
			return SMAPI_ProxyManager;

		try {
			var field = AccessTools.Field(Helper.ModRegistry.GetType(), "ProxyFactory");
			object? InterfaceProxyFactory = field.GetValue(Helper.ModRegistry);
			if (InterfaceProxyFactory is null)
				throw new ArgumentNullException(nameof(InterfaceProxyFactory));

			field = AccessTools.Field(InterfaceProxyFactory.GetType(), "ProxyManager");
			object? ProxyManager = field.GetValue(InterfaceProxyFactory);

			if (ProxyManager is IProxyManager<string> pms)
				SMAPI_ProxyManager = pms;
			else
				throw new ArgumentException(nameof(ProxyManager));

		} catch (Exception ex) {
			Log($"Unable to grab ProxyManager from SMAPI: {ex}", LogLevel.Error);
		}

		return SMAPI_ProxyManager;
	}

	public bool CanProxy<T>(Type destinationType, string destinationModId) {
		return CanProxy(typeof(T), destinationType, destinationModId);
	}

	public bool CanProxy(Type sourceType, Type destinationType, string destinationModId) {
		return CanProxy(sourceType, ModManifest.UniqueID, destinationType, destinationModId);
	}

	public bool CanProxy(Type sourceType, string sourceModId, Type destinationType, string destinationModId) {
		if (sourceModId == destinationModId)
			return false;

		var proxy = GetProxyManager();
		if (proxy is null)
			return false;

		try {
			// Try to un-proxy
			foreach (Type itype in sourceType.GetInterfacesRecursively(includingSelf: true)) {
				var unfactory = proxy.GetProxyFactory(new ProxyInfo<string>(
					target: new TypeInfo<string>(sourceModId, destinationType),
					proxy: new TypeInfo<string>(destinationModId, itype)
				));

				if (unfactory is null)
					continue;

				return true;
			}

			proxy.ObtainProxyFactory(new ProxyInfo<string>(
				target: new TypeInfo<string>(sourceModId, sourceType),
				proxy: new TypeInfo<string>(destinationModId, destinationType)
			));

			return true;
		} catch {
			return false;
		}
	}

	public bool TryProxyRemote<T>(object? sourceInstance, string sourceModId, [NotNullWhen(true)] out T? destinationInstance, bool silent = false, Type? sourceType = null) {
		if (TryProxy(sourceInstance, sourceModId, typeof(T), ModManifest.UniqueID, out object? obj, silent: silent, sourceType: sourceType) && obj is T tobj) {
			destinationInstance = tobj;
			return true;
		}

		destinationInstance = default;
		return false;
	}

	public bool TryUnproxy(object? sourceInstance, [NotNullWhen(true)] out object? unproxiedInstance, bool silent = false, Type? sourceType = null) {
		var proxy = GetProxyManager();
		if (proxy is null || sourceInstance is null) {
			unproxiedInstance = null;
			return false;
		}

		try {
			sourceType ??= sourceInstance.GetType();

			// Short circuit Pintail proxies if we can.
			if (sourceType.GetField("__Target", BindingFlags.Instance | BindingFlags.NonPublic) is FieldInfo field) {
				unproxiedInstance = field.GetValue(sourceInstance);
				if (TryUnproxy(unproxiedInstance, out object? moreUnproxied, silent))
					unproxiedInstance = moreUnproxied;

				return unproxiedInstance is not null;
			}

		} catch (Exception ex) {
			if (!silent)
				Log($"Unable to un-proxy type {sourceInstance.GetType()}: {ex}", LogLevel.Debug);
		}

		unproxiedInstance = null;
		return false;
	}

	public bool TryGetProxyFactory(Type sourceType, string sourceModId, Type destinationType, string destinationModId, [NotNullWhen(true)] out IProxyFactory<string>? factory, bool silent = false) {
		var proxy = GetProxyManager();
		if (sourceModId == destinationModId || proxy is null || sourceType is null || destinationType is null) {
			factory = null;
			return false;
		}

		try {
			factory = proxy.ObtainProxyFactory(new ProxyInfo<string>(
				target: new TypeInfo<string>(sourceModId, sourceType),
				proxy: new TypeInfo<string>(destinationModId, destinationType)
			));

			return true;

		} catch (Exception ex) {
			if (!silent)
				Log($"Unable to proxy type {sourceType} to {destinationType}: {ex}", LogLevel.Debug);

			factory = null;
			return false;
		}
	}

	public bool TryProxy(object? sourceInstance, string sourceModId, Type destinationType, string destinationModId, [NotNullWhen(true)] out object? destinationInstance, bool silent = false, Type? sourceType = null) {
		var proxy = GetProxyManager();
		if (sourceModId == destinationModId || proxy is null || sourceInstance is null) {
			destinationInstance = null;
			return false;
		}

		try {
			sourceType ??= sourceInstance.GetType();

			// Short circuit Pintail proxies if we can.
			if (sourceType.GetField("__Target", BindingFlags.Instance | BindingFlags.NonPublic) is FieldInfo field && destinationType.IsAssignableFrom(field.FieldType)) {
				destinationInstance = field.GetValue(sourceInstance);
				if (destinationInstance is not null)
					return true;
			}

			// Try to un-proxy
			foreach (Type itype in sourceType.GetInterfacesRecursively(includingSelf: true)) {
				var unfactory = proxy.GetProxyFactory(new ProxyInfo<string>(
					target: new TypeInfo<string>(sourceModId, destinationType),
					proxy: new TypeInfo<string>(destinationModId, itype)
				));

				if (unfactory is null)
					continue;

				if (unfactory.TryUnproxy(proxy, sourceInstance, out object? unproxied)) {
					destinationInstance = unproxied;
					return true;
				}
			}

			var factory = proxy.ObtainProxyFactory(new ProxyInfo<string>(
				target: new TypeInfo<string>(sourceModId, sourceType),
				proxy: new TypeInfo<string>(destinationModId, destinationType)
			));

			destinationInstance = factory.ObtainProxy(proxy, sourceInstance);
			return true;

		} catch (Exception ex) {
			if (!silent)
				Log($"Unable to proxy type {sourceInstance.GetType()} to {destinationType}: {ex}", LogLevel.Debug);
			destinationInstance = null;
			return false;
		}
	}

	#endregion

}


#endif
