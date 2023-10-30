/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static StardewValley.LocationRequest;

public delegate void Callback();

namespace StardewSurvivalProject.source.model
{
    public enum MoodLevel
    {
        MentalBreak,
        Distress,
        Sad,
        Discontent,
        Neutral,
        Content,
        Happy,
        Overjoy,
    }

    public class Mood
    {
        private double _value = 50;

        // Name, value, number of minutes to expire (-1 is never expire)
        private List<Tuple<string, double, double>> MoodElements = new List<Tuple<string, double, double>>();

        private readonly Callback _onMentalBreakCallback;

        [JsonConstructor]
        public Mood(double value, MoodLevel level)
        {
            this.Value = value;
            this.Level = level;
        }

        // for new player
        public Mood(Callback OnMentalBreakCallback)
        {
            _onMentalBreakCallback = OnMentalBreakCallback;
        }

        // for save data
        public Mood(Mood mood, Callback OnMentalBreakCallback)
        {
            this.Value = mood.Value;
            // dont really need to assign level since it will be calculated from value
            _onMentalBreakCallback = OnMentalBreakCallback;
        }

        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value > 120 ? 100 : value < -40 ? -40 : value;
                Level = GetMoodLevel(value);
            }
        }

        public MoodLevel Level = MoodLevel.Neutral;

        // Maybe the mood mechanic from Rimworld would be good here

        /** Potential penalty
         * Monotonous task (-5 to -50)
         * Eating the same food (-5 to -50)
         * Eating raw food (-10)
         * Get trash from fishing (-10)
         * NPC dislike your gift (-10)
         * Get fever (-10)
         * (From butcher mod) kill animal (-10 to 0)
        **/

        public MoodLevel GetMoodLevel(double value)
        {
            if (value < 10)
                return MoodLevel.Distress;
            if (value < 25)
                return MoodLevel.Sad;
            if (value < 40)
                return MoodLevel.Discontent;
            if (value < 50)
                return MoodLevel.Neutral;
            if (value < 65)
                return MoodLevel.Content;
            if (value < 75)
                return MoodLevel.Happy;
            else
                return MoodLevel.Overjoy;
        }

        public void CheckForMentalBreak()
        {
            var rand = new Random();
            var out_val = rand.NextDouble();
            // roll the dice every 10 minutes, 1% if discontent, 5% if sad, 20% if distress
            if (Level == MoodLevel.Discontent && out_val < 0.01
                || Level == MoodLevel.Sad && out_val < 0.05
                || Level == MoodLevel.Distress && out_val < 0.2)
            {
                Level = MoodLevel.MentalBreak;
                _onMentalBreakCallback();
            }

        }
    }
}
