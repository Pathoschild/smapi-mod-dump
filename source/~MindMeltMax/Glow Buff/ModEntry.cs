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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Buffs;
using StardewValley.GameData.Objects;

namespace GlowBuff
{
    internal class ModEntry : Mod
    {
        internal static ModEntry Instance;

        public Texture2D HoverIcon;

        public Dictionary<long, int> FarmerToLightSourceMap = [];
        public Dictionary<int, LightSourceData> LightSourceMap = [];

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            Helper.Events.GameLoop.GameLaunched += onGameLaunch;

            Helper.Events.Content.AssetRequested += onAssetRequested;
        }

        public static void OnNewLocation(Farmer who, GameLocation current, GameLocation old)
        {
            if (!Instance.FarmerToLightSourceMap.TryGetValue(who.UniqueMultiplayerID, out int lightSourceId))
                return;
            old?.removeLightSource(lightSourceId);
            if (current is not null && Instance.LightSourceMap.TryGetValue(lightSourceId, out var data))
                current.sharedLights[lightSourceId] = new LightSource(data.TextureId, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), data.Radius, data.Color, lightSourceId, playerID: who.UniqueMultiplayerID);
        }

        public static void ClearLightSource(int id, Farmer who)
        {
            Instance.LightSourceMap.Remove(id);
            OnNewLocation(who, null!, who.currentLocation);
            Instance.FarmerToLightSourceMap.Remove(who.UniqueMultiplayerID);
        }

        private void onGameLaunch(object? sender, GameLaunchedEventArgs e)
        {
            Patches.Patch(this);
            HoverIcon = Helper.ModContent.Load<Texture2D>("Assets/HoverIcon.png");
        }

        private void onAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data\\Buffs"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, BuffData>().Data;

                    data[$"{ModManifest.UniqueID}.Glow"] = new()
                    {
                        DisplayName = Helper.Translation.Get("Buff.DefaultName"),
                        Description = Helper.Translation.Get("Buff.DefaultDescription"),
                        Duration = -2,
                        IconTexture = $"{ModManifest.UniqueID}\\BuffIcon",
                        IconSpriteIndex = 0,
                    };
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo($"{ModManifest.UniqueID}\\BuffIcon"))
                e.LoadFromModFile<Texture2D>("Assets/BuffIcon.png", AssetLoadPriority.Exclusive);
            if (e.NameWithoutLocale.IsEquivalentTo($"{ModManifest.UniqueID}\\HoverIcon"))
                e.LoadFromModFile<Texture2D>("Assets/HoverIcon.png", AssetLoadPriority.Exclusive);
        }
    }
}
