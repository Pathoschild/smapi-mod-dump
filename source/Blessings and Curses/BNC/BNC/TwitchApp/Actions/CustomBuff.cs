/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using System;
using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using static BNC.BuffManager;

namespace BNC.Actions
{
    class CustomBuff : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "id")]
        public string id;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "name")]
        public string Name;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "short_description")]
        public string ShortDesc = "";

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "duration")]
        public int Duration;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "farming")]
        public int Farming;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "fishing")]
        public int Fishing;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "attack")]
        public int Attack;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "defense")]
        public int Defense;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "foraging")]
        public int Foraging;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "max_stamina")]
        public int MaxStamina;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "speed")]
        public int Speed;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "luck")]
        public int Luck;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "mining")]
        public int Mining;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "magnetic_radius")]
        public int MagneticRadius;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "glowing")]
        public int Glowing = 0;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "glowing_color")]
        public String GlowingColor = "LightBlue";

        public override ActionResponse Handle()
        {
            BuffOption buff = new BuffOption(id, Name);

            //If not Default
            if (Duration != 1200 && Duration > 0 && Duration < 5000)
                buff.setDuration(Duration);

            buff.addShortDesc(ShortDesc);

            if (Attack != 0)
                buff.add_attack(Attack);

            if (Defense != 0)
                buff.add_defense(Defense);

            if (Farming != 0)
                buff.add_farming(Farming);

            if (Foraging != 0)
                buff.add_foraging(Foraging);

            if (MaxStamina != 0)
                buff.add_maxStamina(MaxStamina);

            if (Speed != 0)
                buff.add_speed(Speed);

            if (Luck != 0)
                buff.add_luck(Luck);

            if (MagneticRadius != 0)
                buff.add_magneticRadius(MagneticRadius);

            if (Mining != 0)
                buff.add_mining(Mining);

            if (Glowing == 1) {
                var prop = typeof(Color).GetProperty(GlowingColor);
                if (prop != null)
                    buff.setGlow((Color)prop.GetValue(GlowingColor));
                else
                    buff.setGlow(Color.LightBlue);
            }

            if(buff == null) {
                BNC_Core.Logger.Log($"Could not find buff type of: {id}", StardewModdingAPI.LogLevel.Error);
                return ActionResponse.Done;
            }
            else  {
                BuffManager.AddBuffToQueue(buff);
                return ActionResponse.Done;
            }
        }
    }
}
