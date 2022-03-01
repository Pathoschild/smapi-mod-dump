/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Effects;
using SkillfulClothes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Configuration
{
    public class EffectLibrary
    {
        public static EffectLibrary Empty { get; } = new EffectLibrary(false);

        static EffectLibrary defaultInstance;

        public static EffectLibrary Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new EffectLibrary(true);
                }
                return defaultInstance;
            }
        }

        Dictionary<string, Type> availableEffects = new Dictionary<string, Type>();

        protected void DiscoverEffects()
        {
            var lst = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsAssignableTo(typeof(IEffect)) && !x.IsAbstract && !x.IsInterface);
            foreach (var type in lst)
            {
                if (type != typeof(EffectSet))
                {
                    availableEffects.Add(type.Name.ToLower(), type);
                }
            }
        }

        private EffectLibrary(bool discover)
        {
            if (discover)
            {
                DiscoverEffects();
            }
        }

        public bool TryGetEffectType(string effectName, out Type effectType)
        {
            effectName = effectName.ToLower();
            return availableEffects.TryGetValue(effectName, out effectType) || availableEffects.TryGetValue(effectName + "effect", out effectType);
        }

        /// <summary>
        /// Create an Instance of the effect with the given name/identifier
        /// </summary>
        /// <param name="effectName"></param>
        /// <returns></returns>
        public IEffect CreateEffectInstance(string effectName)
        {
            if (TryGetEffectType(effectName, out Type effectType))
            {
                var constr = effectType.GetConstructors().Where(x => x.GetParameters().Count() == 1 && x.GetParameters().First().ParameterType.IsAssignableTo(typeof(IEffectParameters))).FirstOrDefault();

                if (constr == null)
                {
                    // there is no constructor with parameters, us ethe parameter-less one
                    return (IEffect)Activator.CreateInstance(effectType);
                }

                // pass null as parameters to use the default parameters                
                return (IEffect)constr.Invoke(new object[] { null });
            }

            Logger.Error($"Unknown effect: {effectName}");
            return new NullEffect();
        }

        public List<String> GetAllEffectNames()
        {
            return availableEffects.Keys.ToList();
        }

        public List<Type> GetAllEffectTypes()
        {
            return availableEffects.Values.ToList();
        }
    }
}
