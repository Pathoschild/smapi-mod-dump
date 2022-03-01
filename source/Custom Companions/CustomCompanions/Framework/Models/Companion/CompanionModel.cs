/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace CustomCompanions.Framework.Models.Companion
{
    public class CompanionModel
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public ITranslationHelper Translations { get; set; }
        public string Type { get; set; }
        public bool EnablePeriodicPatchCheck { get; set; }
        public bool EnableSpawnAtDayStart { get; set; }
        public bool EnableFarmerCollision { get; set; }
        public bool EnableCharacterCollision { get; set; } = true;
        public bool EnableShadow { get; set; }
        public ShadowModel Shadow { get; set; }
        public bool EnableBreathing { get; set; }
        public int TravelSpeed { get; set; } = 6;
        public int SpawnDirection { get; set; } = -1;
        public int SpawnOffsetX { get; set; }
        public int SpawnOffsetY { get; set; }
        public float MaxIdleDistance { get; set; } = 128f;
        public float MaxDistanceBeforeTeleport { get; set; } = 512f;
        public float DirectionChangeChanceWhileMoving { get; set; } = 0.007f;
        public float DirectionChangeChanceWhileIdle { get; set; } = 0.1f;
        public bool CanHalt { get; set; } = true;
        public float ChanceForHalting { get; set; } = 1.0f;
        public int MinHaltTime { get; set; } = 2000;
        public int MaxHaltTime { get; set; } = 10000;
        public PortraitModel Portrait { get; set; }
        public string PortraitSheetPath { get; set; }
        public string InspectionDialogue { get; set; }
        public int OverheadTextCheckInterval { get; set; } = 5000;
        public float OverheadTextChance { get; set; } = 0.5f;
        public List<OverheadTextModel> OverheadTexts { get; set; } = new List<OverheadTextModel>();
        public int[] DespawnOnTile { get; set; }
        public int DespawnOnTimer { get; set; } = -1;
        public bool Respawn { get; set; }
        public string IdleBehavior { get; set; } = "NOTHING";
        public float[] IdleArguments { get; set; }
        public string TargetNpcName { get; set; }
        public int MinTilesForNearby { get; set; } = 1;
        public CompanionModel UpdateWhenPlayerNearby { get; set; }
        public List<int[]> Colors { get; set; } = new List<int[]>();
        public bool IsPrismatic { get; set; } = false;
        public LightModel Light { get; set; }
        public List<SoundModel> Sounds { get; set; } = new List<SoundModel>();
        public float Scale { get; set; } = 1f;
        public bool AppearUnderwater { get; set; }
        public int FrameSizeWidth { get; set; }
        public int FrameSizeHeight { get; set; }
        public int CollisionPositionX { get; set; }
        public int CollisionPositionY { get; set; }
        public int CollisionPositionWidth { get; set; }
        public int CollisionPositionHeight { get; set; }
        public string TileSheetPath { get; set; }
        public AnimationModel UniformAnimation { get; set; }
        public AnimationModel UpAnimation { get; set; }
        public AnimationModel DownAnimation { get; set; }
        public AnimationModel LeftAnimation { get; set; }
        public AnimationModel RightAnimation { get; set; }

        public string GetId()
        {
            return $"{Owner}.{Name}";
        }

        public bool ContainsColor(Color color)
        {
            return Colors.Any(c => c.SequenceEqual(new int[] { color.R, color.G, color.B, color.A }));
        }

        public override string ToString()
        {
            return $"\n[\n" +
                $"\tOwner: {Owner} | Name: {Name} | Type: {Type} | EnableFarmerCollision: {EnableFarmerCollision}\n" +
                $"\tEnableShadow: {EnableShadow} | Shadow: {Shadow} | EnableBreathing: {EnableBreathing}\n" +
                $"\tTravelSpeed: {TravelSpeed} | SpawnDirection: {SpawnDirection} | SpawnOffsetX: {SpawnOffsetX} | SpawnOffsetY: {SpawnOffsetY}\n" +
                $"\tDirectionChangeChanceWhileMoving: {DirectionChangeChanceWhileMoving} | DirectionChangeChanceWhileMoving: {DirectionChangeChanceWhileIdle}\n" +
                $"\tChanceForHalting: {ChanceForHalting} | MinHaltTime: {MinHaltTime} | MaxHaltTime: {MaxHaltTime}\n" +
                $"\tInspectionDialogue: {InspectionDialogue}\n" +
                $"\tMaxIdleDistance: {MaxIdleDistance} | MaxDistanceBeforeTeleport: {MaxDistanceBeforeTeleport} | IdleBehavior: {IdleBehavior} | IdleArguments: { (IdleArguments is null ? null : IdleArguments) }\n" +
                $"\tColors: {string.Join(",", Colors.Select(c => "[" + string.Join(",", c) + "]"))} | IsPrismatic: {IsPrismatic}\n" +
                $"\tLight: {(Light is null ? null : Light)}\n" +
                $"\tSounds: {string.Join(",", Sounds)}\n" +
                $"\tScale: {Scale} | DrawUnderWater: {AppearUnderwater} | FrameSize: {FrameSizeWidth}x{FrameSizeHeight}\n" +
                $"\tTileSheetPath: {TileSheetPath}\n" +
                $"\tUniformAnimation: {(UniformAnimation is null ? null : UniformAnimation)}\n" +
                $"\tUpAnimation: {(UpAnimation is null ? null : UpAnimation)}\n" +
                $"\tDownAnimation: {(DownAnimation is null ? null : DownAnimation)}\n" +
                $"\tLeftAnimation: {(LeftAnimation is null ? null : LeftAnimation)}\n" +
                $"\tRightAnimation: {(RightAnimation is null ? null : RightAnimation)}\n" +
                $"\tPortrait: {Portrait} | PortraitSheetPath: {PortraitSheetPath}\n]";
        }
    }
}
