/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using Pastel;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace SpriteMaster.Timer;

public sealed class SMTimer : Mod {
	internal static SMTimer Self { get; private set; } = default!;

	private static readonly DateTime ProcessStartTime = Process.GetCurrentProcess().StartTime.ToUniversalTime();

	private sealed class PendingTick {
		internal delegate void CallbackDelegate(in TimeSpan duration);

		internal DateTime StartTime { get; init; } = default;
		internal CallbackDelegate Callback { get; init; } = default!;
		internal int ReferenceTick { get; init; } = 0;

		internal PendingTick() {
			Self.Helper.Events.GameLoop.UpdateTicked += OnTick;
		}

		private void OnTick(object? _, UpdateTickedEventArgs args) {
			int ticks = Game1.ticks;
			if (ticks <= ReferenceTick) {
				return;
			}

			Callback(StartTime.DurationSince());
			Self.PendingTimes.Pop(Callback);

			Self.Helper.Events.GameLoop.UpdateTicked -= OnTick;
		}
	}

	private sealed class PendingTimesClass {
		private readonly HashSet<PendingTick.CallbackDelegate> Set = new();

		internal void Push(
			int referenceTick,
			PendingTick.CallbackDelegate callback,
			in DateTime? startTime = null
		) {
				lock (this) {
					if (!Set.Add(callback)) {
						return;
					}

					_ = new PendingTick {
						StartTime = startTime ?? DateTime.UtcNow,
						ReferenceTick = referenceTick,
						Callback = callback
					};
				}
		}

		internal void Pop(PendingTick.CallbackDelegate callback) {
			lock (this) {
				Set.Remove(callback);
			}
		}
	}

	private readonly PendingTimesClass PendingTimes = new();

	[UsedImplicitly]
	public SMTimer() {
		Self = this;
	}

	[UsedImplicitly]
	public override void Entry(IModHelper help) {
		PendingTimes.Push(
			startTime: ProcessStartTime,
			referenceTick: 2,
			callback: OnStart
		);
		Helper.Events.Specialized.LoadStageChanged += OnStageChanged;
	}

	private void Print(string message) => Monitor.Log(message.Pastel(Color.LightPink), level: LogLevel.Alert);

	private void OnStageChanged(object? sender, LoadStageChangedEventArgs args) {
		switch (args.NewStage) {
			case LoadStage.ReturningToTitle:
				PendingTimes.Pop(OnStart);
				break;
			case LoadStage.SaveParsed:
			case LoadStage.SaveLoadedBasicInfo:
			case LoadStage.SaveLoadedLocations:
			case LoadStage.Preloaded:
			case LoadStage.Loaded:
			case LoadStage.Ready:
				PendingTimes.Push(
					referenceTick: Game1.ticks + 32,
					callback: OnLoad
				);

				break;
		}
	}

	private void OnStart(in TimeSpan duration) {
		Print($"Start Time: {duration.FormatTotalSeconds()}");
	}

	private void OnLoad(in TimeSpan duration) {
		Print($"Load Time: {duration.FormatTotalSeconds()}");
	}
}

internal static class TimerExtensions {
	internal static string FormatTotalSeconds(this in TimeSpan span) {
		var totalSeconds = span.TotalSeconds;
		var secondsRounded = string.Format(CultureInfo.CurrentCulture, "{0:0.00} seconds", totalSeconds);

		return secondsRounded;
	}

	internal static TimeSpan DurationSince(this in DateTime start) => DateTime.UtcNow - start;
}
