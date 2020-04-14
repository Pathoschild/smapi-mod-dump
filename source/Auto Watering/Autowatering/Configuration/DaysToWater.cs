using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autowatering.Configuration
{
    public class DaysToWater
    {

        /// <summary>
        /// Whether to water on Sundays or not
        /// </summary>
        public bool Sunday { get; set; } = true;
        /// <summary>
        /// Whether to water on Mondays or not
        /// </summary>
        public bool Monday { get; set; } = true;

        /// <summary>
        /// Whether to water on Tuesdays or not
        /// </summary>
        public bool Tuesday { get; set; } = true;

        /// <summary>
        /// Whether to water on Wednesdays or not
        /// </summary>
        public bool Wednesday { get; set; } = true;

        /// <summary>
        /// Whether to water on Thursdays or not
        /// </summary>
        public bool Thursday { get; set; } = true;

        /// <summary>
        /// Whether to water on Fridays or not
        /// </summary>
        public bool Friday { get; set; } = true;

        /// <summary>
        /// Whether to water on Saturdays or not
        /// </summary>
        public bool Saturday { get; set; } = true;
    }
}
