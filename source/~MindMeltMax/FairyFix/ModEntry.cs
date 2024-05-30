/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Events;
using StardewValley.TerrainFeatures;
using System.Reflection;

namespace FairyFix
{
    public enum SelectionMode
    {
        Single,
        Connected,
        ConnectedSameCrop,
    }

    internal class ModEntry : Mod
    {
        internal static ModEntry Instance;

        public static Config Config;

        internal static readonly PerScreen<bool> SelectorMode = new(() => false);

        private Texture2D tiles;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = Helper.ReadConfig<Config>();

            helper.Events.GameLoop.GameLaunched += onGameLaunch;
            helper.Events.Display.RenderedWorld += (_, e) => DrawSelectorMode(e.SpriteBatch);
            helper.Events.Input.ButtonPressed += (_, e) =>
            {
                if (Config.ToggleButton.JustPressed())
                    SelectorMode.Value = !SelectorMode.Value;
                if (!SelectorMode.Value)
                    return;
                if (e.Button == SButton.MouseRight || e.Button == SButton.ControllerA)
                    tryToggleTileModData(e.Cursor.Tile);
            };

            /*if (e.IsDown(SButton.LeftControl) && Game1.getFarm().terrainFeatures.TryGetValue(e.Cursor.Tile, out var d) && d is HoeDirt hd && hd.crop != null)
            {
                hd.crop.Kill();
                hd.crop.sourceRect = hd.crop.getSourceRect((int)(hd.Tile.X * 7 + hd.Tile.Y * 11));
            }
            helper.ConsoleCommands.Add("fairyfix", "", (cmd, args) =>
            {
                if (!Context.IsPlayerFree)
                    return;
                var evt = new FairyEvent();
                typeof(FairyEvent).GetField("f", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(evt, Game1.getFarm());
                typeof(FairyEvent).GetField("targetCrop", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(evt, typeof(FairyEvent).GetMethod("ChooseCrop", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(evt, null));
                evt.makeChangesToLocation();
            });*/
        }

        private void onGameLaunch(object? sender, GameLaunchedEventArgs e)
        {
            tiles = Helper.ModContent.Load<Texture2D>("Assets/Tiles.png");
            Patches.Patch(ModManifest.UniqueID);

            var gmcm = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (gmcm is null)
                return;

            gmcm.Register(ModManifest, () => Config = new(), () => Helper.WriteConfig(Config));

            gmcm.AddTextOption(ModManifest, () => Enum.GetName(Config.SelectMode)!, (v) => Config.SelectMode = Enum.Parse<SelectionMode>(v), () => Helper.Translation.Get("Config.SelectMode.Name"), () => Helper.Translation.Get("Config.SelectMode.Description"), Enum.GetNames<SelectionMode>());

            gmcm.AddBoolOption(ModManifest, () => Config.ReviveDeadCrops, (v) => Config.ReviveDeadCrops = v, () => Helper.Translation.Get("Config.ReviveDeadCrops.Name"), () => Helper.Translation.Get("Config.ReviveDeadCrops.Description"));

            gmcm.AddBoolOption(ModManifest, () => Config.ResetOnSeasonChange, (v) => Config.ResetOnSeasonChange = v, () => Helper.Translation.Get("Config.ResetOnSeasonChange.Name"), () => Helper.Translation.Get("Config.ResetOnSeasonChange.Description"));

            gmcm.AddKeybindList(ModManifest, () => Config.ToggleButton, (v) => Config.ToggleButton = v, () => Helper.Translation.Get("Config.ToggleButton.Name"), () => Helper.Translation.Get("Config.ToggleButton.Description"));
        }

        private void DrawSelectorMode(SpriteBatch b)
        {
            if (!SelectorMode.Value || !Context.IsPlayerFree)
                return;
            foreach (var item in Game1.player.currentLocation.terrainFeatures.Pairs)
            {
                if (item.Value is not HoeDirt dirt)
                    continue;
                if (dirt.modData.ContainsKey(Patches.ModDataKey))
                {
                    b.Draw(tiles, Game1.GlobalToLocal(Game1.viewport, item.Key * new Vector2(64f)), new(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    continue;
                }
                if (dirt.crop is Crop c && c.modData.ContainsKey(Patches.ModDataKey))
                {
                    b.Draw(tiles, Game1.GlobalToLocal(Game1.viewport, item.Key * new Vector2(64f)), new(16, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    continue;
                }
                b.Draw(tiles, Game1.GlobalToLocal(Game1.viewport, item.Key * new Vector2(64f)), new(32, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            }
        }

        private void tryToggleTileModData(Vector2 tile)
        {
            bool toggleUnset = Game1.isOneOfTheseKeysDown(Game1.GetKeyboardState(), [new(Keys.LeftControl), new(Keys.RightControl)]) || Game1.input.GetGamePadState().IsButtonDown(Buttons.LeftStick);
            foreach (var dirt in loadToggleTiles(tile))
            {
                if (toggleUnset)
                {
                    dirt.crop?.modData.Remove(Patches.ModDataKey);
                    dirt.modData.Remove(Patches.ModDataKey);
                    continue;
                }
                if (dirt.crop is not null && !dirt.modData.ContainsKey(Patches.ModDataKey))
                {
                    if (dirt.crop.modData.ContainsKey(Patches.ModDataKey))
                    {
                        dirt.crop.modData.Remove(Patches.ModDataKey);
                        dirt.modData[Patches.ModDataKey] = "1";
                        continue;
                    }
                    dirt.crop.modData[Patches.ModDataKey] = "1";
                    continue;
                }
                if (dirt.modData.ContainsKey(Patches.ModDataKey))
                {
                    dirt.modData.Remove(Patches.ModDataKey);
                    continue;
                }
                dirt.modData[Patches.ModDataKey] = "1";
            }
        }

        private HashSet<HoeDirt> loadToggleTiles(Vector2 tile) //Only checks one tile to the left, but works for all other sides \\Or maybe you should give the left check the current tile instead of the initial you numb skull!
        {
            HashSet<HoeDirt> tiles = [];
            var farm = Game1.getFarm();
            var selectMode = Config.SelectMode;
            if (!farm.terrainFeatures.TryGetValue(tile, out var feature) || feature is not HoeDirt dirt)
                return [];
            if (selectMode == SelectionMode.Single)
                return [dirt];
            Queue<Vector2> queue = new([tile]);
            tiles.Add(dirt);

            while (queue.Count > 0)
            {
                Vector2 current = queue.Dequeue();

                if (tryGetHoeDirtAt(farm, new(current.X - 1, current.Y)) is HoeDirt ld && (selectMode != SelectionMode.ConnectedSameCrop || ld.crop?.netSeedIndex.Value == dirt.crop?.netSeedIndex.Value))
                {
                    if (tiles.Add(ld))
                        queue.Enqueue(new(current.X - 1, current.Y));
                }
                if (tryGetHoeDirtAt(farm, new(current.X + 1, current.Y)) is HoeDirt rd && (selectMode != SelectionMode.ConnectedSameCrop || rd.crop?.netSeedIndex.Value == dirt.crop?.netSeedIndex.Value))
                {
                    if (tiles.Add(rd))
                        queue.Enqueue(new(current.X + 1, current.Y));
                }
                if (tryGetHoeDirtAt(farm, new(current.X, current.Y - 1)) is HoeDirt bd && (selectMode != SelectionMode.ConnectedSameCrop || bd.crop?.netSeedIndex.Value == dirt.crop?.netSeedIndex.Value))
                {
                    if (tiles.Add(bd))
                        queue.Enqueue(new(current.X, current.Y - 1));
                }
                if (tryGetHoeDirtAt(farm, new(current.X, current.Y + 1)) is HoeDirt td && (selectMode != SelectionMode.ConnectedSameCrop || td.crop?.netSeedIndex.Value == dirt.crop?.netSeedIndex.Value))
                {
                    if (tiles.Add(td))
                        queue.Enqueue(new(current.X, current.Y + 1));
                }
            }

            return tiles;
        }

        private HoeDirt? tryGetHoeDirtAt(Farm f, Vector2 tile)
        {
            if (f.terrainFeatures.TryGetValue(tile, out var tf))
                if (tf is HoeDirt dirt)
                    return dirt;
            return null;
        }
    }

    public interface IGMCMApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }
}
