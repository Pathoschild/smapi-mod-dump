using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using ModSettingsTabApi.Events;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;

namespace ModSettingsTab.Framework
{
    /// <summary>
    /// modification settings (config.json)
    /// </summary>
    public class StaticConfig : IEnumerable
    {
        /// <summary>
        /// mod static parameter dictionary
        /// </summary>
        private readonly Dictionary<string, JToken> _properties = new Dictionary<string, JToken>();

        /// <summary>
        /// json representation of parameters
        /// (save to config.json)
        /// </summary>
        private readonly JObject _config;

        private readonly Timer _saveTimer;

        private readonly Dictionary<string, Value> _changedValues = new Dictionary<string, Value>();

        private string _changedValue;
        private string _changedNewValue;

        private void Save()
        {
            _saveTimer.Stop();
            if (_changedValues[_changedValue].OldValue != _changedNewValue) _saveTimer.Start();
            else _changedValues.Remove(_changedValue);
        }


        public override string ToString()
        {
            return _config.ToString();
        }

        public IEnumerator GetEnumerator() => _properties.GetEnumerator();

        public StaticConfig(string path, JObject config, string uniqueId)
        {
            _config = config;

            _saveTimer = new Timer(2500.0)
            {
                Enabled = false,
                AutoReset = false
            };
            _saveTimer.Elapsed += async (t, e) =>
            {
                try
                {
                    using (var writer = File.CreateText(path))
                        await writer.WriteAsync(ToString());

                    if (ModData.Api.ApiList.ContainsKey(uniqueId))
                        ModData.NeedReload = !ModData.Api.ApiList[uniqueId].Send(_config, _changedValues) || ModData.NeedReload;
                    else
                        ModData.NeedReload = true;
                    _changedValues.Clear();
                    if (ModData.Config.ShowSavingNotify) 
                        Game1.addHUDMessage(new HUDMessage(ModEntry.I18N.Get("StaticConfig.SuccessMessage"), 2));
                }
                catch (Exception ex)
                {
                    ModEntry.Console.Log(ex.Message, LogLevel.Error);
                    Game1.showRedMessage(ModEntry.I18N.Get("StaticConfig.FailMessage"));
                }
            };
            ParseProperties(_config);
        }

        /// <summary>
        /// checks for a parameter
        /// </summary>
        /// <param name="key">
        /// JToken.Path
        /// The key is made up of property names and array indexes separated by periods, e.g. Manufacturers[0].Name.
        /// </param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// hides the parameter from the page
        /// </summary>
        /// <param name="key">
        /// JToken.Path
        /// The key is made up of property names and array indexes separated by periods, e.g. Manufacturers[0].Name.
        /// </param>
        public void Remove(string key)
        {
            _properties.Remove(key);
        }

        /// <summary>
        /// get or set a parameter by key
        /// </summary>
        /// <remarks>
        /// Automatically saves changes to config.json
        /// </remarks>
        /// <param name="key">
        /// JToken.Path
        /// The key is made up of property names and array indexes separated by periods, e.g. Manufacturers[0].Name.
        /// </param>
        public JToken this[string key]
        {
            get => !_properties.ContainsKey(key) ? null : _properties[key];
            set
            {
                if (!_properties.ContainsKey(key))
                    return;
                if (!_changedValues.ContainsKey(key))
                {
                    _changedValues.Add(key, new Value {OldValue = _properties[key].ToString()});
                }

                _config.SelectToken(key).Replace(value);
                _properties[key] = _config.SelectToken(key);
                _changedValue = key;
                _changedValues[key].NewValue = _changedNewValue = value.ToString();
                Save();
            }
        }

        private void ParseProperties(JToken obj)
        {
            switch (obj.Type)
            {
                case JTokenType.Object:
                    ((JObject) obj).Properties().ToList()
                        .ForEach(p => ParseProperties(p.Value));
                    break;
                case JTokenType.Array:
                {
                    if (!obj.Children().Any())
                    {
                        _properties.Add(obj.Path, obj);
                        break;
                    }

                    switch (obj.Children().First().Type)
                    {
                        case JTokenType.Object:
                        case JTokenType.Array:
                        case JTokenType.Boolean:
                            obj.Children().ToList().ForEach(ParseProperties);
                            break;

                        case JTokenType.String:
                        case JTokenType.Integer:
                        case JTokenType.Float:
                            _properties.Add(obj.Path, obj);
                            break;
                    }

                    break;
                }
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                    _properties.Add(obj.Path, obj);
                    break;
            }
        }
    }
}