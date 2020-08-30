using StardewModdingAPI;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SvFishingMod
{
    [DataContract]
    public class Settings
    {
        private static Settings _localInstance = null;
        private static Settings _remoteInstance = null;
        private static Stopwatch _remoteInstanceTimer = null;
        private float _distanceFromCatchingOverride = -1;
        private int _overrideFishQuality = -1;
        private int _overrideFishType = -1;
        private static bool _remoteSettingsSet = false;
        private static bool _isMultiplayerSession = false;
        private static bool _isServer = false;

        private Settings() { }

        public static Settings DefaultEnabled
        {
            get
            {
                Settings output = new Settings()
                {
                    DisableMod = false,
                };


                return output;
            }
        }

        public static Settings DefaultDisabled
        {
            get
            {
                Settings output = new Settings()
                {
                    DisableMod = true,
                };

                return output;
            }
        }

        public static string ConfigFilePath { get; set; } = null;
        public static IModHelper HelperInstance { get; set; } = null;
        public static IMonitor MonitorInstance { get; set; } = null;
        public static bool RemoteSettingsSet
        {
            get
            {
                return _remoteSettingsSet;
            }
            private set
            {
                if (value)
                {
                    _remoteInstanceTimer?.Stop();
                    _remoteInstanceTimer = null;
                }
                else
                {
                    _remoteInstanceTimer = new Stopwatch();
                    _remoteInstanceTimer.Start();
                }

                _remoteSettingsSet = value;
            }
        }

        public static bool IsMultiplayerSession
        {
            get
            {
                return _isMultiplayerSession;
            }
            set
            {
                MonitorInstance.Log(string.Format("IsMultiplayerSession property changed to {0}", value));
                _isMultiplayerSession = value;
            }
        }

        public static Settings Active
        {
            get
            {
                if (!IsMultiplayerSession || IsServer) return Local;

                if (Remote.EnforceMultiplayerSettings) return Remote;

                return Local;
            }
        }

        public static bool IsServer
        {
            get
            {
                return _isServer;
            }
            set
            {
                MonitorInstance.Log(string.Format("IsServer property changed to {0}", value));
                _isServer = value;
            }
        }


        public static Settings Local
        {
            get
            {
                if (_localInstance == null)
                {
                    if (string.IsNullOrWhiteSpace(ConfigFilePath))
                        _localInstance = Settings.DefaultEnabled;
                    else
                        _localInstance = LoadFromFile(ConfigFilePath);
                }

                return _localInstance;
            }
            set
            {
                _localInstance = value;
            }
        }

        public static Settings Remote
        {
            get
            {
                if (_remoteInstance == null)
                {
                    _remoteInstance = Settings.DefaultDisabled;
                    RemoteSettingsSet = false;
                }

                if (!RemoteSettingsSet)
                {
                    int timeout = 20000;
                    if (_remoteInstanceTimer.ElapsedMilliseconds > timeout)
                    {
                        MonitorInstance.Log(string.Format("Remote server settings not received after {0}ms. Enabling the usage of local settings for the local farmer.", timeout));
                        Remote = Local;
                    }
                }

                return _remoteInstance;
            }
            set
            {
                _remoteInstance = value;
                RemoteSettingsSet = value != null;
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

        [DataMember] public bool EnforceMultiplayerSettings { get; set; } = true; // Remote settings override relies on this value set to True to prevent using local settings when the server has not sent its owns

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