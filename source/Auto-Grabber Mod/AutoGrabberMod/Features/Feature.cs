using System;
using System.Linq;
using AutoGrabberMod.Models;
using AutoGrabberMod.UserInterfaces;

namespace AutoGrabberMod.Features
{
    public abstract class Feature
    {
        public abstract string FeatureName { get; }
        public abstract string FeatureConfig { get; }
        public AutoGrabber Grabber { get; set; }
        public abstract int Order { get; }
        public abstract bool IsAllowed { get; }

        public object Value { get; set; }
        public abstract void Action();
        public void ConfigParse(string[] config)
        {
            if (config.Contains(FeatureConfig)) Value = true;
        }

        public string ConfigValue()
        {
            return ((bool)Value) ? $" |{FeatureConfig}|" : "";
        }

        public OptionsElement[] InterfaceElement()
        {
            return new OptionsElement[] { new OptionsCheckbox(FeatureName, (bool)Value, value => {
                Value = value;
                Grabber.Update();
            }) };
        }

        public void ActionItemAddedRemoved()
        {
            throw new NotImplementedException();
        }
    }
}
