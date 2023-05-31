/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

// Copyright 2023 Jamie Taylor
using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using static System.Net.Mime.MediaTypeNames;

namespace ApiTest {
    public class ModEntry : Mod {
        public ModEntry() {
        }

        internal IToDewApi? api;

        public override void Entry(IModHelper helper) {
            helper.Events.GameLoop.GameLaunched += onLaunched;
        }

        private const string cliCommand = "todo-api-test";
        private readonly Dictionary<ulong, OverlayDataSource> sources = new();

        private void onLaunched(object? sender, GameLaunchedEventArgs e) {
            api = Helper.ModRegistry.GetApi<IToDewApi>("jltaylor-us.ToDew");
            if (api is null) {
                Monitor.Log("Failed getting ToDew API", LogLevel.Error);
                return;
            }
            Helper.ConsoleCommands.Add(cliCommand, "To-dew API tester", this.HandleCli);
        }

        private void HandleCli(string command, string[] args) {
            if (api is null) {
                Monitor.Log("Should not be able to get here if API is null", LogLevel.Error);
                return;
            }
            if (args.Length < 1) {
                Monitor.Log("requires at least one argument", LogLevel.Error);
                return;
            }
            switch (args[0]) {
                case "add": {
                        if (args.Length != 2) {
                            Monitor.Log($"Usage: {cliCommand} add <title>", LogLevel.Info);
                            return;
                        }
                        OverlayDataSource src = new(this, args[1]);
                        var id = api.AddOverlayDataSource(src);
                        sources.Add(id, src);
                        Monitor.Log($"Added souce with ID {id}", LogLevel.Info);
                        break;
                    }
                case "remove": {
                        if (args.Length != 2) {
                            Monitor.Log($"Usage: {cliCommand} remove <id>", LogLevel.Info);
                            return;
                        }
                        var id = uint.Parse(args[1]);
                        api.RemoveOverlayDataSource(id);
                        sources.Remove(id);
                        break;
                    }
                case "refresh":
                    api.RefreshOverlay();
                    break;
                case "add-item": {
                        if (args.Length != 3) {
                            Monitor.Log($"Usage: {cliCommand} add-item <id> <text>", LogLevel.Info);
                            return;
                        }
                        var id = ulong.Parse(args[1]);
                        Monitor.Log($"add-item to {sources[id].GetSectionTitle()}", LogLevel.Debug);
                        sources[id].AddItem(args[2]);
                        break;
                    }
                //case "":
                //    break;
                default:
                    Monitor.Log($"unknown commnd {args[0]}", LogLevel.Error);
                    break;
            }
        }
    }

    public interface IToDewApi {
        uint AddOverlayDataSource(IToDewOverlayDataSource src);
        void RemoveOverlayDataSource(uint handle);
        void RefreshOverlay();
    }
    public interface IToDewOverlayDataSource {
        string GetSectionTitle();
        List<(string text, bool isBold, Action? onDone)> GetItems(int limit);
    }
    internal class OverlayDataSource : IToDewOverlayDataSource {
        private readonly ModEntry theMod;
        private readonly string title;
        private readonly List<string> items = new();
        public OverlayDataSource(ModEntry theMod, string title) {
            this.theMod = theMod;
            this.title = title;
        }

        public List<(string text, bool isBold, Action? onDone)> GetItems(int limit) {
            List<(string text, bool isBold, Action? onDone)> result = new();
            for (int i = 0; i < items.Count && i <= limit; i++) {
                int idx = i;
                result.Add((items[i], false, () => { items.RemoveAt(idx); theMod.api?.RefreshOverlay(); }));
            }
            return result;
        }

        public string GetSectionTitle() {
            return title;
        }

        public void AddItem(string text) {
            items.Add(text);
        }
    }
}
