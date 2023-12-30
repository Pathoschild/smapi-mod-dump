/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/adverserath/QuickGlance
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Reflection;

namespace QuickGlance
{
    class ModConfig
    {
        const string DEFAULT_BINDS = "Home,LeftStick";

        private IModHelper helper;

        public KeybindList ZoomKeys { get; set; } = KeybindList.Parse(DEFAULT_BINDS);
        public float ZoomLevel { get; set; } = 0.5f;
        public bool ToggleZoom { get; set; } = false;

        internal void Reset()
        {
            ZoomKeys = KeybindList.Parse(DEFAULT_BINDS);
            ZoomLevel = .5f;
            ToggleZoom = false;
        }
        internal void Apply()
        {
            helper.WriteConfig(this);
        }
        internal void Register(GMCMAPI gmcm, IModHelper helper, IManifest manifest)
        {
            this.helper = helper;

            gmcm.Register(manifest, Reset, Apply);
            AddOption(gmcm, manifest, nameof(ZoomKeys));
            AddOption(gmcm, manifest, nameof(ZoomLevel));
            AddOption(gmcm, manifest, nameof(ToggleZoom));
        }

        private void AddOption(GMCMAPI gmcm, IManifest manifest, string field)
        {
            PropertyInfo prop = typeof(ModConfig).GetProperty(field);
            if (prop is not null && prop.CanWrite && prop.CanRead)
            {
                Delegate getter = Delegate.CreateDelegate(typeof(Func<>).MakeGenericType(prop.PropertyType), this, prop.GetMethod);
                Delegate setter = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(prop.PropertyType), this, prop.SetMethod);
                string trans() => helper.Translation.Get($"config.{prop.Name.ToLowerInvariant()}");

                switch (prop.GetValue(this))
                {
                    case bool:
                        gmcm.AddBoolOption(manifest, getter as Func<bool>, setter as Action<bool>, trans); break;
                    case float:
                        gmcm.AddNumberOption(manifest, getter as Func<float>, setter as Action<float>, trans); break;
                    case KeybindList:
                        gmcm.AddKeybindList(manifest, getter as Func<KeybindList>, setter as Action<KeybindList>, trans); break;
                }
            }
        }
    }
}
