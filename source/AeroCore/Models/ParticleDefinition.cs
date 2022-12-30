/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Particles;
using AeroCore.Utils;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using System.Collections.Generic;

namespace AeroCore.Models
{
    public class ParticleDefinition
    {
        public string Behavior { get; set; }
        public string Skin { get; set; }
        public object BehaviorSettings { get; set; }
        public object SkinSettings { get; set; }

        public Manager Create(IParticleEmitter emitter, int count)
            => Create(Behavior, Skin, BehaviorSettings, SkinSettings, emitter, count);
        public static Manager Create(string Behavior, string Skin, object BehaviorSettings, object SkinSettings, IParticleEmitter emitter, int count)
        {
            if (!API.API.knownPartBehaviors.TryGetValue(Behavior, out var bgen))
                ModEntry.monitor.Log($"Behavior type '{Behavior}' could not be found", LogLevel.Warn);
            else if (!API.API.knownPartSkins.TryGetValue(Skin, out var sgen))
                ModEntry.monitor.Log($"Skin type '{Skin}' could not be found", LogLevel.Warn);
            else
                return new Manager(
                    count,
                    BehaviorSettings is null ? bgen() : Reflection.MapTo(bgen(), BehaviorSettings),
                    SkinSettings is null ? sgen() : Reflection.MapTo(sgen(), SkinSettings),
                    emitter
                );
            return null;
        }
    }
}
