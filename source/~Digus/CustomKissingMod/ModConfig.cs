/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace CustomKissingMod
{
    public class ModConfig
    {
        public bool DisableDatingRequirement { get; set; }
        public bool DisableEventRequirement { get; set; }
        public bool DisableJealousy { get; set; }
        public bool DisableExhaustionReset { get; set; }
        public int RequiredFriendshipLevel { get; set; }
        public int KissingFriendshipPoints { get; set; }
        public int JealousyFriendshipPoints { get; set; }
        public bool EnableContentPacksOverrides { get; set; }
        public List<NpcConfig> NpcConfigs { get; set; }

        public ModConfig()
        {
            DisableDatingRequirement = false;
            DisableEventRequirement = false;
            DisableJealousy = false;
            DisableExhaustionReset = false;
            RequiredFriendshipLevel = 8;
            KissingFriendshipPoints = 10;
            JealousyFriendshipPoints = -250;
            NpcConfigs = new List<NpcConfig>()
            {
                new NpcConfig
                {
                    Name = "Abigail",
                    Frame = 33,
                    FrameDirectionRight = false,
                    RequiredEvent = "901756"
                },
                new NpcConfig
                {
                    Name = "Alex",
                    Frame = 42,
                    FrameDirectionRight = true,
                    RequiredEvent = "911526"
                },
                new NpcConfig
                {
                    Name = "Elliott",
                    Frame = 35,
                    FrameDirectionRight = false,
                    RequiredEvent = "43"
                },
                new NpcConfig
                {
                    Name = "Emily",
                    Frame = 33,
                    FrameDirectionRight = false,
                    RequiredEvent = "2123343"
                },
                new NpcConfig
                {
                    Name = "Haley",
                    Frame = 28,
                    FrameDirectionRight = true,
                    RequiredEvent = "15"
                },
                new NpcConfig
                {
                    Name = "Harvey",
                    Frame = 31,
                    FrameDirectionRight = false,
                    RequiredEvent = "528052"
                },
                new NpcConfig
                {
                    Name = "Leah",
                    Frame = 25,
                    FrameDirectionRight = true,
                    RequiredEvent = "54"
                },
                new NpcConfig
                {
                    Name = "Maru",
                    Frame = 28,
                    FrameDirectionRight = false,
                    RequiredEvent = "10"
                },
                new NpcConfig
                {
                    Name = "Penny",
                    Frame = 35,
                    FrameDirectionRight = true,
                    RequiredEvent = "38"
                },
                new NpcConfig
                {
                    Name = "Sam",
                    Frame = 36,
                    FrameDirectionRight = true,
                    RequiredEvent = "233104"
                },
                new NpcConfig
                {
                    Name = "Sebastian",
                    Frame = 40,
                    FrameDirectionRight = false,
                    RequiredEvent = "384882"
                },
                new NpcConfig
                {
                    Name = "Shane",
                    Frame = 34,
                    FrameDirectionRight = false,
                    RequiredEvent = "9581348"
                }
            };
        }
    }
}