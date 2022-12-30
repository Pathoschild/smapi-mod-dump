/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Extensions;

using Leclair.Stardew.ThemeManagerFontStudio.Models;
using Leclair.Stardew.ThemeManagerFontStudio.Sources;

using StardewModdingAPI.Events;

namespace Leclair.Stardew.ThemeManagerFontStudio.Managers;

internal record struct LoadingProgress(
	int finishedSources,
	int totalSources,
	int finishedFonts,
	int totalFonts
);

public class ProgressEventArgs : EventArgs {

	public int FinishedSources { get; }
	public int TotalSources { get; }
	public int TotalFonts { get; }
	public int FinishedFonts { get; }

	internal ProgressEventArgs(LoadingProgress source) {
		FinishedSources = source.finishedSources;
		TotalSources = source.totalSources;
		TotalFonts = source.totalFonts;
		FinishedFonts = source.finishedFonts;
	}
}


public class SourceManager : BaseManager {

	public readonly Dictionary<string, IFontSource> FontSources = new(StringComparer.OrdinalIgnoreCase);

	private Dictionary<string, Dictionary<string, IFontData>>? FontData;

	private readonly Dictionary<IFontData, List<Action<IFontData>>> IndividualLoadingTasks = new();

	private readonly List<(Task, Action)> PendingLoadingTasks = new();

	private Task<Dictionary<string, Dictionary<string, IFontData>>>? LoadingTask;

	public SourceManager(ModEntry mod) : base(mod) {
		AddSource("google", new GoogleFontSource(Mod));
		AddSource("local", new LocalFontSource());
	}

	#region Events

	[Subscriber]
	private void OnGameTicked(object? sender, UpdateTickedEventArgs e) {
		if (IsLoading)
			CheckDone();

		int count = PendingLoadingTasks.Count;
		while(count-- > 0) {
			var pair = PendingLoadingTasks[count];
			if (pair.Item1.IsCompleted) {
				PendingLoadingTasks.RemoveAt(count);
				pair.Item2();
			}
		}
	}

	#endregion

	#region Sources

	public bool AddSource(string key, IFontSource source) {
		lock((FontSources as ICollection).SyncRoot) {
			return FontSources.TryAdd(key, source);
		}
	}

	#endregion

	#region Status

	[MemberNotNullWhen(true, nameof(FontData))]
	public bool IsLoaded => FontData is not null;

	[MemberNotNullWhen(true, nameof(LoadingTask))]
	public bool IsLoading => LoadingTask is not null;

	public event EventHandler? LoadingComplete;
	public event EventHandler<ProgressEventArgs>? LoadingProgress;

	#endregion

	#region Single Font Loading

	public void LoadFont(IFontData data, Action<IFontData> callback) {
		if (data is null || data.Source is null || data.IsLoaded)
			return;

		if (IndividualLoadingTasks.TryGetValue(data, out var existing)) {
			if (callback is not null)
				existing.Add(callback);
			return;
		}

		if (!FontSources.TryGetValue(data.Source, out var source))
			return;

		List<Action<IFontData>> callbacks = new();
		if (callback is not null)
			callbacks.Add(callback);

		Task<IFontData?> task = Task.Run(() => source.LoadFont(data));

		void OnComplete() {
			IFontData? result;

			try {
				result = task.Result;
			} catch(Exception ex) {
				Log($"Error while loading font: {ex}", StardewModdingAPI.LogLevel.Error);
				result = null;
			}

			// Got garbage back?
			if (result is null || result.Source != data.Source || result.UniqueId != data.UniqueId)
				result = null;

			if (result is not null && FontData is not null) {
				Dictionary<string, IFontData>? entries;

				lock((FontData as ICollection).SyncRoot) {
					if (!FontData.TryGetValue(result.Source, out entries)) {
						entries = new(StringComparer.OrdinalIgnoreCase);
						FontData[result.Source] = entries;
					}
				}

				lock((entries as ICollection).SyncRoot) {
					entries[result.UniqueId] = result;
				}
			}

			foreach(var cb in callbacks) {
				try {
					cb(result ?? data);
				} catch(Exception ex) {
					Log($"Error when running LoadFont callback: {ex}", StardewModdingAPI.LogLevel.Error);
				}
			}
		}

		PendingLoadingTasks.Add((task, OnComplete));
	}

	#endregion

	#region Bulk Access

	public IReadOnlyList<IFontData>? GetLoadedFonts() {
		if (IsLoading)
			CheckDone();

		if (!IsLoaded)
			return null;

		List<IFontData> result = new();

		foreach(var entry in FontData.Values) {
			foreach(var other in entry.Values) {
				result.Add(other);
			}
		}

		return result;
	}

	#endregion

	#region Bulk Loading

	public void CheckDone() {
		if (!IsLoading)
			return;

		if (!LoadingTask.IsCompleted)
			return;

		try {
			FontData = LoadingTask.Result;
		} catch (Exception ex) {
			Log($"Unable to retrieve result from loading thread: {ex}", StardewModdingAPI.LogLevel.Error);
			FontData = null;
		}

		LoadingTask = null;
		LoadingComplete?.SafeInvoke(this, monitor: Mod.Monitor);
	}

	public void LoadFonts(bool forceReload = false) {
		if (IsLoading)
			return;

		if (IsLoaded) {
			if (!forceReload)
				return;

			FontData = null;
		}

		var progress = new Progress<LoadingProgress>();

		progress.ProgressChanged += (sender, prog) => {
			LoadingProgress?.SafeInvoke(this, new ProgressEventArgs(prog), monitor: Mod.Monitor);
		};

		LoadingTask = Task.Run(() => PerformLoad(progress));
	}

	private async Task<Dictionary<string, Dictionary<string, IFontData>>> PerformLoad(IProgress<LoadingProgress> progress) {

		List<KeyValuePair<string, IFontSource>> sources;

		lock((FontSources as ICollection).SyncRoot) {
			sources = FontSources.ToList();
		}

		int finishedSources = 0;
		int totalSources = sources.Count;
		int finishedFonts = 0;
		int totalFonts = 0;

		Dictionary<string, Dictionary<string, IFontData>> result = new(StringComparer.OrdinalIgnoreCase);

		foreach(var source in sources) {
			progress.Report(new LoadingProgress(finishedSources, totalSources, finishedFonts, totalFonts));

			if (!result.TryGetValue(source.Key, out var entries))
				entries = new Dictionary<string, IFontData>(StringComparer.OrdinalIgnoreCase);

			int total = 0;
			int loaded = 0;

			var prog = new Progress<int>();
			prog.ProgressChanged += (sender, amount) => {
				int difference = amount - total;
				if (difference != 0) {
					totalFonts += difference;
					progress.Report(new LoadingProgress(finishedSources, totalSources, finishedFonts, totalFonts));
				}
			};

			await foreach(var font in source.Value.GetAllFonts(prog)) {
				if (!entries.ContainsKey(font.UniqueId)) {
					finishedFonts++;
					loaded++;
					if (loaded > total) {
						total++;
						totalFonts++;
					}
				}

				entries[font.UniqueId] = font;
				progress.Report(new LoadingProgress(finishedSources, totalSources, finishedFonts, totalFonts));
			}

			if (entries.Count > 0)
				result[source.Key] = entries;

			finishedSources++;
		}

		progress.Report(new LoadingProgress(finishedSources, totalSources, finishedFonts, totalFonts));

		return result;
	}

	#endregion

}
