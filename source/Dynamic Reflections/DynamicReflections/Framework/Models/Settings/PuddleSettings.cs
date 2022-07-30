/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicReflections.Framework.Models.Settings
{
    public class PuddleSettings
    {
        // Note: This property can only override disabling, it cannot force a user to enable reflections
        public const string MapProperty_IsEnabled = "ArePuddleReflectionsEnabled";
        public bool AreReflectionsEnabled { get; set; } = true;

        public const string MapProperty_ShouldGeneratePuddles = "ShouldGeneratePuddles";
        public bool ShouldGeneratePuddles { get; set; } = true;

        public const string MapProperty_ShouldPlaySplashSound = "ShouldPlaySplashSound";
        public bool ShouldPlaySplashSound { get; set; } = true;

        public const string MapProperty_ShouldRainSplashPuddles = "ShouldRainSplashPuddles";
        public bool ShouldRainSplashPuddles { get; set; } = true;

        public const string MapProperty_PuddleReflectionOffset = "PuddleReflectionOffset";
        public Vector2 ReflectionOffset { get; set; } = Vector2.Zero;
        public const string MapProperty_NPCReflectionOffset = "PuddleNPCReflectionOffset";
        public Vector2 NPCReflectionOffset { get; set; } = new Vector2(0f, 0.3f);

        public const string MapProperty_PuddlePercentageWhileRaining = "PuddlePercentageWhileRaining";
        public int PuddlePercentageWhileRaining { get; set; } = 20;

        public const string MapProperty_PuddlePercentageAfterRaining = "PuddlePercentageAfterRaining";
        public int PuddlePercentageAfterRaining { get; set; } = 10;

        public const string MapProperty_BigPuddleChance = "BigPuddleChance";
        public int BigPuddleChance { get; set; } = 25;

        public const string MapProperty_MillisecondsBetweenRaindropSplashes = "MillisecondsBetweenRaindropSplashes";
        public int MillisecondsBetweenRaindropSplashes { get; set; } = 500;

        public const string MapProperty_ReflectionOverlay = "PuddleReflectionOverlay";
        public Color ReflectionOverlay { get; set; } = new Color(255, 255, 255, 155);

        public const string MapProperty_PuddleColor = "PuddleColor";
        public Color PuddleColor { get; set; } = new Color(91, 91, 91, 91);

        public const string MapProperty_RippleColor = "RippleColor";
        public Color RippleColor { get; set; } = new Color(255, 255, 255, 155);


        public bool OverrideDefaultSettings { get; set; }

        public void Reset(PuddleSettings referencedSettings = null)
        {
            if (referencedSettings is null)
            {
                AreReflectionsEnabled = true;
                ShouldGeneratePuddles = true;
                ShouldPlaySplashSound = true;
                ShouldRainSplashPuddles = true;
                ReflectionOffset = Vector2.Zero;
                NPCReflectionOffset = new Vector2(0f, 0.3f);
                PuddlePercentageWhileRaining = 20;
                PuddlePercentageAfterRaining = 10;
                BigPuddleChance = 25;
                MillisecondsBetweenRaindropSplashes = 500;
                ReflectionOverlay = new Color(255, 255, 255, 155);
                PuddleColor = new Color(91, 91, 91, 91);
                RippleColor = new Color(255, 255, 255, 155);
                OverrideDefaultSettings = false;
            }
            else
            {
                AreReflectionsEnabled = referencedSettings.AreReflectionsEnabled;
                ShouldGeneratePuddles = referencedSettings.ShouldGeneratePuddles;
                ShouldPlaySplashSound = referencedSettings.ShouldPlaySplashSound;
                ShouldRainSplashPuddles = referencedSettings.ShouldRainSplashPuddles;
                ReflectionOffset = referencedSettings.ReflectionOffset;
                NPCReflectionOffset = referencedSettings.NPCReflectionOffset;
                PuddlePercentageWhileRaining = referencedSettings.PuddlePercentageWhileRaining;
                PuddlePercentageAfterRaining = referencedSettings.PuddlePercentageAfterRaining;
                BigPuddleChance = referencedSettings.BigPuddleChance;
                MillisecondsBetweenRaindropSplashes = referencedSettings.MillisecondsBetweenRaindropSplashes;
                ReflectionOverlay = referencedSettings.ReflectionOverlay;
                PuddleColor = referencedSettings.PuddleColor;
                RippleColor = referencedSettings.RippleColor;
            }
        }
    }
}
