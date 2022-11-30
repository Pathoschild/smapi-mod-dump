/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace StardewHitboxes
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private bool AllowToggle = true;

        private static bool ShowHitboxes = true;

        private static Texture2D hitboxTexture;

        private static ModConfig Config;

        private static readonly Dictionary<Rectangle, int> weaponHitboxesToRender = new();

        public ModEntry()
        {
            Harmony harmony = new("tylergibbs2.stardewhitboxes");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            hitboxTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            hitboxTexture.SetData(new[] { Color.White });

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.GameLaunched += SetupGMCM;

            helper.Events.Input.ButtonPressed += OnButtonPressed;

            helper.Events.Display.RenderedWorld += RenderedWorld;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            ShowHitboxes = false;
            AllowToggle = Config.Enabled;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Config.ToggleKey.JustPressed() && Context.IsWorldReady && AllowToggle)
                ShowHitboxes = !ShowHitboxes;
        }

        private void SetupGMCM(object sender, GameLaunchedEventArgs e)
        {
            IGenericModConfigMenuApi configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.OnFieldChanged(ModManifest, (o, e) =>
            {
                Config = Helper.ReadConfig<ModConfig>();
                AllowToggle = Config.Enabled;
                if (!Config.Enabled)
                    ShowHitboxes = false;
            });

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enabled",
                tooltip: () => "Whether or not the mod is enabled",
                getValue: () => Config.Enabled,
                setValue: value => Config.Enabled = value
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Toggle Key",
                tooltip: () => "The key used to toggle hitboxes",
                getValue: () => Config.ToggleKey,
                setValue: value => Config.ToggleKey = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Hitbox Opacity",
                tooltip: () => "The opacity for hitboxes",
                getValue: () => Config.HitboxOpacity,
                setValue: value => Config.HitboxOpacity = value
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Farmer Hitbox Color",
                tooltip: () => "The hexadecimal color of the hitbox for farmers",
                getValue: () => Config.FarmerHitboxColor.ToUpper(),
                setValue: value => Config.FarmerHitboxColor = value.ToUpper()
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Character Hitbox Color",
                tooltip: () => "The hexadecimal color of the hitbox for characters",
                getValue: () => Config.CharacterHitboxColor.ToUpper(),
                setValue: value => Config.CharacterHitboxColor = value.ToUpper()
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Monster Hitbox Color",
                tooltip: () => "The hexadecimal color of the hitbox for monsters",
                getValue: () => Config.MonsterHitboxColor.ToUpper(),
                setValue: value => Config.MonsterHitboxColor = value.ToUpper()
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Weapon Hitbox Color",
                tooltip: () => "The hexadecimal color of the hitbox for weapon swings",
                getValue: () => Config.WeaponSwingHitboxColor.ToUpper(),
                setValue: value => Config.WeaponSwingHitboxColor = value.ToUpper()
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Projectile Hitbox Color",
                tooltip: () => "The hexadecimal color of the hitbox for projectiles",
                getValue: () => Config.ProjectileHitboxColor.ToUpper(),
                setValue: value => Config.ProjectileHitboxColor = value.ToUpper()
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Terrain Feature Hitbox Color",
                tooltip: () => "The hexadecimal color of the hitbox for terrain features",
                getValue: () => Config.TerrainFeatureHitboxColor.ToUpper(),
                setValue: value => Config.TerrainFeatureHitboxColor = value.ToUpper()
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Objects Hitbox Color",
                tooltip: () => "The hexadecimal color of the hitbox for objects",
                getValue: () => Config.ObjectsHitboxColor.ToUpper(),
                setValue: value => Config.ObjectsHitboxColor = value.ToUpper()
            );
        }

        public static void RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            Color color = ConvertFromHex(Config.WeaponSwingHitboxColor);

            foreach (Rectangle rect in weaponHitboxesToRender.Keys)
            {
                DrawHitbox(e.SpriteBatch, rect, color);

                weaponHitboxesToRender[rect]--;
                if (weaponHitboxesToRender[rect] <= 0)
                    weaponHitboxesToRender.Remove(rect);
            }
        }

        private static Color ConvertFromHex(string s)
        {
            if (s.Length != 7)
                return Color.Gray;

            int r = Convert.ToInt32(s.Substring(1, 2), 16);
            int g = Convert.ToInt32(s.Substring(3, 2), 16);
            int b = Convert.ToInt32(s.Substring(5, 2), 16);
            return new Color(r, g, b);
        }

        public static Color GetHitboxColorForCharacter(Character character)
        {
            if (character is Farmer)
                return ConvertFromHex(Config.FarmerHitboxColor);
            else if (character is Monster)
                return ConvertFromHex(Config.MonsterHitboxColor);
            else
                return ConvertFromHex(Config.CharacterHitboxColor);
        }

        public static void RenderWeaponAOE(Rectangle areaOfEffect)
        {
            weaponHitboxesToRender[areaOfEffect] = 5;
        }

        public static void DrawHitbox(SpriteBatch b, Projectile projectile)
        {
            Color color = ConvertFromHex(Config.ProjectileHitboxColor);
            DrawHitbox(b, projectile.getBoundingBox(), color);
        }

        public static void DrawHitbox(SpriteBatch b, Character character)
        {
            Color color = GetHitboxColorForCharacter(character);
            DrawHitbox(b, character.GetBoundingBox(), color);
        }

        public static void DrawHitbox(SpriteBatch b, TerrainFeature terrainFeature, Vector2 tileLocation)
        {
            Color color = ConvertFromHex(Config.TerrainFeatureHitboxColor);
            DrawHitbox(b, terrainFeature.getBoundingBox(tileLocation), color);
        }

        public static void DrawHitbox(SpriteBatch b, StardewValley.Object stardewObject, Vector2 tileLocation)
        {
            Color color = ConvertFromHex(Config.ObjectsHitboxColor);
            DrawHitbox(b, stardewObject.getBoundingBox(tileLocation), color);
        }

        public static void DrawHitbox(SpriteBatch b, Rectangle rect, Color color)
        {
            if (!ShowHitboxes)
                return;

            b.Draw(
                hitboxTexture,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(rect.X, rect.Y)),
                rect,
                color * Config.HitboxOpacity,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
        }
    }
}