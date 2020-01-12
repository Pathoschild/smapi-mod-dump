using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using ModSettingsTab.Framework.Components;
using ModSettingsTab.Framework.Integration;
using ModSettingsTab.Menu;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;

namespace ModSettingsTab.Framework
{
    public class Mod
    {
        public readonly List<OptionsElement> Options;
        private readonly StaticConfig _staticConfig;
        private bool _favorite;
        public IManifest Manifest { get; }

        public bool Favorite
        {
            get => _favorite;
            set
            {
                _favorite = value;
                ((OptionsHeading) Options[0]).Favorite = value;
            }
        }

        public Mod(string uniqueId, string directory, StaticConfig config)
        {
            Options = new List<OptionsElement>();
            Manifest = ModEntry.Helper.ModRegistry.Get(uniqueId).Manifest;
            _staticConfig = config;
            InitOptions(directory);
            Favorite = FavoriteData.IsFavorite(Manifest.UniqueID);
        }

        private void InitOptions(string folder)
        {
            var lang = LocalizedContentManager.CurrentLanguageCode;
            var uniqueId = Manifest.UniqueID;
            var uI9NPath = Path.Combine(folder, "settingsTab.json");
            var nI9NPath = Path.Combine(ModEntry.Helper.DirectoryPath, $"data/I9N/{uniqueId}.json");
            ModIntegrationSettings uI9N = null, nI9N = null;

            try
            {
                uI9N = File.Exists(uI9NPath)
                    ? JsonConvert.DeserializeObject<ModIntegrationSettings>(File.ReadAllText(uI9NPath))
                    : null;
                nI9N = File.Exists(nI9NPath)
                    ? JsonConvert.DeserializeObject<ModIntegrationSettings>(File.ReadAllText(nI9NPath))
                    : null;
            }
            catch (Exception e)
            {
                ModEntry.Console.Log(e.Message);
            }


            Options.Add(new OptionsHeading(uniqueId, Manifest, BaseOptionsModPage.SlotSize)
            {
                HoverText = uI9N?.Description?[lang] ?? nI9N?.Description?[lang] ?? Manifest.Description
            });

            foreach (KeyValuePair<string, JToken> param in _staticConfig)
            {
                var option = CreateOption(
                    param.Key, uniqueId, _staticConfig,
                    BaseOptionsModPage.SlotSize, uI9N, nI9N);
                if (option == null) continue;
                Options.Add(option);
            }
        }
        
        public static OptionsElement CreateOption(
            string name,
            string uniqueId,
            StaticConfig staticConfig,
            Point slotSize,
            ModIntegrationSettings uI9N = null,
            ModIntegrationSettings nI9N = null)
        {
            var lang = LocalizedContentManager.CurrentLanguageCode;
            var floatOnly = false;
            var numbersOnly = false;
            var asString = false;
            ParamType type;
            var i9NOpt = uI9N?.Config.Find(o => o.Name == name) ??
                         nI9N?.Config.Find(o => o.Name == name);

            if (i9NOpt == null || i9NOpt.Type == ParamType.None)
            {
                i9NOpt = new Param();
                type = ConvertType(staticConfig[name], out floatOnly, out numbersOnly, out asString);
                return GetOpt();
            }

            if (i9NOpt.Ignore) return null;
            type = i9NOpt.Type;
            return GetOpt();


            OptionsElement GetOpt()
            {
                switch (type)
                {
                    case ParamType.CheckBox:
                        return new OptionsCheckbox(name, uniqueId, i9NOpt.Label[lang] ?? name, staticConfig, slotSize)
                        {
                            AsString = asString,
                            HoverText = i9NOpt.Description[lang],
                            ShowTooltip = !string.IsNullOrEmpty(i9NOpt.Description[lang])
                        };
                    case ParamType.DropDown:
                        return new OptionsDropDown(name, uniqueId, i9NOpt.Label[lang] ?? name,
                            staticConfig, slotSize, i9NOpt.DropDownOptions ?? new List<string>())
                        {
                            HoverText = i9NOpt.Description[lang],
                            ShowTooltip = !string.IsNullOrEmpty(i9NOpt.Description[lang])
                        };
                    case ParamType.InputListener:
                        if (ButtonTryParse(staticConfig[name].ToString(), out var btn))
                        {
                            return new OptionsInputListener(name, uniqueId, i9NOpt.Label[lang] ?? name,
                                staticConfig, slotSize, btn)
                            {
                                HoverText = i9NOpt.Description[lang],
                                ShowTooltip = !string.IsNullOrEmpty(i9NOpt.Description[lang])
                            };
                        }
                        else
                        {
                            type = ParamType.TextBox;
                            return GetOpt();
                        }
                    case ParamType.List:
                        return new OptionsList(name, uniqueId, i9NOpt.Label[lang] ?? name, staticConfig, slotSize)
                        {
                            HoverText = i9NOpt.Description[lang],
                            ShowTooltip = !string.IsNullOrEmpty(i9NOpt.Description[lang])
                        };
                    case ParamType.PlusMinus:
                        return new OptionsPlusMinus(name, uniqueId, i9NOpt.Label[lang] ?? name, staticConfig, slotSize,
                            i9NOpt.PlusMinusOptions)
                        {
                            HoverText = i9NOpt.Description[lang],
                            ShowTooltip = !string.IsNullOrEmpty(i9NOpt.Description[lang])
                        };
                    case ParamType.Slider:
                        return new OptionsSlider(name, uniqueId, i9NOpt.Label[lang] ?? name, staticConfig, slotSize)
                        {
                            SliderMinValue = i9NOpt.SliderMinValue,
                            SliderMaxValue = i9NOpt.SliderMaxValue,
                            SliderStep = i9NOpt.SliderStep,
                            HoverText = i9NOpt.Description[lang],
                            ShowTooltip = !string.IsNullOrEmpty(i9NOpt.Description[lang])
                        };
                    case ParamType.TextBox:
                        return new OptionsTextBox(name, uniqueId, i9NOpt.Label[lang] ?? name, staticConfig, slotSize,
                            i9NOpt.TextBoxFloatOnly ?? floatOnly,
                            i9NOpt.TextBoxNumbersOnly ?? numbersOnly,
                            asString)
                        {
                            HoverText = i9NOpt.Description[lang],
                            ShowTooltip = !string.IsNullOrEmpty(i9NOpt.Description[lang])
                        };
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// defines the desired type of option
        /// </summary>
        /// <param name="t"></param>
        /// <param name="floatOnly"></param>
        /// <param name="numbersOnly"></param>
        /// <param name="asString"></param>
        /// <returns></returns>
        private static ParamType ConvertType(JToken t, out bool floatOnly, out bool numbersOnly, out bool asString)
        {
            floatOnly = false;
            numbersOnly = false;
            asString = false;
            switch (t.Type)
            {
                case JTokenType.Array:
                    return ParamType.List;
                case JTokenType.Float:
                    floatOnly = true;
                    return ParamType.TextBox;
                case JTokenType.Boolean:
                    return ParamType.CheckBox;
                case JTokenType.Integer:
                    numbersOnly = true;
                    return ParamType.TextBox;
                default:
                {
                    var str = t.ToString().Trim();
                    // check int
                    if (int.TryParse(str, out _))
                    {
                        numbersOnly = true;
                        asString = true;
                        return ParamType.TextBox;
                    }

                    // check button
                    if (ButtonTryParse(str, out _))
                    {
                        return ParamType.InputListener;
                    }

                    // check bool
                    if (str.ToLower().Equals("true") || str.ToLower().Equals("false"))
                    {
                        asString = true;
                        return ParamType.CheckBox;
                    }

                    // check float
                    if (float.TryParse(str, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-US"), out _))
                    {
                        floatOnly = true;
                        asString = true;
                        return ParamType.TextBox;
                    }

                    return ParamType.TextBox;
                }
            }
        }
        /// <summary>
        /// reads a button from a string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="btn"></param>
        /// <returns></returns>
        private static bool ButtonTryParse(string str, out SButton btn)
        {
            if (!str.Contains(",") && Enum.TryParse<SButton>(str.Trim(), true, out var result))
            {
                btn = result;
                return true;
            }

            btn = SButton.None;
            return false;
        }
    }
}