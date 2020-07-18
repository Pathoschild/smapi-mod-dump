using StardewModdingAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace SvFishingMod
{
    [DataContract]
    public class Settings
    {
        private static Settings _instance = null;
        private float _distanceFromCatchingOverride = -1;
        private int _overrideFishQuality = -1;
        private int _overrideFishType = -1;

        public static string ConfigFilePath { get; set; } = null;
        public static IModHelper HelperInstance { get; set; } = null;
        public static IMonitor MonitorInstance { get; set; } = null;

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (string.IsNullOrWhiteSpace(ConfigFilePath))
                        _instance = new Settings();
                    else
                        _instance = LoadFromFile(ConfigFilePath);
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        [DataMember] public bool AlwaysCatchDoubleFish { get; set; } = false;
        [DataMember] public bool AlwaysCatchTreasure { get; set; } = false;
        [DataMember] public bool AlwaysPerfectCatch { get; set; } = true;
        [DataMember] public bool AutoReelFish { get; set; } = true;
        [DataMember] public bool DisableMod { get; set; } = false;

        [DataMember]
        public float DistanceFromCatchingOverride
        {
            get
            {
                if (_distanceFromCatchingOverride > 1.0f)
                    return 1.0f;
                if (_distanceFromCatchingOverride < 0.0f)
                    return 0.0f;

                return _distanceFromCatchingOverride;
            }
            set
            {
                _distanceFromCatchingOverride = value;
            }
        }

        [DataMember] public int OverrideBarHeight { get; set; } = -1;
        [DataMember] public bool RemoveBiteDelay { get; set; } = false;

        [DataMember]
        public int OverrideFishQuality
        {
            get
            {
                if (_overrideFishQuality > 4)
                    return 4;
                if (_overrideFishQuality < 0)
                    return -1;

                return _overrideFishQuality;
            }
            set
            {
                _overrideFishQuality = value;
            }
        }

        [DataMember]
        public int OverrideFishType
        {
            get
            {
                if (_overrideFishType == -1)
                    return -1;
                if (_overrideFishType < 128)
                    return 128;

                return _overrideFishType;
            }
            set
            {
                _overrideFishType = value;
            }
        }

        [DataMember] public bool ReelFishCycling { get; set; } = false;

        public static Settings LoadFromFile()
        {
            if (HelperInstance == null)
                throw new NullReferenceException("No SMAPI Mod Helper defined before loading settings file.");

            return LoadFromFile(ConfigFilePath);
        }

        public static Settings LoadFromFile(string filename)
        {
            Settings output;

            try
            {
                output = HelperInstance.Data.ReadJsonFile<Settings>(ConfigFilePath);
                if (output == null)
                    output = new Settings();
                output.SaveToFile(filename);
                MonitorInstance.Log(string.Format("Settings loaded using SMAPI from {0}", filename), LogLevel.Trace);
            }
            catch (Exception ex)
            {
                if (MonitorInstance != null)
                    MonitorInstance.Log(string.Format("[SvFishingMod] Unable to load settings from specified filename {0}", filename, ex.GetType().Name, ex.Message), LogLevel.Error);
                output = new Settings(); // Load defaults
            }

            return output;
        }

        public void SaveToFile()
        {
            SaveToFile(ConfigFilePath);
        }

        public void SaveToFile(string filename)
        {
            if (HelperInstance == null)
                throw new NullReferenceException("No SMAPI Mod Helper defined before saving settings file.");

            HelperInstance.Data.WriteJsonFile<Settings>(filename, this);
        }
    }
}