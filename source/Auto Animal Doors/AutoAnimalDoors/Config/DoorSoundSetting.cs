/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/

using AutoAnimalDoors.StardewValleyWrapper;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace AutoAnimalDoors.Config
{
    public static class DoorSoundSettingUtils
    {
        private static Dictionary<DoorSoundSetting, string> DOOR_SOUND_SETTING_TO_NAME = new Dictionary<DoorSoundSetting, string>()
        {
            { DoorSoundSetting.ALWAYS_OFF, "Always Off" },
            { DoorSoundSetting.ALWAYS_ON, "Always On" },
            { DoorSoundSetting.ONLY_ON_FARM, "Only On Farm" }
        };

        private static Dictionary<string, DoorSoundSetting> NAME_TO_DOOR_SOUND_SETTING = DOOR_SOUND_SETTING_TO_NAME.ToDictionary(x => x.Value, x => x.Key);


        public static string Name(this DoorSoundSetting doorSoundSetting)
        {
            return DOOR_SOUND_SETTING_TO_NAME[doorSoundSetting];
        }

        public static DoorSoundSetting FromName(string doorSoundSettingName)
        {
            if (NAME_TO_DOOR_SOUND_SETTING.ContainsKey(doorSoundSettingName))
            {
                return NAME_TO_DOOR_SOUND_SETTING[doorSoundSettingName];
            }

            Logger.Instance.Log(string.Format("Unable to convert name [{0}] to Door Sound Setting. Defaulting to [Only On Farm]", doorSoundSettingName), LogLevel.Error);
            return DoorSoundSetting.ONLY_ON_FARM;
        }

        public static string[] Names
        {
            get
            {
                return DOOR_SOUND_SETTING_TO_NAME.Values.ToArray();

            }
        }
    }

    public enum DoorSoundSetting { ALWAYS_OFF, ALWAYS_ON, ONLY_ON_FARM }
}
