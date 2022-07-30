/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using DynamicReflections.Framework.Interfaces;
using DynamicReflections.Framework.Models.Settings;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicReflections.Framework.External.GenericModConfigMenu
{
    internal class GMCMHelper
    {
        internal static bool IsLocationOverridingWaterDefault;
        internal static bool IsLocationOverridingPuddleDefault;
        internal static bool IsLocationOverridingSkyDefault;

        internal static readonly string DEFAULT_LOCATION = "Default";
        private static readonly string LOCATION_SELECTOR_ID = "PeacefulEnd.DynamicReflections.LocationSelector.Id";
        private static string _currentLocation = DEFAULT_LOCATION;

        public static void Register(IGenericModConfigMenuApi configApi, DynamicReflections dynamicReflections, bool unregisterOld = false, bool loadLocationNames = false)
        {
            var Helper = dynamicReflections.Helper;
            var ModManifest = dynamicReflections.ModManifest;
            if (unregisterOld is true)
            {
                configApi.Unregister(ModManifest);
            }
            configApi.Register(ModManifest, () => ResetConfig(), delegate { Helper.WriteConfig(DynamicReflections.modConfig); dynamicReflections.SetWaterReflectionSettings(); dynamicReflections.SetPuddleReflectionSettings(); dynamicReflections.SetSkyReflectionSettings(recalculate: true); dynamicReflections.LoadRenderers(); });

            // Register the standard settings
            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.title"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.AreMirrorReflectionsEnabled, value => DynamicReflections.modConfig.AreMirrorReflectionsEnabled = value, () => Helper.Translation.Get("config.general_settings.mirror_reflections"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.AreWaterReflectionsEnabled, value => DynamicReflections.modConfig.AreWaterReflectionsEnabled = value, () => Helper.Translation.Get("config.general_settings.water_reflections"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.ArePuddleReflectionsEnabled, value => DynamicReflections.modConfig.ArePuddleReflectionsEnabled = value, () => Helper.Translation.Get("config.general_settings.puddle_reflections"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.AreNPCReflectionsEnabled, value => DynamicReflections.modConfig.AreNPCReflectionsEnabled = value, () => Helper.Translation.Get("config.general_settings.npc_reflections"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.AreSkyReflectionsEnabled, value => DynamicReflections.modConfig.AreSkyReflectionsEnabled = value, () => Helper.Translation.Get("config.general_settings.sky_reflections"));
            configApi.AddKeybind(ModManifest, () => DynamicReflections.modConfig.QuickMenuKey, value => DynamicReflections.modConfig.QuickMenuKey = value, () => Helper.Translation.Get("config.general_settings.shortcut_key"), () => Helper.Translation.Get("config.general_settings.shortcut_key.description"));

            configApi.AddParagraph(ModManifest, () => String.Empty);
            if (loadLocationNames is true)
            {
                configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.location_specific.title"), () => Helper.Translation.Get("config.location_specific.description"));
                configApi.AddTextOption(ModManifest, () => _currentLocation, value => _currentLocation = value, () => Helper.Translation.Get("config.location_specific.selector"), tooltip: () => Helper.Translation.Get("config.location_specific.description"), DynamicReflections.activeLocationNames, fieldId: LOCATION_SELECTOR_ID);
                configApi.OnFieldChanged(ModManifest, (key, value) => HandleFieldChange(key, value));
                configApi.AddParagraph(ModManifest, () => $"Default Water Settings Overriden by Current Location: {IsLocationOverridingWaterDefault}\n\nDefault Puddle Settings Overriden by Current Location: {IsLocationOverridingPuddleDefault}\n\nDefault Sky Settings Overriden by Current Location: {IsLocationOverridingSkyDefault}");
            }
            configApi.AddParagraph(ModManifest, () => String.Empty);

            configApi.AddPageLink(ModManifest, "water_settings", () => Helper.Translation.Get("config.water_settings.link"));
            configApi.AddPage(ModManifest, "water_settings", () => Helper.Translation.Get("config.water_settings.title"));
            configApi.AddSectionTitle(ModManifest, () => _currentLocation);
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].OverrideDefaultSettings, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].OverrideDefaultSettings = value, () => Helper.Translation.Get("config.general_settings.override_default_settings"), () => Helper.Translation.Get("config.general_settings.override_default_settings.description"));

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.title"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].AreReflectionsEnabled, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].AreReflectionsEnabled = value, () => Helper.Translation.Get("config.general_settings.water_reflections"));
            configApi.AddTextOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionDirection.ToString(), value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionDirection = (Direction)Enum.Parse(typeof(Direction), value), () => Helper.Translation.Get("config.water_settings.reflection_direction"), tooltip: () => Helper.Translation.Get("config.water_settings.reflection_direction.description"), new string[] { Direction.North.ToString(), Direction.South.ToString() });

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.reflection_offets"));
            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.player_offsets"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].PlayerReflectionOffset.X, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].PlayerReflectionOffset = new Vector2(value, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].PlayerReflectionOffset.Y), () => Helper.Translation.Get("config.water_settings.offset.x"), interval: 0.1f);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].PlayerReflectionOffset.Y, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].PlayerReflectionOffset = new Vector2(DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].PlayerReflectionOffset.X, value), () => Helper.Translation.Get("config.water_settings.offset.y"), interval: 0.1f);
            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.npc_offsets"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].NPCReflectionOffset.X, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].NPCReflectionOffset = new Vector2(value, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].NPCReflectionOffset.Y), () => Helper.Translation.Get("config.water_settings.offset.x"), interval: 0.1f);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].NPCReflectionOffset.Y, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].NPCReflectionOffset = new Vector2(DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].NPCReflectionOffset.X, value), () => Helper.Translation.Get("config.water_settings.offset.y"), interval: 0.1f);

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.effects"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].IsReflectionWavy, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].IsReflectionWavy = value, () => Helper.Translation.Get("config.water_settings.is_wavy"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].WaveSpeed, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].WaveSpeed = value, () => Helper.Translation.Get("config.water_settings.wave_speed"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].WaveAmplitude, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].WaveAmplitude = value, () => Helper.Translation.Get("config.water_settings.wave_amplitude"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].WaveFrequency, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].WaveFrequency = value, () => Helper.Translation.Get("config.water_settings.wave_frequency"));

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.water_settings.color.title"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.R, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay = new Color(value, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.G, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.B, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.A), () => Helper.Translation.Get("config.water_settings.color.r"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.G, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay = new Color(DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.R, value, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.B, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.A), () => Helper.Translation.Get("config.water_settings.color.g"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.B, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay = new Color(DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.R, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.G, value, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.A), () => Helper.Translation.Get("config.water_settings.color.b"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.A, value => DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay = new Color(DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.R, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.G, DynamicReflections.modConfig.LocalWaterReflectionSettings[_currentLocation].ReflectionOverlay.B, value), () => Helper.Translation.Get("config.water_settings.color.a"), min: 0, max: 255, interval: 1);
            configApi.AddPageLink(ModManifest, String.Empty, () => Helper.Translation.Get("config.general_settings.link.return_main"));

            configApi.AddPage(ModManifest, String.Empty, () => Helper.Translation.Get("config.general_settings.title"));
            configApi.AddPageLink(ModManifest, "puddle_settings", () => Helper.Translation.Get("config.puddle_settings.link"));

            configApi.AddPage(ModManifest, "puddle_settings", () => Helper.Translation.Get("config.puddle_settings.title"));
            configApi.AddSectionTitle(ModManifest, () => _currentLocation);
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].OverrideDefaultSettings, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].OverrideDefaultSettings = value, () => Helper.Translation.Get("config.general_settings.override_default_settings"), () => Helper.Translation.Get("config.general_settings.override_default_settings.description"));

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.title"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].AreReflectionsEnabled, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].AreReflectionsEnabled = value, () => Helper.Translation.Get("config.general_settings.puddle_reflections"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ShouldGeneratePuddles, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ShouldGeneratePuddles = value, () => Helper.Translation.Get("config.puddle_settings.should_generate_puddles"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ShouldPlaySplashSound, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ShouldPlaySplashSound = value, () => Helper.Translation.Get("config.puddle_settings.should_play_splash_sound"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ShouldRainSplashPuddles, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ShouldRainSplashPuddles = value, () => Helper.Translation.Get("config.puddle_settings.should_rain_splash_puddles"));

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.reflection_offets"));
            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.player_offsets"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOffset.X, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOffset = new Vector2(value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOffset.Y), () => Helper.Translation.Get("config.water_settings.offset.x"), interval: 0.1f);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOffset.Y, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOffset = new Vector2(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOffset.X, value), () => Helper.Translation.Get("config.water_settings.offset.y"), interval: 0.1f);
            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.npc_offsets"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].NPCReflectionOffset.X, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].NPCReflectionOffset = new Vector2(value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].NPCReflectionOffset.Y), () => Helper.Translation.Get("config.water_settings.offset.x"), interval: 0.1f);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].NPCReflectionOffset.Y, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].NPCReflectionOffset = new Vector2(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].NPCReflectionOffset.X, value), () => Helper.Translation.Get("config.water_settings.offset.y"), interval: 0.1f);

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.effects"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddlePercentageWhileRaining, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddlePercentageWhileRaining = value, () => Helper.Translation.Get("config.puddle_settings.puddle_percentage_while_raining"), min: 0, max: 100, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddlePercentageAfterRaining, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddlePercentageAfterRaining = value, () => Helper.Translation.Get("config.puddle_settings.puddle_percentage_after_raining"), min: 0, max: 100, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].BigPuddleChance, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].BigPuddleChance = value, () => Helper.Translation.Get("config.puddle_settings.big_puddle_chance"), min: 0, max: 100, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].MillisecondsBetweenRaindropSplashes, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].MillisecondsBetweenRaindropSplashes = value, () => Helper.Translation.Get("config.puddle_settings.milliseconds_between_raindrop_splashes"), tooltip: () => Helper.Translation.Get("config.puddle_settings.milliseconds_between_raindrop_splashes_description"), min: 100, max: 2000, interval: 100);

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.water_settings.color.title"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.R, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay = new Color(value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.G, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.B, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.A), () => Helper.Translation.Get("config.water_settings.color.r"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.G, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay = new Color(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.R, value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.B, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.A), () => Helper.Translation.Get("config.water_settings.color.g"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.B, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay = new Color(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.R, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.G, value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.A), () => Helper.Translation.Get("config.water_settings.color.b"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.A, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay = new Color(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.R, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.G, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].ReflectionOverlay.B, value), () => Helper.Translation.Get("config.water_settings.color.a"), min: 0, max: 255, interval: 1);

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.puddle_settings.puddle_color.title"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.R, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor = new Color(value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.G, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.B, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.A), () => Helper.Translation.Get("config.puddle_settings.puddle_color.r"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.G, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor = new Color(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.R, value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.B, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.A), () => Helper.Translation.Get("config.puddle_settings.puddle_color.g"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.B, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor = new Color(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.R, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.G, value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.A), () => Helper.Translation.Get("config.puddle_settings.puddle_color.b"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.A, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor = new Color(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.R, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.G, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].PuddleColor.B, value), () => Helper.Translation.Get("config.puddle_settings.puddle_color.a"), min: 0, max: 255, interval: 1);

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.puddle_settings.ripple_color.title"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.R, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor = new Color(value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.G, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.B, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.A), () => Helper.Translation.Get("config.puddle_settings.ripple_color.r"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.G, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor = new Color(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.R, value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.B, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.A), () => Helper.Translation.Get("config.puddle_settings.ripple_color.g"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.B, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor = new Color(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.R, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.G, value, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.A), () => Helper.Translation.Get("config.puddle_settings.ripple_color.b"), min: 0, max: 255, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.A, value => DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor = new Color(DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.R, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.G, DynamicReflections.modConfig.LocalPuddleReflectionSettings[_currentLocation].RippleColor.B, value), () => Helper.Translation.Get("config.puddle_settings.ripple_color.a"), min: 0, max: 255, interval: 1);

            configApi.AddPageLink(ModManifest, String.Empty, () => Helper.Translation.Get("config.general_settings.link.return_main"));

            configApi.AddPage(ModManifest, String.Empty, () => Helper.Translation.Get("config.general_settings.title"));
            configApi.AddPageLink(ModManifest, "sky_settings", () => Helper.Translation.Get("config.sky_settings.link"));
            configApi.AddParagraph(ModManifest, () => $"{Environment.NewLine}");

            configApi.AddPage(ModManifest, "sky_settings", () => Helper.Translation.Get("config.sky_settings.title"));
            configApi.AddSectionTitle(ModManifest, () => _currentLocation);
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].OverrideDefaultSettings, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].OverrideDefaultSettings = value, () => Helper.Translation.Get("config.general_settings.override_default_settings"), () => Helper.Translation.Get("config.general_settings.override_default_settings.description"));

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.general_settings.title"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].AreReflectionsEnabled, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].AreReflectionsEnabled = value, () => Helper.Translation.Get("config.general_settings.sky_reflections"));
            configApi.AddBoolOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].AreShootingStarsEnabled, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].AreShootingStarsEnabled = value, () => Helper.Translation.Get("config.sky_settings.shooting_stars_enabled"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].StarDensityPercentage, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].StarDensityPercentage = value, () => Helper.Translation.Get("config.sky_settings.star_density"), min: 0, max: 100, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.MeteorShowerNightChance, value => DynamicReflections.modConfig.MeteorShowerNightChance = value, () => Helper.Translation.Get("config.sky_settings.meteor_shower_chance"), tooltip: () => Helper.Translation.Get("config.sky_settings.meteor_shower_chance.description"), min: 0, max: 100, interval: 1);

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.sky_settings.shooting_stars.title"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].MaxShootingStarAttemptsPerInterval, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].MaxShootingStarAttemptsPerInterval = value, () => Helper.Translation.Get("config.sky_settings.max_shooting_stars_per_attempt"), tooltip: () => Helper.Translation.Get("config.sky_settings.max_shooting_stars_per_attempt.description"), min: 1, max: 100, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].CometChance, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].CometChance = value, () => Helper.Translation.Get("config.sky_settings.comet_chance"), min: 0, max: 100, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].CometSegmentMin, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].CometSegmentMin = value, () => Helper.Translation.Get("config.sky_settings.comet_segment.min"), min: 1, max: 20, interval: 1);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].CometSegmentMax, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].CometSegmentMax = value, () => Helper.Translation.Get("config.sky_settings.comet_segment.max"), min: 2, max: 20, interval: 1);

            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].MillisecondsBetweenShootingStarAttempt, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].MillisecondsBetweenShootingStarAttempt = value, () => Helper.Translation.Get("config.sky_settings.milliseconds_between_stars"), tooltip: () => Helper.Translation.Get("config.sky_settings.milliseconds_between_stars.description"), min: 50, max: 10000, interval: 50);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].MillisecondsBetweenShootingStarAttemptDuringMeteorShower, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].MillisecondsBetweenShootingStarAttemptDuringMeteorShower = value, () => Helper.Translation.Get("config.sky_settings.milliseconds_between_stars_during_meteor_shower"), tooltip: () => Helper.Translation.Get("config.sky_settings.milliseconds_between_stars.description"), min: 50, max: 10000, interval: 50);

            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].ShootingStarMinSpeed, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].ShootingStarMinSpeed = value, () => Helper.Translation.Get("config.sky_settings.shooting_star_speed.min"), min: 0.01f, max: 1f, interval: 0.01f);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].ShootingStarMaxSpeed, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].ShootingStarMaxSpeed = value, () => Helper.Translation.Get("config.sky_settings.shooting_star_speed.max"), min: 0.01f, max: 1f, interval: 0.01f);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].CometMinSpeed, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].CometMinSpeed = value, () => Helper.Translation.Get("config.sky_settings.comet.min"), min: 0.01f, max: 1f, interval: 0.01f);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].CometMaxSpeed, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].CometMaxSpeed = value, () => Helper.Translation.Get("config.sky_settings.comet.max"), min: 0.01f, max: 1f, interval: 0.01f);

            configApi.AddSectionTitle(ModManifest, () => Helper.Translation.Get("config.sky_settings.star_reflections.title"));
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].GettingDarkWaterAlpha, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].GettingDarkWaterAlpha = value, () => Helper.Translation.Get("config.sky_settings.water_alpha.getting_dark"), tooltip: () => Helper.Translation.Get("config.sky_settings.water_alpha.description"), min: 0.01f, max: 1f, interval: 0.01f);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].HalfwayDarkWaterAlpha, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].HalfwayDarkWaterAlpha = value, () => Helper.Translation.Get("config.sky_settings.water_alpha.halfway_dark"), tooltip: () => Helper.Translation.Get("config.sky_settings.water_alpha.description"), min: 0.01f, max: 1f, interval: 0.01f);
            configApi.AddNumberOption(ModManifest, () => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].FinishedDarkWaterAlpha, value => DynamicReflections.modConfig.LocalSkyReflectionSettings[_currentLocation].FinishedDarkWaterAlpha = value, () => Helper.Translation.Get("config.sky_settings.water_alpha.finished_dark"), tooltip: () => Helper.Translation.Get("config.sky_settings.water_alpha.description"), min: 0.01f, max: 1f, interval: 0.01f);

            configApi.AddPageLink(ModManifest, String.Empty, () => Helper.Translation.Get("config.general_settings.link.return_main"));
        }

        private static void HandleFieldChange(string key, object value)
        {
            if (key != LOCATION_SELECTOR_ID)
            {
                return;
            }

            _currentLocation = value.ToString();
        }

        internal static void ResetConfig()
        {
            DynamicReflections.modConfig = new ModConfig();

            DynamicReflections.modConfig.LocalWaterReflectionSettings[DEFAULT_LOCATION] = DynamicReflections.modConfig.WaterReflectionSettings;
            DynamicReflections.modConfig.LocalPuddleReflectionSettings[DEFAULT_LOCATION] = DynamicReflections.modConfig.PuddleReflectionSettings;
            DynamicReflections.modConfig.LocalSkyReflectionSettings[DEFAULT_LOCATION] = DynamicReflections.modConfig.SkyReflectionSettings;


            // Populate the location-based settings
            RefreshLocationListing(reset: true);
        }

        internal static void RefreshLocationListing(bool reset = false)
        {
            _currentLocation = DEFAULT_LOCATION;

            var location = Game1.currentLocation;
            if (location is null)
            {
                return;
            }
            _currentLocation = IsLocationOverridingWaterDefault || IsLocationOverridingPuddleDefault || IsLocationOverridingSkyDefault ? location.NameOrUniqueName : _currentLocation;

            if (reset is true || DynamicReflections.modConfig.LocalWaterReflectionSettings.ContainsKey(location.NameOrUniqueName) is false)
            {
                DynamicReflections.modConfig.LocalWaterReflectionSettings[location.NameOrUniqueName] = GetDefaultLocationSpecificWaterSettings(location);
            }
            if (reset is true || DynamicReflections.modConfig.LocalPuddleReflectionSettings.ContainsKey(location.NameOrUniqueName) is false)
            {
                DynamicReflections.modConfig.LocalPuddleReflectionSettings[location.NameOrUniqueName] = new PuddleSettings();
            }
            if (reset is true || DynamicReflections.modConfig.LocalSkyReflectionSettings.ContainsKey(location.NameOrUniqueName) is false)
            {
                DynamicReflections.modConfig.LocalSkyReflectionSettings[location.NameOrUniqueName] = new SkySettings();
            }

            DynamicReflections.activeLocationNames[0] = DEFAULT_LOCATION;
            DynamicReflections.activeLocationNames[1] = location.NameOrUniqueName;

            /*
            foreach (var location in Game1.locations.Where(l => l is not null && l.IsOutdoors))
            {
                if (reset is true || DynamicReflections.modConfig.LocalWaterReflectionSettings.ContainsKey(location.NameOrUniqueName) is false)
                {
                    DynamicReflections.modConfig.LocalWaterReflectionSettings[location.NameOrUniqueName] = GetDefaultLocationSpecificWaterSettings(location);
                    DynamicReflections.modConfig.LocalPuddleReflectionSettings[location.NameOrUniqueName] = new PuddleSettings();
                }
            }
            */
        }

        internal static WaterSettings GetDefaultLocationSpecificWaterSettings(GameLocation location)
        {
            switch (location.NameOrUniqueName)
            {
                case "Beach":
                    return new WaterSettings() { OverrideDefaultSettings = true, PlayerReflectionOffset = new Vector2(0f, 1f), NPCReflectionOffset = new Vector2(0f, 0.7f) };
            }
            return new WaterSettings();
        }
    }
}
