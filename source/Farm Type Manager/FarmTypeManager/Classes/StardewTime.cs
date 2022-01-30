/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>
        /// An integer representing a valid in-game time value for Stardew Valley. Generally ranges from 600 (6:00AM) to 2600 (2:00AM) with 10-minute gaps.
        /// </summary>
        [JsonConverter(typeof(StardewTimeConverter))]
        public struct StardewTime
        {
            private int time;
            public int Time
            {
                get
                {

                    return Math.Max(time, 600); //always return 600 or higher
                }

                set
                {
                    int newTime = value;

                    if (newTime < 600) //if this is null or less than 600 (6:00AM)
                    {
                        newTime = 600;
                    }
                    else
                    {
                        if (newTime % 10 > 0) //if this isn't a multiple of 10
                        {
                            newTime = (newTime - (newTime % 10)) + 10; //round up to a multiple of 10
                        }

                        if (newTime % 100 > 50) //if the "minutes" of this time are over 50
                        {
                            newTime = ((newTime / 100) + 1) * 100; //increment to the next hour
                        }
                    }

                    time = newTime;
                }
            }

            public StardewTime(int time) : this()
            {
                Time = time;
            }

            //allows implicit conversion from int values, e.g. "StardewTime time = 600;"
            public static implicit operator StardewTime(int value)
            {
                return new StardewTime(value);
            }

            //allows implicit conversion to int values, e.g. "time + 100" == 700
            public static implicit operator int(StardewTime value)
            {
                return value.Time;
            }

            //allows correct incrementation (e.g. "time++" adds 10 minutes)
            public static StardewTime operator ++(StardewTime value)
            {
                value.Next();
                return value;
            }

            //allows correct decrementation (e.g. "time--" subtracts 10 minutes)
            public static StardewTime operator --(StardewTime value)
            {
                value.Prev();
                return value;
            }

            /// <summary>Increase the Time value by 10 minutes.</summary>
            public void Next()
            {
                int newTime = time + 10; //increment by 10

                if (newTime % 100 > 50) //if the "minutes" of this time are over 50
                {
                    newTime = ((newTime / 100) + 1) * 100; //increment to the next hour
                }

                time = newTime;
            }

            /// <summary>Decrease the Time value by 10 minutes.</summary>
            public void Prev()
            {
                int newTime = time - 10; //decrement by 10

                if (newTime < 600) //if this is null or less than 600 (6:00AM)
                {
                    newTime = 600;
                }
                else
                {
                    if (newTime % 100 > 50) //if the "minutes" of this time are over 50
                    {
                        newTime = ((newTime / 100) * 100) + 50; //decrement to 50
                    }
                }

                time = newTime;
            }
        }

        /// <summary>A custom converter used to cleanly handle StardewTime with Newtonsoft.Json.</summary>
        public class StardewTimeConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(StardewTime));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                int time = JToken.Load(reader).ToObject<int>(serializer);
                return new StardewTime(time); //read the JSON value as an integer
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                int time = (StardewTime)value;
                JToken.FromObject(time, serializer).WriteTo(writer); //write the value to JSON as an integer
            }
        }
    }
}