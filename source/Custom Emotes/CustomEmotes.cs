/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/CustomEmotes
**
*************************************************/

using CustomEmotes.Model;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace CustomEmotes
{
    /// <summary>The mod entry point.</summary>
    public class CustomEmotes : Mod, IAssetEditor
    {
        private readonly LocalizedContentManager _vanillaContent = new(Game1.game1.Content.ServiceProvider, Game1.game1.Content.RootDirectory);
        private ICustomEmotesApi _api;

        internal Dictionary<string, int> EmoteIndexMap { get; } = new();

        internal static CustomEmotes Instance { get; private set; }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("TileSheets\\emotes");
        }

        public void Edit<T>(IAssetData asset)
        {
            var emotes = this.LoadEmotes().Deduplicate(e => e.name).ToList();
            var original = this._vanillaContent.Load<Texture2D>("TileSheets\\emotes");
            var image = asset.AsImage();
            int startIndex = (original.Width / 16) * (original.Height / 16);

            this.EmoteIndexMap.Clear();
            
            image.ExtendImage(0, original.Height + emotes.Count * 16);

            int cursor = 0;
            foreach (var emote in emotes)
            {
                if (emote.index * 16 > emote.texture.Height)
                {
                    this.Monitor.Log($"Emote '{emote.name}' is out of image bounds", LogLevel.Error);
                    continue;
                }

                image.PatchImage(emote.texture, new Rectangle(0, emote.index * 16, original.Width, 16), new Rectangle(0, original.Height + cursor * 16, original.Width, 16));
                this.EmoteIndexMap[emote.name] = startIndex + cursor * (original.Width / 16);
                ++cursor;
            }

            // This loop adds vanilla emote names to lookup map
            // Vanilla emote names are immutable, so they can't be overriden
            foreach (var emote in Farmer.EMOTES)
            {
                if (emote.emoteIconIndex > 0)
                    this.EmoteIndexMap[emote.emoteString] = emote.emoteIconIndex;
            }

            this.Monitor.Log($"Injected {this.EmoteIndexMap.Count} new custom emotes");
        }

        /// <summary>
        /// Loads emotes and their textures
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Emote> LoadEmotes()
        {
            foreach (var pack in this.Helper.ContentPacks.GetOwned())
            {
                if (!pack.HasFile("emotes.json"))
                {
                    this.Monitor.Log($"Custom emotes pack {pack.Manifest.UniqueID} has no 'emotes.json' file", LogLevel.Error);
                    continue;
                }

                foreach (var definition in pack.ReadJsonFile<EmoteDefinition[]>("emotes.json"))
                {
                    if (!pack.HasFile(definition.Image))
                    {
                        this.Monitor.Log($"Missing image '{definition.Image}' in custom emotes pack {pack.Manifest.UniqueID}", LogLevel.Error);
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(definition.EnableWithMod) && !this.Helper.ModRegistry.IsLoaded(definition.EnableWithMod))
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(definition.DisableWithMod) && this.Helper.ModRegistry.IsLoaded(definition.DisableWithMod))
                    {
                        continue;
                    }

                    var assetKey = pack.GetActualAssetKey(definition.Image);
                    var texture = pack.LoadAsset<Texture2D>(definition.Image);

                    foreach (var pair in definition.Map)
                    {
                        yield return new Emote(pair.Value, texture, pair.Key);
                    }
                }
            }
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            helper.Content.AssetEditors.Add(this);

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.command_emote)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Prefix_command_emote))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Dialogue), nameof(Dialogue.getCurrentDialogue)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Prefix_getCurrentDialogue))
            );
        }

        /// <summary>
        /// Provides a mod API for other SMAPI mods
        /// </summary>
        /// <returns></returns>
        public override ICustomEmotesApi GetApi()
        {
            if (this._api == null)
            {
                this._api = new CustomEmotesApi(this);
            }

            return this._api;
        }
    }
}
